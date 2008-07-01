using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLib;
using System.IO;

namespace Replay
{
    public partial class Replay : Form
    {
        TradeLink_Server_WM tl = new TradeLink_Server_WM(TLTypes.HISTORICALBROKER);
        Playback _playback = null;
        HistSim h = null;
        string tickfolder = Util.TLTickDir;
        static Account HISTBOOK = new Account("_HISTBOOK");
        public Replay()
        {
            InitializeComponent();
            tl.gotSrvFillRequest += new OrderDelegate(tl_gotSrvFillRequest);
            tl.PositionPriceRequest += new TradeLink_Server_WM.DecimalStringDelegate(tl_PositionPriceRequest);
            tl.PositionSizeRequest += new TradeLink_Server_WM.IntStringDelegate(tl_PositionSizeRequest);
            tl.DayHighRequest += new TradeLink_Server_WM.DecimalStringDelegate(tl_DayHighRequest);
            tl.DayLowRequest += new TradeLink_Server_WM.DecimalStringDelegate(tl_DayLowRequest);
            tl.OrderCancelRequest += new IntDelegate(tl_OrderCancelRequest);
            
            // setup our special book used to hold bids and offers from historical sources
            // (this is for determining top of book between historical sources and our own orders)
            HISTBOOK.Execute = false; // make sure our special book is never executed by simulator
            HISTBOOK.Notify = false; // don't notify 
        }

        void tl_OrderCancelRequest(long number)
        {
            if (h == null) return;
            h.SimBroker.CancelOrder(number); // send cancel request to broker
        }

        decimal tl_DayLowRequest(string s)
        {
            decimal price = 0;
            lows.TryGetValue(s, out price);
            return price;
        }

        decimal tl_DayHighRequest(string s)
        {
            decimal price = 0;
            highs.TryGetValue(s, out price);
            return price;
        }

        int tl_PositionSizeRequest(string s)
        {
            if (h.SimBroker != null)
                return h.SimBroker.GetOpenPosition(s).Size;
            return 0;
        }

        decimal tl_PositionPriceRequest(string s)
        {
            if (h.SimBroker != null)
                return h.SimBroker.GetOpenPosition(s).AvgPrice;
            return 0;
        }


        private void inputbut_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = "Choose folder containing tick or index archive files...";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                tickfolder = fd.SelectedPath;
            }

        }

        void status(string msg)
        {
            if (statusStrip1.InvokeRequired)
                statusStrip1.Invoke(new DebugDelegate(status), new object[] { msg });
            else
                statuslab.Text = msg;
        }

        private void playbut_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(tickfolder))
            {
                status("Tick folder " + tickfolder + " doesn't exist,  stopping.");
                return;
            }
            highs = new Dictionary<string, decimal>();
            lows = new Dictionary<string, decimal>();
            TickFileFilter tff = new TickFileFilter();
            tff.DateFilter(Util.ToTLDate(monthCalendar1.SelectionEnd),DateMatchType.Day|DateMatchType.Month|DateMatchType.Year);
            h = new HistSim(tickfolder, tff);
            h.GotTick += new TickDelegate(h_GotTick);
            h.GotIndex += new IndexDelegate(h_GotIndex);
            h.SimBroker.GotOrder += new OrderDelegate(SimBroker_GotOrder);
            h.SimBroker.GotFill += new FillDelegate(SimBroker_GotFill);
            h.SimBroker.GotOrderCancel += new IntDelegate(SimBroker_GotOrderCancel);
            _playback = new Playback(h);
            _playback.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_playback_RunWorkerCompleted);
            _playback.ProgressChanged+=new ProgressChangedEventHandler(_playback_ProgressChanged);
            _playback.RunWorkerAsync(new PlayBackArgs((int)trackBar1.Value/5,daystartpicker.Value));
            status("Playback started...");
            playbut.Enabled = false;
            stopbut.Enabled = true;
            trackBar1.Enabled = false;
        }

        void SimBroker_GotOrderCancel(long number)
        {
            tl.newOrderCancel(number);
        }

        void tl_gotSrvFillRequest(Order o)
        {
            // pass tradelink fill requests through to the histsim broker
            // (if histsim has been started)
            if (h != null)
            {
                if (o.time * o.date == 0)
                {
                    o.time = Util.ToTLTime(h.NextTickTime);
                    o.date = Util.ToTLDate(h.NextTickTime);
                }
                // before we send the order, get top of book for same side
                Order oldbbo = h.SimBroker.BestBidOrOffer(o.symbol,o.Side);
                oldbbo.Account = "";

                // then send the order
                h.SimBroker.sendOrder(o);

                // get the new top of book
                Order newbbo = h.SimBroker.BestBidOrOffer(o.symbol,o.Side);
                newbbo.Account = "";

                // if it's changed, notify clients
                if (oldbbo != newbbo)
                    tl.newTick(OrderToTick(newbbo));
            }
        }

        void _playback_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressbar.Value = e.ProgressPercentage;
            string time = "";
            if (h != null) time = " [" + h.NextTickTime + "] ";
            status("Playing... " +time+ e.ProgressPercentage + "%");
        }

        void _playback_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                status("Playback was canceled.");
            else if (e.Error != null)
                status("Playback stopped: " + e.Error.ToString());
            else
                status("Playback completed successfully");
            progressbar.Value = 0;
        }

        void SimBroker_GotFill(Trade t)
        {
            tl.newFill(t);
        }

        void SimBroker_GotOrder(Order o)
        {
            tl.newOrder(o);
        }

        void h_GotIndex(Index idx)
        {
            tl.newIndexTick(idx);
        }

        Dictionary<string, decimal> highs = new Dictionary<string, decimal>();
        Dictionary<string, decimal> lows = new Dictionary<string, decimal>();
        void h_GotTick(Tick t)
        {
            if (t.isTrade)
            {
                decimal price = 0;
                if (highs.TryGetValue(t.sym, out price))
                {
                    if (t.trade > price)
                        highs[t.sym] = t.trade;
                }
                else highs.Add(t.sym, t.trade);
                if (lows.TryGetValue(t.sym, out price))
                {
                    if (t.trade < price)
                        lows[t.sym] = t.trade;
                }
                else lows.Add(t.sym, t.trade);
                tl.newTick(t); // notify of the trade
            }
            else
            {   // it's a quote so we need to update the book

                // first though get the BBO from hist book to detect improvements
                Order oldbid = h.SimBroker.BestBid(t.sym);
                Order oldask = h.SimBroker.BestOffer(t.sym);

                // then update the historical book
                PlaceHistoricalOrder(t);

                // fetch the new book
                Order newbid = h.SimBroker.BestBid(t.sym);
                Order newask = h.SimBroker.BestOffer(t.sym);

                // reset accounts so equality comparisons work properly in next step
                oldbid.Account = "";
                oldask.Account = "";
                newbid.Account = "";
                newask.Account = "";

                // if there are changes, notify clients
                if (oldbid != newbid)
                    tl.newTick(OrderToTick(newbid));
                if (oldask != newask)
                    tl.newTick(OrderToTick(newask));
            }
        }

        Tick OrderToTick(Order o)
        {
            return o.isLimit ? (o.Side ? Tick.NewBid(o.symbol, o.price, o.UnSignedSize) : Tick.NewAsk(o.symbol, o.price, o.UnSignedSize)) : new Tick();
        }

        

        void PlaceHistoricalOrder(Tick t)
            // this function converts a historical quote into an order
            // and places it on a special order book replay uses to determine
            // the BBO for historical tick streams and the BBO between historical ticks
            // and the other order books
        {
            if (t.isTrade) return;

            if (t.hasAsk)
            {
                // if we already have a book for this side we can get rid of it
                foreach (uint oid in hasHistBook(t.sym, false))
                    h.SimBroker.CancelOrder((long)oid); 
                h.SimBroker.sendOrder(new SellLimit(t.sym, t.AskSize, t.ask),HISTBOOK);
            }
            if (t.hasBid)
            {
                // if we already have a book for this side we can get rid of it
                foreach (uint oid in hasHistBook(t.sym, true))
                    h.SimBroker.CancelOrder((long)oid);
                h.SimBroker.sendOrder(new BuyLimit(t.sym, t.BidSize, t.bid), HISTBOOK);
            }
            
        }

        uint[] hasHistBook(string sym, bool side)
            // this function tests whether replay's special "historical" book
            // exits for a given market symbol and side
        {
            List<uint> idxlist = new List<uint>();
            List<Order> olist = h.SimBroker.GetOrderList(HISTBOOK);
            for (int i = 0; i < olist.Count; i++)
                if ((olist[i].symbol == sym) && (olist[i].Side == side))
                    idxlist.Add(olist[i].id);
            return idxlist.ToArray();
        }

        private void stopbut_Click(object sender, EventArgs e)
        {
            _playback.CancelAsync();
            status("Cancel requested.");
            stopbut.Enabled = false;
            playbut.Enabled = true;
            trackBar1.Enabled = true;
        }


    }
}
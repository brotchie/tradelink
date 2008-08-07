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
        HistSim h = new HistSim();
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
            tl.OrderCancelRequest += new UIntDelegate(tl_OrderCancelRequest);
            tl.gotSrvAcctRequest += new TradeLink_Server_WM.StringDelegate(tl_gotSrvAcctRequest);
            tl.gotSrvAcctClosedPLRequest += new TradeLink_Server_WM.DecimalStringDelegate(tl_gotSrvAcctClosedPLRequest);
            tl.gotSrvAcctOpenPLRequest += new TradeLink_Server_WM.DecimalStringDelegate(tl_gotSrvAcctOpenPLRequest);

            h.GotTick += new TickDelegate(h_GotTick);
            h.GotIndex += new IndexDelegate(h_GotIndex);
            h.SimBroker.GotOrder += new OrderDelegate(SimBroker_GotOrder);
            h.SimBroker.GotFill += new FillDelegate(SimBroker_GotFill);
            h.SimBroker.GotOrderCancel += new Broker.OrderCancelDelegate(SimBroker_GotOrderCancel);

            status(Util.TLSIdentity());

            
            // setup our special book used to hold bids and offers from historical sources
            // (this is for determining top of book between historical sources and our own orders)
            HISTBOOK.Execute = false; // make sure our special book is never executed by simulator
            HISTBOOK.Notify = false; // don't notify 
        }

        decimal tl_gotSrvAcctOpenPLRequest(string s)
        {
            // make sure broker exists
            if (h==null) return 0;
            // prepare the account we're getting open pl for
            string acct = s=="" ? Broker.DEFAULTBOOK : s;
            // get trades from this account
            List<Trade> fills = h.SimBroker.GetTradeList(new Account(acct));
            // setup storage for positions we'll create from trades
            Dictionary<string,Position> posdict = new Dictionary<string,Position>();
            // go through every trade and populate the position
            foreach (Trade t in fills)
            {
                Position p = null;
                if (!posdict.TryGetValue(t.symbol, out p))
                    posdict.Add(t.symbol, new Position(t));
                else
                    posdict[t.symbol].Adjust(t);
            }
            // for every-non flat position, calculate the pl and add to the total
            decimal totalopenpl = 0;
            foreach (Position p in posdict.Values)
                if (!p.Flat)
                    totalopenpl += BoxMath.OpenPL(last[p.Symbol], p);
            return totalopenpl;
        }

        decimal tl_gotSrvAcctClosedPLRequest(string s)
        {
            if (h == null) return 0;
            string accts = string.Join(",",h.SimBroker.Accounts);
            if (s == "")
                return h.SimBroker.GetClosedPL(new Account(Broker.DEFAULTBOOK));
            else if (accts.Contains(s))
                return h.SimBroker.GetClosedPL(new Account(s));
            return 0;
        }



        string tl_gotSrvAcctRequest()
        {
            if (h == null) return "";
            return string.Join(",", h.SimBroker.Accounts);
        }

        void tl_OrderCancelRequest(uint number)
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
            if (!s.Contains(",") && (h.SimBroker != null))
                return h.SimBroker.GetOpenPosition(s).Size;
            else if (s.Contains(",") && (h.SimBroker != null))
            {
                string[] r = s.Split(',');
                string sym = r[0];
                string acct = r[1];
                foreach (string a in h.SimBroker.Accounts)
                    if (acct == a)
                        return h.SimBroker.GetOpenPosition(sym, new Account(acct)).Size;
            }
            return 0;
        }

        decimal tl_PositionPriceRequest(string s)
        {
            if (!s.Contains(",") && (h.SimBroker != null))
                return h.SimBroker.GetOpenPosition(s).AvgPrice;
            else if (s.Contains(",") && (h.SimBroker!=null))
            {
                string[] r = s.Split(',');
                string sym = r[0];
                string acct = r[1];
                foreach (string a in h.SimBroker.Accounts)
                    if (acct == a)
                        return h.SimBroker.GetOpenPosition(sym, new Account(acct)).AvgPrice;
            }
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
            h.FileFilter = tff;
            _playback = new Playback(h);
            _playback.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_playback_RunWorkerCompleted);
            _playback.ProgressChanged+=new ProgressChangedEventHandler(_playback_ProgressChanged);
            _playback.RunWorkerAsync(new PlayBackArgs((int)trackBar1.Value/5,daystartpicker.Value));
            status("Playback started...");
            playbut.Enabled = false;
            stopbut.Enabled = true;
            trackBar1.Enabled = false;
        }

        void SimBroker_GotOrderCancel(string sym, bool side,uint id)
        {
            // if we get an order cancel notify from the broker, pass along to our clients
            tl.newOrderCancel(id);
            // send the updated book to our clients for same side as order
            Tick book = OrderToTick(h.SimBroker.BestBidOrOffer(sym, side));
            tl.newTick(book);
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
                {
                    Tick newtick = OrderToTick(newbbo);
                    tl.newTick(newtick);
                }
            }
        }

        void _playback_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressbar.Value = e.ProgressPercentage;
            string time = (h != null) ? h.NextTickTime.ToString() : "";
            status("Playing: " +time+ " ("+e.ProgressPercentage + "%)");
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
            h.Reset();
        }

        void SimBroker_GotFill(Trade t)
        {
            tl.newFill(t,true);
        }

        void SimBroker_GotOrder(Order o)
        {
            tl.newOrder(o,true);
        }

        void h_GotIndex(Index idx)
        {
            tl.newIndexTick(idx);
        }

        Dictionary<string, decimal> highs = new Dictionary<string, decimal>();
        Dictionary<string, decimal> lows = new Dictionary<string, decimal>();
        Dictionary<string, decimal> last = new Dictionary<string, decimal>();
        void h_GotTick(Tick t)
        {
            if (t.isTrade)
            {
                decimal price = 0;
                if (last.TryGetValue(t.sym, out price))
                    last[t.sym] = t.trade;
                else last.Add(t.sym, t.trade);
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
            Tick t = new Tick(o.symbol);
            if (!o.isLimit) return t;
            t.time = o.time;
            t.date = o.date;
            t.sec = o.sec;
            if (o.Side)
            {
                t.bid = o.price;
                t.BidSize = o.UnSignedSize;
                t.be = o.Exchange;
            }
            else
            {
                t.ask = o.price;
                t.AskSize = o.UnSignedSize;
                t.oe = o.Exchange;
            }
            return t;
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
                    h.SimBroker.CancelOrder(oid); 
                Order o = new SellLimit(t.sym, t.AskSize, t.ask);
                o.date = t.date;
                o.time = t.time;
                o.sec = t.sec;
                o.Exchange = t.oe;
                h.SimBroker.sendOrder(o,HISTBOOK);
            }
            if (t.hasBid)
            {
                // if we already have a book for this side we can get rid of it
                foreach (uint oid in hasHistBook(t.sym, true))
                    h.SimBroker.CancelOrder(oid);
                Order o = new BuyLimit(t.sym, t.BidSize, t.bid);
                o.date = t.date;
                o.time = t.time;
                o.sec = t.sec; 
                o.Exchange = t.be;
                h.SimBroker.sendOrder(o, HISTBOOK);
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
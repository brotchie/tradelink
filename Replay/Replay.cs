using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using System.IO;
using TradeLink.API;

namespace Replay
{
    public partial class Replay : Form
    {
        TLServer_WM tl = new TLServer_WM(TLTypes.HISTORICALBROKER);
        Playback _playback = null;
        HistSim h = new HistSim();
        string tickfolder = Util.TLTickDir;
        static Account HISTBOOK = new Account("_HISTBOOK");
        public Replay()
        {
            InitializeComponent();
            tl.newSendOrderRequest += new OrderDelegate(tl_gotSrvFillRequest);
            tl.newDayHighRequest += new DecimalStringDelegate(tl_DayHighRequest);
            tl.newDayLowRequest += new DecimalStringDelegate(tl_DayLowRequest);
            tl.newOrderCancelRequest += new UIntDelegate(tl_OrderCancelRequest);
            tl.newAcctRequest += new StringDelegate(tl_gotSrvAcctRequest);
            tl.newPosList += new PositionArrayDelegate(tl_gotSrvPosList);
            tl.newFeatureRequest+=new MessageArrayDelegate(GetFeatures);
            h.GotTick += new TickDelegate(h_GotTick);
            h.SimBroker.GotOrder += new OrderDelegate(SimBroker_GotOrder);
            h.SimBroker.GotFill += new FillDelegate(SimBroker_GotFill);
            h.SimBroker.GotOrderCancel += new OrderCancelDelegate(SimBroker_GotOrderCancel);
            h.CacheWait = 500;
            // setup playback
            _playback = new Playback(h);
            _playback.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_playback_RunWorkerCompleted);
            _playback.ProgressChanged += new ProgressChangedEventHandler(_playback_ProgressChanged);


            status(Util.TLSIdentity());

            
            // setup our special book used to hold bids and offers from historical sources
            // (this is for determining top of book between historical sources and our own orders)
            HISTBOOK.Execute = false; // make sure our special book is never executed by simulator
            HISTBOOK.Notify = false; // don't notify 
        }

        MessageTypes[] GetFeatures()
        {
            List<MessageTypes> f = new List<MessageTypes>();

            f.Add(MessageTypes.ACCOUNTREQUEST);
            f.Add(MessageTypes.ACCOUNTRESPONSE);
            f.Add(MessageTypes.EXECUTENOTIFY);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.ORDERNOTIFY);
            f.Add(MessageTypes.REGISTERCLIENT);
            f.Add(MessageTypes.REGISTERSTOCK);
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.POSITIONREQUEST);
            f.Add(MessageTypes.POSITIONRESPONSE);
            f.Add(MessageTypes.FEATUREREQUEST);
            f.Add(MessageTypes.FEATURERESPONSE);
            return f.ToArray();
        }

        Position[] tl_gotSrvPosList(string account)
        {
            if (h==null) return new PositionImpl[0];
            List<Trade> tlist = h.SimBroker.GetTradeList(new Account(account));
            List<Position> plist = new List<Position>();
            List<string> slist = new List<string>();
            foreach (TradeImpl t in tlist)
                if (!slist.Contains(t.symbol))
                    slist.Add(t.symbol);
            foreach (string sym in slist)
                plist.Add(h.SimBroker.GetOpenPosition(sym));
            return plist.ToArray();
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
            Dictionary<string,PositionImpl> posdict = new Dictionary<string,PositionImpl>();
            // go through every trade and populate the position
            foreach (Trade t in fills)
            {
                PositionImpl p = null;
                if (!posdict.TryGetValue(t.symbol, out p))
                    posdict.Add(t.symbol, new PositionImpl(t));
                else
                    posdict[t.symbol].Adjust(t);
            }
            // for every-non flat position, calculate the pl and add to the total
            decimal totalopenpl = 0;
            foreach (Position p in posdict.Values)
                if (!p.isFlat)
                    totalopenpl += Calc.OpenPL(last[p.Symbol], p);
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
                // verify user's tick folder exists
                if (!Directory.Exists(fd.SelectedPath))
                {
                    status("Tick folder " + tickfolder + " doesn't exist,  stopping.");
                    return;
                }
                tickfolder = fd.SelectedPath;
                // set the user's tick folder
                h.Folder = tickfolder;
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
            status("preparing simulation");
            // clear highs and lows
            highs = new Dictionary<string, decimal>();
            lows = new Dictionary<string, decimal>();
            // start playback
            _playback.RunWorkerAsync(new PlayBackArgs((int)trackBar1.Value/5,Util.DT2FT(daystartpicker.Value)));
            // notify user
            status("Playback started...");
            // update user interface options
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
                    o.time = lasttime == 0 ? (int)h.NextTickTime : (int)lasttime;
                    o.date = lastdate;
                }
                // before we send the order, get top of book for same side
                Order oldbbo = h.SimBroker.BestBidOrOffer(o.symbol,o.side);
                oldbbo.Account = "";

                // then send the order
                h.SimBroker.sendOrder(o);

                // get the new top of book
                Order newbbo = h.SimBroker.BestBidOrOffer(o.symbol,o.side);
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
            progressbar.Value = (e.ProgressPercentage < 101) && (e.ProgressPercentage>=0) ? e.ProgressPercentage : 0;
            int ctime = (int)(h.NextTickTime % 1000000) / 100;
            string time = (h != null) ? string.Format("{0:####:##}",ctime) : "";
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

        Dictionary<string, decimal> highs = new Dictionary<string, decimal>();
        Dictionary<string, decimal> lows = new Dictionary<string, decimal>();
        Dictionary<string, decimal> last = new Dictionary<string, decimal>();
        long lasttime = 0;
        int lastdate = 0;
        void h_GotTick(Tick t)
        {
            lasttime = t.time;
            lastdate = t.date;
            if (t.isTrade)
            {
                decimal price = 0;
                if (last.TryGetValue(t.symbol, out price))
                    last[t.symbol] = t.trade;
                else last.Add(t.symbol, t.trade);
                if (highs.TryGetValue(t.symbol, out price))
                {
                    if (t.trade > price)
                        highs[t.symbol] = t.trade;
                }
                else highs.Add(t.symbol, t.trade);
                if (lows.TryGetValue(t.symbol, out price))
                {
                    if (t.trade < price)
                        lows[t.symbol] = t.trade;
                }
                else lows.Add(t.symbol, t.trade);
                tl.newTick(t); // notify of the trade
            }
            else
            {   // it's a quote so we need to update the book

                // first though get the BBO from hist book to detect improvements
                Order oldbid = h.SimBroker.BestBid(t.symbol);
                Order oldask = h.SimBroker.BestOffer(t.symbol);

                // then update the historical book
                PlaceHistoricalOrder(t);

                // fetch the new book
                Order newbid = h.SimBroker.BestBid(t.symbol);
                Order newask = h.SimBroker.BestOffer(t.symbol);

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
            Tick t = new TickImpl(o.symbol);
            if (!o.isLimit) return t;
            t.time = o.time;
            t.date = o.date;
            if (o.side)
            {
                t.bid = o.price;
                t.BidSize = o.UnsignedSize;
                t.be = o.Exchange;
            }
            else
            {
                t.ask = o.price;
                t.AskSize = o.UnsignedSize;
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
                foreach (uint oid in hasHistBook(t.symbol, false))
                    h.SimBroker.CancelOrder(oid); 
                OrderImpl o = new SellLimit(t.symbol, t.AskSize, t.ask);
                o.date = t.date;
                o.time = t.time;
                o.Exchange = t.oe;
                h.SimBroker.sendOrder(o,HISTBOOK);
            }
            if (t.hasBid)
            {
                // if we already have a book for this side we can get rid of it
                foreach (uint oid in hasHistBook(t.symbol, true))
                    h.SimBroker.CancelOrder(oid);
                OrderImpl o = new BuyLimit(t.symbol, t.BidSize, t.bid);
                o.date = t.date;
                o.time = t.time;
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
                if ((olist[i].symbol == sym) && (olist[i].side == side))
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

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            // create a new filter
            TickFileFilter tff = new TickFileFilter();
            // populate the filter from user's calendar
            tff.DateFilter(Util.ToTLDate(monthCalendar1.SelectionEnd), DateMatchType.Day | DateMatchType.Month | DateMatchType.Year);
            // set the filter on the simulator
            h.FileFilter = tff;
        }


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TradeLink.API;
using TradeLink.AppKit;
using TradeLink.Common;

namespace Replay
{
    public partial class Replay : AppTracker
    {
        TLServer tl;

        Playback _playback = new Playback();
        HistSim h;
        string tickfolder = Util.TLTickDir;
        static Account HISTBOOK = new Account("_HISTBOOK");
        public const string PROGRAM = "Replay";
        public DebugWindow _dw = new DebugWindow();
        Broker SimBroker = new Broker();
        public Replay()
        {
            

            if (Properties.Settings.Default.TLClientAddress== string.Empty)
                tl = new TLServer_WM() ;
            else
                tl = new TLServer_IP(Properties.Settings.Default.TLClientAddress, Properties.Settings.Default.TLClientPort);
            tl.VerboseDebugging = Properties.Settings.Default.VerboseDebugging;
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            tl.newProviderName = Providers.TradeLink;
            tl.newSendOrderRequest += new OrderDelegateStatus(tl_gotSrvFillRequest);
            tl.newOrderCancelRequest += new LongDelegate(tl_OrderCancelRequest);
            tl.newAcctRequest += new StringDelegate(tl_gotSrvAcctRequest);
            tl.newPosList += new PositionArrayDelegate(tl_gotSrvPosList);
            tl.newFeatureRequest+=new MessageArrayDelegate(GetFeatures);
            tl.newUnknownRequest += new UnknownMessageDelegate(tl_newUnknownRequest);
            tl.newRegisterSymbols += new SymbolRegisterDel(tl_newRegisterSymbols);
            
            SimBroker.UseBidAskFills = Properties.Settings.Default.UseBidAskFills;
            SimBroker.GotOrder += new OrderDelegate(SimBroker_GotOrder);
            SimBroker.GotFill += new FillDelegate(SimBroker_GotFill);
            SimBroker.GotOrderCancel += new OrderCancelDelegate(SimBroker_GotOrderCancel);


            status(Util.TLSIdentity());

            
            // setup our special book used to hold bids and offers from historical sources
            // (this is for determining top of book between historical sources and our own orders)
            HISTBOOK.Execute = false; // make sure our special book is never executed by simulator
            HISTBOOK.Notify = false; // don't notify 
            FormClosing += new FormClosingEventHandler(Replay_FormClosing);
        }

        void Replay_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        void tl_newRegisterSymbols(string client, string symbols)
        {
            v("received from: " + client + " request for symbols: " + symbols);
        }

        bool _noverb = !Properties.Settings.Default.VerboseDebugging;

        void v(string msg)
        {
            if (_noverb)
                return;
            debug(msg);
        }

        long tl_newUnknownRequest(MessageTypes t, string msg)
        {
            v("message: " + t.ToString() + " data: " + msg);
            switch (t)
            {
                case MessageTypes.DOMREQUEST:
                    {
                        string client = string.Empty;
                        int depth = 0;
                        if (Book.ParseDOMRequest(msg, ref depth, ref client))
                        {
                            debug("depth set to: " + depth + " by client: " + client);
                            tickdepth = depth;
                        }
                        break;
                    }
                case MessageTypes.DAYHIGH:
                    {
                        decimal price = 0;
                        highs.TryGetValue(msg, out price);
                        return WMUtil.pack(price);
                    }
                case MessageTypes.DAYLOW:
                    {
                        decimal price = 0;
                        lows.TryGetValue(msg, out price);
                        return WMUtil.pack(price);
                    }
            }
            return (long)MessageTypes.UNKNOWN_MESSAGE;
        }

        int tickdepth = 0;


        MessageTypes[] GetFeatures()
        {
            v("got feature request.");
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.SIMTRADING);
            f.Add(MessageTypes.HISTORICALDATA);
            f.Add(MessageTypes.HISTORICALTRADING);
            f.Add(MessageTypes.ACCOUNTREQUEST);
            f.Add(MessageTypes.ACCOUNTRESPONSE);
            f.Add(MessageTypes.EXECUTENOTIFY);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.ORDERNOTIFY);
            f.Add(MessageTypes.REGISTERCLIENT);
            f.Add(MessageTypes.REGISTERSTOCK);
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.SENDORDERLIMIT);
            f.Add(MessageTypes.SENDORDERMARKET);
            f.Add(MessageTypes.SENDORDERSTOP);
            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.POSITIONREQUEST);
            f.Add(MessageTypes.POSITIONRESPONSE);
            f.Add(MessageTypes.FEATUREREQUEST);
            f.Add(MessageTypes.FEATURERESPONSE);
            f.Add(MessageTypes.DAYHIGH);
            f.Add(MessageTypes.DAYLOW);
            f.Add(MessageTypes.DOMREQUEST);
            f.Add(MessageTypes.DOMRESPONSE);
            return f.ToArray();
        }

        Position[] tl_gotSrvPosList(string account)
        {
            if (h==null) return new PositionImpl[0];
            List<Trade> tlist = SimBroker.GetTradeList(new Account(account));
            List<Position> plist = new List<Position>();
            List<string> slist = new List<string>();
            foreach (TradeImpl t in tlist)
                if (!slist.Contains(t.symbol))
                    slist.Add(t.symbol);
            foreach (string sym in slist)
                plist.Add(SimBroker.GetOpenPosition(sym));
            v("received position list request and returned: " + plist.Count + " positions.");
            return plist.ToArray();
        }

        decimal tl_gotSrvAcctOpenPLRequest(string s)
        {
            // make sure broker exists
            if (h==null) return 0;
            // prepare the account we're getting open pl for
            string acct = s=="" ? Broker.DEFAULTBOOK : s;
            // get trades from this account
            List<Trade> fills = SimBroker.GetTradeList(new Account(acct));
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
            v("received openpl request and returned: " + totalopenpl);
            return totalopenpl;
        }

        decimal tl_gotSrvAcctClosedPLRequest(string s)
        {
            if (h == null) return 0;
            string accts = string.Join(",",SimBroker.Accounts);
            v("received closed pl request for: "+s);
            if (s == "")
                return SimBroker.GetClosedPL(new Account(Broker.DEFAULTBOOK));
            else if (accts.Contains(s))
                return SimBroker.GetClosedPL(new Account(s));
            return 0;
        }



        string tl_gotSrvAcctRequest()
        {
            if (h == null) return "";
            v("received account request");
            return string.Join(",", SimBroker.Accounts);
        }

        void tl_OrderCancelRequest(long number)
        {
            if (h == null) return;
            v("received cancel order request for id: " + number);
            SimBroker.CancelOrder(number); // send cancel request to broker
        }


        int tl_PositionSizeRequest(string s)
        {
            if (!s.Contains(",") && (SimBroker != null))
                return SimBroker.GetOpenPosition(s).Size;
            else if (s.Contains(",") && (SimBroker != null))
            {
                string[] r = s.Split(',');
                string sym = r[0];
                string acct = r[1];
                foreach (string a in SimBroker.Accounts)
                    if (acct == a)
                        return SimBroker.GetOpenPosition(sym, new Account(acct)).Size;
            }
            return 0;
        }

        decimal tl_PositionPriceRequest(string s)
        {
            if (!s.Contains(",") && (SimBroker != null))
                return SimBroker.GetOpenPosition(s).AvgPrice;
            else if (s.Contains(",") && (SimBroker!=null))
            {
                string[] r = s.Split(',');
                string sym = r[0];
                string acct = r[1];
                foreach (string a in SimBroker.Accounts)
                    if (acct == a)
                        return SimBroker.GetOpenPosition(sym, new Account(acct)).AvgPrice;
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
            }

        }

        void status(string msg)
        {
            if (statusStrip1.InvokeRequired)
                statusStrip1.Invoke(new DebugDelegate(status), new object[] { msg });
            else
                statuslab.Text = msg;
            debug(msg);
        }

        void debug(string msg) { _dw.GotDebug(msg); }

        private void playbut_Click(object sender, EventArgs e)
        {
            status("preparing simulation");
            if (_playback.IsBusy)
            {
                status("simulation already in progress");
                return;
            }

            // setup simulation (portfolio realistic)
            h = new MultiSimImpl(tickfolder,FileFilter);
            // bind events
            h.GotTick += new TickDelegate(h_GotTick);
            h.GotDebug += new DebugDelegate(_dw.GotDebug);
            // setup playback
            _playback = new Playback(h);
            _playback.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_playback_RunWorkerCompleted);
            _playback.ProgressChanged += new ProgressChangedEventHandler(_playback_ProgressChanged);


            // clear highs and lows
            highs = new Dictionary<string, decimal>();
            lows = new Dictionary<string, decimal>();
            // start playback
            _playback.RunWorkerAsync(new PlayBackArgs((int)trackBar1.Value/5));
            // notify user
            status("Playback started...");
            // update user interface options
            playbut.Enabled = false;
            stopbut.Enabled = true;
            trackBar1.Enabled = false;
        }

        void SimBroker_GotOrderCancel(string sym, bool side,long id)
        {
            // if we get an order cancel notify from the broker, pass along to our clients
            tl.newCancel(id);
            // send the updated book to our clients for same side as order
            Tick book = OrderToTick(SimBroker.BestBidOrOffer(sym, side));
            tl.newTick(book);
        }

        

        long tl_gotSrvFillRequest(Order o)
        {
            long err = 0;
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
                Order oldbbo = SimBroker.BestBidOrOffer(o.symbol,o.side);
                oldbbo.Account = "";

                // then send the order
                err = SimBroker.SendOrderStatus(o);

                // get the new top of book
                Order newbbo = SimBroker.BestBidOrOffer(o.symbol,o.side);
                newbbo.Account = "";

                // if it's changed, notify clients
                if (oldbbo != newbbo)
                {
                    Tick newtick = OrderToTick(newbbo);
                    tl.newTick(newtick);
                }
            }
            return err;
        }

        void _playback_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // if application is closing, ignore
            if (progressbar == null) return;
            // save some UI processing
            if (e.ProgressPercentage <= progressbar.Value) return;
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
            {
                status("Playback stopped, see messages. ");
                debug("Playback stopped: " + e.Error.ToString());
                CrashReport.Report(PROGRAM, string.Empty, string.Empty, _dw.Content,e.Error, null, false);
            }
            else
                status("Playback completed successfully");
            stopbut.Enabled = false;
            playbut.Enabled = true;
            trackBar1.Enabled = true;
            progressbar.Value = 0;
            h.Reset();
        }

        void SimBroker_GotFill(Trade t)
        {
            tl.newFill(t);
        }

        void SimBroker_GotOrder(Order o)
        {
            tl.newOrder(o);
        }

        Dictionary<string, decimal> highs = new Dictionary<string, decimal>();
        Dictionary<string, decimal> lows = new Dictionary<string, decimal>();
        Dictionary<string, decimal> last = new Dictionary<string, decimal>();
        long lasttime = 0;
        int lastdate = 0;
        void h_GotTick(Tick t)
        {
            // execute pending orders
            SimBroker.Execute(t);
            // only process requested depth
            if (t.depth > tickdepth) return;
            if (tickdepth == 0)
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
                    Order oldbid = SimBroker.BestBid(t.symbol);
                    Order oldask = SimBroker.BestOffer(t.symbol);

                    // then update the historical book
                    PlaceHistoricalOrder(t);

                    // fetch the new book
                    Order newbid = SimBroker.BestBid(t.symbol);
                    Order newask = SimBroker.BestOffer(t.symbol);

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
            else
                tl.newTick(t);
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
            if (t.depth != 0) return;

            if (t.hasAsk)
            {
                // if we already have a book for this side we can get rid of it
                foreach (long oid in hasHistBook(t.symbol, false))
                    SimBroker.CancelOrder(oid); 
                OrderImpl o = new SellLimit(t.symbol, t.AskSize, t.ask);
                o.date = t.date;
                o.time = t.time;
                o.Exchange = t.oe;
                SimBroker.SendOrderAccount(o,HISTBOOK);
            }
            if (t.hasBid)
            {
                // if we already have a book for this side we can get rid of it
                foreach (long oid in hasHistBook(t.symbol, true))
                    SimBroker.CancelOrder(oid);
                OrderImpl o = new BuyLimit(t.symbol, t.BidSize, t.bid);
                o.date = t.date;
                o.time = t.time;
                o.Exchange = t.be;
                SimBroker.SendOrderAccount(o, HISTBOOK);
            }
            
        }

        long[] hasHistBook(string sym, bool side)
            // this function tests whether replay's special "historical" book
            // exits for a given market symbol and side
        {
            List<long> idxlist = new List<long>();
            List<Order> olist = SimBroker.GetOrderList(HISTBOOK);
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

        static TickFileFilter getfilterdate(int date)
        {
                        // create a new filter
            TickFileFilter tff = new TickFileFilter();
            // we dont' select any symbols, so just playback whatever we find on this day
            tff.isSymbolDateMatchUnion = true;
            // populate the filter from user's calendar
            tff.DateFilter(date, DateMatchType.Day | DateMatchType.Month | DateMatchType.Year);
            return tff;

        }

        TickFileFilter FileFilter = getfilterdate(Util.ToTLDate());

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            // set the filter on the simulator
            FileFilter = getfilterdate(Util.ToTLDate(monthCalendar1.SelectionEnd));
        }

        private void _msg_Click(object sender, EventArgs e)
        {
            _dw.Toggle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CrashReport.Report(PROGRAM, string.Empty, string.Empty, _dw.Content, null, null, false);
        }

        private void verbtog_Click(object sender, EventArgs e)
        {
            bool org = tl.VerboseDebugging;
            tl.VerboseDebugging = !org;
            _noverb = !_noverb;
            Properties.Settings.Default.VerboseDebugging = !org;
            debug("Verbose debugging: " + (tl.VerboseDebugging ? "ON" : "OFF"));
        }


    }
}

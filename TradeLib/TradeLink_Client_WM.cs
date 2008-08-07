using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace TradeLib
{
    public class TradeLink_Client_WM : Form , TradeLinkClient
    {

        // clients that want notifications for subscribed stocks can override these methods
        /// <summary>
        /// Occurs when TradeLink receives any type of message [got message].
        /// </summary>
        public event TickDelegate gotTick;
        public event FillDelegate gotFill;
        public event IndexDelegate gotIndexTick;
        public event OrderDelegate gotOrder;
        public event DebugDelegate gotAccounts;
        public event UIntDelegate gotOrderCancel;
        public event TL2MsgDelegate gotSupportedFeatures;

        public TradeLink_Client_WM() : this("TradeLinkClient",true) { }

        public TradeLink_Client_WM(string clientname, bool showarmingonmissingserver)
            : base()
        {
            this.Text = WMUtil.GetUniqueWindow(clientname);
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
            this.Mode(this.TLFound(), false, showarmingonmissingserver);
        }

        string _servername = WMUtil.SIMWINDOW;

        /// <summary>
        /// Gets or sets the other side of the link.  Him is a string that indicates the window name of the other guy.
        /// </summary>
        /// <value>His windowname.</value>
        public string Him
        {
            get
            {
                return _servername;
            }
            set
            {
                _servername = value;
            }
        }

        /// <summary>
        /// Gets or sets my handle of the parent application or form.
        /// </summary>
        /// <value>Me H.</value>
        public IntPtr MeH { get { return this.Handle; } }

        /// <summary>
        /// Sets the preferred communication channel of the link, if multiple channels are avaialble.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public bool Mode(TLTypes mode) { return Mode(mode, false, true); }
        public bool Mode(TLTypes mode, bool showarning) { return Mode(mode, false, showarning); }
        public bool Mode(TLTypes mode, bool throwexceptions, bool showwarning)
        {
            bool HandleExceptions = !throwexceptions;
            LinkType = TLTypes.NONE; // reset before changing link mode
            switch (mode)
            {
                case TLTypes.LIVEBROKER:
                    if (HandleExceptions)
                    {
                        try
                        {
                            GoLive();
                            return true;
                        }
                        catch (TLServerNotFound)
                        {

                            if (showwarning)
                                System.Windows.Forms.MessageBox.Show("No Live broker instance was found.  Make sure broker application + TradeLink server is running.", "TradeLink server not found");
                            return false;
                        }
                    }
                    else GoLive();
                    break;
                case TLTypes.SIMBROKER:
                    if (HandleExceptions)
                    {
                        try
                        {

                            GoSim();
                            return true;
                        }
                        catch (TLServerNotFound)
                        {
                            if (showwarning)
                                System.Windows.Forms.MessageBox.Show("No simulation broker instance was found.  Make sure broker application + TradeLink server is running.", "TradeLink server not found");
                            return false;
                        }
                    }
                    else GoSim();
                    return true;
                    break;
                case TLTypes.HISTORICALBROKER:
                    if (HandleExceptions)
                    {
                        try
                        {
                            GoHist();
                            return true;
                        }
                        catch (TLServerNotFound)
                        {
                            if (showwarning)
                                System.Windows.Forms.MessageBox.Show("No historical broker instance found.  Make sure Replay Server is running.");
                            return false;
                        }
                    }
                    else GoHist();
                    return true;
                    break;
                case TLTypes.TESTBROKER:
                    if (HandleExceptions)
                    {
                        try
                        {
                            GoTest();
                            return true;
                        }
                        catch (TLServerNotFound)
                        {
                            if (showwarning)
                                System.Windows.Forms.MessageBox.Show("No test broker instance found.  Make sure you started a TradeLink_Server object with TLType.TEST.");
                            return false;
                        }
                    }
                    else GoTest();
                    return true;
                    break;
                default:
                    if (showwarning) 
                        System.Windows.Forms.MessageBox.Show("No valid broker instance was found.  Make sure broker application + TradeLink server is running.", "TradeLink server not found");
                    break;
            }
            return false;
        }

        public TLTypes LinkType = TLTypes.NONE;

        /// <summary>
        /// Makes TL client use Broker LIVE server (Broker must be logged in and TradeLink loaded)
        /// </summary>
        public void GoLive() { Disconnect(); himh = WMUtil.HisHandle(WMUtil.LIVEWINDOW); LinkType = TLTypes.LIVEBROKER; Register(); }

        /// <summary>
        /// Makes TL client use Broker Simulation mode (Broker must be logged in and TradeLink loaded)
        /// </summary>
        public void GoSim() { Disconnect(); himh = WMUtil.HisHandle(WMUtil.SIMWINDOW); LinkType = TLTypes.SIMBROKER;  Register(); }

        /// <summary>
        /// Attemptions connection to TL Replay Server
        /// </summary>
        public void GoHist() { Disconnect(); himh = WMUtil.HisHandle(WMUtil.REPLAYWINDOW); LinkType = TLTypes.HISTORICALBROKER; Register(); }

        /// <summary>
        /// Used for testing the TL-BROKER api (programmatically)
        /// </summary>
        public void GoTest() { Disconnect(); himh = WMUtil.HisHandle(WMUtil.TESTWINDOW); LinkType = TLTypes.TESTBROKER; Register(); }
        IntPtr himh = IntPtr.Zero;
        protected long TLSend(TL2 type) { return TLSend(type, ""); }
        protected long TLSend(TL2 type, string m)
        {
            if (himh == IntPtr.Zero) throw new TLServerNotFound();
            long res = WMUtil.SendMsg(m, himh, Handle, (int)type);
            return res;
        }
        /// <summary>
        /// Sends the order.
        /// </summary>
        /// <param name="o">The oorder</param>
        /// <returns>Zero if succeeded, Broker error code otherwise.</returns>
        public int SendOrder(Order o)
        {
            if (o == null) return (int)TL2.GOTNULLORDER;
            if (!o.isValid) return (int)TL2.OK;
            string m = o.Serialize();
            return (int)TLSend(TL2.SENDORDER, m);
        }

        public void RequestFeatures() { TLSend(TL2.FEATUREREQUEST,Text); }

        Dictionary<string, decimal> chighs = new Dictionary<string, decimal>();
        Dictionary<string, decimal> clows = new Dictionary<string, decimal>();
        Dictionary<string, Position> cpos = new Dictionary<string, Position>();

        /// <summary>
        /// Today's high
        /// </summary>
        /// <param name="sym">The symbol.</param>
        /// <returns></returns>
        public decimal DayHigh(string sym) { return WMUtil.unpack(TLSend(TL2.NDAYHIGH, sym)); }
        public decimal FastHigh(string sym)
        {
            try
            {
                return chighs[sym];
            }
            catch (KeyNotFoundException)
            {
                decimal high = DayHigh(sym);
                if (high != 0) chighs[sym] = high;
                return high;
            }
        }
        /// <summary>
        /// Today's low
        /// </summary>
        /// <param name="sym">The symbol</param>
        /// <returns></returns>
        public decimal DayLow(string sym) { return WMUtil.unpack(TLSend(TL2.NDAYLOW, sym)); }
        public decimal FastLow(string sym)
        {
            try
            {
                return clows[sym];
            }
            catch (KeyNotFoundException)
            {
                decimal low = DayLow(sym);
                if (low != 0) clows[sym] = low;
                return low;
            }
        }

        public decimal AccountOpenPL() { return WMUtil.unpack(TLSend(TL2.ACCOUNTOPENPL, "")); }
        public decimal AccountOpenPL(string acct) { return ((acct=="") || (acct==null)) ? 0 : WMUtil.unpack(TLSend(TL2.ACCOUNTOPENPL, acct)); }
        public decimal AccountClosedPL() { return WMUtil.unpack(TLSend(TL2.ACCOUNTCLOSEDPL, "")); }
        public decimal AccountClosedPL(string acct) { return ((acct == "") || (acct == null)) ? 0 : WMUtil.unpack(TLSend(TL2.ACCOUNTCLOSEDPL, acct)); }
        /// <summary>
        /// Today's closing price (zero if still open)
        /// </summary>
        /// <param name="sym">The symbol</param>
        /// <returns></returns>
        public decimal DayClose(string sym) { return WMUtil.unpack(TLSend(TL2.CLOSEPRICE, sym)); }
        /// <summary>
        /// yesterday's closing price (day)
        /// </summary>
        /// <param name="sym">the symbol</param>
        /// <returns></returns>
        public decimal YestClose(string sym) { return WMUtil.unpack(TLSend(TL2.YESTCLOSE, sym)); }
        /// <summary>
        /// Gets opening price for this day
        /// </summary>
        /// <param name="sym">The symbol</param>
        /// <returns>decimal</returns>
        public decimal DayOpen(string sym) { return WMUtil.unpack(TLSend(TL2.OPENPRICE, sym)); }
        public int PosSize(string sym, string account) { return ((account == "") || (account == null)) ? 0 : (int)TLSend(TL2.GETSIZE, sym + "," + account); }
        /// <summary>
        /// Gets position size
        /// </summary>
        /// <param name="sym">The symbol</param>
        /// <returns>signed integer representing position size in shares</returns>
        public int PosSize(string sym) { return (int)TLSend(TL2.GETSIZE, sym); }
        public decimal AvgPrice(string sym, string account) { return ((account == "") || (account == null)) ? 0 : WMUtil.unpack(TLSend(TL2.AVGPRICE, sym + "," + account)); }
        /// <summary>
        /// Returns average price for a position
        /// </summary>
        /// <param name="sym">The symbol</param>
        /// <returns>decimal representing average price</returns>
        public decimal AvgPrice(string sym) { return WMUtil.unpack(TLSend(TL2.AVGPRICE, sym)); }
        /// <summary>
        /// Request an order be canceled
        /// </summary>
        /// <param name="orderid">the id of the order being canceled</param>
        public void CancelOrder(Int64 orderid) { TLSend(TL2.ORDERCANCELREQUEST, orderid.ToString()); }

        /// <summary>
        /// Send an account request, response is returned via the gotAccounts event.
        /// </summary>
        /// <returns>error code, and list of accounts via the gotAccounts event.</returns>
        /// 
        public int RequestAccounts() { return (int)TLSend(TL2.ACCOUNTREQUEST, Text); }



        public Brokers BrokerName 
        { 
            get 
            { 
                long res = TLSend(TL2.BROKERNAME);
                return (Brokers)res;
            } 
        }

        public Position FastPos(string sym)
        {
            try
            {
                return cpos[sym];
            }
            catch (KeyNotFoundException)
            {
                Position p = new Position(sym, AvgPrice(sym), PosSize(sym));
                cpos.Add(sym, p);
                return p;
            }
        }

        public void Disconnect()
        {
            try
            {
                TLSend(TL2.CLEARCLIENT, Text);
            }
            catch (TLServerNotFound) { }
        }

        public void Register()
        {
            TLSend(TL2.REGISTERCLIENT, Text);
        }

        public void Subscribe(MarketBasket mb)
        {
            TLSend(TL2.REGISTERSTOCK, Text + "+" + mb.ToString());
        }

        public void RegIndex(IndexBasket ib)
        {
            TLSend(TL2.REGISTERINDEX, Text + "+" + ib.ToString());
        }

        public void Unsubscribe()
        {
            TLSend(TL2.CLEARSTOCKS, Text);
        }

        public int HeartBeat()
        {
            return (int)TLSend(TL2.HEARTBEAT, Text);
        }



        public TLTypes TLFound()
        {
            TLTypes f = TLTypes.NONE;
            if (WMUtil.Found(WMUtil.SIMWINDOW)) f |= TLTypes.SIMBROKER;
            if (WMUtil.Found(WMUtil.LIVEWINDOW)) f |= TLTypes.LIVEBROKER;
            if (WMUtil.Found(WMUtil.REPLAYWINDOW)) f |= TLTypes.HISTORICALBROKER;
            return f;
        }


        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            long result = 0;
            TradeLinkMessage tlm = WMUtil.ToTradeLinkMessage(ref m);
            if (tlm == null)// if it's not a WM_COPYDATA message 
            {
                base.WndProc(ref m); // let form process it
                return; // we're done
            }

            string msg = tlm.body;
            string[] r = msg.Split(',');
            switch (tlm.type)
            {
                case TL2.ORDERCANCELRESPONSE:
                    if (gotOrderCancel != null) gotOrderCancel(Convert.ToUInt32(msg));
                    break;
                case TL2.TICKNOTIFY:
                    if (Index.isIdx(r[(int)TickField.symbol]))
                    {
                        // we got an index update
                        Index i = Index.Deserialize(msg);
                        if (gotIndexTick != null) gotIndexTick(i);
                        break;
                    }
                    Tick t = Tick.Deserialize(msg);
                    if (t.isTrade)
                    {
                        try
                        {
                            if (t.trade > chighs[t.sym]) chighs[t.sym] = t.trade;
                            if (t.trade < clows[t.sym]) clows[t.sym] = t.trade;
                        }
                        catch (KeyNotFoundException)
                        {
                            decimal high = DayHigh(t.sym);
                            decimal low = DayLow(t.sym);
                            chighs.Add(t.sym, high);
                            if (low == 0) low = 640000;
                            clows.Add(t.sym, low);
                        }
                    }
                    if (gotTick != null) gotTick(t);
                    break;
                case TL2.EXECUTENOTIFY:
                    // date,time,symbol,side,size,price,comment
                    Trade tr = Trade.Deserialize(msg);
                    try
                    {
                        cpos[tr.symbol] = new Position(tr.symbol, AvgPrice(tr.symbol), PosSize(tr.symbol));
                    }
                    catch (KeyNotFoundException)
                    {
                        cpos.Add(tr.symbol, new Position(tr.symbol, AvgPrice(tr.symbol), PosSize(tr.symbol)));
                    }
                    if (gotFill != null) gotFill(tr);
                    break;
                case TL2.ORDERNOTIFY:
                    Order o = Order.Deserialize(msg);
                    if (gotOrder != null) gotOrder(o);
                    break;

                case TL2.ACCOUNTRESPONSE:
                    if (gotAccounts != null) gotAccounts(msg);
                    break;
                case TL2.FEATURERESPONSE:
                    string[] p = msg.Split(',');
                    List<TL2> f = new List<TL2>();
                    foreach (string s in p)
                    {
                        try
                        {
                            f.Add((TL2)Convert.ToInt32(s));
                        }
                        catch (Exception) { }
                    }
                    if (gotSupportedFeatures != null) 
                        gotSupportedFeatures(f.ToArray());
                    break;
            }
            result = 0;
            m.Result = (IntPtr)result;
        }
    }


}

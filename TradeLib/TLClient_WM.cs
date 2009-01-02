using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace TradeLib
{
    public class TLClient_WM : Form , TradeLinkClient
    {

        // clients that want notifications for subscribed stocks can override these methods
        /// <summary>
        /// Occurs when TradeLink receives any type of message [got message].
        /// </summary>
        public event TickDelegate gotTick;
        public event FillDelegate gotFill;
        public event OrderDelegate gotOrder;
        public event DebugDelegate gotAccounts;
        public event UIntDelegate gotOrderCancel;
        public event TL2MsgDelegate gotSupportedFeatures;
        public event PositionDelegate gotPosition;

        public TLClient_WM() : this("TradeLinkClient",true,true) { }
        public TLClient_WM(string clientname, bool showwarningonmissing) : this(clientname, showwarningonmissing, true) { }

        public TLClient_WM(string clientname, bool showarmingonmissingserver, bool handleexceptions)
            : base()
        {
            this.Text = WMUtil.GetUniqueWindow(clientname);
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
            this.Mode(this.TLFound(), handleexceptions, showarmingonmissingserver);
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
        public long TLSend(TL2 type) { return TLSend(type, ""); }
        delegate long TLSendDelegate(TL2 type, string msg);
        public long TLSend(TL2 type, string m)
        {
            if (InvokeRequired)
                return (long)Invoke(new TLSendDelegate(TLSend), new object[] { type, m });
            else
            {
                if (himh == IntPtr.Zero) throw new TLServerNotFound();
                long res = WMUtil.SendMsg(m, himh, Handle, (int)type);
                return res;
            }
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
        public decimal FastHigh(string sym)
        {
            try
            {
                return chighs[sym];
            }
            catch (KeyNotFoundException)
            {
                return 0;
            }
        }
        /// <summary>
        /// Today's low
        /// </summary>
        /// <param name="sym">The symbol</param>
        /// <returns></returns>
        public decimal FastLow(string sym)
        {
            try
            {
                return clows[sym];
            }
            catch (KeyNotFoundException)
            {
                return 0;
            }
        }

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
        /// <summary>
        /// Sends a request for current positions.  gotPosition event will fire for each position record held by the broker.
        /// </summary>
        /// <param name="account">account to obtain position list for (required)</param>
        /// <returns>number of positions to expect</returns>
        public int RequestPositions(string account) { if (account == "") return 0; return (int)TLSend(TL2.POSITIONREQUEST, Text + "+" + account); }

        public Brokers BrokerName 
        { 
            get 
            { 
                long res = TLSend(TL2.BROKERNAME);
                return (Brokers)res;
            } 
        }

        public int ServerVersion { get { return (int)TLSend(TL2.VERSION); } }

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
                    Tick t = Tick.Deserialize(msg);
                    if (t.isTrade)
                    {
                        try
                        {
                            if (t.trade > chighs[t.symbol]) chighs[t.symbol] = t.trade;
                            if (t.trade < clows[t.symbol]) clows[t.symbol] = t.trade;
                        }
                        catch (KeyNotFoundException)
                        {
                            chighs.Add(t.symbol, 0);
                            clows.Add(t.symbol, decimal.MaxValue);
                        }
                    }
                    if (gotTick != null) gotTick(t);
                    break;
                case TL2.EXECUTENOTIFY:
                    // date,time,symbol,side,size,price,comment
                    Trade tr = Trade.Deserialize(msg);
                    if (gotFill != null) gotFill(tr);
                    break;
                case TL2.ORDERNOTIFY:
                    Order o = Order.Deserialize(msg);
                    if (gotOrder != null) gotOrder(o);
                    break;
                case TL2.POSITIONRESPONSE:
                    Position pos = Position.Deserialize(msg);
                    if (gotPosition != null) gotPosition(pos);
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

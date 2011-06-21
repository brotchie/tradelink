using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// TradeLink clients can connect to any supported TradeLink broker.
    /// version of the client that supports the tradelink protocol via windows messaging transport.
    /// </summary>
    [System.ComponentModel.DesignerCategory("")]
    public class TLClient_WM : Form , TLClient
    {

        // clients that want notifications for subscribed stocks can override these methods

        public event TickDelegate gotTick;
        public event FillDelegate gotFill;
        public event OrderDelegate gotOrder;
        public event DebugDelegate gotAccounts;
        public event LongDelegate gotOrderCancel;
        public event MessageTypesMsgDelegate gotFeatures;
        public event PositionDelegate gotPosition;
        public event ImbalanceDelegate gotImbalance;
        public event MessageDelegate gotUnknownMessage;
        public event DebugDelegate SendDebugEvent;
        public event DebugDelegate gotServerUp;
        public event DebugDelegate gotServerDown;

        // member fields
        IntPtr himh = IntPtr.Zero;
        List<MessageTypes> _rfl = new List<MessageTypes>();
        public List<MessageTypes> RequestFeatureList { get { return _rfl; } }
        Dictionary<string, PositionImpl> cpos = new Dictionary<string, PositionImpl>();
        List<Providers> servers = new List<Providers>();
        List<string> srvrwin = new List<string>();
        const int MAXSERVER = 10;
        int _curprovider = -1;

        public void Start()
        {
            Mode();
        }

        public void Stop()
        {

        }

        public new string Name { get { return Text; } set { Text = WMUtil.GetUniqueWindow(value); Mode(_curprovider, false); } }

        public Providers[] ProvidersAvailable { get { return servers.ToArray(); } }
        public int ProviderSelected { get { return _curprovider; } }
        public TLClient_WM() : this(0,WMUtil.CLIENTWINDOW,true) { }
        public TLClient_WM(bool showwarning) : this(0, WMUtil.CLIENTWINDOW, showwarning) { }
        public TLClient_WM(int ProviderIndex) : this(ProviderIndex, WMUtil.CLIENTWINDOW, true ) { }
        public TLClient_WM(int ProviderIndex,string clientname) : this(ProviderIndex, clientname, true) { }
        public TLClient_WM(string clientname, bool showwarningonmissing) : this(0,clientname, showwarningonmissing) { }

        public TLClient_WM(int ProviderIndex, string clientname, bool showarmingonmissingserver)
            : base()
        {
            this.Text = WMUtil.GetUniqueWindow(clientname);
            gotFeatures += new MessageTypesMsgDelegate(TLClient_WM_gotFeatures);
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
            this.Mode(ProviderIndex, showarmingonmissingserver);
        }

        void TLClient_WM_gotFeatures(MessageTypes[] messages)
        {
            lock (_rfl)
            {
                _rfl.Clear();
                foreach (MessageTypes mt in messages)
                    _rfl.Add(mt);
            }
        }

        bool _noverb = true;
        /// <summary>
        /// verbose debugging
        /// </summary>
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }

        void v(string msg)
        {
            if (_noverb)
                return;
            debug(msg);
        }

 
        /// <summary>
        /// Gets or sets my handle of the parent application or form.
        /// </summary>
        /// <value>Me H.</value>
        public IntPtr MeH { get { return this.Handle; } }
        delegate bool ModeDel(int pi, bool warn);
        /// <summary>
        /// Sets the preferred communication channel of the link, if multiple channels are avaialble.
        /// Defaults to first provider found.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public bool Mode() { return Mode(0, true); }
        public bool Mode(int ProviderIndex, bool showwarning)
        {
            if (InvokeRequired)
            {
                bool v = false;
                try
                {
                    v = (bool)Invoke(new ModeDel(Mode), new object[] { ProviderIndex, showwarning });
                }
                catch { v = false; }
                return v;
            }
            else
            {

                TLFound();
                if ((ProviderIndex >= srvrwin.Count) || (ProviderIndex < 0))
                {
                    if (showwarning)
                        System.Windows.Forms.MessageBox.Show("Invalid broker specified or no brokers running.", "TradeLink server not found");
                    return false;
                }

                try
                {
                    Disconnect();
                    himh = WMUtil.HisHandle(srvrwin[ProviderIndex]);
                    Register();
                    RequestFeatures();
                    _curprovider = ProviderIndex;
                    return true;
                }
                catch (TLServerNotFound)
                {

                    if (showwarning)
                        System.Windows.Forms.MessageBox.Show("No Live broker instance was found.  Make sure broker application + TradeLink server is running.", "TradeLink server not found");
                    return false;
                }
                catch (Exception) { return false; }
            }
        }

        
        delegate long TLSendDelegate(MessageTypes type, string msg, IntPtr d);
        public long TLSend(MessageTypes type, long source, long dest, long msgid, string message, ref string result)
        {
            return TLSend(type, message);
        }
        /// <summary>
        /// sends a message to server.  
        /// synchronous responses are returned immediately as a long
        /// asynchronous responses come via their message type, or UnknownMessage event otherwise
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public long TLSend(MessageTypes type) { return TLSend(type, ""); }
        public long TLSend(MessageTypes type, string m) { return TLSend(type, m, himh); }
        public long TLSend(MessageTypes type, string m,int ProviderIndex) { return ((ProviderIndex<0)|| (ProviderIndex>srvrwin.Count)) ? 0 : TLSend(type, m, WMUtil.HisHandle(srvrwin[ProviderIndex])); }
        public long TLSend(MessageTypes type, string m, IntPtr dest)
        {
            if (InvokeRequired)
            {
                // ensure that our object is still running before we message from it
                try
                {
                    return (long)Invoke(new TLSendDelegate(TLSend), new object[] { type, m, dest });
                }
                catch (NullReferenceException) { }
                catch (ObjectDisposedException) { }
                return 0;
            }
            else
            {
                if (dest == IntPtr.Zero) throw new TLServerNotFound();
                long res = WMUtil.SendMsg(m, dest, Handle, (int)type);
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
            if (o == null) return (int)MessageTypes.EMPTY_ORDER;
            if (!o.isValid) return (int)MessageTypes.EMPTY_ORDER;
            string m = OrderImpl.Serialize(o);
            return (int)TLSend(MessageTypes.SENDORDER, m);
        }
        /// <summary>
        /// request a list of features, result will be returned to gotFeatureResponse and RequestFeaturesList
        /// </summary>
        public void RequestFeatures() 
        {
            _rfl.Clear();
            TLSend(MessageTypes.FEATUREREQUEST,Text); 
        }


        /// <summary>
        /// Request an order be canceled
        /// </summary>
        /// <param name="orderid">the id of the order being canceled</param>
        public void CancelOrder(Int64 orderid) { TLSend(MessageTypes.ORDERCANCELREQUEST, orderid.ToString()); }

        /// <summary>
        /// Send an account request, response is returned via the gotAccounts event.
        /// </summary>
        /// <returns>error code, and list of accounts via the gotAccounts event.</returns>
        /// 
        public int RequestAccounts() { return (int)TLSend(MessageTypes.ACCOUNTREQUEST, Text); }
        /// <summary>
        /// send a request so that imbalances are sent when received (via gotImbalance)
        /// </summary>
        /// <returns></returns>
        public int RequestImbalances() { return (int)TLSend(MessageTypes.IMBALANCEREQUEST, Text); }
        /// <summary>
        /// Sends a request for current positions.  gotPosition event will fire for each position record held by the broker.
        /// </summary>
        /// <param name="account">account to obtain position list for (required)</param>
        /// <returns>number of positions to expect</returns>
        public int RequestPositions(string account) { if (account == "") return 0; return (int)TLSend(MessageTypes.POSITIONREQUEST, Text + "+" + account); }

        public Providers BrokerName 
        { 
            get 
            {
                //long res = WMUtil.SendMsg(string.Empty, himh, MeH, (int)MessageTypes.BROKERNAME, 200);
                long res = TLSend(MessageTypes.BROKERNAME);
                return (Providers)res;
            } 
        }

        public int ServerVersion { get { return (int)TLSend(MessageTypes.VERSION); } }

        public void Disconnect()
        {
            try
            {
                TLSend(MessageTypes.CLEARCLIENT, Text);
            }
            catch (TLServerNotFound) { }
        }

        public void Register()
        {
            TLSend(MessageTypes.REGISTERCLIENT, Text);
        }

        public void Subscribe(TradeLink.API.Basket mb)
        {
            TLSend(MessageTypes.REGISTERSTOCK, Text + "+" + mb.ToString());
        }

        public void Unsubscribe()
        {
            TLSend(MessageTypes.CLEARSTOCKS, Text);
        }

        public int HeartBeat()
        {
            return (int)TLSend(MessageTypes.HEARTBEATREQUEST, Text);
        }

        public void RequestDOM()
        {
            int depth = 4; //default depth
            TLSend(MessageTypes.DOMREQUEST, Text + "+" + depth);
        }
        
        public void RequestDOM(int depth)
        {
            TLSend(MessageTypes.DOMREQUEST, Text + "+" + depth);
        }

        public Providers [] TLFound()
        {
            servers.Clear();
            srvrwin.Clear();
            string[] legacy = new string[] { WMUtil.SIMWINDOW, WMUtil.LIVEWINDOW, WMUtil.REPLAYWINDOW, WMUtil.TESTWINDOW, WMUtil.SERVERWINDOW };
            // see if we have a window running with this name and add it
            foreach (string name in legacy)
                addserver(name);
            for (int i = 0; i < MAXSERVER; i++)
                addserver(WMUtil.SERVERWINDOW + "."+i.ToString());
            return servers.ToArray();
        }

        private bool addserver(string name)
        {
            // if server not running, don't add it
            if (!WMUtil.Found(name)) return false;
            // if it is running, get the handle
            IntPtr hand = WMUtil.HisHandle(name);
            // get broker name
            Providers p = (Providers)WMUtil.SendMsg(string.Empty, hand, Handle, (int)MessageTypes.BROKERNAME);
            if (p == Providers.Unknown) return false;
            servers.Add(p);
            srvrwin.Add(name);
            return true;
        }

        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        int _tickerrors = 0;

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
            switch (tlm.type)
            {
                case MessageTypes.TICKNOTIFY:
                    Tick t;
                    try
                    {
                        t = TickImpl.Deserialize(msg);

                    }
                    catch (Exception ex)
                    {
                        _tickerrors++;
                        debug("Error processing tick: " + msg);
                        debug("TickErrors: " + _tickerrors);
                        debug("Error: " + ex.Message + ex.StackTrace);
                        break;
                    }
                    if (gotTick != null) 
                        gotTick(t);
                    break;
                case MessageTypes.IMBALANCERESPONSE:
                    Imbalance i = ImbalanceImpl.Deserialize(msg);
                    if (gotImbalance != null)
                        gotImbalance(i);
                    break;
                case MessageTypes.ORDERCANCELRESPONSE:
                    {
                        long id = 0;
                        if (gotOrderCancel != null)
                            if (long.TryParse(msg, out id))
                                gotOrderCancel(id);
                            else if (SendDebugEvent!=null)
                                SendDebugEvent("Count not parse order cancel: " + msg);
                    }
                    break;
                case MessageTypes.EXECUTENOTIFY:
                    // date,time,symbol,side,size,price,comment
                    try
                    {
                        Trade tr = TradeImpl.Deserialize(msg);
                        if (gotFill != null) gotFill(tr);
                    }
                    catch (Exception ex)
                    {
                        debug("error deserializing fill: " + msg);
                        debug("error: " + ex.Message + ex.StackTrace);
                        debug("broker: " + BrokerName);
                    }
                    break;
                case MessageTypes.ORDERNOTIFY:
                    try
                    {
                        Order o = OrderImpl.Deserialize(msg);
                        if (gotOrder != null) gotOrder(o);
                    }
                    catch (Exception ex)
                    {
                        debug("error deserializing order: " + msg);
                        debug("error: " + ex.Message + ex.StackTrace);
                        debug("broker: " + BrokerName);
                    }
                    break;
                case MessageTypes.POSITIONRESPONSE:
                    try
                    {
                        Position pos = PositionImpl.Deserialize(msg);
                        if (gotPosition != null) gotPosition(pos);
                    }
                    catch (Exception ex)
                    {
                        if (SendDebugEvent!=null)
                            SendDebugEvent(msg+" "+ex.Message + ex.StackTrace);
                    }
                    break;

                case MessageTypes.ACCOUNTRESPONSE:
                    if (gotAccounts != null) gotAccounts(msg);
                    break;
                case MessageTypes.FEATURERESPONSE:
                    string[] p = msg.Split(',');
                    List<MessageTypes> f = new List<MessageTypes>();
                    foreach (string s in p)
                    {
                        try
                        {
                            f.Add((MessageTypes)Convert.ToInt32(s));
                        }
                        catch (Exception) { }
                    }
                    if (gotFeatures != null) 
                        gotFeatures(f.ToArray());
                    if (gotUnknownMessage != null)
                        gotUnknownMessage(tlm.type, 0, 0, 0, string.Empty, ref tlm.body);
                    break;
                case MessageTypes.SERVERDOWN:
                    if (gotServerDown != null)
                        gotServerDown(msg);
                    break;
                case MessageTypes.SERVERUP:
                    if (gotServerUp != null)
                        gotServerUp(msg);
                    break;
                default:
                    if (gotUnknownMessage != null)
                    {
                        gotUnknownMessage(tlm.type, 0, 0, 0, string.Empty, ref tlm.body);
                    }
                    break;
            }
            result = 0;
            m.Result = (IntPtr)result;
        }
    }


}

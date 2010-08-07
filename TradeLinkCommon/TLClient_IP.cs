using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TradeLink.Common
{
    /// <summary>
    /// TradeLink clients can connect to any supported TradeLink broker.
    /// version of the client that supports the tradelink protocol via windows messaging transport.
    /// </summary>
    [System.ComponentModel.DesignerCategory("")]
    public class TLClient_IP : TLClient
    {
        Socket server;
        System.ComponentModel.BackgroundWorker _bw;
        int port = IPUtil.TLDEFAULTBASEPORT;
        int _wait = 50;

        bool connect() { return connect(_curprovider != -1 ? _curprovider : 0); }
        bool connect(int providerindex) { return connect(providerindex, false); }
        bool connect(int providerindex, bool showwarn)
        {
            if ((providerindex >= servers.Count) || (providerindex < 0))
            {
                debug("Ensure provider is running and Mode() is called with correct provider number.   invalid provider: " + providerindex);
                return false;
            }
            if ((server == null) || (!server.Connected && (_curprovider==providerindex)) || (_curprovider!=providerindex))
            {
                try
                {
                    debug("Attempting connection to server: " + serverip[providerindex]);
                    // shutdown cleanly if connected and we're switching servers
                    if ((server!=null) && (server.Connected))
                    {
                        server.Shutdown(SocketShutdown.Both);
                        server.Disconnect(true);
                    }
                    // create socket
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    server.Connect(serverip[providerindex]);
                    if (server.Connected)
                    {
                        // set our name
                        _name = server.LocalEndPoint.ToString();
                        // notify
                        debug("connected to server: " + serverip[providerindex]+" via:"+Name);
                        // set buffer size
                        buffer = new byte[server.ReceiveBufferSize];

                    }
                    else
                        debug("unable to connect to client at: " + serverip[providerindex].ToString());

                }
                catch (Exception ex)
                {
                    debug("exception creating connection to: " + serverip[providerindex].ToString());
                    debug(ex.Message + ex.StackTrace);
                    return false;
                }
                return true;

            }
            return true;
        }

        int bufferoffset = 0;
        byte[] buffer;

        void _bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int p = (int)e.Argument;
            // run until client stopped
            while (_started)
            {
                try
                {
                    // quit thread if requested
                    if (e.Cancel)
                    {
                        _started = false;
                        break;
                    }
                    int ret = server.Receive(buffer, bufferoffset,buffer.Length-bufferoffset, SocketFlags.None);
                    if (ret > 0)
                    {
                        // get messages from data
                        Message[] msgs = Message.gotmessages(ref buffer,ref bufferoffset);
                        // handle messages
                        for (int i = 0; i<msgs.Length; i++)
                            handle(msgs[i].Type, msgs[i].Content);
                    }
                    else
                        Thread.Sleep(_wait);
                }
                catch (SocketException ex)
                {
                    debug("socket exception: " + ex.SocketErrorCode + ex.Message + ex.StackTrace);

                }
                catch (Exception ex)
                {
                    debug(ex.Message + ex.StackTrace);
                }
                if ((server == null) || !server.Connected)
                {
                    if ((p >= 0) && (p < serverip.Count))
                        debug("client lost connection to server: " + serverip[p]);
                    if (connect(p, false))
                        debug("recovered connection to: " + serverip[p]);
                    else
                        debug("unable to recover connection to: " + serverip[p]);
                    break;
                }
            }
        }

        bool _started = false;

        private void StartRecieve()
        {
            
        }

        public void Start()
        {
            debug("Start called, attempting to restart current provider.");
            Mode(_curprovider, false);


        }

        public void Stop()
        {
            _started = false;

            try
            {
                _bw.CancelAsync();

                server.Shutdown(SocketShutdown.Both);
                server.Close();


            }
            catch (SocketException ex)
            {
                debug("socket exception: " + ex.Message + ex.StackTrace);
            }
            catch (Exception ex)
            {
                debug("Error stopping TLClient_IP " + ex.Message + ex.StackTrace);
            }
            debug("Stopped: " + Name);
        }
        

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

        List<MessageTypes> _rfl = new List<MessageTypes>();
        public List<MessageTypes> RequestFeatureList { get { return _rfl; } }
        Dictionary<string, PositionImpl> cpos = new Dictionary<string, PositionImpl>();
        List<Providers> servers = new List<Providers>();

        const int MAXSERVER = 10;
        List<IPEndPoint> serverip = new List<IPEndPoint>();
        int _curprovider = -1;
        string _name = string.Empty;
        public string Name
        {
            get
            {
                return _name;
            }
            set { }
        }

        int _disconnectretry = 3;
        public int DisconnectRetries { get { return _disconnectretry; } set { _disconnectretry = value; } }

        public Providers[] ProvidersAvailable { get { return servers.ToArray(); } }
        public int ProviderSelected { get { return _curprovider; } }

        public TLClient_IP(bool showwarning) : this(0, WMUtil.CLIENTWINDOW, showwarning) { }
        public TLClient_IP(int ProviderIndex,string clientname) : this(ProviderIndex, clientname, true) { }
        public TLClient_IP(string clientname, bool showwarningonmissing) : this(0,clientname, showwarningonmissing) { }

        public TLClient_IP(int ProviderIndex, string clientname, bool showarmingonmissingserver)
            : base()
        {
            gotFeatures += new MessageTypesMsgDelegate(TLClient_WM_gotFeatures);
            this.Mode(ProviderIndex, showarmingonmissingserver);
        }

        public static List<IPEndPoint> GetEndpoints(int port, params string[] servers)
        {
            List<IPEndPoint> ip = new List<IPEndPoint>();
            foreach (string server in servers)
                ip.Add(new IPEndPoint(IPAddress.Parse(server),port));
            return ip;
        }
        public static List<IPEndPoint> GetEndpoints(params IPEndPoint[] eps) 
        { 
            List<IPEndPoint> ip = new List<IPEndPoint>();
            foreach(IPEndPoint ep in eps)
                ip.Add(ep);
            return ip;
        }

        public TLClient_IP() : this(IPUtil.TLDEFAULTBASEPORT) { }
        public TLClient_IP(int serverport)
            : this(IPAddress.Loopback.ToString(),serverport)
        {
            port = serverport;
        }

        public TLClient_IP(int serverport,DebugDelegate deb)
            : this(IPAddress.Loopback.ToString(),serverport,deb)
        {
            port = serverport;
        }


        public TLClient_IP(string server, int port)
            : this(GetEndpoints(port, new string[] { server }), 0, "tlclient")
        {
            this.port = port;
        }

        public TLClient_IP(string server, int port,DebugDelegate deb)
            : this(GetEndpoints(port, new string[] { server }), 0, "tlclient", DEFAULTRETRIES, DEFAULTWAIT, deb)
        {
            this.port = port;
        }

        public TLClient_IP(string[] servers, int port)
            : this(GetEndpoints(port, servers), 0, "tlclient")
        {
            this.port = port;
        }

        const int DEFAULTWAIT = 100;
        const int DEFAULTRETRIES = 3;

        public TLClient_IP(string[] servers, int port,DebugDelegate deb)
            : this(GetEndpoints(port, servers), 0, "tlclient", DEFAULTRETRIES,DEFAULTWAIT,deb)
        {
            this.port = port;
        }

        public TLClient_IP(string[] servers, int port, int ProviderIndex) : this(GetEndpoints(port,servers),ProviderIndex,"tlclient")
        {
            this.port = port;
        }
        public TLClient_IP(string[] servers, int port, int ProviderIndex, DebugDelegate deb)
            : this(GetEndpoints(port, servers), ProviderIndex, "tlclient", DEFAULTRETRIES, DEFAULTWAIT,deb)
        {
            this.port = port;
        }
        public TLClient_IP(List<IPEndPoint> servers, int ProviderIndex, string Clientname) : this(servers, ProviderIndex, Clientname, DEFAULTRETRIES, DEFAULTWAIT, null) { }
        public TLClient_IP(List<IPEndPoint> servers, int ProviderIndex, string Clientname, int disconnectretries) : this(servers, ProviderIndex, Clientname, disconnectretries, DEFAULTWAIT, null) { }
        public TLClient_IP(List<IPEndPoint> servers, int ProviderIndex, string Clientname, int disconnectretries, int wait) : this(servers, ProviderIndex, Clientname, disconnectretries,wait, null) { }
        public TLClient_IP(List<IPEndPoint> servers, int ProviderIndex, string Clientname, int disconnectretries, int wait, DebugDelegate deb)
        {
            _wait = wait;
            SendDebugEvent = deb;
            serverip = servers;
            Mode(ProviderIndex, false);
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
            // search our provider list
            TLFound();
            // see if called from start
            if (ProviderIndex < 0)
            {
                debug("provider index cannot be less than zero, using first provider.");
                ProviderIndex = 0;
            }
            // attempt to connect to preferred
            bool ok = connect(ProviderIndex, false);
            if (!_started && ok)
            {
                // restart if we connected
                _started = true;
                // background thread to receive messages
                debug("client starting background thread.");
                _bw = new System.ComponentModel.BackgroundWorker();
                _bw.WorkerSupportsCancellation = true;
                _bw.DoWork += new System.ComponentModel.DoWorkEventHandler(_bw_DoWork);
                _bw.RunWorkerAsync(ProviderIndex);
            }
            if (!ok)
            {
                debug("Unable to connect to provider: " + ProviderIndex);
                return false;
            }

            try
            {
                // disconnect from provider if we were already registered
                Disconnect();
                // register ourselves with provider
                Register();
                // request list of features from provider
                RequestFeatures();
                // assuming we got this far, mark selected provider current
                _curprovider = ProviderIndex;
                return true;
            }
            catch (SocketException ex)
            {

                debug("socket exception: " + ex.SocketErrorCode + ex.Message + ex.StackTrace);
                    
            }
            catch (Exception ex) 
            { 
                debug(ex.Message+ex.StackTrace);
                
            }
            debug("Server not found at index: " + ProviderIndex);
            return false; 
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
        public long TLSend(MessageTypes type) { return TLSend(type, string.Empty); }
        public long TLSend(MessageTypes type, string m)
        {
            // encode
            byte[] data = Message.sendmessage(type, m);
            int len = 0;
            try
            {
                // send request
                len = server.Send(data);
                return 0;
            }
            catch (SocketException ex)
            {
                debug("exception: "+ex.Message+ex.StackTrace);
                debug("disconnected from server: " + serverip[_curprovider] + ", attempting reconnect...");
                int count = 0;
                bool rok = false;
                while (count++<_disconnectretry)
                {
                    rok = connect(_curprovider,false);
                    if (rok)
                        break;
                }
                debug(rok ? "reconnect suceeded." : "reconnect failed.");
                
            }
            catch (Exception ex)
            {
                debug("error sending: " + type + " " + m);
                debug(ex.Message + ex.StackTrace);
                
            }
            return (long)MessageTypes.UNKNOWN_ERROR;
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
            TLSend(MessageTypes.FEATUREREQUEST,Name); 
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
        public int RequestAccounts() { return (int)TLSend(MessageTypes.ACCOUNTREQUEST, Name); }
        /// <summary>
        /// send a request so that imbalances are sent when received (via gotImbalance)
        /// </summary>
        /// <returns></returns>
        public int RequestImbalances() { return (int)TLSend(MessageTypes.IMBALANCEREQUEST, Name); }
        /// <summary>
        /// Sends a request for current positions.  gotPosition event will fire for each position record held by the broker.
        /// </summary>
        /// <param name="account">account to obtain position list for (required)</param>
        /// <returns>number of positions to expect</returns>
        public int RequestPositions(string account) { if (account == "") return 0; return (int)TLSend(MessageTypes.POSITIONREQUEST, Name + "+" + account); }

        public Providers BrokerName 
        { 
            get 
            { 
                long res = TLSend(MessageTypes.BROKERNAME);
                return (Providers)res;
            } 
        }

        public int ServerVersion { get { return (int)TLSend(MessageTypes.VERSION); } }

        public void Disconnect()
        {
            try
            {
                TLSend(MessageTypes.CLEARCLIENT, Name);
            }
            catch (TLServerNotFound) { }
        }

        public void Register()
        {
            TLSend(MessageTypes.REGISTERCLIENT, Name);
        }

        public void Subscribe(TradeLink.API.Basket mb)
        {
            TLSend(MessageTypes.REGISTERSTOCK, Name + "+" + mb.ToString());
        }

        public void Unsubscribe()
        {
            TLSend(MessageTypes.CLEARSTOCKS, Name);
        }

        public int HeartBeat()
        {
            return (int)TLSend(MessageTypes.HEARTBEAT, Name);
        }

        public void RequestDOM()
        {
            int depth = 4; //default depth
            TLSend(MessageTypes.DOMREQUEST, Name + "+" + depth);
        }
        
        public void RequestDOM(int depth)
        {
            TLSend(MessageTypes.DOMREQUEST, Name + "+" + depth);
        }

        public Providers [] TLFound()
        {
            v("Searching provider list...");
            v("clearing existing list of available providers");
            servers.Clear();
            // build name request
            byte[] nrequest = Message.sendmessage(MessageTypes.BROKERNAME, string.Empty);
            // get name for every server provided by client
            foreach (IPEndPoint ep in serverip)
            {
                try
                {
                    v("Attempting to connect to: " + ep.ToString());
                    // attempt to connect
                    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    s.Connect(ep);
                    // request name
                    s.Send(nrequest);
                    // try to get result
                    byte[] data = new byte[s.ReceiveBufferSize];
                    int len = s.Receive(data);
                    int pcode = (int)Providers.Unknown;
                    try
                    {
                        pcode = BitConverter.ToInt32(data, 0);
                        Providers p = (Providers)pcode;
                        if (p != Providers.Unknown)
                        {
                            debug("provider: " + p.ToString() + " at: " + ep.ToString());
                            servers.Add(p);
                        }
                        else
                        {
                            debug("skipping unknown provider at: " + ep.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        debug("error adding providing at: " + ep.ToString() + " pcode: " + pcode);
                        debug(ex.Message + ex.StackTrace);
                    }
                }
                catch (Exception ex)
                {
                    debug("exception connecting to server: " + ep.ToString());
                    debug(ex.Message + ex.StackTrace);
                }
            }
            v("found " + servers.Count + " providers.");
            return servers.ToArray();
        }


        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        int _tickerrors = 0;
        bool _noverb = false;
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }

        void v(string msg)
        {
            if (_noverb) return;
            debug(msg);
        }

        void handle(MessageTypes type, string msg)
        {
            long result = 0;
            switch (type)
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
                    Trade tr = TradeImpl.Deserialize(msg);
                    if (gotFill != null) gotFill(tr);
                    break;
                case MessageTypes.ORDERNOTIFY:
                    Order o = OrderImpl.Deserialize(msg);
                    if (gotOrder != null) gotOrder(o);
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
                        gotUnknownMessage(type, 0, 0, 0, string.Empty, ref msg);
                    }
                    break;
            }
            result = 0;
        
        }

            
            
    }

    // credit : http://www.codeproject.com/KB/IP/realtimeapp.aspx


}

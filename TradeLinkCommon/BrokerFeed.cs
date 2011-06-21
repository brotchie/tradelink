using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using System.Threading;

namespace TradeLink.Common
{
    public class BrokerFeed : TLClient
    {
        string PROGRAM = "BrokerFeed";
        public string Name { get { return PROGRAM; } set { PROGRAM = value; } }
        bool feedready = false;
        TLClient quote;
        TLClient execute;
        Providers _feed;
        Providers _broker;
        bool _reqpref = true;
        bool _isprefq = false;
        bool _isprefx = false;
        /// <summary>
        /// returns whether preferred feed is being used
        /// </summary>
        public bool isPreferredFeed { get { return _isprefq; } }
        /// <summary>
        /// returns whether preferred broker is being used
        /// </summary>
        public bool isPreferredBroker { get { return _isprefx; } }
        /// <summary>
        /// returns current client being used for feed
        /// </summary>
        public TLClient FeedClient { get { return quote; } set { quote = value; }  }
        /// <summary>
        /// returns client of broker provider
        /// </summary>
        public TLClient BrokerClient { get { return execute; } set { execute = value; } }
        /// <summary>
        /// returns current feed provider
        /// </summary>
        public Providers Feed { get { return _feed; } }
        /// <summary>
        /// returns current broker provider
        /// </summary>
        public Providers Broker { get { return _broker; } }
        /// <summary>
        /// whether feed/broker connections will be attempted if preferred options are not available
        /// </summary>
        public bool RequirePreferred { get { return !_reqpref; } }
        bool _threadsafe = true;
        /// <summary>
        /// whether extra thread safety is enabled (generally not useful)
        /// </summary>
        public bool isThreadSafe { get { return _threadsafe; } }
        /// <summary>
        /// create a new brokerfeed with default parameters
        /// </summary>
        public BrokerFeed() : this(Providers.Unknown, Providers.Unknown, true,false) { }
        Thread _reader;
        bool _readergo = true;

        /// <summary>
        /// not used.   call BrokerClient.ServerVersion or FeedClient.ServerVersion
        /// </summary>
        public int ServerVersion { get { return 0; } }
        /// <summary>
        /// not used.   See Broker or Feed properties.
        /// </summary>
        public Providers BrokerName { get { return Providers.Unknown; } }

        string[] _servers = new string[0];
        int _port = IPUtil.TLDEFAULTBASEPORT;

        /// <summary>
        /// create broker feed
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="broker"></param>
        /// <param name="useany"></param>
        /// <param name="threadsafe"></param>
        public BrokerFeed(Providers feed, Providers broker, bool useany, bool threadsafe) : this(feed, broker, useany, threadsafe, "BrokerFeed", new string[0],IPUtil.TLDEFAULTBASEPORT) { }
        /// <summary>
        /// if you provide ip addresses, BF will use IP as the transport.
        /// otherwise it uses windows ipc/messaging
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="broker"></param>
        /// <param name="useany"></param>
        /// <param name="threadsafe"></param>
        /// <param name="program"></param>
        /// <param name="servers"></param>
        /// <param name="port"></param>
        public BrokerFeed(Providers feed, Providers broker, bool useany, bool threadsafe,string program, string[] servers, int port)
        {
            _servers = servers;
            _port = port;
            PROGRAM = program;
            _feed = feed;
            _broker = broker;
            _reqpref = useany;
            _threadsafe = threadsafe;
            if (_threadsafe)
            {
                _reader = new Thread(new ParameterizedThreadStart(readdata));
                _reader.Start();
            }
        }

        // thread-safe buffers
        RingBuffer<Tick> _kbuf = new RingBuffer<Tick>(10000);
        RingBuffer<Order> _obuff = new RingBuffer<Order>(1000);
        RingBuffer<Trade> _fbuff = new RingBuffer<Trade>(100);
        RingBuffer<long> _cbuff = new RingBuffer<long>(1000);
        RingBuffer<Position> _pbuff = new RingBuffer<Position>(100);
        RingBuffer<string> _abuff = new RingBuffer<string>(10);
        RingBuffer<GenericMessage> _mbuff = new RingBuffer<GenericMessage>(500);
        RingBuffer<GenericMessage> _mqbuff = new RingBuffer<GenericMessage>(500);

        void readdata(object obj)
        {
            while (_readergo)
            {
                try {
                while (_kbuf.hasItems)
                {
                    Tick k = _kbuf.Read();
                    if (gotTick != null)
                        gotTick(k);
                }

                while (_obuff.hasItems)
                {
                    Order o = _obuff.Read();
                    if (gotOrder != null)
                        gotOrder(o);
                }

                while (_cbuff.hasItems)
                {
                    long c = _cbuff.Read();
                    if (gotOrderCancel != null)
                        gotOrderCancel(c);
                }

                while (_fbuff.hasItems)
                {
                    Trade f = _fbuff.Read();
                    if (gotFill != null)
                        gotFill(f);
                }

                while (_mbuff.hasItems)
                {
                    GenericMessage m = _mbuff.Read();
                    if (gotUnknownMessage != null)
                        gotUnknownMessage(m.Type, m.Source, m.Dest, m.ID, m.Request, ref m.Response);
                }

                while (_mqbuff.hasItems)
                {
                    GenericMessage m = _mqbuff.Read();
                    if (gotUnknownMessage != null)
                        gotUnknownMessage(m.Type, m.Source, m.Dest, m.ID, m.Request, ref m.Response);

                }

                while (_pbuff.hasItems)
                {
                    Position p = _pbuff.Read();
                    if (gotPosition != null)
                        gotPosition(p);
                }

                while (_abuff.hasItems)
                {
                    string acct = _abuff.Read();
                    if (gotAccounts != null)
                        gotAccounts(acct);
                }


                    Thread.Sleep(100);
                }
                catch (ThreadInterruptedException) { _interrupts++; }
            }
        }

        int _interrupts = 0;
        /// <summary>
        /// return # of interrupts when running in thread safe mode
        /// </summary>
        public int SafeThreadInterrupts { get { return _interrupts; } }
        /// <summary>
        /// subscribe to symbols from feed provider
        /// </summary>
        /// <param name="b"></param>
        public void Subscribe(Basket b)
        {
            if (quote == null) return;
            v("subscribing via feed to: " + b.ToString());
            quote.Subscribe(b);
        }
        /// <summary>
        /// unsubscribe symbols from feed provider
        /// </summary>
        public void Unsubscribe()
        {
            if (quote != null)
                quote.Unsubscribe();
        }
        /// <summary>
        /// register with providers
        /// </summary>
        public void Register()
        {
            if (quote != null)
                quote.Register();
            if (execute != null)
                execute.Register();
        }
        /// <summary>
        /// send a message to providers
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public long TLSend(MessageTypes type, string message)
        {
            string r = string.Empty;
            long res = TLSend(type, 0, 0, 0, message, ref r);
            return res;
        }
        /// <summary>
        /// send a message to providers
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="msgid"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public long TLSend(MessageTypes type, long source, long dest, long msgid, string message, ref string result)
        {
            v(type.ToString() + " sending to all providers: "+message);
            for (int i = 0; i < _pcon.Count; i++)
                if (_pcon[i].RequestFeatureList.Contains(type))
                {
                    bool showret = false;
                    // prepare message
                    switch (type)
                    {
                        case MessageTypes.DOMREQUEST:
                            message = message.Replace(Book.EMPTYREQUESTOR, _pcon[i].Name);
                            showret = true;
                            break;
                        case MessageTypes.BARREQUEST:
                            {
                                BarRequest br = BarImpl.ParseBarRequest(message);
                                br.Client = _pcon[i].Name;
                                message = BarImpl.BuildBarRequest(br);
                                showret = true;
                            }
                            break;
                        case MessageTypes.FEATUREREQUEST:
                            message = _pcon[i].Name;
                            showret = true;
                            break;
                    }
                    long res = _pcon[i].TLSend(type, message);
                    result = res.ToString();
                    if (gotUnknownMessage != null)
                        gotUnknownMessage(type, source, dest, msgid, message, ref result);

                    return res;
                }
                else if (VerboseDebugging)
                    v(_pcon[i].BrokerName + " " + _pcon[i].Name + " does not support feature " + type + ", dropping message.");
            return 0;
            
        }

        public void Disconnect()
        {
            v("disconnecting from all providers.");
            if (quote!=null)
                quote.Disconnect();
            if (execute != null)
                execute.Disconnect();
            for (int i = 0; i < _pcon.Count; i++)
                if (_pcon[i] != null)
                    _pcon[i].Disconnect();
        }

        public int HeartBeat()
        {
            if (quote != null)
                quote.HeartBeat();
            if (execute != null)
                execute.HeartBeat();
            return 0;
        }

        public bool Mode()
        {
            if (quote != null)
                quote.Mode();
            if (execute != null)
                execute.Mode();
            return true;
        }

        public void CancelOrder(long id)
        {
            if (execute == null) return;
            v("sending cancel: " + id);
            execute.CancelOrder(id);
        }

        public int SendOrder(Order o)
        {
            if (execute == null) return (int)MessageTypes.BROKERSERVER_NOT_FOUND;
            v("sending order: " + o.ToString());
            return execute.SendOrder(o);
        }

        public bool Mode(int provider, bool warn) { return true;  }

        public void Stop()
        {
            try
            {
                v("got stop request.");
                Disconnect();
                if (_threadsafe)
                {
                    _readergo = false;
                    try
                    {
                        _reader.Interrupt();
                    }
                    catch { }
                }
            }
            catch { }
        }

        Providers[] _pavail = new Providers[0];
        public Providers[] ProvidersAvailable { get { return _pavail; } }
        List<TLClient> _pcon = new List<TLClient>();
        public int ProviderSelected { get { return -1; } }
        public List<MessageTypes> RequestFeatureList { get { return _RequestFeaturesList; } }
        public void ProvidersUpdate()
        {
            TLClient_WM tl = new TLClient_WM(false);
            _pavail = tl.ProvidersAvailable;
            tl.Disconnect();
        }
        /// <summary>
        /// start broker feed
        /// </summary>
        public void Start()
        {
            Reset();
        }

        /// <summary>
        /// reset brokerfeed, look for any new servers and attempt to connect to current preferred providers
        /// </summary>
        public void Reset()
        {

            feedready = false;
            if (IPUtil.hasValidAddress(_servers))
                debug("At least one valid IpAddress found, attempting IP transport.");
            else
                debug("No ip addresses specified, attempting Windows IPC.");

            TLClient tl = getsearchclient();
            
            _pavail = tl.ProvidersAvailable;
            if (_pavail.Length == 0)
                debug("No providers were found. Ensure connectors are running.");

            bool setquote = false;
            bool setexec = false;

            // see if we can get preferred providers
            int xi = getproviderindex(_broker);
            int qi = getproviderindex(_feed);
            _isprefq = (qi != -1) && hasminquote(tl, qi);
            _isprefx = (xi != -1) && hasminexec(tl, xi);
            if (!isPreferredFeed)
                debug("preferred data not available: " + _feed);
            if (!isPreferredBroker)
                debug("preferred execute not available: " + _broker);
            // search for features

            for (int i = 0; i < ProvidersAvailable.Length; i++)
            {
                if ((qi != -1) && (xi != -1)) break;
                // switch to provider
                if ((qi == -1) && hasminquote(tl, i))
                    qi = i;
                if ((xi == -1) && hasminexec(tl, i))
                    xi = i;
            }

            // see if we're allowed to fallback

            // not allowed
            if (RequirePreferred)
            {
                setquote = isPreferredFeed ;
                setexec = isPreferredBroker ;
            }
            else // ok to fallback,but where
            {
 
                setquote = (qi != -1);
                setexec = (xi != -1);
            }

            // map handlers
            if (setquote)
            {
                quote = getrealclient(qi, PROGRAM + "quote");
                quote.gotFeatures += new MessageTypesMsgDelegate(quote_gotFeatures);
                debug("DataFeed: " + quote.BrokerName + " " + quote.ServerVersion);
                _feed = quote.BrokerName;
                // clear any leftover subscriptions
                quote.Unsubscribe();
                if (isThreadSafe)
                {
                    quote.gotTick += new TickDelegate(quote_gotTick);
                    quote.gotUnknownMessage += new MessageDelegate(quote_gotUnknownMessage);
                }
                else
                {
                    quote.gotTick += new TickDelegate(quote_gotTick2);
                    quote.gotUnknownMessage += new MessageDelegate(quote_gotUnknownMessage2);
                }
                quote.gotImbalance += new ImbalanceDelegate(quote_gotImbalance);
            }
            if (setexec)
            {
                execute = getrealclient(xi, PROGRAM + "exec");
                _broker = execute.BrokerName;
                execute.gotFeatures += new MessageTypesMsgDelegate(execute_gotFeatures);
                if (isThreadSafe)
                {
                    execute.gotAccounts += new DebugDelegate(execute_gotAccounts);
                    execute.gotFill += new FillDelegate(execute_gotFill);
                    execute.gotOrder += new OrderDelegate(execute_gotOrder);
                    execute.gotOrderCancel += new LongDelegate(execute_gotOrderCancel);
                    execute.gotPosition += new PositionDelegate(execute_gotPosition);
                    execute.gotUnknownMessage += new MessageDelegate(execute_gotUnknownMessage);

                }
                else
                {
                    execute.gotAccounts += new DebugDelegate(execute_gotAccounts2);
                    execute.gotFill += new FillDelegate(execute_gotFill2);
                    execute.gotOrder += new OrderDelegate(execute_gotOrder2);
                    execute.gotOrderCancel += new LongDelegate(execute_gotOrderCancel2);
                    execute.gotPosition += new PositionDelegate(execute_gotPosition2);
                    execute.gotUnknownMessage += new MessageDelegate(execute_gotUnknownMessage2);
                }
                debug("Executions: " + execute.BrokerName + " " + execute.ServerVersion);
                if (RequestAccountsOnStartup)
                    RequestAccounts();
            }

            feedready = true;
            // connect to the rest
            for (int i = 0; i < ProvidersAvailable.Length; i++)
            {
                // skip existing connections
                if (i == xi)
                {
                    _pcon.Add(execute);
                    continue;
                }
                if ((xi!=qi) && (i == qi))
                {
                    _pcon.Add(quote);
                    continue;
                }
                // add new connections
                TLClient newcon = getrealclient(i, PROGRAM);
                newcon.gotFeatures += new MessageTypesMsgDelegate(newcon_gotFeatures);
                newcon.gotUnknownMessage += new MessageDelegate(newcon_gotUnknownMessage);
                _pcon.Add(newcon);
            }
            tl.Disconnect();
            tl = null;
        }

        void newcon_gotUnknownMessage(MessageTypes type, long source, long dest, long msgid, string request, ref string response)
        {
            if (gotUnknownMessage != null)
                gotUnknownMessage(type, source, dest, msgid, request, ref response);
        }

        void newcon_gotFeatures(MessageTypes[] messages)
        {
            
        }

        void quote_gotUnknownMessage2(MessageTypes type, long source, long dest, long msgid, string request, ref string response)
        {
            if (gotUnknownMessage != null)
                gotUnknownMessage(type, source, dest, msgid, request, ref response);
        }

        void quote_gotTick2(Tick t)
        {
            if (gotTick != null)
                gotTick(t);
        }

        void execute_gotUnknownMessage2(MessageTypes type, long source, long dest, long msgid, string request, ref string response)
        {
            if (gotUnknownMessage != null)
                gotUnknownMessage(type, source, dest, msgid, request, ref response);
        }

        void execute_gotPosition2(Position pos)
        {
            if (gotPosition != null)
                gotPosition(pos);
        }

        void execute_gotOrderCancel2(long number)
        {
            if (gotOrderCancel != null)
                gotOrderCancel(number);
        }

        void execute_gotOrder2(Order o)
        {
            if (gotOrder != null)
                gotOrder(o);
        }

        void execute_gotFill2(Trade t)
        {
            if (gotFill != null)
                gotFill(t);
        }

        void execute_gotAccounts2(string msg)
        {
            debug("accounts: " + msg);
            if (RequestPositionsOnAccounts && (msg!=string.Empty))
            {
                string[] accts = msg.Split(',');
                foreach (string a in accts)
                    RequestPositions(a);
            }
            if (gotAccounts != null)
                gotAccounts(msg);

        }

        void quote_gotFeatures(MessageTypes[] messages)
        {
            _RequestFeaturesList.AddRange(messages);
        }

        void execute_gotFeatures(MessageTypes[] messages)
        {
            _RequestFeaturesList.AddRange(messages);
        }

        List<MessageTypes> _RequestFeaturesList = new List<MessageTypes>();

        public void RequestFeatures()
        {
            _RequestFeaturesList.Clear();
            if (quote != null)
                quote.RequestFeatures();
            if (execute != null)
                execute.RequestFeatures();
        }

        void quote_gotUnknownMessage(MessageTypes type, long source, long dest, long msgid, string request, ref string response)
        {
            _mqbuff.Write(new GenericMessage(type, source, dest, msgid, request, response));
        }

        void execute_gotUnknownMessage(MessageTypes type, long source, long dest, long msgid, string request, ref string response)
        {
            _mbuff.Write(new GenericMessage(type, source, dest, msgid, request, response));

        }

        void execute_gotPosition(Position pos)
        {
            _pbuff.Write(pos);
        }

        void execute_gotOrderCancel(long number)
        {
            _cbuff.Write(number);
            _reader.Interrupt();
        }

        void execute_gotOrder(Order o)
        {
            _obuff.Write(o);
            _reader.Interrupt();
        }

        void execute_gotFill(Trade t)
        {
            _fbuff.Write(t);
            _reader.Interrupt();
        }

        bool _requestpositionsonaccount = true;
        /// <summary>
        /// request positions automatically when accounts are received
        /// </summary>
        public bool RequestPositionsOnAccounts { get { return _requestpositionsonaccount; } set { _requestpositionsonaccount = value; } }

        bool _requestaccountsonstart = true;
        /// <summary>
        /// request accounts when connection is reset or started
        /// </summary>
        public bool RequestAccountsOnStartup { get { return _requestaccountsonstart; } set { _requestaccountsonstart = value; } }

        void execute_gotAccounts(string msg)
        {
            debug("accounts: " + msg);
            if (RequestPositionsOnAccounts)
                RequestPositions(msg);
            _abuff.Write(msg);
        }
        /// <summary>
        /// request positions for a given account
        /// </summary>
        /// <param name="acct"></param>
        public void RequestPositions(string acct)
        {

            try
            {
                debug("requesting positions for account: " + acct);
                long r = execute.TLSend(MessageTypes.POSITIONREQUEST, execute.Name + "+" + acct);
            }
            catch (Exception ex)
            {
                debug("position request error: "+acct + ex.Message + ex.StackTrace);
            }

        }
        /// <summary>
        /// request accounts available on connection
        /// </summary>
        public void RequestAccounts()
        {
            try
            {
                debug("requesting accounts on: " + Name);
                long r = execute.TLSend(MessageTypes.ACCOUNTREQUEST, execute.Name);
            }
            catch (Exception ex)
            {
                debug("Account request error: " + ex.Message + ex.StackTrace);
            }
        }



        void quote_gotTick(Tick t)
        {
            _kbuf.Write(t);
            _reader.Interrupt();
        }

        void quote_gotImbalance(Imbalance imb)
        {
            if (gotImbalance != null)
                gotImbalance(imb);
        }

        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);

        }


        public bool isProviderAvailable(Providers p)
        {
            int idx = getproviderindex(p);
            return idx != -1;
        }
        /// <summary>
        /// whether feed is connected
        /// </summary>
        public bool isFeedConnected { get { return quote != null; } }
        /// <summary>
        /// whether broker is connected
        /// </summary>
        public bool isBrokerConnected { get { return execute != null; } }

        int getproviderindex(Providers p)
        {
            if (p == Providers.Unknown) return -1;
            for (int i = 0; i < ProvidersAvailable.Length; i++)
                if (ProvidersAvailable[i] == p) return i;
            return -1;
        }

        const int MAXFEATUREWAIT = 30;

        static bool hasminquote(TLClient tl, int provider)
        {
            try
            {
                bool v = (tl.ProviderSelected == provider) || tl.Mode(provider, false);
                bool test = true;
                int count = 0;
                while ((tl.BrokerName != Providers.Unknown) && (tl.BrokerName != Providers.Error)
                    && (tl.RequestFeatureList.Count == 0) && (count++ < MAXFEATUREWAIT))
                    Thread.Sleep(10);
                test &= tl.RequestFeatureList.Contains(MessageTypes.TICKNOTIFY);
                tl.Disconnect();
                return test && v;
            }
            catch { return false; }
        }

        static bool hasminexec(TLClient tl, int provider)
        {
            try
            {
                bool v = (tl.ProviderSelected == provider) || tl.Mode(provider, false);
                bool test = true;
                int count = 0;
                while ((tl.BrokerName != Providers.Unknown) && (tl.BrokerName != Providers.Error)
                    && (tl.RequestFeatureList.Count == 0) && (count++ < MAXFEATUREWAIT))
                    Thread.Sleep(10);
                test &= tl.RequestFeatureList.Contains(MessageTypes.EXECUTENOTIFY);
                test &= tl.RequestFeatureList.Contains(MessageTypes.SENDORDER);
                test &= tl.RequestFeatureList.Contains(MessageTypes.ORDERCANCELREQUEST);
                test &= tl.RequestFeatureList.Contains(MessageTypes.ORDERCANCELRESPONSE);
                tl.Disconnect();
                return test && v;
            }
            catch { return false; }
        }

        public bool ModifyFeed(int provider) { return ModifyFeed(provider, true); }
        public bool ModifyFeed(int provider, bool warn)
        {
            if (!feedready) return false;
            TLClient tl = getsearchclient();
            if ((provider < 0) || (provider > ProvidersAvailable.Length)) return false;
            Providers p = ProvidersAvailable[provider];
            bool ok = hasminquote(tl, provider);
            if (!ok)
            {
                System.Windows.Forms.MessageBox.Show(p.ToString() + " does not support quotes.");
                return false;
            }
            this.v(tl.Name + " " + provider + " " + tl.BrokerName + (ok ? " has feed support." : " no feed support."));
            _feed = p;
            tl.Disconnect();
            Reset();
            return true;
        }

        bool _noverb = true;
        /// <summary>
        /// enable/disable extended debugging
        /// </summary>
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }

        void v(string msg)
        {
            if (_noverb) return;
            debug(msg);
        }


        TLClient getsearchclient()
        {
            if (!IPUtil.hasValidAddress(_servers))
            {
                TLClient tmp = new TLClient_WM(false);
                tmp.VerboseDebugging = VerboseDebugging;
                tmp.SendDebugEvent+=new DebugDelegate(debug);
                return tmp;
            }
            else
            {
                TLClient_IP tmp = new TLClient_IP(_servers, _port);
                tmp.VerboseDebugging = VerboseDebugging;
                tmp.SendDebugEvent+=new DebugDelegate(debug);
                return tmp;
            }
        }

        TLClient getrealclient(int pidx, string name)
        {
            if (!IPUtil.hasValidAddress(_servers))
                return new TLClient_WM(pidx, name, false);
            else
            {
                TLClient_IP tmp = new TLClient_IP(TLClient_IP.GetEndpoints(_port, _servers), pidx, name, 3, 10, debug);
                tmp.VerboseDebugging = VerboseDebugging;
                return tmp;
            }
        }

        public bool ModifyBroker(int provider) { return ModifyBroker(provider, true); }
        public bool ModifyBroker(int provider, bool warn)
        {
            if (!feedready) return false;
            TLClient tl = getsearchclient();
            if ((provider < 0) || (provider > ProvidersAvailable.Length)) return false;
            Providers p = ProvidersAvailable[provider];
            bool ok = hasminexec(tl, provider);
            if (!ok)
            {
                System.Windows.Forms.MessageBox.Show(p.ToString() + " does not support execution.");
                return false;
            }
            this.v(tl.Name + " " + provider + " " + tl.BrokerName + (ok ? " has feed support." : " no feed support."));
            _broker = p;
            tl.Disconnect();
            Reset();
            return true;
        }

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

    }
}

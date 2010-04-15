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
        public bool isPreferredFeed { get { return _isprefq; } }
        public bool isPreferredBroker { get { return _isprefx; } }
        public TLClient FeedClient { get { return quote; } set { quote = value; }  }
        public TLClient BrokerClient { get { return execute; } set { execute = value; } }
        public Providers Feed { get { return _feed; } }
        public Providers Broker { get { return _broker; } }
        public bool RequirePreferred { get { return !_reqpref; } }
        bool _threadsafe = true;
        public bool isThreadSafe { get { return _threadsafe; } }
        public BrokerFeed() : this(Providers.Unknown, Providers.Unknown, true,false) { }
        Thread _reader;
        bool _readergo = true;

        public int ServerVersion { get { return 0; } }
        public Providers BrokerName { get { return Providers.Unknown; } }

        public BrokerFeed(Providers feed, Providers broker, bool useany, bool threadsafe) : this(feed, broker, useany, threadsafe, "BrokerFeed") { }
        public BrokerFeed(Providers feed, Providers broker, bool useany, bool threadsafe,string program)
        {
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
        public int SafeThreadInterrupts { get { return _interrupts; } }

        public void Subscribe(Basket b)
        {
            if (quote == null) return;
            quote.Subscribe(b);
        }

        public void Unsubscribe()
        {
            if (quote != null)
                quote.Unsubscribe();
        }

        public void Register()
        {
            if (quote != null)
                quote.Register();
            if (execute != null)
                execute.Register();
        }

        public long TLSend(MessageTypes type, string message)
        {
            for (int i = 0; i < _pcon.Count; i++)
                _pcon[i].TLSend(type, message);
            return 0;
        }

        public void Disconnect()
        {
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
            execute.CancelOrder(id);
        }

        public int SendOrder(Order o)
        {
            if (execute == null) return (int)MessageTypes.BROKERSERVER_NOT_FOUND;
            return execute.SendOrder(o);
        }

        public bool Mode(int provider, bool warn) { return true;  }

        public void Stop()
        {
            try
            {
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

        public void Reset()
        {

            feedready = false;
            TLClient_WM tl = new TLClient_WM(false);
            _pavail = tl.ProvidersAvailable;

            bool setquote = false;
            bool setexec = false;

            // see if we can get preferred providers
            int xi = getproviderindex(_broker);
            int qi = getproviderindex(_feed);
            _isprefq = qi != -1;
            _isprefx = xi != -1;
            if (!isPreferredFeed)
                debug("preferred data not available: " + _feed);
            if (!isPreferredBroker)
                debug("preferred execute not available: " + _broker);
            // see if we're allowed to fallback

            // not allowed
            if (RequirePreferred)
            {
                setquote = isPreferredFeed && hasminquote(tl, qi);
                setexec = isPreferredBroker && hasminexec(tl, xi);
            }
            else // ok to fallback,but where
            {
                // see if we need to search
                for (int i = 0; i < ProvidersAvailable.Length; i++)
                {
                    if ((qi != -1) && (xi != -1)) break;
                    // switch to provider
                    if ((qi == -1) && hasminquote(tl, i))
                        qi = i;
                    if ((xi == -1) && hasminexec(tl, i))
                        xi = i;
                }
                setquote = (qi != -1) && hasminquote(tl, qi);
                setexec = (xi != -1) && hasminexec(tl, xi);
            }

            // map handlers
            if (setquote)
            {
                quote = new TLClient_WM(qi, PROGRAM + "quote", false);
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

            }
            if (setexec)
            {
                execute = new TLClient_WM(xi, PROGRAM + "exec", false);
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
                if ((i == xi) || (i == qi)) continue;
                // add new connections
                TLClient newcon = new TLClient_WM(i, PROGRAM, false);
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
            if (RequestPositionsOnAccounts)
                RequestPositions(msg);
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
                long r = execute.TLSend(MessageTypes.POSITIONREQUEST, BrokerClient.Name + "+" + acct);
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
                long r = execute.TLSend(MessageTypes.ACCOUNTREQUEST, Name);
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

        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);

        }


        public bool isProviderAvailable(Providers p)
        {
            int idx = getproviderindex(p);
            return idx != -1;
        }
        public bool isFeedConnected { get { return quote != null; } }
        public bool isBrokerConnected { get { return execute != null; } }

        int getproviderindex(Providers p)
        {
            if (p == Providers.Unknown) return -1;
            for (int i = 0; i < ProvidersAvailable.Length; i++)
                if (ProvidersAvailable[i] == p) return i;
            return -1;
        }

        static bool hasminquote(TLClient_WM tl, int provider)
        {
            bool v = tl.Mode(provider, false);
            bool test = true;
            test &= tl.RequestFeatureList.Contains(MessageTypes.TICKNOTIFY);
            tl.Disconnect();
            return test && v;
        }

        static bool hasminexec(TLClient_WM tl, int provider)
        {
            bool v = tl.Mode(provider, false);
            bool test = true;
            test &= tl.RequestFeatureList.Contains(MessageTypes.EXECUTENOTIFY);
            test &= tl.RequestFeatureList.Contains(MessageTypes.SENDORDER);
            test &= tl.RequestFeatureList.Contains(MessageTypes.ORDERCANCELREQUEST);
            test &= tl.RequestFeatureList.Contains(MessageTypes.ORDERCANCELRESPONSE);
            tl.Disconnect();
            return test && v;
        }

        public bool ModifyFeed(int provider) { return ModifyFeed(provider, true); }
        public bool ModifyFeed(int provider, bool warn)
        {
            if (!feedready) return false;
            TLClient_WM tl = new TLClient_WM(false);
            if ((provider < 0) || (provider > ProvidersAvailable.Length)) return false;
            Providers p = ProvidersAvailable[provider];
            if (!hasminquote(tl, provider))
            {
                System.Windows.Forms.MessageBox.Show(p.ToString() + " does not support quotes.");
                return false;
            }
            tl.Disconnect();
            Reset();
            return true;
        }

        public bool ModifyBroker(int provider) { return ModifyBroker(provider, true); }
        public bool ModifyBroker(int provider, bool warn)
        {
            if (!feedready) return false;
            TLClient_WM tl = new TLClient_WM(false);
            if ((provider < 0) || (provider > ProvidersAvailable.Length)) return false;
            Providers p = ProvidersAvailable[provider];
            if (!hasminexec(tl, provider))
            {
                System.Windows.Forms.MessageBox.Show(p.ToString() + " does not support execution.");
                return false;
            }
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
        public event DebugDelegate SendDebug;
        public event DebugDelegate gotServerUp;
        public event DebugDelegate gotServerDown;

    }
}

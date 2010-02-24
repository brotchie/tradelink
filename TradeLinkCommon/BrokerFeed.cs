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
        public TLClient FeedClient { get { return quote; } set { quote = value; }  }
        public TLClient BrokerClient { get { return execute; } set { execute = value; } }
        public Providers Feed { get { return _feed; } }
        public Providers Broker { get { return _broker; } }
        public bool RequirePreferred { get { return !_reqpref; } }
        bool _threadsafe = true;
        public bool isThreadSafe { get { return _threadsafe; } }
        public BrokerFeed() : this(Providers.Unknown, Providers.Unknown, true,true) { }
        Thread _reader;
        bool _readergo = true;

        public int ServerVersion { get { return 0; } }
        public Providers BrokerName { get { return Providers.Unknown; } }

        public BrokerFeed(Providers feed, Providers broker, bool useany, bool threadsafe)
        {
            _feed = feed;
            _broker = broker;
            _reqpref = useany;
            _threadsafe = threadsafe;
            if (_threadsafe)
            {
                _reader = new Thread(new ParameterizedThreadStart(readdata));
            }
        }

        // thread-safe buffers
        RingBuffer<Tick> _kbuf = new RingBuffer<Tick>(10000);
        RingBuffer<Order> _obuff = new RingBuffer<Order>(1000);
        RingBuffer<Trade> _fbuff = new RingBuffer<Trade>(100);
        RingBuffer<uint> _cbuff = new RingBuffer<uint>(1000);
        RingBuffer<Position> _pbuff = new RingBuffer<Position>(100);
        RingBuffer<string> _abuff = new RingBuffer<string>(10);
        RingBuffer<GenericMessage> _mbuff = new RingBuffer<GenericMessage>(500);
        RingBuffer<GenericMessage> _mqbuff = new RingBuffer<GenericMessage>(500);

        void readdata(object obj)
        {
            while (_readergo)
            {
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
                    uint c = _cbuff.Read();
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
        }

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
            return 0;
        }

        public void Disconnect()
        {
            if (quote!=null)
                quote.Disconnect();
            if (execute != null)
                execute.Disconnect();
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

        public Providers[] _ProvidersAvailable = new Providers[0];
        public Providers[] ProvidersAvailable { get { return _ProvidersAvailable; } }
        public int ProviderSelected { get { return -1; } }
        public List<MessageTypes> RequestFeatureList { get { return _RequestFeaturesList; } }
        public void ProvidersUpdate()
        {
            TLClient_WM tl = new TLClient_WM(false);
            _ProvidersAvailable = tl.ProvidersAvailable;
            tl.Disconnect();
        }

        public void Reset()
        {

            feedready = false;
            TLClient_WM tl = new TLClient_WM(false);
            _ProvidersAvailable = tl.ProvidersAvailable;

            bool setquote = false;
            bool setexec = false;

            // see if we can get preferred providers
            int xi = getproviderindex(_broker);
            int qi = getproviderindex(_feed);
            bool prefq = qi != -1;
            bool prefx = xi != -1;
            if (!prefq)
                debug("preferred quote not available: " + _feed);
            if (!prefx)
                debug("preferred execute not available: " + _broker);
            // see if we're allowed to fallback

            // not allowed
            if (RequirePreferred)
            {
                setquote = prefq && hasminquote(tl, qi);
                setexec = prefx && hasminexec(tl, xi);
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
                // clear any leftover subscriptions
                quote.Unsubscribe();
                if (isThreadSafe)
                {
                    quote.gotTick += new TickDelegate(quote_gotTick);
                    quote.gotUnknownMessage += new MessageDelegate(quote_gotUnknownMessage);
                }

            }
            if (setexec)
            {
                execute = new TLClient_WM(xi, PROGRAM + "exec", false);
                execute.gotFeatures += new MessageTypesMsgDelegate(execute_gotFeatures);
                if (isThreadSafe)
                {
                    execute.gotAccounts += new DebugDelegate(execute_gotAccounts);
                    execute.gotFill += new FillDelegate(execute_gotFill);
                    execute.gotOrder += new OrderDelegate(execute_gotOrder);
                    execute.gotOrderCancel += new UIntDelegate(execute_gotOrderCancel);
                    execute.gotPosition += new PositionDelegate(execute_gotPosition);
                    execute.gotUnknownMessage += new MessageDelegate(execute_gotUnknownMessage);

                }
                debug("Executions: " + execute.BrokerName + " " + execute.ServerVersion);
            }

            feedready = true;
            tl.Disconnect();
            tl = null;
        }

        void quote_gotFeatures(MessageTypes[] messages)
        {
            _RequestFeaturesList.AddRange(messages);
        }

        void execute_gotFeatures(MessageTypes[] messages)
        {
            _RequestFeaturesList.AddRange(messages);
        }

        public List<MessageTypes> _RequestFeaturesList = new List<MessageTypes>();

        public void RequestFeatures()
        {
            _RequestFeaturesList.Clear();
            if (quote != null)
                quote.RequestFeatures();
            if (execute != null)
                execute.RequestFeatures();
        }

        void quote_gotUnknownMessage(MessageTypes type, uint source, uint dest, uint msgid, string request, ref string response)
        {
            _mqbuff.Write(new GenericMessage(type, source, dest, msgid, request, response));
        }

        void execute_gotUnknownMessage(MessageTypes type, uint source, uint dest, uint msgid, string request, ref string response)
        {
            _mbuff.Write(new GenericMessage(type, source, dest, msgid, request, response));

        }

        void execute_gotPosition(Position pos)
        {
            _pbuff.Write(pos);
        }

        void execute_gotOrderCancel(uint number)
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

        void execute_gotAccounts(string msg)
        {
            _abuff.Write(msg);
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
            if (p == null) return -1;
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
        public event UIntDelegate gotOrderCancel;
        public event MessageTypesMsgDelegate gotFeatures;
        public event PositionDelegate gotPosition;
        public event ImbalanceDelegate gotImbalance;
        public event MessageDelegate gotUnknownMessage;
        public event DebugDelegate SendDebug;
        public event DebugDelegate gotServerUp;
        public event DebugDelegate gotServerDown;

    }
}

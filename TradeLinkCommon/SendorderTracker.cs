using System;
using System.Collections.Generic;
using TradeLink.Common;
using TradeLink.API;
using System.Threading;
using System.ComponentModel;

namespace TradeLink.Common
{
    public delegate void OrderId_Status(Order o, int stat, Providers p, int conid);
    public class SendorderTracker
    {
        TLClient _tl;
        int _estsymcount = 200;
        /// <summary>
        /// provide the estimated # of symbols and the client to use
        /// </summary>
        /// <param name="tl"></param>
        /// <param name="estSymbols"></param>
        public SendorderTracker(TLClient tl, int estSymbols)
        {
            _tl = tl;
            _estsymcount = estSymbols;
            _ordersend.DoWork += new DoWorkEventHandler(_ordersend_DoWork);
            _loaded = new GenericTracker<bool>(estSymbols);
            _loaded.NewTxt += new TextIdxDelegate(_loaded_NewTxt);
            _resendq = new RingBuffer<Order>(estSymbols);
            _orderq = new RingBuffer<Order>(estSymbols);
            _retries = new GenericTracker<int>(estSymbols * 10);
        }

        int _defaultmaxretries = 0;
        public int MaxRetries { get { return _defaultmaxretries; } set { _defaultmaxretries = value; } }


        void _loaded_NewTxt(string txt, int idx)
        {

        }

        GenericTracker<int> _retries;
        int _SLEEP = 100;
        /// <summary>
        /// Milliseconds to wait after all items have been processed, before processing new items
        /// </summary>
        public int Wait { get { return _SLEEP; } set { _SLEEP = value; } }
        int _SUBSCRIBEPAUSE = 250;
        /// <summary>
        /// milliseconds to pause after subscribing to a new symbol
        /// (assuming SubscribeSymbol is enabled)
        /// </summary>
        public int SubscribePause { get { return _SUBSCRIBEPAUSE; } set { _SUBSCRIBEPAUSE = value; } }
        /// <summary>
        /// start the tracker, required to process orders
        /// </summary>
        public void Start()
        {
            if ((_tl == null) || (_tl.ProvidersAvailable.Length == 0) || (_tl.ProviderSelected == -1))
            {
                debug("Broker not connected. Start failed.");
                return;
            }
            if (_tl.BrokerName == Providers.Unknown)
            {

                debug("Unknown broker, not able to send orders.");
                _tl = null;
            }
            if (!_tl.RequestFeatureList.Contains(MessageTypes.SENDORDER))
            {
                debug("broker " + _tl.BrokerName + " " + _tl.ServerVersion + " does not support sending orders.");
                _tl = null;
            }

            if (!_ordersend.IsBusy)
            {
                _processorderq = true;

                _ordersend.RunWorkerAsync();
                debug("Starting sendorder tracker on: " + _tl.BrokerName + " connection: " + _tl.Name + " id: " + _tl.ProviderSelected);
            }
            else
                debug("Sendorder tracker is already running.");
        }
        /// <summary>
        /// stop the tracker, required upon closing application
        /// </summary>
        public void Stop()
        {
            debug("Stopping sendorder tracker.");
            _processorderq = false;
        }
        GenericTracker<bool> _loaded;

        bool _subscribefirst = true;
        /// <summary>
        /// auto-subscribe to symbols before sending orders
        /// </summary>
        public bool SubscribeBeforeSend { get { return _subscribefirst; } set { _subscribefirst = value; } }
        bool _resenderror = true;
        /// <summary>
        /// resend order on any error
        /// </summary>
        public bool ResendOnError { get { return _resenderror; } set { _resenderror = value; _resenderrorselect = !value; } }
        bool _resenderrorselect = false;
        /// <summary>
        /// resend only on selected errors
        /// </summary>
        public bool ResendOnSelectError { get { return _resenderrorselect; } set { _resenderrorselect = value; _resenderror = !value; } }
        List<int> _selerr = new List<int>();
        /// <summary>
        /// specify a list of error codes whereby orders will quality to be resent
        /// </summary>
        public List<int> SelectedErrors { get { return _selerr; } set { _selerr = value; } }
        /// <summary>
        /// send order and track it
        /// </summary>
        /// <param name="o"></param>
        public void sendorder(Order o)
        {
            // ensure thread is running
            if (!_processorderq)
            {
                debug("Sendorder tracker has not been Started, orders will not be sent.");
            }
            //get index
            int idx = _loaded.getindex(o.symbol);
            if (idx == GenericTracker.UNKNOWN)
            {
                // add index
                _loaded.addindex(o.symbol, false);
                // first time for this symbol, subscribe
                if (SubscribeBeforeSend && subscribe(o.symbol))
                {
                    debug(o.symbol + " not subscribed, subscribing.");
                }
            }
            if (o.id == 0)
            {
                debug("No id, disabling max retry checking: " + o.ToString());
            }
            else
            {
                int oidx = _retries.addindex(o.id.ToString(), 0);
            }
            debug(o.symbol + " tracking order: " + o.ToString());
            // queoe order
            _orderq.Write(o);


        }
        /// <summary>
        /// receive debug notifications from this component
        /// </summary>
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        Basket _mb = new BasketImpl();
        /// <summary>
        /// get or set the current market basket of this component
        /// </summary>
        Basket MarketBasket { get { return _mb; } set { _mb = value; } }
        bool subscribe(string sym)
        {
            if (!subscribed(sym))
            {
                _mb.Add(sym);
                _tl.Subscribe(_mb);
                return true;
            }
            return false;
        }

        bool subscribed(string sym)
        {
            foreach (Security s in _mb)
                if (s.Symbol == sym) return true;
            return false;
        }

        /// <summary>
        /// sends status of an order
        /// </summary>
        public event OrderId_Status OrderIdStatusEvent;


        int sendordernow(Order o)
        {
            if (_tl == null) return (int)MessageTypes.BROKERSERVER_NOT_FOUND;
            int err = _tl.SendOrder(o);
            if (err != 0)
            {
                debug(o.symbol + " order err: " + err + " " + Util.PrettyError(_tl.BrokerName, err));
            }
            else
            {
                _loaded[o.symbol] = true;
                debug(o.symbol + " order sent: " + o.ToString());
            }

            if ((o.id != 0) && (OrderIdStatusEvent != null))
                OrderIdStatusEvent(o, err, _tl.BrokerName, _tl.ProviderSelected);


            return err;
        }

        void _ordersend_DoWork(object sender, DoWorkEventArgs e)
        {
            // process queue on seperate thread
            while (_processorderq)
            {
                // process not yet sent, add to resend
                while (_orderq.hasItems)
                    _resendq.Write(_orderq.Read());
                // prepare new resends
                List<Order> add2resend = new List<Order>(_estsymcount);
                // process resends
                while (_resendq.hasItems)
                {
                    Order o = _resendq.Read();
                    // if symbol is not loaded and subscribe is requested, wait
                    if (SubscribeBeforeSend && !_loaded[o.symbol])
                        Thread.Sleep(_SUBSCRIBEPAUSE);
                    // get retry index
                    int oidx = _retries.getindex(o.id.ToString());
                    // get maximum retry
                    int maxretry = MaxRetries == 0 ? int.MaxValue : MaxRetries;
                    int tries = oidx < 0 ? 0 : _retries[oidx];
                    if (tries++ < maxretry)
                    {
                        // send
                        int err = sendordernow(o);
                        // if error, add to resend
                        if ((err != 0) && ResendOnError)
                        {
                            debug(o.symbol + " queing for resend " + o.id);
                            add2resend.Add(o);
                        }
                        else if ((err != 0) && ResendOnSelectError && SelectedErrors.Contains(err))
                        {
                            debug(o.symbol + " queing for resend " + o.id);
                            add2resend.Add(o);
                        }
                        else if (err != 0)
                        {
                            debug(o.symbol + " no re-send requested.");
                        }
                    }
                    if (oidx >= 0)
                        _retries[oidx] = tries;
                    if (_resendq.hasItems)
                        Thread.Sleep(_WAITITEM);

                }
                // add new resends
                foreach (Order o in add2resend)
                    _resendq.Write(o);

                Thread.Sleep(_SLEEP);
            }
        }

        int _WAITITEM = 0;
        /// <summary>
        /// Milliseconds to wait between processing each queue item
        /// </summary>
        public int WaitItem { get { return _WAITITEM; } set { _WAITITEM = value; } }
        BackgroundWorker _ordersend = new BackgroundWorker();
        // unsent
        RingBuffer<Order> _orderq;
        // resend
        RingBuffer<Order> _resendq;
        bool _processorderq = true;
    }
}

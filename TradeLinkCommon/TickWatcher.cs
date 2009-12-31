using System;
using System.Collections.Generic;
using System.Collections;
using TradeLink.API;
using System.Threading;

namespace TradeLink.Common
{
    /// <summary>
    /// Used to watch a stream of ticks, and send alerts when the stream goes idle for a specified time.
    /// </summary>
    public class TickWatcher : TickIndicator
    {
        private int _defaultwait = 60;
        private bool _alertonfirst = true;
        /// <summary>
        ///  returns count of symbols that have ticked at least once
        /// </summary>
        public int Count { get { return _last.Count; } }
        /// <summary>
        /// gets last time a tick was received for symbol
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public int this[string sym]
        {
            get
            {
                int lasttime = 0;
                if (_last.TryGetValue(sym, out lasttime))
                    return lasttime;
                return 0;
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (string s in _last.Keys)
                yield return s;
        }
        private Dictionary<string, int> _last = new Dictionary<string, int>();
        /// <summary>
        /// alert thrown when no ticks have arrived since AlertThreshold.
        /// Time of last tick is provided.
        /// </summary>
        public event Int32Delegate GotMassAlert;
        /// <summary>
        /// alert thrown when AlertThreshold is exceeded for a symbol
        /// </summary>
        public event SymDelegate GotAlert;
        /// <summary>
        /// alert thrown when first tick arrives for symbol
        /// </summary>
        public event SymDelegate GotFirstTick;
        public bool FireFirstTick { get { return _alertonfirst; } set { _alertonfirst = value; } }
        /// <summary>
        /// minimum threshold in seconds when no tick updates have been received, alerts can be thrown.
        /// </summary>
        public int AlertThreshold { get { return _defaultwait; } set { _defaultwait = value; } }

        Thread _bw = null;
        volatile int _lasttime = 0;
        /// <summary>
        /// most recent time received
        /// </summary>
        public int RecentTime { get { return _lasttime; } }
        /// <summary>
        /// Watches the specified tick.
        /// Alerts if wait time exceeded.
        /// </summary>
        /// <param name="tick">The tick.</param>
        /// <returns></returns>
        public bool newTick(Tick tick) 
        {
            _lasttime = tick.time;
            if ((GotAlert != null) || (GotFirstTick != null))
            {
                int last = tick.time;
                // ensure we are storing per-symbol times
                if (!_last.TryGetValue(tick.symbol, out last))
                {
                    _last.Add(tick.symbol, tick.time);
                    if (_alertonfirst) // if we're notifying when first tick arrives, do it.
                        if (GotFirstTick != null)
                            GotFirstTick(tick.symbol);
                    last = tick.time;
                    return false;
                }
                // if alerts requested, check for idle symbol
                if (GotAlert != null)
                {
                    int span = Util.FTDIFF(last, tick.time);
                    bool alert = span > _defaultwait;
                    if (alert)
                        GotAlert(tick.symbol);
                    return alert;
                }
                // store time
                _last[tick.symbol] = tick.time;
            }
            return false; 
        }
        /// <summary>
        /// send alerts for idle symbols using current time as comparison point
        /// </summary>
        /// <returns></returns>
        public int SendAlerts() { return SendAlerts(DateTime.Now); }
        /// <summary>
        /// Sends the alerts for tickstreams who have gone idle based on the provided datetime.
        /// </summary>
        /// <param name="date">The current datetime.</param>
        public int SendAlerts(DateTime time)
        {
            return SendAlerts(Util.DT2FT(time), _defaultwait);
        }
        /// <summary>
        /// sends alerts for i
        /// </summary>
        /// <param name="date"></param>
        public int SendAlerts(int time)
        {
            return SendAlerts(time, _defaultwait);
        }

        /// <summary>
        /// Sends the alerts for tickstreams who have gone idle based on the provided datetime.
        /// </summary>
        /// <param name="date">The datetime.</param>
        /// <param name="AlertSecondsWithoutTick">The alert seconds without tick.</param>
        public int SendAlerts(int time, int AlertSecondsWithoutTick) 
        {
            int c = 0;
            foreach (string sym in _last.Keys)
                if (GotAlert != null)
                    if (Util.FTDIFF(_last[sym], time) > AlertSecondsWithoutTick)
                    {
                        c++;
                        GotAlert(sym);
                    }
            return c;
        }

        bool _continue = true;
        void backgroundpoll()
        {
            while (_continue)
            {
                if (PollProcess != null)
                    PollProcess(_lasttime);
                if ((GotMassAlert != null) && (_defaultwait!=0))
                {
                    int span = Util.FTDIFF(_lasttime, Util.DT2FT(DateTime.Now));
                    bool alert = span > _defaultwait;
                    if (alert)
                        GotMassAlert(_lasttime);
                }
                System.Threading.Thread.Sleep(_pollint);
            }
        }
        public event Int32Delegate PollProcess;
        volatile int _pollint = 0;
        public int BackgroundPollInterval { get { return _pollint; } set { _pollint = value; if (_pollint == 0) Stop(); } }
        public TickWatcher() : this(1000) { }
        /// <summary>
        /// creates a tickwatcher and polls specificed millseconds
        /// if timer has expired, sends alert.
        /// Background polling occurs in addition to tick-induced time checks.
        /// </summary>
        /// <param name="BackgroundPollIntervalms">Value in millseconds to wait between checks.  0 = disable background checks</param>
        public TickWatcher(int BackgroundPollIntervalms)
        {
            _pollint = BackgroundPollIntervalms;
            if (_pollint != 0)
                Start();
        }

        public void Start()
        {
            if ((_bw ==null) && (_pollint!=0))
            {
                _bw = new Thread(this.backgroundpoll);
                _bw.Start();
            }
            else if ((_bw!=null) && (_bw.ThreadState== ThreadState.Stopped))
            {
                _bw.Start();
            }
        }

        public void Stop()
        {
            // flag to stop
            _continue = false;
            // wait a moment 
            System.Threading.Thread.Sleep(100);
            // if it hasn't stoppped yet, force stop
            if ((_bw != null) && (_bw.ThreadState != ThreadState.Stopped)
                && (_bw.ThreadState != ThreadState.StopRequested))
            {
                try
                {
                    _bw.Abort();
                    _bw = null;
                }
                catch { }
            }
        }

    }
}

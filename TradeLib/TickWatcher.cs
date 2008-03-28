using System;
using System.Collections.Generic;

namespace TradeLib
{
    /// <summary>
    /// Used to watch a stream of ticks, and send alerts when the stream goes idle for a specified time.
    /// </summary>
    public class TickWatcher
    {
        private int _defaultwait = 60;
        private bool _alertonfirst = true;
        private Dictionary<string, long> _last = new Dictionary<string, long>();
        public event StockDelegate Alerted;
        public event StockDelegate FirstTick;
        public bool FireFirstTick { get { return _alertonfirst; } set { _alertonfirst = value; } }
        public int DefaultWait { get { return _defaultwait; } set { _defaultwait = value; } }
        /// <summary>
        /// Watches the specified tick.
        /// </summary>
        /// <param name="tick">The tick.</param>
        /// <returns></returns>
        public bool Watch(Tick tick) { return Watch(tick, _defaultwait); }
        public bool Watch(Tick tick, int NoTicksAlertWait) 
        {
            long last = Util.ToDateTime(tick.date, tick.time, tick.sec).Ticks;
            if (!_last.ContainsKey(tick.sym))
            {
                _last.Add(tick.sym, last);
                if (_alertonfirst) // if we're notifying when first tick arrives, do it.
                    if (FirstTick != null) 
                        FirstTick(new Stock(tick.sym, Util.ToTLDate(_last[tick.sym])));
                return true;
            }
            TimeSpan span = new TimeSpan(Util.ToDateTime(tick.date, tick.time, tick.sec).Ticks - _last[tick.sym]);
            bool alert = span.TotalSeconds>NoTicksAlertWait;
            _last[tick.sym] = last;
            if (alert && (Alerted!=null)) Alerted(new Stock(tick.sym,tick.date));
            return !alert;
        }
        /// <summary>
        /// Sends the alerts for tickstreams who have gone idle based on the provided datetime.
        /// </summary>
        /// <param name="date">The current datetime.</param>
        public void SendAlerts(DateTime date)
        {
            SendAlerts(date, DefaultWait);
        }
        /// <summary>
        /// Sends the alerts for tickstreams who have gone idle based on the provided datetime.
        /// </summary>
        /// <param name="date">The datetime.</param>
        /// <param name="AlertSecondsWithoutTick">The alert seconds without tick.</param>
        public void SendAlerts(DateTime date, int AlertSecondsWithoutTick) 
        { 
            foreach (string sym in _last.Keys)
                if (Alerted!=null)
                    if ((new TimeSpan(date.Ticks - _last[sym])).TotalSeconds>AlertSecondsWithoutTick)
                        Alerted(new Stock(sym,Util.ToTLDate(_last[sym])));
        }



    }
}

using System;
using System.Collections.Generic;

namespace TradeLib
{
    public class TickWatcher
    {
        private int _defaultwait = 300;
        private bool _alertonfirst = true;
        private Dictionary<string, long> _last = new Dictionary<string, long>();
        public event StockDelegate Alerted;
        public event StockDelegate FirstTick;
        public bool FireFirstTick { get { return _alertonfirst; } set { _alertonfirst = value; } }
        public int DefaultWait { get { return _defaultwait; } set { _defaultwait = value; } }
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
        public void SendAlerts(DateTime date)
        {
            SendAlerts(date, DefaultWait);
        }
        public void SendAlerts(DateTime date, int AlertSecondsWithoutTick) 
        { 
            foreach (string sym in _last.Keys)
                if (Alerted!=null)
                    if ((new TimeSpan(date.Ticks - _last[sym])).TotalSeconds>AlertSecondsWithoutTick)
                        Alerted(new Stock(sym,Util.ToTLDate(_last[sym])));
        }



    }
}

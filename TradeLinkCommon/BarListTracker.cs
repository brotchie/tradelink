using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    public class BarListTracker
    {
        public event SymBarIntervalDelegate GotNewBar;
        /// <summary>
        /// create a barlist tracker with all the intervals available
        /// (specify only intervals you need to get faster performance)
        /// </summary>
        public BarListTracker() : this(BarListImpl.ALLINTERVALS) { }
        public BarListTracker(BarInterval interval) : this(new BarInterval[] { interval }) { }
        BarInterval _default = BarInterval.FiveMin;
        BarInterval[] _requested = new BarInterval[0];
        public BarInterval DefaultInterval { get { return _default; } set { _default = value; } }
        /// <summary>
        /// intervals requested when tracker was created
        /// </summary>
        public BarInterval[] Intervals { get { return _requested; } }
        public BarListTracker(BarInterval[] intervals)
        {
            _requested = intervals;
        }
        Dictionary<string, BarListImpl> _bdict = new Dictionary<string, BarListImpl>();
        public int SymbolCount { get { return _bdict.Count; } }
        /// <summary>
        /// gets barlist for a given symbol.   will return an invalid barlist if no ticks have been received for symbol
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public BarList this[string sym]
        {

            get
            {
                BarListImpl bl;
                if (_bdict.TryGetValue(sym, out bl))
                    return (BarList)bl;
                bl = new BarListImpl(sym,_requested);
                bl.DefaultInterval = _default;
                bl.GotNewBar += new SymBarIntervalDelegate(bl_GotNewBar);
                _bdict.Add(sym, bl);
                return bl;
            }
        }

        // pass bar events out of the barlist tracker
        void bl_GotNewBar(string symbol, BarInterval interval)
        {
            if (GotNewBar != null)
                GotNewBar(symbol, interval);
        }
        /// <summary>
        /// give any ticks to this symbol and tracker will create barlists automatically 
        /// </summary>
        /// <param name="k"></param>
        public void newTick(Tick k)
        {
            BarListImpl bl;
            if (!_bdict.TryGetValue(k.symbol, out bl))
            {
                bl = new BarListImpl(k.symbol,_requested);
                bl.DefaultInterval = _default;
                bl.GotNewBar+=new SymBarIntervalDelegate(bl_GotNewBar);
                _bdict.Add(k.symbol, bl);
            }
            bl.newTick(k);
        }
    }
}

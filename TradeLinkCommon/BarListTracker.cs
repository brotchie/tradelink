using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// used to track lists of bars for MANY symbols.
    /// BarListTracker (blt) will accept ticks and auto-create bars as barlists as needed.
    /// Access bars via blt["IBM"].RecentBar.Close
    /// </summary>
    public class BarListTracker : GotTickIndicator
    {
        public void GotTick(Tick k) { newTick(k); }
        public event SymBarIntervalDelegate GotNewBar;
        /// <summary>
        /// create a barlist tracker with all the intervals available
        /// (specify only intervals you need to get faster performance)
        /// </summary>
        public BarListTracker() : this(BarListImpl.ALLINTERVALS) { }
        public BarListTracker(BarInterval interval) : this(new BarInterval[] { interval }) { }
        int _default = (int)BarInterval.FiveMin;
        int[] _requested = new int[0];
        BarInterval[] _reqtype = new BarInterval[0];
        /// <summary>
        /// default custom interval used by this tracker
        /// </summary>
        int DefaultCustomInterval { get { return _default; } }
        /// <summary>
        /// custom bar intervals used by this tracker
        /// </summary>
        public int[] CustomIntervals { get { return _requested; } }
        public BarInterval DefaultInterval 
        { 
            get 
            { 
                return BarListImpl.Int2BarInterval(new int[] {_default})[0]; 
            } 
            set 
            { 
                _default = BarListImpl.BarInterval2Int(new BarInterval[] { value })[0];
                foreach (string sym in _bdict.Keys)
                    _bdict[sym].DefaultInterval = value;
            } 
        }
        /// <summary>
        /// intervals requested when tracker was created
        /// </summary>
        public BarInterval[] Intervals { get { return BarListImpl.Int2BarInterval(_requested); } }
        /// <summary>
        /// creates tracker for single custom interval
        /// </summary>
        /// <param name="custominterval"></param>
        public BarListTracker(int custominterval) : this(new int[] { custominterval }, new BarInterval[] { BarInterval.CustomTime }) { }
        /// <summary>
        /// creates tracker for number of custom intervals.
        /// (use this if you want to mix standard and custom intervals)
        /// </summary>
        /// <param name="customintervals"></param>
        public BarListTracker(int[] customintervals, BarInterval[] intervaltypes)
        {
            _default = customintervals[0];
            _requested = customintervals;
            _reqtype = intervaltypes;
        }
        /// <summary>
        /// creates tracker for specified number of standard intervals
        /// </summary>
        /// <param name="intervals"></param>
        public BarListTracker(BarInterval[] intervals)
        {
            _requested = BarListImpl.BarInterval2Int(intervals);
            _reqtype = intervals;
            _default = (int)intervals[0];
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
                return this[sym, _default];
            }


            set
            {
                BarListImpl bl;
                if (_bdict.TryGetValue(sym, out bl))
                    _bdict[sym] = (BarListImpl)value;
                else
                    _bdict.Add(sym, (BarListImpl)value);

            }
        }

        public BarList this[string sym, int interval]
        {
            get
            {
                BarListImpl bl;
                if (_bdict.TryGetValue(sym, out bl))
                {
                    bl.DefaultCustomInterval = interval;
                    return (BarList)bl;
                }
                bl = new BarListImpl(sym, _requested, _reqtype);
                bl.DefaultCustomInterval = interval;
                bl.GotNewBar += new SymBarIntervalDelegate(bl_GotNewBar);
                _bdict.Add(sym, bl);
                return bl;
            }

        }

        /// <summary>
        /// clears all data from tracker
        /// </summary>
        public void Reset()
        {
            foreach (BarList bl in _bdict.Values)
                bl.Reset();
            _bdict.Clear();
        }

        public IEnumerator GetEnumerator() { foreach (string sym in _bdict.Keys) yield return sym; }

        // pass bar events out of the barlist tracker
        void bl_GotNewBar(string symbol, int interval)
        {
            if (GotNewBar != null)
                GotNewBar(symbol, interval);
        }
        /// <summary>
        /// give any ticks (trades) to this symbol and tracker will create barlists automatically 
        /// </summary>
        /// <param name="k"></param>
        public void newTick(Tick k)
        {
            BarListImpl bl;
            if (!_bdict.TryGetValue(k.symbol, out bl))
            {
                bl = new BarListImpl(k.symbol,_requested,_reqtype);
                bl.DefaultCustomInterval = _default;
                bl.GotNewBar+=new SymBarIntervalDelegate(bl_GotNewBar);
                _bdict.Add(k.symbol, bl);
            }
            bl.newTick(k);
        }
        /// <summary>
        /// add any data point to bar
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="p"></param>
        /// <param name="time"></param>
        /// <param name="date"></param>
        /// <param name="size"></param>
        public void newPoint(string symbol, decimal p, int time, int date, int size)
        {
            BarListImpl bl;
            if (!_bdict.TryGetValue(symbol, out bl))
            {
                bl = new BarListImpl(symbol, _requested, _reqtype);
                bl.DefaultCustomInterval = _default;
                bl.GotNewBar += new SymBarIntervalDelegate(bl_GotNewBar);
                _bdict.Add(symbol, bl);
            }
            bl.newPoint(symbol,p, time, date, size);
        }

        /// <summary>
        /// build bar with bid data rather than trades
        /// </summary>
        /// <param name="k"></param>
        public void newBid(Tick k)
        {
            if (!k.hasBid) return;
            newPoint(k.symbol, k.bid, k.time, k.date, k.BidSize);
        }
        /// <summary>
        /// build bar with ask data rather than trades
        /// </summary>
        /// <param name="k"></param>
        public void newAsk(Tick k)
        {
            if (!k.hasAsk) return;
            newPoint(k.symbol, k.ask, k.time, k.date, k.AskSize);
        }
    }
}

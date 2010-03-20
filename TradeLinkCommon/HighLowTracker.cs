using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.Common
{
    /// <summary>
    /// track highs and lows
    /// </summary>
    public class HighLowTracker
    {
        GenericTracker<decimal> _highs;
        GenericTracker<decimal> _lows;
        /// <summary>
        /// tracks highs and lows
        /// </summary>
        public HighLowTracker() : this(50) { }
        /// <summary>
        /// tracks highs and lows for approx # of symbols
        /// </summary>
        /// <param name="estSymbols"></param>
        public HighLowTracker(int estSymbols)
        {
            _highs = new GenericTracker<decimal>(estSymbols);
            _lows = new GenericTracker<decimal>(estSymbols);
        }
        /// <summary>
        /// create new high or low index 
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="idx"></param>
        public void NewTxt(string txt, int idx)
        {
            _highs.addindex(txt, decimal.MinValue);
            _lows.addindex(txt, decimal.MaxValue);
        }



        /// <summary>
        /// get high
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public decimal High(string sym) 
        { 
            return _highs[sym]; 
        }
        /// <summary>
        /// get low
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public decimal Low(string sym) 
        { 
            return _lows[sym]; 
        }
        /// <summary>
        /// set a new high
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="high"></param>
        public void newHigh(string sym, decimal high) 
        { 
            _highs[sym] = high; 
        }
        /// <summary>
        /// set a new low
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="low"></param>
        public void newLow(string sym, decimal low) 
        { 
            _lows[sym] = low; 
        }
        /// <summary>
        /// set high/low from tick, return true if new high or low was reached
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool newTick(Tick k)
        {
            if (!k.isTrade) return false;
            return newPoint(k.symbol,k.trade );
        }
        /// <summary>
        /// sets high/low from tick, given an index
        /// </summary>
        /// <param name="k"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool newTick(Tick k, int idx)
        {
            if (!k.isTrade) return false;
            return newPoint(idx, k.trade);
        }
        /// <summary>
        /// set high/low from a point
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool newPoint(string sym,decimal p)
        {
            int idx = _highs.getindex(sym);
            if (idx == GenericTracker.UNKNOWN)
            {
                NewTxt(sym, 0);
                idx = _highs.Count - 1;
            }
            return newPoint(idx,p);
        }
        /// <summary>
        /// set high low for a point given an index
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool newPoint(int idx, decimal p)
        {
            bool v = false;
            if (p > _highs[idx])
            {
                _highs[idx] = p;
                v |= true;
            }
            if (p < _lows[idx])
            {
                _lows[idx] = p;
                v |= true;
            }
            return v;
            
        }
        /// <summary>
        /// clear all highs and lows
        /// </summary>
        public void Reset()
        {
            _highs.Clear();
            _lows.Clear();
        }

    }
}

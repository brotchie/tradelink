using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.Common
{
    /// <summary>
    /// track highs
    /// </summary>
    public class HighTracker : GenericTracker<decimal>, GotTickIndicator, GenericTrackerDecimal
    {
        public void GotTick(Tick k) { newTick(k); }
        public decimal getvalue(int idx) { return this[idx]; }
        public decimal getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, decimal v) { this[idx] = v; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="estSymbols"></param>
        /// <param name="name"></param>
        public HighTracker(int estSymbols,string name) : base(estSymbols,name)
        {
        }

        public HighTracker(int estSymbols) : base(estSymbols, "HIGH") { }
        public HighTracker(string name) : base(100, name) { }
        public HighTracker() : this(100, "HIGH") { }

        /// <summary>
        /// set high/low from tick, return true if new high or low was reached
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool newTick(Tick k)
        {
            if (!k.isTrade) return false;
            return newPoint(k.symbol, k.trade);
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
        /// set low from a point
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool newPoint(string sym, decimal p)
        {
            int idx = getindex(sym);
            if (idx<0)
                idx = addindex(sym, decimal.MinValue);
            return newPoint(idx, p);
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

            if (p > this[idx])
            {
                this[idx] = p;
                v |= true;
                if (NewHighEvent!= null)
                    NewHighEvent(getlabel(idx));
            }
            return v;

        }


        public event SymDelegate NewHighEvent;
    }
    /// <summary>
    /// track lows
    /// </summary>
    public class LowTracker : GenericTracker<decimal>, GotTickIndicator, GenericTrackerDecimal
    {
        public void GotTick(Tick k) { newTick(k); }
        public decimal getvalue(int idx) { return this[idx]; }
        public decimal getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, decimal v) { this[idx] = v; }
        /// <summary>
        /// set high/low from tick, return true if new high or low was reached
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool newTick(Tick k)
        {
            if (!k.isTrade) return false;
            return newPoint(k.symbol, k.trade);
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
        /// set low from a point
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool newPoint(string sym, decimal p)
        {
            int idx = getindex(sym);
            if (idx<0)
                idx = addindex(sym,decimal.MaxValue);
            return newPoint(idx, p);
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

            if (p < this[idx])
            {
                this[idx] = p;
                v |= true;
                if (NewLowEvent != null)
                    NewLowEvent(getlabel(idx));
            }
            return v;

        }

        public LowTracker() : this(100, "LOW") { }
        public LowTracker(string name) : this(100, name) { }
        public LowTracker(int estsym) : this(estsym, "LOW") { }

     
                /// <summary>
        /// tracks highs and lows
        /// </summary>
        public LowTracker(int estSymbols, string name)
            : base(estSymbols, name)
        {
        }




        public event SymDelegate NewLowEvent;
    }

}

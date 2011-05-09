using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// easily trade positions for a collection of securities.
    /// automatically builds positions from existing positions and new trades.
    /// </summary>
    public class PositionTracker : GenericTracker<Position>,GotPositionIndicator,GotFillIndicator
    {
        /// <summary>
        /// create a tracker
        /// </summary>
        public PositionTracker() : this(3) { }
        /// <summary>
        /// create tracker with approximate # of positions
        /// </summary>
        /// <param name="estimatedPositions"></param>
        public PositionTracker(int estimatedPositions) : base(estimatedPositions,"POSITION") 
        {
            NewTxt += new TextIdxDelegate(PositionTracker_NewTxt);
        }
        public PositionTracker(string name) : base(name) { }
        void PositionTracker_NewTxt(string txt, int idx)
        {
            if (NewSymbol!= null)
                NewSymbol(txt);
        }

        public void GotPosition(Position p) { Adjust(p); }
        public void GotFill(Trade f) { Adjust(f); }
        
        

        /// <summary>
        /// clear all positions.  use with caution.
        /// also resets default account.
        /// </summary>
        public void Clear()
        {
            _defaultacct = string.Empty;
            base.Clear();
        }
        string _defaultacct = string.Empty;
        /// <summary>
        /// Default account used when querying positions
        /// (if never set by user, defaults to first account provided via adjust)
        /// </summary>
        public string DefaultAccount { get { return _defaultacct; } set { _defaultacct = value; } }
        /// <summary>
        /// get position given positions symbol (assumes default account)
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public new Position this[string symbol] 
        { 
            get 
            {
                int idx = getindex(symbol + DefaultAccount);
                if (idx<0)
                    return new PositionImpl(symbol,0,0,0,DefaultAccount);  
                return this[idx];
            } 
        }
        /// <summary>
        /// get a position in tracker given symbol and account
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public Position this[string symbol, string account] 
        { 
            get 
            {
                int idx = getindex(symbol + account);
                if (idx<0)
                    return new PositionImpl(symbol,0,0,0,account);  
                return this[idx];
            } 
        }

        /// <summary>
        /// get position given positions symbol (assumes default account)
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public new Position this[int idx]
        {
            get
            {
                if (idx < 0)
                    return new PositionImpl();
                Position p = base[idx];
                if (p == null)
                    return new PositionImpl(getlabel(idx));
                return p;
            }
        }
        


        decimal _totalclosedpl = 0;
        /// <summary>
        /// gets sum of all closed pl for all positions
        /// </summary>
        public decimal TotalClosedPL { get { return _totalclosedpl; } }
        
        /// <summary>
        /// Create a new position, or overwrite existing position
        /// </summary>
        /// <param name="newpos"></param>
        public void NewPosition(Position newpos)
        {
            Adjust(newpos);
        }

        /// <summary>
        /// Adjust an existing position, or create new one if none exists.
        /// </summary>
        /// <param name="fill"></param>
        /// <returns>any closed PL for adjustment</returns>
        public decimal Adjust(Trade fill)
        {
            int idx = getindex(fill.symbol + fill.Account);
            return Adjust(fill, idx);
        }
        /// <summary>
        /// Adjust an existing position, or create a new one... given a trade and symbol+account index
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public decimal Adjust(Trade fill, int idx)
        {
            decimal cpl = 0;
            
            if (_defaultacct == string.Empty)
                _defaultacct = fill.Account ;
            
            if (idx < 0)
                addindex(fill.symbol + fill.Account, new PositionImpl(fill));
            else
            {
                cpl += this[idx].Adjust(fill);
            }
            _totalclosedpl += cpl;
            return cpl;
        }

        /// <summary>
        /// called when a new position is added to tracker.
        /// </summary>
        public event SymDelegate NewSymbol;
        /// <summary>
        /// overwrite existing position, or start new position
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public decimal Adjust(Position newpos)
        {
            
            if (_defaultacct == string.Empty)
                _defaultacct = newpos.Account;
            int idx = getindex(newpos.Symbol + newpos.Account);
            if (idx < 0)
                addindex(newpos.Symbol + newpos.Account, new PositionImpl(newpos));
            else
            {
                base[idx] = new PositionImpl(newpos);
                _totalclosedpl += newpos.ClosedPL;
            }
            return 0;
        }



    }

    /// <summary>
    /// track only position size
    /// </summary>
    public class PositionSizeTracker : PositionTracker, GenericTrackerInt
    {
        public PositionSizeTracker() : base("POSSIZE") { }
        public int getvalue(int idx) { return this[idx]; }
        public int getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, int v) { }
        public int addindex(string txt, int v) { return getindex(txt); }
        public new int this[int idx] { get { return base[idx].Size; } set {} }
        public new int this[string txt] { get { return base[txt].Size; } set { } }
    }
    /// <summary>
    /// track only position price
    /// </summary>
    public class PositionPriceTracker : PositionTracker, GenericTrackerDecimal
    {
        public PositionPriceTracker() : base("POSPRICE") { }
        public decimal getvalue(int idx) { return this[idx]; }
        public decimal getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, decimal v) { }
        public int addindex(string txt, decimal v) { return getindex(txt); }
        public new decimal this[int idx] { get { return base[idx].AvgPrice; } set { } }
        public new decimal this[string txt] { get { return base[txt].AvgPrice; } set { } }
    }
    /// <summary>
    /// track only whether position is flat
    /// </summary>
    public class FlatPositionTracker : PositionTracker, GenericTrackerBool
    {
        public FlatPositionTracker() : base("ISFLAT") { }
        public bool getvalue(int idx) { return this[idx].isFlat; }
        public bool getvalue(string txt) { return this[txt].isFlat; }
        public void setvalue(int idx, bool v) {  }
        public int addindex(string txt, bool v) { return getindex(txt); }
    }
    /// <summary>
    /// track only whether position is long
    /// </summary>
    public class LongPositionTracker : PositionTracker, GenericTrackerBool
    {
        public LongPositionTracker() : base("ISLONG") { }
        public bool getvalue(int idx) { return this[idx].isLong; }
        public bool getvalue(string txt) { return this[txt].isLong; }
        public void setvalue(int idx, bool v) { }
        public int addindex(string txt, bool v) { return getindex(txt); }
    }

    /// <summary>
    /// track only whether position is short
    /// </summary>
    public class ShortPositionTracker : PositionTracker, GenericTrackerBool
    {
        public ShortPositionTracker() : base("ISSHORT") { }
        public bool getvalue(int idx) { return this[idx].isShort; }
        public bool getvalue(string txt) { return this[txt].isShort; }
        public void setvalue(int idx, bool v) { }
        public int addindex(string txt, bool v) { return getindex(txt); }
    }

    
}

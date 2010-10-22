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
    public class PositionTracker : GenericTracker<Position>
    {
        /// <summary>
        /// create a tracker
        /// </summary>
        public PositionTracker() : this(3) { }
        /// <summary>
        /// create tracker with approximate # of positions
        /// </summary>
        /// <param name="estimatedPositions"></param>
        public PositionTracker(int estimatedPositions) : base(estimatedPositions,"POSSIZE") 
        {
            NewTxt += new TextIdxDelegate(PositionTracker_NewTxt);
        }
        void PositionTracker_NewTxt(string txt, int idx)
        {
            if (NewSymbol!= null)
                NewSymbol(txt);
        }
        
        

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
                this[idx] = new PositionImpl(newpos);
                _totalclosedpl += newpos.ClosedPL;
            }
            return 0;
        }



    }
}

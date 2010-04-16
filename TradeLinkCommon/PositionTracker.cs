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
    public class PositionTracker
    {
        /// <summary>
        /// create a tracker
        /// </summary>
        public PositionTracker() : this(3) { }
        /// <summary>
        /// create tracker with approximate # of positions
        /// </summary>
        /// <param name="estimatedPositions"></param>
        public PositionTracker(int estimatedPositions) 
        {
            _poslist = new List<Position>(estimatedPositions);
        }
        List<Position> _poslist = null;
        Dictionary<string, int> _symidx = new Dictionary<string, int>();

        /// <summary>
        /// clear all positions.  use with caution.
        /// also resets default account.
        /// </summary>
        public void Clear()
        {
            _defaultacct = string.Empty;
            _poslist.Clear();
            _symidx.Clear();
        }
        string _defaultacct = string.Empty;
        /// <summary>
        /// Default account used when querying positions
        /// (if never set by user, defaults to first account provided via adjust)
        /// </summary>
        public string DefaultAccount { get { return _defaultacct; } set { _defaultacct = value; } }
        /// <summary>
        /// get position given position's index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public Position this[int idx] { get { return _poslist[idx]; } }
        /// <summary>
        /// get position given positions symbol (assumes default account)
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position this[string symbol] 
        { 
            get 
            {
                int idx = -1;
                if (_symidx.TryGetValue(symbol+DefaultAccount, out idx))
                    return _poslist[idx];
                Position p = new PositionImpl(symbol,0,0,0,DefaultAccount);  
                return p;
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
                int idx = -1;
                if (_symidx.TryGetValue(symbol+account, out idx))
                    return _poslist[idx];
                Position p = new PositionImpl(symbol,0,0,0,account);  
                return p;
            } 
        }
        /// <summary>
        /// enumerate through positions in tracker using foreach
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator() 
        { 
            foreach (Position p in _poslist) 
                yield return p; 
        }
        /// <summary>
        /// count of positions stored in tracker
        /// </summary>
        public int Count { get { return _poslist.Count; } }
        /// <summary>
        /// array of positions in tracker
        /// </summary>
        /// <returns></returns>
        public Position[] ToArray()
        {
            return _poslist.ToArray();
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
            _totalclosedpl += newpos.ClosedPL;
            if (_defaultacct == string.Empty)
                _defaultacct = newpos.Account;
            int idx = 0;
            if (_symidx.TryGetValue(newpos.Symbol+newpos.Account,out idx))
                _poslist[idx] = new PositionImpl(newpos);
            else
            {
                _poslist.Add(new PositionImpl(newpos));
                _symidx.Add(newpos.Symbol+newpos.Account,_poslist.Count-1);
                if (NewSymbol != null)
                    NewSymbol(newpos.Symbol);
            }
        }

        /// <summary>
        /// Adjust an existing position, or create new one if none exists.
        /// </summary>
        /// <param name="fill"></param>
        /// <returns>any closed PL for adjustment</returns>
        public decimal Adjust(Trade fill)
        {
            decimal cpl = 0;
            int idx = -1;
            if (_defaultacct == string.Empty)
                _defaultacct = fill.Account ;

            if (_symidx.TryGetValue(fill.symbol+fill.Account, out idx))
                cpl += _poslist[idx].Adjust(fill);
            else
            {
                _poslist.Add(new PositionImpl(fill));
                _symidx.Add(fill.symbol+fill.Account, _poslist.Count - 1);
                if (NewSymbol != null)
                    NewSymbol(fill.symbol);
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
        public decimal Adjust(Position p)
        {
            // overwrite existing position
            NewPosition(p);
            return 0;
        }



    }
}

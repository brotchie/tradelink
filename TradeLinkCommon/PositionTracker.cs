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
        public PositionTracker() : this(3) { }
        public PositionTracker(int estimatedPositions) 
        {
            _poslist = new List<Position>(estimatedPositions);
        }
        List<Position> _poslist = null;
        Dictionary<string, int> _symidx = new Dictionary<string, int>();

        public Position this[int idx] { get { return _poslist[idx]; } }
        public Position this[string symbol] 
        { 
            get 
            {
                int idx = -1;
                if (_symidx.TryGetValue(symbol, out idx))
                    return _poslist[idx];
                return new PositionImpl(symbol);  
            } 
        }
        public IEnumerator GetEnumerator() 
        { 
            foreach (Position p in _poslist) 
                yield return p; 
        }

        public int Count { get { return _poslist.Count; } }
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
            Position p;
            int idx = 0;
            if (_symidx.TryGetValue(newpos.Symbol,out idx))
                _poslist[idx] = new PositionImpl(newpos);
            else
            {
                _poslist.Add(new PositionImpl(newpos));
                _symidx.Add(newpos.Symbol,_poslist.Count-1);
            }
        }

        /// <summary>
        /// Adjust an existing position, or create new one if none exists.
        /// </summary>
        /// <param name="fill"></param>
        /// <returns>any closed PL for adjustment</returns>
        public decimal Adjust(Trade fill)
        {
            Position p;
            decimal cpl = 0;
            int idx = -1;
            if (_symidx.TryGetValue(fill.symbol, out idx))
                cpl += _poslist[idx].Adjust(fill);
            else
            {
                _poslist.Add(new PositionImpl(fill));
                _symidx.Add(fill.symbol, _poslist.Count - 1);
            }
            _totalclosedpl += cpl;
            return cpl;
        }
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

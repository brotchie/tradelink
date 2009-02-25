using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{

    public class PositionTracker
    {
        public PositionTracker() { }
        Dictionary<string, Position> posdict = new Dictionary<string, Position>();

        public Position this[string symbol] { get { Position p; if (posdict.TryGetValue(symbol, out p)) return p; return new PositionImpl(symbol);  } }
        public IEnumerator GetEnumerator() { foreach (Position p in posdict.Values) yield return p; }
        
        /// <summary>
        /// Create a new position, or overwrite existing position
        /// </summary>
        /// <param name="newpos"></param>
        public void NewPosition(Position newpos)
        {
            Position p;
            if (posdict.TryGetValue(newpos.Symbol, out p))
                posdict[newpos.Symbol] = newpos;
            else
                posdict.Add(newpos.Symbol, newpos);
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
            if (posdict.TryGetValue(fill.symbol, out p))
                cpl += posdict[fill.symbol].Adjust(fill);
            else
                posdict.Add(fill.symbol, new PositionImpl(fill));
            return cpl;
        }



    }
}

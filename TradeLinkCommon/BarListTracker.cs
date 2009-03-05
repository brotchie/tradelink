using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    public class BarListTracker
    {
        public BarListTracker() : this(new string[0]) { }
        /// <summary>
        /// preinitialize a tracker with selected symbols
        /// </summary>
        /// <param name="symbols"></param>
        public BarListTracker(string[] symbols)
        {
            for (int i = 0; i < symbols.Length; i++)
                _bdict.Add(symbols[i], new BarListImpl(symbols[i]));
        }
        Dictionary<string, BarListImpl> _bdict = new Dictionary<string, BarListImpl>();
        public int SymbolCount { get { return _bdict.Count; } }
        public BarList this[string sym]
        {

            get
            {
                BarListImpl bl;
                if (_bdict.TryGetValue(sym, out bl))
                    return (BarList)bl;
                bl = new BarListImpl(sym);
                _bdict.Add(sym, bl);
                return bl;
            }
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
                bl = new BarListImpl(k.symbol);
                _bdict.Add(k.symbol, bl);
            }
            bl.newTick(k);
        }
    }
}

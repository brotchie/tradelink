using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{
    public class OffsetTracker
    {
        decimal _defaultprofitdist = .2m;
        decimal _defaultstopdist = .1m;
        public decimal DefaultProfit { get { return _defaultprofitdist; } set { _defaultprofitdist = value; } }
        public decimal DefaultStop { get { return _defaultstopdist; } set { _defaultstopdist = value; } }
        Dictionary<string, decimal> _profitdist = new Dictionary<string, decimal>();
        Dictionary<string, decimal> _stopdist = new Dictionary<string, decimal>();
        public event OrderDelegate SentOffset;
        public OffsetTracker()
        {
        }

        // stop distances
        decimal[] _stopd = new decimal[0];
        decimal[] _profitd = new decimal[0];
        // symbols
        string[] _syms = new string[0];
        // pending
        uint[] _profitid = new uint[0];
        uint[] _stopid = new uint[0];
        // cancels
        uint[] _profitc = new uint[0];
        uint[] _stopc = new uint[0];

        int symidx(string sym)
        {
            // see if we already have this symbol, if we do return index
            for (int i = 0; i < _syms.Length; i++)
                if (_syms[i] == sym) return i;
            // otherwise create space for new symbol
            string[] tmp = new string[_syms.Length + 1];
            // copy in original symbols
            Array.Copy(_syms, tmp, _syms.Length);
            // add new symbol
            tmp[_syms.Length] = sym;
            // make this new list the permanent list
            _syms = tmp;
            // return index of this symbol we just added
            return _syms.Length - 1;
        }

        public void GotFill(Trade t)
        {
            

        }

        public void GotCancel(uint id)
        {
        }

    }
}

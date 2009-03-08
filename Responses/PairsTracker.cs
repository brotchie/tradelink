using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using TradeLink.API;

namespace Responses
{
    /// <summary>
    /// class used to simplifying pairs trading in tradelink
    /// </summary>
    public delegate void DecimalDelegate(decimal val);
    public class PairsTracker
    {

        /// <summary>
        /// event fires when spread breaks outside specified bound
        /// </summary>
        public event DecimalDelegate SpreadOutsideBounds;


        public PairsTracker(string symA, string symB, decimal A2B_RATIO, int SpreadBoundary)
        {
            _syma = symA;
            _symb = symB;
            _ratio = A2B_RATIO;
            _spreadbound = SpreadBoundary;
        }

        string _syma = "";
        string _symb = "";
        decimal _ratio = 1;
        int _spreadbound = int.MaxValue;
        decimal _pa = 0;
        decimal _pb = 0;
        decimal _parel = 0;
        public string Asym { get { return _syma; } }
        public string Bsym { get { return _symb; } }
        public int Bound { get { return _spreadbound; } set { _spreadbound = value; } }
        public decimal Aprice { get { return _pa; } }
        public decimal Bprice { get { return _pb; } }
        public decimal ApriceRel { get { return _parel; } }
        public bool hasPrices { get { return _pa * _pb != 0; } }
        /// <summary>
        /// current spread in basis points (B-A)/B
        /// </summary>
        public int Spread { get { return hasPrices ? (int)(((_pb - _parel)*10000)/_pb) : 0; } }

        public void GotTick(Tick k)
        {
            if (!k.isTrade) return;
            if (k.symbol == _symb)
                _pb = k.trade;
            if (k.symbol == _syma)
                _pa = k.trade;
            if (!hasPrices) return;
            _parel = _pa * _ratio;
            int spread = Spread;
            if ((Math.Abs(spread) > _spreadbound) && (SpreadOutsideBounds != null))
                SpreadOutsideBounds(spread);
        }


    }
}

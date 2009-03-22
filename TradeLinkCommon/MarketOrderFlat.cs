using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.Common
{
    public class MarketOrderFlat : OrderImpl
    {
        public MarketOrderFlat(Position current) : this(current, 1m) { }
        public MarketOrderFlat(Position current, decimal percent, bool normalizeSize, int MinimumLotSize) : base(current.Symbol, normalizeSize ? Calc.Norm2Min((decimal)percent * current.FlatSize, MinimumLotSize) : (int)(percent*current.FlatSize)) { }
        public MarketOrderFlat(Position current, decimal percent) : this(current, percent, true, 100) { }
    }
}

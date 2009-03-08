using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.Common
{
    public class MarketOrderFlat : OrderImpl
    {
        public MarketOrderFlat(Position current) : base(current.Symbol, current.FlatSize) { }
    }
}

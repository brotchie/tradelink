

namespace TradeLink.Common
{
    /// <summary>
    /// Create market orders.
    /// </summary>
    public class MarketOrder : OrderImpl
    {
        public MarketOrder(string sym, int size) : base(sym, size) { }
        public MarketOrder(string sym, bool side, int size) : base(sym, side, System.Math.Abs(size)) { }
        public MarketOrder(string sym, bool side, int size, string comment) : base(sym, side, System.Math.Abs(size), comment) { }
    }
    /// <summary>
    /// Create buy market orders.
    /// </summary>
    public class BuyMarket : OrderImpl
    {
        public BuyMarket(string sym, int size) : base(sym, true, System.Math.Abs(size)) { }
        public BuyMarket(string sym, int size, string comment) : base(sym, true, System.Math.Abs(size), comment) { }
    }
    /// <summary>
    /// Create sell market orders.
    /// </summary>
    public class SellMarket : OrderImpl
    {
        public SellMarket(string sym, int size) : base(sym, false, System.Math.Abs(size)) { }
        public SellMarket(string sym, int size, string comment) : base(sym, false, System.Math.Abs(size), comment) { }
    }
}

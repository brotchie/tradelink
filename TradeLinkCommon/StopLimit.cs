using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLink.Common
{
    /// <summary>
    /// Create stop limit orders.
    /// </summary>
    public class StopLimitOrder : OrderImpl
    {
        public StopLimitOrder(string sym, bool side, int size, decimal price, decimal stop, long orderid) : base(sym, side, size, price, stop, string.Empty, 0, 0, orderid) { }
        public StopLimitOrder(string sym, bool side, int size, decimal price, decimal stop, string comment) : base(sym, side, size, price, stop, comment, 0, 0) { }
        public StopLimitOrder(string sym, bool side, int size, decimal price, decimal stop) : base(sym, side, size, price, stop, "", 0, 0) { }
    }
    /// <summary>
    /// Create buy stop limit orders.
    /// </summary>
    public class BuyStopLimit : OrderImpl
    {
        public BuyStopLimit(string sym, int size, decimal price, decimal stop, long orderid) : base(sym, true, size, price, stop, string.Empty, 0, 0, orderid) { }
        public BuyStopLimit(string sym, int size, decimal price, decimal stop, string comment) : base(sym, true, size, price, stop, comment, 0, 0) { }
        public BuyStopLimit(string sym, int size, decimal price, decimal stop) : base(sym, true, size, price, stop, "", 0, 0) { }
    }

    /// <summary>
    /// Create sell-stop limit orders.
    /// </summary>
    public class SellStopLimit : OrderImpl
    {
        public SellStopLimit(string sym, int size, decimal price, decimal stop, long orderid) : base(sym, false, size, price, stop, string.Empty, 0, 0, orderid) { }
        public SellStopLimit(string sym, int size, decimal price, decimal stop, string comment) : base(sym, false, size, price, stop, comment, 0, 0) { }
        public SellStopLimit(string sym, int size, decimal price, decimal stop) : base(sym, false, size, price, stop, "", 0, 0) { }
    }
}

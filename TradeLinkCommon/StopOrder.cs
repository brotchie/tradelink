using System;

namespace TradeLink.Common
{
    /// <summary>
    /// A stop-loss order.
    /// </summary>
    public class StopOrder : OrderImpl
    {
        public StopOrder(string sym, int size, decimal stop, long id) : base(sym, size>0, Math.Abs(size), 0, stop, string.Empty, 0,0, id) { }
        public StopOrder(string sym, bool side, int size, decimal stop, string comment) : base(sym, side, Math.Abs(size), 0, stop, comment, 0, 0) { }
        public StopOrder(string sym, bool side, int size, decimal stop) : base(sym, side, Math.Abs(size), 0, stop, "", 0, 0) { }
        public StopOrder(string sym, bool side, int size, decimal stop, long orderid) : base(sym, side, Math.Abs(size), 0, stop, "", 0, 0, orderid) { }
    }
    /// <summary>
    /// Create a buystop order.
    /// </summary>
    public class BuyStop : OrderImpl
    {
        
        public BuyStop(string sym, int size, decimal stop, string comment) : base(sym, true, Math.Abs(size), 0, stop, comment, 0, 0) { }
        public BuyStop(string sym, int size, decimal stop) : base(sym, true, Math.Abs(size), 0, stop, "", 0, 0) { }
        public BuyStop(string sym, int size, decimal stop, long orderid) : base(sym, true, Math.Abs(size), 0, stop, "", 0, 0, orderid) { }

    }
    /// <summary>
    /// Create a sellstop order.
    /// </summary>
    public class SellStop : OrderImpl
    {
        
        public SellStop(string sym, int size, decimal stop, string comment) : base(sym, false, Math.Abs(size), 0, stop, comment, 0, 0) { }
        public SellStop(string sym, int size, decimal stop) : base(sym, false, size, Math.Abs(size),stop, "", 0, 0) { }
        public SellStop(string sym, int size, decimal stop, long orderid) : base(sym, false, Math.Abs(size), 0, stop, "", 0, 0, orderid) { }

    }

    /// <summary>
    /// Create a trailing stop order
    /// </summary>
    public class TrailStop : OrderImpl
    {
        public TrailStop(string sym, int trailsize, decimal trailamt,string comment) : base(sym,trailsize>0,trailsize,comment) { trail = trailamt; }
        public TrailStop(string sym,int trailsize, decimal trailamt) : this(sym,trailsize,trailamt,"") {}
    }
}

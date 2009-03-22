
namespace TradeLink.Common
{
    /// <summary>
    /// Create limit orders.
    /// </summary>
    public class LimitOrder : OrderImpl
    {
        public LimitOrder(string sym, bool side, int size, decimal price, uint orderid) : base(sym, side, size, price, 0, string.Empty, 0, 0,orderid) { }
        public LimitOrder(string sym, bool side, int size, decimal price, string comment) : base(sym, side, size, price, 0, comment, 0, 0) { }
        public LimitOrder(string sym, bool side, int size, decimal price) : base (sym,side,size,price,0,"",0,0) { }
    }
    /// <summary>
    /// Create buy limit orders.
    /// </summary>
    public class BuyLimit : OrderImpl
    {
        public BuyLimit(string sym, int size, decimal price, uint orderid) : base(sym, true, size, price, 0, string.Empty, 0, 0,orderid) { }
        public BuyLimit(string sym, int size, decimal price, string comment) : base (sym,true,size,price,0,comment,0,0) { }
        public BuyLimit(string sym, int size, decimal price) : base (sym,true,size,price,0,"",0,0) { }
    }

    /// <summary>
    /// Create sell-limit orders.
    /// </summary>
    public class SellLimit : OrderImpl
    {
        public SellLimit(string sym, int size, decimal price, uint orderid) : base(sym, false, size, price, 0, string.Empty, 0, 0,orderid) { }
        public SellLimit(string sym, int size, decimal price, string comment) : base (sym,false,size,price,0,comment,0,0) { }
        public SellLimit(string sym, int size, decimal price) : base (sym,false,size,price,0,"",0,0) { }
    }

    /// <summary>
    /// Create a buy OPG order
    /// </summary>
    public class BuyOPG : OrderImpl
    {
        public BuyOPG(string sym, int size, decimal price, string comment) : base(sym,true,size,price,0,comment,0,0) { TIF = "OPG"; }
        public BuyOPG(string sym, int size,decimal price) : this(sym,size,price,"") {}
        public BuyOPG(string sym, int size, decimal price, uint orderid) : base(sym, true, size, price, 0, string.Empty, 0, 0,orderid) { TIF = "OPG"; }

    }

    public class SellOPG : OrderImpl
    {
        public SellOPG(string sym, int size, decimal price, string comment) : base(sym, false, size, price, 0, comment, 0, 0) { TIF = "OPG"; }
        public SellOPG(string sym, int size, decimal price) : this(sym, size, price, "") { }
        public SellOPG(string sym, int size, decimal price, uint orderid) : base(sym, false, size, price, 0, string.Empty, 0, 0,orderid) { TIF = "OPG"; }

    }

    public class OPGOrder : OrderImpl
    {
        public OPGOrder(string sym, bool side, int size, decimal price, string comment) : base(sym, side, size, price, 0, comment, 0, 0) { TIF = "OPG"; }
        public OPGOrder(string sym, bool side, int size, decimal price) : this(sym, side, size, price, "") { }
        public OPGOrder(string sym, bool side, int size, decimal price, uint orderid) : base(sym, side, size, price, 0, string.Empty, 0, 0, orderid) { TIF = "OPG"; }
    }



}


namespace TradeLib
{
    public class LimitOrder : Order
    {
        public LimitOrder(string sym, bool side, int size, decimal price, string comment) : base(sym, side, size, price, 0, comment, 0, 0) { }
        public LimitOrder(string sym, bool side, int size, decimal price) : base (sym,side,size,price,0,"",0,0) { }
    }
    public class BuyLimit : Order
    {
        public BuyLimit(string sym, int size, decimal price, string comment) : base (sym,true,size,price,0,comment,0,0) { }
        public BuyLimit(string sym, int size, decimal price) : base (sym,true,size,price,0,"",0,0) { }
    }
    public class SellLimit : Order
    {
        public SellLimit(string sym, int size, decimal price, string comment) : base (sym,false,size,price,0,comment,0,0) { }
        public SellLimit(string sym, int size, decimal price) : base (sym,false,size,price,0,"",0,0) { }
    }

}

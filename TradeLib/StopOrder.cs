
namespace TradeLib
{
    public class StopOrder : Order
    {
        public StopOrder(string sym, bool side, int size, decimal stop, string comment) : base(sym, side, size, 0, stop, comment, 0, 0) { }
        public StopOrder(string sym, bool side, int size, decimal stop) : base(sym, side, size, 0, stop, "", 0, 0) { }
    }
    public class BuyStop : Order
    {
        public BuyStop(string sym, int size, decimal stop, string comment) : base(sym, true, size, 0, stop, comment, 0, 0) { }
        public BuyStop(string sym, int size, decimal stop) : base(sym, true, size, 0, stop, "", 0, 0) { }
    }
    public class SellStop : Order
    {
        public SellStop(string sym, int size, decimal stop, string comment) : base(sym, false, size, 0, stop, comment, 0, 0) { }
        public SellStop(string sym, int size, decimal stop) : base(sym, false, size, 0, stop, "", 0, 0) { }
    }
}

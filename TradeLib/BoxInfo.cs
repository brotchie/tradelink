using System;


namespace TradeLib
{
    public class BoxInfo
    {
        decimal _high = decimal.MinValue;
        decimal _low = decimal.MaxValue;
        decimal _open = 0;
        decimal _close = 0;
        decimal _yestclose = 0;
        public virtual decimal High { get { return _high; } set { _high = value; } }
        public virtual decimal Low { get { return _low; } set { _low = value; } }
        public virtual decimal Open { get { return _open; } set { _open = value; } }
        public virtual decimal Close { get { return _close; } set { _close = value; } }
        public virtual decimal YestClose { get { return _yestclose; } set { _yestclose = value; } }
    }
}

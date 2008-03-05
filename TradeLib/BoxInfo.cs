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
        public void Update(Tick t) { if (t.isTrade) Update(t.trade); }
        public void Update(decimal v)
        {
            if (Open == 0) Open = v;
            if (v > High) High = v;
            if (v < Low) Low = v;
        }
        public static BoxInfo FromBarList(BarList bl)
        {
            BoxInfo bi = new BoxInfo();
            if (!bl.HasBar()) return bi;
            bi.High = BarMath.HH(bl);
            bi.Low = BarMath.LL(bl);
            bi.Open = bl[0].Open;
            return bi;
        }
    }
}

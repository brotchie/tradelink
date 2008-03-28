using System;


namespace TradeLib
{
    /// <summary>
    /// Common information requested or needed by boxes.
    /// </summary>
    public class BoxInfo
    {
        decimal _high = decimal.MinValue;
        decimal _low = decimal.MaxValue;
        decimal _open = 0;
        decimal _close = 0;
        decimal _yestclose = 0;
        /// <summary>
        /// Gets or sets the high for the day in the traded stock.
        /// </summary>
        /// <value>The high.</value>
        public virtual decimal High { get { return _high; } set { _high = value; } }
        /// <summary>
        /// Gets or sets the low for the day.
        /// </summary>
        /// <value>The low.</value>
        public virtual decimal Low { get { return _low; } set { _low = value; } }
        /// <summary>
        /// Gets or sets the opening price.
        /// </summary>
        /// <value>The open.</value>
        public virtual decimal Open { get { return _open; } set { _open = value; } }
        /// <summary>
        /// Gets or sets the closing price (zero for still open).
        /// </summary>
        /// <value>The close.</value>
        public virtual decimal Close { get { return _close; } set { _close = value; } }
        /// <summary>
        /// Gets or sets yesterday's close.
        /// </summary>
        /// <value>The yest close.</value>
        public virtual decimal YestClose { get { return _yestclose; } set { _yestclose = value; } }
        /// <summary>
        /// Update the high/low/open with the most recent tick.
        /// </summary>
        /// <param name="t">The tick.</param>
        public void Update(Tick t) { if (t.isTrade) Update(t.trade); }
        /// <summary>
        /// Updates the high/low/open with the most recent trade.
        /// </summary>
        /// <param name="v">The value of the last trade.</param>
        public void Update(decimal v)
        {
            if (Open == 0) Open = v;
            if (v > High) High = v;
            if (v < Low) Low = v;
        }
        /// <summary>
        /// Create a box-info instance from an existing BarList
        /// </summary>
        /// <param name="bl">The barlist.</param>
        /// <returns>a BoxInfo</returns>
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

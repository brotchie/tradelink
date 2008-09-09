using System;

namespace TradeLib
{

    /// <summary>
    /// Used for tracking indicies, such as SP500 futures, NASDAQ, OIH, etc
    /// </summary>
    public class Index : Security
    {
        /// <summary>
        /// Determines whether the specified symbol is an index.
        /// </summary>
        /// <param name="sym">The sym.</param>
        /// <returns>
        /// 	<c>true</c> if the specified sym is idx; otherwise, <c>false</c>.
        /// </returns>
        public static bool isIdx(string sym)
        {
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("[A-Z0-9#]{1,4}$");
            System.Text.RegularExpressions.Regex r2 = new System.Text.RegularExpressions.Regex("^[/$]");
            string us = sym.ToUpper();
            bool match = r.IsMatch(us) && (r2.IsMatch(us) || us.Contains("#"));
            return match;
        }
        /// <summary>
        /// Gets a value indicating whether this instance is a valid index.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public override bool isValid { get { return Index.isIdx(this.name); } }
        public Index(Index copythisidx)
        {
            Name = copythisidx.Name;
            last = copythisidx.Value;
            open = copythisidx.open;
            high = copythisidx.high;
            low = copythisidx.low;
            close = copythisidx.close;
            date = copythisidx.date;
            time = copythisidx.time;
        }
        public Index() { _type = SecurityType.IDX; }
        public Index(string symbol) : this(symbol, 0, 0, 0, 1000000, 0,0,0) { }
        public Index(string symbol, decimal tick, decimal o, decimal h, decimal l, decimal c) : this(symbol, tick, o, h, l, c, 0, 0) { }
        public Index(string symbol, decimal tick, decimal o, decimal h, decimal l, decimal c, int date, int time)
        {
            Name = symbol;
            last = tick;
            open = o;
            high = h;
            low = l;
            close = c;
            Date = date;
            Time = time;
            _type = SecurityType.IDX;
        }
        string name = "";
        int time = 0;
        int date = 0;
        decimal open = 0;
        decimal high = 0;
        decimal low = 10000000;
        decimal close = 0;
        decimal last = 0;
        public int Date { get { return date; } set { date = value; } }
        public int Time { get { return time; } set { time = value; } }
        public override string Name { get { return name; } set { if (isIdx(value)) name = value; } }
        public decimal Value { get { return last; } }
        public decimal Open { get { return open; } }
        public decimal High { get { return high; } }
        public decimal Low { get { return low; } }
        public decimal Close { get { return close; } }
        private enum iorder
        {
            sym = 0,
            date,
            time,
            value,
            open,
            high,
            low,
            close,
        }

        /// <summary>
        /// Serializes the specified index.  Used for writing IDX files.
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            string s = "";
            s = Name+","+Date + "," + Time + "," + Value + "," + Open + "," + High + "," + Low + "," + Close + ",";
            return s;
        }
        /// <summary>
        /// Deserializes the specified string into an Index object.  Used for reading IDX files.
        /// </summary>
        /// <param name="val">The index in string form.</param>
        /// <returns></returns>
        public static Index Deserialize(string val)
        {
            string[] r = val.Split(',');
            Index i = null;
            try
            {
                i = new Index(r[(int)iorder.sym], Convert.ToDecimal(r[(int)iorder.value]), Convert.ToDecimal(r[(int)iorder.open]), Convert.ToDecimal(r[(int)iorder.high]), Convert.ToDecimal(r[(int)iorder.low]), Convert.ToDecimal(r[(int)iorder.close]), Convert.ToInt32(r[(int)iorder.date]), Convert.ToInt32(r[(int)iorder.time]));
            }
            catch (InvalidCastException) { }
            return i;
        }

        /// <summary>
        /// Convert an Index into a Tick.  Mainly for conveince, use with caution.
        /// </summary>
        /// <returns></returns>
        public Tick ToTick()
        {
            Tick t = new Tick(Name);
        	t.time = this.Time;
        	t.date = this.Date;
        	t.trade = this.Value;
            t.size = -1; // this is to make tick field "isTrade" return true...
        	return t;
        }
        /// <summary>
        /// Create a stock-equivalent instance from this Index.
        /// </summary>
        /// <returns></returns>
        public Stock ToStock()
        {
            Stock s = new Stock(StockifyIndex(Name), this.Date);
            return s;
        }
        /// <summary>
        /// Create a valid stock symbol name from an index name.
        /// </summary>
        /// <param name="IndexName">Name of the index.</param>
        /// <returns></returns>
        public static string StockifyIndex(string IndexName)
        {
            string sym = IndexName.Replace("/", "");
            sym = sym.Replace("$", "");
            return sym;
        }

        private System.IO.StreamReader _histfile = null;
        public bool hasHistorical { get { return (_histfile != null) && !_histfile.EndOfStream; } }
        public Index NextTick { get { if (!hasHistorical) return new Index(); Index i = Index.Deserialize(_histfile.ReadLine()); if (i == null) _histfile.Close(); return i; } }
        public static Index FromFile(string filename)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(filename);
            Index i = new Index();
            i = Index.Deserialize(sr.ReadLine());
            i._histfile = sr;
            return i;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

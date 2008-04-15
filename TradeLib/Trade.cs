using System;

namespace TradeLib
{
    /// <summary>
    /// A trade or execution of a stock order.  Also called a fill.
    /// </summary>
    [Serializable]
    public class Trade
    {
        public string accountid = "";
        public string symbol = null;
        public bool side = true;
        public int size = 0;
        public decimal price = 0;
        public decimal stopp = 0;
        public string comment = null;
        public int date = 0;
        public int time = 0;
        public int xsize = 0;
        public decimal xprice = 0;
        public int xsec = 0;
        public int xtime = 0;
        public int xdate = 0;
        public bool Side { get { return side; } }
        public int Size { get { return xsize; } }
        public decimal Price { get { return xprice; } }
        public virtual bool isValid { get { return (xsize != 0) && (xprice != 0) && (xtime != 0) && (xdate != 0) && (symbol != null); } }
        /// <summary>
        /// true if this is a real Trade, otherwise it's still an order.
        /// </summary>
        public bool isFilled = false;  // default to false (= order)
        public Trade() : this(null, true, 0, 0, 0, null, 0, 0) { }
        public Trade(string symbol, decimal fillprice, int fillsize) : this(symbol, fillprice, fillsize, DateTime.Now) { }
        public Trade(string sym, decimal fillprice, int fillsize, DateTime tradedate)
        {
            if (sym != null) symbol = sym.ToUpper();
            if ((fillsize == 0) || (fillprice == 0)) throw new Exception("Invalid trade: Zero price or size provided.");
            xtime = Util.ToTLTime(tradedate);
            xdate = Util.ToTLDate(tradedate);
            xsize = fillsize;
            xprice = fillprice;
            side = (fillsize > 0);
        }

        public Trade(string sym, bool side, int size, decimal p, decimal s, string c, int time, int date)
        {
            if (sym!=null) this.symbol = sym.ToUpper();
            this.side = side;
            this.size = size;
            this.price = p;
            this.stopp = s;
            this.comment = c;
            this.time = time;
            this.date = date;
            this.isFilled = true;
        }
        public void Fill(decimal xp)
        {
            this.xprice = xp;
            this.xsize = this.size;
            this.xtime = this.time;
            this.xdate = this.date;
            this.isFilled = true;
        }
        public void Fill(Tick t)
        {
            this.xprice = t.trade;
            this.xsize = this.size;
            this.xtime = t.time;
            this.xdate = t.date;
            this.isFilled = true;
        }
        public void Fill(decimal xp, int xs, int xt)
        {
            this.xprice = xp;
            this.xsize = xs;
            this.xtime = xt;
            this.isFilled = true;
        }
        public override string ToString()
        {
            return ToString(',');
        }
        public string ToString(char delimiter)
        {
            int usize = Math.Abs(xsize);
            string[] trade = new string[] { xdate.ToString(), xtime.ToString(), symbol, (side ? "BUY" : "SELL"), usize.ToString(), xprice.ToString("N2"), comment };
            return string.Join(delimiter.ToString(), trade);
        }
        /// <summary>
        /// Convert this Trade to a TradeLink Mesasge.
        /// </summary>
        /// <returns></returns>
        public string TLmsg()
        {
            const char d = ',';
            return xdate.ToString() + d + xtime.ToString() + d + xsec.ToString() + d + symbol + d + side.ToString() + d + xsize.ToString() + d + xprice.ToString() + d + comment + d;
        }
    }

}

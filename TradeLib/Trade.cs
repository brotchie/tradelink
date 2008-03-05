using System;

namespace TradeLib
{
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
        public virtual bool isValid { get { return (xsize != 0) && (xprice != 0) && (xtime != 0) && (xdate != 0) && (symbol != null); } }
        public bool isFilled = false;  // default to false (= order)
        public Trade() : this(null, true, 0, 0, 0, null, 0, 0) { }
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
            const char d = ',';
            return xdate.ToString() + d + xtime.ToString() + d + symbol + d + (side ? "BUY" : "SELL") + d + Math.Abs(xsize) + d + xprice + d + comment;
        }
        public string TLmsg()
        {
            const char d = ',';
            return xdate.ToString() + d + xtime.ToString() + d + xsec.ToString() + d + symbol + d + side.ToString() + d + xsize.ToString() + d + xprice.ToString() + d + comment + d;
        }
    }

}

using System;

namespace TradeLib
{
    /// <summary>
    /// A trade or execution of a stock order.  Also called a fill.
    /// </summary>
    [Serializable]
    public class Trade
    {
        protected Currency cur = Currency.USD;
        protected Security type = Security.STK;
        protected string ex = "NYSE";
        public Currency Currency { get { return cur; } set { cur = value; } }
        public Security Security { get { return type; } set { type = value; } }
        public string Exchange { get { return ex; } set { ex = value; } }
        public string Account { get { return accountid; } set { accountid = value; } }
        protected string accountid = "";
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
        public bool isFilled = false;  // default to false (= order)
        public bool Side { get { return side; } }
        public int Size { get { return xsize; } }
        public Trade(Trade copytrade)
        {
            // copy constructor, for copying using by-value (rather than by default of by-reference)
            cur = copytrade.cur;
            type = copytrade.type;
            ex = copytrade.ex;
            accountid = copytrade.accountid;
            symbol = copytrade.symbol;
            side = copytrade.side;
            size = copytrade.size;
            price = copytrade.price;
            stopp = copytrade.stopp;
            comment = copytrade.comment;
            date = copytrade.date;
            time = copytrade.time;
            xsize = copytrade.xsize;
            xprice = copytrade.xprice;
            xsec = copytrade.xsec;
            xtime = copytrade.xtime;
            xdate = copytrade.xdate;
            isFilled = copytrade.isFilled;
        }
        public decimal Price { get { return xprice; } }
        public virtual bool isValid { get { return (xsize != 0) && (xprice != 0) && (xtime != 0) && (xdate != 0) && (symbol != null); } }
        /// <summary>
        /// true if this is a real Trade, otherwise it's still an order.
        /// </summary>

        public Trade() { }
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

        protected Trade(string sym, bool side, int size, decimal p, decimal s, string c, int time, int date)
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
        public void Fill(decimal xprice, int xsize, int xtime, int xdate, int xsec)
        {
            this.xprice = xprice;
            this.xsize = xsize;
            this.xtime = xtime;
            this.xdate = xdate;
            this.xsec = xsec;
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
        public static Trade FromString(string tradestring) { return FromString(tradestring, ','); }
        public static Trade FromString(string tradestring, char delimiter)
        {
            string[] rec = tradestring.Split(delimiter);
            Trade t = new Trade(rec[2], rec[3] == "BUY", Convert.ToInt32(rec[4]), Convert.ToDecimal(rec[5]), 0, rec[6], Convert.ToInt32(rec[1]), Convert.ToInt32(rec[0]));
            t.Fill(t.price);
            return t;
        }
        /// <summary>
        /// Serialize trade as a string
        /// </summary>
        /// <returns></returns>
        public virtual string Serialize()
        {
            const char d = ',';
            return xdate.ToString() + d + xtime.ToString() + d + xsec.ToString() + d + symbol + d + side.ToString() + d + xsize.ToString() + d + xprice.ToString() + d + comment + d + ex + d + accountid + d + this.Security.ToString() + d + this.Currency.ToString();
        }
        /// <summary>
        /// Deserialize string to Trade
        /// </summary>
        /// <returns></returns>
        public static Trade Deserialize(string message)
        {
            string[] rec = message.Split(',');
            bool side = Convert.ToBoolean(rec[(int)TradeField.Side]);
            int size = Convert.ToInt32(rec[(int)TradeField.Size]);
            size = Math.Abs(size) * (side ? 1 : -1);
            decimal xprice = Convert.ToDecimal(rec[(int)TradeField.Price]);
            string sym = rec[(int)TradeField.Symbol];
            Trade t = new Trade(sym, xprice, size);
            t.xdate = Convert.ToInt32(rec[(int)TradeField.xDate]);
            t.xtime = Convert.ToInt32(rec[(int)TradeField.xTime]);
            t.xsec = Convert.ToInt32(rec[(int)TradeField.xSeconds]);
            t.comment = rec[(int)TradeField.Comment];
            t.Account = rec[(int)TradeField.Account];
            t.Exchange = rec[(int)TradeField.Exchange];
            t.Currency = (Currency)Enum.Parse(typeof(Currency), rec[(int)TradeField.Currency]);
            t.Security = (Security)Enum.Parse(typeof(Security), rec[(int)TradeField.Security]);
            return t;
        }
    }

    public enum TradeField
    {
        xDate=0,
        xTime,
        xSeconds,
        Symbol,
        Side,
        Size,
        Price,
        Comment,
        Exchange,
        Account,
        Security,
        Currency,
    }
}

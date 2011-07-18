using System;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// A trade or execution of a stock order.  Also called a fill.
    /// </summary>
    [Serializable]
    public class TradeImpl : Trade
    {
        long _id = 0;
        CurrencyType cur = CurrencyType.USD;
        SecurityType type = SecurityType.NIL;
        string _localsymbol = "";
        string accountid = "";
        string _sym = "";
        bool _side = true;
        string _comment = "";
        int _xsize = 0;
        int _xdate = 0;
        int _xtime = 0;
        decimal _xprice = 0;
        string _ex = string.Empty;
        public int UnsignedSize { get { return Math.Abs(_xsize); } }
        public string ex { get { return _ex; } set { _ex = value; } }
        public long id { get { return _id; } set { _id = value; } }
        public string LocalSymbol { get { return _localsymbol; } set { _localsymbol = value; } }
        public CurrencyType Currency { get { return cur; } set { cur = value; } }
        public SecurityType Security { get { return type; } set { type = value; } }
        public string Exchange { get { return ex; } set { ex = value; } }
        public string Account { get { return accountid; } set { accountid = value; } }
        public string symbol { get { return _sym; } set { _sym = value; } }
        public bool side { get { return _side; } set { _side = value; } }
        [Obsolete]
        public string comment { get { return _comment; } set { _comment = value; } }
        public int xsize { get { return _xsize; } set { _xsize = value; } }
        public decimal xprice { get { return _xprice; } set { _xprice = value; } }
        public int xdate { get { return _xdate; } set { _xdate = value; } }
        public int xtime { get { return _xtime; } set { _xtime = value; } }
        public bool isFilled { get { return (xprice * xsize) != 0; } }
        public TradeImpl(Trade copytrade)
        {
            // copy constructor, for copying using by-value (rather than by default of by-reference)
            id = copytrade.id;
            cur = copytrade.Currency;
            type = copytrade.Security;
            ex = copytrade.ex;
            accountid = copytrade.Account;
            symbol = copytrade.symbol;
            side = copytrade.side;
            comment = copytrade.comment;
            xsize = copytrade.xsize;
            xprice = copytrade.xprice;
            xtime = copytrade.xtime;
            xdate = copytrade.xdate;
        }
        public virtual decimal Price { get { return xprice; } }
        public virtual bool isValid { get { return (xsize != 0) && (xprice != 0) && (xtime+xdate != 0) && (symbol != null) && (symbol!=""); } }
        /// <summary>
        /// true if this is a real Trade, otherwise it's still an order.
        /// </summary>

        public TradeImpl() { }
        public TradeImpl(string symbol, decimal fillprice, int fillsize) : this(symbol, fillprice, fillsize, DateTime.Now) { }
        public TradeImpl(string sym, decimal fillprice, int fillsize, DateTime tradedate) : this(sym,fillprice, fillsize, Util.ToTLDate(tradedate),Util.DT2FT(tradedate)) {}
        public TradeImpl(string sym, decimal fillprice, int fillsize, int filldate, int filltime)
        {
            if (sym != null) symbol = sym.ToUpper();
            if ((fillsize == 0) || (fillprice == 0)) throw new Exception("Invalid trade: Zero price or size provided.");
            xtime = filltime;
            xdate = filldate;
            xsize = fillsize;
            xprice = fillprice;
            side = (fillsize > 0);
        }


        public override string ToString()
        {
            return ToString(',',true);
        }
        public string ToString(bool includeid) { return ToString(',', includeid); }
        public string ToString(char delimiter) { return ToString(delimiter, true); }
        public string ToString(char delimiter,bool includeid)
        {
            int usize = Math.Abs(xsize);
            string[] trade = new string[] { xdate.ToString(), xtime.ToString(), symbol, (side ? "BUY" : "SELL"), usize.ToString(), xprice.ToString("F2"), Account };
            if (!includeid)
                return string.Join(delimiter.ToString(), trade);
            return string.Join(delimiter.ToString(), trade) + delimiter + id;
        }
        public static Trade FromString(string tradestring) { return FromString(tradestring, ','); }
        public static Trade FromString(string tradestring, char delimiter)
        {
            string[] r = tradestring.Split(delimiter);
            Trade t = new TradeImpl();
            t.xdate = Convert.ToInt32(r[0]);
            t.xtime = Convert.ToInt32(r[1]);
            t.symbol = r[2];
            t.Account = r[6];
            t.xprice = Convert.ToDecimal(r[5]);
            t.side = r[3]=="BUY";
            t.xsize = Convert.ToInt32(r[4]);
            return t;
        }
        /// <summary>
        /// Serialize trade as a string
        /// </summary>
        /// <returns></returns>
        public static string Serialize(Trade t)
        {
            const char d = ',';
            return t.xdate.ToString() + d + t.xtime.ToString() + d + d + t.symbol + d + t.side.ToString() + d + t.xsize.ToString() + d + t.xprice.ToString(System.Globalization.CultureInfo.InvariantCulture) + d + t.comment + d + t.Account + d + t.Security.ToString() + d + t.Currency.ToString() + d + t.LocalSymbol + d + t.id.ToString() + d + t.ex; ;
        }
        /// <summary>
        /// Deserialize string to Trade
        /// </summary>
        /// <returns></returns>
        public static Trade Deserialize(string message)
        {
            Trade t = null;
            string[] rec = message.Split(',');
            if (rec.Length < 14) throw new InvalidTrade();
            bool side = Convert.ToBoolean(rec[(int)TradeField.Side]);
            int size = Convert.ToInt32(rec[(int)TradeField.Size]);
            size = Math.Abs(size) * (side ? 1 : -1);
            decimal xprice = Convert.ToDecimal(rec[(int)TradeField.Price],System.Globalization.CultureInfo.InvariantCulture);
            string sym = rec[(int)TradeField.Symbol];
            t = new TradeImpl(sym, xprice, size);
            t.xdate = Convert.ToInt32(rec[(int)TradeField.xDate]);
            t.xtime = Convert.ToInt32(rec[(int)TradeField.xTime]);
            t.comment = rec[(int)TradeField.Comment];
            t.Account = rec[(int)TradeField.Account];
            t.LocalSymbol = rec[(int)TradeField.LocalSymbol];
            t.id = Convert.ToInt64(rec[(int)TradeField.ID]);
            t.ex = rec[(int)TradeField.Exch];
            t.Currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), rec[(int)TradeField.Currency]);
            t.Security = (SecurityType)Enum.Parse(typeof(SecurityType), rec[(int)TradeField.Security]);

            return t;
        }

        public string ToChartLabel()
        {
            return ToChartLabel(this);
        }

        public static string ToChartLabel(Trade fill)
        {
            return (fill.xsize * (fill.side ? 1 : -1)).ToString() + fill.symbol;
        }

        public Security Sec { get { return new SecurityImpl(symbol, ex, Security); } }
    }


}

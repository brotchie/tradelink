using System;

namespace TradeLib
{
    /// <summary>
    /// Specify an order to buy or sell a quantity of stock.
    /// </summary>
    [Serializable]
    public class Order : Trade
    {
        public Order() : base() { }
        public Order(bool side) : base() { this.side = side; } 
        public string TIF = "DAY";
        public int sec = 0;
        public override bool isValid { get { return (symbol != null) && (size != 0); } }
        public bool isMarket { get { return (price == 0) && (stopp == 0); } }
        public bool isLimit { get { return (price != 0); } }
        public bool isStop { get { return (stopp != 0); } }
        public int SignedSize { get { return Math.Abs(size) * (side ? 1 : -1); } }
        public int UnSignedSize { get { return Math.Abs(size); } }
        public Order(Order copythis)
        {
            this.symbol = copythis.symbol;
            this.stopp = copythis.stopp;
            this.comment = copythis.comment;
            this.cur = copythis.cur;
            this.accountid = copythis.accountid;
            this.date = copythis.date;
            this.ex = copythis.ex;
            this.price = copythis.price;
            this.Security = copythis.Security;
            this.side = copythis.side;
            this.size = copythis.size;
            this.time = copythis.time;
            this.LocalSymbol = copythis.LocalSymbol;
            this.id = copythis.id;
            this.TIF = copythis.TIF;
            // shouldn't be used but we'll take them anyways
            this.xdate = copythis.xdate;
            this.xprice = copythis.xprice;
            this.xsec = copythis.xsec;
            this.xsize = copythis.xsize;
            this.xtime = copythis.xtime;
        }
        public Order(string sym, bool side, int size, decimal p, decimal s, string c, int time, int date)
        {
            this.symbol = sym;
            this.side = side;
            this.size = System.Math.Abs(size);
            this.price = p;
            this.stopp = s;
            this.comment = c;
            this.time = time;
            this.date = date;
        }
        public Order(string sym, bool side, int size)
        {
            this.symbol = sym;
            this.side = side;
            this.price = 0;
            this.stopp = 0;
            this.comment = "";
            this.time = 0;
            this.date = 0;
            this.size = System.Math.Abs(size);
        }
        public Order(string sym, bool side, int size, string c)
        {
            this.symbol = sym;
            this.side = side;
            this.price = 0;
            this.stopp = 0;
            this.comment = c;
            this.time = 0;
            this.date = 0;
            this.size = System.Math.Abs(size);
        }
        public Order(string sym, int size)
        {
            this.symbol = sym;
            this.side = size > 0;
            this.price = 0;
            this.stopp = 0;
            this.comment = "";
            this.time = 0;
            this.date = 0;
            this.size = System.Math.Abs(size);
        }
        public override string ToString()
        {
            if (this.isFilled) return base.ToString();
            return this.date + ":" + this.time + (side ? " BUY" : " SELL") + Math.Abs(size) + " " + this.symbol + "@" + (isMarket ? "Market" : (isLimit ? this.price.ToString("N2") : this.stopp.ToString("N2"))) + ((this.accountid!="") ? "["+accountid+"]" : "");
        }

        /// <summary>
        /// Serialize order as a string
        /// </summary>
        /// <returns></returns>
        public override string Serialize()
        {
            if (this.isFilled) return base.Serialize();
            //return symbol + d + (side ? "true" : "false")+d + Math.Abs(size) + d + price + d + stopp + d + comment + d + ex + d + accountid + d + this.Security.ToString() + d + this.Currency.ToString() + d + LocalSymbol;
            string [] r = new string[] { symbol,(side ? "true" : "false"),UnSignedSize.ToString(),price.ToString(),stopp.ToString(),comment,ex,accountid,this.Security.ToString(),this.Currency.ToString(),LocalSymbol,id.ToString(),TIF,date.ToString(),time.ToString(),sec.ToString()};
            return string.Join(",", r);
        }
        /// <summary>
        /// Deserialize string to Order
        /// </summary>
        /// <returns></returns>
        public new static Order Deserialize(string message)
        {
            string[] rec = message.Split(','); // get the record
            bool side = Convert.ToBoolean(rec[(int)OrderField.Side]);
            int size = Convert.ToInt32(rec[(int)OrderField.Size]);
            decimal oprice = Convert.ToDecimal(rec[(int)OrderField.Price]);
            decimal ostop = Convert.ToDecimal(rec[(int)OrderField.Stop]);
            string sym = rec[(int)OrderField.Symbol];
            Order o = new Order(sym, side, size);
            o.price = oprice;
            o.stopp = ostop;
            o.comment = rec[(int)OrderField.Comment];
            o.Account = rec[(int)OrderField.Account];
            o.Exchange = rec[(int)OrderField.Exchange];
            o.LocalSymbol = rec[(int)OrderField.LocalSymbol];
            o.Currency = (Currency)Enum.Parse(typeof(Currency), rec[(int)OrderField.Currency]);
            o.Security = (SecurityType)Enum.Parse(typeof(SecurityType), rec[(int)OrderField.Security]);
            o.id = Convert.ToUInt32(rec[(int)OrderField.OrderID]);
            o.TIF = rec[(int)OrderField.OrderTIF];
            try
            {
                o.date = Convert.ToInt32(rec[(int)OrderField.oDate]);
                o.time = Convert.ToInt32(rec[(int)OrderField.oTime]);
                o.sec = Convert.ToInt32(rec[(int)OrderField.oSec]);
            }
            catch (Exception) { } 
            return o;
        }

        public override int GetHashCode()
        {
            return symbol.GetHashCode() + accountid.GetHashCode() + (int)(price * 100) + (int)(stopp * 100) + ex.GetHashCode() + SignedSize + LocalSymbol.GetHashCode() + (int)Currency + (int)Security;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Order o = (Order)obj;
            return Equals(o);
        }

        public bool Equals(Order o)
        {
            if (o == null) return false;
            bool r = true;
            r &= price == o.price;
            r &= stopp == o.stopp;
            r &= symbol == o.symbol;
            r &= accountid == o.accountid;
            r &= ex == o.ex;
            r &= TIF == o.TIF;
            r &= LocalSymbol == o.LocalSymbol;
            r &= SignedSize == o.SignedSize;
            r &= Security == o.Security;
            return r;
        }
    }

    public enum OrderField
    {
        Symbol = 0,
        Side,
        Size,
        Price,
        Stop,
        Comment,
        Exchange,
        Account,
        Security,
        Currency,
        LocalSymbol, // non-pretty symbol or contract symbol for futures
        OrderID,
        OrderTIF,
        oDate,
        oTime,
        oSec,
    }

}

using System;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// Specify an order to buy or sell a quantity of a security.
    /// </summary>
    [Serializable]
    public class OrderImpl : TradeImpl, Order
    {
        string _tif = "DAY";
        int _sec, _date, _time,_size;
        decimal _price,_stopp,_trail;
        public new int UnsignedSize { get { return Math.Abs(_size); } }
        public string TIF { get { return _tif; } set { _tif = value; } }
        public decimal trail { get { return _trail; } set { _trail = value; } }
        public new string ex { get { return _ex; } set { _ex = value; } }
        public int size { get { return _size; } set { _size = value; } }
        public int sec { get { return _sec; } set { _sec = value; } }
        public decimal price { get { return _price; } set { _price = value; } }
        public decimal stopp { get { return _stopp; } set { _stopp = value; } }
        public int date { get { return _date; } set { _date = value; } }
        public int time { get { return _time; } set { _time = value; } }
        public OrderImpl() : base() { }
        public OrderImpl(bool side) : base() { this.side = side; } 
        public new bool isValid 
        { 
            get 
            { 
                if (isFilled) return base.isValid;
                return (symbol != null) && (size != 0); 
            } 
        }
        public bool isMarket { get { return (price == 0) && (stopp == 0); } }
        public bool isLimit { get { return (price != 0); } }
        public bool isStop { get { return (stopp != 0); } }
        public bool isTrail { get { return trail != 0; } }
        public int SignedSize { get { return Math.Abs(size) * (side ? 1 : -1); } }
        public int UnSignedSize { get { return Math.Abs(size); } }
        public override decimal Price
        {
            get
            {
                return isStop ? stopp : price; 
            }
        }
        public OrderImpl(Order copythis)
        {
            this.symbol = copythis.symbol;
            this.stopp = copythis.stopp;
            this.comment = copythis.comment;
            this.Currency = copythis.Currency;
            this.Account= copythis.Account;
            this.date = copythis.date;
            this.ex= copythis.ex;
            this.price = copythis.price;
            this.Security = copythis.Security;
            this.side = copythis.side;
            this.size = copythis.size;
            this.time = copythis.time;
            this.LocalSymbol = copythis.LocalSymbol;
            this.id = copythis.id;
            this.TIF = copythis.TIF;
        }
        public OrderImpl(string sym, bool side, int size, decimal p, decimal s, string c, int time, int date)
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
        public OrderImpl(string sym, bool side, int size, decimal p, decimal s, string c, int time, int date, uint orderid)
        {
            this.symbol = sym;
            this.side = side;
            this.size = System.Math.Abs(size);
            this.price = p;
            this.stopp = s;
            this.comment = c;
            this.time = time;
            this.date = date;
            this.id = orderid;
        }
        public OrderImpl(string sym, bool side, int size)
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
        public OrderImpl(string sym, bool side, int size, string c)
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
        public OrderImpl(string sym, int size)
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
            return (side ? " BUY" : " SELL") + UnSignedSize + " " + this.symbol + "@" + (isMarket ? "Market" : (isLimit ? this.price.ToString("N2") : this.stopp.ToString("N2"))) + "["+this.Account+"]" +this.date + ":" + this.time ;
        }

        /// <summary>
        /// Fills this order with a tick
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Fill(Tick t) { return Fill(t, false); }
        public bool Fill(Tick t, bool fillOPG)
        {
            if (!t.isTrade) return false;
            if (!fillOPG && TIF=="OPG") return false;
            if ((isLimit && Side && (t.trade <= price)) // buy limit
                || (isLimit && !Side && (t.trade>=price))// sell limit
                || (isStop && Side && (t.trade>=stopp)) // buy stop
                || (isStop && !Side && (t.trade<=stopp)) // sell stop
                || isMarket)
            {
                this.xprice = t.trade;
                this.xsize = t.size >= UnsignedSize ? size : t.size;
                this.xtime = t.time;
                this.xdate = t.date;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Try to fill incoming order against this order.  If orders match.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>order can be cast to valid Trade and function returns true.  Otherwise, false</returns>
        public bool Fill(Order o)
        {
            // sides must match
            if (Side==o.side) return false;
            // orders must be valid
            if (!o.isValid || !this.isValid) return false;
            // acounts must be different
            if (o.Account == Account) return false;
            if ((isLimit && Side && (o.price<=price)) // buy limit cross
                || (isLimit && !Side && (o.price>=price))// sell limit cross
                || (isStop && Side && (o.price>=stopp)) // buy stop
                || (isStop && !Side && (o.price<=stopp)) // sell stop
                || isMarket)
            {
                this.xprice = o.isLimit ? o.price : o.stopp;
                if (xprice==0) xprice = isLimit ? Price : stopp;
                this.xsize = o.UnsignedSize >= UnsignedSize ? UnsignedSize : o.UnsignedSize;
                this.xtime = o.time;
                this.xdate = o.date;
                return isFilled;
            }
            return false;
        }

        /// <summary>
        /// Serialize order as a string
        /// </summary>
        /// <returns></returns>
        public static string Serialize(Order o)
        {
            if (o.isFilled) return TradeImpl.Serialize((Trade)o);
            string[] r = new string[] { o.symbol, (o.side ? "true" : "false"), o.UnsignedSize.ToString(), o.price.ToString(), o.stopp.ToString(), o.comment, o.ex, o.Account, o.Security.ToString(), o.Currency.ToString(), o.LocalSymbol, o.id.ToString(), o.TIF, o.date.ToString(), o.time.ToString(), "", o.trail.ToString() };
            return string.Join(",", r);
        }
        /// <summary>
        /// Deserialize string to Order
        /// </summary>
        /// <returns></returns>
        public new static Order Deserialize(string message)
        {
            Order o = null;
            string[] rec = message.Split(','); // get the record
            if (rec.Length < 17) throw new InvalidOrder();
            bool side = Convert.ToBoolean(rec[(int)OrderField.Side]);
            int size = Convert.ToInt32(rec[(int)OrderField.Size]);
            decimal oprice = Convert.ToDecimal(rec[(int)OrderField.Price]);
            decimal ostop = Convert.ToDecimal(rec[(int)OrderField.Stop]);
            string sym = rec[(int)OrderField.Symbol];
            o = new OrderImpl(sym, side, size);
            o.price = oprice;
            o.stopp = ostop;
            o.comment = rec[(int)OrderField.Comment];
            o.Account = rec[(int)OrderField.Account];
            o.ex = rec[(int)OrderField.Exchange];
            o.LocalSymbol = rec[(int)OrderField.LocalSymbol];
            o.Currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), rec[(int)OrderField.Currency]);
            o.Security = (SecurityType)Enum.Parse(typeof(SecurityType), rec[(int)OrderField.Security]);
            o.id = Convert.ToUInt32(rec[(int)OrderField.OrderID]);
            o.TIF = rec[(int)OrderField.OrderTIF];
            o.trail = Convert.ToDecimal(rec[(int)OrderField.Trail]);
            o.date = Convert.ToInt32(rec[(int)OrderField.oDate]);
            o.time = Convert.ToInt32(rec[(int)OrderField.oTime]);
            return o;
        }

        public static uint Unique { get { return (uint)((decimal)DateTime.Now.TimeOfDay.TotalMilliseconds); } }
    }



}

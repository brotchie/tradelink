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
        public OrderInstructionType ValidInstruct { get { return (OrderInstructionType)Enum.Parse(typeof(OrderInstructionType), _tif, true); } set { _tif = value.ToString().Replace("OrderInstructionType", string.Empty).Replace(".", string.Empty); } }
        string _tif = "DAY";
        int  _date, _time,_size;
        decimal _price,_stopp,_trail;
        int _virtowner = 0;
        public int VirtualOwner { get { return _virtowner; } set { _virtowner = value; } }
        public new int UnsignedSize { get { return Math.Abs(_size); } }
        public string TIF { get { return _tif; } set { _tif = value; } }
        public decimal trail { get { return _trail; } set { _trail = value; } }

        public int size { get { return _size; } set { _size = value; } }
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
            this.size = System.Math.Abs(size) * (side ? 1 : -1);
            this.price = p;
            this.stopp = s;
            this.comment = c;
            this.time = time;
            this.date = date;
        }
        public OrderImpl(string sym, bool side, int size, decimal p, decimal s, string c, int time, int date, long orderid)
        {
            this.symbol = sym;
            this.side = side;
            this.size = System.Math.Abs(size) * (side ? 1 : -1);
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
            this.size = System.Math.Abs(size) * (side ? 1 : -1);
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
            this.size = System.Math.Abs(size) * (side ? 1 : -1);
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
            this.size = System.Math.Abs(size) * (side ? 1 : -1);
        }
        public override string ToString()
        {
            return ToString(2);
        }
        public string ToString(int decimals)
        {
            if (this.isFilled) return base.ToString();
            return (side ? " BUY" : " SELL") + UnsignedSize + " " + this.symbol + "@" + (isMarket ? "Mkt" : (isLimit ? this.price.ToString("N"+decimals.ToString()) : this.stopp.ToString("N"+decimals.ToString())+"stp")) + " ["+this.Account+"] " +id.ToString()+(isLimit && isStop ? " stop: "+stopp.ToString("N"+decimals.ToString()) : string.Empty);
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
            if (t.symbol != symbol) return false;
            if (!fillOPG && TIF=="OPG") return false;
            if ((isLimit && side && (t.trade <= price)) // buy limit
                || (isLimit && !side && (t.trade >= price))// sell limit
                || (isStop && side && (t.trade >= stopp)) // buy stop
                || (isStop && !side && (t.trade <= stopp)) // sell stop
                || isMarket)
            {
                this.xprice = t.trade;
                this.xsize = t.size >= UnsignedSize ? UnsignedSize : t.size;
                this.xsize *= side ? 1 : -1;
                this.xtime = t.time;
                this.xdate = t.date;
                return true;
            }
            return false;
        }
        /// <summary>
        /// fill against bid and ask rather than trade
        /// </summary>
        /// <param name="k"></param>
        /// <param name="smart"></param>
        /// <param name="fillOPG"></param>
        /// <returns></returns>
        public bool Fill(Tick k, bool bidask, bool fillOPG)
        {
            if (!bidask)
                return Fill(k, fillOPG);
            // buyer has to match with seller and vice verca
            bool ok = side ? k.hasAsk : k.hasBid;
            if (!ok) return false;
            decimal p = side ? k.ask : k.bid;
            int s = side ? k.AskSize : k.BidSize;
            if (k.symbol != symbol) return false;
            if (!fillOPG && TIF == "OPG") return false;
            if ((isLimit && side && (p <= price)) // buy limit
                || (isLimit && !side && (p >= price))// sell limit
                || (isStop && side && (p >= stopp)) // buy stop
                || (isStop && !side && (p <= stopp)) // sell stop
                || isMarket)
            {
                this.xprice = p;
                this.xsize = (s >= UnsignedSize ? UnsignedSize : s) * (side ? 1 : -1);
                this.xtime = k.time;
                this.xdate = k.date;
                return true;
            }
            return false;
        }
        /// <summary>
        /// fill against bid and ask rather than trade
        /// </summary>
        /// <param name="k"></param>
        /// <param name="OPG"></param>
        /// <returns></returns>
        public bool FillBidAsk(Tick k, bool OPG)
        {
            return Fill(k, true, OPG);
        }
        /// <summary>
        /// fill against bid and ask rather than trade
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool FillBidAsk(Tick k)
        {
            return Fill(k, true, false);
        }

        /// <summary>
        /// Try to fill incoming order against this order.  If orders match.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>order can be cast to valid Trade and function returns true.  Otherwise, false</returns>
        public bool Fill(Order o)
        {
            // sides must match
            if (side==o.side) return false;
            // orders must be valid
            if (!o.isValid || !this.isValid) return false;
            // acounts must be different
            if (o.Account == Account) return false;
            if ((isLimit && side && (o.price <= price)) // buy limit cross
                || (isLimit && !side && (o.price >= price))// sell limit cross
                || (isStop && side && (o.price >= stopp)) // buy stop
                || (isStop && !side && (o.price <= stopp)) // sell stop
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
            string[] r = new string[] { o.symbol, (o.side ? "true" : "false"), (o.UnsignedSize * (o.side ? 1 : -1)).ToString(), o.price.ToString(System.Globalization.CultureInfo.InvariantCulture), o.stopp.ToString(System.Globalization.CultureInfo.InvariantCulture), o.comment, o.ex, o.Account, o.Security.ToString(), o.Currency.ToString(), o.LocalSymbol, o.id.ToString(), o.TIF, o.date.ToString(), o.time.ToString(), "", o.trail.ToString(System.Globalization.CultureInfo.InvariantCulture) };
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
            int size = Math.Abs(Convert.ToInt32(rec[(int)OrderField.Size])) * (side ? 1 : -1);
            decimal oprice = Convert.ToDecimal(rec[(int)OrderField.Price], System.Globalization.CultureInfo.InvariantCulture);
            decimal ostop = Convert.ToDecimal(rec[(int)OrderField.Stop], System.Globalization.CultureInfo.InvariantCulture);
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
            o.id = Convert.ToInt64(rec[(int)OrderField.OrderID]);
            o.TIF = rec[(int)OrderField.OrderTIF];
            decimal trail = 0;
            if (decimal.TryParse(rec[(int)OrderField.Trail], System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out trail))
                o.trail = trail;
            o.date = Convert.ToInt32(rec[(int)OrderField.oDate]);
            o.time = Convert.ToInt32(rec[(int)OrderField.oTime]);
            return o;
        }

        public static long Unique { get { return DateTime.Now.Ticks; } }
    }



}

using System;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// A tick is both the smallest unit of time and the most simple unit of data in TradeLink (and the markets)
    /// It is an abstract container for last trade, last trade size, best bid, best offer, bid and offer sizes.
    /// </summary>
    [Serializable]
    public struct TickImpl : TradeLink.API.Tick
    {
        int _symidx;
        public int symidx { get { return _symidx; } set { _symidx = value; } }
        public string symbol { get { return _sym; } set { _sym = value; } }
        public int size { get { return _size; } set { _size = value; } }
        public int depth { get { return _depth; } set { _depth = value; } }
        public int date { get { return _date; } set { _date = value; } }
        public int time { get { return _time; } set { _time = value; } }
        public long datetime { get { return _datetime; } set { _datetime = value; } }
        /// <summary>
        /// normal bid size (size/100 for equities, /1 for others)
        /// </summary>
        public int bs { get { return _bs; } set { _bs = value; } }
        /// <summary>
        /// normal ask size (size/100 for equities, /1 for others)
        /// </summary>
        public int os { get { return _os; } set { _os = value; } }
        public decimal trade { get { return _trade*Const.IPRECV; } set { _trade = (ulong)(value*Const.IPREC); } }
        public decimal bid { get { return _bid * Const.IPRECV; } set { _bid = (ulong)(value * Const.IPREC); } }
        public decimal ask { get { return _ask * Const.IPRECV; } set { _ask = (ulong)(value * Const.IPREC); } }
        public string ex { get { return _ex; } set { _ex = value; } }
        public string be { get { return _be; } set { _be = value; } }
        public string oe { get { return _oe; } set { _oe = value; } }
        public bool isIndex { get { return _size < 0; } }
        public bool hasBid { get { return (_bid != 0) && (_bs != 0); } }
        public bool hasAsk { get { return (_ask != 0) && (_os != 0); } }
        public bool isFullQuote { get { return hasBid && hasAsk; } }
        public bool isQuote { get { return (!isTrade && (hasBid || hasAsk)); } }
        public bool isTrade { get { return (_trade != 0) && (_size> 0); } }
        public bool hasTick { get { return (this.isTrade || hasBid || hasAsk); } }
        public bool isValid { get { return (_sym!= "") && (isIndex || hasTick); } }
        public bool atHigh(decimal high) { return (isTrade && (_trade>=high)); }
        public bool atLow(decimal low) { return (isTrade && (_trade <= low)); }
        /// <summary>
        /// tick.bs*100 (only for equities)
        /// </summary>
        public int BidSize { get { return _bs * 100; } set { _bs = (int)((double)value / 100); } }
        /// <summary>
        /// tick.os*100 (only for equities)
        /// </summary>
        public int AskSize { get { return _os * 100; } set { _os = (int)((double)value / 100); } }
        public int TradeSize { get { return ts*100; } set { _size = (int)(value / 100); } }
        public int ts { get { return _size / 100; } } // normalized to bs/os
        internal long _datetime;
        Security _Sec;
        string _sym;
        string _be;
        string _oe;
        string _ex;
        int _bs;
        int _os;
        int _size;
        int _depth;

        int _date;
        int _time;
        internal ulong _trade;
        internal ulong _bid;
        internal ulong _ask;

        public TickImpl(string symbol) 
        {
            _Sec = new SecurityImpl(symbol);
            _sym = symbol;
            _be = "";
            _oe = "";
            _ex = "";
            _bs = 0;
            _os = 0;
            _size = 0;
            _depth = 0;
            _date = 0;
            _time = 0;
            _trade = 0;
            _bid = 0;
            _ask = 0;
            _datetime = 0;
            _symidx = 0;
        }
        public static TickImpl Copy(Tick c)
        {
            TickImpl k = new TickImpl();
            if (c.symbol != "") k.symbol = c.symbol;
            k.time = c.time;
            k.date = c.date;
            k.datetime = c.datetime;
            k.size = c.size;
            k.depth = c.depth;
            k.trade = c.trade;
            k.bid = c.bid;
            k.ask = c.ask;
            k.bs = c.bs;
            k.os = c.os;
            k.be = c.be;
            k.oe = c.oe;
            k.ex = c.ex;
            k.symidx = c.symidx;
            return k;
        }
        /// <summary>
        /// this constructor creates a new tick by combining two ticks
        /// this is to handle tick updates that only provide bid/ask changes.
        /// </summary>
        /// <param name="a">old tick</param>
        /// <param name="b">new tick or update</param>
        public static Tick Copy(TickImpl a, TickImpl b)
        {
            TickImpl k = new TickImpl();
            if (b.symbol != a.symbol) return k; // don't combine different symbols
            if (b.time < a.time) return k; // don't process old updates
            k.time = b.time;
            k.date = b.date;
            k.datetime = b.datetime;
            k.symbol = b.symbol;
            k.depth = b.depth;
            k.symidx = b.symidx;
            if (b.isTrade)
            {
                k.trade = b.trade;
                k.size = b.size;
                k.ex = b.ex;
                //
                k.bid = a.bid;
                k.ask = a.ask;
                k.os = a.os;
                k.bs = a.bs;
                k.be = a.be;
                k.oe = a.oe;
            }
            if (b.hasAsk && b.hasBid)
            {
                k.bid = b.bid;
                k.ask = b.ask;
                k.bs = b.bs;
                k.os = b.os;
                k.be = b.be;
                k.oe = b.oe;
                //
                k.trade = a.trade;
                k.size = a.size;
                k.ex = a.ex;
            }
            else if (b.hasAsk)
            {
                k.ask = b.ask;
                k.os = b.os;
                k.oe = b.oe;
                //
                k.bid = a.bid;
                k.bs = a.bs;
                k.be = a.be;
                k.trade = a.trade;
                k.size = a.size;
                k.ex = a.ex;
            }
            else if (b.hasBid)
            {
                k.bid = b.bid;
                k.bs = b.bs;
                k.be = b.be;
                //
                k.ask = a.ask;
                k.os = a.os;
                k.oe = a.oe;
                k.trade = a.trade;
                k.size = a.size;
                k.ex = a.ex;
            }
            return k;
        }

        public override string ToString()
        {
            if (!this.hasTick) return "";
            if (this.isTrade) return symbol+" "+this.size + "@" + this.trade + " " + this.ex;
            else return symbol+" "+this.bid + "x" + this.ask + " (" + this.bs + "x" + this.os + ") " + this.be + "x" + this.oe;
        }

        public static string Serialize(Tick t)
        {
            const char d = ',';
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(t.symbol);
            sb.Append(d);
            sb.Append(t.date);
            sb.Append(d);
            sb.Append(t.time);
            sb.Append(d);
            // unused field
            sb.Append(d);
            sb.Append(t.trade.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(t.size); 
            sb.Append(d);
            sb.Append(t.ex);
            sb.Append(d);
            sb.Append(t.bid.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(t.ask.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(t.bs);
            sb.Append(d);
            sb.Append(t.os);
            sb.Append(d);
            sb.Append(t.be);
            sb.Append(d);
            sb.Append(t.oe);
            sb.Append(d);
            sb.Append(t.depth);
            return sb.ToString();
        }

        public static Tick Deserialize(string msg)
        {
            string [] r = msg.Split(',');
            Tick t = new TickImpl();
            decimal d = 0;
            int i = 0;
            t.symbol = r[(int)TickField.symbol];
            if (decimal.TryParse(r[(int)TickField.trade], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                t.trade = d;
            if (decimal.TryParse(r[(int)TickField.bid],  System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture,out d))
                t.bid = d;
            if (decimal.TryParse(r[(int)TickField.ask], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                t.ask = d;
            if (int.TryParse(r[(int)TickField.tsize], out i))
                t.size = i;
            if (int.TryParse(r[(int)TickField.asksize], out i))
                t.os= i;
            if (int.TryParse(r[(int)TickField.bidsize], out i))
                t.bs = i;
            if (int.TryParse(r[(int)TickField.time], out i))
                t.time = i;
            if (int.TryParse(r[(int)TickField.date], out i))
                t.date = i;
            if (int.TryParse(r[(int)TickField.tdepth], out i))
                t.depth = i;
            t.ex = r[(int)TickField.tex];
            t.be = r[(int)TickField.bidex];
            t.oe = r[(int)TickField.askex];
            t.datetime = t.date * 1000000 + t.time;
            return t;
        }

        public void SetQuote(int date, int time, int sec, decimal bid, decimal ask, int bidsize, int asksize, string bidex, string askex)
        {
        	this.date = date;
        	this.time = time;
        	this.bid = bid;
        	this.ask = ask;
        	this.bs = bidsize;
        	this.os = asksize;
        	this.be = bidex;
        	this.oe = askex;
        	this.trade =0;
        	this.size = 0;
            this.depth = 0;
        }
        //overload with depth field
        public void SetQuote(int date, int time, int sec, decimal bid, decimal ask, int bidsize, int asksize, string bidex, string askex, int depth)
        {
            this.date = date;
            this.time = time;
            this.bid = bid;
            this.ask = ask;
            this.bs = bidsize;
            this.os = asksize;
            this.be = bidex;
            this.oe = askex;
            this.trade = 0;
            this.size = 0;
            this.depth = depth;
        }
        //date, time, sec, Convert.ToDecimal(r[(int)T.PRICE]), isize, r[(int)T.EXCH]
        public void SetTrade(int date, int time, int sec, decimal price, int size, string exch)
        {
        	this.ex = exch;
        	this.date = date;
        	this.time = time;
        	this.trade = price;
        	this.size = size;
        	this.bid = 0;
        	this.ask = 0;
        	this.os = 0;
        	this.bs = 0;
        }


        public static TickImpl NewBid(string sym, decimal bid, int bidsize) { return NewQuote(sym, Util.ToTLDate(DateTime.Now), Util.ToTLTime(DateTime.Now),  bid, 0, bidsize, 0, "", ""); }
        public static TickImpl NewAsk(string sym, decimal ask, int asksize) { return NewQuote(sym, Util.ToTLDate(DateTime.Now), Util.ToTLTime(DateTime.Now),  0, ask, 0, asksize, "", ""); }
        public static TickImpl NewQuote(string sym, decimal bid, decimal ask, int bidsize, int asksize, string be, string oe) { return NewQuote(sym, Util.ToTLDate(DateTime.Now), Util.ToTLTime(DateTime.Now), bid, ask, bidsize, asksize, be, oe); }
        public static TickImpl NewQuote(string sym, int date, int time, decimal bid, decimal ask, int bidsize, int asksize, string be, string oe)
        {
            TickImpl q = new TickImpl(sym);
            q.date = date;
            q.time = time;
            q.bid = bid;
            q.ask = ask;
            q.be = be.Trim();
            q.oe = oe.Trim();
            q.AskSize = asksize;
            q.BidSize= bidsize;
            q.trade = 0;
            q.size = 0;
            q.depth = 0;
            return q;
        }
        //methods overloaded with depth field
        public static TickImpl NewBid(string sym, decimal bid, int bidsize, int depth) { return NewQuote(sym, Util.ToTLDate(DateTime.Now), Util.ToTLTime(DateTime.Now),bid, 0, bidsize, 0, "", "", depth); }
        public static TickImpl NewAsk(string sym, decimal ask, int asksize, int depth) { return NewQuote(sym, Util.ToTLDate(DateTime.Now), Util.ToTLTime(DateTime.Now),0,ask, 0, asksize, "", "", depth); }
        public static TickImpl NewQuote(string sym, int date, int time, decimal bid, decimal ask, int bidsize, int asksize, string be, string oe, int depth)
        {
            TickImpl q = new TickImpl(sym);
            q.date = date;
            q.time = time;
            q.bid = bid;
            q.ask = ask;
            q.be = be.Trim();
            q.oe = oe.Trim();
            q.AskSize = asksize;
            q.BidSize = bidsize;
            q.trade = 0;
            q.size = 0;
            q.depth = depth;
            return q;
        }

        public static TickImpl NewTrade(string sym, decimal trade, int size) { return NewTrade(sym, Util.ToTLDate(DateTime.Now), Util.ToTLTime(DateTime.Now),  trade, size, ""); }
        public static TickImpl NewTrade(string sym, int date, int time, decimal trade, int size, string ex)
        {
            TickImpl t = new TickImpl(sym);
            t.date = date;
            t.time = time;
            t.trade = trade;
            t.size = size;
            t.ex = ex.Trim();
            t.bid = 0;
            return t;
        }



    }

    enum TickField
    { // tick message fields from TL server
        symbol = 0,
        date,
        time,
        KUNUSED,
        trade,
        tsize,
        tex,
        bid,
        ask,
        bidsize,
        asksize,
        bidex,
        askex,
        tdepth,
    }
}

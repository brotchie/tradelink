using System;

namespace TradeLib
{
    /// <summary>
    /// A tick is both the smallest unit of time and the most simple unit of data in TradeLink (and the markets)
    /// It is an abstract container for last trade, last trade size, best bid, best offer, bid and offer sizes.
    /// </summary>
    [Serializable]
    public class Tick
    {
        public string sym = "";
        public int factor = 100;
        public int time;
        public int date;
        public int sec;
        public int size;
        public decimal trade;
        public decimal bid;
        public decimal ask;
        public int bs;
        public int os;
        public string be;
        public string oe;
        public string ex;
        public bool IndexTick { get { return size < 0; } }
        public bool hasBid { get { return (bid != 0) && (bs != 0); } }
        public bool hasAsk { get { return (ask != 0) && (os != 0); } }
        public bool FullQuote { get { return hasBid && hasAsk; } }
        public bool isQuote { get { return (!isTrade && (hasBid || hasAsk)); } }
        public bool isTrade { get { return (trade != 0) && (size != 0); } }
        public bool hasTick { get { return (this.isTrade || hasBid || hasAsk); } }
        public bool isValid { get { return (sym != "") && hasTick; } }
        public bool atHigh(decimal high) { return (isTrade && (trade>=high)); }
        public bool atLow(decimal low) { return (isTrade && (trade <= low)); }
        public bool atHigh(BoxInfo bi) { return (isTrade && (trade >= bi.High)); }
        public bool atLow(BoxInfo bi) { return (isTrade && (trade <= bi.Low)); }
        public int BidSize { get { return bs * 100; } set { bs = (int)(value / 100); } }
        public int AskSize { get { return os * 100; } set { os = (int)(value / 100); } }
        public int TradeSize { get { return ts*100; } set { size = (int)(value / 100); } }
        public int ts { get { return size / 100; } } // normalized to bs/os
        public Tick() { }
        public Tick(string symbol) { this.sym = symbol; }
        public Tick(Tick c)
        {
            if (c.sym!="") sym = c.sym;
            time = c.time;
            date = c.date;
            sec = c.sec;
            size = c.size;
            trade = c.trade; 
            bid = c.bid; 
            ask = c.ask;
            bs = c.bs;
            os = c.os;
            be = c.be;
            oe = c.oe;
            ex = c.ex;
        }
        public Tick(Tick a, Tick b)
        {   // this constructor creates a new tick by combining two ticks
            // this is to handle tick updates that only provide bid/ask changes (like anvil)

            // a = old tick, b = new tick or tick update
            if (b.sym != a.sym) return; // don't combine different symbols
            if (b.time < a.time) return; // don't process old updates
            time = b.time;
            date = b.date;
            sec = b.sec;
            sym = b.sym;

            if (b.isTrade)
            {
                trade = b.trade;
                size = b.size;
                ex = b.ex;
                //
                bid = a.bid;
                ask = a.ask;
                os = a.os;
                bs = a.bs;
                be = a.be;
                oe = a.oe;
            }
            else if (b.hasAsk && b.hasBid)
            {
                bid = b.bid;
                ask = b.ask;
                bs = b.bs;
                os = b.os;
                be = b.be;
                oe = b.oe;
                //
                trade = a.trade;
                size = a.size;
                ex = a.ex;
            }
            else if (b.hasAsk)
            {
                ask = b.ask;
                os = b.os;
                oe = b.oe;
                //
                bid = a.bid;
                bs = a.bs;
                be = a.be;
                trade = a.trade;
                size = a.size;
                ex = a.ex;
            }
            else if (b.hasBid)
            {
                bid = b.bid;
                bs = b.bs;
                be = b.be;
                //
                ask = a.ask;
                os = a.os;
                oe = a.oe;
                trade = a.trade;
                size = a.size;
                ex = a.ex;
            }
        }

        public override string ToString()
        {
            if (!this.hasTick) return "";
            if (this.isTrade) return sym+" "+this.size + "@" + this.trade + " " + this.ex;
            else return sym+" "+this.bid + "x" + this.ask + " (" + this.bs + "x" + this.os + ") " + this.be + "," + this.oe;
        }

        public string Serialize()
        {
            Tick t = this;
            const char d = ',';
            string s = t.sym + d + t.date + d + t.time + d + t.sec + d + t.trade + d + t.size + d + t.ex + d + t.bid + d + t.ask + d + t.bs + d + t.os + d + t.be + d + t.oe + d;
            return s;
        }

        public static Tick Deserialize(string msg)
        {
            string [] r = msg.Split(',');
            Tick t = new Tick();
            t.sym = r[(int)TickField.symbol];
            t.trade = Convert.ToDecimal(r[(int)TickField.trade]);
            t.size = Convert.ToInt32(r[(int)TickField.tsize]);
            t.bid = Convert.ToDecimal(r[(int)TickField.bid]);
            t.ask = Convert.ToDecimal(r[(int)TickField.ask]);
            t.os = Convert.ToInt32(r[(int)TickField.asksize]);
            t.bs = Convert.ToInt32(r[(int)TickField.bidsize]);
            t.ex = r[(int)TickField.tex];
            t.be = r[(int)TickField.bidex];
            t.oe = r[(int)TickField.askex];
            t.time = Convert.ToInt32(r[(int)TickField.time]);
            t.sec = Convert.ToInt32(r[(int)TickField.sec]);
            t.date = Convert.ToInt32(r[(int)TickField.date]);
            return t;
        }

        public void SetQuote(int date, int time, int sec, decimal bid, decimal ask, int bidsize, int asksize, string bidex, string askex)
        {
        	this.date = date;
        	this.time = time;
        	this.sec = sec;
        	this.bid = bid;
        	this.ask = ask;
        	this.bs = bidsize;
        	this.os = asksize;
        	this.be = bidex;
        	this.oe = askex;
        	this.trade =0;
        	this.size = 0;
        }
        //date, time, sec, Convert.ToDecimal(r[(int)T.PRICE]), isize, r[(int)T.EXCH]
        public void SetTrade(int date, int time, int sec, decimal price, int size, string exch)
        {
        	this.ex = exch;
        	this.date = date;
        	this.time = time;
        	this.sec = sec;
        	this.trade = price;
        	this.size = size;
        	this.bid = 0;
        	this.ask = 0;
        	this.os = 0;
        	this.bs = 0;
        }


        public static Tick NewBid(string sym, decimal bid, int bidsize) { return NewQuote(sym, 0, 0, 0, bid, 0, bidsize, 0, "", ""); }
        public static Tick NewAsk(string sym, decimal ask, int asksize) { return NewQuote(sym, 0, 0, 0, 0, ask, 0, asksize, "", ""); }
        public static Tick NewQuote(string sym, int date, int time, int sec, decimal bid, decimal ask, int bidsize, int asksize, string be, string oe)
        {
            Tick q = new Tick(sym);
            q.date = date;
            q.time = time;
            q.sec = sec;
            q.bid = bid;
            q.ask = ask;
            q.be = be.Trim();
            q.oe = oe.Trim();
            q.AskSize = asksize;
            q.BidSize= bidsize;
            q.trade = 0;
            q.size = 0;
            return q;
        }

        public static Tick NewTrade(string sym, decimal trade, int size) { return NewTrade(sym, 0, 0, 0, trade, size, ""); }
        public static Tick NewTrade(string sym, int date, int time, int sec, decimal trade, int size, string ex)
        {
            Tick t = new Tick(sym);
            t.date = date;
            t.time = time;
            t.sec = sec;
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
        sec,
        trade,
        tsize,
        tex,
        bid,
        ask,
        bidsize,
        asksize,
        bidex,
        askex,
    }
}

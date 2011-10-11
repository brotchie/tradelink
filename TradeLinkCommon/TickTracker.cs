using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// keep track of bid/ask and last data for symbols
    /// </summary>
    public class TickTracker : GenericTrackerI, GotTickIndicator
    {
        public void GotTick(Tick k)
        {
            newTick(k);
        }
        public Type TrackedType
        {
            get
            {
                return typeof(Tick);
            }
        }

        /// <summary>
        /// gets decimal value of last trade price for given index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public decimal ValueDecimal(int idx)
        {
            return (decimal)this[idx].trade;
        }

        /// <summary>
        /// gets decimal value of last trade price given label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public decimal ValueDecimal(string txt)
        {
            return (decimal)this[txt].trade;
        }

        public object Value(int idx) { return this[idx]; }
        public object Value(string txt) { return this[txt]; }

        public void Clear()
        {
            bid.Clear();
            ask.Clear();
            bs.Clear();
            be.Clear();
            oe.Clear();
            os.Clear();
            ts.Clear();
            ex.Clear();
            date.Clear();
            time.Clear();
            last.Clear();
        }
        int _estlabels = 10;
        /// <summary>
        /// create ticktracker
        /// </summary>
        public TickTracker() : this(10) { }
        /// <summary>
        /// create ticktracker with some approximate # of symbols to track
        /// </summary>
        /// <param name="estlabels"></param>
        public TickTracker(int estlabels)
        {
            _estlabels = estlabels;
            bid = new GenericTracker<decimal>(_estlabels);
            ask = new GenericTracker<decimal>(_estlabels);
            last = new GenericTracker<decimal>(_estlabels);
            bs = new GenericTracker<int>(_estlabels);
            be = new GenericTracker<string>(_estlabels);
            oe = new GenericTracker<string>(_estlabels);
            os = new GenericTracker<int>(_estlabels);
            ts = new GenericTracker<int>(_estlabels);
            ex = new GenericTracker<string>(_estlabels);
            date = new GenericTracker<int>(_estlabels);
            time = new GenericTracker<int>(_estlabels);
            // setup generic trackers to track tick information
            last.NewTxt += new TextIdxDelegate(last_NewTxt);
        }
        public TickTracker(string name) { _name = name; }
        

        /// <summary>
        /// called when new text label is added
        /// </summary>
        public event TextIdxDelegate NewTxt;

        void last_NewTxt(string txt, int idx)
        {
            date.addindex(txt, 0);
            time.addindex(txt, 0);
            bid.addindex(txt, 0);
            ask.addindex(txt, 0);
            bs.addindex(txt, 0);
            os.addindex(txt, 0);
            ts.addindex(txt, 0);
            ex.addindex(txt, string.Empty);
            be.addindex(txt, string.Empty);
            oe.addindex(txt, string.Empty);
            if (NewTxt!=null)
                NewTxt(txt,idx);
        }


        GenericTracker<int> date;
        GenericTracker<int> time;
        GenericTracker<decimal> bid;
        GenericTracker<decimal> ask;
        GenericTracker<decimal> last;
        GenericTracker<int> bs;
        GenericTracker<int> os;
        GenericTracker<int> ts;
        GenericTracker<string> be;
        GenericTracker<string> oe;
        GenericTracker<string> ex;

        public string Display(int idx) { return this[idx].ToString(); }
        public string Display(string txt) { return this[txt].ToString(); }

        public string getlabel(int idx) { return last.getlabel(idx); }

        string _name = "TICKS";
        public string Name { get { return _name; } set { _name = value; } }

        public int Count { get { return last.Count; } }

        /// <summary>
        /// track a new symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public int addindex(string symbol)
        {
            return last.addindex(symbol, 0);
        }
        /// <summary>
        /// get index of an existing symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public int getindex(string symbol)
        {
            return last.getindex(symbol);
        }



        /// <summary>
        /// get the bid
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public decimal Bid(int idx) { return bid[idx]; }
        /// <summary>
        /// get the bid
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public decimal Bid(string sym) { return bid[sym]; }
        /// <summary>
        /// get the ask
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public decimal Ask(int idx) { return ask[idx]; }
        /// <summary>
        /// get the ask
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public decimal Ask(string sym) { return ask[sym]; }
        /// <summary>
        /// get the last trade
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public decimal Last(int idx) { return last[idx]; }
        /// <summary>
        /// get the last trade
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public decimal Last(string sym) { return last[sym]; }
        /// <summary>
        /// whether we have a bid
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool HasBid(int idx) { return bid[idx] != 0; }
        /// <summary>
        /// whether we have a bid
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool HasBid(string sym) { return bid[sym] != 0; }
        /// <summary>
        /// whether we have a ask
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool HasAsk(string sym) { return ask[sym] != 0; }
        /// <summary>
        /// whether we have a ask
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool HasAsk(int idx) { return ask[idx] != 0; }
        /// <summary>
        /// whether we have a last price
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool HasLast(int idx) { return last[idx] != 0; }
        /// <summary>
        /// whether we have a last price
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool HasLast(string sym) { return last[sym] != 0; }
        /// <summary>
        /// whether we have a bid/ask and last
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool HasAll(string sym) { return HasBid(sym) && HasAsk(sym) && HasLast(sym); }
        /// <summary>
        /// whether we have a bid/ask and last
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool HasAll(int idx) { return HasBid(idx) && HasAsk(idx) && HasLast(idx); }
        /// <summary>
        /// whether we have a bid/ask
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool HasQuote(string sym) { return HasBid(sym) && HasAsk(sym); }
        /// <summary>
        /// whether we have a bid/ask
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool HasQuote(int idx) { return HasBid(idx) && HasAsk(idx); }
        /// <summary>
        /// get a tick in tick format
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public Tick Tick(int idx)
        {
            return this[idx];
        }
        /// <summary>
        /// get a tick in tick format
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public Tick Tick(string sym)
        {
            return this[sym];
        }
        /// <summary>
        /// get a tick in tick format
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public Tick this[int idx]
        {
            get
            {
                Tick k = new TickImpl(last.getlabel(idx));
                k.date = date[idx];
                k.time = time[idx];
                k.trade = last[idx];
                k.size = ts[idx];
                k.ex = ex[idx];
                k.bid = bid[idx];
                k.bs = bs[idx];
                k.be = be[idx];
                k.ask = ask[idx];
                k.os = os[idx];
                k.oe = oe[idx];
                return k;
            }
        }
        /// <summary>
        /// get a tick in tick format
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public Tick this[string sym]
        {
            get
            {
                int idx = last.getindex(sym);
                if (idx < 0) return new TickImpl();
                Tick k = new TickImpl(last.getlabel(idx));
                k.date = date[idx];
                k.time = time[idx];
                k.trade = last[idx];
                k.size = ts[idx];
                k.ex = ex[idx];
                k.bid = bid[idx];
                k.bs = bs[idx];
                k.be = be[idx];
                k.ask = ask[idx];
                k.os = os[idx];
                k.oe = oe[idx];
                return k;
            }
        }

        /// <summary>
        /// update the tracker with a new tick
        /// </summary>
        /// <param name="k"></param>
        /// <param name="idx"></param>
        public void newTick(Tick k, int idx)
        {
            if (idx < 0) 
                return;
            // update date/time
            time[idx] = k.time;
            date[idx] = k.date;
            // update bid/ask/last
            if (k.isTrade)
            {
                last[idx] = k.trade;
                ex[idx] = k.ex;
                ts[idx] = k.size;
            }
            if (k.hasAsk)
            {
                ask[idx] = k.ask;
                oe[idx] = k.oe;
                os[idx] = k.os;
            }
            if (k.hasBid)
            {
                bid[idx] = k.bid;
                bs[idx] = k.bs;
                be[idx] = k.be;
            }
        }
        /// <summary>
        /// update the tracker with a new tick
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool newTick(Tick k)
        {
            // get index
            int idx = getindex(k.symbol);
            // add if unknown
            if (idx < 0)
                idx = addindex(k.symbol);
            // update date/time
            time[idx] = k.time;
            date[idx] = k.date;
            // update bid/ask/last
            if (k.isTrade)
            {
                last[idx] = k.trade;
                ex[idx] = k.ex;
                ts[idx] = k.size;
            }
            if (k.hasAsk)
            {
                ask[idx] = k.ask;
                oe[idx] = k.oe;
                os[idx] = k.os;
            }
            if (k.hasBid)
            {
                bid[idx] = k.bid;
                bs[idx] = k.bs;
                be[idx] = k.be;
            }
            return true;
        }
    }
    /// <summary>
    /// track only bid price
    /// </summary>
    public class BidTracker : GenericTracker<decimal>, GenericTrackerDecimal, GotTickIndicator
    {
        public BidTracker() : base("BID") { }
        public decimal getvalue(int idx) { return this[idx]; }
        public decimal getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, decimal v) { this[idx] = v; }
        public int addindex(string txt, decimal v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            if (!k.hasBid) return;
            int idx = addindex(k.symbol);
            this[idx] = k.bid;
        }
    }
    /// <summary>
    /// track only ask price
    /// </summary>
    public class AskTracker : GenericTracker<decimal>, GenericTrackerDecimal, GotTickIndicator
    {
        public AskTracker() : base("ASK") { }
        public decimal getvalue(int idx) { return this[idx]; }
        public decimal getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, decimal v) { this[idx] = v; }
        public int addindex(string txt, decimal v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            if (!k.hasAsk) return;
            int idx = addindex(k.symbol);
            this[idx] = k.ask;
        }
    }
    /// <summary>
    /// track only last price
    /// </summary>
    public class LastTracker : GenericTracker<decimal>, GenericTrackerDecimal, GotTickIndicator
    {
        public LastTracker() : base("LAST") { }
        public decimal getvalue(int idx) { return this[idx]; }
        public decimal getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, decimal v) { this[idx] = v; }
        public int addindex(string txt, decimal v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            if (!k.isTrade) return;
            int idx = addindex(k.symbol);
            this[idx] = k.trade;
        }
    }

    /// <summary>
    /// track only last trade size
    /// </summary>
    public class SizeTracker : GenericTracker<int>, GenericTrackerInt, GotTickIndicator
    {
        public SizeTracker() : base("SIZE") { }
        public int getvalue(int idx) { return this[idx]; }
        public int getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, int v) { this[idx] = v; }
        public int addindex(string txt, int v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            if (!k.isTrade) return;
            int idx = addindex(k.symbol);
            this[idx] = k.size;
        }
    }

    /// <summary>
    /// track only last bid size
    /// </summary>
    public class BidSizeTracker : GenericTracker<int>, GenericTrackerInt, GotTickIndicator
    {
        public BidSizeTracker() : base("BIDSIZE") { }
        public int getvalue(int idx) { return this[idx]; }
        public int getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, int v) { this[idx] = v; }
        public int addindex(string txt, int v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            if (!k.hasBid) return;
            int idx = addindex(k.symbol);
            this[idx] = k.bs;
        }
    }

    /// <summary>
    /// track only last ask size
    /// </summary>
    public class AskSizeTracker : GenericTracker<int>, GenericTrackerInt, GotTickIndicator
    {
        public AskSizeTracker() : base("ASKSIZE") { }
        public int getvalue(int idx) { return this[idx]; }
        public int getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, int v) { this[idx] = v; }
        public int addindex(string txt, int v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            if (!k.hasAsk) return;
            int idx = addindex(k.symbol);
            this[idx] = k.os;
        }
    }
    /// <summary>
    /// whether last tick in given symbol was a trade
    /// </summary>
    public class IsTradeTracker : GenericTracker<bool>, GenericTrackerBool, GotTickIndicator
    {
        public IsTradeTracker() : base("ISTRADE") {}
        public bool getvalue(int idx) { return this[idx]; }
        public bool getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, bool v) { this[idx] = v; }
        public int addindex(string txt, bool v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            int idx = addindex(k.symbol);
            this[idx] = k.isTrade;
        }
    }
    /// <summary>
    /// whether last tick in given symbol had bid information
    /// </summary>
    public class IsBidTracker : GenericTracker<bool>, GenericTrackerBool, GotTickIndicator
    {
        public IsBidTracker() : base("ISBID") { }
        public bool getvalue(int idx) { return this[idx]; }
        public bool getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, bool v) { this[idx] = v; }
        public int addindex(string txt, bool v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            int idx = addindex(k.symbol);
            this[idx] = k.hasBid;
        }
    }
    /// <summary>
    /// whether last tick in given symbol had ask information
    /// </summary>
    public class IsAskTracker : GenericTracker<bool>, GenericTrackerBool, GotTickIndicator
    {
        public IsAskTracker() : base("ISASK") { }
        public bool getvalue(int idx) { return this[idx]; }
        public bool getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, bool v) { this[idx] = v; }
        public int addindex(string txt, bool v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            int idx = addindex(k.symbol);
            this[idx] = k.hasAsk;
        }
    }

}

using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// keep track of bid/ask and last data for symbols
    /// </summary>
    public class TickTracker : TickIndicator
    {

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
            // setup generic trackers to track tick information
            last.NewTxt += new TextIdxDelegate(last_NewTxt);
        }

        void last_NewTxt(string txt, int idx)
        {
            bid.addindex(txt, 0);
            ask.addindex(txt, 0);
        }

        GenericTracker<decimal> bid;
        GenericTracker<decimal> ask;
        GenericTracker<decimal> last;
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
                k.trade = last[idx];
                k.bid = bid[idx];
                k.ask = ask[idx];
                k.os = 1;
                k.bs = 1;
                k.size = 1;
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
                k.trade = last[idx];
                k.bid = bid[idx];
                k.ask = ask[idx];
                k.os = 1;
                k.bs = 1;
                k.size = 1;
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
            if (idx < 0) return;
            // update bid/ask/last
            if (k.isTrade)
                last[idx] = k.trade;
            if (k.hasAsk)
                ask[idx] = k.ask;
            if (k.hasBid)
                bid[idx] = k.bid;
        }
        /// <summary>
        /// update the tracker with a new tick
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool newTick(Tick k)
        {
            int idx = last.getindex(k.symbol);
            if (idx < 0) return false;
            // update bid/ask/last
            if (k.isTrade)
                last[idx] = k.trade;
            if (k.hasAsk)
                ask[idx] = k.ask;
            if (k.hasBid)
                bid[idx] = k.bid;
            return true;
        }
    }
}

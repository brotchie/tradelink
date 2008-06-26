using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    /// <summary>
    /// Used to describe trends between consequtive or sequential tick instances.
    /// </summary>
    public class TickLink : TickIndicator
    { // don't have high and low yet
        private Tick a = null;
        private Tick b = null;
        /// <summary>
        /// Gets or sets the A tick, or the most recent tick.
        /// </summary>
        /// <value>The A.</value>
        public Tick A { get { return a; } set { a = value; } }
        /// <summary>
        /// Gets or sets the B tick, the older tick.
        /// </summary>
        /// <value>The B.</value>
        public Tick B { get { return b; } set { b = value; } }
        private bool t { get { return a.isTrade; } }
        public decimal Ask { get { return _ask; } }
        public decimal Bid { get { return _bid; } }
        public bool Quoted { get { return (_ask != 0) || (_bid != 0); } }
        public bool FullQuote { get { return (_ask != 0) && (_bid != 0); } }
        public bool PlusTick { get { return Valid && t && (_last!=0) && (_last<a.trade); } }
        public bool MinusTick { get { return Valid && t && (_last!=0) && (_last>a.trade); } }
        public bool TakesOffer { get { return (Valid && t && b.hasAsk && (a.trade >= b.ask)); } }
        public bool HitsBid { get { return (Valid && t && b.hasBid && (a.trade <= b.bid)); } }
        public bool Perfect { get { return (Valid && t && ((a.ts >= b.os) && b.hasAsk) || ((a.ts >=b.bs) && b.hasBid)); } }
        public bool TakesOfferPerfect { get { return t && Valid && TakesOffer && Perfect; } }
        public bool HitsBidPerfect { get { return t && Valid && HitsBid && Perfect; } }
        public bool BidsOffer { get { return !t && Valid && a.hasBid && b.hasAsk && (a.bid >= b.ask); } }
        public bool OffersBid { get { return !t && Valid && a.hasAsk && b.hasBid && (a.ask <= b.bid); } }
        public bool BidsUp { get { return !t && Valid && a.hasBid && b.hasBid && (a.bid > b.bid); } }
        public bool BidsDown { get { return !t && Valid && a.hasBid && b.hasBid && (a.bid < b.bid); } }
        public bool OffersDown { get { return !t && Valid && (a.ask < b.ask); } }
        public bool OffersUp { get { return !t && Valid && a.hasAsk && b.hasAsk && (a.ask > b.ask); } }
        public bool OfferReload { get { return !t && Valid && a.hasAsk && b.hasAsk && (a.trade == b.ask) && (a.ask == b.ask); } }
        public bool BidReload { get { return !t && Valid && a.hasBid && b.hasBid && (a.trade == b.bid) && (a.bid == b.bid); } }
        public bool Valid { get { return (a != null) && (b != null) && a.hasTick && b.hasTick; } }
        private decimal _last;
        private decimal _bid;
        private decimal _ask;
        public virtual bool newTick(Tick tick) 
        {
            if (tick.hasAsk) _ask = tick.ask;
            if (tick.hasBid) _bid = tick.bid;
            if ((a!=null) && a.isTrade)
                _last = a.trade;
            if (a == null) a = new Tick(tick);
            else if (!a.hasTick) a = new Tick(a, tick);
            else if (a.hasTick)
            {
                b = new Tick(a);
                a = new Tick(tick);
            }
            return Valid; 
        }
        public override string ToString()
        {
            string s = "";
            if (!Valid) return s;
            if (PlusTick) s += "+Tick";
            if (MinusTick) s += "-Tick";
            if (TakesOffer) s += "TakesOffer";
            if (HitsBid) s += "HitsBid";
            if (Perfect) s += "Perfect";
            if (BidsOffer) s += "BidsOffer";
            if (OffersBid) s += "OffersBid";
            if (BidsUp) s += "BidsUp";
            if (BidsDown) s += "BidsDown";
            if (OffersDown) s += "OffersDown";
            if (OffersUp) s += "OffersUp";
            if (OfferReload) s += "OfferReload";
            if (BidReload) s += "BidReload";
            return s;
        }
    }
}

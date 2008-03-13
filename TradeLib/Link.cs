using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    public class TickLink
    { // don't have high and low yet
        private Tick a = null;
        private Tick b = null;
        public Tick A { get { return a; } set { a = value; } }
        public Tick B { get { return b; } set { b = value; } }
        private bool t { get { return a.isTrade; } }
        public bool PlusTick { get { return Valid && t && (a.trade>b.trade); } }
        public bool MinusTick { get { return Valid && t && (a.trade < b.trade); } }
        public bool TakesOffer { get { return (Valid && (a.trade >= b.ask)); } }
        public bool HitsBid { get { return (Valid && (a.trade <= b.bid)); } }
        public bool Perfect { get { return (Valid && t && (a.ts >= b.os) || (a.ts >=b.bs)); } }
        public bool TakesOfferPerfect { get { return t && Valid && TakesOffer && Perfect; } }
        public bool HitsBidPerfect { get { return t && Valid && HitsBid && Perfect; } }
        public bool BidsOffer { get { return !t && Valid && (a.bid >= b.ask); } }
        public bool OffersBid { get { return !t && Valid && (a.ask <= b.bid); } }
        public bool BidsUp { get { return !t && Valid && (a.bid > b.bid); } }
        public bool BidsDown { get { return !t && Valid && (a.bid < b.bid); } }
        public bool OffersDown { get { return !t && Valid && (a.ask < b.ask); } }
        public bool OffersUp { get { return !t && Valid && (a.ask > b.ask); } }
        public bool OfferReload { get { return !t && Valid && (a.trade == b.ask) && (a.ask == b.ask); } }
        public bool BidReload { get { return !t && Valid && (a.trade == b.bid) && (a.bid == b.bid); } }
        public bool Valid { get { return (a != null) && (b != null) && a.hasTick && b.hasTick; } }
        public virtual bool Tick(Tick tick) 
        {
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

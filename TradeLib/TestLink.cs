using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLib;

namespace TestTradeLib
{
	[TestFixture]
	public class TestLink
	{
		public TestLink()
		{
		}

        [Test]
        public void Basic()
        {
            const string s = "TST";
            const string x = "";
            Tick[] timesales = new Tick[] 
			{ 
				Tick.NewAsk(s,100.05m,100), //hasAsk
				Tick.NewBid(s,100m,100),//hasBid, FullQuoteLink
				Tick.NewBid(s,100.01m,100),//hasBid,BidsUp
				Tick.NewQuote(s,0,0,0,100.01m,100.05m,200,100,x,x),//FullQuoteTick
				Tick.NewTrade(s,0,0,0,100.01m,200,x),//isTrade,HisBidPerfect
				Tick.NewBid(s,99.99m,200),//hasBid
				Tick.NewTrade(s,0,0,0,99.99m,200,x),//isTrade,hitsBidPerfect,minustick
				Tick.NewBid(s,99.75m,100),//hasBid
				Tick.NewBid(s,99.75m,200),//hasBid
				Tick.NewAsk(s,99.95m,500),//hasOffer
				Tick.NewAsk(s,99.91m,100),//offersDown
				Tick.NewAsk(s,99.85m,200),//offersdown
				Tick.NewTrade(s,0,0,0,99.85m,200,x),//hitsofferperfect
                Tick.NewTrade(s,0,0,0,99.86m,100,x),//plustick
                Tick.NewTrade(s,0,0,0,99.86m,100,x),//isTrade
				Tick.NewAsk(s,99.83m,100),
			};
            TickLink t = new TickLink();
            int i = 0;
            Assert.That(!t.Valid);
            Assert.That(!t.FullQuote);
            Assert.That(!t.Quoted);
            t.newTick(timesales[i++]);
            Assert.That(t.Quoted);
            Assert.That(!t.FullQuote);
            Assert.That(!t.Valid);
            Assert.That(t.A.hasTick);
            Assert.That(t.B == null);
            t.newTick(timesales[i++]);
            Assert.That(t.Valid);
            Assert.That(t.Quoted && t.FullQuote);
            // tick link assertions for raised bid
            t.newTick(timesales[i++]);
            Assert.That(t.Ask-t.Bid == .04m);
            Assert.That(t.Valid);
            Assert.That(t.BidsUp);
            Assert.That(!t.BidsDown);
            Assert.That(!t.BidReload);
            Assert.That(!t.HitsBid);
            Assert.That(!t.HitsBidPerfect);
            Assert.That(!t.MinusTick);
            Assert.That(!t.OfferReload);
            Assert.That(!t.OffersBid);
            Assert.That(!t.OffersDown);
            Assert.That(!t.OffersUp);
            Assert.That(!t.Perfect);
            Assert.That(!t.PlusTick);
            Assert.That(!t.TakesOffer);
            Assert.That(!t.TakesOfferPerfect);

            // tick link assertions for restating last quote
            // as full with increased size
            t.newTick(timesales[i++]);
            Assert.That(t.Valid);
            Assert.That(!t.BidsUp);
            Assert.That(!t.BidsDown);
            Assert.That(!t.BidReload);
            Assert.That(!t.HitsBid);
            Assert.That(!t.HitsBidPerfect);
            Assert.That(!t.MinusTick);
            Assert.That(!t.OfferReload);
            Assert.That(!t.OffersBid);
            Assert.That(!t.OffersDown);
            Assert.That(!t.OffersUp);
            Assert.That(!t.Perfect);
            Assert.That(!t.PlusTick);
            Assert.That(!t.TakesOffer);
            Assert.That(!t.TakesOfferPerfect);

            // hitting the bid perfect
            t.newTick(timesales[i++]);
            Assert.That(t.Valid);
            Assert.That(!t.BidsUp);
            Assert.That(!t.BidsDown);
            Assert.That(!t.BidReload);
            Assert.That(t.HitsBid);
            Assert.That(t.HitsBidPerfect);
            Assert.That(!t.MinusTick);
            Assert.That(!t.OfferReload);
            Assert.That(!t.OffersBid);
            Assert.That(!t.OffersDown);
            Assert.That(!t.OffersUp);
            Assert.That(t.Perfect);
            Assert.That(!t.PlusTick);
            Assert.That(!t.TakesOffer);
            Assert.That(!t.TakesOfferPerfect);

            // nothing
            t.newTick(timesales[i++]);
            Assert.That(t.Valid);
            Assert.That(!t.BidsUp);
            Assert.That(!t.BidsDown);
            Assert.That(!t.BidReload);
            Assert.That(!t.HitsBid);
            Assert.That(!t.HitsBidPerfect);
            Assert.That(!t.MinusTick);
            Assert.That(!t.OfferReload);
            Assert.That(!t.OffersBid);
            Assert.That(!t.OffersDown);
            Assert.That(!t.OffersUp);
            Assert.That(!t.Perfect);
            Assert.That(!t.PlusTick);
            Assert.That(!t.TakesOffer);
            Assert.That(!t.TakesOfferPerfect);


            // hitting the bid perfect, also should be minus tick
            t.newTick(timesales[i++]);
            Assert.That(t.Valid);
            Assert.That(!t.BidsUp);
            Assert.That(!t.BidsDown);
            Assert.That(!t.BidReload);
            Assert.That(t.HitsBid);
            Assert.That(t.HitsBidPerfect);
            Assert.That(t.MinusTick);
            Assert.That(!t.OfferReload);
            Assert.That(!t.OffersBid);
            Assert.That(!t.OffersDown);
            Assert.That(!t.OffersUp);
            Assert.That(t.Perfect);
            Assert.That(!t.PlusTick);
            Assert.That(!t.TakesOffer);
            Assert.That(!t.TakesOfferPerfect);

            // bids down or nothing with 2tick memory
            t.newTick(timesales[i++]);
            Assert.That(t.Valid);
            // ups size on bid (nothing)
            t.newTick(timesales[i++]);
            Assert.That(t.Valid);
            // lowers offer (or nothing with our 2tick memory)
            t.newTick(timesales[i++]);
            Assert.That(t.Valid);
            // lowers offer
            t.newTick(timesales[i++]);
            Assert.That(t.Valid);
            Assert.That(!t.BidsUp);
            Assert.That(!t.BidsDown);
            Assert.That(!t.BidReload);
            Assert.That(!t.HitsBid);
            Assert.That(!t.HitsBidPerfect);
            Assert.That(!t.MinusTick);
            Assert.That(!t.OfferReload);
            Assert.That(!t.OffersBid);
            Assert.That(t.OffersDown);
            Assert.That(!t.OffersUp);
            Assert.That(!t.Perfect);
            Assert.That(!t.PlusTick);
            Assert.That(!t.TakesOffer);
            Assert.That(!t.TakesOfferPerfect);

            // lowers offer
            t.newTick(timesales[i++]);
            Assert.That(t.Valid);
            Assert.That(!t.BidsUp);
            Assert.That(!t.BidsDown);
            Assert.That(!t.BidReload);
            Assert.That(!t.HitsBid);
            Assert.That(!t.HitsBidPerfect);
            Assert.That(!t.MinusTick);
            Assert.That(!t.OfferReload);
            Assert.That(!t.OffersBid);
            Assert.That(t.OffersDown);
            Assert.That(!t.OffersUp);
            Assert.That(!t.Perfect);
            Assert.That(!t.PlusTick);
            Assert.That(!t.TakesOffer);
            Assert.That(!t.TakesOfferPerfect);

            // hits off perfect, also should be minus tick
            t.newTick(timesales[i++]);
            Assert.That(t.Valid);
            Assert.That(!t.BidsUp);
            Assert.That(!t.BidsDown);
            Assert.That(!t.BidReload);
            Assert.That(!t.HitsBid);
            Assert.That(!t.HitsBidPerfect);
            Assert.That(t.MinusTick);
            Assert.That(!t.OfferReload);
            Assert.That(!t.OffersBid);
            Assert.That(!t.OffersDown);
            Assert.That(!t.OffersUp);
            Assert.That(t.Perfect);
            Assert.That(!t.PlusTick);
            Assert.That(t.TakesOffer);
            Assert.That(t.TakesOfferPerfect);

            // plustick
            t.newTick(timesales[i++]);
            Assert.That(t.Valid);
            Assert.That(!t.BidsUp);
            Assert.That(!t.BidsDown);
            Assert.That(!t.BidReload);
            Assert.That(!t.HitsBid);
            Assert.That(!t.HitsBidPerfect);
            Assert.That(!t.MinusTick);
            Assert.That(!t.OfferReload);
            Assert.That(!t.OffersBid);
            Assert.That(!t.OffersDown);
            Assert.That(!t.OffersUp);
            Assert.That(!t.Perfect);
            Assert.That(t.PlusTick);
            Assert.That(!t.TakesOffer);
            Assert.That(!t.TakesOfferPerfect);

            // nothing
            t.newTick(timesales[i++]);
            Assert.That(t.Valid);
            Assert.That(!t.BidsUp);
            Assert.That(!t.BidsDown);
            Assert.That(!t.BidReload);
            Assert.That(!t.HitsBid);
            Assert.That(!t.HitsBidPerfect);
            Assert.That(!t.MinusTick);
            Assert.That(!t.OfferReload);
            Assert.That(!t.OffersBid);
            Assert.That(!t.OffersDown);
            Assert.That(!t.OffersUp);
            Assert.That(!t.Perfect);
            Assert.That(!t.PlusTick);
            Assert.That(!t.TakesOffer);
            Assert.That(!t.TakesOfferPerfect);






        }

    }
}

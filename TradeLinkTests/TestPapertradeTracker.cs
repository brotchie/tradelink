using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLink.Common;
using TradeLink.API;
using TradeLink.Research;

namespace TestTradeLink
{
    [TestFixture]
    public class TestPapertradeTracker
    {

        PapertradeTracker ptt ;
        PositionTracker pt;
        [Test]
        public void TestTradeFill()
        {
            ptt = new PapertradeTracker();
            ptt.UseBidAskFills = false;
            pt = new PositionTracker();
            const string SYM = "TST";
            ptt.GotFillEvent += new FillDelegate(ptt_GotFillEvent);
            ptt.GotOrderEvent+=new OrderDelegate(ptt_GotOrderEvent);
            // send an order
            ptt.sendorder(new BuyMarket(SYM, 1000));
            // verify it did not fill
            Assert.AreEqual(0, pt[SYM].Size);
            // send a tick
            ptt.newTick(TickImpl.NewTrade(SYM, 10, 100));
            // verify it fills
            Assert.AreEqual(100, pt[SYM].Size);


        }

        [Test]
        public void TestBidAsk()
        {
            ptt = new PapertradeTracker();
            pt = new PositionTracker();
            const string SYM = "TST";
            ptt.GotFillEvent += new FillDelegate(ptt_GotFillEvent);
            ptt.GotOrderEvent += new OrderDelegate(ptt_GotOrderEvent);
            // enable bid ask fills
            ptt.UseBidAskFills = true;
            // send an order
            ptt.sendorder(new BuyMarket(SYM, 1000));
            // verify it did not fill
            Assert.AreEqual(0, pt[SYM].Size);
            // send a tick
            ptt.newTick(TickImpl.NewTrade(SYM, 10, 100));
            // verify it did not fill
            Assert.AreEqual(0, pt[SYM].Size);
            // send bid
            ptt.newTick(TickImpl.NewBid(SYM, 10, 100));
            // verify it did not fill
            Assert.AreEqual(0, pt[SYM].Size);
            // send ask
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            // verify it fills
            Assert.AreEqual(100, pt[SYM].Size);


        }

        Order last = new OrderImpl();
        void ptt_GotOrderEvent(Order o)
        {
            last = o;
        }

        void ptt_GotFillEvent(Trade t)
        {
            pt.Adjust(t);
        }
    }
}

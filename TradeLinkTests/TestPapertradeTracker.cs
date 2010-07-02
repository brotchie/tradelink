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
            ptt.SendDebugEvent+=new DebugDelegate(debug);
            ptt.UseBidAskFills = false;
            fills = 0;
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
            fills = 0;
            const string SYM = "TST";
            ptt.SendDebugEvent += new DebugDelegate(debug);
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
            Assert.AreEqual(1, fills);
            // fill rest of the order
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            Assert.AreEqual(10, fills);
            Assert.AreEqual(1000, pt[SYM].Size);
            // send a bunch more ticks and ensure nothing else is filled
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            ptt.newTick(TickImpl.NewAsk(SYM, 12, 100));
            Assert.AreEqual(0, ptt.QueuedOrders.Length);
            Assert.AreEqual(10, fills);
            Assert.AreEqual(1000, pt[SYM].Size);

        }

        Order last = new OrderImpl();
        void ptt_GotOrderEvent(Order o)
        {
            last = o;
        }

        int fills = 0;
        void ptt_GotFillEvent(Trade t)
        {
            fills++;
            pt.Adjust(t);
        }

        void debug(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}

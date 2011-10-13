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
    public class TestOverSell
    {
        OversellTracker ost = new OversellTracker();

        [Test]
        public void SplitWithPartialFillRoundMinsize100()
        {
            lasto = new OrderImpl();
            oc = 0;
            ost = new OversellTracker();
            ost.SendOrderEvent += new OrderDelegate(ost_SendOrderEvent);
            ost.SendDebugEvent += new DebugDelegate(rt.d);
            ost.MinLotSize = 100;

            ost.Split = true;
            // take a position
            ost.GotPosition(new PositionImpl("TST", 100, 58));
            // over sell
            Order o = new SellMarket("TST", 100);
            o.id = 1;
            ost.sendorder(o);
            // verify that flat and adjustment were sent
            Assert.AreEqual(1, oc);
            Assert.AreEqual(-100, lasto.size);
        }

        [Test]
        public void TestNormalEntry()
        {
            lasto = new OrderImpl();
            oc = 0;
            ost = new OversellTracker();
            ost.SendOrderEvent += new OrderDelegate(ost_SendOrderEvent);
            ost.SendDebugEvent += new DebugDelegate(rt.d);
            // take a position
            ost.GotPosition(new PositionImpl("TST"));
            // sell
            ost.sendorder(new SellMarket("TST", 500));
            // verify that was sent
            Assert.AreEqual(1, oc);
            Assert.AreEqual(-500, lasto.size);


        }

        [Test]
        public void TestNormalExit()
        {
            lasto = new OrderImpl();
            oc = 0;
            ost = new OversellTracker();
            ost.SendOrderEvent += new OrderDelegate(ost_SendOrderEvent);
            ost.SendDebugEvent += new DebugDelegate(rt.d);
            // take a position
            ost.GotPosition(new PositionImpl("TST",100,500));
            // sell
            ost.sendorder(new SellMarket("TST", 500));
            // verify that was sent
            Assert.AreEqual(1, oc);
            Assert.AreEqual(-500, lasto.size);


        }

        [Test]
        public void SplitCancelCopy()
        {
            lasto = new OrderImpl();
            oc = 0;
            cancels.Clear();
            ost = new OversellTracker();
            ost.SendOrderEvent += new OrderDelegate(ost_SendOrderEvent);
            ost.SendDebugEvent += new DebugDelegate(rt.d);
            ost.SendCancelEvent += new LongDelegate(ost_SendCancelEvent);

            ost.Split = true;
            // take a position
            ost.GotPosition(new PositionImpl("TST", 100, 300));
            // over sell
            Order o = new SellMarket("TST", 500);
            o.id = 5;
            ost.sendorder(o);
            // verify that only flat was sent
            Assert.AreEqual(2, oc);
            Assert.AreEqual(-200, lasto.size);
            // make sure we've not canceled
            Assert.AreEqual(0, cancels.Count);
            // cancel original order
            ost.sendcancel(5);
            // ensure two cancels sent
            Assert.AreEqual(2, cancels.Count);
            // ensure different cancels
            Assert.AreEqual(5, cancels[0]);
            Assert.AreNotEqual(5, cancels[1]);

            // do it again

            // take a position
            ost.GotPosition(new PositionImpl("TST", 100, 300));

            // over sell
            o = new SellMarket("TST", 500);
            o.id = 10;
            ost.sendorder(o);
            // verify that only flat was sent
            Assert.AreEqual(4, oc);
            Assert.AreEqual(-200, lasto.size);
            // make sure we've not canceled
            Assert.AreEqual(2, cancels.Count);
            // cancel original order
            ost.sendcancel(10);
            // ensure two cancels sent
            Assert.AreEqual(4, cancels.Count);
            // ensure different cancels
            Assert.AreEqual(10, cancels[2]);
            Assert.AreNotEqual(10, cancels[3]);


        }

        List<long> cancels = new List<long>();
        void ost_SendCancelEvent(long val)
        {
            cancels.Add(val);
        }

        [Test]
        public void TestOverSellDrop()
        {
            lasto = new OrderImpl();
            oc = 0;
            ost = new OversellTracker();
            ost.SendOrderEvent += new OrderDelegate(ost_SendOrderEvent);
            ost.SendDebugEvent += new DebugDelegate(rt.d);
            // take a position
            ost.GotPosition(new PositionImpl("TST", 100, 300));
            // over sell
            ost.sendorder(new SellMarket("TST", 500));
            // verify that only flat was sent
            Assert.AreEqual(1, oc);
            Assert.AreEqual(-300, lasto.size);

        }

        [Test]
        public void TestOverCoverDrop()
        {
            lasto = new OrderImpl();
            oc = 0;
            ost = new OversellTracker();
            ost.SendOrderEvent += new OrderDelegate(ost_SendOrderEvent);
            ost.SendDebugEvent+=new DebugDelegate(rt.d);

            // take a position
            ost.GotPosition(new PositionImpl("TST", 100, -300));
            // over sell
            ost.sendorder(new BuyMarket("TST", 500));
            // verify that only flat was sent
            Assert.AreEqual(1, oc);
            Assert.AreEqual(300, lasto.size);

        }

        [Test]
        public void TestOverSellAdjust()
        {
            lasto = new OrderImpl();
            oc = 0;
            ost = new OversellTracker();
            ost.SendOrderEvent += new OrderDelegate(ost_SendOrderEvent);
            ost.SendDebugEvent += new DebugDelegate(rt.d);

            ost.Split = true;
            // take a position
            ost.GotPosition(new PositionImpl("TST", 100, 300));
            // over sell
            ost.sendorder(new SellMarket("TST", 500));
            // verify that flat and adjustment were sent
            Assert.AreEqual(2, oc);
            Assert.AreEqual(-200, lasto.size);

        }

        [Test]
        public void TestOverBuyAdjust()
        {
            lasto = new OrderImpl();
            oc = 0;
            ost = new OversellTracker();
            ost.SendOrderEvent += new OrderDelegate(ost_SendOrderEvent);
            ost.SendDebugEvent += new DebugDelegate(rt.d);

            ost.Split = true;
            // take a position
            ost.GotPosition(new PositionImpl("TST", 100, -300));
            // over sell
            ost.sendorder(new BuyMarket("TST", 500));
            // verify that flat and adjustment were sent
            Assert.AreEqual(2, oc);
            Assert.AreEqual(200, lasto.size);

        }

        Order lasto = new OrderImpl();
        int oc = 0;
        void ost_SendOrderEvent(Order o)
        {
            oc++;
            lasto = o;
        }
    }
}

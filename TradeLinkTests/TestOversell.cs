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
            // verify that only flat was sent
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
            // verify that only flat was sent
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

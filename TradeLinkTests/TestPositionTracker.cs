using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLink.API;
using TradeLink.Common;

namespace TestTradeLink
{
    [TestFixture]
    public class TestPositionTracker
    {
        public TestPositionTracker() { }

        [Test]
        public void Adjust()
        {
            const string s = "IBM";
            TradeImpl t1 = new TradeImpl(s, 100, 100);

            PositionTracker pt = new PositionTracker();

            // make we have no position yet
            Assert.IsTrue(pt[t1.symbol].isFlat);

            // send some adjustments
            decimal cpl = 0;
            cpl += pt.Adjust(t1);
            cpl += pt.Adjust(t1);

            // verify that adjustments took hold
            Assert.AreEqual(0, cpl);
            Assert.AreEqual(200, pt[t1.symbol].Size);
        }

        [Test]
        public void NewPosition()
        {
            const string s = "IBM";
            Position p = new PositionImpl(s,80,500);

            PositionTracker pt = new PositionTracker();
            Assert.IsTrue(pt[s].isFlat);
            pt.NewPosition(p);
            Assert.AreEqual(500, pt[s].Size);



        }
    }
}

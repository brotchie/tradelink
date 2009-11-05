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

        [Test]
        public void InitAndAdjust()
        {
            const string sym = "IBM";
            // startup position tracker
            PositionTracker pt = new PositionTracker();
            PositionTracker pt2 = new PositionTracker();
            // give pt our initial position
            Position init = new PositionImpl(sym, 0, 0);
            pt.Adjust(init);
            pt2.Adjust(init);
            // fill a trade in both places
            Trade fill = new TradeImpl(sym,100, 100);
            pt.Adjust(fill);
            pt2.Adjust(fill);
            // make sure it's only 100 in both places
            Assert.AreEqual(100, pt[sym].Size);
            Assert.AreEqual(100, pt2[sym].Size);
        }

        [Test]
        public void BlankPositionReq()
        {
            PositionTracker pt = new PositionTracker();
            bool except = false;
            int s = 100;
            try
            {
                s = pt["IBM"].Size;
            }
            catch { except = true; }
            Assert.AreEqual(0, s);
            Assert.IsFalse(except);
        }
    }
}

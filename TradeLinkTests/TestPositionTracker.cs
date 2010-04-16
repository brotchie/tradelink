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

        [Test]
        public void MultipleAccount()
        {
            // setup defaults for 1st and 2nd accounts and positions
            string sym = "TST";
            string a1 = "account1";
            string a2 = "account2";
            int s1 = 300;
            int s2 = 500;
            decimal p = 100m;

            // create position tracker
            PositionTracker pt = new PositionTracker();

            // set initial position in 1st account
            pt.Adjust(new PositionImpl(sym, p, s1, 0, a1));

            // set initial position in 2nd account
            pt.Adjust(new PositionImpl(sym, p, s2, 0, a2));

            // verify I can query default account and it's correct
            Assert.AreEqual(s1, pt[sym].Size);
            // change default to 2nd account
            pt.DefaultAccount = a2;
            // verify I can query default and it's correct
            Assert.AreEqual(s2, pt[sym].Size);
            // verify I can query 1st account and correct
            Assert.AreEqual(s1, pt[sym,a1].Size);
            // verify I can query 2nd account and correct
            Assert.AreEqual(s2, pt[sym,a2].Size);
            // get fill in sym for 1st account
            TradeImpl f = new TradeImpl(sym, p, s1);
            f.Account = a1;
            pt.Adjust(f);
            // get fill in sym for 2nd account
            TradeImpl f2 = new TradeImpl(sym, p, s2);
            f2.Account = a2;
            pt.Adjust(f2);
            // verify that I can querry 1st account and correct
            Assert.AreEqual(s1*2, pt[sym, a1].Size);
            // verify I can query 2nd account and correct
            Assert.AreEqual(s2*2, pt[sym, a2].Size);
            // reset
            pt.Clear();
            // ensure I can query first and second account and get flat symbols
            Assert.AreEqual(0, pt[sym].Size);
            Assert.AreEqual(0, pt[sym, a1].Size);
            Assert.AreEqual(0, pt[sym, a2].Size);
            Assert.IsTrue(pt[sym, a1].isFlat);
            Assert.IsTrue(pt[sym, a2].isFlat);
            Assert.IsTrue(pt[sym].isFlat);
            Assert.AreEqual(string.Empty, pt.DefaultAccount);
        }
    }
}

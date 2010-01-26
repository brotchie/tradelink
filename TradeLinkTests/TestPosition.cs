using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using NUnit.Framework;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TestPosition
    {
        public TestPosition() { }
        const string s = "TST";
        DateTime dt = DateTime.Now;
        [Test]
        public void Basics()
        {
            PositionImpl p = new PositionImpl(s);
            Assert.AreEqual(0,p.Size);
            Assert.That(p.Symbol!="","hassymbol");
            Assert.AreEqual(0,p.AvgPrice);
            Assert.That(p.isFlat,"isflat");
            Assert.That(p.isValid,"isvalid");
            PositionImpl p2 = new PositionImpl(s, 10, 100,0);
            PositionImpl p2copy = new PositionImpl(p2);
            Assert.AreEqual(p2.AvgPrice, p2copy.AvgPrice);
            Assert.AreEqual(p2.Size, p2copy.Size);
            Assert.AreEqual(p2.ClosedPL, p2copy.ClosedPL);
            Assert.AreEqual(p2.Symbol, p2copy.Symbol);
            p.Adjust(p2);
            Assert.That(p.Size == 100);
            Assert.IsTrue(p.Symbol!="", "hassymbol");
            Assert.That(p.AvgPrice == 10);
            Assert.IsFalse(p.isFlat);
            Assert.IsTrue(p.isLong);
            Assert.IsTrue(p.isValid);
            bool invalidexcept = false;
            PositionImpl p3 = null;
            try
            {
                p3 = new PositionImpl(s, 0, 100, 0);
            }
            catch
            {
                invalidexcept = true;
            }
            Assert.That(invalidexcept);
            p3 = new PositionImpl(s, 12, 100,0);
            p.Adjust(p3);
            Assert.AreEqual(11,p.AvgPrice);
            Assert.That(p.isLong);
            Assert.That(p.isValid);
            Assert.That(!p.isFlat);
            Assert.That(p.Size == 200);
            p.Adjust(new TradeImpl(s, 13, -100,dt));
            Assert.That(p.AvgPrice == 11);
            Assert.That(p.isLong);
            Assert.That(p.isValid);
            Assert.That(!p.isFlat);
            Assert.That(p.Size == 100);
            TradeImpl lasttrade = new TradeImpl(s, 12, -100,dt);
            decimal profitFromP2toLASTTRADE = Calc.ClosePL(p2, lasttrade);
            Assert.That(profitFromP2toLASTTRADE == (lasttrade.xprice-p2.AvgPrice)*Math.Abs(lasttrade.xsize));
        }

        [Test]
        public void PositionAccountTest()
        {
            TradeImpl t = new TradeImpl("TST", 100, 100);
            t.Account = "ME";
            TradeImpl t2 = new TradeImpl("TST", 200, 200);
            Assert.That(t.isValid);
            Assert.That(t2.isValid);
            t2.Account = "HIM";
            PositionImpl p = new PositionImpl(t);
            p.Adjust(t);
            bool failed = false;            try
            {
                p.Adjust(t2);
            }
            catch (Exception) { failed = true; }
            Assert.IsTrue(failed);

        }

        [Test]
        public void FlipSideInOneTrade()
        {
            // this is illegal on the exchanges, but supported by certain
            // retail brokers so we're going to allow tradelink to support it
            // BE CAREFUL WITH THIS FEATURE.  make sure you won't be fined for doing this, before you do it.
            string s = "IBM";
            // long position
            PositionImpl p = new PositionImpl(s, 100m,200);
            // sell more than we've got to change sides
            TradeImpl flip = new TradeImpl(s, 99, -400);
            decimal cpl = p.Adjust(flip);
            // make sure we captured close of trade
            Assert.AreEqual(-200, cpl); 
            // make sure we captured new side and price
            Assert.AreEqual(-200, p.Size);
            Assert.AreEqual(99, p.AvgPrice);

        }

        [Test]
        public void UsingTrades()
        {
            // long
            PositionImpl p = new PositionImpl(new TradeImpl(s, 80, 100,dt));
            Assert.That(p.isLong);
            Assert.That(p.Size == 100);
            decimal pl = p.Adjust(new TradeImpl(s, 84, -100,dt));
            Assert.That(p.isFlat);
            Assert.AreEqual((84 - 80) * 100,pl);
            // short
            pl = 0;
            p = new PositionImpl(new TradeImpl(s, 84, -100,dt));
            Assert.That(!p.isLong);
            Assert.That(p.Size == -100);
            pl = p.Adjust(new TradeImpl(s, 80, 100,dt));
            Assert.That(pl == (84 - 80) * 100);
            Assert.That(p.isFlat);
        }

        [Test]
        public void SerializeDeserialize()
        {
            const string s = "TST";
            const decimal x = 10m;
            const int z = -100;
            const decimal cpl = 5.05m;
            PositionImpl p = new PositionImpl(s, x, z, cpl);
            string msg = PositionImpl.Serialize(p);

            Position c = PositionImpl.Deserialize(msg);
            Assert.That(c.Symbol == s, c.Symbol);
            Assert.That(c.AvgPrice == x, c.AvgPrice.ToString());
            Assert.That(c.Size == z, c.Size.ToString());
            Assert.That(c.ClosedPL == cpl, c.ClosedPL.ToString());
        }

        [Test]
        public void CreateInvalidFromSymbol()
        {
            const string sym = "TST";

            bool except = false;
            Position p = null;
            try
            {
                p = new PositionImpl(sym);
            }
            catch { except = true; }

            Assert.IsNotNull(p);
            Assert.IsTrue(p.isValid);
            Assert.IsFalse(except);
                
        }

        [Test]
        public void ClosedPL()
        {
            const string sym = "RYN";
            PositionTracker pt = new PositionTracker();
            Position p = new PositionImpl(sym, 44.39m, 800, 0);
            pt.Adjust(p);
            System.IO.StreamReader sr = new System.IO.StreamReader("TestPositionClosedPL.txt");
            string[] file = sr.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in file)
            {
                Trade t = TradeImpl.FromString(line);
                pt.Adjust(t);
            }
            Assert.AreEqual(-66, pt[sym].ClosedPL);


        }
    }
}

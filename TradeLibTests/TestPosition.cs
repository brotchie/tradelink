using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
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
            Position p = new Position(s);
            Assert.AreEqual(0,p.Size);
            Assert.That(p.hasSymbol,"hassymbol");
            Assert.AreEqual(0,p.AvgPrice);
            Assert.That(p.isFlat,"isflat");
            Assert.That(p.isValid,"isvalid");
            Position p2 = new Position(s, 10, 100,0);
            Position p2copy = new Position(p2);
            Assert.AreEqual(p2.AvgPrice, p2copy.AvgPrice);
            Assert.AreEqual(p2.Size, p2copy.Size);
            Assert.AreEqual(p2.ClosedPL, p2copy.ClosedPL);
            Assert.AreEqual(p2.Symbol, p2copy.Symbol);
            p.Adjust(p2);
            Assert.That(p.Size == 100);
            Assert.IsTrue(p.hasSymbol, "hassymbol");
            Assert.That(p.AvgPrice == 10);
            Assert.IsFalse(p.isFlat);
            Assert.IsTrue(p.isLong);
            Assert.IsTrue(p.isValid);
            Position p3 = new Position(s, 0, 100,0);
            Assert.That(!p3.isValid);
            p3 = new Position(s, 12, 100,0);
            p.Adjust(p3);
            Assert.That(p.AvgPrice == 11);
            Assert.That(p.isLong);
            Assert.That(p.isValid);
            Assert.That(!p.isFlat);
            Assert.That(p.Size == 200);
            p.Adjust(new Trade(s, 13, -100,dt));
            Assert.That(p.AvgPrice == 11);
            Assert.That(p.isLong);
            Assert.That(p.isValid);
            Assert.That(!p.isFlat);
            Assert.That(p.Size == 100);
            Trade lasttrade = new Trade(s, 12, -100,dt);
            decimal profitFromP2toLASTTRADE = Calc.ClosePL(p2, lasttrade);
            Assert.That(profitFromP2toLASTTRADE == (lasttrade.xprice-p2.AvgPrice)*Math.Abs(lasttrade.xsize));
        }

        [Test]
        public void FlipSideInOneTrade()
        {
            // this is illegal on the exchanges, but supported by certain
            // retail brokers so we're going to allow tradelink to support it
            // BE CAREFUL WITH THIS FEATURE.  make sure you won't be fined for doing this, before you do it.
            string s = "IBM";
            // long position
            Position p = new Position(s, 100m,200);
            // sell more than we've got to change sides
            Trade flip = new Trade(s, 99, -400);
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
            Position p = new Position(new Trade(s, 80, 100,dt));
            Assert.That(p.isLong);
            Assert.That(p.Size == 100);
            decimal pl = p.Adjust(new Trade(s, 84, -100,dt));
            Assert.That(p.isFlat);
            Assert.AreEqual((84 - 80) * 100,pl);
            // short
            pl = 0;
            p = new Position(new Trade(s, 84, -100,dt));
            Assert.That(!p.isLong);
            Assert.That(p.Size == -100);
            pl = p.Adjust(new Trade(s, 80, 100,dt));
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
            Position p = new Position(s, x, z, cpl);
            string msg = p.Serialize();

            Position c = Position.Deserialize(msg);
            Assert.That(c.Symbol == s, c.Symbol);
            Assert.That(c.AvgPrice == x, c.AvgPrice.ToString());
            Assert.That(c.Size == z, c.Size.ToString());
            Assert.That(c.ClosedPL == cpl, c.ClosedPL.ToString());
        }
    }
}

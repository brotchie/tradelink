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
            Assert.That(p.Size == 0);
            Assert.That(p.hasSymbol);
            Assert.That(p.AvgPrice == 0);
            Assert.That(p.Flat);
            Assert.That(p.isValid);
            Position p2 = new Position(s, 10, 100);
            p.Adjust(p2);
            Assert.That(p.Size == 100);
            Assert.That(p.hasSymbol);
            Assert.That(p.AvgPrice == 10);
            Assert.That(!p.Flat);
            Assert.That(p.Side);
            Assert.That(p.isValid);
            Position p3 = new Position(s, 0, 100);
            Assert.That(!p3.isValid);
            p3 = new Position(s, 10, 0);
            Assert.That(!p3.isValid);
            p3 = new Position(s, 12, 100);
            p.Adjust(p3);
            Assert.That(p.AvgPrice == 11);
            Assert.That(p.Side);
            Assert.That(p.isValid);
            Assert.That(!p.Flat);
            Assert.That(p.Size == 200);
            p.Adjust(new Trade(s, 13, -100,dt));
            Assert.That(p.AvgPrice == 11);
            Assert.That(p.Side);
            Assert.That(p.isValid);
            Assert.That(!p.Flat);
            Assert.That(p.Size == 100);
            Trade lasttrade = new Trade(s, 12, -100,dt);
            decimal profitFromP2toLASTTRADE = BoxMath.ClosePL(p2, lasttrade);
            Assert.That(profitFromP2toLASTTRADE == (lasttrade.xprice-p2.AvgPrice)*Math.Abs(lasttrade.xsize));
        }

        [Test]
        public void UsingTrades()
        {
            // long
            Position p = new Position(new Trade(s, 80, 100,dt));
            Assert.That(p.Side);
            Assert.That(p.Size == 100);
            decimal pl = p.Adjust(new Trade(s, 84, -100,dt));
            Assert.That(p.Flat);
            Assert.That(pl == (84 - 80) * 100);
            // short
            pl = 0;
            p = new Position(new Trade(s, 84, -100,dt));
            Assert.That(!p.Side);
            Assert.That(p.Size == -100);
            pl = p.Adjust(new Trade(s, 80, 100,dt));
            Assert.That(pl == (84 - 80) * 100);
            Assert.That(p.Flat);
        }
    }
}

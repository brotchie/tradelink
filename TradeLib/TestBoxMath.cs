using System;
using NUnit.Framework;
using TradeLib;

namespace TestTradeLib
{
    [TestFixture]
    public class TestBoxMath
    {
        bool Long = true;
        bool Short = false;
        string stock = "IBM";
        decimal last = 100.45m;
        decimal entry = 99.47m;
        int lsize = 200;
        int ssize = -500;
        Position lp, sp;
        [SetUp]
        protected void Setup()
        {
            lp = new Position(stock, entry, lsize);
            sp = new Position(stock, entry, ssize);
        }
        [Test]
        public void PL()
        {
            decimal pl = .98m;
            Assert.That(BoxMath.SharePL(last, entry, Long) == pl);
            Assert.That(BoxMath.SharePL(last, entry, Short) == -pl);
            Assert.That(BoxMath.SharePL(last, entry, lsize) == pl);
            Assert.That(BoxMath.SharePL(last, entry, ssize) == -pl);
            Assert.That(BoxMath.SharePL(last, lp) == pl);
            Assert.That(BoxMath.SharePL(last, sp) == -pl);
            Assert.That(BoxMath.PositionPL(last, lp) == lp.Size* pl);
            Assert.That(BoxMath.PositionPL(last, sp) == sp.Size*pl);
        }
    }
}

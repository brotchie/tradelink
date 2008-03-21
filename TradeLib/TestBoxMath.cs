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
        Position lp, sp,lc,sc;
        [SetUp]
        protected void Setup()
        {
            lp = new Position(stock, entry, lsize);
            sp = new Position(stock, entry, ssize);

            //closing positions
            lc = new Position(stock, last, lsize / -2);
            sc = new Position(stock, last, -ssize);
        }
        [Test]
        public void OpenPL()
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
        [Test]
        public void ClosePL()
        {
            decimal pl = .98m;
            Assert.That(BoxMath.CloseSharePL(lp, lc) == pl);
            Assert.That(BoxMath.CloseSharePL(sp, sc) == -pl);
            Assert.That(BoxMath.ClosePositionPL(lp, lc) == pl*Math.Abs(lsize/2)); // matches closing size
            Assert.That(BoxMath.ClosePositionPL(sp, sc) == pl*ssize);
        }
    }
}

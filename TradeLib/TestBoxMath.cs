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
        Trade lc, sc;
        [SetUp]
        protected void Setup()
        {
            lp = new Position(stock, entry, lsize);
            sp = new Position(stock, entry, ssize);

            //closing trades
            lc = new Trade(stock, last, lsize / -2);
            sc = new Trade(stock, last, -ssize);
        }
        [Test]
        public void OpenPL()
        {
            decimal pl = .98m;
            Assert.That(BoxMath.OpenPT(last, entry, Long) == pl);
            Assert.That(BoxMath.OpenPT(last, entry, Short) == -pl);
            Assert.That(BoxMath.OpenPT(last, entry, lsize) == pl);
            Assert.That(BoxMath.OpenPT(last, entry, ssize) == -pl);
            Assert.That(BoxMath.OpenPT(last, lp) == pl);
            Assert.That(BoxMath.OpenPT(last, sp) == -pl);
            Assert.That(BoxMath.OpenPL(last, lp) == lp.Size* pl);
            Assert.That(BoxMath.OpenPL(last, sp) == sp.Size*pl);
        }
        [Test]
        public void ClosePL()
        {
            decimal pl = .98m;
            Assert.That(BoxMath.ClosePT(lp, lc) == pl);
            Assert.That(BoxMath.ClosePT(sp, sc) == -pl);
            Assert.That(BoxMath.ClosePL(lp, lc) == pl*(lsize/2)); // matches closing size
            Assert.That(BoxMath.ClosePL(sp, sc) == pl*ssize);
        }
    }
}

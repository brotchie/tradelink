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
            Assert.AreEqual(pl,BoxMath.OpenPT(last, entry, Long));
            Assert.AreEqual(-pl,BoxMath.OpenPT(last, entry, Short));
            Assert.AreEqual(pl, BoxMath.OpenPT(last, entry, lsize));
            Assert.AreEqual(-pl,BoxMath.OpenPT(last, entry, ssize));
            Assert.AreEqual(pl, BoxMath.OpenPT(last, lp));
            Assert.AreEqual(-pl,BoxMath.OpenPT(last, sp));
            Assert.AreEqual(lp.Size * pl,BoxMath.OpenPL(last, lp) );
            Assert.AreEqual(sp.Size * pl,BoxMath.OpenPL(last, sp) );
        }
        [Test]
        public void ClosePL()
        {
            decimal pl = .98m;
            Assert.AreEqual(pl,BoxMath.ClosePT(lp, lc));
            Assert.AreEqual(-pl,BoxMath.ClosePT(sp, sc));
            Assert.AreEqual(pl*(lsize/2),BoxMath.ClosePL(lp, lc)); // matches closing size
            Assert.AreEqual((entry-last)*-ssize, BoxMath.ClosePL(sp, sc));
        }
    }
}

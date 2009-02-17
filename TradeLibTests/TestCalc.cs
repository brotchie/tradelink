using System;
using NUnit.Framework;
using TradeLink.Common;

namespace TestTradeLink
{
    [TestFixture]
    public class TestCalc
    {
        bool Long = true;
        bool Short = false;
        string stock = "IBM";
        decimal last = 100.45m;
        decimal entry = 99.47m;
        int lsize = 200;
        int ssize = -500;
        PositionImpl lp, sp;
        TradeImpl lc, sc;
        [SetUp]
        protected void Setup()
        {
            lp = new PositionImpl(stock, entry, lsize);
            sp = new PositionImpl(stock, entry, ssize);

            //closing trades
            lc = new TradeImpl(stock, last, lsize / -2);
            sc = new TradeImpl(stock, last, -ssize);
        }
        [Test]
        public void OpenPL()
        {
            decimal pl = .98m;
            Assert.AreEqual(pl,Calc.OpenPT(last, entry, Long));
            Assert.AreEqual(-pl,Calc.OpenPT(last, entry, Short));
            Assert.AreEqual(pl, Calc.OpenPT(last, entry, lsize));
            Assert.AreEqual(-pl,Calc.OpenPT(last, entry, ssize));
            Assert.AreEqual(pl, Calc.OpenPT(last, lp));
            Assert.AreEqual(-pl,Calc.OpenPT(last, sp));
            Assert.AreEqual(lp.Size * pl,Calc.OpenPL(last, lp) );
            Assert.AreEqual(sp.Size * pl,Calc.OpenPL(last, sp) );
        }
        [Test]
        public void ClosePL()
        {
            decimal pl = .98m;
            Assert.AreEqual(pl,Calc.ClosePT(lp, lc));
            Assert.AreEqual(-pl,Calc.ClosePT(sp, sc));
            Assert.AreEqual(pl*(lsize/2),Calc.ClosePL(lp, lc)); // matches closing size
            Assert.AreEqual((entry-last)*-ssize, Calc.ClosePL(sp, sc));
        }
    }
}

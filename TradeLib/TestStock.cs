using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
{
    [TestFixture]
    public class TestStock
    {

        public TestStock() { }

        [Test]
        public void Basics()
        {
            Stock s = new Stock("");
            Assert.That(s != null);
            Assert.That(!s.isValid);
            s = new Stock("TST");
            Assert.That(s.isValid);
        }

        [Test]
        public void ValidSymbols()
        {
            Assert.That(v("X"));
            Assert.That(i("/SPX"));
            Assert.That(i("$TICK"));
            Assert.That(i(""));
            Assert.That(i("-"));
            Assert.That(i("_LVS"));
            Assert.That(v("LVS"));
            Assert.That(v("lvs"));
            Assert.That(v("A"));
            Assert.That(i("!A"));
            Assert.That(v("WM"));
            Assert.That(v("GOOG"));
        }

        public bool v(string s) { return Stock.isStock(s); }
        public bool i(string s) { return !Stock.isStock(s); }
    }
}

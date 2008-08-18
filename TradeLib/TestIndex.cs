using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
{
    [TestFixture]
    public class TestIndex
    {
        public TestIndex() { }

        [Test]
        public void Basics()
        {
            Index i = new Index("");
            Assert.That(i != null);
            Assert.That(!i.isValid);
            i = new Index("/SPX");
            Assert.That(i.isValid);

        }

        [Test]
        public void IndexNames()
        {
            Assert.That(v("/SPX"));
            Assert.That(v("$TICK"));
            Assert.That(i("/"));
            Assert.That(i(" "));
            Assert.That(i("/ "));
            Assert.That(i(""));
            Assert.That(v("/QQQQ"));
        }

        bool v(string s) { return Index.isIdx(s); } // should be valid
        bool i(string s) { return !Index.isIdx(s); } // should be invalid

    }
}

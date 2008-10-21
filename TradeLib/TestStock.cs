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
            Security s = new Security("");
            Assert.That(s != null);
            Assert.That(!s.isValid);
            s = new Security("TST");
            Assert.That(s.isValid);
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using NUnit.Framework;

namespace TestTradeLink
{
    [TestFixture]
    public class TestStock
    {

        public TestStock() { }

        [Test]
        public void Basics()
        {
            SecurityImpl s = new SecurityImpl("");
            Assert.That(s != null);
            Assert.That(!s.isValid);
            s = new SecurityImpl("TST");
            Assert.That(s.isValid);
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using NUnit.Framework;

namespace TestTradeLink
{
    [TestFixture]
    public class TestWMUtil
    {
        public TestWMUtil() { }

        [Test]
        public void Packing()
        {
            decimal normal = 37.56m;
            long packed = WMUtil.pack(normal);
            decimal unpacked = WMUtil.unpack(packed);
            Assert.That(normal == unpacked, normal.ToString()+" -> "+unpacked.ToString());
        }
    }
}

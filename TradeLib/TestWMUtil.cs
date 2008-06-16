using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
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

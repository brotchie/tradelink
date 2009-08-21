using System;
using System.Collections.Generic;
using TradeLink.API;
using TradeLink.Common;
using TradeLink.Research;
using NUnit.Framework;

namespace TestTradeLink
{
    [TestFixture]
    public class TestRandomTicks
    {
        public TestRandomTicks()
        {
        }

        [Test]
        public void Basics()
        {
            Tick[] ticks = RandomTicks.GenerateSymbol("TST", 1000);
            bool v = true;
            foreach (Tick k in ticks)
                v &= k.isValid;
            Assert.IsTrue(v);
        }
    }
}

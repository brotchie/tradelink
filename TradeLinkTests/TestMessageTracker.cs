using System;
using System.Collections.Generic;
using TradeLink.API;
using TradeLink.Common;
using NUnit.Framework;

namespace TestTradeLink
{
    [TestFixture]
    public class TestMessageTracker
    {
        MessageTracker _mt = new MessageTracker();
        BarListTracker _blt = new BarListTracker(); // default intervals

        public TestMessageTracker()
        {
            _mt.BLT = _blt;
        }

        [Test]
        public void TestDailyBars()
        {
            

        }

        [Test]
        public void TestIntradayBars()
        {



            
        }
    }
}

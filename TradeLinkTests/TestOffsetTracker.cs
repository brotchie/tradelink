using System;
using System.Collections.Generic;
using TradeLink.API;
using System.Text;
using NUnit.Framework;
using TradeLink.Common;

namespace TestTradeLink
{
    [TestFixture]
    public class TestOffsetTracker
    {
        public TestOffsetTracker() { }

        [Test]
        public void Basics()
        {
            OffsetTracker ot = new OffsetTracker();
            ot.DefaultProfit = .25m;
            ot.DefaultStop = .15m;


        }
    }
}

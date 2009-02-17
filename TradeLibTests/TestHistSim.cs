using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLink.Common;

namespace TestTradeLink
{
    [TestFixture]
    public class TestHistSim
    {
        public TestHistSim() { }

        TickImpl[] timesales = new TickImpl[] 
            {
                TickImpl.NewTrade("IBM",100,100),
                TickImpl.NewTrade("LVS",90,200),
                TickImpl.NewQuote("LVS",20080425,931,0,90.00m,90.11m,10,2,"NYS","NYS"),
                TickImpl.NewTrade("LVS",90.11m,100),
                TickImpl.NewTrade("IBM",100,200),
            };

        [Test]
        public void Basics()
        {
            //HistSim h = new HistSim(new TickFileFilter(new string[] { "IBM" }));
        }
    }
}

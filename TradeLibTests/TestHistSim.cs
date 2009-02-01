using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLib;

namespace TestTradeLib
{
    [TestFixture]
    public class TestHistSim
    {
        public TestHistSim() { }

        Tick[] timesales = new Tick[] 
            {
                Tick.NewTrade("IBM",100,100),
                Tick.NewTrade("LVS",90,200),
                Tick.NewQuote("LVS",20080425,931,0,90.00m,90.11m,10,2,"NYS","NYS"),
                Tick.NewTrade("LVS",90.11m,100),
                Tick.NewTrade("IBM",100,200),
            };

        [Test]
        public void Basics()
        {
            //HistSim h = new HistSim(new TickFileFilter(new string[] { "IBM" }));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLib;

namespace TestTradeLib
{
    [TestFixture]
    public class TestBar
    {

        public TestBar() { }

        const string sym = "TST";
        const int d = 20070517;
        const int t = 935;
        const string x = "NYSE";
        Tick[] ticklist = new Tick[] { 
                Tick.NewTrade(sym,d,t,0,10,100,x),
                Tick.NewTrade(sym,d,t+1,0,10,100,x),
                Tick.NewTrade(sym,d,t+2,0,10,100,x),
                Tick.NewTrade(sym,d,t+3,0,10,100,x),
                Tick.NewTrade(sym,d,t+4,0,15,100,x), 
                Tick.NewTrade(sym,d,t+5,0,16,100,x), 
                Tick.NewTrade(sym,d,t+6,0,16,100,x),
                Tick.NewTrade(sym,d,t+7,0,10,100,x), 
                Tick.NewTrade(sym,d,t+7,10,10,100,x), 
            };

        [Test]
        public void Construction()
        {
            Bar b = new Bar();
            Assert.That(!b.isValid);
            Assert.That(!b.isNew);
            b.newTick(ticklist[0]);
            Assert.That(b.isValid);
            Assert.That(b.isNew);
            b.newTick(ticklist[1]);
            Assert.That(b.isValid);
            Assert.That(!b.isNew);
            Assert.That(b.Volume == 200);
            b.newTick(Tick.NewQuote(sym,d,t,0,10m,11m,1,1,x,x));
            Assert.That(b.TradeCount == 2);


        }

        [Test]
        public void BarIntervals()
        {


            Bar b = new Bar(BarInterval.FiveMin);
            int accepts = 0;
            foreach (Tick k in ticklist)
                if (b.newTick(k)) accepts++;
            Assert.That(accepts == 5);

            b = new Bar(BarInterval.FifteenMin);
            accepts = 0;
            foreach (Tick k in ticklist)
                if (b.newTick(k)) accepts++;
            Assert.That(accepts == 9);

            b = new Bar(BarInterval.Minute);
            accepts = 0;
            for (int i = 7; i<ticklist.Length; i++)
                if (b.newTick(ticklist[i])) accepts++;
            Assert.That(accepts == 2);

        }
    }
}

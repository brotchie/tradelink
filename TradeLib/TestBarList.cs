using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
{
    [TestFixture]
    public class TestBarList
    {

        const string sym = "TST";
        const int d = 20070517;
        const int t = 935;
        const string x = "NYSE";
        Tick[] ticklist = new Tick[] { 
                Tick.NewTrade(sym,d,t,0,10,100,x), // new on all intervals
                Tick.NewTrade(sym,d,t+1,0,10,100,x), //new on 1min
                Tick.NewTrade(sym,d,t+2,0,10,100,x),
                Tick.NewTrade(sym,d,t+3,0,10,100,x),
                Tick.NewTrade(sym,d,t+4,0,15,100,x), 
                Tick.NewTrade(sym,d,t+5,0,16,100,x), //new on 5min
                Tick.NewTrade(sym,d,t+6,0,16,100,x),
                Tick.NewTrade(sym,d,t+7,0,10,100,x), 
                Tick.NewTrade(sym,d,t+7,10,10,100,x), 
            };

        [Test]
        public void NewBars()
        {
            BarList bl = new BarList(BarInterval.FiveMin);
            int newbars = 0;
            foreach (Tick k in ticklist)
            {
                bl.newTick(k);
                if (bl.NewBar)
                    newbars++;
            }

            Assert.That(newbars == 2, newbars.ToString());


            bl = new BarList(BarInterval.Minute);
            newbars = 0;
            foreach (Tick k in ticklist)
            {
                bl.newTick(k);
                if (bl.NewBar)
                    newbars++;
            }

            Assert.That(newbars == 8, newbars.ToString());

        }
    }
}

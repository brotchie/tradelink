using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using NUnit.Framework;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TestBarList
    {

        const string sym = "TST";
        const int d = 20070517;
        const int t = 93500;
        const string x = "NYSE";
        TickImpl[] ticklist = new TickImpl[] { 
                TickImpl.NewTrade(sym,d,t,10,100,x), // new on all intervals
                TickImpl.NewTrade(sym,d,t+100,10,100,x), //new on 1min
                TickImpl.NewTrade(sym,d,t+200,10,100,x),
                TickImpl.NewTrade(sym,d,t+300,10,100,x),
                TickImpl.NewTrade(sym,d,t+400,15,100,x), 
                TickImpl.NewTrade(sym,d,t+500,16,100,x), //new on 5min
                TickImpl.NewTrade(sym,d,t+600,16,100,x),
                TickImpl.NewTrade(sym,d,t+700,10,100,x), 
                TickImpl.NewTrade(sym,d,t+710,10,100,x), 
            };

        [Test]
        public void NewBars()
        {
            BarListImpl bl = new BarListImpl(BarInterval.FiveMin);
            int newbars = 0;
            foreach (TickImpl k in ticklist)
            {
                bl.newTick(k);
                if (bl.NewBar)
                    newbars++;
            }

            Assert.AreEqual(2,newbars);


            bl = new BarListImpl(BarInterval.Minute);
            newbars = 0;
            foreach (TickImpl k in ticklist)
            {
                bl.newTick(k);
                if (bl.NewBar)
                    newbars++;
            }

            Assert.AreEqual(8, newbars);

        }
        [Test]
        public void HourTest()
        {
            int t = 191500;
            TickImpl[] tape = new TickImpl[] { 
                TickImpl.NewTrade(sym,d,t,10,100,x), // new on all intervals
                TickImpl.NewTrade(sym,d,t+100,10,100,x), 
                TickImpl.NewTrade(sym,d,t+200,10,100,x),
                TickImpl.NewTrade(sym,d,t+300,10,100,x),
                TickImpl.NewTrade(sym,d,t+400,15,100,x), 
                TickImpl.NewTrade(sym,d,t+500,16,100,x), 
                TickImpl.NewTrade(sym,d,t+600,16,100,x),
                TickImpl.NewTrade(sym,d,t+700,10,100,x), 
                TickImpl.NewTrade(sym,d,t+710,10,100,x), 
                TickImpl.NewTrade(sym,d,t+10000,10,100,x), // new on hour interval
            };

            int newbars = 0;
            BarListImpl bl = new BarListImpl(BarInterval.Hour, sym);
            foreach (TickImpl k in tape)
            {
                bl.newTick(k);
                if (bl.NewBar)
                    newbars++;
            }

            Assert.AreEqual(2, newbars);
        }


    }
}

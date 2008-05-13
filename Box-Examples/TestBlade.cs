using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLib;

namespace Blade
{
    [TestFixture]
    public class TestBlade
    {
        public TestBlade() { }


        [Test]
        public void Basics()
        {
            const string sym = "TST";
            const int d = 20080509;
            const int t = 935;
            const string x = "NYSE";
            Tick[] ticklist = new Tick[] { 
                Tick.NewTrade(sym,d,t,0,10,100,x),
                Tick.NewTrade(sym,d,t+1,0,10,100,x),
                Tick.NewTrade(sym,d,t+2,0,10,100,x),
                Tick.NewTrade(sym,d,t+3,0,10,100,x),
                Tick.NewTrade(sym,d,t+4,0,15,100,x), // blade up
                Tick.NewTrade(sym,d,t+5,0,16,100,x), // new bar (blades reset)
                Tick.NewTrade(sym,d,t+6,0,16,100,x),
                Tick.NewTrade(sym,d,t+7,0,10,100,x), // blade down
                Tick.NewTrade(sym,d,t+7,10,10,100,x), // still a blade down (same bar)
            };

            BarList bl = new BarList(BarInterval.FiveMin,sym);
            Blade b = new Blade(.2m); // 20 percent move is a blade
            int up=0,down=0,newbar=0;

            foreach (Tick k in ticklist)
            {
                bl.AddTick(k);
                b.newBar(bl);
                if (bl.NewBar) newbar++;
                if (b.BladesUP) up++;
                if (b.BladesDOWN) down++;
            }

            Assert.That(up == 1);
            Assert.That(down == 2);
            Assert.That(newbar == 2);

        }
    }
}

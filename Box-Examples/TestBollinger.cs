using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;
using NUnit.Framework;


namespace box
{
    [TestFixture]
    public class TestBollinger
    {
        public TestBollinger()
        {
        }
        const string s = "TST";
        const string x = "NYS";
        const int d = 20070917;
        const int t = 929;

        // we test 2 standard deviations here


        Tick[] ticklist = new Tick[] {
            Tick.NewTrade(s,d,t,0,1,100,x),
            Tick.NewTrade(s,d,t+1,0,2,100,x),
            Tick.NewTrade(s,d,t+2,0,3,100,x),
            Tick.NewTrade(s,d,t+3,0,4,100,x),
            Tick.NewTrade(s,d,t+4,0,5,100,x),
        };

        [Test]
        public void BollingerTestticklookbacks()
        {
            Bollinger bbbb = new Bollinger(2, 4);
            foreach (Tick t in ticklist)
            {
                bbbb.newTick(t);
            }


            Assert.That(bbbb.Mean == 3.5M, bbbb.Mean.ToString());
            Assert.That(bbbb.Devavg == 1.25M, bbbb.Devavg.ToString());
            Assert.That(bbbb.Sd == 1.1180339887498948482045868343656, bbbb.Sd.ToString());
            Assert.That(bbbb.Upperband == 5.73606797749978M, bbbb.Upperband.ToString());
            Assert.That(bbbb.Lowerband == 1.26393202250022M, bbbb.Lowerband.ToString());
        }
        
        
        
        [Test]
        public void BollingerTesttick()
        {
            Bollinger b = new Bollinger(2, 10);

            foreach (Tick t in ticklist)
            {
                b.newTick(t);
            }


            Assert.That(b.Mean == 3, b.Mean.ToString());
            Assert.That(b.Devavg == 2, b.Devavg.ToString());
            Assert.That(b.Sd == 1.4142135623730950488016887242097, b.Sd.ToString());
            Assert.That(b.Upperband == 5.8284271247462M, b.Upperband.ToString());
            Assert.That(b.Lowerband == 0.1715728752538M, b.Lowerband.ToString());
        }


        [Test]
        public void BollingerTestticknoargs()
        {
            Bollinger bb = new Bollinger();
            foreach (Tick t in ticklist)
            {
                bb.newTick(t);
            }


            Assert.That(bb.Mean == 3, bb.Mean.ToString());
            Assert.That(bb.Devavg == 2, bb.Devavg.ToString());
            Assert.That(bb.Sd == 1.4142135623730950488016887242097, bb.Sd.ToString());
            Assert.That(bb.Upperband == 5.8284271247462M, bb.Upperband.ToString());
            Assert.That(bb.Lowerband == 0.1715728752538M, bb.Lowerband.ToString());
        }

        [Test]

        public void BollingerBarlisttest()
        {

            string symbol = "TST";


            BarList bl = new BarList(BarInterval.Minute, symbol);
            Bollinger bbb = new Bollinger(2, BarInterval.Minute, 10);
            foreach (Tick t in ticklist)
            {
                bl.AddTick(t);
                bbb.newBar(bl);
            }

            Assert.That(bbb.Mean == 3, bbb.Mean.ToString());
            Assert.That(bbb.Devavg == 2, bbb.Devavg.ToString());
            Assert.That(bbb.Sd == 1.4142135623730950488016887242097, bbb.Sd.ToString());
            Assert.That(bbb.Upperband == 5.8284271247462M, bbb.Upperband.ToString());
        }
        [Test]
        public void BollingerBarlisttestlookbacks()
        {
            string symbol = "TST";

            BarList bl = new BarList(BarInterval.Minute, symbol);
            Bollinger bbb = new Bollinger(2, BarInterval.Minute, 4);
            foreach (Tick t in ticklist)
            {
                bl.AddTick(t);
                bbb.newBar(bl);
            }

            Assert.That(bbb.Mean == 3.5M, bbb.Mean.ToString());
            Assert.That(bbb.Devavg == 1.25M, bbb.Devavg.ToString());
            Assert.That(bbb.Sd == 1.1180339887498948482045868343656, bbb.Sd.ToString());
            Assert.That(bbb.Upperband == 5.73606797749978M, bbb.Upperband.ToString());
            Assert.That(bbb.Lowerband == 1.26393202250022M, bbb.Lowerband.ToString());
        }

        [Test]
        public void QuoteOnlyTest()
        {
            Tick[] timesales = new Tick[] {
                Tick.NewBid("TST",100m,100),
                Tick.NewAsk("TST",100.1m,200),
            };

            Bollinger b = new Bollinger();
            BarList bl = new BarList(BarInterval.FiveMin, "TST");

            foreach (Tick k in timesales)
            {
                bl.newTick(k);
                b.newBar(bl);
            }

            // average volume should be zero bc
            // with only quotes we should have no bars to process
            Assert.That(b.Mean== 0, b.Mean.ToString());
            Assert.That(!bl.Has(1), bl.ToString());
        }
    
    
    }
        
}










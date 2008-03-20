using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
{
    [TestFixture]
    public class TestBox
    {
        public TestBox()
        {
        }
        int debugs = 0;
        const string s = "TST";
        const string x = "NYS";
        const int d = 20070917;
        const int t = 929;

        Tick[] timesales = new Tick[] { 
                Tick.NewTrade(s,d,t,0,10,100,x),
                Tick.NewTrade(s,d,t+1,0,10,100,x),
                Tick.NewTrade(s,d,t+2,0,10,100,x),
                Tick.NewTrade(s,d,t+3,0,10,100,x),
            };

        [Test]
        public void BlankBox()
        {

            Box b = new Box();
            Assert.That(!b.Off);
            Assert.That(b.QuickOrder);
            Assert.That(b.Trades == 0);
            Assert.That(!b.UseLimits);
            Assert.That(b.NewsHandler == null);
            Assert.That(!b.Debug);
            Assert.That(b.DayStart == 930);
            // this box doesn't do anything, so it returns a blank/invalid order
            // for every tick that it trades
            for (int i = 0; i < timesales.Length; i++)
                Assert.That(!b.Trade(timesales[i], new BarList(), new Position(s), new BoxInfo()).isValid);
            // no debugs were sent
            Assert.That(debugs == 0);
            
        }
        class Always : Box 
        {
            public Always() : base() { MinSize = 100; }
            public Always(NewsService ns) : base(ns) { MinSize = 100; }
            protected override int  Read(Tick tick, BarList bl, BoxInfo boxinfo)
            {
                D("entering");
                return MinSize;
            }
        }

        [Test]
        public void AlwaysEnter()
        {
            Always b = new Always();
            Assert.That(b.MinSize == 100);
            int good = 0;
            for (int i = 0; i < timesales.Length; i++)
                if (b.Trade(timesales[i], new BarList(), new Position(s), new BoxInfo()).isValid)
                    good++;
            // first trade was pre-market so we only have 3 total;
            Assert.That(good == 3);
            // no debugs were sent
            Assert.That(debugs == 0);
        }


        [Test]
        public void NewsTest()
        {
            NewsService ns = new NewsService();
            ns.NewsEventSubscribers +=new NewsDelegate(ns_NewsEventSubscribers);
            Always b = new Always(ns);
            // this time we want to throw news events for debugging statements
            b.Debug = true;
            int good = 0;
            for (int i = 0; i < timesales.Length; i++)
                if (b.Trade(timesales[i], new BarList(), new Position(s), new BoxInfo()).isValid)
                    good++;
            Assert.That(good == 3);
            // news from the box was received.
            Assert.That(debugs>0);

        }
        void  ns_NewsEventSubscribers(News news)
        {
            Console.WriteLine(news.Msg);
 	        debugs++;
        }

    }

}

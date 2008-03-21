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
        const string f = "/SPX";
        const decimal fnom = 1300;
        Index[] futures = new Index[] { 
            new Index(f,fnom,fnom,fnom,fnom,fnom),
            new Index(f,fnom+2,fnom,fnom+2,fnom,fnom+2),
            new Index(f,fnom+3,fnom,fnom+3,fnom,fnom+3),
            new Index(f,fnom+2.5m,fnom,fnom+3,fnom,fnom+2.5m),
        };

        // test the constructor and make sure it never enters a trade
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

        // make sure this box generates an order for every trade
        [Test]
        public void AlwaysEnter()
        {
            Always b = new Always();
            Assert.That(b.MinSize == 100);
            int good = 0;
            int i = 0;

            if (b.Trade(timesales[i++], new BarList(), new Position(s), new BoxInfo()).isValid)
                    good++;
            Assert.That(b.Trades == 0);
            Assert.That(b.Adjusts == 0);
            if (b.Trade(timesales[i++], new BarList(), new Position(s), new BoxInfo()).isValid)
                good++;
            Assert.That(b.Trades == 0);
            Assert.That(b.Adjusts == 1); 
            if (b.Trade(timesales[i++], new BarList(), new Position(s), new BoxInfo()).isValid)
                good++;
            Assert.That(b.Trades == 0);
            Assert.That(b.Adjusts == 2); 
            if (b.Trade(timesales[i++], new BarList(), new Position(s), new BoxInfo()).isValid)
                good++;
            Assert.That(b.Trades == 0);
            Assert.That(b.Adjusts == 3);
            // first trade was pre-market so we only have 3 total;
            Assert.That(good == 3);
            // no debugs were sent
            Assert.That(debugs == 0);
        }

        // make sure debugs are received as news events when debugging is enabled
        [Test]
        public void NewsTest()
        {
            NewsService ns = new NewsService();
            ns.NewsEventSubscribers +=new NewsDelegate(ns_NewsEventSubscribers);
            Always b = new Always(ns);
            // this time we want to throw news events for debugging statements
            b.Debug = true;
            int good = 0;
            b.D("Starting debug test for NUnit...");
            for (int i = 0; i < timesales.Length; i++)
                if (b.Trade(timesales[i], new BarList(), new Position(s), new BoxInfo()).isValid)
                    good++;
            b.D("NUnit testing complete...");
            Assert.That(good == 3);
            // news from the box was received.
            Assert.That(debugs>0);

        }
        void  ns_NewsEventSubscribers(News news)
        {
            Console.WriteLine(news.Msg);
 	        debugs++;
        }

        // Make sure indicies are received
        public class IndexBox : Box
        {
            public IndexBox() : base() { GotIndex += new IndexDelegate(IndexBox_GotIndex); }
            public int indexticks = 0;
            public bool athigh = false;
            void IndexBox_GotIndex(Index idx)
            {
                athigh = (idx.Value == idx.High);
                indexticks++;
            }
        }


        [Test]
        public void IndexTest()
        {
            IndexBox ibox = new IndexBox();
            int highs = 0;
            for (int i = 0; i < futures.Length; i++)
            {
                // send futures update
                ibox.NewIndex(futures[i]);
                // trade the box
                ibox.Trade(timesales[i], new BarList(), new Position(s), new BoxInfo());
                if (ibox.athigh) highs++;
            }
            Assert.That(highs == 3);
            Assert.That(ibox.indexticks == 4);
        }

        public class LimitsTest : Box
        {
            public LimitsTest() { UseLimits = true; }
            protected override int Read(Tick tick, BarList bl, BoxInfo boxinfo)
            {
                // go short off first trade
                if (tick.isTrade && (PosSize == 0)) return MinSize;
                // cover at the next opportunity
                else if (tick.isTrade && (PosSize != 0))
                {
                    Shutdown("All done for today");
                    return Flat;
                }
                return 0;
            }
        }

        [Test]
        public void UseLimitsTest()
        {
            LimitsTest b = new LimitsTest();
            // we're skipping the first trade bc it's pre-market and we're not testing
            // that in this test
            int i = 1;
            Order o;
            Position p = new Position(s);
            Assert.That(b.Trades == 0);
            Assert.That(b.Adjusts == 0);
            Assert.That(b.UseLimits);
            Assert.That(!b.Off);
            Assert.That(b.PosSize == 0);
            o = b.Trade(timesales[i++], new BarList(), p, new BoxInfo());
            Assert.That(o.isValid);
            // fill our order with next tick and just our position
            o.Fill(timesales[i]);
            p.Adjust((Trade)o);
            Assert.That(b.Adjusts == 1);
            Assert.That(b.Trades == 0);
            o = b.Trade(timesales[i++], new BarList(), p, new BoxInfo());
            Assert.That(o.isValid);
            Assert.That(b.Adjusts == 2);
            Assert.That(b.Trades == 1); // should be flat now
            o = b.Trade(timesales[i++], new BarList(), new Position(s), new BoxInfo());
            Assert.That(!o.isValid); // no more orders, as
            Assert.That(b.Off); // we should be shutdown
        }

    }

}

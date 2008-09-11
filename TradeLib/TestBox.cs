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
            Assert.That(b.Turns == 0);
            Assert.That(!b.TradeCaps);
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
            public Always() { MinSize = 100; }
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
            b.AllowMultipleOrders = true;
            Assert.That(b.MinSize == 100);
            int good = 0;
            int i = 0;
            Order o = new Order();
            o = b.Trade(timesales[i++], new BarList(), new Position(s), new BoxInfo());
            if (o.isValid)
                good++;
            Assert.That(b.Turns == 0);
            Assert.That(b.Adjusts == 0);
            o = b.Trade(timesales[i++], new BarList(), new Position(s), new BoxInfo());
            if (o.isValid)
                good++;
            Assert.That(b.Turns == 0);
            Assert.That(b.Adjusts == 1);
            o = b.Trade(timesales[i++], new BarList(), new Position(s), new BoxInfo());
            if (o.isValid)
                good++;
            Assert.That(b.Turns == 0);
            Assert.That(b.Adjusts == 2);
            o = b.Trade(timesales[i++], new BarList(), new Position(s), new BoxInfo());
            if (o.isValid)
                good++;
            Assert.That(b.Turns == 0);
            Assert.That(b.Adjusts == 3);
            // first trade was pre-market so we only have 3 total;
            Assert.That(good == 3);
            // no debugs were sent
            Assert.That(debugs == 0);
        }

        [Test]
        // make sure this box only generates orders when last one has been filled
        public void OneOrderAtTime() 
        {
            Always b = new Always();
            b.AllowMultipleOrders = false; // this is the default, but it's what we're testing
            b.MaxSize = Int32.MaxValue; // lets not restrict our maximum position for this example
            Broker broker = new Broker();
            Tick t;
            Assert.That(b.MinSize == 100);
            int good = 0;
            int i = 0;
            Order o = new Order();
            t = new Tick(timesales[i++]);
            broker.Execute(t);
            o = b.Trade(t, new BarList(), broker.GetOpenPosition(s), new BoxInfo());
            broker.sendOrder(o);
            if (o.isValid)
                good++;
            Assert.That(b.Turns == 0);
            Assert.That(b.Adjusts == 0);
            t = new Tick(timesales[i++]);
            broker.Execute(t);
            o = b.Trade(t, new BarList(), broker.GetOpenPosition(s), new BoxInfo());
            broker.sendOrder(o);
            if (o.isValid)
                good++;
            Assert.That(b.Turns == 0);
            Assert.That(b.Adjusts == 1);
            t = new Tick(timesales[i++]);
            broker.Execute(t);
            o = b.Trade(t, new BarList(), broker.GetOpenPosition(s), new BoxInfo());
            // lets change this to a limit order, so he doesn't get filled on just any tick
            o.price = 1;
            broker.sendOrder(o);
            if (o.isValid)
                good++;
            Assert.That(b.Turns == 0);
            Assert.That(b.Adjusts == 2);
            t = new Tick(timesales[i++]);
            broker.Execute(t);
            o = b.Trade(t, new BarList(), broker.GetOpenPosition(s), new BoxInfo());
            broker.sendOrder(o);
            if (o.isValid)
                good++;
            Assert.That(b.Turns == 0);
            Assert.That(b.Adjusts == 2);
            // first trade was pre-market 2nd order was never filled so 3rd was ignored... 2 total.
            Assert.That(good == 2);
        }


        // make sure debugs are received as news events when debugging is enabled
        [Test]
        public void NewsTest()
        {
            // subscribe to news service that will count everytime a debug is sent
            Always b = new Always(); // send debugs from this box to our news service
            b.GotDebug += new DebugFullDelegate(b_GotDebug);
            b.AllowMultipleOrders = true; // lets allow multiple orders for more debugging
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

        void b_GotDebug(Debug debug)
        {
            Console.WriteLine(debug.Msg);
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
            public LimitsTest() { TradeCaps = true; }
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
        public void ThrottlesTest()
        {
            LimitsTest b = new LimitsTest();
            // we're skipping the first trade bc it's pre-market and we're not testing
            // that in this test
            int i = 1;
            Order o;
            Position p = new Position(s);
            Assert.That(b.Turns == 0);
            Assert.That(b.Adjusts == 0);
            Assert.That(b.TradeCaps);
            Assert.That(!b.Off);
            Assert.That(b.PosSize == 0);
            o = b.Trade(timesales[i++], new BarList(), p, new BoxInfo());
            Assert.That(o.isValid);
            // fill our order with next tick and just our position
            o.Fill(timesales[i]);
            p.Adjust((Trade)o);
            Assert.That(b.Adjusts == 1);
            Assert.That(b.Turns == 0);
            o = b.Trade(timesales[i++], new BarList(), p, new BoxInfo());
            Assert.That(o.isValid);
            Assert.That(b.Adjusts == 2);
            Assert.That(b.Turns == 1); // should be flat now
            o = b.Trade(timesales[i++], new BarList(), new Position(s), new BoxInfo());
            Assert.That(!o.isValid); // no more orders, as
            Assert.That(b.Off); // we should be shutdown
        }

        [Test]
        public void MaxSizeTest()
        {
            Always b = new Always();
            b.MinSize = 100;
            b.MaxSize = 200;
            Position p = new Position(s);
            Order o = new Order();
            Assert.That(b.MinSize==100);
            Assert.That(b.MaxSize==200);
            for (int i = 0; i < timesales.Length; i++)
            {
                Tick t = new Tick(timesales[i]);
                if (o.isValid && t.isTrade)
                {
                    o.Fill(t);
                    p.Adjust((Trade)o);
                    o = new Order();
                }
                Assert.That(p.Size<=b.MaxSize);
                o = b.Trade(timesales[i], new BarList(), p, new BoxInfo());
            }
            Assert.That(p.Size == 200);

            // Now we'll set maxsize to 100
            b = new Always();
            b.MinSize = 100;
            b.MaxSize = 100;
            p = new Position(s);
            o = new Order();
            Assert.That(b.MinSize == 100);
            Assert.That(b.MaxSize == 100);
            for (int i = 0; i < timesales.Length; i++)
            {
                Tick t = new Tick(timesales[i]);
                if (o.isValid && t.isTrade)
                {
                    o.Fill(t);
                    p.Adjust((Trade)o);
                    o = new Order();
                }
                Assert.That(p.Size <= b.MaxSize);
                o = b.Trade(timesales[i], new BarList(), p, new BoxInfo());
            }
            Assert.That(p.Size == 100);

        }

        public class FullOrder : Box
        {
            public FullOrder() 
            { 
                QuickOrder = false; 
                Name = "full order"; 
                AllowMultipleOrders = true;
                Debug = true;
            }
            int orders = 0;
            protected override Order ReadOrder(Tick tick, BarList bl, BoxInfo boxinfo)
            {
                Order o = new Order();

                if (orders == 0)
                {
                    o = new LimitOrder(s, true, 200, 10);
                }
                if (orders==1)
                    CancelOrders(true);
                if (o.isValid)
                    orders++;
                return o;
            }
        }

        Broker b = new Broker();
        FullOrder fb = new FullOrder();

        [Test]
        public void FullOrderAndCancel()
        {

            b.GotOrder += (fb.gotOrderSink);
            b.GotOrderCancel += new Broker.OrderCancelDelegate(b_GotOrderCancel);
            fb.CancelOrderSource += new UIntDelegate(fb_CancelOrderSource);
            fb.Symbol = s;
            fb.GotDebug += new DebugFullDelegate(f_GotDebug);

            Tick[] timesales = new Tick[] { 
                Tick.NewTrade(s,d,t+1,0,100,100,x),
                Tick.NewTrade(s,d,t+2,0,100,100,x),
                Tick.NewTrade(s,d,t+3,0,100,100,x),
                Tick.NewTrade(s,d,t+3,0,100,100,x),
            };

            int i = 0;
            Tick k = timesales[i++];
            b.Execute(k);
            Assert.That(b.GetOrderList().Count == 0);
            b.sendOrder(fb.Trade(k, new BarList(), new Position(s), new BoxInfo()));
            Assert.That(b.GetOrderList().Count==1,b.GetOrderList().Count.ToString());

            k = timesales[i++];
            b.Execute(k);
            b.sendOrder(fb.Trade(k, new BarList(), new Position(s), new BoxInfo()));
            Assert.That(b.GetOrderList().Count == 0, b.GetOrderList().Count.ToString());

            k = timesales[i++];
            b.Execute(k);
            b.sendOrder(fb.Trade(k, new BarList(), new Position(s), new BoxInfo()));
            Assert.That(b.GetOrderList().Count == 0, b.GetOrderList().Count.ToString());





        }

        void fb_CancelOrderSource(uint number)
        {
            b.CancelOrder(number);
        }

        void b_GotOrderCancel(string sym, bool side, uint id)
        {
            fb.gotCancelSink(id);
        }

        void f_GotDebug(Debug debug)
        {
            Console.WriteLine(debug.Msg);
        }
    }

}

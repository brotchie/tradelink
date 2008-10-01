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
        Order o = new Order();

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
        int sbcount = 0;
        [Test]
        public void StandardBox()
        {
            DayTradeBox b = new DayTradeBox();
            b.SendOrder += new OrderDelegate(b_SendOrder);

            Assert.That(!b.Off);
            Assert.That(b.DayStart == 930);
            Assert.That(sbcount == 0);
            // default box does not trade
            // for every tick that it trades
            for (int i = 0; i < timesales.Length; i++)
            {
                b.GotTick(timesales[i]);
                Assert.That(!o.isValid);
            }
            Assert.That(sbcount == timesales.Length, sbcount.ToString());
           
        }

        void b_SendOrder(Order order)
        {
            sbcount++;
            o = order;
        }
        class Always : DayTradeBox
        {
            public Always() { MinSize = 100; }
            protected override Order ReadOrder(Tick tick, BarList bl)
            {
                D("entering");
                return new BuyMarket(Symbol,MinSize);
            }
        }

        // make sure this box generates an order for every trade
        [Test]
        public void AlwaysEnter()
        {
            Always b = new Always();
            b.SendOrder+=new OrderDelegate(b_SendOrder);
            sbcount = 0;
            Assert.That(b.MinSize == 100);
            int good = 0;
            int i = 0;
            b.GotTick(timesales[i++]);
            if (o.isValid)
                good++;
            o = new Order();
            b.GotTick(timesales[i++]);
            if (o.isValid)
                good++;
            o = new Order(); 
            b.GotTick(timesales[i++]);
            if (o.isValid)
                good++;
            o = new Order();
            b.GotTick(timesales[i++]);
            if (o.isValid)
                good++;
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
            b.MaxSize = Int32.MaxValue; // lets not restrict our maximum position for this example
            Broker broker = new Broker();
            broker.GotFill += new FillDelegate(broker_GotFill);
            Tick t;
            Assert.That(b.MinSize == 100);
            int good = 0;
            int i = 0;
            o = new Order();
            t = new Tick(timesales[i++]);
            broker.Execute(t);
            o = new Order();
            b.GotTick(t);
            
            broker.sendOrder(o);
            if (o.isValid)
                good++;

            t = new Tick(timesales[i++]);
            broker.Execute(t);
            o = new Order();
            b.GotTick(t); 
            broker.sendOrder(o);
            if (o.isValid)
                good++;
            t = new Tick(timesales[i++]);
            broker.Execute(t);
            o = new Order();
            b.GotTick(t);
            // lets change this to a limit order, so he doesn't get filled on just any tick
            o.price = 1;
            broker.sendOrder(o);
            if (o.isValid)
                good++;

            t = new Tick(timesales[i++]);
            broker.Execute(t);
            o = new Order();
            b.GotTick(t); 
            broker.sendOrder(o);
            if (o.isValid)
                good++;

            // first trade was pre-market 2nd order was never filled so 3rd was ignored... 2 total.
            Assert.That(good == 2);
        }

        void broker_GotFill(Trade t)
        {
            
        }


        // make sure debugs are received as news events when debugging is enabled
        [Test]
        public void NewsTest()
        {
            // subscribe to news service that will count everytime a debug is sent
            Always b = new Always(); // send debugs from this box to our news service
            b.SendDebug += new DebugFullDelegate(b_GotDebug);
            int good = 0;
            for (int i = 0; i < timesales.Length; i++)
            {
                o = new Order();
                b.GotTick(timesales[i]);
                if (o.isValid)                
                    good++;
            }
            Assert.That(good == 3);
            // news from the box was received.
            Assert.That(debugs>0);
        }

        void b_GotDebug(Debug debug)
        {
            debugs++;
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
                b.GotPosition(p);
                b.GotTick(timesales[i]);
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
                b.GotPosition(p);
                b.GotTick(timesales[i]);
            }
            Assert.That(p.Size == 100);

        }

        public class FullOrder : DayTradeBox
        {
            public FullOrder() 
            { 
                Name = "full order"; 
            }
            int orders = 0;
            protected override Order ReadOrder(Tick tick,BarList bl)
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

            b.GotOrder += fb.GotOrder;
            b.GotOrderCancel += new Broker.OrderCancelDelegate(b_GotOrderCancel);
            fb.SendOrder += new OrderDelegate(fb_SendOrder);
            fb.SendCancel += new UIntDelegate(fb_CancelOrderSource);
            fb.Symbol = s;
            fb.SendDebug+= new DebugFullDelegate(f_GotDebug);

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
            o = new Order();
            fb.GotTick(k);
            b.sendOrder(o);
            Assert.That(b.GetOrderList().Count==1,b.GetOrderList().Count.ToString());

            k = timesales[i++];
            b.Execute(k);
            o = new Order();
            fb.GotTick(k);
            b.sendOrder(o);
            Assert.That(b.GetOrderList().Count == 0, b.GetOrderList().Count.ToString());

            k = timesales[i++];
            b.Execute(k);
            o = new Order();
            fb.GotTick(k);
            b.sendOrder(o);
            Assert.That(b.GetOrderList().Count == 0, b.GetOrderList().Count.ToString());
        }

        void fb_SendOrder(Order order)
        {
            o = order;
        }

        void fb_CancelOrderSource(uint number)
        {
            b.CancelOrder(number);
        }

        void b_GotOrderCancel(string sym, bool side, uint id)
        {
            fb.GotOrderCancel(id);
        }

        void f_GotDebug(Debug debug)
        {
            
        }
    }

}

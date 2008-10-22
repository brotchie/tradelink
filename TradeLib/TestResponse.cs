using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
{
    [TestFixture]
    public class TestResponse
    {
        public TestResponse()
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


        // test the constructor and make sure it never enters a trade
        int sbcount = 0;
        [Test]
        public void StandardResponse()
        {
            Never b = new Never();
            b.SendOrder += new OrderDelegate(b_SendOrder);

            Assert.That(!b.isValid);
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
        public class Always : Response
        {
            public int MinSize = 100;
            public int MaxSize = 100;
            public Always() {  }
            public virtual void GotTick(Tick tick)
            {
                SendDebug(Debug.Create("entering"));
                SendOrder(new BuyMarket(tick.symbol, MinSize));
            }

            public virtual void GotOrder(Order order) {}
            public void GotFill(Trade fill) {}
            public void GotOrderCancel(uint cancelid) { }
            public void Reset() { }

            public void GotPosition(Position p) { }

            string[] _inds = new string[0];
            string _name = "";
            string _full = "";

            public bool isValid { get { return true; } }

            public string[] Indicators { get { return _inds; } set { _inds = value; } }

            public string Name { get { return _name; } set { _name = value; } }

            public string FullName { get { return _full; } set { _full = value; } }

            public event DebugFullDelegate SendDebug;
            public event OrderDelegate SendOrder;
            public event UIntDelegate SendCancel;
            public event ObjectArrayDelegate SendIndicators;
        }

        public class Never : Always
        {
            public override void GotTick(Tick tick)
            {
                
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


    }

}

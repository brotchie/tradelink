using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using NUnit.Framework;
using TradeLink.API;

namespace TestTradeLink
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
        Order o = new OrderImpl();

        TickImpl[] timesales = new TickImpl[] { 
                TickImpl.NewTrade(s,d,t,10,100,x),
                TickImpl.NewTrade(s,d,t+100,10,100,x),
                TickImpl.NewTrade(s,d,t+200,10,100,x),
                TickImpl.NewTrade(s,d,t+300,10,100,x),
            };


        // test the constructor and make sure it never enters a trade
        int sbcount = 0;
        [Test]
        public void StandardResponse()
        {
            Never b = new Never();
            b.SendOrder += new OrderDelegate(b_SendOrder);
            sbcount = 0;
            Assert.That(b.isValid);
            Assert.That(sbcount == 0);
            // default response does not trade
            // for every tick that it trades
            for (int i = 0; i < timesales.Length; i++)
            {
                b.GotTick(timesales[i]);
                Assert.That(o.isValid);
            }
            Assert.AreEqual(0, sbcount);
           
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
                SendDebug(DebugImpl.Create("entering"));
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

            public bool isValid { get { return true; } set { } }

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

        // make sure this response generates an order for every trade
        [Test]
        public void AlwaysEnter()
        {
            Always b = new Always();
            b.SendDebug += new DebugFullDelegate(b_SendDebug);
            b.SendOrder+=new OrderDelegate(b_SendOrder);
            sbcount = 0;
            debugs = 0;
            Assert.That(b.MinSize == 100);
            int good = 0;
            int i = 0;
            b.GotTick(timesales[i++]);
            if (o.isValid)
                good++;
            o = new OrderImpl();
            b.GotTick(timesales[i++]);
            if (o.isValid)
                good++;
            o = new OrderImpl(); 
            b.GotTick(timesales[i++]);
            if (o.isValid)
                good++;
            o = new OrderImpl();
            b.GotTick(timesales[i++]);
            if (o.isValid)
                good++;

            Assert.AreEqual(4, good);
            Assert.AreEqual(4, debugs);
        }

        void b_SendDebug(Debug debug)
        {
            debugs++;
        }


        void broker_GotFill(TradeImpl t)
        {
            
        }


        // make sure debugs are received as news events when debugging is enabled
        [Test]
        public void NewsTest()
        {
            // subscribe to news service that will count everytime a debug is sent
            Always b = new Always(); // send debugs from reponse to our news service
            b.SendDebug += new DebugFullDelegate(b_SendDebug);
            b.SendOrder+=new OrderDelegate(b_SendOrder);
            int good = 0;
            debugs = 0;
            for (int i = 0; i < timesales.Length; i++)
            {
                o = new OrderImpl();
                b.GotTick(timesales[i]);
                if (o.isValid)                
                    good++;
            }
            Assert.AreEqual(4, good);
            // news from the box was received.
            Assert.That(debugs>0);
        }





    }

}

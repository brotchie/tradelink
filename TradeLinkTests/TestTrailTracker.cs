using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;
using NUnit.Framework;

namespace TestTradeLink
{
    [TestFixture]
    public class TestTrailTracker
    {
        public TestTrailTracker() { }
        const string SYM = "TST";
        public Tick[] SampleData()
        {
            return new Tick[] {
                TickImpl.NewTrade(SYM,10,100), // get fill for initial position
                TickImpl.NewTrade(SYM,10,100), 
                TickImpl.NewTrade(SYM,10,100),
                TickImpl.NewTrade(SYM,10,100),
                TickImpl.NewTrade(SYM,10,100), 
                TickImpl.NewTrade(SYM,11,100),  // new high
                TickImpl.NewTrade(SYM,10.50m,100), // retrace... FLAT!
                TickImpl.NewTrade(SYM,10.50m,1), // not enough to fill flat order
                TickImpl.NewTrade(SYM,10.50m,100), // flat order should be completely filled here
                TickImpl.NewTrade(SYM,10.50m,100),
                TickImpl.NewTrade(SYM,10.50m,100), // want to make sure we are not oversold
                TickImpl.NewTrade(SYM,10.50m,100),
                TickImpl.NewTrade(SYM,10.50m,100),
            };
        }

        [Test]
        public void Basics()
        {
            // setup trail tracker
            TrailTracker tt = new TrailTracker();
            tt.SendOrder += new OrderDelegate(tt_SendOrder);
            // set 15c trailing stop
            tt.DefaultTrail = new OffsetInfo(0,.15m);
            // verify it's set
            Assert.AreEqual(.15m,tt.DefaultTrail.StopDist);
            // get feed
            Tick [] tape = SampleData();
            // test broker
            Broker b = new Broker();
            // get fills over to trail tracker
            b.GotFill += new FillDelegate(tt.Adjust);
            // take initial position
            b.SendOrderStatus(new MarketOrder(SYM, 100));
            // get orders from trail tracker
            tt.SendOrder += new OrderDelegate(b.SendOrder);
            // no orders to start
            oc = 0;
            // iterate through feed
            for (int i = 0; i < tape.Length; i++ )
            {
                Tick k = tape[i];
                // set a date and time
                k.date = 20070926;
                k.time = 95500;
                // execute orders, nothing to do on first two ticks
                b.Execute(k);
                // pass every tick to tracker
                tt.newTick(k);

            }
            // get position
            Position p = b.GetOpenPosition(SYM);
            // verify position is flat
            Assert.IsTrue(p.isFlat,p.ToString());
            // one retrace sent at the end
            Assert.AreEqual(1, oc);
            
        }

        int oc = 0;
        Order trail = null;
        void tt_SendOrder(Order o)
        {
            oc++;
            trail = o;
        }

        void debug(string msg)
        {
            Console.WriteLine(msg);
        }

        [Test]
        public void TrailStartedAfterFirePoint()
        {
            // setup trail tracker
            TrailTracker tt = new TrailTracker();
            tt.isValid = true;
            const string acct = "DEFAULT";
            tt.pt.DefaultAccount = acct;
            tt.SendOrder += new OrderDelegate(tt_SendOrder);
            //tt.SendDebug += new DebugFullDelegate(tt_SendDebug);
            // set 15c trailing stop
            tt.DefaultTrail = new OffsetInfo(0, .15m);
            // verify it's set
            Assert.AreEqual(.15m, tt.DefaultTrail.StopDist);
            // 
            tt.TrailByDefault = true;
            // put in a position to track
            tt.Adjust(new PositionImpl(SYM, 11.00m, 100, 0m,acct));
            // check position in tt-pt
            Assert.AreEqual(1, tt.pt.Count);
            // manually enter a trail
            tt[SYM] = new OffsetInfo(0m, .25m);
            // check if trail entered for symbol
            Assert.AreEqual(.25m, tt[SYM].StopDist);


            // get feed
            Tick[] tape = SampleData();
            // test broker
            Broker b = new Broker();
            // get fills over to trail tracker
            b.GotFill += new FillDelegate(tt.Adjust);
            // get orders from trail tracker
            tt.SendOrder += new OrderDelegate(b.SendOrder);
            // no orders to start
            oc = 0;
            // iterate through feed
            for (int i = 0; i < tape.Length; i++)
            {
                Tick k = tape[i];
                // set a date and time
                k.date = 20070926;
                k.time = 95500;
                // execute orders, nothing to do on first two ticks
                b.Execute(k);
                // pass every tick to tracker
                tt.newTick(k);

            }

            Assert.AreEqual(1, oc);
        }

        void tt_SendDebug(Debug deb)
        {
            debug(deb.Msg);
        }
        
    }
}

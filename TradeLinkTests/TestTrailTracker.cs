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
                TickImpl.NewTrade(SYM,10,100),
                TickImpl.NewTrade(SYM,10,100),
                TickImpl.NewTrade(SYM,10,100), 
                TickImpl.NewTrade(SYM,11,100),  // new high
                TickImpl.NewTrade(SYM,10.50m,100), // retrace

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
            // test position
            Position tp = new PositionImpl(SYM, 10, 100);
            // no orders to start
            oc = 0;
            // iterate through feed
            for (int i = 0; i < tape.Length; i++ )
            {
                Tick k = tape[i];
                // nothing to do on first two ticks
                if (i==2)
                {
                    // position established on third tick
                    tt.Adjust(tp);
                }
                // no orders sent until retrace happens
                Assert.AreEqual(0, oc);

                // pass every tick to tracker
                tt.GotTick(k);

            }
            // one retrace sent at the end
            Assert.AreEqual(1, oc);
            // verify it offsets position
            Assert.AreEqual(trail.UnsignedSize,tp.UnsignedSize);
            Assert.AreEqual(trail.side, !tp.isLong);
            
        }

        int oc = 0;
        Order trail = null;
        void tt_SendOrder(Order o)
        {
            oc++;
            trail = o;
        }
    }
}

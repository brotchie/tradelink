using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using NUnit.Framework;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TestBarList
    {
        int newbars = 0;
        [Test]
        public void FiveMin()
        {
            // get some sample data to fill barlist
            Tick[] ticklist = SampleData();
            // prepare barlist
            BarListImpl bl = new BarListImpl(BarInterval.FiveMin);
            bl.GotNewBar+=new SymBarIntervalDelegate(bl_GotNewBar);
            // reset count
            newbars = 0;
            // create bars from all ticks available
            foreach (TickImpl k in ticklist)
            {
                /// add tick to bar
                bl.newTick(k);
            }

            // verify we had expected number of bars
            Assert.AreEqual(3,newbars);
            // verify symbol was set
            Assert.AreEqual(sym, bl.Symbol);
            // verify each bar symbol matches barlist
            foreach (Bar b in bl)
                Assert.AreEqual(bl.Symbol, b.Symbol);
        }

        [Test]
        public void PointFiveMin()
        {
            // get some sample data to fill barlist
            Tick[] ticklist = SampleData();
            // prepare barlist
            BarListImpl bl = new BarListImpl(BarInterval.FiveMin);
            bl.GotNewBar += new SymBarIntervalDelegate(bl_GotNewBar);
            // reset count
            newbars = 0;
            // create bars from all ticks available
            foreach (TickImpl k in ticklist)
            {
                /// add tick to bar
                bl.newPoint(k.symbol,k.trade,k.time,k.date,k.size);
            }

            // verify we had expected number of bars
            Assert.AreEqual(3, bl.Count);
        }

        [Test]
        public void OneMinute()
        {
            // prepare barlist
            BarList bl = new BarListImpl(BarInterval.Minute);
            // reset count
            int newbars = 0;
            // build bars from ticks available
            foreach (TickImpl k in SampleData())
            {
                // add tick to bar
                bl.newTick(k);
                // count if it's a new bar
                if (bl.RecentBar.isNew)
                    newbars++;
            }
            // verify expected # of bars are present
            Assert.AreEqual(9, newbars);
            // verify barcount is same as newbars
            Assert.AreEqual(newbars, bl.Count);

        }

        [Test]
        public void PointMinute()
        {
            // prepare barlist
            BarList bl = new BarListImpl(BarInterval.Minute);
            // reset count
            int newbars = 0;
            // build bars from ticks available
            foreach (TickImpl k in SampleData())
            {
                // add tick to bar
                bl.newPoint(k.symbol,k.trade,k.time,k.date,k.size);
                // count if it's a new bar
                if (bl.RecentBar.isNew)
                    newbars++;
            }
            // verify expected # of bars are present
            Assert.AreEqual(9, newbars);
            // verify barcount is same as newbars
            Assert.AreEqual(newbars, bl.Count);
        }


        const string sym = "TST";

        public static Tick[] SampleData()
        {

            const int d = 20070517;
            const int t = 93500;
            const string x = "NYSE";
            Tick[] tape = new Tick[] { 
                TickImpl.NewTrade(sym,d,t,10,100,x), // new on all intervals
                TickImpl.NewTrade(sym,d,t+100,10,100,x), // new on 1min
                TickImpl.NewTrade(sym,d,t+200,10,100,x),
                TickImpl.NewTrade(sym,d,t+300,10,100,x),
                TickImpl.NewTrade(sym,d,t+400,15,100,x), 
                TickImpl.NewTrade(sym,d,t+500,16,100,x), //new on 5min
                TickImpl.NewTrade(sym,d,t+600,16,100,x),
                TickImpl.NewTrade(sym,d,t+700,10,100,x), 
                TickImpl.NewTrade(sym,d,t+710,10,100,x), 
                TickImpl.NewTrade(sym,d,t+10000,10,100,x), // new on hour interval
            };
            return tape;
        }

        [Test]
        public void BarMath()
        {
            // get tickdata
            Tick[] tape = SampleData();
            // create bar
            BarList bl = new BarListImpl(BarInterval.Minute);
            // pass ticks to bar
            foreach (Tick k in tape)
                bl.newTick(k);
            // verify HH
            Assert.AreEqual(16, Calc.HH(bl));
            // verify LL
            Assert.AreEqual(10, Calc.LL(bl));
            // verify average
            Assert.AreEqual(11.888888888888888888888888889m, Calc.Avg(bl.Open()));
        }

        [Test]
        public void HourTest()
        {
            // get data
            Tick[] tape = SampleData();
            // count new hour bars
            newbars = 0;
            // setup hour bar barlist
            BarListImpl bl = new BarListImpl(BarInterval.Hour, sym);
            // handle new bar events
            bl.GotNewBar+=new SymBarIntervalDelegate(bl_GotNewBar);
            // add ticks to bar
            foreach (Tick k in tape)
            {
                // add ticks
                bl.newTick(k);
            }
            // make sure we have at least 1 bars
            Assert.IsTrue(bl.Has(1));
            // make sure we actually have two bars
            Assert.AreEqual(2, newbars);
            Assert.AreEqual(bl.Count, newbars);
        }

        [Test]
        public void PointHour()
        {
            // get data
            Tick[] tape = SampleData();
            // count new hour bars
            newbars = 0;
            // setup hour bar barlist
            BarListImpl bl = new BarListImpl(BarInterval.Hour, sym);
            // handle new bar events
            bl.GotNewBar += new SymBarIntervalDelegate(bl_GotNewBar);
            // add ticks to bar
            foreach (Tick k in tape)
            {
                // add ticks
                bl.newPoint(k.symbol,k.trade,k.time,k.date,k.size);
            }
            // make sure we have at least 1 bars
            Assert.IsTrue(bl.Has(1));
            // make sure we actually have two bars
            Assert.AreEqual(2, bl.Count);
        }

        [Test]
        public void DefaultIntervalAndReset()
        {
            // get some data
            Tick[] tape = SampleData();
            // setup an hour barlist
            BarList bl = new BarListImpl();
            bl.DefaultInterval = BarInterval.Hour;
            // build the barlist
            foreach (Tick k in tape)
                bl.newTick(k);
            // make sure we have 2 hour bars
            Assert.AreEqual(2, bl.Count);
            // switch default
            bl.DefaultInterval = BarInterval.FiveMin;
            // make sure we have 3 5min bars
            Assert.AreEqual(3, bl.Count);
            // reset it
            bl.Reset();
            // verify we have no data
            Assert.AreEqual(0, bl.Count);
        }


        [Test]
        public void NewBarEvent()
        {
            // get tickdata
            Tick[] tape = SampleData();
            // reset bar count
            newbars = 0;
            // request hour interval
            BarList bl = new BarListImpl(BarInterval.Hour, sym);
            // handle new bars
            bl.GotNewBar += new SymBarIntervalDelegate(bl_GotNewBar);


            foreach (TickImpl k in tape)
                bl.newTick(k);

            Assert.AreEqual(2, newbars);
        }

        void bl_GotNewBar(string symbol, int interval)
        {
            newbars++;
        }

        [Test]
        public void FromEPF()
        {
            // get sample tick data
            BarList bl = BarListImpl.FromEPF("FTI20070926.EPF");
            // verify expected number of 5min bars exist (78 in 9:30-4p)
            Assert.AreEqual(83, bl.Count);

        }

        [Test]
        public void FromTIK()
        {
            // get sample tick data
            BarList bl = BarListImpl.FromTIK("FTI20070926.TIK");
            // verify expected number of 5min bars exist (78 in 9:30-4p)
            Assert.AreEqual(83, bl.Count);

        }

        [Test]
        public void FromGoogle()
        {
            // get a year chart
            BarList bl = BarListImpl.DayFromGoogle("IBM");
            // make sure it's there
            Assert.IsTrue(bl.isValid);
            // verify we have at least a year of bar data
            Assert.GreaterOrEqual(bl.Count,199);
        }

        [Test]
        public void NegativeBars()
        {
            // get sample tick data
            BarList bl = BarListImpl.FromEPF("FTI20070926.EPF");
            // verify expected number of 5min bars exist (78 in 9:30-4p)
            Assert.AreEqual(83, bl.Count);
            // verify that 5th bar from end is same as 77th bar
            Assert.AreEqual(bl[-5].High, bl[77].High);
            Assert.AreEqual(bl[-5].Open, bl[77].Open);
            Assert.AreEqual(bl[-5].Low, bl[77].Low);
            Assert.AreEqual(bl[-5].Close, bl[77].Close);
            Assert.AreEqual(bl[-5].Bardate, bl[77].Bardate);
            Assert.AreEqual(bl[-5].Bartime, bl[77].Bartime);
        }

        [Test]
        public void CustomInterval()
        {
            // request 5 second bars
            const int MYINTERVAL = 5;
            BarList bl = new BarListImpl(sym, MYINTERVAL);
            // verify custom interval
            Assert.AreEqual(MYINTERVAL, bl.DefaultCustomInterval);
            Assert.AreEqual(MYINTERVAL, bl.CustomIntervals[0]);
            // iterate ticks
            foreach (Tick k in SampleData())
                bl.newTick(k);
            // count em
            Assert.AreEqual(10, bl.Count);

        }

        [Test]
        public void TickInterval()
        {
            // request 2 tick bars
            const int MYINTERVAL = 2;
            BarList bl = new BarListImpl(sym, MYINTERVAL, BarInterval.CustomTicks);
            // verify custom interval
            Assert.AreEqual(MYINTERVAL, bl.DefaultCustomInterval);
            Assert.AreEqual(MYINTERVAL, bl.CustomIntervals[0]);
            // iterate ticks
            foreach (Tick k in SampleData())
                bl.newTick(k);
            // count em
            Assert.AreEqual(5, bl.Count);
        }

        [Test]
        public void VolInterval()
        {            // request 300 volume bars
            const int MYINTERVAL = 300;
            BarList bl = new BarListImpl(sym, MYINTERVAL, BarInterval.CustomVol);
            // verify custom interval
            Assert.AreEqual(MYINTERVAL, bl.DefaultCustomInterval);
            Assert.AreEqual(MYINTERVAL, bl.CustomIntervals[0]);
            // iterate ticks
            foreach (Tick k in SampleData())
                bl.newTick(k);
            // count em
            Assert.AreEqual(4, bl.Count);
        }

        [Test]
        public void AsyncFromGoogle()
        {
            bool r = BarListImpl.DayFromGoogleAsync("GE", new BarListDelegate(testbar));

            Assert.IsTrue(r);
            // no result yet
            Assert.IsNull(blt);
            // keep track of polls
            int polls = 0;
            // wait moment for result
            do
            {
                System.Threading.Thread.Sleep(100);
            }
            while ((blt == null) && (polls++ < 30));
            // verify result
            Assert.AreEqual(blt.Symbol, "GE");
            Assert.GreaterOrEqual(blt.Count, 199);
                


        }

        BarList blt = null;
        
        void testbar(BarList bars)
        {
            blt = bars;
        }


    }
}

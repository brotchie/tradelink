using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLink.Common;

namespace TestTradeLink
{
    [TestFixture]
    public class TestHistSim
    {
        public TestHistSim() { }

        const double EXPECT2SYMTIME = .6;

        [Test]
        public void RawTicks()
        {
            HistSim h = new HistSim(Environment.CurrentDirectory+"\\");
            h.Initialize();
            h.GotTick += new TradeLink.API.TickDelegate(h_GotTick);

            Assert.AreEqual(0, tickcount);
            Assert.AreEqual(0, syms.Count);
            Assert.AreEqual(0, lasttime);
            Assert.Greater(h.TicksPresent, 0);

            DateTime start = DateTime.Now;

            h.PlayTo(HistSim.ENDSIM);
            

            double time = DateTime.Now.Subtract(start).TotalSeconds;

            // make sure ticks arrived in order
            Assert.IsTrue(GOODTIME,"Tick arrived out-of-order.");
            // check running time
            Assert.LessOrEqual(time, EXPECT2SYMTIME,"may fail on slow machines");
            Assert.AreEqual(3,syms.Count);
            // tick count is = 42610 (FTI) + 5001 (SPX)
            Assert.AreEqual(42610 + 4991 + 8041, tickcount);
            // variance from approximate count should be less than 1%
            Assert.Less((tickcount - h.TicksPresent) / h.TicksPresent, .01);
            // actual count should equal simulation count
            Assert.AreEqual(h.TicksProcessed, tickcount);
            // last time is 1649 on SPX
            Assert.AreEqual(20080318155843, lasttime);
            // printout simulation runtime
            Console.WriteLine();
            Console.WriteLine("Ticks: " + tickcount + ", versus: " + h.TicksPresent + " estimated.");
            Console.WriteLine("Runtime: " + time.ToString("N2") + "sec, versus: " + EXPECT2SYMTIME + "sec expected.");
            Console.WriteLine("Speed: " + ((double)tickcount / time).ToString("N0") + " ticks/sec");
        }
        int tickcount = 0;
        List<string> syms = new List<string>();
        long lasttime = 0;
        bool GOODTIME = true;
        void h_GotTick(TradeLink.API.Tick t)
        {
            if (!syms.Contains(t.symbol))
                syms.Add(t.symbol);
            tickcount++;
            bool viol = t.datetime<lasttime;
            GOODTIME &= !viol;
            lasttime = t.datetime;
        }


        public void ExecutionTest()
        {
            HistSim h = new HistSim(Environment.CurrentDirectory + "\\");
            h.Initialize();


            Assert.AreEqual(0, tickcount);
            Assert.AreEqual(0, syms.Count);
            Assert.AreEqual(0, lasttime);
            Assert.Greater(h.TicksPresent, 0);

            DateTime start = DateTime.Now;

            h.PlayTo(HistSim.ENDSIM);


            double time = DateTime.Now.Subtract(start).TotalSeconds;

        }
    }
}

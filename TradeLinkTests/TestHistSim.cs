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

        double EXPECTRAW = .6;
        double EXPECTEX = .8;
        double EXPECTBARS = .6;

        [Test]
        public void RawPerformance()
        {
            HistSim h = new HistSim(Environment.CurrentDirectory+"\\");
            h.Initialize();
            h.GotTick += new TradeLink.API.TickDelegate(raw_GotTick);

            tickcount = 0;
            syms.Clear();
            lasttime = 0;

            Assert.AreEqual(0, tickcount);
            Assert.AreEqual(0, syms.Count);
            Assert.AreEqual(0, lasttime);
            Assert.Greater(h.TicksPresent, 0);
            if (Environment.ProcessorCount == 1) EXPECTRAW *= 2.5;

            DateTime start = DateTime.Now;

            h.PlayTo(HistSim.ENDSIM);
            

            double time = DateTime.Now.Subtract(start).TotalSeconds;

            // make sure ticks arrived in order
            Assert.IsTrue(GOODTIME,"Tick arrived out-of-order.");
            // check running time
            Assert.LessOrEqual(time, EXPECTRAW,"may fail on slow machines");
            Assert.AreEqual(3,syms.Count);
            // tick count is = 42610 (FTI) + 5001 (SPX) + 8041 (ABN)
            Assert.AreEqual(42610 + 4991 + 8041, tickcount);
            // variance from approximate count should be less than 1%
            Assert.Less((tickcount - h.TicksPresent) / h.TicksPresent, .01);
            // actual count should equal simulation count
            Assert.AreEqual(h.TicksProcessed, tickcount);
            // last time is 1649 on SPX
            Assert.AreEqual(20080318155843, lasttime);
            // printout simulation runtime
            Console.WriteLine("Raw runtime: " + time.ToString("N2") + "sec, versus: " + EXPECTRAW + "sec expected.");
            Console.WriteLine("Raw speed: " + ((double)tickcount / time).ToString("N0") + " ticks/sec");
        }
        int tickcount = 0;
        List<string> syms = new List<string>();
        long lasttime = 0;
        bool GOODTIME = true;
        void raw_GotTick(TradeLink.API.Tick t)
        {
            if (!syms.Contains(t.symbol))
                syms.Add(t.symbol);
            tickcount++;
            bool viol = t.datetime<lasttime;
            GOODTIME &= !viol;
            lasttime = t.datetime;
        }

        HistSim execute = new HistSim(Environment.CurrentDirectory + "\\");
        int fillcount = 0;
        int desiredfills = 1000;
        [Test]
        public void ExecutionPerformance()
        {
            execute.GotTick += new TradeLink.API.TickDelegate(execute_GotTick);
            execute.SimBroker.GotFill += new TradeLink.API.FillDelegate(SimBroker_GotFill);

            execute.Initialize();

            tickcount = 0;
            lasttime = 0;

            Assert.AreEqual(0, lasttime);
            Assert.Greater(execute.TicksPresent, 0);
            if (Environment.ProcessorCount == 1) EXPECTEX *= 2.5;

            DateTime start = DateTime.Now;

            execute.PlayTo(HistSim.ENDSIM);

            double time = DateTime.Now.Subtract(start).TotalSeconds;

            Assert.AreEqual(desiredfills, fillcount);
            Assert.LessOrEqual(time, EXPECTEX);
            Console.WriteLine("Execution runtime: " + time.ToString("N2") + "sec, versus: " + EXPECTEX + "sec expected.");
            Console.WriteLine("Execution " + ((double)tickcount / time).ToString("N0") + " ticks/sec.  "+((double)fillcount / time).ToString("N0") + " fills/sec");
        }

        void SimBroker_GotFill(TradeLink.API.Trade t)
        {
            fillcount++;
        }

        void execute_GotTick(TradeLink.API.Tick t)
        {
            tickcount++;
            // generate fills periodically
            if (fillcount >= desiredfills) return;
            if (tickcount % 50 == 0)
            {
                bool side = fillcount % 2 == 0;
                execute.SimBroker.sendOrder(new MarketOrder(t.symbol, side, 100));
            }
        }

        BarListTracker bt = new BarListTracker();
        [Test]
        public void BarPerformance()
        {
            HistSim h = new HistSim(Environment.CurrentDirectory + "\\");
            h.GotTick += new TradeLink.API.TickDelegate(h_GotTick);

            h.Initialize();

            tickcount = 0;
            lasttime = 0;

            Assert.AreEqual(0, lasttime);
            Assert.Greater(h.TicksPresent, 0);
            if (Environment.ProcessorCount == 1) EXPECTBARS *= 2.5;

            DateTime start = DateTime.Now;

            h.PlayTo(HistSim.ENDSIM);

            double time = DateTime.Now.Subtract(start).TotalSeconds;
            Assert.GreaterOrEqual(tickcount, 50000);
            Assert.AreEqual(3, bt.SymbolCount);
            Assert.LessOrEqual(time, EXPECTBARS);
            Console.WriteLine("BarList runtime: " + time.ToString("N2") + "sec, versus: " + EXPECTBARS + "sec expected.");
            Console.WriteLine("BarList " + ((double)tickcount / time).ToString("N0") + " ticks/sec");
        }

        void h_GotTick(TradeLink.API.Tick t)
        {
            tickcount++;
            bt.newTick(t);
        }
    }
}

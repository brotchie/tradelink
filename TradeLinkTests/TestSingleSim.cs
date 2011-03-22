using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLink.API;
using TradeLink.Common;

namespace TestTradeLink
{
    [TestFixture]
    public class TestSingleSim
    {
        public TestSingleSim() { }

        double EXPECTRAW = .6;
        double EXPECTEX = .8;
        double EXPECTBARS = .6;

        [Test]
        public void RawSingle()
        {
            SingleSimImpl h = new SingleSimImpl(new string[] { "FTI20070926.TIK" });
            rawbase("rawsingle",1,h);
            // tick count is = 42610 (FTI) + 5001 (SPX) + 8041 (ABN)
            Assert.Greater(tickcount, 40000);
        }


        [Test]
        public void RawPerformance() 
        { 
            SingleSimImpl h = new SingleSimImpl(Environment.CurrentDirectory);
            rawbase("raw performance multi",3,h);
            // tick count is = 42610 (FTI) + 5001 (SPX) + 8041 (ABN)
            Assert.AreEqual(42610 + 4991 + 8041, tickcount);
            // check running time
            Assert.LessOrEqual(h.RunTimeSec, EXPECTRAW, "may fail on slow machines");

            // last time is 1649 on SPX
            Assert.AreEqual(20080318155843, lasttime);
        }
        public void rawbase(string name,int symcount, SingleSimImpl sim)
        {
            rt.d(name.ToUpper());
            SingleSimImpl h = sim;
            h.GotTick += new TradeLink.API.TickDelegate(raw_GotTick);
            h.GotDebug += new DebugDelegate(h_GotDebug);
            tickcount = 0;
            syms.Clear();
            lasttime = 0;
                Assert.AreEqual(0, tickcount);
                Assert.AreEqual(0, syms.Count);
                Assert.AreEqual(0, lasttime);


            if (Environment.ProcessorCount == 1) EXPECTRAW *= 2.5;

            DateTime start = DateTime.Now;

            h.PlayTo(SingleSimImpl.ENDSIM);



            // printout simulation runtime
            
            rt.d("Ticks received: " + tickcount + " sent: " + h.TicksProcessed + " estimate: " + h.TicksPresent);
            rt.d("Raw runtime: " + h.RunTimeSec.ToString("N2") + "sec, versus: " + EXPECTRAW + "sec expected.");
            rt.d("Raw speed: " + h.RunTimeTicksPerSec.ToString("N0") + " ticks/sec");


            // make sure ticks arrived in order
            Assert.IsTrue(h.isTickPlaybackOrdered, "Tick arrived out-of-order.");
            // ensure got expected number of symbols
            Assert.AreEqual(symcount, syms.Count);
            // variance from approximate count should be less than 1%
            Assert.Less((tickcount - h.TicksPresent) / h.TicksPresent, .01);
            // actual count should equal simulation count
            Assert.AreEqual(h.TicksProcessed, tickcount);

            h.Stop();
            rt.d(name.ToUpper());
        }

        [Test]
        public void RawPerformanceWithLoad()
        {
            System.ComponentModel.BackgroundWorker bw = new System.ComponentModel.BackgroundWorker();
            bw.DoWork += new System.ComponentModel.DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            SingleSimImpl h = new SingleSimImpl(Environment.CurrentDirectory);
            rawbase("raw performance multi w/load", 3, h);
            // tick count is = 42610 (FTI) + 5001 (SPX) + 8041 (ABN)
            Assert.AreEqual(42610 + 4991 + 8041, tickcount);
            // check running time
            Assert.LessOrEqual(h.RunTimeSec, EXPECTRAW, "may fail on slow machines");

            // last time is 1649 on SPX
            Assert.AreEqual(20080318155843, lasttime);
            RawPerformance();
            bw.CancelAsync();
            run = false;
            // tick count is = 42610 (FTI) + 5001 (SPX) + 8041 (ABN)
            Assert.AreEqual(42610 + 4991 + 8041, tickcount);
            // check running time
            Assert.LessOrEqual(h.RunTimeSec, EXPECTRAW, "may fail on slow machines");

            // last time is 1649 on SPX
            Assert.AreEqual(20080318155843, lasttime);
        }

        long product = 0;
        int loadcount = 0;
        bool run = true;

        void bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Random r = new Random();
            run = true;
            while (!e.Cancel && run)
            {
                product = r.Next() * r.Next() + loadcount++;
                if (loadcount % 1000 == 0)
                    System.Threading.Thread.Sleep(10);
            }
            rt.d("load simulation completed. calculations performed: "+loadcount);
            
        }

        void h_GotDebug(string msg)
        {
            rt.d(msg);
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
            bool viol = t.datetime < lasttime;
            GOODTIME &= !viol;
            lasttime = t.datetime;
        }

        int fillcount = 0;
        int desiredfills = 1000;
        SingleSimImpl h;
        //[Test]
        public void ExecutionPerformance()
        {
            System.Threading.Thread.Sleep(100);
            h = new SingleSimImpl(Environment.CurrentDirectory);
            h.Initialize();
            h.GotTick += new TradeLink.API.TickDelegate(execute_GotTick);
            h.SimBroker.GotFill += new TradeLink.API.FillDelegate(SimBroker_GotFill);

            tickcount = 0;
            lasttime = 0;

            Assert.AreEqual(0, tickcount);
            Assert.AreEqual(0, syms.Count);
            Assert.AreEqual(0, lasttime);
            Assert.Greater(h.TicksPresent, 0);
            if (Environment.ProcessorCount == 1) EXPECTEX *= 2.5;

            DateTime start = DateTime.Now;

            h.PlayTo(SingleSimImpl.ENDSIM);

            double time = DateTime.Now.Subtract(start).TotalSeconds;

            rt.d("Execution runtime: " + time.ToString("N2") + "sec, versus: " + EXPECTEX + "sec expected.");
            rt.d("Execution " + ((double)tickcount / time).ToString("N0") + " ticks/sec.  " + ((double)fillcount / time).ToString("N0") + " fills/sec");

            Assert.AreEqual(desiredfills, fillcount);
            Assert.LessOrEqual(time, EXPECTEX);
            h.Stop();
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
                h.SimBroker.SendOrderStatus(new MarketOrder(t.symbol, side, 100));
            }
        }

        BarListTracker bt = new BarListTracker();

        //[Test]
        public void BarPerformance()
        {
            SingleSimImpl h = new SingleSimImpl(Environment.CurrentDirectory);
            h.GotTick += new TradeLink.API.TickDelegate(h_GotTick);

            h.Initialize();

            tickcount = 0;
            lasttime = 0;

            Assert.AreEqual(0, lasttime);
            Assert.Greater(h.TicksPresent, 0);
            if (Environment.ProcessorCount == 1) EXPECTBARS *= 2.5;

            DateTime start = DateTime.Now;

            h.PlayTo(SingleSimImpl.ENDSIM);

            double time = DateTime.Now.Subtract(start).TotalSeconds;
            h.Stop();
            Assert.GreaterOrEqual(tickcount, 50000);
            Assert.AreEqual(3, bt.SymbolCount);
            Assert.LessOrEqual(time, EXPECTBARS);
            rt.d("BarList runtime: " + time.ToString("N2") + "sec, versus: " + EXPECTBARS + "sec expected.");
            rt.d("BarList " + ((double)tickcount / time).ToString("N0") + " ticks/sec");
        }

        void h_GotTick(TradeLink.API.Tick t)
        {
            tickcount++;
            bt.newTick(t);
        }

        [SetUp]
        public void start()
        {
            run = true;
        }

        [TearDown]
        public void stop()
        {
            run = false;
        }
    }
}

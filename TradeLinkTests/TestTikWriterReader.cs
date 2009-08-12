using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLink.Common;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TestTikWriterReader
    {
        public TestTikWriterReader()
        {
        }

        const int DATE = 20090811;
        const string FILE = "TST20090811.tik";
        const string SYM = "TST";
        const int TICKCOUNT = 100000;
        string PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        System.Collections.Generic.List<Tick> readdata = new List<Tick>(TICKCOUNT);

        [Test]
        public void TikTypes()
        {
            System.Collections.Generic.List<Tick> data = new List<Tick>();
            // bid
            data.Add(TickImpl.NewBid(SYM, 10, 100));
            // ask
            data.Add(TickImpl.NewAsk(SYM, 11, 200));
            // full quote
            data.Add(TickImpl.NewQuote(SYM, DATE,93000,00,10, 11,300,300,"NYSE","ARCA"));
            // trade
            data.Add(TickImpl.NewTrade(SYM, 10, 400));
            // full tick
            Tick full = TickImpl.Copy((TickImpl)data[3], (TickImpl)data[2]);
            data.Add(full);

            // write and read the data
            writeandread(data.ToArray());

            //verify the count
            Assert.AreEqual(data.Count, readdata.Count);

            // verify the data
            bool equal = true;
            System.Text.StringBuilder sb = new StringBuilder(string.Empty);
            for (int i = 0; i < data.Count; i++)
            {
                bool start = equal;
                equal &= data[i].bid == readdata[i].bid;
                equal &= data[i].bs== readdata[i].bs;
                equal &= data[i].be == readdata[i].be;
                equal &= data[i].ask== readdata[i].ask;
                equal &= data[i].os== readdata[i].os;
                equal &= data[i].oe == readdata[i].oe;
                equal &= data[i].trade== readdata[i].trade;
                equal &= data[i].size == readdata[i].size;
                equal &= data[i].ex == readdata[i].ex;
                equal &= data[i].depth == readdata[i].depth;
                if (equal!=start)
                    sb.Append(i+" ");
            }
            Assert.IsTrue(equal, "bad ticks: " + sb.ToString()+data[0].ToString()+readdata[0].ToString());


        }

        [Test]
        public void WriteandRead()
        {
            // get some data to test with
            Tick[] data = 
                TradeLink.Research.RandomTicks.GenerateSymbol(SYM, TICKCOUNT);
            // apply date and time to ticks
            for (int i = 0; i < TICKCOUNT; i++)
            {
                data[i].date = DATE;
                data[i].time = Util.DT2FT(DateTime.Now);
            }
            // write and read data, clocking time
            double elapms = writeandread(data,true);
            // verify length
            Assert.AreEqual(data.Length, readdata.Count);
            // verify content
            bool equal = true;
            for (int i = 0; i < TICKCOUNT; i++)
                equal &= data[i].trade == readdata[i].trade;
            Assert.IsTrue(equal, "read/write mismatch on TIK data.");
            // verify performance
            double rate = TICKCOUNT/(elapms/1000);
            Assert.GreaterOrEqual(rate, 90000);


        }

        double writeandread(Tick[] data) { return writeandread(data, false); }
        double writeandread(Tick[] data, bool printperf)
        {            
            // clear out the read buffer
            readdata.Clear();
            // remove existing file
            removefile();
            // keep track of time
            double elapms;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            // write new file from data
            TikWriter tw = new TikWriter(PATH,SYM,DATE);
            sw.Start();
            foreach (Tick k in data)
                tw.newTick(k);
            sw.Stop();
            tw.Close();
            elapms = (double)sw.ElapsedMilliseconds;
            if (printperf)
                Console.WriteLine("write speed (ticks/sec): " + (data.Length / (elapms / 1000)).ToString("n0"));

            // read file back in from file
            TikReader tr = new TikReader(PATH+"//"+FILE);
            tr.gotTick += new TickDelegate(tr_gotTick);
            sw.Reset();
            sw.Start();
            while (tr.NextTick()) ;
            sw.Stop();
            tr.Close();
            elapms = (double)sw.ElapsedMilliseconds;
            if (printperf)
                Console.WriteLine("read speed (ticks/sec): " + (data.Length/(elapms/1000)).ToString("n0"));
            
            // remove test file
            removefile();
            
            return elapms;
        }

        void tr_gotTick(Tick t)
        {
            readdata.Add(t);
        }

        static void removefile()
        {
            try
            {
                System.IO.File.Delete(FILE);
            }
            catch (Exception ex) 
            { 
                System.Diagnostics.Debug.WriteLine(ex.Message); 
            }
        }

    }
}
    
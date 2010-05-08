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

        const int TICKCOUNT = 100000;
        string PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        System.Collections.Generic.List<Tick> readdata = new List<Tick>(TICKCOUNT);

        [Test]
        public void TikTypes()
        {
            // get symbol
            string SYM = TradeLink.Research.RandomSymbol.GetSymbol((int)DateTime.Now.Ticks);
            // prepare data
            System.Collections.Generic.List<Tick> data = new List<Tick>();
            // bid
            data.Add(TickImpl.NewBid(SYM, 10, 100));
            // ask
            data.Add(TickImpl.NewAsk(SYM, 11, 200));
            // full quote
            data.Add(TickImpl.NewQuote(SYM, DATE,93000,10, 11,300,300,"NYSE","ARCA"));
            // trade
            data.Add(TickImpl.NewTrade(SYM, DATE,93100,10, 400,"NYSE"));
            // full tick
            Tick full = TickImpl.Copy((TickImpl)data[2], (TickImpl)data[3]);
            data.Add(full);

            // write and read the data
            writeandread(data.ToArray(),DATE,false);

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
            // get symbol
            string SYM = TradeLink.Research.RandomSymbol.GetSymbol((int)DateTime.Now.Ticks);

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
            double elapms = writeandread(data,0,true);
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

        [Test]
        public void FilenameTest()
        {
            // get symbol
            string SYM = "EUR/USD";
            const int SHORTTEST = 1000;
            // get some data to test with
            Tick[] data =
                TradeLink.Research.RandomTicks.GenerateSymbol(SYM, SHORTTEST);
            // apply date and time to ticks
            for (int i = 0; i < SHORTTEST; i++)
            {
                data[i].date = DATE;
                data[i].time = Util.DT2FT(DateTime.Now);
            }
            // write and read data, clocking time
            double elapms = writeandread(data, 0, true);
            // verify length
            Assert.AreEqual(data.Length, readdata.Count);
            // verify content
            bool equal = true;
            for (int i = 0; i < SHORTTEST; i++)
                equal &= data[i].trade == readdata[i].trade;
            Assert.IsTrue(equal, "read/write mismatch on TIK data.");


        }

        [Test]
        public void FilenameTestOption()
        {
            // get symbol
            string SYM = "MSTR 201007 CALL 85.0000 OPT";
            const int SHORTTEST = 1000;
            // get some data to test with
            Tick[] data =
                TradeLink.Research.RandomTicks.GenerateSymbol(SYM, SHORTTEST);
            // apply date and time to ticks
            for (int i = 0; i < SHORTTEST; i++)
            {
                data[i].date = DATE;
                data[i].time = Util.DT2FT(DateTime.Now);
            }
            // write and read data, clocking time
            double elapms = writeandread(data, 0, true);
            // verify length
            Assert.AreEqual(data.Length, readdata.Count);
            // verify content
            bool equal = true;
            for (int i = 0; i < SHORTTEST; i++)
                equal &= data[i].trade == readdata[i].trade;
            Assert.IsTrue(equal, "read/write mismatch on TIK data.");


        }

        double writeandread(Tick[] data, int date, bool printperf)
        {            
            // clear out the read buffer
            readdata.Clear();
            // keep track of time
            double elapms;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            // write new file from data
            TikWriter tw = new TikWriter(PATH,data[0].symbol,date==0 ? data[0].date: date);
            string FILE = tw.Filepath;
            sw.Start();
            foreach (Tick k in data)
                tw.newTick(k);
            sw.Stop();
            tw.Close();
            elapms = (double)sw.ElapsedMilliseconds;
            if (printperf)
                Console.WriteLine("write speed (ticks/sec): " + (data.Length / (elapms / 1000)).ToString("n0"));

            // read file back in from file
            TikReader tr = new TikReader(FILE);
            tr.gotTick += new TickDelegate(tr_gotTick);
            sw.Reset();
            sw.Start();
            while (tr.NextTick()) ;
            sw.Stop();
            tr.Close();
            elapms = (double)sw.ElapsedMilliseconds;
            if (printperf)
                Console.WriteLine("read speed (ticks/sec): " + (data.Length/(elapms/1000)).ToString("n0"));

            // remove file
            removefile(FILE);
            
            return elapms;
        }

        [Test]
        public void TikFromTick()
        {
            // get symbol
            string SYM = TradeLink.Research.RandomSymbol.GetSymbol((int)DateTime.Now.Ticks);

            // get some data to test with
            Tick[] data =
                TradeLink.Research.RandomTicks.GenerateSymbol(SYM, 10);

            writeandread(data, DATE,false);

            Assert.AreEqual(data.Length, readdata.Count);

        }



        void tr_gotTick(Tick t)
        {
            readdata.Add(t);
        }

        public static bool removefile(string FILE)
        {
            if (!System.IO.File.Exists(FILE)) return true;
            try
            {
                System.IO.File.Delete(FILE);
                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
        }

    }
}
    
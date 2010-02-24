using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLink.API;
using TradeLink.Research;
using TradeLink.Common;

namespace TestTradeLink
{
    [TestFixture]
    public class TestTickArchiver
    {

        public TestTickArchiver()
        {

        }

        string PATH = Environment.CurrentDirectory;
        const string SYM = "TST";
        const int MAXTICKS = 1000;
        const int DATE = 20090815;
        
        [Test]
        public void CreateRead()
        {
            readdata.Clear();
            readdata2.Clear();
            FILE = TikWriter.SafeFilename(SYM, PATH, DATE);
            TestTikWriterReader.removefile(FILE);
            {
                Tick[] data = RandomTicks.GenerateSymbol(SYM, MAXTICKS);

                TickArchiver ta = new TickArchiver(Environment.CurrentDirectory);
                for (int i = 0; i < data.Length; i++)
                {
                    data[i].date = DATE;
                    data[i].time = Util.DT2FT(DateTime.Now);
                    ta.newTick(data[i]);
                }
                ta.Stop();
                
                // read file back in from file
                TikReader tr = new TikReader(FILE);
                tr.gotTick += new TickDelegate(tr_gotTick);
                while (tr.NextTick()) ;

                // verify length
                Assert.AreEqual(data.Length, readdata.Count);
                // verify content
                bool equal = true;
                for (int i = 0; i < MAXTICKS; i++)
                    equal &= data[i].trade == readdata[i].trade;
                tr.Close();

                readdata.Clear();
                Assert.IsTrue(equal, "ticks did not matched archive.");
                TestTikWriterReader.removefile(FILE);
            }
            

            


        }

        [Test]
        public void Multiday()
        {
            readdata.Clear();
            readdata2.Clear();
            int d = 20100223;
            int t = 235900;
            int t1 = 0;
            const decimal p = 50;
            int s = 100;

            string FILE1 = TikWriter.SafeFilename(SYM, PATH, d);
            TestTikWriterReader.removefile(FILE1);
            string FILE2 = TikWriter.SafeFilename(SYM, PATH, d+1);
            TestTikWriterReader.removefile(FILE2);


            Tick[] data = new Tick[] 
            {
                TickImpl.NewTrade(SYM,d,t++,p,s,string.Empty),
                TickImpl.NewTrade(SYM,d,t++,p,s,string.Empty),
                TickImpl.NewTrade(SYM,d,t++,p,s,string.Empty),
                TickImpl.NewTrade(SYM,d,t++,p,s,string.Empty),
                TickImpl.NewTrade(SYM,d,t++,p,s,string.Empty),
                // day two
                TickImpl.NewTrade(SYM,++d,t1++,p,s,string.Empty),
                TickImpl.NewTrade(SYM,d,t1++,p,s,string.Empty),
                TickImpl.NewTrade(SYM,d,t1++,p,s,string.Empty),
                TickImpl.NewTrade(SYM,d,t1++,p,s,string.Empty),
                TickImpl.NewTrade(SYM,d,t1++,p,s,string.Empty),
            };


            TickArchiver ta = new TickArchiver(Environment.CurrentDirectory);
            for (int i = 0; i < data.Length; i++)
            {
                ta.newTick(data[i]);
            }
            ta.Stop();

            // read file back in from files
            if (System.IO.File.Exists(FILE1))
            {
                TikReader tr = new TikReader(FILE1);
                tr.gotTick += new TickDelegate(tr_gotTick);
                while (tr.NextTick()) ;
                tr.Close();
            }
            
            if (System.IO.File.Exists(FILE2))
            {
                TikReader tr2 = new TikReader(FILE2);
                tr2.gotTick += new TickDelegate(tr2_gotTick);
                while (tr2.NextTick()) ;
                tr2.Close();
            }

            // verify length
            Assert.AreEqual(5,readdata2.Count);
            Assert.AreEqual(5, readdata.Count);

            TestTikWriterReader.removefile(FILE1);
            TestTikWriterReader.removefile(FILE2);
        }

        void tr2_gotTick(Tick t)
        {
            readdata2.Add(t);
        }

        List<Tick> readdata = new List<Tick>();
        List<Tick> readdata2 = new List<Tick>();
        string FILE;


        void tr_gotTick(Tick t)
        {
            readdata.Add(t);
        }
    }
}

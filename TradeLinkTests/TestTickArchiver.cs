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

        List<Tick> readdata = new List<Tick>();
        string FILE;


        void tr_gotTick(Tick t)
        {
            readdata.Add(t);
        }
    }
}

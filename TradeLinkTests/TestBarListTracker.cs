using System;
using System.Collections.Generic;
using TradeLink.Common;
using NUnit.Framework;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TestBarListTracker
    {
        public TestBarListTracker() { }

        [Test]
        public void Basics()
        {
            BarListTracker blt = new BarListTracker();
            blt.GotNewBar+=new SymBarIntervalDelegate(blt_GotNewBar);

            Tick [] tape = TestBarList.SampleData();
            // get second tape and change symbol
            Tick[] tape2 = TestBarList.SampleData();
            for (int i = 0; i<tape2.Length; i++)
                tape2[i].symbol = "TST2";

            // add ticks from both tape to tracker
            for (int i = 0; i<tape.Length; i++)
            {
                blt.newTick(tape[i]);
                blt.newTick(tape2[i]);
            }

            //make sure we got two symbols as bar events
            Assert.AreEqual(2, syms.Count);
            // make sure our symbols matched barlist count
            Assert.AreEqual(blt.SymbolCount, syms.Count);

            int secondcount = 0;
            string symstring = string.Empty;
            foreach (string sym in blt)
            {
                secondcount++;
                symstring += sym;
            }

            // make sure enumeration equals symbol count
            Assert.AreEqual(syms.Count, secondcount);
            // make sure symbols are there
            Assert.IsTrue(symstring.Contains("TST") && symstring.Contains("TST2"));

        }

        List<string> syms = new List<string>();
        void  blt_GotNewBar(string symbol, int interval)
        {
            if (!syms.Contains(symbol))
                syms.Add(symbol);
        }


    }
}

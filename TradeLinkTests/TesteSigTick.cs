using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using NUnit.Framework;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TesteSigTick
    {

        [Test]
        public void Hours()
        {

            System.IO.StreamReader sr = new System.IO.StreamReader("TestWAG.txt");
            sr.ReadLine();
            sr.ReadLine();

            BarListImpl bl = new BarListImpl(BarInterval.Hour, "WAG");
            Tick k = new TickImpl();
            int tickcount = 0;
            while (!sr.EndOfStream) 
            {
                k = eSigTick.FromStream(bl.Symbol, sr);
                if (tickcount == 0)
                {
                    Assert.IsTrue(k.isValid);
                    Assert.AreEqual(20070926041502, k.datetime);
                    Assert.AreEqual(20070926, k.date);
                    Assert.AreEqual(041502, k.time);
                    Assert.AreEqual(43.81m, k.bid);
                    Assert.AreEqual(51.2m, k.ask);
                    Assert.AreEqual(1, k.bs);
                    Assert.AreEqual(1, k.os);
                    Assert.IsTrue(k.be.Contains("PSE"));
                    Assert.IsTrue(k.oe.Contains("PSE"));
                }
                tickcount++;
                bl.newTick(k);
            }
            // hour is what we asked for
            Assert.AreEqual(BarInterval.Hour,bl.DefaultInterval);
            // there are 4 trades on hour intervals, 6/7/8/9
            Assert.AreEqual(4,bl.Count);

        }
    }
}

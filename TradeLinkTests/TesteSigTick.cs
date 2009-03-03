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

            BarListImpl bl = new BarListImpl(BarInterval.Hour, "WAG");
            Tick k = new TickImpl();

            while (!sr.EndOfStream) 
            {
                k = eSigTick.FromStream("WAG", sr);
                bl.newTick(k);
            }
            // hour is what we asked for
            Assert.That(bl.Int == BarInterval.Hour, bl.Int.ToString());
            // there are 4 trades on hour intervals, 6/7/8/9
            Assert.AreEqual(4,bl.Count);

        }
    }
}

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
            string contents = sr.ReadToEnd();

            string[] file = contents.Split(Environment.NewLine.ToCharArray());

            BarListImpl bl = new BarListImpl(BarInterval.Hour, "WAG");
            int count = 0;
            eSigTick e = new eSigTick();
            for (int i = 0; i<file.Length; i++)
            {
                if (i < 3) continue;
                string line = file[i];
                e.Load(line);
                e.symbol = "WAG";
                if (bl.newTick(e))
                    count++;
            }

            Assert.That(bl.Int == BarInterval.Hour, bl.Int.ToString());
            Assert.That(bl.Has(4, BarInterval.Hour), bl.Count.ToString());
            Assert.That(bl.Count == count, bl.Count.ToString());

        }
    }
}

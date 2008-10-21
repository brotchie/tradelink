using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
{
    [TestFixture]
    public class TesteSigTick
    {

        [Test]
        public void Hours()
        {

            string[] file = TestTradeLib.Properties.Resources.TestWAG.Split(Environment.NewLine.ToCharArray());

            BarList bl = new BarList(BarInterval.Hour, "WAG");
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

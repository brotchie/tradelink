using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
{
    [TestFixture]
    public class TestTickFileFilter
    {
        public TestTickFileFilter() { }
        string[] filenames = { 
            "GM20070601.EPF",
            "SPX20070601.IDX",
            "LVS20080101.EPF",
            "GOOG20070926.EPF",
        };

        [Test]
        public void Basics()
        {
            TickFileFilter tff = new TickFileFilter(new string[] { "GM" });
            string[] result = tff.Allows(filenames);
            Assert.That(result.Length == 1);
            Assert.That(result[0] == filenames[0]);
            tff = new TickFileFilter();
            tff.DateFilter(20070000, DateMatchType.Year);
            result = tff.Allows(filenames);
            Assert.That(result.Length == 3);
            Assert.That(result[2] == filenames[3]);
        }

    }
}

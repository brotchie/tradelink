using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using NUnit.Framework;

namespace TestTradeLink
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
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(filenames[3], result[2]);
        }

        [Test]
        public void SerializeDeserialize()
        {
            TickFileFilter tff = new TickFileFilter(new string[] { "IBM", "MHS", "T" });
            tff.DateFilter(20070000, DateMatchType.Year);
            string msg = TickFileFilter.Serialize(tff);

            TickFileFilter f2 = TickFileFilter.Deserialize(msg);

            string msg2 = TickFileFilter.Serialize(f2);

            Assert.AreEqual(msg, msg2);

        }

    }
}

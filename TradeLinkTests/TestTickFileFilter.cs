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
            "GM20090101.EPF",
        };

        [Test]
        public void Basics()
        {
            // build a one symbol filter
            TickFileFilter tff = new TickFileFilter(new string[] { "GM" });
            // get results from above data files
            string[] result = tff.Allows(filenames);
            // make sure both files for this symbol match
            Assert.AreEqual(2,result.Length);
            // make sure the actual file names are the same
            Assert.AreEqual(result[0],filenames[0]);
            Assert.AreEqual(result[1], filenames[4]);
            // build a new filter
            tff = new TickFileFilter();
            // request all matching files for a given year
            tff.DateFilter(20070000, DateMatchType.Year);
            tff.isDateMatchUnion = true;
            tff.isSymbolDateMatchUnion = true;
            // do the match
            result = tff.Allows(filenames);
            // make sure we found 3 files from this year
            Assert.AreEqual(3, result.Length);
            // make sure the filename is the same
            Assert.AreEqual(filenames[3], result[2]);
        }

        [Test]
        public void AndTest()
        {
            // build a filter with two stocks
            TickFileFilter tff = new TickFileFilter(new string[] { "GM","SPX" });
            // add date file for year 
            tff.DateFilter(20070000, DateMatchType.Year);
            // add another date filter for month
            tff.DateFilter(600, DateMatchType.Month);
            // set DateFilter to AND/intersection
            tff.isDateMatchUnion = false;
            // make sure three stocks match
            string[] result = tff.Allows(filenames);
            Assert.AreEqual(3, result.Length);
            // set more exclusive filter
            tff.isSymbolDateMatchUnion = false;
            // make sure two stocks match
            result = tff.Allows(filenames);
            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void SerializeDeserialize()
        {
            TickFileFilter tff = new TickFileFilter(new string[] { "IBM", "MHS", "T" });
            tff.DateFilter(20070000, DateMatchType.Year);
            tff.isDateMatchUnion = false;
            tff.isSymbolDateMatchUnion = false;
            string msg = TickFileFilter.Serialize(tff);

            TickFileFilter f2 = TickFileFilter.Deserialize(msg);

            string msg2 = TickFileFilter.Serialize(f2);

            Assert.AreEqual(msg, msg2);
            Assert.AreEqual(tff.isDateMatchUnion, f2.isDateMatchUnion);
            Assert.AreEqual(tff.isSymbolDateMatchUnion, f2.isDateMatchUnion);

        }

    }
}

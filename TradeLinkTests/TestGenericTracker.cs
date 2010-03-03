using System;
using NUnit.Framework;
using TradeLink.Common;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TestGenericTracker
    {
        int newtxt = 0;
        [Test]
        public void Basics()
        {
            // reset count
            newtxt = 0;
            // track something
            GenericTracker<object> gt = new GenericTracker<object>();
            // count new items
            gt.NewTxt += new TextIdxDelegate(gt_NewTxt);
            // get some symbols
            string[] syms = new string[] { "IBM", "LVS", "IBM", "WAG", "GOOG" };
            // add them
            foreach (string sym in syms)
                gt.addindex(sym, sym == "IBM" ? null : new object());
            // ensure we have them
            Assert.AreEqual(4, newtxt);
            Assert.AreNotEqual(gt.Count, syms.Length);
            // test fetching by label
            Assert.IsNull(gt["IBM"]);
            Assert.IsNotNull(gt["GOOG"]);
            Assert.AreEqual(0, gt.getindex("IBM"));
            // get label from index
            Assert.AreEqual("GOOG", gt.getlabel(3));
        }

        void gt_NewTxt(string txt, int idx)
        {
            newtxt++;
        }
    }
}

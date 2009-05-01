using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;
using NUnit.Framework;

namespace TestTradeLink
{
    [TestFixture]
    public class TestImbalance
    {
        public TestImbalance() {}

        [Test]
        public void SerializeDeserilize()
        {
            // setup imbalance
            const string s = "IBM";
            const string e = "NYSE";
            const int x = 500000;
            const int t = 1550;
            const int p = 1540;
            const int px = 400000;
            Imbalance i = new ImbalanceImpl(s, e, x, t, px, p,999);

            // verify it's valid
            Assert.IsTrue(i.isValid);

            // serialize it
            string msg = ImbalanceImpl.Serialize(i);
            // deserialize it somewhere else
            Imbalance ni = ImbalanceImpl.Deserialize(msg);

            // make sure it's valid
            Assert.IsTrue(ni.isValid);

            // verify it's the same
            Assert.AreEqual(i.Symbol, ni.Symbol);
            Assert.AreEqual(i.Exchange, ni.Exchange);
            Assert.AreEqual(i.PrevImbalance, ni.PrevImbalance);
            Assert.AreEqual(i.PrevTime, ni.PrevTime);
            Assert.AreEqual(i.ThisImbalance, ni.ThisImbalance);
            Assert.AreEqual(i.ThisTime, ni.ThisTime);
            Assert.AreEqual(i.InfoImbalance, ni.InfoImbalance);
        }



    }
}

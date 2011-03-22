using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;
using NUnit.Framework;

namespace TestTradeLink
{
    [TestFixture]
    public class TestIndication
    {
        public TestIndication() {}

        [Test]
        public void SerializeDeserilize()
        {
            // setup indication
            const string s = "IBM";
            const string e = "NYSE";
            const int t = 092500;
            const decimal h = 128.08m;
            const decimal l = 126.25m;
            const int v = 1;

            Indication i = new IndicationImpl(s, e, t, v, h, l);

            // verify it's valid
            Assert.IsTrue(i.isValid);

            // serialize it
            string msg = IndicationImpl.Serialize(i);
            // deserialize it somewhere else
            Indication ni = IndicationImpl.Deserialize(msg);

            // make sure it's valid
            Assert.IsTrue(ni.isValid);

            // verify it's the same
            Assert.AreEqual(i.Symbol, ni.Symbol);
            Assert.AreEqual(i.Exchange, ni.Exchange);
            Assert.AreEqual(i.Time, ni.Time);
            Assert.AreEqual(i.High, ni.High);
            Assert.AreEqual(i.Low, ni.Low);
        }
    }

    [TestFixture]
    public class TestHaltResume
    {
        public TestHaltResume() { }

        [Test]
        public void SerializeDeserilize()
        {
            // setup halt-resume
            const string s = "IBM";
            const string e = "NYSE";
            const int t = 092500;
            const string a = "1025";
            const string r = "540";

            HaltResume i = new HaltResumeImpl(s, e, t, a, r);

            // serialize it
            string msg = HaltResumeImpl.Serialize(i);
            // deserialize it somewhere else
            HaltResume ni = HaltResumeImpl.Deserialize(msg);

            // verify it's the same
            Assert.AreEqual(i.Symbol, ni.Symbol);
            Assert.AreEqual(i.Exchange, ni.Exchange);
            Assert.AreEqual(i.Time, ni.Time);
            Assert.AreEqual(i.Status, ni.Status);
            Assert.AreEqual(i.Reason, ni.Reason);
        }
    }
}

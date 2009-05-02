using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using NUnit.Framework;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TestSecurity 
    {
        public TestSecurity() { }

        [Test]
        public void Parsing()
        {
            // tests to parse and generate user-supplied security specifiers
            SecurityImpl nyse = new SecurityImpl("LVS");
            string p = nyse.ToString();

            SecurityImpl t = SecurityImpl.Parse(p);
            Assert.That(t.Symbol == nyse.Symbol, t.Symbol);
            Assert.That(!t.hasDest, t.DestEx);
            Assert.That(t.Type == nyse.Type, t.Type.ToString());

            SecurityImpl crude = SecurityImpl.Parse("CLV8 FUT GLOBEX");
            Assert.That(crude.Symbol == "CLV8", crude.Symbol);
            Assert.That(crude.hasDest, crude.DestEx);
            Assert.That(crude.Type == SecurityType.FUT, crude.Type.ToString());
            SecurityImpl goog = SecurityImpl.Parse("GOOG");
            Assert.AreEqual("GOOG", goog.FullName);


            
        }
    }
}

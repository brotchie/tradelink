using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
{
    [TestFixture]
    public class TestSecurity 
    {
        public TestSecurity() { }

        [Test]
        public void Parsing()
        {
            // tests to parse and generate user-supplied security specifiers
            Security nyse = new Security("LVS");
            string p = nyse.ToString();

            Security t = Security.Parse(p);
            Assert.That(t.Symbol == nyse.Symbol, t.Symbol);
            Assert.That(!t.hasDest, t.DestEx);
            Assert.That(t.Type == nyse.Type, t.Type.ToString());

            Security crude = Security.Parse("CLV8 GLOBEX FUT");
            Assert.That(crude.Symbol == "CLV8", crude.Symbol);
            Assert.That(crude.hasDest, crude.DestEx);
            Assert.That(crude.Type == SecurityType.FUT, crude.Type.ToString());

            
        }
    }
}

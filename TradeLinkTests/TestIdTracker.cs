using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using NUnit.Framework;

namespace TestTradeLink
{
    [TestFixture]
    public class TestIdTracker
    {
        
        public TestIdTracker()
        {
        }

        IdTracker it = new IdTracker();

        [Test]
        public void Basics()
        {
            // make sure consequetive assignments don't match
            Assert.AreNotEqual(it.AssignId, it.AssignId);
            // make sure checks of ids do match
            Assert.AreEqual(it.NextId, it.NextId);

        }
    }
}

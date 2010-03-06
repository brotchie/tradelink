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

        IdTracker it;

        [Test]
        public void Basics()
        {
            // create new tracker
            it = new IdTracker();
            // make sure consequetive assignments don't match
            Assert.AreNotEqual(it.AssignId, it.AssignId);
            // make sure checks of ids do match
            Assert.AreEqual(it.NextId, it.NextId);
        }

        [Test]
        public void VirtualIds()
        {
            const int BOX1ID = 22;
            const int BOX2ID = 23;
            const int ATTEMPTS = 1000;
            // store ids
            List<long> b1 = new List<long>(ATTEMPTS);
            List<long> b2 = new List<long>(ATTEMPTS);
            // create a new tracker with virtual id represent my book
            it = new IdTracker(BOX1ID);
            // create a second tracker with unique id
            IdTracker it2 = new IdTracker(BOX2ID);
            // make sure consequetive assignments don't match
            Assert.AreNotEqual(it.AssignId, it.AssignId);
            // make sure checks of ids do match
            Assert.AreEqual(it.NextId, it.NextId);
            // assign 1000 ids to make sure we overlap with tracker2 initial id
            for (int i = 0; i < ATTEMPTS; i++)
            {
                b1.Add(it.AssignId);
            }
            // assign 1000 ids to make sure we overlap with tracker1 assignments
            for (int i = 0; i < ATTEMPTS; i++)
            {
                b2.Add(it2.AssignId);
            }
            bool overlap = false;
            // test for id overlap
            for (int i = 0; i < ATTEMPTS; i++)
                for (int j = 0; j < ATTEMPTS; j++)
                    overlap |= (b1[i] == b2[j]);
            Assert.IsFalse(overlap, "ids overlapped");

        }
    }
}

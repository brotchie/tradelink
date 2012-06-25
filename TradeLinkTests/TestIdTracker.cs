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


        [Test]
        public void NamedIds()
        {
            IdTracker idt = new IdTracker();
            idt.SendDebugEvent+=new TradeLink.API.DebugDelegate(Console.WriteLine);
            const string sym = "TST";
            // get an id
            string id1 = "my market entry";
            string id2 = "my limit entry";
            string id3 = "my exit";
            var entry = new MarketOrder(sym, 100, idt[id1]);
            var lmt = new LimitOrder(sym, 100, 11, idt[id2]);
            var exit = new StopOrder(sym, 100, 9, idt[id3]);

            // verify they are unique
            Assert.AreNotEqual(entry.id, lmt.id, "entry market and limit should not match");
            Assert.AreNotEqual(exit.id, lmt.id, "exit and entry limit should not match");

            // make sure they always return the same value
            var c1 = idt[id1];
            var c2 = idt[id2];
            var c3 = idt[id3];
            Assert.AreEqual(c1, entry.id, id1 + " id changed");
            Assert.AreEqual(c2, lmt.id, id2 + " id changed");
            Assert.AreEqual(c3, exit.id, id3+" id changed");

            // test resetting
            idt[id3] = 0;
            var newc3 = idt[id3];
            Assert.AreNotEqual(newc3, c3, id3 + " did not change after a reset");

            // request it again, should be same

            var newc3compare = idt[id3];
            Assert.AreEqual(newc3, newc3compare, id3 + " changed after a read request");



        }


        [Test]
        public void SymbolNamedIds()
        {
            IdTracker idt = new IdTracker();
            idt.SendDebugEvent += new TradeLink.API.DebugDelegate(Console.WriteLine);
            const string sym = "TST";
            const string sym2 = "BST";
            // get an id
            string id1 = "my market entry";
            var entry = new MarketOrder(sym, 100, idt[sym,id1]);
            var entry2 = new MarketOrder(sym, 100, idt[sym2, id1]);


            // verify they are unique
            Assert.AreNotEqual(entry.id, entry2.id, "different symbol entries should not match");
            

            // make sure they always return the same value
            var c1 = idt[sym,id1];
            var c1_2 = idt[sym2,id1];

            Assert.AreEqual(c1, entry.id, id1 + " id changed");

            // test resetting
            idt[sym,id1] = 0;
            var newc1 = idt[sym,id1];
            Assert.AreNotEqual(newc1, c1, id1 + " did not change after a reset");

            // request it again, should be same

            var newc1compare = idt[sym,id1];
            Assert.AreEqual(newc1, newc1compare, id1 + " changed after a read request");

            // test fetching idname from id
            Assert.AreEqual(idt.idname(sym,id1), idt.getidname(newc1compare));
            Assert.AreEqual(IdTracker.UNKNOWN_IDNAME, idt.getidname(2));



        }
    }
}

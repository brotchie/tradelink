using System;
using NUnit.Framework;
using TradeLink.Common;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TestBasket
    {
        public TestBasket()
        {
        }

        [Test]
        public void BasketBasics()
        {
            BasketImpl mb = new BasketImpl();
            Assert.That(mb != null);
            SecurityImpl i = new SecurityImpl("IBM");
            mb = new BasketImpl(i);
            Assert.That(mb.hasStock);
            mb.Remove(i);
            Assert.That(!mb.hasStock);
            mb.Add(new SecurityImpl("LVS"));
            Assert.That(mb[0].Symbol=="LVS",mb[0].ToString());
            mb.Add(new SecurityImpl("IBM"));
            Assert.That(mb[1].Symbol=="IBM");
            BasketImpl newbasket = new BasketImpl(new SecurityImpl("FDX"));
            newbasket.Add(mb);
            mb.Clear();
            Assert.That(mb.Count==0);
            Assert.That(newbasket.Count==3);
        }

        [Test]
        public void Multiple()
        {
            // setup some symbols
            string[] ab = new string[] { "IBM", "LVS", "T", "GS", "MHS" };
            string[] bb = new string[] { "LVS", "MHS" };
            // create baskets from our symbols
            Basket mb = new BasketImpl( ab);
            Basket rem = new BasketImpl(bb);
            // verify symbol counts of our baskets
            Assert.That(mb.Count == ab.Length);
            Assert.That(rem.Count == bb.Length);
            // remove one basket from another
            mb.Remove(rem);
            // verify count matches
            Assert.That(mb.Count == 3,mb.Count.ToString());

            // add single symbol
            Basket cb = new BasketImpl("GM");
            // add another symbol
            cb.Add("GOOG");
            // verify we have two
            Assert.AreEqual(2, cb.Count);
            // attempt to add dupplicate
            cb.Add("GM");
            // verify we have two
            Assert.AreEqual(2, cb.Count);


        }

        [Test]
        public void Serialization()
        {
            BasketImpl mb = new BasketImpl();
            mb.Add(new SecurityImpl("IBM"));
            BasketImpl compare = BasketImpl.Deserialize(mb.ToString());
            Assert.That(compare.Count == 1);
            mb.Clear();
            compare = BasketImpl.Deserialize(mb.ToString());
            Assert.That(compare.Count==0);

            mb.Clear();
            SecurityImpl longform = SecurityImpl.Parse("CLZ8 FUT NYMEX");
            mb.Add(longform);
            compare = BasketImpl.Deserialize(mb.ToString());
            Assert.AreEqual(longform.ToString(),compare[0].ToString());



        }

        [Test]
        public void Enumeration()
        {
            BasketImpl mb = new BasketImpl(new string[] { "IBM", "MHS", "LVS", "GM" });
            string[] l = new string[4];
            int i = 0;
            foreach (SecurityImpl s in mb)
                l[i++] = s.Symbol;
            Assert.AreEqual(4, i);

        }

        [Test]
        public void Files()
        {
            // create basket
            BasketImpl mb = new BasketImpl(new string[] { "IBM", "MHS", "LVS", "GM" });
            // save it to a file
            const string file = "test.txt";
            BasketImpl.ToFile(mb, file);
            // restore it
            Basket nb = BasketImpl.FromFile(file);
            // verify it has same number of symbols
            Assert.AreEqual(mb.Count, nb.Count);
            // remove original contents from restored copy
            nb.Remove(mb);
            // verify nothing is left
            Assert.AreEqual(0, nb.Count);
        }
    }
}

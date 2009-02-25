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
            BasketImpl mb = new BasketImpl(new string[] { "IBM","LVS","T","GS","MHS" } );
            BasketImpl rem = new BasketImpl(new string[] { "LVS", "MHS" });
            Assert.That(mb.Count == 5);
            Assert.That(rem.Count == 2);
            mb.Remove(rem);
            Assert.That(mb.Count == 3,mb.Count.ToString());
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

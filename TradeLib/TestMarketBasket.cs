using System;
using NUnit.Framework;
using TradeLib;

namespace TestTradeLib
{
    [TestFixture]
    public class TestMarketBasket
    {
        public TestMarketBasket()
        {
        }

        [Test]
        public void BasketBasics()
        {
            MarketBasket mb = new MarketBasket();
            Assert.That(mb != null);
            Stock i = new Stock("IBM");
            mb = new MarketBasket(i);
            Assert.That(mb.hasStock);
            mb.Remove(i);
            Assert.That(!mb.hasStock);
            mb.Add(new Stock("LVS"));
            Assert.That(mb.Get(0).Symbol=="LVS",mb[0].ToString());
            mb.Add(new Stock("IBM"));
            Assert.That(mb[1].Symbol=="IBM");
            MarketBasket newbasket = new MarketBasket(new Stock("FDX"));
            newbasket.Add(mb);
            mb.Clear();
            Assert.That(mb.Count==0);
            Assert.That(newbasket.Count==3);
        }

        [Test]
        public void Multiple()
        {
            MarketBasket mb = new MarketBasket(new string[] { "IBM","LVS","T","GS","MHS" } );
            MarketBasket rem = new MarketBasket(new string[] { "LVS", "MHS" });
            Assert.That(mb.Count == 5);
            Assert.That(rem.Count == 2);
            mb.Subtract(rem);
            Assert.That(mb.Count == 3,mb.Count.ToString());
        }

        [Test]
        public void Serialization()
        {
            MarketBasket mb = new MarketBasket();
            mb.Add(new Stock("IBM"));
            MarketBasket compare = MarketBasket.FromString(mb.ToString());
            Assert.That(compare.Count == 1);
            mb.Clear();
            compare = MarketBasket.FromString(mb.ToString());
            Assert.That(compare.Count==0);



        }
    }
}

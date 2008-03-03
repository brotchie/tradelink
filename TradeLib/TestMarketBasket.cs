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
            Assert.That(mb.Get(0).Symbol=="LVS");
            mb.Add(new Stock("IBM"));
            Assert.That(mb[1].Symbol=="IBM");
            MarketBasket newbasket = new MarketBasket(new Stock("FDX"));
            newbasket.Add(mb);
            mb.Clear();
            Assert.That(mb.Count==0);
            Assert.That(newbasket.Count==3);
        }
    }
}

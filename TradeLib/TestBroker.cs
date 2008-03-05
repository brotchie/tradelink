using System;
using System.Collections.Generic;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
{
    [TestFixture]
    public class TestBroker
    {
        public TestBroker()
        {

        }
        Broker broker = new Broker();

        [Test]
        public void Basics()
        {
            Broker broker = new Broker();
            Order o = new Order();
            bool succeeded = broker.sendOrder(o);
            Assert.That(!succeeded);
        }

        [Test]
        public void MultiAccount()
        {
            const string sym = "TST";
            Order o = new BuyMarket(sym,100);
            const string me = "tester";
            const string other = "anotherguy";
            Account a = new Account(me);
            Account b = new Account(other);
            Account c = new Account("sleeper");
            // send order to account for jfranta
            Assert.That(broker.sendOrder(o, a));
            Assert.That(broker.sendOrder(o, b));
            Tick t = new Tick(sym);
            t.trade = 100m;
            t.size = 200;
            Assert.That(broker.Execute(t)==2);
            Position apos = broker.GetOpenPosition(sym,a);
            Position bpos = broker.GetOpenPosition(sym,b);
            Position cpos = broker.GetOpenPosition(sym, c);
            Assert.That(apos.Side && (apos.Size == 100));
            Assert.That(bpos.Side && (bpos.Size == 100));
            Assert.That(cpos.Flat);
            // make sure that default account doesn't register
            // any trades
            Assert.That(broker.GetOpenPosition(sym).Flat);
        }
    }
}

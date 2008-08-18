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
        int fills = 0,orders = 0, warn=0;
        const string s = "TST";

        [Test]
        public void Basics()
        {
            Broker broker = new Broker();
            broker.GotFill += new FillDelegate(broker_GotFill);
            broker.GotOrder += new OrderDelegate(broker_GotOrder);
            broker.GotWarning += new DebugDelegate(broker_GotWarning);
            Order o = new Order();
            uint failsoninvalid= broker.sendOrder(o);
            Assert.That(failsoninvalid==0);
            Assert.That(warn == 1);
            Assert.That(orders == 0);
            Assert.That(fills == 0);
            o = new BuyMarket(s, 100);
            Assert.That(broker.sendOrder(o)>0);
            Assert.That(orders == 1);
            Assert.That(fills == 0);
            Assert.That(broker.Execute(Tick.NewTrade(s,10,200)) == 1);
            Assert.That(fills == 1);
            // no warnings since first warning
            Assert.That(warn == 1);
        }
        void broker_GotWarning(string msg)
        {
            warn++;
        }

        void broker_GotOrder(Order o)
        {
            orders++;
        }

        void broker_GotFill(Trade t)
        {
            fills++;
        }

        int gottickDP = 0;
        Tick receivedtickDP;

        [Test]
        public void DataProvider()
        {
            Tick t = Tick.NewTrade(s, 10, 700);
            // feature to pass-through ticks to any subscriber
            // this can be connected to tradelink library to allow filtered subscribptions
            // and interapplication communication
            Broker broker = new Broker();
            broker.GotTick += new TickDelegate(broker_GotTick);
            Assert.That((receivedtickDP == null) && (gottickDP == 0));
            broker.Execute(t); // should fire a gotTick
            Assert.That(gottickDP != 0);
            Assert.That((receivedtickDP != null) && (receivedtickDP.trade == t.trade));

        }
        void broker_GotTick(Tick tick)
        {
            receivedtickDP = new Tick(tick);
            gottickDP++;
        }

        [Test]
        public void BBO()
        {
            Broker broker = new Broker();
            const string s = "TST";
            const decimal p1 = 10m;
            const decimal p2 = 11m;
            const int x = 100;
            Order bid,offer;

            // send bid, make sure it's BBO (since it's only order on any book)
            broker.sendOrder(new BuyLimit(s, x, p1));
            bid = broker.BestBid(s);
            offer = broker.BestOffer(s);
            Assert.That(bid.isValid && (bid.price==p1) && (bid.size==x), bid.ToString());
            Assert.That(!offer.isValid, offer.ToString());

            // add better bid, make sure it's BBO
            uint id1 = broker.sendOrder(new BuyLimit(s, x, p2));
            bid = broker.BestBid(s);
            offer = broker.BestOffer(s);
            Assert.That(bid.isValid && (bid.price == p2) && (bid.size == x), bid.ToString());
            Assert.That(!offer.isValid, offer.ToString());

            // add another bid at same price on another account, make sure it's additive
            uint id2 = broker.sendOrder(new BuyLimit(s, x, p2),new Account("ANOTHER_ACCOUNT"));
            bid = broker.BestBid(s);
            offer = broker.BestOffer(s);
            Assert.That(bid.isValid && (bid.price == p2) && (bid.size == (2*x)), bid.ToString());
            Assert.That(!offer.isValid, offer.ToString());

            // cancel order and make sure bbo returns
            broker.CancelOrder(id1);
            broker.CancelOrder(id2);
            bid = broker.BestBid(s);
            offer = broker.BestOffer(s);
            Assert.That(bid.isValid && (bid.price == p1) && (bid.size == x), bid.ToString());
            Assert.That(!offer.isValid, offer.ToString());

            // other test ideas
            // replicate above tests for sell-side


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
            Assert.That(broker.sendOrder(o, a)>0);
            Assert.That(broker.sendOrder(o, b)>0);
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

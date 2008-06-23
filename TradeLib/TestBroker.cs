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
            bool succeeded = broker.sendOrder(o);
            Assert.That(!succeeded);
            Assert.That(warn == 1);
            Assert.That(orders == 0);
            Assert.That(fills == 0);
            o = new BuyMarket(s, 100);
            Assert.That(broker.sendOrder(o));
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

        int gottickbook = 0;
        Tick tickbook;

        public void BBOProvider()
        {
            // this test is to show that GotTicks fired from Brokers will
            // reflect any BBO-affecting information in this broker's open-order queue
            const decimal price = 10m;
            const int size = 700;

            // setup the book
            Tick t = Tick.NewBid(s, price, size);

            // initialize our test variables
            Broker broker = new Broker();
            decimal limit = price + 1m;
            int limitsize = size * 2;
            broker.GotTick +=new TickDelegate(broker_GotTick2);
            Assert.That((gottickbook==0) && (tickbook==null));

            // verify received tick matches our limit order
            broker.sendOrder(new BuyLimit(s, size, limit));  // throws gottick if it's top of book
            Assert.That(gottickbook == 1, gottickbook.ToString());
            Assert.That((tickbook.bid == limit) && (tickbook.bs == size), tickbook.ToString());

            // if our order stays in queue, next tick should reflect our order as the best bid
            broker.Execute(t); // should throw gotTick
            Assert.That(gottickbook == 2, gottickbook.ToString());
            Assert.That((tickbook.bid == limit) && (tickbook.bs == size), tickbook.ToString());

            // lets do same as previous two examples, except put our limit at current best bid
            broker.CancelOrders();
            broker.sendOrder(new BuyLimit(s, limitsize, price)); // increases size only
            broker.Execute(t);
            Assert.That(gottickbook == 4, gottickbook.ToString());
            Assert.That((tickbook.bid == price) && (tickbook.bs == limitsize), tickbook.ToString());

            // lets throw an order below best bid and make sure it DOESN'T do anything
            broker.CancelOrders();
            broker.sendOrder(new BuyLimit(s, size, price - 1)); // no gotTick this time
            broker.Execute(t);
            Assert.That(gottickbook == 5, gottickbook.ToString());
            Assert.That((tickbook.bid == price) && (tickbook.bs == size), tickbook.ToString());

            // make sure buy stop does nothing
            broker.sendOrder(new BuyStop(s, size, price)); // no gotTick
            broker.Execute(t);
            Assert.That(gottickbook == 6, gottickbook.ToString());
            Assert.That((tickbook.bid==price) && (tickbook.bs==size),tickbook.ToString());

            // make sure buy market does nothing
            broker.sendOrder(new BuyMarket(s, size)); // no gotTick
            broker.Execute(t);
            Assert.That(gottickbook == 7, gottickbook.ToString());
            Assert.That((tickbook.bid == price) && (tickbook.bs == size), tickbook.ToString());

            // other test that could be inserted:
            // sell stop (doesn't affect)
            // sell market (doesn't affect)
            // throw order below best offer (affects best office size + price)
            // throw order at best offer with more size (affects size at best offer)
            // throw order above best offer (shouldn't affect book)
        }

        void broker_GotTick2(Tick tick)
        {
            gottickbook++;
            tickbook = new Tick(tick);
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

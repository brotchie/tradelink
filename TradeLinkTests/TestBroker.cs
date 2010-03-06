using System;
using System.Collections.Generic;
using TradeLink.Common;
using NUnit.Framework;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TestBroker
    {
        public TestBroker()
        {

        }
        Broker broker = new Broker();
        int fills = 0, orders = 0;
        const string s = "TST";

        [Test]
        public void Basics()
        {
            Broker broker = new Broker();
            broker.GotFill += new FillDelegate(broker_GotFill);
            broker.GotOrder += new OrderDelegate(broker_GotOrder);
            OrderImpl o = new OrderImpl();
            int error = broker.SendOrderStatus(o);
            Assert.AreNotEqual((int)MessageTypes.OK,error);
            Assert.That(orders == 0);
            Assert.That(fills == 0);
            o = new BuyMarket(s, 100);
            broker.SendOrderStatus(o);
            Assert.That(orders == 1);
            Assert.That(fills == 0);
            Assert.That(broker.Execute(TickImpl.NewTrade(s,10,200)) == 1);
            Assert.That(fills == 1);

            // test that a limit order is not filled outside the market
            o = new BuyLimit(s, 100, 9);
            broker.SendOrderStatus(o);
            Assert.AreEqual(0, broker.Execute(TickImpl.NewTrade(s, 10, 100)));
            Assert.That(fills == 1); // redudant but for counting

            // test that limit order is filled inside the market
            Assert.AreEqual(1, broker.Execute(TickImpl.NewTrade(s, 8, 100)));
            Assert.That(fills == 2);

            OrderImpl x = new OrderImpl();
            // test that a market order is filled when opposite book exists
            o = new SellLimit(s, 100, 11);
            x = new BuyMarket(s, 100);
            const string t2 = "trader2";
            x.Account = t2;
            broker.SendOrderStatus(o);
            broker.SendOrderStatus(x);
            Assert.AreEqual(3, fills); 

            // test that a market order is not filled when no book exists
            // on opposite side

            // clear existing orders
            broker.CancelOrders();
            o = new SellMarket(s, 100);
            o.Account = t2;
            broker.SendOrderStatus(o);
            Assert.AreEqual(3, fills);
            

            
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
            broker.SendOrderStatus(new BuyLimit(s, x, p1));
            bid = broker.BestBid(s);
            offer = broker.BestOffer(s);
            Assert.That(bid.isValid && (bid.price==p1) && (bid.size==x), bid.ToString());
            Assert.That(!offer.isValid, offer.ToString());

            // add better bid, make sure it's BBO
            Order o;
            // Order#1... 100 shares buy at $11 
            o = new BuyLimit(s, x, p2,1);
            broker.SendOrderStatus(o);
            bid = broker.BestBid(s);
            offer = broker.BestOffer(s);
            Assert.IsTrue(bid.isValid);
            Assert.AreEqual(p2,bid.price);
            Assert.AreEqual(x, bid.size);
            Assert.That(!offer.isValid, offer.ToString());

            // add another bid at same price on another account, make sure it's additive
            //order #2... 100 shares buy at $11
            o = new BuyLimit(s, x, p2,2);
            o.Account = "ANOTHER_ACCOUNT";
            broker.SendOrderStatus(o);
            bid = broker.BestBid(s);
            offer = broker.BestOffer(s);
            Assert.IsTrue(bid.isValid);
            Assert.AreEqual(p2, bid.price);
            Assert.AreEqual(x*2, bid.size); 
            Assert.That(!offer.isValid, offer.ToString());

            // cancel order and make sure bbo returns
            broker.CancelOrder(1);
            broker.CancelOrder(2);
            bid = broker.BestBid(s);
            offer = broker.BestOffer(s);
            Assert.IsTrue(bid.isValid);
            Assert.AreEqual(p1,bid.price);
            Assert.AreEqual(x,bid.size);
            Assert.IsTrue(!offer.isValid, offer.ToString());

            // other test ideas
            // replicate above tests for sell-side


        }






        [Test]
        public void MultiAccount()
        {
            const string sym = "TST";

            const string me = "tester";
            const string other = "anotherguy";
            const int od = 20070926;
            const int ot = 95400;
            Account a = new Account(me);
            Account b = new Account(other);
            Account c = new Account("sleeper");
            OrderImpl oa = new BuyMarket(sym,100);
            OrderImpl ob = new BuyMarket(sym, 100);
            oa.time = ot;
            oa.date = od;
            ob.time = ot;
            ob.date = od;

            oa.Account = me;
            ob.Account = other;
            // send order to account for jfranta
            broker.SendOrderStatus(oa);
            broker.SendOrderStatus(ob);
            TickImpl t = new TickImpl(sym);
            t.trade = 100m;
            t.size = 200;
            t.date = od;
            t.time = ot;
            Assert.AreEqual(2,broker.Execute(t));
            Position apos = broker.GetOpenPosition(sym,a);
            Position bpos = broker.GetOpenPosition(sym,b);
            Position cpos = broker.GetOpenPosition(sym, c);
            Assert.That(apos.isLong);
            Assert.AreEqual(100,apos.Size);
            Assert.That(bpos.isLong);
            Assert.AreEqual(100,bpos.Size);
            Assert.That(cpos.isFlat);
            // make sure that default account doesn't register
            // any trades
            Assert.That(broker.GetOpenPosition(sym).isFlat);
        }

        [Test]
        public void OPGs()
        {
            Broker broker = new Broker();
            const string s = "TST";
            // build and send an OPG order
            OrderImpl opg = new BuyOPG(s, 200, 10);
            broker.SendOrderStatus(opg);

            // build a tick on another exchange
            TickImpl it = TickImpl.NewTrade(s, 9, 100);
            it.ex = "ISLD";

            // fill order (should fail)
            int c = broker.Execute(it);
            Assert.AreEqual(0, c);

            // build opening price for desired exchange
            TickImpl nt = TickImpl.NewTrade(s, 9, 10000);
            nt.ex = "NYS";
            // fill order (should work)

            c = broker.Execute(nt);

            Assert.AreEqual(1, c);

            // add another OPG, make sure it's not filled with another tick

            TickImpl next = TickImpl.NewTrade(s, 9, 2000);
            next.ex = "NYS";

            OrderImpl late = new BuyOPG(s, 200, 10);
            broker.SendOrderStatus(late);
            c = broker.Execute(next);
            Assert.AreEqual(0, c);

        }
    }
}

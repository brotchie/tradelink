using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using NUnit.Framework;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TestOrder
    {
        public TestOrder() { }

        [Test]
        public void Defaults()
        {
            // assert that a default order is:
            // not valid, not filled
            OrderImpl o = new OrderImpl();
            Assert.That(!o.isValid);
            Assert.That(!o.isFilled);
        }

        [Test]
        public void MarketOrder()
        {
            const string s = "SYM";
            OrderImpl o = new OrderImpl(s,100);
            Assert.That(o.isValid);
            Assert.That(o.isMarket);
            Assert.That(!o.isLimit);
            Assert.That(!o.isStop);
            Assert.That(!o.isFilled);
            Assert.That(o.symbol == s);
        }

        [Test]
        public void Fill()
        {
            const string s = "TST";
            // market should fill on trade but not on quote
            OrderImpl o = new BuyMarket(s, 100);
            Assert.That(o.Fill(TickImpl.NewTrade(s, 9, 100)));
            Assert.That(!o.Fill(TickImpl.NewBid(s, 8, 100)));

            // buy limit

            // limit should fill if order price is inside market
            o = new BuyLimit(s, 100, 10m);
            Assert.That(o.Fill(TickImpl.NewTrade(s, 9, 100)));
            // shouldn't fill outside market
            o = new BuyLimit(s, 100, 10m);
            Assert.That(!o.Fill(TickImpl.NewTrade(s, 11, 100)));

            // sell limit

            // limit should fill if order price is inside market
            o = new SellLimit(s, 100, 10m);
            Assert.That(o.Fill(TickImpl.NewTrade(s, 11, 100)));
            // shouldn't fill outside market
            o = new SellLimit(s, 100, 10m);
            Assert.That(!o.Fill(TickImpl.NewTrade(s, 9, 100)));

            // buy stop

            o = new BuyStop(s, 100, 10m);
            Assert.That(o.Fill(TickImpl.NewTrade(s, 11, 100)));
            // shouldn't fill outside market
            o = new BuyStop(s, 100, 10m);
            Assert.That(!o.Fill(TickImpl.NewTrade(s, 9, 100)));

            // sell stop

            o = new SellStop(s, 100, 10m);
            Assert.That(o.Fill(TickImpl.NewTrade(s, 9, 100)));
            // shouldn't fill outside market
            o = new SellStop(s, 100, 10m);
            Assert.That(!o.Fill(TickImpl.NewTrade(s, 11, 100)));

            // always fail filling an invalid tick
            o = new BuyMarket(s, 100);
            Assert.IsFalse(o.Fill(TickImpl.NewTrade(s, 0, 0)));

            // always fail filling invalid order
            o = new BuyLimit(s, 100, 10);
            OrderImpl x = new OrderImpl();
            Assert.IsFalse(o.Fill(x));

            // always fail filling an order that doesn't cross market
            x = new BuyMarket(s, 100);
            Assert.IsFalse(o.Fill(x));

            const string t2 = "trader2";
            // suceed on crossing market
            x = new SellMarket(s,100);
            x.Account = t2;
            Assert.IsTrue(o.Fill(x));

            // fail when accounts are the same
            x = new SellMarket(s, 100);
            x.Account = o.Account;
            Assert.IsFalse(o.Fill(x));


            // fail on match outside of market
            x = new SellLimit(s, 100, 11);
            x.Account = t2;
            Assert.IsFalse(o.Fill(x));

            // succeed on limit cross
            o = new BuyLimit(s, 100, 10);
            x = new SellLimit(s, 100, 10);
            x.Account = t2;
            Assert.IsTrue(o.Fill(x));

            // make sure we can stop cross
            o = new SellStop(s, 100, 10);
            x = new BuyMarket(s, 100);
            x.Account = t2;
            Assert.IsTrue(o.Fill(x));

        }

        [Test]
        public void SerializationAndDeserialization()
        {
            // create an order
            const string s = "TST";
            const string x = "NYSE";
            const string a = "ACCOUNT";
            const string u = "COMMENT";
            const string ot = "GTC";
            const decimal p = 10;
            const int z = 100;
            const CurrencyType c = CurrencyType.USD;
            const SecurityType t = SecurityType.STK;
            Order o = new OrderImpl(s, z);
            o.date = 20080718;
            o.time = 94800;
            o.price = p;
            o.Account = a;
            o.ex = x;
            o.Currency = c;
            o.Security = t;
            o.comment = u;
            o.TIF = ot;
            // convert it to a message
            string msg = OrderImpl.Serialize(o);

            // convert it back to an object and validate nothing was lost
            string exception=null;
            Order n = new OrderImpl();
            try
            {
                n = OrderImpl.Deserialize(msg);
            }
            catch (Exception ex) { exception = ex.ToString(); }
            Assert.That(exception==null, msg+" "+exception);
            Assert.That(n.Account == a,n.Account);
            Assert.That(n.symbol == s,n.symbol);
            Assert.That(n.size == z,n.size.ToString());
            Assert.That(n.price == p,n.price.ToString());
            Assert.That(n.Exchange == x,n.Exchange);
            Assert.That(n.comment == u,n.comment);
            Assert.That(n.Security == t,n.Security.ToString());
            Assert.That(n.Currency == c,n.Currency.ToString());
            Assert.That(n.TIF == ot, n.TIF);
            Assert.That(n.date == o.date, n.date.ToString());
            Assert.That(n.time == o.time, n.time.ToString());
        }


    }
}

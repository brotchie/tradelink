using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
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
            Order o = new Order();
            Assert.That(!o.isValid);
            Assert.That(!o.isFilled);
        }

        [Test]
        public void MarketOrder()
        {
            const string s = "SYM";
            Order o = new Order(s,100);
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
            Order o = new BuyMarket(s, 100);
            Assert.That(o.Fill(Tick.NewTrade(s, 9, 100)));
            Assert.That(!o.Fill(Tick.NewBid(s, 8, 100)));

            // buy limit

            // limit should fill if order price is inside market
            o = new BuyLimit(s, 100, 10m);
            Assert.That(o.Fill(Tick.NewTrade(s, 9, 100)));
            // shouldn't fill outside market
            o = new BuyLimit(s, 100, 10m);
            Assert.That(!o.Fill(Tick.NewTrade(s, 11, 100)));

            // sell limit

            // limit should fill if order price is inside market
            o = new SellLimit(s, 100, 10m);
            Assert.That(o.Fill(Tick.NewTrade(s, 11, 100)));
            // shouldn't fill outside market
            o = new SellLimit(s, 100, 10m);
            Assert.That(!o.Fill(Tick.NewTrade(s, 9, 100)));

            // buy stop

            o = new BuyStop(s, 100, 10m);
            Assert.That(o.Fill(Tick.NewTrade(s, 11, 100)));
            // shouldn't fill outside market
            o = new BuyStop(s, 100, 10m);
            Assert.That(!o.Fill(Tick.NewTrade(s, 9, 100)));

            // sell stop

            o = new SellStop(s, 100, 10m);
            Assert.That(o.Fill(Tick.NewTrade(s, 9, 100)));
            // shouldn't fill outside market
            o = new SellStop(s, 100, 10m);
            Assert.That(!o.Fill(Tick.NewTrade(s, 11, 100)));

            // always fail filling an invalid tick
            o = new BuyMarket(s, 100);
            Assert.IsFalse(o.Fill(Tick.NewTrade(s, 0, 0)));

            // always fail filling invalid order
            o = new BuyLimit(s, 100, 10);
            Order x = new Order();
            Assert.IsFalse(o.Fill(x));

            // always fail filling an order that doesn't cross market
            x = new BuyMarket(s, 100);
            Assert.IsFalse(o.Fill(x));

            // suceed on crossing market
            x = new SellMarket(s,100);
            Assert.IsTrue(o.Fill(x));

            // fail on match outside of market
            x = new SellLimit(s, 100, 11);
            Assert.IsFalse(o.Fill(x));

            // succeed on limit cross
            x = new SellLimit(s, 100, 10);
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
            const Currency c = Currency.USD;
            const SecurityType t = SecurityType.STK;
            Order o = new Order(s, z);
            o.sec = 2;
            o.date = 20080718;
            o.time = 948;
            o.price = p;
            o.Account = a;
            o.Exchange = x;
            o.Currency = c;
            o.Security = t;
            o.comment = u;
            o.TIF = ot;
            // convert it to a message
            string msg = o.Serialize();

            // convert it back to an object and validate nothing was lost
            string exception=null;
            Order n = new Order();
            try
            {
                n = Order.Deserialize(msg);
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
            Assert.That(n.sec == o.sec, n.sec.ToString());
        }

        [Test]
        public void EqualsTest()
        {
            // following equals guidelines listed here:
            // http://msdn.microsoft.com/en-us/library/ms173147(VS.80).aspx

            // x.Equals(x) returns true.
            Order x = new Order("XTST", -300);
            Assert.That(x.Equals(x), x.ToString());

            // x.Equals(y) returns the same value as y.Equals(x).
            Order y = new Order("YTST",200);
            bool v1 = x.Equals(y);
            bool v2 = y.Equals(x);
            Assert.That(v1 == v2, x.ToString() + y.ToString());

            // if (x.Equals(y) && y.Equals(z)) returns true, then x.Equals(z) returns true.
            y = new Order(x);
            Order z = new Order(x);
            Assert.That(x.Equals(y) && y.Equals(z) && x.Equals(z));

            // Successive invocations of x.Equals(y) return the same value as long as the objects referenced by x and y are not modified.
            v1 = x.Equals(y);
            v2 = x.Equals(y);
            Assert.That(v1 == v2);

            // x.Equals(null) returns false.
            Assert.That(!x.Equals(null));


        }
    }
}

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
            const Security t = Security.STK;
            Order o = new Order(s, z);
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
        }
    }
}

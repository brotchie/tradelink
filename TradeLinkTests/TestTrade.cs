using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLink.Common;
using TradeLink.API;


namespace TestTradeLink
{
    [TestFixture]
    public class TestTrade
    {
        public TestTrade() { }

        [Test]
        public void Defaults()
        {
            TradeImpl t = new TradeImpl();
            Assert.That(!t.isValid, t.ToString());
            Assert.That(!t.isFilled, t.ToString());
        }

        [Test]
        public void Construction()
        {
            TradeImpl t = new TradeImpl("TST",10,100,DateTime.Now);
            Assert.That(t.isValid,t.ToString());
            Assert.That(t.isFilled,t.ToString());

            //midnight check
            t.xdate = 20081205;
            t.xtime = 0;
            Assert.That(t.isValid);
            t.xtime = 0;
            t.xdate = 0;
            Assert.That(!t.isValid);
        }

        [Test]
        public void SerializeDeserialize()
        {
            // create object
            string sym = "TST";
            decimal price = 10;
            int size = 100;
            DateTime date = DateTime.Now;
            TradeImpl t = new TradeImpl(sym, price, size, date);
            long magicid = 555;
            t.id = magicid;
            t.Exchange = "NYMEX";
            // serialize it for transmission
            string msg = TradeImpl.Serialize(t);
            // deserialize it
            string threwexception = null;
            Trade newtrade = null;
            try
            {
                newtrade = TradeImpl.Deserialize(msg);
            }
            catch (Exception ex) { threwexception = ex.ToString(); }

            Assert.That(threwexception == null, threwexception);
            Assert.That(newtrade.isFilled, newtrade.ToString());
            Assert.That(newtrade.isValid, newtrade.ToString());
            Assert.That(newtrade.symbol == sym, newtrade.symbol);
            Assert.That(newtrade.xprice == price, newtrade.xprice.ToString());
            Assert.That(newtrade.xdate != 0);
            Assert.That(newtrade.xtime != 0);
            Assert.That(newtrade.xsize == size);
            Assert.That(newtrade.id == magicid);
            Assert.AreEqual(newtrade.ex,t.Exchange);
        }

    }
}

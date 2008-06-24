using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLib;


namespace TestTradeLib
{
    [TestFixture]
    public class TestTrade
    {
        public TestTrade() { }

        [Test]
        public void Defaults()
        {
            Trade t = new Trade();
            Assert.That(!t.isValid, t.ToString());
            Assert.That(!t.isFilled, t.ToString());
        }

        [Test]
        public void Construction()
        {
            Trade t = new Trade("TST",10,100,DateTime.Now);
            Assert.That(t.isValid,t.ToString());
            Assert.That(t.isFilled,t.ToString());
        }

        [Test]
        public void SerializeDeserialize()
        {
            // create object
            string sym = "TST";
            decimal price = 10;
            int size = 100;
            DateTime date = DateTime.Now;
            Trade t = new Trade(sym, price, size, date);
            uint magicid = 555;
            t.id = magicid;
            // serialize it for transmission
            string msg = t.Serialize();
            // deserialize it
            string threwexception = null;
            Trade newtrade = null;
            try
            {
                newtrade = Trade.Deserialize(msg);
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
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLink.Common;
using TradeLink.API;
using TradeLink.Research;

namespace TestTradeLink
{
    [TestFixture]
    public class TestREGSHOTracker
    {
        REGSHO_ShortTracker sho = new REGSHO_ShortTracker();

        const string sym = "TST";

        [Test]
        public void Tests()
        {
            long id = 1;

            Order o = new OrderImpl();

            // take a position
            sho.GotPosition(new PositionImpl(sym,100,300));

            // accept two exits
            o = new SellLimit(sym, 100, 200, id++);
            Assert.IsFalse(sho.isOrderShort(o));
            sho.GotOrder(o);
            o = new SellLimit(sym, 200, 105, id++);
            Assert.IsFalse(sho.isOrderShort(o));
            sho.GotOrder(o);

            // send another short
            o = new SellStop(sym, 300, 99);
            Assert.IsTrue(sho.isOrderShort(o));


        }
    }
}

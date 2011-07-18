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
        public void Basic()
        {
            long id = 1;
            sho = new REGSHO_ShortTracker();
            sho.SendDebugEvent+=new DebugDelegate(rt.d);
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

        const string ACCT = "ACCT";

        [Test]
        public void BasicWithAccount()
        {
            long id = 1;
            sho = new REGSHO_ShortTracker();
            sho.SendDebugEvent += new DebugDelegate(rt.d);
            Order o = new OrderImpl();

            // take a position
            sho.GotPosition(new PositionImpl(sym, 100, 300,0,ACCT));

            // accept two exits
            o = new SellLimit(sym, 100, 200, id++);
            o.Account = ACCT;
            Assert.IsFalse(sho.isOrderShort(o));
            sho.GotOrder(o);
            o = new SellLimit(sym, 200, 105, id++);
            o.Account = ACCT;
            Assert.IsFalse(sho.isOrderShort(o));
            sho.GotOrder(o);

            // send another short
            o = new SellStop(sym, 300, 99);
            o.Account = ACCT;
            Assert.IsTrue(sho.isOrderShort(o));


        }
    }
}

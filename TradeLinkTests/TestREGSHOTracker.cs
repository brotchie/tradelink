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
        public void DoubleBasicWithFlat()
        {
            long id = 1;
            sho = new REGSHO_ShortTracker();
            sho.SendDebugEvent += new DebugDelegate(sho_SendDebugEvent);
            sho.VerboseDebugging = true;
            Order o = new OrderImpl();

            // take a position
            sho.GotPosition(new PositionImpl(sym, 89.7m, 100));

            // accept two exits
            o = new SellStop(sym, 100, 89.65m, id++);
            long stop1 = o.id;
            Assert.IsFalse(sho.isOrderShort(o), "entry1: first sell was incorrectly short");
            sho.GotOrder(o);
            o = new SellLimit(sym, 100, 89.75m, id++);
            long profit1 = o.id;
            Assert.IsTrue(sho.isOrderShort(o), "entry1: second sell was incorrectly sell");
            sho.GotOrder(o);


            // flat
            o = new SellStop(sym, 100, 89.65m, stop1);
            o.Fill(TickImpl.NewTrade(sym,89.62m,100));
            sho.GotFill((Trade)o);
            sho.GotCancel(profit1);

            // do again
            // take a position
            o = new BuyMarket(sym,100);
            o.id = id++;
            o.Fill(TickImpl.NewTrade(sym, 89.64m, 100));
            sho.GotFill((Trade)o);

            // accept two exits
            o = new SellStop(sym, 100, 89.65m, id++);
            Assert.IsFalse(sho.isOrderShort(o), "entry2: first sell was incorrectly short");
            sho.GotOrder(o);
            o = new SellLimit(sym, 100, 89.75m, id++);
            Assert.IsTrue(sho.isOrderShort(o), "entry2: second sell was incorrectly NOT short");
            sho.GotOrder(o);



        }

        int shodebug = 0;
        void sho_SendDebugEvent(string msg)
        {
            shodebug++ ;
            rt.d(shodebug + " " + msg);
        }

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

        int pendingsize(string sym)
        {
            int s = 0;
            foreach (Order o in sho)
                if (o.symbol == sym)
                    if (sho.isPending(o.id))
                        s += o.UnsignedSize;
            return s;
        }

        [Test]
        public void FillPendTest()
        {
            long id = 1;
            sho = new REGSHO_ShortTracker();
            sho.SendDebugEvent += new DebugDelegate(rt.d);
            sho.VerboseDebugging = true;
            lastids.Clear();
            Order o = new OrderImpl();

            // send order 
            o = new BuyLimit(sym, 100, 10, id++);
            so(o);
            // verify pending
            Assert.AreEqual(100, pendingsize(sym),"missing pending size");
            // fill it
            Assert.IsTrue(o.Fill(TickImpl.NewTrade(sym,10,100)),"no fill");
            sho.GotFill((Trade)o);
            // verify it's not pending
            Assert.AreEqual(0, pendingsize(sym),"missing pending size");

        }

        [Test]
        public void FillThenStopAndLimitOversell()
        {
            long id = 1;
            sho = new REGSHO_ShortTracker();
            sho.SendDebugEvent += new DebugDelegate(rt.d);
            sho.VerboseDebugging = true;
            lastids.Clear();
            Order o = new OrderImpl();

            // send some initial orders
            so(new SellLimit(sym, 100, 25.83m, id++));
            so(new SellStop(sym, 100, 25.83m, id++));
            o = new SellStop(sym, 200, 25.83m, id++);
            so(o);
            // cancel first two orders
            sho.GotCancel(lastids[0]);
            sho.GotCancel(lastids[1]);
            // fill last order
            Assert.IsTrue(o.Fill(TickImpl.NewTrade(sym, 25.80m, 200)), "missing initial fill");
            sho.GotFill((Trade)o);
            // check pending size
            Assert.AreEqual(0, pendingsize(sym), "had pending size after cancels and fills");
            // flat position
            sho.GotPosition(new PositionImpl(sym,0,0,0,ACCT));
            


            // take a position
            o = new BuyLimit(sym, 100, 25.83m, id++);
            o.Account = ACCT;
            Assert.IsFalse(sho.isOrderShort(o), "entry buy never a short.");
            sho.GotOrder(o);
            Assert.IsTrue(o.Fill(TickImpl.NewTrade(sym, 25.80m, 100)), "unable to fill order");
            Trade t = (Trade)o;
            Assert.IsTrue(t.isValid && t.isFilled, "not a valid trade");
            sho.GotFill(t);


            // accept two exits
            o = new SellStop(sym, 100, 21.09m, id++);
            o.Account = ACCT;
            Assert.IsFalse(sho.isOrderShort(o), "first exit was wrongly a short");
            sho.GotOrder(o);
            o = new SellLimit(sym, 100, 21.19m, id++);
            o.Account = ACCT;
            Assert.IsTrue(sho.isOrderShort(o), "second exit was wrongly a sell");
            sho.GotOrder(o);
        }


        [Test]
        public void BasicStopAndLimit()
        {
            long id = 1;
            sho = new REGSHO_ShortTracker();
            sho.SendDebugEvent += new DebugDelegate(rt.d);
            sho.VerboseDebugging = true;
            Order o = new OrderImpl();

            // take a position
            o = new BuyLimit(sym, 100, 21.18m, id++);
            o.Account = ACCT;
            Assert.IsFalse(sho.isOrderShort(o), "entry buy never a short.");
            sho.GotOrder(o);
            Assert.IsTrue(o.Fill(TickImpl.NewTrade(sym,21.14m,100)),"unable to fill order");
            Trade t = (Trade)o;
            Assert.IsTrue(t.isValid && t.isFilled, "not a valid trade");
            sho.GotFill(t);
            

            // accept two exits
            o = new SellStop(sym, 100, 21.09m, id++);
            o.Account = ACCT;
            Assert.IsFalse(sho.isOrderShort(o),"first exit was wrongly a short");
            sho.GotOrder(o);
            o = new SellLimit(sym, 100, 21.19m, id++);
            o.Account = ACCT;
            Assert.IsTrue(sho.isOrderShort(o), "second exit was wrongly a sell");
            sho.GotOrder(o);

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

        List<long> lastids = new List<long>();
        void so(Order o)
        {
            lastids.Add(o.id);
            o.Account = ACCT;
            sho.GotOrder(o);

        }
    }
}

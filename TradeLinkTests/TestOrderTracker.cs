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
    public class TestOrderTracker
    {

        OrderTracker ot = new OrderTracker();
        const string sym = "TST";
        Order o = new OrderImpl();
        long id = 1;

        [Test]
        public void Sent()
        {
            // reset everything
            id = 1;
            o = new OrderImpl();
            ot = new OrderTracker();
            ot.SendDebugEvent += new DebugDelegate(ot_SendDebugEvent);
            ot.VerboseDebugging = true;

            // verify no size/pending/cancel
            Assert.AreEqual(0, ot.Sent(id),"sent but not sent");
            Assert.IsFalse(ot.isCompleted(id),"completed but not sent");
            Assert.IsFalse(ot.isCanceled(id), "wrongly canceled");
            Assert.IsFalse(ot.isPending(id), "wrongly pending");
            Assert.IsFalse(ot.isTracked(id), "wrongly tracked");
            // send a buy order
            ot.GotOrder(new BuyLimit(sym, 100, 100, id++));
            // verify order is there
            Assert.IsTrue(ot.SentOrder(id - 1).isValid, "no valid order");
            // verify size/pending/cancel
            Assert.AreEqual(100, ot.Sent(id-1), "not sent");
            Assert.AreEqual(0, ot.Filled(id - 1), "incorrect fill size");
            Assert.IsFalse(ot.isCompleted(id - 1), "completed but not filled");
            Assert.IsFalse(ot.isCanceled(id - 1), "wrongly canceled");
            Assert.IsTrue(ot.isPending(id - 1), "not pending");
            Assert.IsTrue(ot.isTracked(id - 1), "not tracked");

            // do sell order

            // verify no size/pending/cancel
            Assert.AreEqual(0, ot.Sent(id), "sent but not sent");
            Assert.IsFalse(ot.isCompleted(id), "completed but not sent");
            Assert.IsFalse(ot.isCanceled(id), "wrongly canceled");
            Assert.IsFalse(ot.isPending(id), "wrongly pending");
            Assert.IsFalse(ot.isTracked(id), "wrongly tracked");
            // send a sell order
            ot.GotOrder(new SellLimit(sym, 100, 100, id++));
            // verify order is there
            Assert.IsTrue(ot.SentOrder(id - 1).isValid, "no valid order");
            // verify size/pending/cancel
            Assert.AreEqual(-100, ot.Sent(id - 1), "not sent");
            Assert.AreEqual(0, ot.Filled(id - 1), "incorrect fill size");
            Assert.IsFalse(ot.isCompleted(id - 1), "completed but not filled");
            Assert.IsFalse(ot.isCanceled(id - 1), "wrongly canceled");
            Assert.IsTrue(ot.isPending(id - 1), "not pending");
            Assert.IsTrue(ot.isTracked(id - 1), "not tracked");
            
        }



        [Test]
        public void Fill()
        {
            // reset everything
            id = 1;
            o = new OrderImpl();
            ot = new OrderTracker();
            ot.SendDebugEvent += new DebugDelegate(ot_SendDebugEvent);
            ot.VerboseDebugging = true;

            // verify no size/pending/cancel
            Assert.AreEqual(0, ot.Sent(id), "sent but not sent");
            Assert.IsFalse(ot.isCompleted(id), "completed but not sent");
            Assert.IsFalse(ot.isCanceled(id), "wrongly canceled");
            Assert.IsFalse(ot.isPending(id), "wrongly pending");
            Assert.IsFalse(ot.isTracked(id), "wrongly tracked");
            // send a buy order
            o = new BuyLimit(sym, 100, 100, id++);
            ot.GotOrder(o);
            // fill it
            Assert.IsTrue(o.Fill(TickImpl.NewTrade(sym,100,100)),"order did not fill");
            ot.GotFill((Trade)o);
            // verify order is there
            Assert.IsTrue(ot.SentOrder(id - 1).isValid, "no valid order");
            // verify size/pending/cancel
            Assert.AreEqual(100, ot.Sent(id - 1), "not sent buy");
            Assert.AreEqual(100, ot.Filled(id - 1), "incorrect fill size buy");
            Assert.IsTrue(ot.isCompleted(id - 1), "wrongly not filled");
            Assert.IsFalse(ot.isCanceled(id - 1), "wrongly canceled");
            Assert.IsFalse(ot.isPending(id - 1), "wrongly pending");
            Assert.IsTrue(ot.isTracked(id - 1), "not tracked");

            // do sell order

            // verify no size/pending/cancel
            Assert.AreEqual(0, ot.Sent(id), "sent but not sent");
            Assert.IsFalse(ot.isCompleted(id), "completed but not sent");
            Assert.IsFalse(ot.isCanceled(id), "wrongly canceled");
            Assert.IsFalse(ot.isPending(id), "wrongly pending");
            Assert.IsFalse(ot.isTracked(id), "wrongly tracked");
            // send sell order
            o = new SellLimit(sym, 100, 100, id++);
            ot.GotOrder(o);
            // fill it
            Assert.IsTrue(o.Fill(TickImpl.NewTrade(sym, 100, 100)), "order did not fill");
            ot.GotFill((Trade)o);
            // verify order is there
            Assert.IsTrue(ot.SentOrder(id - 1).isValid, "no valid order");
            // verify size/pending/cancel
            Assert.AreEqual(-100, ot.Filled(id - 1), "incorrect fill size sell");
            Assert.AreEqual(-100, ot.Sent(id - 1), "not sent sell");
            Assert.IsTrue(ot.isCompleted(id - 1), "wrongly not filled");
            Assert.IsFalse(ot.isCanceled(id - 1), "wrongly canceled");
            Assert.IsFalse(ot.isPending(id - 1), "wrongly pending");
            Assert.IsTrue(ot.isTracked(id - 1), "not tracked");
        }

        [Test]
        public void PartialFill()
        {
            // reset everything
            id = 1;
            o = new OrderImpl();
            ot = new OrderTracker();
            ot.SendDebugEvent += new DebugDelegate(ot_SendDebugEvent);
            ot.VerboseDebugging = true;

            // verify no size/pending/cancel
            Assert.AreEqual(0, ot.Sent(id), "sent but not sent");
            Assert.IsFalse(ot.isCompleted(id), "completed but not sent");
            Assert.IsFalse(ot.isCanceled(id), "wrongly canceled");
            Assert.IsFalse(ot.isPending(id), "wrongly pending");
            Assert.IsFalse(ot.isTracked(id), "wrongly tracked");
            // send a buy order
            o = new BuyLimit(sym, 200, 100, id++);
            ot.GotOrder(o);
            // fill it
            Assert.IsTrue(o.Fill(TickImpl.NewTrade(sym, 100, 100)), "order did not fill");
            ot.GotFill((Trade)o);
            // verify order is there
            Assert.IsTrue(ot.SentOrder(id - 1).isValid, "no valid order");
            // verify size/pending/cancel
            Assert.AreEqual(200, ot.Sent(id - 1), "not sent buy");
            Assert.AreEqual(100, ot.Filled(id - 1), "incorrect fill size buy");
            Assert.IsFalse(ot.isCompleted(id - 1), "wrongly completed");
            Assert.IsFalse(ot.isCanceled(id - 1), "wrongly canceled");
            Assert.IsTrue(ot.isPending(id - 1), "wrongly pending");
            Assert.IsTrue(ot.isTracked(id - 1), "not tracked");

            // do sell order

            // verify no size/pending/cancel
            Assert.AreEqual(0, ot.Sent(id), "sent but not sent");
            Assert.IsFalse(ot.isCompleted(id), "completed but not sent");
            Assert.IsFalse(ot.isCanceled(id), "wrongly canceled");
            Assert.IsFalse(ot.isPending(id), "wrongly pending");
            Assert.IsFalse(ot.isTracked(id), "wrongly tracked");
            // send sell order
            o = new SellLimit(sym, 200, 100, id++);
            ot.GotOrder(o);
            // fill it
            Assert.IsTrue(o.Fill(TickImpl.NewTrade(sym, 100, 100)), "order did not fill");
            ot.GotFill((Trade)o);
            // verify order is there
            Assert.IsTrue(ot.SentOrder(id - 1).isValid, "no valid order");
            // verify size/pending/cancel
            Assert.AreEqual(-100, ot.Filled(id - 1), "incorrect fill size sell");
            Assert.AreEqual(-200, ot.Sent(id - 1), "not sent sell");
            Assert.IsFalse(ot.isCompleted(id - 1), "wrongly completed");
            Assert.IsFalse(ot.isCanceled(id - 1), "wrongly canceled");
            Assert.IsTrue(ot.isPending(id - 1), "wrongly pending");
            Assert.IsTrue(ot.isTracked(id - 1), "not tracked");
        }

        [Test]
        public void Cancel()
        {
            // reset everything
            id = 1;
            o = new OrderImpl();
            ot = new OrderTracker();
            ot.SendDebugEvent += new DebugDelegate(ot_SendDebugEvent);
            ot.VerboseDebugging = true;

            // verify no size/pending/cancel
            Assert.AreEqual(0, ot.Sent(id), "sent but not sent");
            Assert.IsFalse(ot.isCompleted(id), "completed but not sent");
            Assert.IsFalse(ot.isCanceled(id), "wrongly canceled");
            Assert.IsFalse(ot.isPending(id), "wrongly pending");
            Assert.IsFalse(ot.isTracked(id), "wrongly tracked");
            // send a buy order
            o = new BuyLimit(sym, 200, 100, id++);
            ot.GotOrder(o);
            // cancel it
            ot.GotCancel(id - 1);
            // verify order is there
            Assert.IsTrue(ot.SentOrder(id - 1).isValid, "no valid order");
            // verify size/pending/cancel
            Assert.AreEqual(200, ot.Sent(id - 1), "not sent buy");
            Assert.AreEqual(0, ot.Filled(id - 1), "incorrect fill size buy");
            Assert.IsFalse(ot.isCompleted(id - 1), "wrongly completed");
            Assert.IsTrue(ot.isCanceled(id - 1), "not canceled");
            Assert.IsFalse(ot.isPending(id - 1), "wrongly pending");
            Assert.IsTrue(ot.isTracked(id - 1), "not tracked");

            // do sell order

            // verify no size/pending/cancel
            Assert.AreEqual(0, ot.Sent(id), "sent but not sent");
            Assert.IsFalse(ot.isCompleted(id), "completed but not sent");
            Assert.IsFalse(ot.isCanceled(id), "wrongly canceled");
            Assert.IsFalse(ot.isPending(id), "wrongly pending");
            Assert.IsFalse(ot.isTracked(id), "wrongly tracked");
            // send sell order
            o = new SellLimit(sym, 200, 100, id++);
            ot.GotOrder(o);
            // fill it
            ot.GotCancel(id - 1);

            // verify order is there
            Assert.IsTrue(ot.SentOrder(id - 1).isValid, "no valid order");
            // verify size/pending/cancel
            Assert.AreEqual(0, ot.Filled(id - 1), "incorrect fill size sell");
            Assert.AreEqual(-200, ot.Sent(id - 1), "not sent sell");
            Assert.IsFalse(ot.isCompleted(id - 1), "wrongly completed");
            Assert.IsTrue(ot.isCanceled(id - 1), "not canceled");
            Assert.IsFalse(ot.isPending(id - 1), "wrongly pending");
            Assert.IsTrue(ot.isTracked(id - 1), "not tracked");
        }

        void ot_SendDebugEvent(string msg)
        {
            rt.d(msg);
        }

    }
}

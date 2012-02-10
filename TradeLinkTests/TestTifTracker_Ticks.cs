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
    public class TestTifTracker_Ticks
    {
        TIFTracker_Ticks tks = new TIFTracker_Ticks();

        void reset()
        {
            tks = new TIFTracker_Ticks();
            tks.SendCancelEvent += new LongDelegate(tks_SendCancelEvent);
            tks.SendDebugEvent += new DebugDelegate(tks_SendDebugEvent);
            tks.SendOrderEvent += new OrderDelegate(tks_SendOrderEvent);
            lasto = new OrderImpl();
            lastcancel = 0;
        }

        Order lasto = new OrderImpl();
        long lastcancel = 0;

        void tks_SendOrderEvent(Order o)
        {
            lasto = o;
            tks.GotOrder(o);
        }

        void tks_SendDebugEvent(string msg)
        {
            rt.d(msg);
        }

        void tks_SendCancelEvent(long val)
        {
            lastcancel = val;
        }

        const string sym = "TST";
        const decimal p = 10;

        [Test]
        public void SeveralTicks()
        {
            // reset everything 
            reset();
            // we'll use quotes to enforce tighter tifs (default)
            Assert.IsFalse(tks.IgnoreQuotes, "ignorequotes is expected to be off by default");
            // verify we haven't done anything
            Assert.IsFalse(lasto.isValid, "order was sent prior to doing anything");
            Assert.AreEqual(0, lastcancel, "cancel sent prior to doing anything");
            // send order with tif
            tks.SendOrderTIF(new BuyLimit(sym, 100, p), 2);
            // verify received
            Assert.IsTrue(lasto.isValid, "order not sent");
            // send some ticks
            tks.newTick(TickImpl.NewTrade(sym, p, 100));
            // verify no cancel
            Assert.AreEqual(0, lastcancel,"cancel was sent prior to tif expiration");
            //send more ticks
            tks.newTick(TickImpl.NewTrade(sym, p, 100));
            // verify cancel
            Assert.AreNotEqual(0, lastcancel, "cancel was NOT sent after tif expiration");
        }

        [Test]
        public void IOC()
        {
            // reset everything 
            reset();
            // we'll use quotes to enforce tighter tifs
            tks.IOCMode();
            Assert.IsTrue(tks.IgnoreQuotes, "ignore quotes should be true when ioc mode is enabled");
            // verify we haven't done anything
            Assert.IsFalse(lasto.isValid, "order was sent prior to doing anything");
            Assert.AreEqual(0, lastcancel, "cancel sent prior to doing anything");

            // send order with tif
            tks.SendOrderTIF(new BuyLimit(sym, 100, p), 1);
            // verify received
            Assert.IsTrue(lasto.isValid, "order not sent");
            // send some ticks
            tks.newTick(TickImpl.NewBid(sym, p, 100));
            tks.newTick(TickImpl.NewBid(sym, p, 100));
            tks.newTick(TickImpl.NewBid(sym, p, 100));
            tks.newTick(TickImpl.NewBid(sym, p, 100));
            tks.newTick(TickImpl.NewAsk(sym, p, 100));
            // verify no cancel
            Assert.AreEqual(0, lastcancel, "cancel was sent prior to tif expiration");
            tks.newTick(TickImpl.NewTrade(sym, p, 100));
            // verify cancel
            Assert.AreNotEqual(0, lastcancel, "cancel was NOT sent after tif expiration");
        }
    }
}

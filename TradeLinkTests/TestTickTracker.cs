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
    public class TestTickTracker
    {

        TickTracker kt = new TickTracker();
        const string sym = "TST";
        const decimal p = 15;
        const int ss = 1;
        const int s = ss*100;

        [Test]
        public void Basics()
        {
            kt = new TickTracker();

            // startup tests
            Assert.IsFalse(kt[sym].isValid,"valid tick at startup");
            Assert.IsFalse(kt[sym].isTrade,"trade at startup");
            Assert.IsFalse(kt[sym].isQuote,"quote at startup");
            Assert.IsFalse(kt[sym].hasAsk,"ask at startup");
            Assert.IsFalse(kt[sym].hasBid,"bid at startup");

            // trade occurs
            kt.newTick(TickImpl.NewTrade(sym, p, s));

            Assert.IsTrue(kt[sym].isValid, "no valid tick at startup");
            Assert.IsTrue(kt[sym].isTrade, "no trade at startup");
            Assert.AreEqual(p, kt.Last(sym),"wrong trade price");
            Assert.AreEqual(s, kt[sym].size, "wrong trade size");
            Assert.IsFalse(kt[sym].isQuote, "quote at startup");
            Assert.IsFalse(kt[sym].hasAsk, "ask at startup");
            Assert.IsFalse(kt[sym].hasBid, "bid at startup");

            // bid
            kt.newTick(TickImpl.NewBid(sym, p, s));
            // verify bid
            Assert.IsTrue(kt[sym].isValid, "no valid tick at startup");
            Assert.AreEqual(p, kt.Bid(sym), "wrong bid price");
            Assert.AreEqual(ss, kt[sym].bs, "wrong bid size");
            Assert.IsFalse(kt[sym].hasAsk, "ask at startup");
            Assert.IsTrue(kt[sym].hasBid, "bid at startup");

            // verify trade still shows
            Assert.IsTrue(kt[sym].isValid, "no valid tick at startup");
            Assert.IsTrue(kt[sym].isTrade, "no trade at startup");
            Assert.AreEqual(p, kt.Last(sym), "wrong trade price");
            Assert.AreEqual(s, kt[sym].size, "wrong trade size");
            Assert.IsFalse(kt[sym].isQuote, "quote at startup");
            Assert.IsFalse(kt[sym].hasAsk, "ask at startup");
            Assert.IsTrue(kt[sym].hasBid, "bid at startup");

            // ask
            kt.newTick(TickImpl.NewAsk(sym, p, s));
            // verify ask
            Assert.IsTrue(kt[sym].isValid, "no valid tick at startup");
            Assert.IsTrue(kt[sym].isFullQuote, "no quote at startup");
            Assert.AreEqual(p, kt.Ask(sym), "wrong ask price");
            Assert.AreEqual(ss, kt[sym].os, "wrong ask size");
            Assert.IsTrue(kt[sym].hasAsk, "ask at startup");
            Assert.IsTrue(kt[sym].hasBid, "bid at startup");


            // verify trade still shows
            Assert.IsTrue(kt[sym].isValid, "no valid tick at startup");
            Assert.IsTrue(kt[sym].isTrade, "no trade at startup");
            Assert.AreEqual(p, kt.Last(sym), "wrong trade price");
            Assert.AreEqual(s, kt[sym].size, "wrong trade size");
            Assert.IsFalse(kt[sym].isQuote, "quote at startup");
            Assert.IsTrue(kt[sym].hasAsk, "ask at startup");
            Assert.IsTrue(kt[sym].hasBid, "bid at startup");

            // verify bid still shows
            Assert.IsTrue(kt[sym].isValid, "no valid tick at startup");
            Assert.IsTrue(kt[sym].isFullQuote, "no quote at startup");
            Assert.AreEqual(p, kt.Bid(sym), "wrong bid price");
            Assert.AreEqual(ss, kt[sym].bs, "wrong bid size");
            Assert.IsTrue(kt[sym].hasAsk, "ask at startup");
            Assert.IsTrue(kt[sym].hasBid, "bid at startup");


            

            
        }
    }
}

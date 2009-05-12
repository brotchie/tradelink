using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using NUnit.Framework;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TestTick
    {

        public TestTick() { }

        [Test]
        public void Basics()
        {
            TickImpl t = new TickImpl();
            Assert.That(!t.isValid);
            t.symbol = "IBM";
            t.size = 100;
            t.trade = 1;
            Assert.That(t.isValid);
            Assert.That(t.isTrade);
            Assert.That(!t.isQuote);

            t = new TickImpl("TST");
            t.TradeSize = 100;
            Assert.That(t.TradeSize == t.ts * 100, t.TradeSize.ToString());

            t = new TickImpl("TST");
            t.BidSize = 200;
            Assert.That(t.BidSize == t.bs * 100, t.BidSize.ToString());

            t = new TickImpl("TST");
            t.AskSize = 300;
            Assert.That(t.AskSize == t.os*100, t.AskSize.ToString());
        }

        [Test]
        public void StaticFactories()
        {
            // factory inputs
            const string s = "TST";
            const decimal p = 13m;
            const int z = 400;
            
            // produce a new ask tick
            TickImpl t = TickImpl.NewAsk(s, p, z);
            Assert.That(t.hasAsk && !t.hasBid, t.ToString());
            Assert.That(t.ask==p, t.ask.ToString());
            Assert.That(t.AskSize == z, t.AskSize.ToString());
            Assert.That(t.os == (int)(z / 100), t.os.ToString());
            Assert.That(t.symbol == s);

            // produce bid tick
            t = TickImpl.NewBid(s, p, z);
            Assert.That(t.hasBid && !t.hasAsk, t.ToString());
            Assert.That(t.bid== p, t.bid.ToString());
            Assert.That(t.BidSize == z, t.BidSize.ToString());
            Assert.That(t.bs == (int)(z / 100), t.bs.ToString()); 
            Assert.That(t.symbol == s);

            // produce a trade tick
            t = TickImpl.NewTrade(s, p, z);
            Assert.That(t.isTrade && !t.isQuote, t.ToString());
            Assert.That(t.trade == p, t.trade.ToString());
            Assert.That(t.TradeSize == z, t.TradeSize.ToString());
            Assert.That(t.ts == (int)(z / 100), t.ts.ToString());
            Assert.That(t.symbol == s);



        }

        [Test]
        public void Serialization()
        {
            const string t = "TST";
            const decimal p = 10m;
            const int s = 300;
            const int date = 20080702;
            const int time = 93503;
            TickImpl pre = TickImpl.NewTrade(t, p, s);
            pre.time = time;
            pre.date = date;
            pre.bid = p;
            pre.ask = p;
            pre.os = s;
            pre.bs = s;
            pre.ex = t;
            pre.be = t;
            pre.oe = t;
            pre.depth = 5;
            string serialize = TickImpl.Serialize(pre);
            Tick post = TickImpl.Deserialize(serialize);
            Assert.That(post.time == pre.time, post.time.ToString());
            Assert.That(post.date == pre.date);
            Assert.That(post.bs == pre.bs);
            Assert.That(post.bid == pre.bid);
            Assert.That(post.ask == pre.ask);
            Assert.That(post.os == pre.os);
            Assert.That(post.ex == pre.ex);
            Assert.That(post.be == pre.be);
            Assert.That(post.oe == pre.oe);
            Assert.That(post.depth == pre.depth);
        }


    }
}

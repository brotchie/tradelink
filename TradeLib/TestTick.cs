using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
{
    [TestFixture]
    public class TestTick
    {

        public TestTick() { }

        [Test]
        public void Basics()
        {
            Tick t = new Tick();
            Assert.That(!t.isValid);
            t.sym = "IBM";
            t.size = 100;
            t.trade = 1;
            Assert.That(t.isValid);
            Assert.That(t.isTrade);
            Assert.That(!t.isQuote);
        }

        [Test]
        public void Serialization()
        {
            const string t = "TST";
            const decimal p = 10m;
            const int s = 300;
            const int date = 20080702;
            const int time = 935;
            const int sec = 3;
            Tick pre = Tick.NewTrade(t, p, s);
            pre.time = time;
            pre.date = date;
            pre.sec = sec;
            pre.bid = p;
            pre.ask = p;
            pre.os = s;
            pre.bs = s;
            pre.ex = t;
            pre.be = t;
            pre.oe = t;
            string serialize = pre.Serialize();
            Tick post = Tick.Deserialize(serialize);
            Assert.That(post.time == pre.time, post.time.ToString());
            Assert.That(post.date == pre.date);
            Assert.That(post.sec == pre.sec);
            Assert.That(post.bs == pre.bs);
            Assert.That(post.bid == pre.bid);
            Assert.That(post.ask == pre.ask);
            Assert.That(post.os == pre.os);
            Assert.That(post.ex == pre.ex);
            Assert.That(post.be == pre.be);
            Assert.That(post.oe == pre.oe);
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLink.Common;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TestBar
    {

        public TestBar() { }

        const string sym = "TST";
        const int d = 20070517;
        const int t = 93500;
        const string x = "NYSE";
        TickImpl[] ticklist = new TickImpl[] { 
                TickImpl.NewTrade(sym,d,t,10,100,x),
                TickImpl.NewTrade(sym,d,t+100,10,100,x),
                TickImpl.NewTrade(sym,d,t+200,10,100,x),
                TickImpl.NewTrade(sym,d,t+300,10,100,x),
                TickImpl.NewTrade(sym,d,t+400,15,100,x), 
                TickImpl.NewTrade(sym,d,t+500,16,100,x), 
                TickImpl.NewTrade(sym,d,t+600,16,100,x),
                TickImpl.NewTrade(sym,d,t+700,10,100,x), 
                TickImpl.NewTrade(sym,d,t+710,10,100,x), 
            };

        [Test]
        public void Construction()
        {
            BarImpl b = new BarImpl();
            Assert.That(!b.isValid);
            Assert.That(!b.isNew);
            b.newTick(ticklist[0]);
            Assert.That(b.isValid);
            Assert.That(b.isNew);
            b.newTick(ticklist[1]);
            Assert.That(b.isValid);
            Assert.That(!b.isNew);
            Assert.That(b.Volume == 200);
            b.newTick(TickImpl.NewQuote(sym,d,t,10m,11m,1,1,x,x));
            Assert.That(b.TradeCount == 2);


        }

        [Test]
        public void BarIntervals()
        {


            BarImpl b = new BarImpl(BarInterval.FiveMin);
            int accepts = 0;
            foreach (TickImpl k in ticklist)
                if (b.newTick(k)) accepts++;
            Assert.AreEqual(5, accepts);

            b = new BarImpl(BarInterval.FifteenMin);
            accepts = 0;
            foreach (TickImpl k in ticklist)
                if (b.newTick(k)) accepts++;
            Assert.AreEqual(9, accepts);

            b = new BarImpl(BarInterval.Minute);
            accepts = 0;
            for (int i = 7; i<ticklist.Length; i++)
                if (b.newTick(ticklist[i])) accepts++;
            Assert.AreEqual(2,accepts);

        }

        [Test]
        public void BarTime()
        {
            Bar b = new BarImpl(1, 1, 1, 1, 1, 20100302, 93533, "IBM", (int)BarInterval.FiveMin);
            Assert.AreEqual(93500, b.Bartime);
            Assert.AreEqual(93533, b.time);
            Console.WriteLine(b.Bartime + " " + b.time);

            b = new BarImpl(1, 1, 1, 1, 1, 20100302, 93533, "IBM", (int)BarInterval.Hour);
            Assert.AreEqual(90000, b.Bartime);
            Assert.AreEqual(93533, b.time);
            Console.WriteLine(b.Bartime + " " + b.time);

            b = new BarImpl(1, 1, 1, 1, 1, 20100302, 95504, "IBM", (int)BarInterval.FiveMin);
            Assert.AreEqual(95500, b.Bartime);
            Assert.AreEqual(95504, b.time);
            Console.WriteLine(b.Bartime + " " + b.time);
        }

        [Test]
        public void SerializeDeseralize()
        {
            Bar b = new BarImpl(1, 1, 1, 1, 1, 20100302, 93533, "IBM", (int)BarInterval.FiveMin);
            string msg = BarImpl.Serialize(b);
            Bar cb = BarImpl.Deserialize(msg);

            Assert.AreEqual(b.Symbol, cb.Symbol);
            Assert.AreEqual(b.time, cb.time);
            Assert.AreEqual(b.Interval, cb.Interval);
            Assert.AreEqual(b.High, cb.High);
            Assert.AreEqual(b.Low, cb.Low);
            Assert.AreEqual(b.Open, cb.Open);
            Assert.AreEqual(b.Close, cb.Close);
            Assert.AreEqual(b.Volume, cb.Volume);
            Assert.AreEqual(b.Bardate, cb.Bardate);

        }

        // TestBarImpl
        [Test]
        public void BarsBack()
        {
            DateTime present = DateTime.Parse("2010/1/3 15:00:00");
            DateTime past = BarImpl.DateFromBarsBack(5, BarInterval.FiveMin, present);
            Assert.AreEqual(20100103, Util.ToTLDate(past));
            Assert.AreEqual(143500, Util.ToTLTime(past));

        }
    }
}

using NUnit.Framework;
using TradeLib;
using System;


namespace TestTradeLib
{
    [TestFixture]
    public class TestUtil
    {

        [Test]
        public void TLDatematch()
        {
            int dd = 20070601;
            int m = 20071201;
            int m2 = 20060131;
            Assert.That(Util.TLDateMatch(dd, m, DateMatchType.Year));
            Assert.That(!Util.TLDateMatch(dd, m, DateMatchType.Month));
            Assert.That(!Util.TLDateMatch(dd, m, DateMatchType.None));
            Assert.That(Util.TLDateMatch(dd, m, DateMatchType.Day));
            Assert.That(Util.TLDateMatch(dd, m, DateMatchType.Day | DateMatchType.Year));
            Assert.That(!Util.TLDateMatch(dd, m, DateMatchType.Day | DateMatchType.Month));
            Assert.That(!Util.TLDateMatch(dd, m2, DateMatchType.Day | DateMatchType.Month | DateMatchType.Year));
        }
        int tldate = 20070731;
        int tltime1 = 931;
        int tltime2 = 1400;
        int tlsec = 45;
        DateTime d;

        [Test]
        public void Date()
        {
            d = Util.ToDateTime(tldate);
            Assert.That(d.Year == 2007);
            Assert.That(d.Month == 7);
            Assert.That(d.Day == 31);
            d = Util.ToDateTime(tltime1, 0);
            Assert.That(d.Hour == 9);
            Assert.That(d.Minute == 31);
            Assert.That(d.Second == 0);
            d = Util.ToDateTime(tldate, tltime2, tlsec);
            Assert.That(d.Year == 2007);
            Assert.That(d.Month == 7);
            Assert.That(d.Day == 31);
            Assert.That(d.Hour == 14);
            Assert.That(d.Minute == 00);
            Assert.That(d.Second == 45);
        }
        [Test]
        [ExpectedException("System.Exception")]
        public void DateException()
        {
            d = Util.ToDateTime(0);
        }
    }
}

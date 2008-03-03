using NUnit.Framework;
using TradeLib;
using System;


namespace TestTradeLib
{
    [TestFixture]
    public class TestUtil
    {
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

using System;
using TradeLib;
using NUnit.Framework;

namespace TestTradeLib
{
    [TestFixture]
    public class TestAccount
    {
        public TestAccount() { }

        [Test]
        public void Basics()
        {
            Account a = new Account();
            Assert.That(!a.isValid);
            const string myid = "jfranta";
            a = new Account(myid);
            Assert.That(a.isValid);
            Assert.That(a.ID == myid);
        }

    }
}

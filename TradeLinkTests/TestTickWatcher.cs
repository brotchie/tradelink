using System;
using NUnit.Framework;
using TradeLink.Common;
using TradeLink.API;

namespace TestTradeLink
{
    [TestFixture]
    public class TestTickWatcher
    {
        int alertssent = 0;
        public TestTickWatcher()
        {
        }

        [Test]
        public void WatcherTest()
        {
            // create a new tick watcher
            TickWatcher tw = new TickWatcher();
            tw.Alerted += new SecurityDelegate(test_alert);
            const string sym = "TST";
            const int y = 2008;
            const int m = 1;
            const int d = 1;
            int date = Util.ToTLDate(new DateTime(y, m, d));
            TickImpl t = TickImpl.NewTrade(sym,date,130000,100m,100,"");
            // watch this stock and supply a watch time
            // we have no previous updates to no whether to alert on, 
            // so it returns true
            Assert.That(tw.Watch(t, 300));
            tw.DefaultWait = 300;
            Assert.That(tw.DefaultWait == 300);
            TickImpl t2 = TickImpl.Copy(t);
            t2.time = 130458;
            // this should succeed bc it's within the window (but no alert sent)
            Assert.IsTrue(tw.Watch(t2));

            // this time check should send no alerts bc it's w/in window
            tw.SendAlerts(new DateTime(y,m,d, 13, 7, 0));

            TickImpl t3 = TickImpl.Copy(t2);
            t3.time = 131000;
            // this should return false and send an alert
            Assert.That(!tw.Watch(t3));

            // this timecheck is outside the window, should alert
            DateTime iswaylate = new DateTime(y,m,d, 14, 0, 1);
            tw.SendAlerts(iswaylate);

            Assert.AreEqual(2,alertssent); // here's our alert check
        }
        public void test_alert(Security s)
        {
            alertssent++;
        }
    }
}

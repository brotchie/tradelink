using System;
using NUnit.Framework;
using TradeLib;

namespace TestTradeLib
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
            tw.Alerted += new StockDelegate(test_alert);
            const string sym = "TST";
            const int y = 2008;
            const int m = 1;
            const int d = 1;
            int date = Util.ToTLDate(new DateTime(y, m, d));
            Tick t = Tick.NewTrade(sym,date,1300,0,100m,100,"");
            // watch this stock and supply a watch time
            // we have no previous updates to no whether to alert on, 
            // so it returns true
            Assert.That(tw.Watch(t, 300));
            tw.DefaultWait = 300;
            Assert.That(tw.DefaultWait == 300);
            Tick t2 = new Tick(t);
            t2.time = 1304;
            t2.sec = 58;
            // this should succeed bc it's within the window
            Assert.That(tw.Watch(t2));

            // this time check should send no alerts bc it's w/in window
            tw.SendAlerts(new DateTime(y,m,d, 13, 7, 0));

            Tick t3 = new Tick(t2);
            t3.time = 1310;
            // this should return false and send an alert
            Assert.That(!tw.Watch(t3));

            // this timecheck is outside the window, should alert
            DateTime iswaylate = new DateTime(y,m,d, 14, 0, 0);
            tw.SendAlerts(iswaylate);

            Assert.That(alertssent == 2); // here's our alert check
        }
        public void test_alert(Stock s)
        {
            alertssent++;
            Console.WriteLine(s.Symbol + " is overdue.");
        }
    }
}

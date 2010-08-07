using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using TradeLink.API;
using TradeLink.Common;


namespace TestTradeLink
{
    [TestFixture]
    public class TestTradeLink_IP
    {
        // each side of our "link"
        TLClient c;
        TLClient c2;
        TLServer_IP s;

        // counters used to test link events are working
        int ticks;
        int fills;
        int orders;
        int fillrequest;
        int imbalances;

        // 2nd client counters
        int copyticks;
        int copyfills;
        int copyorders;

        const string SYM = "TST";

        public TestTradeLink_IP() 
        {
            start();

        }

        void start()
        {
            s = new TLServer_IP(IPUtil.LOCALHOST, IPUtil.TLDEFAULTTESTPORT, 0, 100000);
            s.SendDebugEvent += new DebugDelegate(debug);
            s.Start();

            // make sure we select our own loopback, if other servers are running
            c = new TLClient_IP(TLClient_IP.GetEndpoints(IPUtil.TLDEFAULTTESTPORT,new string[] { System.Net.IPAddress.Loopback.ToString() }),0,"tlclient",0,0,debug);


            // create a second client to verify order and fill copying work
            //c2 = new TLClient_IP(TLClient_IP.GetEndpoints(IPUtil.TLDEFAULTTESTPORT, new string[] { System.Net.IPAddress.Loopback.ToString() }), 0, "tlclient2", 0, 0, debug);


            // register server events (so server can process orders)
            s.newSendOrderRequest += new OrderDelegateStatus(tl_gotSrvFillRequest);

            // setup client events
            c.gotFill += new FillDelegate(tlclient_gotFill);
            c.gotOrder += new OrderDelegate(tlclient_gotOrder);
            c.gotTick += new TickDelegate(tlclient_gotTick);
            c.gotImbalance += new ImbalanceDelegate(c_gotImbalance);
            c.gotFeatures += new MessageTypesMsgDelegate(c_gotFeatures);
            // setup second client events to check copying
            //c2.gotFill += new FillDelegate(c2_gotFill);
            //c2.gotOrder += new OrderDelegate(c2_gotOrder);
            //c2.gotTick += new TickDelegate(c2_gotTick);
        }

        MessageTypes[] cfeatures = new MessageTypes[0];
        void c_gotFeatures(MessageTypes[] messages)
        {
            cfeatures = messages;
        }

        void debug(string msg)
        {
            Console.WriteLine(msg);
        }

        void s_SendDebugEvent(string msg)
        {
            debug(msg);
        }


        [TestFixtureSetUp]
        [Test]
        public void StartupTests()
        {

            // discover our states
            Providers[] p = c.ProvidersAvailable;
            Assert.Greater(p.Length, 0);
            Assert.GreaterOrEqual(c.ProviderSelected, 0);
            Assert.AreEqual(Providers.TradeLink, p[c.ProviderSelected]);
            debug("done startup");
        }

        
        [Test]
        public void FeatureTests()
        {
            sleep(200);
            Assert.GreaterOrEqual(cfeatures.Length, 1);
            debug("done features");
        }

        void sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }

        [Test]
        public void TickTests()
        {
            // havent' sent any ticks, so shouldn't have any counted
            Assert.That(ticks == 0, ticks.ToString());

            // have to subscribe to a stock to get notified on fills for said stock
            c.Subscribe(new BasketImpl(new SecurityImpl(SYM)));

            //send a tick from the server
            TickImpl t = TickImpl.NewTrade(SYM, 10, 100);
            s.newTick(t);

            sleep(150);
            // make sure the client got it
            Assert.That(ticks == 1, ticks.ToString());
            // make sure other clients did not get ticks 
            // (cause we didnt' subscribe from other clients)
            Assert.AreNotEqual(copyticks, ticks);

        }



        const int TICKSENT = 5000;
        
        [Test]
        public void TickPerformance()
        {

            // expected performance
            const decimal EXPECT = .8m;
            // get ticks for test
            
            Tick[] tick = TradeLink.Research.RandomTicks.GenerateSymbol(SYM, TICKSENT);
            // subscribe to symbol
            c.Unsubscribe();
            c.Subscribe(new BasketImpl(SYM));
            // reset ticks
            int save = ticks;
            ticks = 0;
            // start clock
            DateTime st = DateTime.Now;

            // process ticks
            for (int i = 0; i < tick.Length; i++)
                s.newTick(tick[i]);

            sleep(200);
            // stop clock
            double time = DateTime.Now.Subtract(st).TotalSeconds;
            decimal ticksec = TICKSENT / (decimal)time;
            // make sure time exists
            Assert.Greater(time, 0);
            // make sure it's less than expected
            Assert.LessOrEqual(time, EXPECT);
            // make sure we got all the ticks
            Assert.AreEqual(TICKSENT, ticks);
            Assert.AreEqual(0, copyticks);
            debug("protocol performance (tick/sec): " + ticksec.ToString("N0"));

            // restore ticks
            ticks = save;
        }

        [TestFixtureTearDown]
        public void StopTests()
        {
            stop();
        }

        void stop()
        {
            try
            {
                c.Stop();
                c.Stop();
                s.Stop();
            }
            catch (Exception ex)
            {
                debug("error stopping TL_IP tests... " + ex.Message + ex.StackTrace);
            }
        }

        // event handlers

        long tl_gotSrvFillRequest(Order o)
        {
            s.newOrder(o);
            fillrequest++;
            return 0;
        }

        void tlclient_gotTick(Tick t)
        {
            ticks++;

        }

        void tlclient_gotOrder(Order o)
        {
            orders++;
        }

        void tlclient_gotFill(Trade t)
        {
            fills++;
        }

        // 2nd client handlers
        void c2_gotTick(Tick t)
        {
            copyticks++;
        }

        void c2_gotOrder(Order o)
        {
            copyorders++; 
        }

        void c2_gotFill(Trade t)
        {
            copyfills++;
        }

        void c_gotImbalance(Imbalance imb)
        {
            imbalances++;
        }
    }
}

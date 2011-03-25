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
        // max time (sec) for all ip tests
        const int MAXTIMESEC = 15;

        // each side of our "link"
        TLClient_IP c;
        TLClient c2;
        TLServer_IP s;

        // counters used to test link events are working
        int ticks = 0;
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
            debug("starting setup...");
            s = new TLServer_IP(IPUtil.LOCALHOST, IPUtil.TLDEFAULTTESTPORT, 0, 100000);
            s.VerboseDebugging = true;
            s.SendDebugEvent += new DebugDelegate(debug);
            s.newProviderName = Providers.TradeLink;
            s.Start(5,200,true);
            Assert.IsTrue(s.isStarted);
            // make sure we select our own loopback, if other servers are running
            c = new TLClient_IP(TLClient_IP.GetEndpoints(s.Port,new string[] { System.Net.IPAddress.Loopback.ToString() }),0,"tlclient",0,0,debugc,true);
            c.VerboseDebugging = true;

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
            c.GotConnectEvent += new Int32Delegate(c_GotConnectEvent);
            c.GotDisconnectEvent += new Int32Delegate(c_GotDisconnectEvent);
            
            
            // setup second client events to check copying
            //c2.gotFill += new FillDelegate(c2_gotFill);
            //c2.gotOrder += new OrderDelegate(c2_gotOrder);
            //c2.gotTick += new TickDelegate(c2_gotTick);
            debug("ending setup.");
            startt = DateTime.Now;

            // prepare to playback ticks continuously
            bw = new System.ComponentModel.BackgroundWorker();
            bw.DoWork += new System.ComponentModel.DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();
            debug("waiting for helper to start...");
            while (!_looping)
                sleep(50);
        }
        System.ComponentModel.BackgroundWorker bw;

        bool up = false;
        bool downspecialhit = false;
        bool checkdownspecial = false;
        void c_GotDisconnectEvent(int val)
        {
            up = false;
            if (checkdownspecial)
                downspecialhit = true;
            
            debug("C disconnect. ");
        }

        void c_GotConnectEvent(int val)
        {
            up = true;
            debug("C connect. ");
        }

        MessageTypes[] cfeatures = new MessageTypes[0];
        void c_gotFeatures(MessageTypes[] messages)
        {
            cfeatures = messages;
        }

        void debugc(string msg)
        {
            debug("C " + msg);
        }

        string ts
        {
            get
            {
                DateTime dt = DateTime.Now;
                return Util.ToTLTime(dt) + "." + dt.Millisecond;
            }
        }

        void debug(string msg)
        {
            
            Console.WriteLine(ts+": "+msg);
        }

        void s_SendDebugEvent(string msg)
        {
            debug(msg);
        }

        DateTime startt ;


        
        [Test]
        public void FeatureTests()
        {
            debug("begin features");
            sleep(200);
            Providers[] p = c.ProvidersAvailable;
            Assert.Greater(p.Length, 0);
            Assert.GreaterOrEqual(c.ProviderSelected, 0);
            Assert.AreEqual(Providers.TradeLink, p[c.ProviderSelected]);
            Assert.GreaterOrEqual(cfeatures.Length, 1,features());
            debug("done features");
        }

        string features()
        {
            string s = string.Empty;
            foreach (MessageTypes mt in cfeatures)
                s += mt.ToString() + " ";
            return s;
        }

        void sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }


        DateTime st;
        const int TICKSENT = 5000;
        // netstat -n -p TCP 1
        [Test]
        public void TickPerformance()
        {
            debug("begin tick performance");
            // expected performance
            const decimal EXPECT = .8m;
            SLEEP = 0;

           
            // subscribe to symbol
            c.Subscribe(new BasketImpl(SYM));
            // start clock
            st = DateTime.Now;

            // wait for some to arrive
            // reset ticks
            ticks = 0;
            sentticks = 0;
            while ((ticks < TICKSENT) && _runtest)
                sleep(1);
            // stop clock
            double time = DateTime.Now.Subtract(st).TotalSeconds;
            decimal ticksec = time==0 ? 0 : ticks / (decimal)time;
            // make sure time exists
            Assert.Greater(time, 0);
            // make sure it's less than expected
            Assert.LessOrEqual(time, EXPECT);

            Assert.AreEqual(0, copyticks);
            debug("end protocol performance (tick/sec): " + ticksec.ToString("N0")+" got/sent: "+ticks+"/"+sentticks);
            c.Subscribe(new BasketImpl());
        }

        [Test]
        public void ServerDisconnect()
        {
            debug("begin test: serverdisconnect");
            _runtest = true;
            SLEEP = 1;

            // reset ticks
            ticks = 0;
            up = true;
            checkdownspecial = false;
            downspecialhit = false;
            // subscribe to symbol
            c.Subscribe(new BasketImpl(SYM));
            // reset ticks
            ticks = 0;
            sentticks = 0;
            // wait for ticks to arrive
            while (_runtest && (!up || (ticks == 0)))
            {
                sleep(50);
            }
            // ensure some ticks arrived
            Assert.IsTrue(up, ts);
            Assert.Greater(ticks, 0,ts);
            
            // disconnect on server side
            debug("sending disconnect... waiting for detection.");
            checkdownspecial = true;
            s.SrvClearClient(c.Name,false);
            // wait until failure detected on client
            while (!downspecialhit && _runtest)
            {
                sleep(10);
            }
            debug("disconnect detected... waiting for recovery...");
            // wait until reconnect
            while (!up && _runtest)
            {
                sleep(10);
            }
            
            // verify up
            Assert.IsTrue(up,ts);
            // verify ticks are still being sent
            Assert.IsTrue(_runtest);
            //debug("connection is back up... waiting for more ticks...");
            // save ticks
            int lastticks = ticks;
            // wait for more ticks to arrive
            const decimal FAILMULT = 2;
            decimal WAIT = (ticks*FAILMULT) ;
            while (_runtest && (ticks < WAIT))
            {
                if (sentticks % 100 == 0)
                {
                    //debug("ticks sent: " + sentticks + " recv: " + ticks);
                }
                sleep(50);
            }
            // ensure more ticks have arrived
            Assert.GreaterOrEqual(ticks, WAIT);
            // stop test
            debug("end serverdisconnect.  beforefail: "+lastticks+" total: "+ticks);
            c.Subscribe(new BasketImpl());
            
        }

        [Test]
        public void ClientDisconnect()
        {
            debug("begin test: clientdisconnect");
            _runtest = true;
            SLEEP = 1;

            // reset ticks
            ticks = 0;
            up = true;
            checkdownspecial = false;
            downspecialhit = false;
            // subscribe to symbol
            c.Subscribe(new BasketImpl(SYM));
            // reset ticks
            ticks = 0;
            sentticks = 0;

            // wait for ticks to arrive
            while (_runtest && (!up || (ticks == 0)))
            {
                sleep(50);
            }
            // ensure some ticks arrived
            Assert.IsTrue(up, ts);
            Assert.Greater(ticks, 0, ts);

            // disconnect on server side
            debug("sending disconnect... waiting for detection.");
            checkdownspecial = true;
            c.Disconnect(false);
            
            // wait until failure detected on client
            while (!downspecialhit && _runtest)
            {
                sleep(10);
                checkmaxtime();
            }
            debug("disconnect detected... waiting for recovery...");
            // wait until reconnect
            while (!up)
            {
                sleep(10);
                checkmaxtime();
            }

            // verify up
            Assert.IsTrue(up, ts);
            // verify ticks are still being sent
            Assert.IsTrue(_runtest);
            //debug("connection is back up... waiting for more ticks...");
            // save ticks
            int lastticks = ticks;
            // wait for more ticks to arrive
            const decimal FAILMULT = 2;
            decimal WAIT = (ticks * FAILMULT);
            while (_runtest && (ticks < WAIT))
            {
                if (sentticks % 100 == 0)
                {
                    //debug("ticks sent: " + sentticks + " recv: " + ticks);
                }
                sleep(50);
            }
            // ensure more ticks have arrived
            Assert.GreaterOrEqual(ticks, WAIT);
            // stop test
            debug("end serverdisconnect.  beforefail: " + lastticks + " total: " + ticks);
            c.Subscribe(new BasketImpl());

        }

        Tick[] tick = TradeLink.Research.RandomTicks.GenerateSymbol(SYM, TICKSENT);

        bool _runtest = true;
        int sentticks = 0;
        int SLEEP = 0;
        bool _looping = false;
        void bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            _runtest = true;
            // send ticks continuously while test is running
            try
            {
                debug("starting helper sending ticks...");


                int i = 0;
                while (_runtest)
                {
                    _looping = true;
                    Tick k = tick[i++];
                    if (i >= tick.Length)
                        i= 0;
                    
                    s.newTick(k);
                    sentticks++;
                    if (SLEEP!=0)
                        sleep(SLEEP);
                    if (i % 250 == 0)
                    {
                        checkmaxtime();
                    }
                }
                debug("completed helper sending ticks");
            }
            catch (Exception ex)
            {
                debug("got error running test thread." + ex.Message + ex.StackTrace);
                _runtest = false;
            }
        }

        void checkmaxtime()
        {
            if (DateTime.Now.Subtract(startt).TotalSeconds > MAXTIMESEC)
            {
                _runtest = false;
            }
        }

        [TestFixtureTearDown]
        public void StopTests()
        {
            debug("begin stop tests");
            stop();
            debug("end stop tests.");
        }

        void stop()
        {

            
            try
            {
                _runtest = false;
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
            //debug("recv: " + t);
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

using System;
using System.Collections.Generic;
using TradeLink.API;
using TradeLink.Common;
using NUnit.Framework;

namespace TestTradeLink
{
    [TestFixture]
    public class TestAsyncResponse
    {
        public TestAsyncResponse()
        {
            ar.GotTick += new TickDelegate(ar_GotTick);
            ar.GotTickQueueEmpty += new VoidDelegate(ar_GotTickQueueEmpty);
            ar.GotTickOverrun += new VoidDelegate(ar_GotTickOverrun);
            ar.GotImbalanceQueueEmpty += new VoidDelegate(ar_GotImbalanceQueueEmpty);
            ar.GotImbalance += new ImbalanceDelegate(ar_GotImbalance);
            ar.GotBadTick += new VoidDelegate(ar_GotBadTick);
            ar.GotImbalanceOverrun += new VoidDelegate(ar_GotImbalanceOverrun);
        }

        void ar_GotImbalanceOverrun()
        {
            debug("imbalance overrun");
        }

        void ar_GotTickOverrun()
        {
            debug("tick overrun");
        }

        void ar_GotBadTick()
        {
            debug("null tick, read: " + ar.BadTickRead + " written: " + ar.BadTickWritten);
        }




        bool tickdone = false;
        void ar_GotTickQueueEmpty()
        {
            tickdone = tc >= MAXTICKS;
        }

        bool imbdone = false;
        void ar_GotImbalanceQueueEmpty()
        {
            imbdone = ic >= MAXIMBS;
        }


        const int MAXTICKS = 20000;
        const int MAXIMBS = 1000;
        const string SYM = "TST";
        AsyncResponse ar = new AsyncResponse();
        const int MAXWAITS = 100;

        [Test]
        public void TickTest()
        {
            // get ticks
            Tick[] sent = 
                TradeLink.Research.RandomTicks.GenerateSymbol(SYM, MAXTICKS);

            // send ticks
            for (int i = 0; i < sent.Length; i++)
                ar.newTick(sent[i]);

            int waits = 0;
            bool testtimeout = false;
            // wait for reception
            while (!tickdone)
            {
                System.Threading.Thread.Sleep(10);
                if (waits++ % 50 == 0)
                {
                    if (ar.BadTickWritten + ar.BadTickRead > 0)
                        break;
                    //System.Diagnostics.Debugger.Break();
                    Console.WriteLine(string.Format("waits: {0} tickcount: {1}", waits, tc));
                }
                if ((waits * 10) > sent.Length)
                {
                    testtimeout = true;
                    break;
                }
            }

            Assert.AreEqual(0, ar.BadTickRead,"null ticks read");
            Assert.AreEqual(0, ar.BadTickWritten, "null ticks written");
            Assert.AreEqual(0, ar.TickOverrun,"tick overrun");

            // verify done
            string reason = testtimeout ? "TICKTESTTIMEOUT " : string.Empty;
            reason += tc.ToString() + " ticks recv/"+MAXTICKS.ToString();
            Assert.IsTrue(tickdone, reason);


            //verify count
            Assert.AreEqual(MAXTICKS, tc);

            // verify order
            Assert.IsTrue(torder);
        }

        [Test]
        public void ImbalanceTest()
        {
            // get imbalances
            Random r =new Random((int)DateTime.Now.Ticks);
            List<Imbalance> sent = new List<Imbalance>();
            for (int i = 0; i < MAXIMBS; i++)
                sent.Add(new ImbalanceImpl(SYM, "NYSE", r.Next(-1000000, 1000000), i, 0, 0, 0));

            // send imbalances
            for (int i = 0; i < sent.Count; i++)
                ar.newImbalance(sent[i]);

            // wait for reception
            int waits = 0;
            bool testtimeout = false;
            // wait for reception
            while (!imbdone)
            {
                System.Threading.Thread.Sleep(10);
                if ((waits++ % 50) == 0)
                {
                    if (ar.BadTickRead + ar.BadTickWritten > 0)
                        break;
                    //System.Diagnostics.Debugger.Break();
                    Console.WriteLine(string.Format("waits: {0} tickcount: {1}", waits, tc));
                }
                if ((waits * 10) > sent.Count)
                {
                    testtimeout = true;
                    break;
                }

            }

            // verify no null imbalances
            Assert.AreEqual(0, ar.BadImbalanceRead,"null imbalances read");
            Assert.AreEqual(0, ar.BadImbalanceWritten, "null imbalances written");

            //verify no overruns
            Assert.AreEqual(0, ar.ImbalanceOverrun,"imbalance overrun");
            string reason = testtimeout ? "IBMTESTTIMEOUT " : string.Empty;
            reason += tc.ToString() + " imbalances recv/" + MAXIMBS.ToString();
            // verify done
            Assert.IsTrue(imbdone, reason);

            // verify count
            Assert.AreEqual(MAXIMBS, ic);

            // verify order
            Assert.IsTrue(iorder);

        }

        Tick[] trecv = new Tick[MAXTICKS];
        Imbalance[] irecv = new Imbalance[MAXIMBS];

        int ic = 0;
        int tc = 0;
        bool iorder = true;
        bool torder = true;
        int lastt = 0;
        int lasti = 0;

        void ar_GotImbalance(Imbalance imb)
        {
            try
            {
                bool v = (lasti <= imb.ThisTime);
                if (!v)
                    iorder = false;
                irecv[ic++] = imb;
                lasti = imb.ThisTime;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ic++;
            }
        }

        void ar_GotTick(Tick t)
        {
            try
            {
                bool v = true;
                v = (lastt <= t.time);
                if (!v)
                    torder = false;
                trecv[tc++] = t;
                lastt = t.time;
                tickdone = tc >= MAXTICKS;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                tc++;
            }

        }

        void debug(string msg)
        {
            Console.WriteLine(msg);
        }

        [TestFixtureTearDown]
        public void StopTest()
        {
            ar.Stop();
        }
    }
}

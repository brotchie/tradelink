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
            ar.GotImbalance += new ImbalanceDelegate(ar_GotImbalance);
        }


        const int MAXTICKS = 20000;
        const int MAXIMBS = 1000;
        const string SYM = "TST";
        AsyncResponse ar = new AsyncResponse();


        [Test]
        public void TickTest()
        {
            // get ticks
            Tick[] sent = 
                TradeLink.Research.RandomTicks.GenerateSymbol(SYM, MAXTICKS);

            // send ticks
            for (int i = 0; i < sent.Length; i++)
                ar.newTick(sent[i]);

            // wait for reception
            System.Threading.Thread.CurrentThread.Join(250);

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
            System.Threading.Thread.CurrentThread.Join(250);

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
            bool v = (lasti <= imb.ThisTime);
            if (!v)
                iorder = false;
            irecv[ic++] = imb;
            lasti = imb.ThisTime;
        }

        void ar_GotTick(Tick t)
        {
            bool v = (lastt <= t.time);
            if (!v)
                torder = false;
            trecv[tc++] = t;
            lastt = t.time;
        }

        [TestFixtureTearDown]
        public void StopTest()
        {
            ar.Stop();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLink.Common;
using TradeLink.API;
using TradeLink.Research;
using System.Runtime.InteropServices;

namespace TestTradeLink
{
    [TestFixture(Description="May not work on machines without brokerserver installed to default location."),Explicit]
    public class TestTLFastInterop
    {
        [DllImport("c:\\program files\\tradelink\\TradeLibFast.dll",EntryPoint="TLSENDORDER")]
        public static extern int SendOrder(string sym, bool side, int size, double limit, double stop, int id, string account, string dest);
        

        [Test]
        public void OrderPrecision()
        {
            // prepare random
            Random r = new Random((int)DateTime.Now.Ticks);
            // get a price
            double p = r.NextDouble() * 1000000 / 100;
            // send the order
            int err = SendOrder("TST", true, 300, p, 0, 1, string.Empty, string.Empty);
            // make sure it was received
            Assert.AreEqual((int)MessageTypes.OK, err);
            // verify price to 5 decimal places
            double expect = Math.Round(p, 5);
            double actual = (double)Math.Round(last.price, 5);
            Assert.AreEqual(expect, actual,last.ToString() );

        }

        [Test]
        public void InterOpSpeed()
        {
            const int TESTSIZE = 100;
            count = 0;
            // get some random prices
            Tick[] data = RandomTicks.GenerateSymbol(sym, TESTSIZE);
            // track the time
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            int bad = 0;
            sw.Start();
            for (int i = 0; i < data.Length; i++)
            {
                int err = SendOrder(sym, true, 300, (double)data[i].trade, 0, 1, "TESTACCOUNT", "TESTDEST");
                if (err!=0) bad++;
            }
            sw.Stop();
            long elapms = sw.ElapsedMilliseconds;
            double elap = (double)elapms/1000;
            double rate = TESTSIZE / elap;
            Console.WriteLine("InterOpSpeed elap: " + elap.ToString("N1") + " rate: " + rate.ToString("N0") + " orders/sec");
            // make sure orders received
            Assert.AreEqual(data.Length, count);
            // make sure no bad orders
            Assert.AreEqual(0,bad);
            // make sure fast
            Assert.LessOrEqual(elap, .5);
        }

        Order last = new OrderImpl();
        int count = 0;

        long tl_newSendOrderRequest(Order o)
        {
            last = o;
            count++;
            return 0;
  
        }
        const string sym = "TST";
        public TestTLFastInterop()
        {
            // start a server
            TLServer_WM tl = new TLServer_WM();
            tl.newProviderName = Providers.TradeLink;
            // handle new orders
            tl.newSendOrderRequest += new OrderDelegateStatus(tl_newSendOrderRequest);

        }
    }


}

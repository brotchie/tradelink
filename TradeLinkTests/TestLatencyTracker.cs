using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLink.Common;
using TradeLink.API;


namespace TestTradeLink
{
    [TestFixture]
    public class TestLatencyTracker
    {
        LatencyTracker lt = new LatencyTracker();
        const string sym = "IBM";
        const int size = 100;
        [Test]
        public void Test()
        {
            // reset measurements
            reset();

            // create tests
            long id = 1;
            Order o = new BuyMarket(sym, 2*size);
            o.id = id++;
            // start measuring
            lt.sendorder(o);
            // ack
            lt.GotOrder(o);
            // ensure we got a measurement
            Assert.IsTrue(gotmeasurement(),mez);
            // reset measurements
            reset();

            // fill
            o.Fill(TickImpl.NewTrade(sym, 100, size));
            Trade t = (Trade)o;
            lt.GotFill(t);
            // ensure we got a measurement
            Assert.IsTrue(gotmeasurement(),mez);
            // reset measurements
            reset();

            // cancel
            lt.sendcancel(o.id);
            System.Threading.Thread.Sleep(50);
            // ack cancel
            lt.GotCancel(o.id);
            // ensure we got a measurement
            Assert.IsTrue(gotmeasurement(),mez);

        }

        bool gotmeasurement()
        {
            return lastid * (int)lasttype != 0;
        }
        string mez { get { return lastlatency + " " + lastid + " " + lasttype.ToString(); } }
        void reset()
        {
            lastlatency = 0;
            lastid = 0;
            lasttype = MessageTypes.OK;
        }

        public TestLatencyTracker() 
        {
            lt.SendLatency += new LatencyDelegate(lt_SendLatency);
        }
        double lastlatency = 0;
        MessageTypes lasttype = MessageTypes.OK;
        long lastid = 0;
        void lt_SendLatency(MessageTypes type, long id, double time)
        {
            lastid = id;
            lasttype = type;
            lastlatency = time;
        }
    }
}

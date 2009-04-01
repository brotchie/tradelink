using System;
using System.Collections.Generic;
using TradeLink.API;
using System.Text;
using NUnit.Framework;
using TradeLink.Common;

namespace TestTradeLink
{
    [TestFixture]
    public class TestOffsetTracker
    {
        public TestOffsetTracker() 
        {
            ot.SendCancel += new UIntDelegate(ot_SendCancel);
            ot.SendOffset += new OrderDelegate(ot_SendOffset);
        }

        OffsetTracker ot = new OffsetTracker();
        List<Order> profits = new List<Order>();
        List<Order> stops = new List<Order>();

        [Test]
        public void Basics()
        {
            const string SYM = "TST";
            const decimal PRICE = 100;
            const decimal POFFSET = .2m;
            const decimal SOFFSET = .1m;
            const int SIZE = 300;
            // make sure offsets don't exist
            Assert.AreEqual(0,profits.Count);
            Assert.AreEqual(0,stops.Count);
            // setup offset defaults
            ot.DefaultOffset = new OffsetInfo(POFFSET,SOFFSET);
            // send position update to generate offsets
            ot.UpdatePosition(new PositionImpl(SYM, PRICE,SIZE));
            // verify orders exist
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            Order profit = profits[0];
            Order stop = stops[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE + POFFSET, profit.price);
            Assert.AreEqual(SIZE, profit.UnsignedSize);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE - SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE, stop.UnsignedSize);



            // send position update
            ot.UpdatePosition(new TradeImpl(SYM, PRICE+2, SIZE));
            // verify only one order exists
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            profit = profits[0];
            stop = stops[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE +1+ POFFSET, profit.price);
            Assert.AreEqual(SIZE*2, profit.UnsignedSize);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE +1- SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE*2, stop.UnsignedSize);

            // partial hit the profit order
            ot.UpdatePosition(new TradeImpl(SYM, PRICE + 1, -1 * SIZE));
            // verify only one order exists on each side
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            profit = profits[0];
            stop = stops[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE + 1 + POFFSET, profit.price);
            Assert.AreEqual(SIZE, profit.UnsignedSize);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + 1 - SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE, stop.UnsignedSize);

        }

        void ot_SendOffset(Order o)
        {
            if (o.isLimit)
                profits.Add(o);
            else if (o.isStop)
                stops.Add(o);
        }

        void ot_SendCancel(uint number)
        {
            
            for (int i = profits.Count - 1; i >= 0; i--)
                if (profits[i].id == number)
                    profits.RemoveAt(i);
            for (int i = stops.Count - 1; i >= 0; i--)
                if (stops[i].id == number)
                    stops.RemoveAt(i);


            ot.GotCancel(number);
        }
    }
}

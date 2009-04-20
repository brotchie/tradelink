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
        public void IgnoreByDefault()
        {
            // ignore all symbols by default, unless custom defined
            ot.IgnoreDefault = true;
            // reset "book"
            profits.Clear();
            stops.Clear();
            // reset offsets
            ot.ClearCustom();
            // make sure offsets don't exist
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(0, stops.Count);
            // send position update to generate offsets
            ot.Adjust(new PositionImpl(SYM, PRICE, SIZE));
            // make sure offsets don't exist
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(0, stops.Count);
            // add a custom offset 
            ot[SYMB] = SampleOffset();
            // send position update to generate offsets
            ot.Adjust(new PositionImpl(SYMB, PRICE, SIZE));
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

        }

        [Test]
        public void CustomOffsets()
        {

            // get default offset
            ot.DefaultOffset = SampleOffset();
            // verify that random symbol has default values
            Assert.AreEqual(ot.DefaultOffset.ProfitPercent, ot[SYMB].ProfitPercent);
            Assert.AreEqual(ot.DefaultOffset.StopPercent, ot[SYMB].StopPercent);
            Assert.AreEqual(ot.DefaultOffset.ProfitDist, ot[SYMB].ProfitDist);
            Assert.AreEqual(ot.DefaultOffset.StopDist, ot[SYMB].StopDist);
            // add custom offset different than default
            ot[SYMB] = new OffsetInfo(POFFSET * 2, SOFFSET * 2, .5m, .5m,true,100);
            // verify custom has taken effect
            Assert.AreEqual(ot.DefaultOffset.ProfitPercent/2, ot[SYMB].ProfitPercent);
            Assert.AreEqual(ot.DefaultOffset.StopPercent/2, ot[SYMB].StopPercent);
            Assert.AreEqual(ot.DefaultOffset.ProfitDist*2, ot[SYMB].ProfitDist);
            Assert.AreEqual(ot.DefaultOffset.StopDist*2, ot[SYMB].StopDist);
            // verify another symbol still has default
            Assert.AreEqual(ot.DefaultOffset.ProfitPercent, ot[SYMC].ProfitPercent);
            Assert.AreEqual(ot.DefaultOffset.StopPercent, ot[SYMC].StopPercent);
            Assert.AreEqual(ot.DefaultOffset.ProfitDist, ot[SYMC].ProfitDist);
            Assert.AreEqual(ot.DefaultOffset.StopDist, ot[SYMC].StopDist);


        }

        public static OffsetInfo SampleOffset()
        {
            return new OffsetInfo(POFFSET, SOFFSET);
        }
        const string SYMB = "TST2";
        const string SYMC = "TST3";
        const string SYM = "TST";
        const decimal PRICE = 100;
        const decimal POFFSET = .2m;
        const decimal SOFFSET = .1m;
        const int SIZE = 300;

        [Test]
        public void Basics()
        {
            // reset "book"
            profits.Clear();
            stops.Clear();
            // make sure offsets don't exist
            Assert.AreEqual(0,profits.Count);
            Assert.AreEqual(0,stops.Count);
            // setup offset defaults
            ot.DefaultOffset = SampleOffset();
            // send position update to generate offsets
            ot.Adjust(new PositionImpl(SYM, PRICE,SIZE));
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
            ot.Adjust(new TradeImpl(SYM, PRICE+2, SIZE));
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
            ot.Adjust(new TradeImpl(SYM, PRICE + 1, -1 * SIZE));
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

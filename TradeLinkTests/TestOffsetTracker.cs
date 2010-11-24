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

        }

        void ot_SendDebug(string msg)
        {
            Console.WriteLine(msg);
        }

        void reset()
        {
            ot = new OffsetTracker();
            ot.SendCancelEvent += new LongDelegate(ot_SendCancel);
            ot.SendOrderEvent += new OrderDelegate(ot_SendOffset);
            ot.SendDebug += new DebugDelegate(ot_SendDebug);
            ot.HitOffset += new HitOffsetDelegate(ot_HitOffset);
            profits.Clear();
            stops.Clear();
            _lasthit = 0;
            _sym = string.Empty;
        }

        long _lasthit = 0;
        string _sym = string.Empty;
        void ot_HitOffset(string sym, long id, decimal price)
        {
            debug(sym + " hit offset: " + id);
            _sym = sym;
            _lasthit = id;
        }

        [Test]
        public void ResendTest2()
        {
            // reset "book" to start from scratch
            reset();
            
            // SETOFFSET("IBM", 0.03, 0, 1, 0);
            const string sym = "IBM";
            const decimal pdist = .03m;
            const decimal pct = 1;
            ot[sym] = new OffsetInfo(pdist, 0, pct, 0, false, 1);
            Assert.AreEqual(pdist, ot[sym].ProfitDist);
            Assert.AreEqual(pct, ot[sym].ProfitPercent);
            // entry fill
            // 094508: fill: 20100423,94532,IBM,SELL,10,128.85, 0
            fill(new TradeImpl(sym, 128.85m, -10));
            Assert.AreEqual(-10, ot.PositionTracker[sym].Size);
            // profit 
            // 094508: sent new profit: 634076112353906253  BUY10 IBM@128.82 [] 634076112353906253
            Assert.AreEqual(1, profits.Count);
            Order profit = profits[0];
            Assert.AreEqual(128.82m, profit.price);
            Assert.AreEqual(10, profit.size);
            // fill profit
            // 094609: fill: 20100423,94632,IBM,BUY,10,128.82, 634076112353906253
            // 094609: IBM hit profit: 634076112353906253
            Assert.IsTrue(profit.Fill(TickImpl.NewTrade(sym, 128.82m, 10)));
            Trade profitfill = (Trade)profit;
            fill(profitfill);

            // we're now flat
            // 094609: IBM now flat.
            Assert.IsTrue(ot.PositionTracker[sym].isFlat);
            // tick
            ot.newTick(TickImpl.NewTrade(sym,128.82m,100));
            Assert.AreEqual(0, profits.Count);
            // re-enter
            //094722: fill: 20100423,94746,IBM,SELL,10,128.86, 100947
            fill(new TradeImpl(sym, 128.86m, -10));
            Assert.AreEqual(-10, ot.PositionTracker[sym].Size);
            // we should now have a profit offset
            Assert.AreEqual(1, profits.Count);
            profit = profits[0];
            Assert.AreEqual(128.83m, profit.price);
            Assert.AreEqual(10, profit.size);

        }

        OffsetTracker ot;
        List<Order> profits = new List<Order>();
        List<Order> stops = new List<Order>();

        [Test]
        public void IgnoreByDefault()
        {
            // reset "book"
            reset();
            // ignore all symbols by default, unless custom defined
            ot.IgnoreDefault = true;
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
            // reset "book"
            reset();

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
        public void StopAndProfit()
        {
            // reset "book"
            reset();

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
            // tick
            ot.newTick(nt());
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
            // tick
            ot.newTick(nt());
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

        [Test]
        public void NoResendPartial()
        {
            // reset "book"
            reset();

            // make sure offsets don't exist
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(0, stops.Count);
            // setup offset defaults
            ot.DefaultOffset = SampleOffset();
            // send position update to generate offsets
            ot.Adjust(new PositionImpl(SYM, PRICE, SIZE));
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
            ot.Adjust(new TradeImpl(SYM, PRICE + 2, SIZE));
            // tick
            ot.newTick(nt());
            // verify only one order exists
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            profit = profits[0];
            stop = stops[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE + 1 + POFFSET, profit.price);
            Assert.AreEqual(SIZE * 2, profit.UnsignedSize);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + 1 - SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE * 2, stop.UnsignedSize);

            // get ids before we hit profit
            long pid = profit.id;
            long sid = stop.id;
            // partial hit the profit order
            Trade t = new TradeImpl(SYM, PRICE + 1, -1 * SIZE);
            t.id = pid;
            ot.Adjust(t);
            // tick
            ot.newTick(nt());
            // verify only one order exists on each side
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            profit = profits[0];
            stop = stops[0];
            // verify profit offset and it should be same id
            Assert.AreEqual(pid, profit.id);
            Assert.IsTrue(profit.isValid);
            // verify stop offset (id should change)
            Assert.AreNotEqual(sid, stop.id);
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + 1 - SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE, stop.UnsignedSize);

        }

        [Test]
        public void HitAndResend()
        {
            // reset "book"
            reset();

            // make sure offsets don't exist
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(0, stops.Count);
            // setup offset defaults
            ot.DefaultOffset = SampleOffset();
            // send position update to generate offsets
            ot.Adjust(new PositionImpl(SYM, PRICE, -1 * SIZE));
            // verify orders exist
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            Order profit = profits[0];
            Order stop = stops[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE - POFFSET, profit.price);
            Assert.AreEqual(SIZE, profit.UnsignedSize);
            Assert.IsTrue(profit.side);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE, stop.UnsignedSize);
            Assert.IsTrue(stop.side);



            // send position update
            ot.Adjust(new TradeImpl(SYM, PRICE + 2, -1 * SIZE));
            // tick
            ot.newTick(nt());
            // verify only one order exists
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            profit = profits[0];
            stop = stops[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE + 1 - POFFSET, profit.price);
            Assert.AreEqual(SIZE * 2, profit.UnsignedSize);
            Assert.IsTrue(profit.side);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + 1 + SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE * 2, stop.UnsignedSize);
            Assert.IsTrue(stop.side);

            // fully hit the profit order
            fill(new TradeImpl(SYM, PRICE + 1, SIZE * 2),profit.id);
            // tick
            ot.newTick(nt());
            // verify we're flat 
            Assert.IsTrue(ot.PositionTracker[SYM].isFlat);
            // verify only no order exists on either side
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(0, stops.Count);
            // take new position
            fill(new TradeImpl(SYM, PRICE, -1 * SIZE));
            // verify we're not flat 
            Assert.IsTrue(ot.PositionTracker[SYM].isShort);
            // verify orders exist
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            profit = profits[0];
            stop = stops[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE - POFFSET, profit.price);
            Assert.AreEqual(SIZE, profit.UnsignedSize);
            Assert.IsTrue(profit.side);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE, stop.UnsignedSize);
            Assert.IsTrue(stop.side);

        }

        [Test]
        public void StopAndProfitShort()
        {
            // reset "book"
            reset();

            // make sure offsets don't exist
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(0, stops.Count);
            // setup offset defaults
            ot.DefaultOffset = SampleOffset();
            // send position update to generate offsets
            ot.Adjust(new PositionImpl(SYM, PRICE, -1*SIZE));
            // verify orders exist
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            Order profit = profits[0];
            Order stop = stops[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE - POFFSET, profit.price);
            Assert.AreEqual(SIZE, profit.UnsignedSize);
            Assert.IsTrue(profit.side);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE, stop.UnsignedSize);
            Assert.IsTrue(stop.side);



            // send position update
            ot.Adjust(new TradeImpl(SYM, PRICE + 2, -1*SIZE));
            // tick
            ot.newTick(nt());
            // verify only one order exists
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            profit = profits[0];
            stop = stops[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE + 1 - POFFSET, profit.price);
            Assert.AreEqual(SIZE * 2, profit.UnsignedSize);
            Assert.IsTrue(profit.side);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + 1 + SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE * 2, stop.UnsignedSize);
            Assert.IsTrue(stop.side);

            // partial hit the profit order
            ot.Adjust(new TradeImpl(SYM, PRICE + 1, SIZE));
            // tick
            ot.newTick(nt());
            // verify only one order exists on each side
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            profit = profits[0];
            stop = stops[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE + 1 - POFFSET, profit.price);
            Assert.AreEqual(SIZE, profit.UnsignedSize);
            Assert.IsTrue(profit.side);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + 1 + SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE, stop.UnsignedSize);
            Assert.IsTrue(stop.side);

        }

        [Test]
        public void StopAndProfitShortFlat()
        {
            // reset "book"
            reset();

            // make sure offsets don't exist
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(0, stops.Count);
            // setup offset defaults
            ot.DefaultOffset = SampleOffset();
            // send position update to generate offsets
            ot.Adjust(new PositionImpl(SYM, PRICE, -1 * SIZE));
            // verify orders exist
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            Order profit = profits[0];
            Order stop = stops[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE - POFFSET, profit.price);
            Assert.AreEqual(SIZE, profit.UnsignedSize);
            Assert.IsTrue(profit.side);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE, stop.UnsignedSize);
            Assert.IsTrue(stop.side);



            // send position update
            ot.Adjust(new TradeImpl(SYM, PRICE + 2, -1 * SIZE));
            // tick
            ot.newTick(nt());
            // verify only one order exists
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            profit = profits[0];
            stop = stops[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE + 1 - POFFSET, profit.price);
            Assert.AreEqual(SIZE * 2, profit.UnsignedSize);
            Assert.IsTrue(profit.side);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + 1 + SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE * 2, stop.UnsignedSize);
            Assert.IsTrue(stop.side);

            // fully hit the profit order
            fill(new TradeImpl(SYM, PRICE + 1, SIZE*2),profit.id);
            // tick
            ot.newTick(nt());
            // verify only one order exists on each side
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(0, stops.Count);

        }

        [Test]
        public void StopOnly()
        {
            // reset "book"
            reset();

            // make sure offsets don't exist
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(0, stops.Count);
            // setup offset defaults
            ot.DefaultOffset = new OffsetInfo(0, SOFFSET, 0, 1, false, 1);
            // send position update to generate offsets
            ot.Adjust(new PositionImpl(SYM, PRICE, SIZE));
            // verify orders exist
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            Order stop = stops[0];
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE - SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE, stop.UnsignedSize);



            // send position update
            ot.Adjust(new TradeImpl(SYM, PRICE + 2, SIZE));
            // tick
            ot.newTick(nt());
            // verify only one order exists
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            stop = stops[0];
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + 1 - SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE * 2, stop.UnsignedSize);

            // partial hit the profit order
            ot.Adjust(new TradeImpl(SYM, PRICE +1 - 1, -1 * SIZE));
            // tick
            ot.newTick(nt());

            // verify stop offset
            // get orders
            stop = stops[0];
            Assert.AreEqual(SIZE, ot.PositionTracker[SYM].Size);
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + 1 - SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE, stop.UnsignedSize);


        }

        [Test]
        public void ProfitOnly()
        {
            // reset "book"
            reset();

            // make sure offsets don't exist
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(0, stops.Count);
            // setup offset defaults
            ot.DefaultOffset = new OffsetInfo(POFFSET, 0, 1, 0, false, 1);
            // send position update to generate offsets
            ot.Adjust(new PositionImpl(SYM, PRICE, SIZE));
            // verify orders exist
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(0, stops.Count);
            // get orders
            Order profit = profits[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE + POFFSET, profit.price);
            Assert.AreEqual(SIZE, profit.UnsignedSize);



            // send position update
            ot.Adjust(new TradeImpl(SYM, PRICE + 2, SIZE));
            // tick
            ot.newTick(nt());
            // verify only one order exists
            Assert.AreEqual(1, profits.Count);
            // get orders
            profit = profits[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE + 1 + POFFSET, profit.price);
            Assert.AreEqual(SIZE * 2, profit.UnsignedSize);

            // partial hit the profit order
            ot.Adjust(new TradeImpl(SYM, PRICE + 1, -1 * SIZE));
            // tick
            ot.newTick(nt());
            // verify only one order exists on each side
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(0, stops.Count);
            // get orders
            profit = profits[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE + 1 + POFFSET, profit.price);
            Assert.AreEqual(SIZE, profit.UnsignedSize);

        }

        [Test]
        public void NoSimulataneous()
        {
            // reset "book"
            reset();
            // turn off simulat
            ot.AllowSimulatenousOrders = false;
            // make sure offsets don't exist
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(0, stops.Count);
            // setup offset defaults
            ot.DefaultOffset = SampleOffset();
            // send position update to generate offsets
            ot.Adjust(new PositionImpl(SYM, PRICE, SIZE));
            // verify stop exists
            Assert.AreEqual(0, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // send tick
            ot.newTick(nt());
            // verify stop and profit exist
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
            fill(new TradeImpl(SYM, PRICE + 2, SIZE));
            // tick
            ot.newTick(nt());
            ot.newTick(nt());
            // verify only one order exists
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            profit = profits[0];
            stop = stops[0];
            // verify profit offset
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(PRICE + 1 + POFFSET, profit.price);
            Assert.AreEqual(SIZE * 2, profit.UnsignedSize);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + 1 - SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE * 2, stop.UnsignedSize);

            // partial hit the profit order
            long pid = profit.id;
            fill(new TradeImpl(SYM, PRICE + 1, -1 * SIZE),profit.id);
            // tick
            ot.newTick(nt());
            ot.newTick(nt());
            // verify only one order exists on each side
            Assert.AreEqual(1, profits.Count);
            Assert.AreEqual(1, stops.Count);
            // get orders
            profit = profits[0];
            stop = stops[0];
            // verify profit offset is same
            Assert.IsTrue(profit.isValid);
            Assert.AreEqual(pid, profit.id);
            // verify stop offset
            Assert.IsTrue(stop.isValid);
            Assert.AreEqual(PRICE + 1 - SOFFSET, stop.stopp);
            Assert.AreEqual(SIZE, stop.UnsignedSize);

        }

        Tick nt() { return (Tick)TickImpl.NewTrade(SYM, PRICE, SIZE); }

        void fill(Trade f) { fill(f, 0); }
        void fill(Trade fill, long id)
        {
            if (fill.id == 0)
            {
                debug("no id on fill: " + fill.ToString());
                fill.id = (id!=0) ? id : ot.Ids.AssignId;
            }
            bool hit = false;
            for (int i = profits.Count - 1; i >= 0; i--)
                if ((profits[i].id == fill.id) && (profits[i].UnsignedSize==fill.UnsignedSize))
                {
                    debug("filled profit: " + fill.id);
                    hit = true;
                    profits.RemoveAt(i);
                }
            for (int i = stops.Count - 1; i >= 0; i--)
                if ((stops[i].id == fill.id) && (stops[i].UnsignedSize==fill.UnsignedSize))
                {
                    debug("filled stop: " + fill.id);
                    hit = true;
                    stops.RemoveAt(i);
                }
            ot.Adjust(fill);
        }

        void ot_SendOffset(Order o)
        {
            if (o.isLimit)
                profits.Add(o);
            else if (o.isStop)
                stops.Add(o);
        }

        void ot_SendCancel(long number)
        {
            bool hit = false;
            for (int i = profits.Count - 1; i >= 0; i--)
                if (profits[i].id == number)
                {
                    hit = true;
                    profits.RemoveAt(i);
                }
            for (int i = stops.Count - 1; i >= 0; i--)
                if (stops[i].id == number)
                {
                    hit = true;
                    stops.RemoveAt(i);
                }

            if (hit)
                ot.GotCancel(number);
            else
                debug("unable to cancel: "+number);
                
        }

        void debug(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}

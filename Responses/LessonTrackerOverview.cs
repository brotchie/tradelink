using System;
using TradeLink.API;
using TradeLink.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace Responses
{
    public class LessonTrackerOverview : ResponseTemplate
    {
        BarListTracker blt = new BarListTracker(BarInterval.ThirtyMin);
        OffsetTracker ot = new OffsetTracker();
        PositionTracker pt = new PositionTracker();
        public LessonTrackerOverview()
        {
            // bind tracker events

            // handle new bar events
            blt.GotNewBar += new SymBarIntervalDelegate(blt_GotNewBar);

            // bind offset events to tradelink primitives
            ot.SendCancelEvent+=new LongDelegate(sendcancel);
            ot.SendDebug+=new DebugDelegate(D);
            ot.SendOrderEvent+=new OrderDelegate(sendorder);

            // set tracker parameters, eg default offset
            ot.DefaultOffset = new OffsetInfo(.5m, .25m);
        }

        void blt_GotNewBar(string symbol, int interval)
        {
            D(symbol + " made new bar: " + interval);
        }

        public override void GotOrderCancel(long id)
        {
            // pass offset cancels back through
            ot.GotCancel(id);
        }

        public override void GotTick(Tick k)
        {
            // update offsets
            ot.newTick(k);
            // build bars from ticks
            blt.newTick(k);
        }

        public override void GotFill(Trade f)
        {
            ot.Adjust(f);
            pt.Adjust(f);
        }

        public override void GotPosition(Position p)
        {
            ot.Adjust(p);
            pt.Adjust(p);
        }

    }
}

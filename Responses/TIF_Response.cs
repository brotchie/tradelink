using System;
using System.Collections.Generic;
using TradeLink.Common;
using TradeLink.API;

namespace Responses
{
    public class TIF_Response : ResponseTemplate
    {
        TIFTracker tt;
        IdTracker idt;
        public override void Reset()
        {
            idt = new IdTracker(ID);
            tt = new TIFTracker(idt);
            tt.SendCancelEvent += new LongDelegate(sendcancel);
            tt.SendDebugEvent += new DebugDelegate(D);
            tt.SendOrderEvent += new OrderDelegate(sendorder);
            tt.DefaultTif = 3;

        }

        int sends = 0;

        public override void GotTick(Tick k)
        {
            // enforce tifs
            tt.newTick(k);
            // ignore quotes
            if (!k.isTrade)
                return;
            // send entry
            if (cancels == sends)
            {
                Order o = new BuyLimit(k.symbol, 100, k.trade * .7m);
                tt.SendOrder(o);
                D(o.symbol + " sent tif-enforced order: " + o);
                sends++;
            }
        }

        int cancels = 0;
        public override void GotOrderCancel(long id)
        {
            cancels++;
            tt.GotCancel(id);
            D("tif cancel received: " + id);
        }
    }
}

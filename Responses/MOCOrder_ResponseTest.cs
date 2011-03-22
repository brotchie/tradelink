using System;
using TradeLink.Common;
using TradeLink.API;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Responses
{
    /// <summary>
    /// Response Name  :
    /// Last Modified  :
    /// Parameter Notes:
    /// Synopsis       :
    /// </summary>

    public class MOCOrder_ResponseTest : ResponseTemplate
    {
        ///////////////////////////////////////////////////////////////////////
        // Initializers
        ///////////////////////////////////////////////////////////////////////
        public MOCOrder_ResponseTest()
            : base()
        {
            Name = "Test MOCOrder";
            Reset();
        }

        public override void Reset()
        {
            RequestedForSymbol = new GenericTracker<bool>();
        }

        ///////////////////////////////////////////////////////////////////////
        // TradeLink Hooks
        ///////////////////////////////////////////////////////////////////////
        public override void GotMessage(MessageTypes type, long source, long dest, long msgid, string request, ref string response)
        {
        }

        public override void GotTick(Tick tick)
        {
            if (RequestedForSymbol.getindex(tick.symbol) == GenericTracker.UNKNOWN
                || RequestedForSymbol[tick.symbol] == false)
            {
                RequestedForSymbol.addindex(tick.symbol, true);
                Order myOrder = new MOCOrder(tick.symbol, true, 100);
                sendorder(myOrder);
                //senddebug(myOrder.TIF);
                senddebug("Sent Order: " + myOrder);
            }
        }

        public override void GotFill(Trade fill)
        {
        }

        public override void GotOrder(Order order)
        {
        }

        public override void GotOrderCancel(long cancelid)
        {
        }

        

        ///////////////////////////////////////////////////////////////////////
        // Member Objects
        ///////////////////////////////////////////////////////////////////////
        GenericTracker<bool> RequestedForSymbol = new GenericTracker<bool>();
    }
}

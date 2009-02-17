using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLink.API
{
    public interface Response
    {
        // Response input
        void GotTick(Tick tick);
        void GotOrder(Order order);
        void GotFill(Trade fill);
        void GotOrderCancel(uint orderid);
        void GotPosition(Position pos);

        // Response output
        event ObjectArrayDelegate SendIndicators;
        event OrderDelegate SendOrder;
        event UIntDelegate SendCancel;
        event DebugFullDelegate SendDebug;

        // response control
        void Reset();

        // Response Information
        bool isValid { get; }
        string Name { get; set; }
        string FullName { get; set; }
        string[] Indicators { get; set; }
    }
}

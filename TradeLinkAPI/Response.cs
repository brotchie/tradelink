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
        void GotOrderCancel(long orderid);
        void GotPosition(Position pos);
        void GotMessage(MessageTypes type, long source, long dest, long msgid, string request,ref string response);

        // Response output
        event ResponseStringDel SendIndicatorsEvent;
        event OrderSourceDelegate SendOrderEvent;
        event LongSourceDelegate SendCancelEvent;
        event DebugDelegate SendDebugEvent;
        event MessageDelegate SendMessageEvent;
        event BasketDelegate SendBasketEvent;
        event ChartLabelDelegate SendChartLabelEvent;
        event TicketDelegate SendTicketEvent;

        // response control
        void Reset();

        // Response Information
        bool isValid { get; set; }
        string Name { get; set; }
        string FullName { get; set; }
        string[] Indicators { get; set; }
        int ID { get; set; }
    }
}

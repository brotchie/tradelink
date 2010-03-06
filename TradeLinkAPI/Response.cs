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
        void GotMessage(MessageTypes type, uint source, uint dest, uint msgid, string request,ref string response);

        // Response output
        event StringParamDelegate SendIndicatorsEvent;
        event OrderDelegate SendOrderEvent;
        event UIntDelegate SendCancelEvent;
        event DebugFullDelegate SendDebugEvent;
        event MessageDelegate SendMessageEvent;
        event BasketDelegate SendBasketEvent;
        event ChartLabelDelegate SendChartLabelEvent;

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

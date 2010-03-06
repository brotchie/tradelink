using System;
using System.Collections.Generic;
using TradeLink.API;
using System.Text;

namespace TradeLink.Common
{
    /// <summary>
    /// used to signify a response was invalid, an incompatible version or otherwise unloadable.
    /// </summary>
    public class InvalidResponse : Response
    {

        public void GotTick(Tick tick)
        {
        }
        public void GotOrder(Order order)
        {
        }
        public void GotFill(Trade fill)
        {
        }
        public void GotOrderCancel(uint cancelid)
        {
        }
        string _name = "";
        string _fn = "InvalidResponse";
        public int ID { get { return -1; } set { } }
        public void Reset() { }
        public void GotMessage(MessageTypes t, uint source, uint dest, uint id, string data, ref string data2) { }
        public void GotPosition(Position p) { }
        public bool isValid { get { return false; } set { } }
        public string[] Indicators { get { return new string[0]; } set { } }
        public string Name { get { return _name; } set { _name = value; } }
        public string FullName { get { return _fn; } set { _fn = value; } }
        public event BasketDelegate SendBasketEvent;
        public event DebugFullDelegate SendDebugEvent;
        public event OrderDelegate SendOrderEvent;
        public event UIntDelegate SendCancelEvent;
        public event StringParamDelegate SendIndicatorsEvent;
        public event MessageDelegate SendMessageEvent;
        public event ChartLabelDelegate SendChartLabelEvent;
    }
}

using System;
using System.Collections.Generic;
using TradeLink.API;
using System.Text;

namespace TradeLink.Common
{
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
        public void Reset() { }
        public void GotPosition(Position p) { }
        public bool isValid { get { return false; } set { } }
        public string[] Indicators { get { return new string[0]; } set { } }
        public string Name { get { return _name; } set { _name = value; } }
        public string FullName { get { return _fn; } set { _fn = value; } }
        public event DebugFullDelegate SendDebug;
        public event OrderDelegate SendOrder;
        public event UIntDelegate SendCancel;
        public event StringParamDelegate SendIndicators;
    }
}

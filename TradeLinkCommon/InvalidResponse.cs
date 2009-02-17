using System;
using System.Collections.Generic;
using TradeLink.API;
using System.Text;

namespace TradeLink.Common
{
    public class InvalidResponse : TradeLink.API.Response
    {
        // Response input
        public void GotTick(Tick tick) { }
        public void GotOrder(Order order) { }
        public void GotFill(Trade fill) { }
        public void GotOrderCancel(uint orderid) { }
        public void GotPosition(Position pos) { }

        // Response output
        public event ObjectArrayDelegate SendIndicators;
        public event OrderDelegate SendOrder;
        public event UIntDelegate SendCancel;
        public event DebugFullDelegate SendDebug;

        // response control
        public void Reset() { }

        string _name = "ERROR";
        string _fullname = "";

        // Response Information
        public bool isValid { get { return false; } set { } }
        public string Name { get { return _name; } set { _name = value; } }
        public string FullName { get { return _fullname; } set { _fullname = value; } }
        public string[] Indicators { get { return new string[0]; } set { } }
    }
}

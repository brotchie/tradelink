using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using TradeLink.API;

namespace Responses
{
    // you can copy+paste this to get a working response w/o all the comments
    // OR... just implement ResponseTemplate
    /// <summary>
    /// Used as a blank implementation of response.
    /// Also see ResponseTemplate.
    /// </summary>
    public class EmptyResponseTemplate : Response
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
        public void Reset() { }
        public void GotPosition(Position p) { }
        public bool isValid { get { return true; } set { } }
        public string[] Indicators { get { return new string[0]; } set { } }
        public string Name { get { return ""; } set { } }
        public string FullName { get { return ""; } set { } }
        public event DebugFullDelegate SendDebug;
        public event OrderDelegate SendOrder;
        public event UIntDelegate SendCancel;
        public event StringParamDelegate SendIndicators;
    }
}

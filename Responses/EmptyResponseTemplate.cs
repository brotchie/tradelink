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
        public void GotOrderCancel(long cancelid)
        {
        }
        public void GotMessage(MessageTypes type, long source, long dest, long id, string request, ref string response) { }
        public void Reset() { }
        public void GotPosition(Position p) { }
        public bool isValid { get { return true; } set { } }
        public string[] Indicators { get { return new string[0]; } set { } }
        public string Name { get { return ""; } set { } }
        public string FullName { get { return ""; } set { } }
        public event DebugFullDelegate SendDebugEvent;
        public event OrderSourceDelegate SendOrderEvent;
        public event LongSourceDelegate SendCancelEvent;
        public event ResponseStringDel SendIndicatorsEvent;
        public event MessageDelegate SendMessageEvent;
        public event BasketDelegate SendBasketEvent;
        public event ChartLabelDelegate SendChartLabelEvent;
        public event TicketDelegate SendTicketEvent;
        public int ID { get; set; }
    }
}

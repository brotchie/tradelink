using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace Responses
{
    /// <summary>
    /// demonstrates response that uses historical bar data
    /// </summary>
    public class BarRequestor : ResponseTemplate
    {
        MessageTracker _mt = new MessageTracker();
        BarListTracker _blt = new BarListTracker();

        public override void Reset()
        {
            _mt.BLT = _blt;
        }

        public override void GotTick(Tick k)
        {
            // if we don't have bar data, request historical data
            if (_blt[k.symbol].Count == 0)
            {
                D(k.symbol + " no bars found, requesting...");
                sendmessage(MessageTypes.BARREQUEST, BarImpl.BuildBarRequest(k.symbol, BarInterval.Hour));
            }
            D(k.symbol + " bar count: " + _blt[k.symbol].Count);
            // update whatever data we have with ticks
            _blt.newTick(k);

        }

        public override void GotMessage(MessageTypes type, long source, long dest, long msgid, string request, ref string response)
        {
            if (type== MessageTypes.BARRESPONSE)
                D(response);
            _mt.GotMessage(type, source, dest, msgid, request, ref response);
        }

    }
}

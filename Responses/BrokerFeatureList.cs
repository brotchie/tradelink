using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace Responses
{
    public class BrokerFeatureList : ResponseTemplate
    {
          public BrokerFeatureList()
        {
            _mt.GotFeatures += new MessageArrayDel(_mt_GotFeatures);
        }

        void _mt_GotFeatures(MessageTypes[] messages)
        {
            D("got feature response");
            foreach (MessageTypes mt in messages)
                D("feature: " + mt.ToString());
        }
        MessageTracker _mt = new MessageTracker();
        public override void GotMessage(MessageTypes type, long source, long dest, long msgid, string request, ref string response)
        {
            _mt.GotMessage(type, source, dest, msgid, request, ref response);
        }
        public override void Reset()
        {
            D("sending feature request.");
            sendmessage(MessageTypes.FEATUREREQUEST,string.Empty);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;


namespace TradeLink.Common
{
    public class Listener
    {
        public virtual MessageTypes[] ListenFor() { return new MessageTypes[0]; }
        public virtual void OnPacket(Packet lp) { }
    }
}

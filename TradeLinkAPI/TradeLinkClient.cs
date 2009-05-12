using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    /// <summary>
    /// Generic interface for TradeLink Clients.  
    /// </summary>
    public interface TradeLinkClient
    {
        int SendOrder(Order order);
        void GoLive();
        void GoSim();
        void Disconnect();
        void Register();
        void Subscribe(Basket mb);
        void Unsubscribe();
        long TLSend(MessageTypes type, string message);
        int HeartBeat();
        void RequestDOM();
    }

    /// <summary>
    /// Used to indicate that a TradeLink Server was not running.
    /// </summary>
    public class TLServerNotFound : Exception { }
}

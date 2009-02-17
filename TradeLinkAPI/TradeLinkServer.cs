using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLink.API
{
    /// <summary>
    /// For providing execution and data subscription services to tradelink clients.
    /// 
    /// </summary>
    public interface TradeLinkServer
    {
        void newTick(Tick tick);
        void newFill(Trade trade);
        int NumClients { get; }
    }


    /// <summary>
    /// Types of TradeLink Servers
    /// </summary>
    public enum TLTypes
    {
        NONE = 0,
        SIMBROKER = 2,
        LIVEBROKER = 4,
        HISTORICALBROKER = 8,
        TESTBROKER = 16,
    }
}

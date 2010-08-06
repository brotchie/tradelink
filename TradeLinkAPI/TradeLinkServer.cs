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
        /// <summary>
        /// enable extended debugging
        /// </summary>
        bool VerboseDebugging { get; set; }
        /// <summary>
        /// send subscribed clients new tick
        /// </summary>
        /// <param name="tick"></param>
        void newTick(Tick tick);
        /// <summary>
        /// send clients new fill
        /// </summary>
        /// <param name="trade"></param>
        void newFill(Trade trade);
        /// <summary>
        /// number of client connected
        /// </summary>
        int NumClients { get; }
        /// <summary>
        /// send clients new order
        /// </summary>
        /// <param name="o"></param>
        void newOrder(Order o);
        /// <summary>
        /// send clients new cancel ack
        /// </summary>
        /// <param name="id"></param>
        void newCancel(long id);
        /// <summary>
        /// start server
        /// </summary>
        void Start();
        /// <summary>
        /// stop server
        /// </summary>
        void Stop();
        /// <summary>
        /// notify of debug events
        /// </summary>
        event DebugDelegate SendDebugEvent;
    }



}

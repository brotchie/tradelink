using System;
using System.Collections.Generic;

namespace TradeLink.API
{
    /// <summary>
    /// template used to build tradelink servers
    /// </summary>
    public class TLServerTemplate : TradeLinkServer
    {
        /// <summary>
        /// send message to a client
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        /// <param name="client"></param>
        public virtual void TLSend(string msg, MessageTypes type, string client)
        {
        }
        /// <summary>
        /// send message to a client
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        /// <param name="client"></param>
        public virtual void TLSend(string msg, MessageTypes type, int client)
        {
        }
        /// <summary>
        /// send debug
        /// </summary>
        public event DebugDelegate SendDebugEvent;
        /// <summary>
        /// notify clients about a fill
        /// </summary>
        /// <param name="fill"></param>
        public virtual void newFill(Trade fill)
        {

        }
        /// <summary>
        /// notify clients about a tick
        /// </summary>
        /// <param name="k"></param>
        public virtual void newTick(Tick k)
        {

        }
        /// <summary>
        /// start server
        /// </summary>
        public virtual void Start()
        {
        }
        /// <summary>
        /// stop server
        /// </summary>
        public virtual void Stop()
        {

        }
        /// <summary>
        /// notify clients about a cancel
        /// </summary>
        /// <param name="id"></param>
        public virtual void newCancel(long id)
        {
        }
        /// <summary>
        /// notify clients about a position
        /// </summary>
        /// <param name="o"></param>
        public virtual void newOrder(Order o)
        {
        }

        public virtual int NumClients { get { return 0; } }

        public virtual bool VerboseDebugging { get { return false; } set { } }

        public virtual void newAccountRequest()
        {
        }


    }
}

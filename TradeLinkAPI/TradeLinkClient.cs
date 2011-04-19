using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    /// <summary>
    /// Generic interface for TradeLink Clients.  
    /// </summary>
    public interface TLClient
    {
        /// <summary>
        /// send order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        int SendOrder(Order order);
        /// <summary>
        /// cancel order
        /// </summary>
        /// <param name="id"></param>
        void CancelOrder(long id);
        /// <summary>
        /// disconnect from server (should call or may have problems with reconnects)
        /// </summary>
        void Disconnect();
        /// <summary>
        /// connect to a server
        /// </summary>
        void Register();

        /// <summary>
        /// request ticks for symbols
        /// </summary>
        /// <param name="mb"></param>
        void Subscribe(Basket mb);
        /// <summary>
        /// unrequest ticks
        /// </summary>
        void Unsubscribe();
        /// <summary>
        /// send a message
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        long TLSend(MessageTypes type, string message);
        /// <summary>
        /// send a message
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="msgid"></param>
        /// <param name="request"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        long TLSend(MessageTypes type, long source, long dest, long msgid, string request, ref string result);
        [Obsolete]
        int HeartBeat();
        /// <summary>
        /// get version of server
        /// </summary>
        int ServerVersion { get; }
        /// <summary>
        /// get name of provider/server
        /// </summary>
        Providers BrokerName { get; }
        /// <summary>
        /// receive ticks
        /// </summary>
        event TickDelegate gotTick;
        /// <summary>
        /// receive trades
        /// </summary>
        event FillDelegate gotFill;
        /// <summary>
        /// receive orders
        /// </summary>
        event OrderDelegate gotOrder;
        /// <summary>
        /// receive account information
        /// </summary>
        event DebugDelegate gotAccounts;
        /// <summary>
        /// receive cancel acks
        /// </summary>
        event LongDelegate gotOrderCancel;
        /// <summary>
        /// request features supported by provider
        /// </summary>
        void RequestFeatures();
        /// <summary>
        /// get providers available
        /// </summary>
        Providers[] ProvidersAvailable { get; }
        /// <summary>
        /// get selected provider
        /// </summary>
        int ProviderSelected { get; }
        /// <summary>
        /// get features for selected provider
        /// </summary>
        List<MessageTypes> RequestFeatureList { get; }
        /// <summary>
        /// receive features
        /// </summary>
        event MessageTypesMsgDelegate gotFeatures;
        /// <summary>
        /// receive [initial] positions
        /// </summary>
        event PositionDelegate gotPosition;
        /// <summary>
        /// receive imbalances
        /// </summary>
        event ImbalanceDelegate gotImbalance;
        /// <summary>
        /// receive messages from broker
        /// </summary>
        event MessageDelegate gotUnknownMessage;
        /// <summary>
        /// receive debug messages from client
        /// </summary>
        event DebugDelegate SendDebugEvent;
        /// <summary>
        /// stop client
        /// </summary>
        void Stop();
        /// <summary>
        /// start client
        /// </summary>
        void Start();
        /// <summary>
        /// connect to a provider (make it selected)
        /// </summary>
        /// <param name="ProviderIndex"></param>
        /// <param name="showwarning"></param>
        /// <returns></returns>
        bool Mode(int ProviderIndex, bool showwarning);
        /// <summary>
        /// reconnect to provider or re-search providers
        /// </summary>
        /// <returns></returns>
        bool Mode();
        /// <summary>
        /// get name of this client
        /// </summary>
        string Name { get; set; }

        bool VerboseDebugging { get; set; }
    }

    /// <summary>
    /// Used to indicate that a TradeLink Server was not running.
    /// </summary>
    public class TLServerNotFound : Exception { }
}

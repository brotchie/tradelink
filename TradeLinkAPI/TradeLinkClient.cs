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
        int SendOrder(Order order);
        void CancelOrder(long id);
        void Disconnect();
        void Register();
        void Subscribe(Basket mb);
        void Unsubscribe();
        long TLSend(MessageTypes type, string message);
        int HeartBeat();
        int ServerVersion { get; }
        Providers BrokerName { get; }
        event TickDelegate gotTick;
        event FillDelegate gotFill;
        event OrderDelegate gotOrder;
        event DebugDelegate gotAccounts;
        event UIntDelegate gotOrderCancel;
        void RequestFeatures();
        Providers[] ProvidersAvailable { get; }
        int ProviderSelected { get; }
        List<MessageTypes> RequestFeatureList { get; }
        event MessageTypesMsgDelegate gotFeatures;
        event PositionDelegate gotPosition;
        event ImbalanceDelegate gotImbalance;
        event MessageDelegate gotUnknownMessage;
        bool Mode(int ProviderIndex, bool showwarning);
        bool Mode();
        string Name { get; set; }
    }

    /// <summary>
    /// Used to indicate that a TradeLink Server was not running.
    /// </summary>
    public class TLServerNotFound : Exception { }
}

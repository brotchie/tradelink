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
        void Disconnect();
        void Register();
        void Subscribe(Basket mb);
        void Unsubscribe();
        long TLSend(MessageTypes type, string message);
        int HeartBeat();
        void RequestDOM();
        event TickDelegate gotTick;
        event FillDelegate gotFill;
        event OrderDelegate gotOrder;
        event DebugDelegate gotAccounts;
        event UIntDelegate gotOrderCancel;
        void RequestFeatures();
        event MessageTypesMsgDelegate gotFeatures;
        event PositionDelegate gotPosition;
        event ImbalanceDelegate gotImbalance;
        event MessageDelegate gotUnknownMessage;
        bool Mode(int ProviderIndex, bool showwarning);
        bool Mode();
    }

    /// <summary>
    /// Used to indicate that a TradeLink Server was not running.
    /// </summary>
    public class TLServerNotFound : Exception { }
}

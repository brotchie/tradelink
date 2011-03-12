using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{
    public class TLCS : TLServer, TLClient
    {
        public string ClientName(int clientnum) { return _name; }
        public Basket AllClientBasket { get { return new BasketImpl(); } }
        public bool SymbolSubscribed(string sym) { return true; }
        string _name = string.Empty;
        /// <summary>
        /// send order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int SendOrder(Order order) { return 0; }
        /// <summary>
        /// cancel order
        /// </summary>
        /// <param name="id"></param>
        public void CancelOrder(long id) { }
        /// <summary>
        /// disconnect from server (should call or may have problems with reconnects)
        /// </summary>
        public void Disconnect() { }
        /// <summary>
        /// connect to a server
        /// </summary>
        public void Register() { }

        /// <summary>
        /// request ticks for symbols
        /// </summary>
        /// <param name="mb"></param>
        public void Subscribe(Basket mb) { }
        /// <summary>
        /// unrequest ticks
        /// </summary>
        public void Unsubscribe() { }
        /// <summary>
        /// send a message
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public long TLSend(MessageTypes type, string message) { return 0; }
        /// send a message
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="msgid"></param>
        /// <param name="request"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public long TLSend(MessageTypes type, long source, long dest, long msgid, string request, ref string result) { return 0; }
        [Obsolete]
        public int HeartBeat() { return 0; }
        /// <summary>
        /// get version of server
        /// </summary>
        public int ServerVersion { get { return 0; } }
        /// <summary>
        /// get name of provider/server
        /// </summary>
        public Providers BrokerName { get { return Providers.Unknown; } }
        /// <summary>
        /// receive ticks
        /// </summary>
        public event TickDelegate gotTick;
        /// <summary>
        /// receive trades
        /// </summary>
        public event FillDelegate gotFill;
        /// <summary>
        /// receive orders
        /// </summary>
        public event OrderDelegate gotOrder;
        /// <summary>
        /// receive account information
        /// </summary>
        public event DebugDelegate gotAccounts;
        /// <summary>
        /// receive cancel acks
        /// </summary>
        public event LongDelegate gotOrderCancel;
        /// <summary>
        /// request features supported by provider
        /// </summary>
        public void RequestFeatures() { }
        /// get providers available
        /// </summary>
        public Providers[] ProvidersAvailable { get { return new Providers[0]; } }
        /// <summary>
        /// get selected provider
        /// </summary>
        public int ProviderSelected { get { return -1; } }
        /// <summary>
        /// get features for selected provider
        /// </summary>
        public List<MessageTypes> RequestFeatureList { get { return new List<MessageTypes>(); } }
        /// <summary>
        /// receive features
        /// </summary>
        public event MessageTypesMsgDelegate gotFeatures;
        /// <summary>
        /// receive [initial] positions
        /// </summary>
        public event PositionDelegate gotPosition;
        /// <summary>
        /// receive imbalances
        /// </summary>
        public event ImbalanceDelegate gotImbalance;
        /// <summary>
        /// receive messages from broker
        /// </summary>
        public event MessageDelegate gotUnknownMessage;
        /// <summary>
        /// receive debug messages from client
        /// </summary>
        public event DebugDelegate SendDebugEvent;
        /// <summary>
        /// stop client
        /// </summary>
        public void Stop() { }
        /// <summary>
        /// start client
        /// </summary>
        public void Start() { }
        /// <summary>
        /// connect to a provider (make it selected)
        /// </summary>
        /// <param name="ProviderIndex"></param>
        /// <param name="showwarning"></param>
        /// <returns></returns>
        public bool Mode(int ProviderIndex, bool showwarning) { return false; }
        /// <summary>
        /// reconnect to provider or re-search providers
        /// </summary>
        /// <returns></returns>
        public bool Mode() { return false; }
        /// <summary>
        /// get name of this client
        /// </summary>
        public string Name { get { return string.Empty; } set { } }


        public string ClientSymbols(string client) { return string.Empty; }



        public Providers newProviderName { get; set; }

        /// <summary>
        /// enable extended debugging
        /// </summary>
        public bool VerboseDebugging { get { return false; } set { } }
        /// <summary>
        /// send subscribed clients new tick
        /// </summary>
        /// <param name="tick"></param>
        public void newTick(Tick tick) { }
        /// <summary>
        /// send clients new fill
        /// </summary>
        /// <param name="trade"></param>
        public void newFill(Trade trade) { }
        /// <summary>
        /// number of client connected
        /// </summary>
        public int NumClients { get { return 0; } }
        /// <summary>
        /// send clients new order
        /// </summary>
        /// <param name="o"></param>
        public void newOrder(Order o) { }
        /// <summary>
        /// send clients new cancel ack
        /// </summary>
        /// <param name="id"></param>
        public void newCancel(long id) {  }


        /// <summary>
        /// send message to a client
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        /// <param name="client"></param>
        public void TLSend(string msg, MessageTypes type, string client) { }

        /// <summary>
        /// send message to a client
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        /// <param name="client"></param>
        public void TLSend(string msg, MessageTypes type, int client) { }

        public event StringDelegate newAcctRequest;
        public event OrderDelegateStatus newSendOrderRequest;
        public event LongDelegate newOrderCancelRequest;
        public event PositionArrayDelegate newPosList;
        public event SymbolRegisterDel newRegisterSymbols;
        public event MessageArrayDelegate newFeatureRequest;
        public event UnknownMessageDelegate newUnknownRequest;
        public event UnknownMessageDelegateSource newUnknownRequestSource;
        public event VoidDelegate newImbalanceRequest;

        /// <summary>
        /// notify clients of a new imbalance
        /// </summary>
        /// <param name="imb"></param>
        public void newImbalance(Imbalance imb) { }
    }
}

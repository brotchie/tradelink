using System;
using System.Collections.Generic;

using System.Windows.Forms;

namespace TradeLib
{
    /// <summary>
    /// Generic interface for TradeLink implementations.  The TradeLink API.
    /// </summary>
    public interface TradeLinkClient
    {
        int SendOrder(Order order);
        void RegIndex(IndexBasket ib);
        void GoLive();
        void GoSim();
        void Disconnect();
        void Register();
        void Subscribe(MarketBasket mb);
        void Unsubscribe();
        int HeartBeat();
    }

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
    /// Get stock and position information
    /// </summary>
    interface TradeLinkInfo
    {
        decimal AvgPrice(string symbol);
        decimal DayClose(string symbol);
        decimal DayHigh(string symbol);
        decimal DayLow(string symbol);
        decimal DayOpen(string symbol);
        int PosSize(string symbol);
        decimal FastHigh(string symbol);
        decimal FastLow(string symbol);
        Position FastPos(string symbol);
        decimal YestClose(string symbol);
    }


    public delegate void MessageDelegate(TL2 msgid, string source);
    public delegate void TickDelegate(Tick t);
    public delegate void FillDelegate(Trade t);
    public delegate void IndexDelegate(Index idx);
    public delegate void StockDelegate(Stock stock);
    public delegate void OrderDelegate(Order o);
    public delegate void IntDelegate(Int64 number);
    public delegate void UIntDelegate(UInt32 number);
    public delegate void TL2MsgDelegate(TL2 [] messages);

    /// <summary>
    /// Used to indicate that a TradeLink Broker Connector was not running.
    /// </summary>
    public class TLServerNotFound : Exception { }

    /// <summary>
    /// Brokerage types
    /// </summary>
    public enum TLTypes
    {
        NONE = 0,
        SIMBROKER = 2,
        LIVEBROKER = 4,
        HISTORICALBROKER = 8,
        TESTBROKER = 16,
    }


    /// <summary>
    /// TradeLink2 message type description, assume a request for said information... unless otherwise specified
    /// </summary>
    public enum TL2
    {
        OK = 0,
        SENDORDER = 1,
        AVGPRICE,
        POSOPENPL,
        POSCLOSEDPL,
        POSLONGPENDSHARES,
        POSSHORTPENDSHARES,
        LRPBID,
        LRPASK,
        POSTOTSHARES,
        LASTTRADE,
        LASTSIZE,
        NDAYHIGH,
        NDAYLOW,
        INTRADAYHIGH,
        INTRADAYLOW,
        OPENPRICE,
        CLOSEPRICE,
        NLASTTRADE = 20,
        NBIDSIZE,
        NASKSIZE,
        NBID,
        NASK,
        ISSIMULATION,
        GETSIZE,
        YESTCLOSE,
        BROKERNAME,
        ACCOUNTOPENPL,
        ACCOUNTCLOSEDPL,
        TICKNOTIFY = 100,
        EXECUTENOTIFY,
        REGISTERCLIENT,
        REGISTERSTOCK,
        CLEARSTOCKS,
        CLEARCLIENT,
        HEARTBEAT,
        ORDERNOTIFY,
        INFO,
        QUOTENOTIFY,
        TRADENOTIFY,
        REGISTERINDEX,
        DAYRANGE,
        ACCOUNTRESPONSE = 500,
        ACCOUNTREQUEST,
        ORDERCANCELREQUEST,
        ORDERCANCELRESPONSE,
        FEATUREREQUEST,
        FEATURERESPONSE,
        FEATURE_NOT_IMPLEMENTED = 994,
        CLIENTNOTREGISTERED = 995,
        GOTNULLORDER = 996,
        UNKNOWNMSG,
        UNKNOWNSYM,
        TL_CONNECTOR_MISSING,
    }


}


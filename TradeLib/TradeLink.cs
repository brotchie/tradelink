using System;
using System.Collections.Generic;

using System.Windows.Forms;

namespace TradeLib
{
    /// <summary>
    /// Generic interface for TradeLink implementations.  The TradeLink API.
    /// </summary>
    public abstract class TradeLinkClient
    {
        public abstract int SendOrder(Order order);
        public abstract void RegIndex(IndexBasket ib);
        public abstract void GoLive();
        public abstract void GoSim();
        public abstract void Disconnect();
        public abstract void Register();
        public abstract void Subscribe(MarketBasket mb);
        public abstract void Unsubscribe();
        public abstract int HeartBeat();
        /// <summary>
        /// Gets a user-friendly string for Assent's Anvil error messages.
        /// </summary>
        /// <param name="errorcode">The errorcode.</param>
        /// <returns></returns>
        public static string PrettyError(Brokers broker, int errorcode)
        {
            // assent errors
            string[] anvil = { "SO_OK", "SO_NO_ACCOUNT", "SO_NO_SERVER_CONNECTION", "SO_STOCK_NOT_INITIALIZED", "SO_BUYING_POWER_EXCEEDED", "SO_SIZE_ZERO", "SO_INCORRECT_PRICE", "SO_INCORRECT_SIDE", "SO_NO_BULLETS_FOR_CHEAP_STOCK", "SO_NO_SHORTSELL_FOR_CHEAP_STOCK", "SO_NO_ONOPENORDER_FOR_NASDAQ_STOCK", "SO_NO_ONCLOSEORDER_FOR_NASDAQ_STOCK", "SO_NO_ONCLOSEORDER_AGAINST_IMBALANCE_AFTER_1540", "SO_NO_ONCLOSEORDER_AFTER_1600", "SO_NO_SIZEORDER_FOR_NON_NASDAQ_STOCK", "SO_NO_STOPORDER_FOR_NASDAQ_STOCK", "SO_MAX_ORDER_SIZE_EXCEEDED", "SO_MAX_POSITION_SIZE_EXCEEDED", "SO_MAX_POSITION_VALUE_EXCEEDED", "SO_TRADING_LOCKED", "SO_TRADING_HISTORY_NOT_LOADED", "SO_NO_SOES_ORDER_WHEN_MARKET_CLOSED", "SO_NO_SDOT_ORDER_WHEN_MARKET_CLOSED", "SO_MAX_OPEN_POSITIONS_EXCEEDED", "SO_MAX_POSITION_PENDING_ORDERS_EXCEEDED", "SO_MAX_TOTAL_SHARES_EXCEEDED", "SO_MAX_TRADED_SHARES_EXCEEDED", "SO_AMEX_ORDER_EXECUTION_BLOCKED", "SO_NYSE_ORDER_EXECUTION_BLOCKED", "SO_NASDAQ_ORDER_EXECUTION_BLOCKED", "SO_ARCA_ORDER_EXECUTION_BLOCKED", "SO_MAX_LOSS_EXCEEDED", "SO_MAX_LOSS_PER_STOCK_EXCEEDED", "SO_MAX_OPEN_LOSS_PER_STOCK_EXCEEDED", "SO_NYSE_ODD_LOT_VIOLATION", "SO_SHORT_EXEMPT_NOT_INSTITUTIONAL", "SO_SELL_SIZE_GREATER_THAN_POSITION", "SO_NO_SHORT_BEFORE_SELL_COVER_POSITION", "SO_SHORT_CAN_EXECUTE_BEFORE_SELL", "SO_SAME_PRICE_VENUE_OVERSELL", "SO_STAGING_TICKET_EXCEEDED", "SO_DESTINATION_NOT_RECOGNIZED", "SO_HIT_OWN_ORDERS", "SO_SHORT_SELL_VIOLATION", "SO_POTENTIAL_OVERSELL" };
            TL2 message = (TL2)errorcode;
            switch (message)
            {
                    // tradelink messages
                case TL2.UNKNOWNSYM: return "Unknown symbol.";
                case TL2.UNKNOWNMSG: return "Unknown message.";
                case TL2.TL_CONNECTOR_MISSING: return "TradeLink Server not found.";
                case TL2.GOTNULLORDER: return "Unable to read order.";
                case TL2.OK: return "Ok";
                default:
                    // broker-specific messages
                    if ((errorcode < anvil.Length) && (broker==Brokers.Assent)) 
                        return anvil[errorcode];
                    break;
            }
            return "Unknown error: "+errorcode.ToString();
        }

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
        void GoSrv();
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

    /// <summary>
    /// Used to indicate that a TradeLink Broker Connector was not running.
    /// </summary>
    public class TLServerNotFound : Exception { }


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
        ACCOUNTRESPONSE,
        ACCOUNTREQUEST,
        FEATURE_NOT_IMPLEMENTED = 994,
        CLIENTNOTREGISTERED = 995,
        GOTNULLORDER = 996,
        UNKNOWNMSG,
        UNKNOWNSYM,
        TL_CONNECTOR_MISSING,
    }


}


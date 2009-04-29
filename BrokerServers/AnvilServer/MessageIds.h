#ifndef MESSAGEIDSH
#define MESSAGEIDSH

#include "CommonIds.h"

enum MessageIds
{
    M_TEXT_ASCII =                                              500,
	// message from the Crossed Locked list to the observers
	// whenever there is a change in the lists.
    M_CROSSED_LOCKED_UPDATE =                                   2001,

	// An invalid request ID was sent - something that is not supported by an object.
	// This indicates a programmatic error.
    M_INVALID_REQUEST,

	// Changes to the Island OpenBook.
    M_ISLAND_UPDATE,

	// When an invalid symbol is entered into the stock cache.
    M_ERR_INVALID_SYMBOL,
	
	// When a refresh message should be sent to all the clients.
    M_REFRESH_STOCK,

	// Changes to Level 1 Quote.
    M_LEVEL1_QUOTE,

	// Changes to the Nasdaq Stock - Soes changes.
    M_NASDAQ_STOCK =                                            2008,

	// Changes to the Aggregated Book <see AggregatedBook.h>
    M_AGGREGATED_BOOK =                                         2010,

	// Ticker
    M_TICKER,

	// A Generic Flush All Message for 
	// objects in our Stock Cache.
    M_FLUSH_ALL,
	// Message to flush the Crossed Locked History entries.
    M_FLUSH_CROSSED_LOCKED,

    M_FLUSH_ALL_OPEN_BOOKS,

	// FYI message ( primary for SELECTNET FYIs )
    M_FYI,

	// heart beat message sent out by the server to all the
	// connected clients.
    M_HEARTBEAT,

	// A remote server sends this message to identify itself
	// so that the core market server can forward all messages
	// to it.
    M_REMOTE_SERVER,

	//message sent to sync time
    M_SYNC_TIME,

    M_FLUSH_BOOK_FOR_STOCK,

	//message sent to reset allsequence numbers to zero
    M_RESET_SEQUENCE_NUMBERS =                                  2020,

	// message sent out when a new Symbol gets added dynamically
	// to stock cache.
    M_NEW_SYMBOL,

    M_FLUSH_ALL_ISLAND =                                        2023,
    M_UPDATE_ISLAND_PARTY,

	// A Generic Flush All Message for 
	// objects in our Stock Cache.
    M_FLUSH_VOLUME,

    M_KEEPALIVE =                                               2027,
    M_TRANSACTIONS_KEEPALIVE =                                  2028,

	M_FLUSH_ATTRIBUTED_BOOK	=									2031,
	M_FLUSH_ATTRIBUTED_BOOK_FOR_STOCK =							2032,


//    M_ITCH_ADD_ORDER =                          3001,
//    M_ITCH_MODIFY_ORDER,
/*
	// Ouch driver Messages.
    M_OUCH_ORDER_ENTER,
    M_OUCH_ORDER_CANCEL,

	// Ouch Order responses.
    M_OUCH_ORDER_ACCEPTED,
    M_OUCH_ORDER_CANCELED,
    M_OUCH_ORDER_EXECUTED,
    M_OUCH_ORDER_BROKEN,

	// Ouch Reject messages
    M_OUCH_REJECT_ORDER,
    M_OUCH_REJECT_CANCEL,

	// Ouch Messages for SQL Notification
    M_OUCH_EXECUTED_SQL,
    M_OUCH_CANCELLED_SQL,
*/
    // ITCH 1.00
    M_ITCH_1_00_NewVisibleOrder =               3013,
    M_ITCH_1_00_VisibleOrderExecution,
    M_ITCH_1_00_HiddenOrderExecution,
    M_ITCH_1_00_CanceledOrder,
    M_ITCH_1_00_BrokenTrade,

	M_ITCH_100_EXECUTION_CONTRA_WITH_PRICE =	3026,
	M_ITCH_100_VISIBLE_EXECUTION =				3027,

	M_ITCH_1_00_ATTRIBUTED_CanceledOrder =		3049,
	M_ITCH_1_00_NewVisibleAttributedOrder,
	M_ITCH_1_00_VisibleAttributedOrderExecution,
	M_ITCH_1_00_HiddenAttributedOrderExecution,
	M_ITCH_1_00_BrokenAttributedTrade,

    M_NW2_DATABASE_INIT =                       4001,
    M_NW2_SELECTNET403,
    M_NW2_SELECTNET402,
    M_NW2_MM_QUOTE,
    M_NW2_INSIDE_QUOTE,
    M_NW2_LAST_TRADE,
    M_NW2_SOES_INFO,
// Request to NW2 to refresh Symbols
//    M_NW2_REFRESH_SYMBOLS,
// SelectNet Order messages.
    M_SELECTNET_ORDER_ENTRY =					4009,
    M_SELECTNET_ORDER_CANCEL,
    M_SELECTNET_ENTRY_RESPONSE,
    M_SELECTNET_CANCEL_RESPONSE,
    M_SELECTNET_CANCEL_SQL,
//TAL Specific messages
    M_TAL_LAST_TRADE,
    M_NW2_INDEX_DETAILS,
    M_SELECTNET_ORDER_WAS_NOT_SENT,
    M_SELECTNET_ORDER_UPDATE,
    M_SELECTNET_EXECUTION,
/*
    M_CQS_SOCKET_CONNECT,
    M_CQS_SOCKET_DISCONNECT,
    M_eSignal_SOCKET_CONNECT,
    M_eSignal_SOCKET_DISCONNECT,
    M_eSignal_DISPLAY_TRACE,
    M_XPress_SOCKET_CONNECT,
    M_XPress_SOCKET_DISCONNECT,
*/
    M_MARKET_IMBALANCE                          = 4026,

    M_LEVEL2_QUOTE,
    M_LAST_TRADE_SHORT,
    M_LEVEL2_QUOTE_SINGLE,
    M_LEVEL1_QUOTE_SINGLE						= 4031,

//    M_SYMBOL_MARKET_INFO                        = 4036,

    M_NEW_MARKET_IMBALANCE                      = 4044,
//    M_LEVEL2_ATTRIBUTED_QUOTE                   = 4045,
    M_LRP_BID										= 4046,
    M_LRP_ASK										= 4047,
    M_LRP_BIDASK									= 4048,
// Market Session Open & Close message 
// from NQDS.

	M_NYSE_IMBALANCE							= 4101,
	M_NYSE_IMBALANCE_NONE						= 4102,

    M_MARKET_CLOSE                              = 5001,
    M_MARKET_OPEN,
    M_NQDS_SEQUENCE,

    M_SOES_ENTRY		     = 6501,
    M_SOES_CANCEL,
    M_SOES_EXECUTION,
    M_SOES_CANCEL_RESPONSE,
    M_SOES_SNET_EXECUTION,
    M_SOES_ENTRY_REJECT,

	M_FEED_ID_ADD								= 7201,
	M_FEED_ID_REMOVE							= 7202,

/*

    M_SOES_SYSTEM_CHECK	     = 6510;
    M_SOES_SUSPEND_SEQ_CHECK,
    M_SOES_INIT_CONNECTION,
    M_SOES_CLOSE_CONNECTION,
    M_SOES_GOOD_MORNING,
    M_SOES_GOOD_NITE,
    M_SOES_RESET_ORDER_SEQ,

    M_SOES_CANCEL_SQL,
    M_SOES_RESET_HEART_BEAT_TIMER,
    M_SOES_UPDATE_CHANNELS_STATUS,
    M_SOES_UPDATE_CHANNEL_STATUS,
    M_SOES_LOGON_RESPONSE,
    M_SOES_CREATE_TRANSPORT_LAYER,
    M_SOES_SET_CHANNEL_READY_TIMER,

    M_CTCI_SOCKET_DISCONNECT,
    M_CTCI_SOCKET_CONNECT,

    M_SOES_DISPLAY_TRACE,

    M_SOES_CANCEL_REJECT,
*/

    M_REQ_LOGON	=                               10001,
    M_RESP_LOGON,
    M_REQ_LAYOUT_LOAD,
    M_RESP_LAYOUT_LOAD,
    M_REQ_LAYOUT_SAVE,
    M_RESP_LAYOUT_SAVE,
    M_CLIENT_LOGON_AUTHENTICATED, // message is defined in the BClientHandler.h
    M_REQ_RECONNECT,
    M_REQ_LOGON_EX,
    M_RESP_LOGON_EX,
    M_CLIENT_LOGON_AUTHENTICATED_EX, // message is defined in the BClientHandler.h

    M_REQ_LOGON_EX_1 =                          10016,
    M_RESP_LOGON_EX_1 =                         10017,

//Orders
    M_POOL_ORDER_ADDED =                        11001,
    M_POOL_INITIALIZE =                         11002,
//obsolete. use M_REQ_LOAD_POOL_FIRST_EX
//    M_REQ_LOAD_POOL =                           11003,
    M_POOL_UPDATE_ORDER =                       11004,
    M_REQ_NEW_ORDER =                           11006,
    M_REQ_CANCEL_ORDER =                        11007,
    M_POOL_ASSIGN_ORDER_ID =                    11008,
    M_POOL_EXECUTION =                          11010,
    M_POOL_CANCEL =                             11011,

    M_CANCEL_REJECTED =                         11016,
    M_REQ_LOAD_POOL_FIRST_EX =                  11017,
    M_REQ_LOAD_POOL_NEXT_EX =                   11018,
    M_RESP_LOAD_POOL_NEXT_EX =                  11019,
    M_RESP_LOAD_POOL_COMPLETED =                11020,
    M_POOL_BULLETS_ADD =                        11021,
    M_REQ_BULLETS_UPDATE =                      11022,
    M_RESP_BULLETS_UPDATE =                     11023,

// Trade changes done from Outside.
    M_TRADE_INSERT =                            11026,
//    M_TRADE_UPDATE,
    M_TRADE_DELETE =                            11028,

// messages from market to trader (used in ticker)
    M_MT_EXECUTION,
    M_MT_UR_OUT,
    M_MT_ORDER_REJECTED,
    M_MT_CANCEL_REJECTED,
    M_MT_TEXT,
    
    M_REQ_LOAD_POOL_NEXT =                      11035,
    M_REQ_LOAD_POOL_FIRST =                     11036,
    M_REQ_CANCEL_WITH_REPLACEMENT_ORDER =       11040,
    M_MT_ORDER_STATUS_REPORT =                  11041,
    M_REQ_ORDER_STATUS_REPORT =                 11042,

	M_REQ_CANCEL_STAGING_ORDER =				11061,
	M_STAGING_ORDER_CANCELED =					11062,
	M_STAGING_ORDER_CANCEL_REJECTED =			11063,
	M_STAGING_ORDER_LOCKED_BY_ANOTHER =			11066,
	M_STAGING_ORDER_LOCK_REJECTED =				11067,
	M_STAGING_ORDER_UNLOCKED =					11069,
	M_STAGING_ORDER_UNLOCK_REJECTED =			11070,

	M_ASSIGN_STAGING_ORDER_ID =					11077,
	M_REQ_LOCK_STAGING_ORDER =					11078,
	M_STAGING_ORDER_LOCKED =					11079,
	M_REQ_UNLOCK_STAGING_ORDER =				11080,
	M_REQ_NEW_STAGING_ORDER =					11082,

	M_REQ_NEW_OPTION_ORDER =					11085,
	M_UPDATE_OPTION_ORDER =						11086,
    M_POOL_ASSIGN_OPTION_ORDER_ID =             11087,

	M_POOL_LOCATE =								11088,

    M_REQ_LOAD_LOGGED_ACCOUNT_FIRST =           11090,
    M_REQ_LOAD_LOGGED_ACCOUNT_NEXT,
    M_RESP_LOAD_LOGGED_ACCOUNT_NEXT,
    M_RESP_LOAD_LOGGED_ACCOUNT_COMPLETED,
    M_RESP_LOAD_LOGGED_ACCOUNT,

///////////////    
    M_REQ_STOCK_CACHE_STATUS =                  20001,
	M_REQ_REFRESH_SYMBOL,
	M_REQ_CROSSED_LOCKED,
	M_REQ_HEARTBEAT,
	M_REQ_FILTER_SYMBOLS,
	M_REQ_POPULATION,
	M_REQ_POPULATE_NEXT,	
	M_REQ_CACHE_SIZE,	
	M_REQ_RESEND_PACKET,	

	// Request sent by a remote server to get the market data
	// from a core server.
	M_REQ_REMOTE,
	M_REQ_LOG_TRADER_PNL,
	M_REQ_BROADCAST_MSG,
//	M_REQ_CLIENTUPDATE,
//	M_REQ_SENDFILE,
	M_REQ_FROM_CLIENT_UNSUBSCRIBE_SYMBOL =		20015,

	M_REQ_REFRESH_FEED =						20050,
	M_RESP_REFRESH_FEED,
	M_RESP_REFRESH_FEED_FAILED,
	M_REQ_UNSUBSCRIBE_FEED,
	M_RESP_UNSUBSCRIBE_FEED,
	M_RESP_UNSUBSCRIBE_FEED_FAILED,

    M_RESP_STOCK_CACHE_STATUS =                 21001,
	M_RESP_REFRESH_SYMBOL,
	M_RESP_CROSSED_LOCKED,
	M_RESP_HEARTBEAT,
	M_RESP_REFRESH_SYMBOL_FAILED,
	M_RESP_POPULATION_DONE,
	M_RESP_CACHE_SIZE,
	M_RESP_RESEND_PACKET,
//	M_RESP_CLIENTUPDATE,
//	M_RESP_SENDFILE,
    M_RESP_TO_CLIENT_UNSUBSCRIBE_SYMBOL_FAILED = 21011,
	M_RESP_TO_CLIENT_SYMBOL_UNSUBSCRIBED,

	M_REQ_REFRESH_OPTION =						21013,
	M_RESP_REFRESH_OPTION =						21014,
	M_RESP_REFRESH_OPTION_FAILED =				21015,
	M_REQ_FROM_CLIENT_UNSUBSCRIBE_OPTION =		21016,
	M_RESP_TO_CLIENT_OPTION_UNSUBSCRIBED =		21017,
	M_RESP_TO_CLIENT_UNSUBSCRIBE_OPTION_FAILED =21018,
	M_REQ_REFRESH_UNDERLIER =					21019,
	M_RESP_REFRESH_UNDERLIER =					21020,
	M_RESP_REFRESH_UNDERLIER_FAILED =			21021,

    M_BOOK_NEW_ORDER =	                        22001,
    M_BOOK_MODIFY_ORDER,
    M_BOOK_DELETE_ORDER,
    M_BOOK_NYSE_OPEN_BOOK,

	M_ANVIL_SERVER =							30000,

    MS_REFRESH_SYMBOL_CHART =                   30001,
//    MS_REFRESH_INDEX_CHART,

    MS_REQ_POPULATE_FIRST_SYMBOL_SORTABLE =     30003,
    MS_REQ_POPULATE_NEXT_SYMBOL_SORTABLE,
//Obsolete
    MS_RESP_REFRESH_SYMBOL_SORTABLE,
/////
    MS_RESP_SYMBOL_SORTABLE_POPULATION_DONE,
//Obsolete
    MS_RESP_SYMBOL_SORTABLE_BID,
    MS_RESP_SYMBOL_SORTABLE_ASK,
    MS_RESP_SYMBOL_SORTABLE_BIDASK,
/////
    MS_RESP_SYMBOL_SORTABLE_TRADE,

    MS_RESP_NEXT_MINUTE,
    MS_RESP_INIT,
    MS_RESP_SYMBOL_SORTABLE_ECNTRADE,
    MS_RESP_SYMBOL_SORTABLE_CLOSEPRICE,

    MS_REQ_POPULATE_FIRST_INDEX_SORTABLE,
    MS_REQ_POPULATE_NEXT_INDEX_SORTABLE,
    MS_RESP_REFRESH_INDEX_SORTABLE,
    MS_RESP_INDEX_SORTABLE_POPULATION_DONE,

    MS_REQ_VERSION,
    MS_RESP_VERSION,

    MS_RESP_SYMBOL_SORTABLE_LEVEL1,
    MS_RESP_REFRESH_SYMBOL_SORTABLE_PACK,

    MS_RESP_ADMIN_MESSAGE,
    MS_RESP_ADMIN_MESSAGE_DONE,

    MS_RESP_SYMBOL_SORTABLE_L2_LARGE_QUOTE,
    MS_RESP_SYMBOL_SORTABLE_FYI,

    MS_RESP_SYMBOL_SORTABLE_PRE_MARKET_INDICATOR,

    MS_RESP_SYMBOL_SORTABLE_NYS_BID,
    MS_RESP_SYMBOL_SORTABLE_NYS_ASK,


    M_ORDER_CANCEL_AI,

    MS_UPGRADE_TO_VERSION,

    MS_RESP_SYMBOL_SORTABLE_NYS_BIDSIZE,
    MS_RESP_SYMBOL_SORTABLE_NYS_ASKSIZE,

    MS_RESP_SYMBOL_SORTABLE_NYS_BIDQUOTE,
    MS_RESP_SYMBOL_SORTABLE_NYS_ASKQUOTE,

    MS_REQ_HISTORY_CHART,
    MS_RESP_HISTORY_CHART,

	MS_RESP_SYMBOL_SORTABLE_BID_LRP,
	MS_RESP_SYMBOL_SORTABLE_ASK_LRP,
	MS_RESP_SYMBOL_SORTABLE_BIDASK_LRP,

	MS_RESP_SYMBOL_SORTABLE_NYS_BIDCONDITION,
	MS_RESP_SYMBOL_SORTABLE_NYS_ASKCONDITION,

	MS_LEGACY_VERSION,

	MS_REQ_HISTORY_PRINTS,
	MS_RESP_HISTORY_PRINTS,
	MS_RESP_HISTORY_PRINTS_LOADING_DONE,

	MS_RESP_MARKET_DISCONNECTED,

	MS_RESP_SYMBOL_SORTABLE_CLOSEQUOTE,
	MS_RESP_SYMBOL_SORTABLE_PRINT_REMOVED,

	M_MS_NYSE_IMBALANCE,
	M_MS_NYSE_IMBALANCE_NONE,

};


#endif

#ifndef COMMONIDSH
#define COMMONIDSH

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

const unsigned int LENGTH_SYMBOL = 8;
const unsigned int LENGTH_SEQURITYNAME = 31;

enum BookIds
{
    ISLD_BOOK = 0,
	BATS_BOOK,//INCA_BOOK
    ARCA_BOOK,
	TRAC_BOOK,//BRUT_BOOK,
    NYSE_BOOK,

    BTRD_BOOK,//SIZE_BOOK
    EDGA_BOOK,
    EDGX_BOOK,

    NSX_BOOK,

	MAX_BOOKS,
};

enum CommonMessageIds
{
    MSGID_CONNECTION_MADE =                     60001,
    MSGID_CONNECTION_LOST,
    MSGID_BROADCAST_RECEIVED,
    MSGID_SUBSCRIBE,
    MSGID_UNSUBSCRIBE,
    MSGID_RECEIVER_TIMER,
//    MSGID_SYSTEM_IDLE,
    MSGID_LOGOUT =								60008,
    MSGID_CONNECTION_FAILED,
    MSGID_SERVICE,
    MSGID_CLIENT_ACCEPTED,
	MSGID_CONNECTION_STARTED,
	MSGID_DATA_RECEIVED,
	MSGID_DATA_PROCESSED,
};

enum OrderState
{
	PR_ORDER				= 0x00,
	PR_ACCEPT_COUNTER_OFFER	= 0x01,
	PR_CANCEL				= 0x02,
	PR_NONE					= 0x03,
};

enum HistoricalOrderType
{
	LP_EXECUTION = 1,
	LP_ORDER	 = 2,
	LP_CANCEL	 = 3,
	LP_BULLET	 = 4,
	LP_STAGING_ORDER = 5,
	LP_OPTION_ORDER = 6,
};

enum TransactionSide
{
	SIDE_BUY				= 0x0,
	SIDE_SELL				= 0x1,
	SIDE_SSHORT				= 0x2,
	SIDE_SSHORTEXEMPT		= 0x3,
};

enum Tif
{
	TIF_IOC					= 0,
	TIF_ON_OPEN_NON_NASDAQ_LISTED = 99991,
	TIF_ON_CLOSE_NON_NASDAQ_LISTED = 99992,
	TIF_ISLAND_DAY			= 99998,
	TIF_ISLAND_EXTENDED_DAY	= 99999,

    TIF_DEFAULT             = 999999,

	TIF_NOW_AND_NEXT_CROSSING_SESSION = 0xFFFFFFF5,
    TIF_ON_CLO_IMBALANCE_ONLY   = 0xfffffff6,
    TIF_IOC_EXTENDED_DAY        = 0xfffffff7,
    TIF_CROSSING_SESSION        = 0xfffffff8,
    TIF_ON_OPG_IMBALANCE_ONLY   = 0xfffffff9,

	TIF_ON_CLOSE			= 0xfffffffa,
	TIF_FOK					= 0xfffffffb,
	TIF_GTX					= 0xfffffffc,
	TIF_GTC					= 0xfffffffd,
	TIF_OPENING				= 0xfffffffe,
	TIF_DAY					= 0xffffffff
};

enum DestinationExchange
{
	DE_DEFAULT	= 0x0,
//    DE_NYSE_NX  = 0x1,
    DE_AMEX		= 0x2,
	DE_NYSE		= 0x3,
};

enum DestinationMarket
{
	MARKET_ISLD		= 0x00,
	MARKET_EDGX		= 0x01,
	MARKET_SOES		= 0x02,
	MARKET_BRAS		= 0x03,
	MARKET_ARCA		= 0x04,
//	MARKET_ATTN		= 0x05,
//	MARKET_BRUT		= 0x06,
	MARKET_BTRD		= 0x07,
//	MARKET_INCA		= 0x08,
	MARKET_TRAC		= 0x09,
	MARKET_BATS		= 0x0A,
	MARKET_SUPERDOT	= 0x0B,
	MARKET_ERCO		= 0x0C,
	MARKET_NSDQ		= 0x0C,
	MARKET_OESI		= 0x0D,
	MARKET_FIX		= 0x0E,
	MARKET_BELX		= 0x0F,
};

enum ExtendedMarket
{
    FLAG_BYTE_0_EXTENDED_MARKET_ASE     = 'A',  // don't trade
    FLAG_BYTE_0_EXTENDED_MARKET_BSE     = 'B',  // certification left
    FLAG_BYTE_0_EXTENDED_MARKET_NSX     = 'C',
    FLAG_BYTE_0_EXTENDED_MARKET_NASD    = 'D',  // don't trade
    FLAG_BYTE_0_EXTENDED_MARKET_ISE     = 'I',
    FLAG_BYTE_0_EXTENDED_MARKET_CSE     = 'M',
    FLAG_BYTE_0_EXTENDED_MARKET_NYS     = 'N',  // don't trade
    FLAG_BYTE_0_EXTENDED_MARKET_PSE     = 'P',  // don't trade
    FLAG_BYTE_0_EXTENDED_MARKET_NASDAQ  = 'T',  // don't trade
    FLAG_BYTE_0_EXTENDED_MARKET_CBOE    = 'W',  // certification left
    FLAG_BYTE_0_EXTENDED_MARKET_PSX     = 'X',

    FLAG_BYTE_0_EXTENDED_MARKET_ATD     = 'E',
    FLAG_BYTE_0_EXTENDED_MARKET_GOLD     = 'F',
    FLAG_BYTE_0_EXTENDED_MARKET_NYFX     = 'G',

    FLAG_BYTE_0_EXTENDED_MARKET_ISEO     = 'O',

	FLAG_BYTE_0_EXTENDED_MARKET_UNKNOWN = '\0'
};

enum Tracking
{
	TR_ON_CLIENT				= 0x0,
	TR_CLIENT_TO_SERVER			= 0x1,
	TR_ON_SERVER				= 0x2,
	TR_SERVER_TO_MARKET			= 0x3,
	TR_ON_MARKET				= 0x4,
	TR_CANCELLED_BY_SERVER		= 0x5,
	TR_CANCELLED_BY_MARKET		= 0x6,
	TR_REJECTED_BY_SERVER		= 0x7,
	TR_REJECTED_BY_MARKET		= 0x8,
	TR_FILLED					= 0x9,
	TR_PARTIALLY_FILLED			= 0xA,
	TR_COUNTEROFFERED			= 0xB,
	TR_CANCELLED_BY_CLIENT		= 0xC,
	TR_SERVER_TO_SUPERSERVER	= 0xD,
	TR_ON_SUPERSERVER			= 0xE
};

enum RStockAttributes
{
	STOCKATTR_SHORTABLE		      = 0x01,
	STOCKATTR_IPO				  = 0x02,
	STOCKATTR_UPC11830		      = 0x04,

	STOCKATTR_TEST				  = 0x08,
//	STOCKATTR_EXPENSIVE_TO_BORROW = 0x08,
	STOCKATTR_HALTED	          = 0x10,
    STOCKATTR_CALLABLE            = 0x20,
    STOCKATTR_REG_SHO             = 0x40,
    STOCKATTR_ELASTIC_BROADCAST   = 0x80,
};

enum
{
	TRADE_GREATERTHANASK = 0,
	TRADE_EQUALTOASK,
	TRADE_BETWEENBIDASK,
	TRADE_EQUALTOBID,
	TRADE_LESSTHANBID,
    TRADE_NONE,

	TRADE_STATUSMASK = TRADE_GREATERTHANASK | TRADE_EQUALTOASK | TRADE_BETWEENBIDASK | TRADE_EQUALTOBID | TRADE_LESSTHANBID | TRADE_NONE,

	TRADE_Exch_ASE = 0x00010000,
	TRADE_Exch_NYS = 0x00020000,
	TRADE_Exch_NAS = 0x00040000,
	TRADE_Exch_BSE = 0x00080000,
	TRADE_Exch_CIN = 0x00100000,
	TRADE_Exch_CSE = 0x00200000,
	TRADE_Exch_PSE = 0x00400000,
	TRADE_Exch_CBO = 0x00800000,
	TRADE_Exch_PHS = 0x01000000,
    TRADE_Exch_ISLD = 0x02000000,
    TRADE_Exch_ARCA = 0x04000000,
//    TRADE_Exch_BRUT = 0x08000000,
//    TRADE_Exch_INCA = 0x10000000,
	TRADE_Exch_ISE = 0x20000000,
	TRADE_Exch_ADF = 0x40000000,
	TRADE_ExchMask = TRADE_Exch_ASE |
        TRADE_Exch_NYS |
        TRADE_Exch_NAS |
        TRADE_Exch_BSE |
        TRADE_Exch_CIN |
        TRADE_Exch_CSE |
        TRADE_Exch_PSE |
        TRADE_Exch_CBO |
        TRADE_Exch_PHS |
        TRADE_Exch_ISLD |
        TRADE_Exch_ARCA |
//        TRADE_Exch_BRUT |
//        TRADE_Exch_INCA |
		TRADE_Exch_ISE |
		TRADE_Exch_ADF
};

enum RExchange
{
    ANY     = 0x00,
	NASDAQ	= 0x01,
	NYSE	= 0x02,
	AMEX	= 0x03,
	ARCA	= 0x04,
	CBOE	= 0x05,

    ExchangeCount
};

enum ExecExchange
{
	ExecExch_ANY,
	ExecExch_ASE,
	ExecExch_NYS,
	ExecExch_NAS,
	ExecExch_BSE,
	ExecExch_CIN,
	ExecExch_CSE,
	ExecExch_PSE,
	ExecExch_CBO,
	ExecExch_PHS,

	ExecExch_NotUsed1,
	ExecExch_NotUsed2,
	ExecExch_NotUsed3,
	ExecExch_NotUsed4,

	ExecExch_ISE,
	ExecExch_ADF,
	ExecExch_NSD,

	ExecExch_Count
};

enum ExecutionAlgorithm
{
    AON                 = 'A',
    SUMO_ALG_PRICE_FEE  = 'E',
    SUMO_ALG_PRICE_TIME = 'T',
    SUMO_ALG_PRICE_SIZE = 'Z',
    SUMO_ALG_UNKNOWN    = '\0'
};

enum SpecialOrderFlags
{
    PEG_TO_PRIMARY              = 'R',
    PEG_TO_MARKET               = 'P',
    DISCRETION_FLAG             = 'N',
//    HUNTER_FLAG                 = 'H',
//    AON                         = 'A',
    DIRECTED                    = 'D',
    PREFEREED                   = 'P',
    EXCHANGE_CROSSED_FLAG       = 'X',
    EXCHANGE_THRU_DIRECTED      = 'T',
    EXCHANGE_THRU_NOT_DIRECTED  = 'U'
};
/*
enum SpecialOrderTypes
{
    PEG_PRIMARY,
    PEG_MARKET,
    DISCRETION,
    HUNTER,
    EXCHANGE_THRU,
    EXCHANGE_ONLY,
    DIRECTED_PREFERRED,
    EXCHANGE_CROSSED,
    MKT_LIMIT
};
*/
enum RTickType
{
    NOTICK = 0,
    UPTICK,
    DOWNTICK
};

enum RMarketStatus
{
	MARKET_OPEN,
	MARKET_CLOSE
};

enum TImbalanceFlags
{
    FLAG_UNDEFINED_IMBALANCE    = -1,

    FLAG_IMBALANCE_BUY          = 7,
    FLAG_IMBALANCE_SELL         = 8,

    FLAG_MOC_IMBALANCE_BUY      = 9,
    FLAG_MOC_IMBALANCE_SELL     = 10,

    FLAG_NO_MOC_IMBALANCE       = 11,
    FLAG_NO_IMBALANCE           = 12
};

enum
{
	FLAG_IONLY				= 0x01,
	FLAG_NEGOTIABLE			= 0x02,
	FLAG_ROUTED				= 0x04,
	FLAG_PEGOPENING			= 0x08,
	FLAG_RESET				= 0x10,
	FLAG_REVTYPE_SIDE		= 0x20,
	FLAG_REVTYPE_SIZE		= 0x40,
	FLAG_REVTYPE_MINSIZE	= 0x80,
	FLAG_REVTYPE_PRICE		= 0x100,
	FLAG_REVTYPE_NEGOTIABLE	= 0x200,
	FLAG_REVTYPE_ROUTED		= 0x400,
	FLAG_REVTYPE_PEGTYPE	= 0x800,
	FLAG_REVTYPE_MASK		= 0xfe0,

	FLAG_MAX_VALUE			= 0x3fffff
};

enum StatusFlags
 {
  //agregation:two lower bits
  SF_AGR_NONE  = 0x00,
  SF_AGR_INSIDE = 0x01,
  SF_AGR_FULL  = 0x02,
  SF_AGR_MASK  = 0x03,
 
        //attributed quote
        SF_ATTRIB_TYPE_INSIDE   = 0x04,
        SF_ATTRIB_TYPE_FULL     = 0x08,
        SF_ATTRIB_TYPE_MASK     = 0x0C,
  
  //market maker moves
  SF_MOVE_UNDEFINED=0x00,
  //downside moves
  SF_MOVE_JOINS_ASK  = 0x01 << 4,  
  SF_MOVE_LEAVES_BID  = 0x02 << 4,
  SF_MOVE_REFRESHES_ASK = 0x03 << 4,
  SF_MOVE_NEW_LOW_ASK  = 0x04 << 4,
  SF_MOVE_DROPS_BID  = 0x05 << 4,
  SF_MOVE_BID_TO_ASK  = 0x06 << 4,
  SF_MOVE_BID_TICK_DOWN = 0x07 << 4,
  SF_MOVE_ASK_TICK_DOWN   = 0x08 << 4,
  SF_MOVE_DOWNSIDE_MASK = 0x0F << 4,
  //upside moves
  SF_MOVE_LEAVES_ASK  = 0x01 << 10,
  SF_MOVE_JOINS_BID  = 0x02 << 10,
  SF_MOVE_REFRESHES_BID = 0x03 << 10,
  SF_MOVE_NEW_HIGH_BID = 0x04 << 10,
  SF_MOVE_LIFTS_ASK  = 0x05 << 10,
  SF_MOVE_ASK_TO_BID  = 0x06 << 10,
  SF_MOVE_BID_TICK_UP  = 0x07 << 10,
  SF_MOVE_ASK_TICK_UP  = 0x08 << 10,
  SF_MOVE_UPSIDE_MASK  = 0x0F << 10
 };

enum QuoteConditions
{
   QUOTE_CONDITION_SlowQuoteOnOfferSide                         = 'A', // Slow Quote on Offer Side
   QUOTE_CONDITION_SlowQuoteOnBidSide                           = 'B', // Slow Quote on Bid Side
   QUOTE_CONDITION_Closing                                      = 'C', // Closing
   QUOTE_CONDITION_NewsDissemination                            = 'D', // News Dissemination
   QUOTE_CONDITION_SlowQuoteDueToAnLRPOrGapQuoteOnTheBidSide    = 'E', // Slow Quote Due to an LRP or Gap Quote on the Bid Side (Redefined)
   QUOTE_CONDITION_SlowQuoteDueToAnLRPOrGapQuoteOnTheOfferSide  = 'F', // Slow Quote Due to an LRP or Gap Quote on the Offer Side (Redefined)
   QUOTE_CONDITION_TradingRangeIndication                       = 'G', // Trading Range Indication
   QUOTE_CONDITION_SlowQuoteOnTheBidAndOfferSides               = 'H', // Slow Quote on the Bid and Offer Sides
   QUOTE_CONDITION_OrderImbalance                               = 'I', // Order Imbalance
   QUOTE_CONDITION_DueToRelatedSecurityNewsDissemination        = 'J', // Due To Related Security-News Dissemination
   QUOTE_CONDITION_DueToRelatedSecurityNewsPending              = 'K', // Due To Related Security-News Pending
   QUOTE_CONDITION_ClosedMarketMaker                            = 'L', // Closed Market Maker (NASD)
   QUOTE_CONDITION_AdditionalInformation                        = 'M', // Additional Information
   QUOTE_CONDITION_NonFirmQuote                                 = 'N', // Non-Firm Quote
   QUOTE_CONDITION_OpeningQuote                                 = 'O', // Opening Quote
   QUOTE_CONDITION_NewsPending                                  = 'P', // News Pending
   QUOTE_CONDITION_AdditionalInformationDueToRelatedSecurity    = 'Q', // Additional Information-Due To Related Security
   QUOTE_CONDITION_Regular                                      = 'R', // Regular (NASD Open)
   QUOTE_CONDITION_DueToRelatedSecurity                         = 'S', // Due To Related Security
   QUOTE_CONDITION_Resume                                       = 'T', // Resume
   QUOTE_CONDITION_LRPOrOther                                   = 'U', // Slow Quote Due to a   - NYSE Liquidity Replenishment Point (LRP), 
                                                                                           //   - Amex Tolerance Breach (Spread, Momentum or Gap Trade Tolerance), or 
                                                                                           //   - Gap Quote on Both the Bid and Offer Sides (New)
   QUOTE_CONDITION_InViewOfCommon                                       = 'V', // In View of Common
   QUOTE_CONDITION_SlowQuoteDueToSetSlowListOnBothTheBidAndOfferSides   = 'W', // Slow Quote Due to Set Slow List on Both the Bid and Offer Sides (New)
   QUOTE_CONDITION_EquipmentChangeover                                  = 'X', // Equipment Changeover
   QUOTE_CONDITION_NoOpenNoResume                                       = 'Z', // No Open/No Resume

   QUOTE_CONDITION_UNKNOWN = '\0'
};
/*
enum FlagsLRP
{
	LRP_SIDE = 1,
};
*/
#endif
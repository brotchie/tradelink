#ifndef _TDATYPES_FILE
#define _TDATYPES_FILE

#pragma pack(push) 
#pragma pack(1)

typedef int                 LongBool;

//------------------------------------------------------
// these constants are the subscription type for subscribing to symbols
//------------------------------------------------------
#define   TDAPI_L1         1
#define   TDAPI_L2         2
#define   TDAPI_ISLD       3
#define   TDAPI_ARCA       4
#define   TDAPI_News       5
#define   TDAPI_Actives    6
#define   TDAPI_LII        7
#define   TDAPI_TV         8
#define   TDAPI_BARS       9
#define   TDAPI_ACCTMSG    10
#define   TDAPI_TIMESALES  11

//------------------------------------------------------
// the type of order submit for the OrderCommand
//------------------------------------------------------
#define TDAPI_SUBMIT_New            1
#define TDAPI_SUBMIT_Cancel         3
#define TDAPI_SUBMIT_Modify         5

//------------------------------------------------------
// the callback function type (Event IDs)
//------------------------------------------------------
#define TDAPI_Callback_GotBackfill        1
#define TDAPI_Callback_GotBalances        2
#define TDAPI_Callback_GotMessage         3
#define TDAPI_Callback_GotNews            4
#define TDAPI_Callback_GotPositions       5
#define TDAPI_Callback_GotTransactions    6
#define TDAPI_Callback_L1Data             7
#define TDAPI_Callback_L2Data             8
#define TDAPI_Callback_OrderAck           9
#define TDAPI_Callback_StatusChange       10
#define TDAPI_Callback_Ping               11
#define TDAPI_Callback_ActivesData        12
#define TDAPI_Callback_L1DataPtr          13
#define TDAPI_Callback_GotBalancesPtr     14
#define TDAPI_Callback_GotLastOrderStatus 15
#define TDAPI_Callback_GotSnapQuote       16
#define TDAPI_Callback_ActivesDataPtr     17
#define TDAPI_Callback_GotPandL           18
#define TDAPI_Callback_TVL2Data           19
#define TDAPI_Callback_OptionChain        20
#define TDAPI_Callback_GotHistBackfill    21
#define TDAPI_Callback_OHLCBar            22
#define TDAPI_Callback_OptionChainWithQuotes 23
#define TDAPI_Callback_OHLCBarPtr         24
#define TDAPI_Callback_MessageKey         25
#define TDAPI_Callback_AcctMessage        26
#define TDAPI_Callback_GotPositionsWithQuotes 27
#define TDAPI_Callback_AcctHistory        28
#define TDAPI_Callback_TimeSales      29

//------------------------------------------------------
// TAPI_Mask_XXX - the masks to see what changed in the TAPIStockQuote record
//------------------------------------------------------
#define TAPI_MASK_Bid             0x000000000001
#define TAPI_MASK_Ask             0x000000000002
#define TAPI_MASK_Last            0x000000000004
#define TAPI_MASK_High            0x000000000008
#define TAPI_MASK_Low             0x000000000010
#define TAPI_MASK_PrevClose       0x000000000020
#define TAPI_MASK_Open            0x000000000040
#define TAPI_MASK_Change          0x000000000080
#define TAPI_MASK_High52          0x000000000100
#define TAPI_MASK_Low52           0x000000000200
#define TAPI_MASK_Tick            0x000000000400
#define TAPI_MASK_Volume          0x000000000800
#define TAPI_MASK_BidSize         0x000000001000
#define TAPI_MASK_AskSize         0x000000002000
#define TAPI_MASK_Lastsize        0x000000004000
#define TAPI_MASK_BidExch         0x000000008000
#define TAPI_MASK_AskExch         0x000000010000
#define TAPI_MASK_Exchange        0x000000020000
#define TAPI_MASK_TradeTime       0x000000040000
#define TAPI_MASK_QuoteTime       0x000000080000
#define TAPI_MASK_TradeDate       0x000000100000
#define TAPI_MASK_QuoteDate       0x000000200000
#define TAPI_MASK_IslandBid       0x000000400000
#define TAPI_MASK_IslandAsk       0x000000800000
#define TAPI_MASK_IslandVol       0x000001000000
#define TAPI_MASK_IslandBidSize   0x000002000000
#define TAPI_MASK_IslandAskSize   0x000004000000
#define TAPI_MASK_Name            0x000008000000
#define TAPI_MASK_Marginable      0x000010000000
#define TAPI_MASK_Shortable       0x000020000000
#define TAPI_MASK_Volatility      0x000040000000
#define TAPI_MASK_TradeID         0x000080000000
#define TAPI_MASK_Digits          0x000100000000
#define TAPI_MASK_PE              0x000200000000
#define TAPI_MASK_Dividend        0x000400000000
#define TAPI_MASK_Yield           0x000800000000
#define TAPI_MASK_NAV             0x001000000000
#define TAPI_MASK_Fund            0x002000000000
#define TAPI_MASK_ExchName        0x004000000000
#define TAPI_MASK_DivDate         0x008000000000
#define TAPI_MASK_OpenInt         0x010000000000
#define TAPI_MASK_TimeVal         0x020000000000
#define TAPI_MASK_Delta           0x080000000000
#define TAPI_MASK_Gamma           0x100000000000
#define TAPI_MASK_Theta           0x200000000000
#define TAPI_MASK_Vega            0x400000000000
#define TAPI_MASK_Rho             0x800000000000
#define TAPI_MASK_MH_Last     0x1000000000000;
#define TAPI_MASK_MH_LastSize  0x2000000000000;
#define TAPI_MASK_MH_TradeDate  0x4000000000000;
#define TAPI_MASK_MH_TradeTime 0x8000000000000;
#define TAPI_MASK_MH_Change 0x10000000000000;
#define TAPI_MASK_MH_IsQuote  0x20000000000000;
#define TAPI_MASK_MH_IsTrade  0x40000000000000;
#define TAPI_MASK_InOutOfMoney   0x80000000000000;

#define TAPI_MASK_StrikePrice    0x100000000000000;
#define TAPI_MASK_ContractType   0x200000000000000;
#define TAPI_MASK_UnderlyingSymbol 0x400000000000000;
#define TAPI_MASK_ExpMonth       0x800000000000000;
#define TAPI_MASK_DaysToExp      0x1000000000000000;

//------------------------------------------------------
// TAPI_FieldID_XXXX - the field IDs for L1
//------------------------------------------------------
#define TAPI_FieldID_Symbol	  0
#define TAPI_FieldID_Bid	  1
#define TAPI_FieldID_Ask          2
#define TAPI_FieldID_Last	  3
#define TAPI_FieldID_BidSize	  4
#define TAPI_FieldID_AskSize	  5
#define TAPI_FieldID_BidExch	  6
#define TAPI_FieldID_AskExch      7
#define TAPI_FieldID_Volume	  8
#define TAPI_FieldID_LastSize	  9
#define TAPI_FieldID_TradeTime	  10
#define TAPI_FieldID_QuoteTime	  11
#define TAPI_FieldID_High	  12
#define TAPI_FieldID_Low	  13
#define TAPI_FieldID_Tick	  14
#define TAPI_FieldID_PrevClose    15
#define TAPI_FieldID_Exchange	  16
#define TAPI_FieldID_Marginable	  17
#define TAPI_FieldID_Shortable	  18
#define TAPI_FieldID_IslandBid	  19
#define TAPI_FieldID_IslandAsk	  20
#define TAPI_FieldID_IslandVol	  21
#define TAPI_FieldID_QuoteDate	  22
#define TAPI_FieldID_TradeDate	  23
#define TAPI_FieldID_Volatility	  24
#define TAPI_FieldID_Name         25
#define TAPI_FieldID_TradeID	  26
#define TAPI_FieldID_Digits	  27
#define TAPI_FieldID_Open	  28
#define TAPI_FieldID_Change	  29
#define TAPI_FieldID_High52       30
#define TAPI_FieldID_Low52        31
#define TAPI_FieldID_PE	          32
#define TAPI_FieldID_Dividend     33
#define TAPI_FieldID_Yield        34
#define TAPI_FieldID_IslandBidSize  35
#define TAPI_FieldID_IslandAskSize  36
#define TAPI_FieldID_NAV	  37
#define TAPI_FieldID_FUND	  38
#define TAPI_FieldID_ExchName     39
#define TAPI_FieldID_DivDate      40

#define TAPI_Opt_FieldID_Symbol	  0
#define TAPI_Opt_FieldID_Name     1
#define TAPI_Opt_FieldID_Bid      2
#define TAPI_Opt_FieldID_Ask      3
#define TAPI_Opt_FieldID_Last	  4
#define TAPI_Opt_FieldID_High	  5
#define TAPI_Opt_FieldID_Low	  6
#define TAPI_Opt_FieldID_PrevClose 7
#define TAPI_Opt_FieldID_Volume	  8
#define TAPI_Opt_FieldID_OpenInt  9
#define TAPI_Opt_FieldID_Volatility 10
#define TAPI_Opt_FieldID_QuoteTime 12
#define TAPI_Opt_FieldID_InTheMoney 13
#define TAPI_Opt_FieldID_Open     19
#define TAPI_Opt_FieldID_BidSize   20
#define TAPI_Opt_FieldID_AskSize   21
#define TAPI_Opt_FieldID_LastSize  22
#define TAPI_Opt_FieldID_Change    23
#define TAPI_Opt_FieldID_Strike    24
#define TAPI_Opt_FieldID_ContractType 25
#define TAPI_Opt_FieldID_Underlying 26
#define TAPI_Opt_FieldID_ExpMonth  27
#define TAPI_Opt_FieldID_TimeVal  29
#define TAPI_Opt_FieldID_DaysToExp 31
#define TAPI_Opt_FieldID_Delta    32
#define TAPI_Opt_FieldID_Gamma    33
#define TAPI_Opt_FieldID_Theta    34
#define TAPI_Opt_FieldID_Vega     35
#define TAPI_Opt_FieldID_Rho      36

#define TAPI_FieldID_NAV	  37
#define TAPI_FieldID_FUND	  38
#define TAPI_FieldID_ExchName     39
#define TAPI_FieldID_DivDate      40

#define TAPI_FieldID_MH_Last      41
#define TAPI_FieldID_MH_LastSize  42
#define TAPI_FieldID_MH_TradeDate 43
#define TAPI_FieldID_MH_TradeTime 44
#define TAPI_FieldID_MH_Change    45
#define TAPI_FieldID_MH_IsQuote   46
#define TAPI_FieldID_MH_IsTrade   47




//------------------------------------------------------
// TAPIAccountInfo - the record that holds miscellaneous information about the account
//------------------------------------------------------
typedef struct TAPIAccountInfo_s{
   char         AccountID[32];         // Account ID
   char         Description[64];       // Description of the account
   LongBool 	Associated;
   LongBool 	Unified;
   LongBool 	Express;
   LongBool 	OptDirect;
   LongBool 	StkDirect;
   LongBool 	APEX;
   LongBool 	Level2;
   LongBool 	StockTrading;
   LongBool 	MarginTrading;
   LongBool 	OptionTrading;
   LongBool 	StreamingNews;
   LongBool 	Level1;
   int 	Timeout;
   LongBool             NYSEDelayed;
   LongBool 	NASDAQDelayed;
   LongBool 	OPRADelayed;
   LongBool 	AMEXDelayed;
   char 	Professional[4];
   char         OptTradingAuth[8];
   char         DisplayName[32];
   char         Reserved[36];
} TAPIAccountInfo;


//------------------------------------------------------
// TAPICharge - the record that holds charges for a current account message (account message can have several charges)
//------------------------------------------------------
typedef struct TAPICharge_s{
   char         Desc[32];
   double       Amount;
} TAPICharge;

//------------------------------------------------------
// these constants are the account message types
//------------------------------------------------------
#define TDAPI_ACCTMG_UNKNOWN              0
#define TDAPI_ACCTMG_SUBSCRIBED         1
#define TDAPI_ACCTMG_ERROR                    2
#define TDAPI_ACCTMG_BrokenTrade           3
#define TDAPI_ACCTMG_ManualExecution    4
#define TDAPI_ACCTMG_OrderActivation        5
#define TDAPI_ACCTMG_OrderCancelReplaceRequest   6
#define TDAPI_ACCTMG_OrderCancelRequest   7
#define TDAPI_ACCTMG_OrderEntryRequest   8
#define TDAPI_ACCTMG_OrderFill           9
#define TDAPI_ACCTMG_OrderPartialFill    10
#define TDAPI_ACCTMG_OrderRejection    11
#define TDAPI_ACCTMG_TooLateToCancel   12
#define TDAPI_ACCTMG_UROUT              13


//------------------------------------------------------
// TAPIAccountMsg - a message sent when something happens - for example, order execution
//------------------------------------------------------
typedef struct TAPIAccountMsg_s{
   char         MessType[64];
   char         AccountID[32];

   char         OrderNumber[32];
   char         OrigOrderNumber[32];

   char         CUSIP[12];
   char         Symbol[32];
   char         SecurityType[64];
   char         SecurityCateg[64];

   char         OrderType[32];
   char         OrderSide[32];
   char         TIFSession[32];
   char         TIFExpiration[20];
   char         EnteredDateTime[20];

   double    LimitPrice;
   double    Quantity ;
   double    PendingCancQty;
   double    CancelledQty;
   double    RemainingQty;

   char         SpecConditions[32];
   char         MarketCode[32];
   double       ActivationPrice;
   char         LastUpdated[20];
   int          RejectCode;
   char         RejectReason[256];
   char         ReportedBy[64];
   char         CompletionCode[32];

   char         ExecID[32];
   char         ExecType[32];
   char         ExecTimeStamp[20];
   double      ExecQty;
   double      ExecPrice;
   LongBool  ExecIsAverage;
   double       ExecRemains;

  double       TSAmount;
  char           TSMethod[8];

  int             MessTypeCode;

   char         Reserved[492]; 

   int          NumCharges;
   TAPICharge   Charge[100];
} TAPIAccountMsg;



//------------------------------------------------------
// TAPIQuote - the record that holds current quote data for the symbol
//------------------------------------------------------
typedef struct TAPIQuote_s{
  char     Symbol[32];        //symbol
  __int64  ChangeFiledMask1;  // mask that determines which fields have changed in this quote
  __int64  ChangeFiledMask2;  // extended mask that determines which fields have changed in this quote

  double   Bid;
  double   Ask;
  double   Last;
  double   High;
  double   Low;
  double   PrevClose;
  double   Open;
  double   Change;
  double   High52;
  double   Low52;

  int      Tick;        // ASCII - either U or D

  __int64  Volume;

  int      BidSize;  
  int      AskSize;  
  int      LastSize;  

  int      BidExchange;   // ASCII - one character
  int      AskExchange;   // ASCII - one character
  int      Exchange;      // ASCII - one character

  int      TradeTime;   // seconds since midnight, eastern
  int      QuoteTime;   // seconds since midnight, eastern
  int      TradeDate;   // days since 1/1/1970
  int      QuoteDate;   // days since 1/1/1970

  double   IslandBid;
  double   IslandAsk;
  __int64  IslandVolume;
  int      IslandBidSize;  
  int      IslandAskSize;  

  char     Name[64];

  LongBool Marginable;
  LongBool Shortable;

  double   Volatility;
  int      TradeID;     // ASCII - one character
  int      Digits;
  double   PE;
  double   Dividend;
  double   Yield;
  double   NAV;
  double   Fund;

  char     ExchangeName[20];
  char     DividendDate[12];

  int      OpenInterest;

  double   TimeValue;
  double   Delta;
  double   Gamma;
  double   Theta;
  double   Vega;
  double   Rho;

  double   fMH_Last;
  int      nMH_LastSize;
  int      tMH_TradeDate;
  int      tMH_TradeTime;
  double   fMH_Change;
  LongBool bMH_IsQuote;
  LongBool bMH_IsTrade;

  double  fInOutOfMoney;
  double  fStrikePrice;
  char     ContractType[4];
  char     UnderlyingSymbol[16];
  int nExpMonth;
  int nDaysToExp;
  

  char     Reserved[68];
} TAPIQuote;


//------------------------------------------------------
// TAPIAccountHistoryRecord - the record that holds an account transaction
//------------------------------------------------------
typedef struct TAPIAccountHistoryRecord_s{
        char DateTime[20];
        char RecordType[4];
        char SubType[8];
        char BuySellCode[4];
        char AssetType[4];
        char Symbol[32];
        char Description[64];
        char CUSIP[12];
        double Price;
        double Quantity;
        double TransValue;
        double Commission;
        double Fees;
        double AdditionalFees;
        LongBool CashBalanceEffect;
        char TransactionID[32];
        char OrderNumber[32];
        char OpenClose[4];
        char OptionExpiration[28];
        char OptionUnderlying[32];
        char OptionType[4];
        double OptionStrike;
        double AccruedInterest;
        char ParentChild[4];
        double SharesBefore;
        double SharesAfter;
        double OtherCharges;
        double RedemptionFee;
        double cdscFee;
        double BondInterestRate;
        char BondMaturityDate[20];
        char SplitRatio[16];
        char ExecutedDateTime[20];
        char Reserved[108];
}TAPIAccountHistoryRecord;

//----------------------------------------------------------
// TAPIPosition - the record describing a single account position
//----------------------------------------------------------
typedef struct TAPIPosition_s{
   char   AccountID[32];         // Account ID
   char   Symbol[32];            // position's symbol
   char   SymbolWithPrefix[32];  // position's symbol with prefix
   char   Name[64];              // Company name or symbol's description
   int    AssetType;             // ASCII: E - Equity or ETF, F - Mutual Fund, I - Index, O - Option, B - Bond
   char   CUSIP[12];         
   int    AccountType;           // 1 - Cash 2 - Margin 3 - Short 4 - Dividend/Interest
   double ClosePrice;
   char   PositionType[12];      // LONG or SHORT
   double AveragePrice;
   double Quantity;
   char   PutCall[4];            // C or P - only if option
   double MaintReq;              // Maintenance Requirement
   char   Reserved[28];
} TAPIPosition;

//----------------------------------------------------------
// TAPIPosition - the record describing a single account position
//----------------------------------------------------------
typedef struct TAPIPositionWithQuote_s{
   char   AccountID[32];         // Account ID
   char   Symbol[32];            // position's symbol
   char   SymbolWithPrefix[32];  // position's symbol with prefix
   char   Name[64];              // Company name or symbol's description
   int    AssetType;             // ASCII: E - Equity or ETF, F - Mutual Fund, I - Index, O - Option, B - Bond
   char   CUSIP[12];         
   int    AccountType;           // 1 - Cash 2 - Margin 3 - Short 4 - Dividend/Interest
   double ClosePrice;
   char   PositionType[12];      // LONG or SHORT
   double AveragePrice;
   double Quantity;
   char   PutCall[4];            // C or P - only if option
   double MaintReq;              // Maintenance Requirement
   char   Reserved[28];
   TAPIQuote Quote;
} TAPIPositionWithQuote;


typedef struct TAPIBalances_s{
        char          AccountID[32];     //account ID
        LongBool      DayTrader;         // TRUE if defined as a daytrader
        int           RoundTrips;
        LongBool      ClosingTransactionsOnly;

        LongBool   InCall;
        LongBool   InPotentialCall;

        double      CashBalance;
        double      CashBalanceChange;
        double      MoneyMarketBalance;
        double      MoneyMarketBalanceChange;
        double      LongStockValue;
        double      LongStockValueChange;
        double      LongOptionValue;
        double      LongOptionValueChange;
        double      ShortOptionValue;
        double      ShortOptionValueChange;
        double      MutualFundValue;
        double      MutualFundValueChange;
        double      BondValue;
        double      BondValueChange;
        double      AccountValue;
        double      AccountValueChange;
        double      PendingDeposits;
        double      PendingDepositsChange;
        double      ShortBalance;
        double      ShortBalanceChange;
        double      MarginBalance;
        double      MarginBalanceChange;
        double      ShortStockValue;
        double      ShortStockValueChange;
        double      LongMarginableValue;
        double      LongMarginableValueChange;
        double      ShortMarginableValue;
        double      ShortMarginableValueChange;
        double      MarginEquity;
        double      MarginEquityChange;
        double      EquityPercentage;
        double      EquityPercentageChange;
        double      MaintenanceRequirement;
        double      MaintenanceRequirementChange;
        double      MaintenanceCallValue;
        double      MaintenanceCallValueChange;
        double      RegulationTCallValue;
        double      RegulationTCallValueChange;
        double      DayTradingCallValue;
        double      DayTradingCallValueChange;
        double      CashDebitCallValue;
        double      CashDebitCallValueChange;

        double      CashForWithdrawal;
        double      UnsettledCash;
        double      NonMarginableFunds;

        double      DayEquityCallValue;
        double      OptionBuyingPower;
        double      StockBuyingPower;
        double      DayTradingBuyingPower;
        double      AvailableFundsForTrading;
        double      Reserved[40];
} TAPIBalances;

typedef struct TAPIActivesItem_s{
  char    Symbol[12];        // the active symbol
  double  Exe;
  __int64 NumTrades;
} TAPIActivesItem;

typedef struct TAPIActives_s{
      char   ActivesType[16];     // the type of actives record - of form "EXCHANGE-PERIOD"
      int    SampleDuration;      // the period
      char   StartTime[12];       // start timestamp
      char   DisplayTime[12];     // display timestamp

      TAPIActivesItem  ActiveBuyTrades[10];
      TAPIActivesItem  ActiveSellTRades[10];
      TAPIActivesItem  ActiveBuyShares[10];
      TAPIActivesItem  ActiveSellShares[10];
      TAPIActivesItem  ActiveTrades[10];
      TAPIActivesItem  ActiveShares[10];
} TAPIActives; 

typedef struct TAPIFill_s{
   char     AccountID[32];     // Account ID
   char     OrderNumber[32];   // Order Number to which the Fill belongs
   char     ID[32];            // Fill ID
   char     Symbol[32];        // symbol
   char     Action[4];         // same as in TAPITransaction
   double   Quantity;
   double   Price;
   double   Commission;
   char     TimeStamp[16];     // YYYYMMDDHHMMSS
}  TAPIFill;

typedef struct TAPITransaction_s{
   char   AccountID[32];           // Account ID
   char   OrderNumber[32];         // Order Number
   char   RootOrderNumber[32];     // Root Order Number
   char   ParentOrderNumber[32];   // Parent Order Number

   LongBool   Cancelable;
   LongBool   Editable;
   LongBool   ComplexOption;
   LongBool   Enhanced;

   char   EnhancedOrderType[32];   
   char   Relationship[32];   
   char   OptionStrategy[32];

   char   OrderType[4];      // M - Market L - Limit S - Stop X - Stop Limit D - Debit C - Credit T - Trailing Stop EX - Exercise Option

   char   Symbol[32];             // symbol
   char   SymbolWithPrefix[32];   // symbol with prefix
   char   SecurityType[64];       // the type of the security traded
                                       // Common
                                       // Preferred
                                       // Convertible Preferred
                                       // Rights
                                       // Warrant
                                       // Units
                                       // Mutual Fund
                                       // Limited Partnership
                                       // Call Option
                                       // Put Option
                                       // Bank Call Option
                                       // Escrow
                                       // Corporate Bond
                                       // Convertible Bond
                                       // Municipal Bond
                                       // Short Term Paper
                                       // Bond Unit
                                       // Municipal Assessment District
                                       // US Treasury Bill
                                       // US Treasury Note
                                       // US Treasury Bond
                                       // Other Government
                                       // US Treasury Zero Coupon
                                       // Government Mortgage

   int      AssetType;     // ASCII - E- Equity or ETF,  F - Mutual FUnd, O - Option
   char     Action[4];    // B - Buy S - Sell SS - Short Sell BC - Buy to Cover E - Exchange EX - Exercise Option
   char     TradeType[4]; // 1 — Normal Market 2 — External Hour Market 4 — German Market 8 — AM Session 16 — Seamless Session

   double   InitialQuantity;
   double   RemainingQuantity;
   double   LimitPrice;
   double   StopPrice;

   char     SpecialConditions[4];   // AON, DNR, FOK


   char     TIFSession[12];         // GX=GTC+Ext D=Day DX=Day+Ext GTD=GTC
   char     TIFExpiration[12];      // expiration date
   char     DisplayStatus[16];
   char     RoutingStatus[16];

   char     ReceivedDateTime[16];   // YYYYMMDDHHMMSS
   char     ReportedDateTime[16];   // YYYYMMDDHHMMSS
   char     CancelDateTime[16];     // YYYYMMDDHHMMSS

   char     DestRoutingMode[16];   
   char     DestOptionExchange[16];   
   char     DestResponseDescription[16];   

   char     ActDestRoutingMode[16];   
   char     ActDestOptionExchange[16];   
   char     ActDestResponseDescription[16];   

   int      RoutingDisplaySize;

   int      NumFills;
   double   AverageFillPrice;
   double   TotalFillCommission;
   char     LastExecutionDateTime[16];   // YYYYMMDDHHMMSS

   int      PutCall;
   int      OpenClose;
   char     TrailingStopMethod[8];   // PERCENT or POINTS
   double   FilledQuantity; 
   char     Reserved[84];
} TAPITransaction;   

//------------------------------------------------------
// TAPIOHLCBar - the OHLC bar
//------------------------------------------------------
typedef struct TAPIOHLCBar_s{
  char        Symbol[32];       // symbol
  int         Sequence;         // minutes since 8am eastern
  double      Open;
  double      High;
  double      Low;
  double      Close;
  int         Volume;
  int         Time;         // # of seconds since midnight
  int         Date;         // # of days since 1/1/1970
} TAPIOHLCBar;      	

//------------------------------------------------------
// TAPITimeSalesRecord - the Time/Sales Record
//------------------------------------------------------
typedef struct TAPITimeSalesRecord_s{
  int    TimeStamp;
  double   Price;
  int   Volume;
  int  Sequence;
  int  Updated;
} TAPITimeSalesRecord;      	


//------------------------------------------------------
// TAPIBFItem - one backfill item
//------------------------------------------------------
typedef struct TAPIBFItem_s{
  int         Sequence;         // minutes since 8am eastern
  double      Open;
  double      High;
  double      Low;
  double      Close;
  int         Volume;
  int         Time;         // # of seconds since midnight
  int         Date;         // # of days since 1/1/1970
} TAPIBFItem;      	

//------------------------------------------------------
// TAPIHistItem - one backfill item
//------------------------------------------------------
typedef struct TAPIHistItem_s{
  double      Open;
  double      High;
  double      Low;
  double      Close;
  __int64     Volume;
  int         Date;         // # of days since 1/1/1970
} TAPIHistItem;      	

//------------------------------------------------------
// TAPINOIIData
//------------------------------------------------------
typedef struct TAPINOIIDATA_s{
  int    Typ; // ASCII  O Regular Opening Cross
              //        I Intra-day Opening for IPO Securities
              //        H Intra-day Opening for Halted Securities
              //        C Regular Closing Cross
  int    PairedShares;
  int    ImbalanceShares;
  int    ImbalanceSide; // ASCII  B Buy Side Imbalance
                        //        S Sell Side Imbalance
                        //        N No Imbalance
                        //        O No marketable On Open/Close Orders in NASDAQ
  double CurrentRefPrice;
  double FarIndicative;
  double NearIndicative;
  int PriceVariance; //  ASCII L Less than 1%
                     //        1 1%
                     //        2 2%
                     //        3 3%
                     //        4 4%
                     //        5 5%
                     //        6 6%
                     //        7 7%
                     //        8 8%
                     //        9 9%
                     //        A 10% to 19.99%
                     //        B 20% to 29.99%
                     //        C 30% or Greater
                     //        Space Price Variation Indicator cannot be calculated
} TAPINOIIDATA;


//------------------------------------------------------
// TAPIL2Record
//------------------------------------------------------
typedef struct TAPIL2Record_s{
        char MMID[4];
        double Price;
        int Size;
} TAPIL2Record;

//------------------------------------------------------
// TAPILIIRecord
//------------------------------------------------------
typedef struct TAPILIIRecord_s{
        char MMID[4];
        double Price;
        int Size;
        int TimeStamp;
} TAPILIIRecord;

//------------------------------------------------------
// TAPIOptionChainItem
//------------------------------------------------------
typedef struct TAPIOptionChainItem_s{
        char Symbol[8];
        char CallPut[4];     // C or P
        char ExpDate[12];     // yyyymmdd
        int  ExpDays;	     // # of days to expiration
        char ExpType[4];     // L (leap) or R (regular)
        double Strike;
        char OptType[4];     // ST (standard) or NS (non-standard)
} TAPIOptionChainItem;

typedef struct TAPIOptionChainWQItem_s{
        char Symbol[8];
        char CallPut[4];     // C or P
        char ExpDate[12];     // yyyymmdd
        int  ExpDays;	     // # of days to expiration
        char ExpType[4];     // L (leap) or R (regular)
        double Strike;
        char OptType[4];     // ST (standard) or NS (non-standard)

        char Punctuality[4];    //R or D

        double fBid;
        double fAsk;
        double fLast;
        double fChange;
        double fChangePercent;

        int nVolume;

        int nBidSize; 
		int nAskSize;
        int LastTradeTime;  // seconds since midnight, eastern
        int LastTradeDate;  // days since 1/1/1970


        int nOpenInterest;
        double fDelta;
        double fGamma;
        double fTheta;
        double fVega;
        double fRho;
        double fImpliedVol;
        double fTimeValueInd;
        double fMultiplier;

        char InOutTheMoney[4];  // IN, OUT or NR (near)
        double fTheoreticalVal;

        char Reserved[40];

} TAPIOptionChainWQItem;

//------------------------------------------------------
// TAPINewsHistItem - the news history item
//------------------------------------------------------
typedef struct TAPINewsItem_s{
        char HeadlineID[24];
        char Headline[256];
        int Timestamp;    // # of seconds since 1/1/1970 in GMT
        char Source[32];
        char URL[512];
} TAPINewsItem;

//------------------------------------------------------
// TDAAPIStreamStatus - the status of the TDAAPI thread
//------------------------------------------------------
#define API_STREAMSTATUS_Idle                            1
#define API_STREAMSTATUS_LoggingIn                       2
#define API_STREAMSTATUS_LoginFailed                     3
#define API_STREAMSTATUS_LoggedIn                        4
#define API_STREAMSTATUS_StartedMainRequest              5
#define API_STREAMSTATUS_Running                         6
#define API_STREAMSTATUS_ConnectionDied                  7
#define API_STREAMSTATUS_ChunkTimeOut                    8
#define API_STREAMSTATUS_SystemTemporarilyUnavailable    9
#define API_STREAMSTATUS_LogoutFailed                    10
#define API_STREAMSTATUS_LoggedOut                       11
#define API_STREAMSTATUS_KeepAliveValid                  12
#define API_STREAMSTATUS_KeepAliveInvalid                13


//===========================================================================
// the set of callback events that the TTDAAPI control calls. These callbacks
// will always be called in the context of the main thread (wrapped in Synchronize).
//=======================================================================================

typedef void WINAPI TAPIActivesDataEvent(int OType, int OPtr, const char *Symbol, TAPIActives Actives);
typedef void WINAPI TAPIGotBackfillEvent(int OType, int OPtr, const char *Sym, int Tot, TAPIBFItem *Ptr, const char *Err);
typedef void WINAPI TAPIGotHistBackfillEvent(int OType, int OPtr, const char *Sym, int Tot, TAPIHistItem *Ptr, const char *Err);
typedef void WINAPI TAPIGotBalancesEvent(int OType, int OPtr, TAPIBalances B);
typedef void WINAPI TAPIGotLastOrderStatus(int OType, int OPtr, const char *Acc, const char *LastOrderStatus);
typedef void WINAPI TAPIGotMessageEvent(int OType, int OPtr, const char *Mess);
typedef void WINAPI TAPIGotNewsEvent(int OType, int OPtr, const char *Sym, int Tot, TAPINewsItem *Ptr, const char *Err);
typedef void WINAPI TAPIGotPositionsEvent(int OType, int OPtr, int Tot, TAPIPosition *Ptr);
typedef void WINAPI TAPIGotPositionsWithQuotesEvent(int OType, int OPtr, int Tot, TAPIPositionWithQuote *Ptr);
typedef void WINAPI TAPIGotSnapQuoteEvent(int OType, int OPtr, TAPIQuote *Ptr);
typedef void WINAPI TAPIGotTransactionEvent(int OType, int OPtr, int TotOrders, TAPITransaction *Trans, int TotFills, TAPIFill *Fills);
typedef void WINAPI TAPIL1DataEvent(int OType, int OPtr, TAPIQuote Quote);
typedef void WINAPI TAPIL2DataEvent(int OType, int OPtr, const char *Sym, int NumBidRows, int NumBids, int NumAskRows, int NumAsks, TAPIL2Record *Bids, TAPIL2Record *Asks);
typedef void WINAPI TAPITVL2DataEvent(int OType, int OPtr, const char *Sym, int NumBids, int NumAsks, TAPILIIRecord *Bids, TAPILIIRecord *Asks, TAPINOIIDATA *NOII);
typedef void WINAPI TAPIOrderAckEvent(int OType, int OPtr, const char *AckType, const char *AckStr);
typedef void WINAPI TAPIPingEvent(int OType, int OPtr, char H);
typedef void WINAPI StatusChangeEvent(int OType, int OPtr, int OldStatus, int NewStatus);
typedef void WINAPI TAPIGotPandLEvent(int OType, int OPtr, const char *Acc, double PandL, double RealizedPandL,double UnrealizedPandL);
typedef void WINAPI TAPIGotOHLCBarEvent(int OType, int OPtr, TAPIOHLCBar Bar);
typedef void WINAPI TAPIGotOHLCBarPtrEvent(int OType, int OPtr, TAPIOHLCBar *Bar);
typedef void WINAPI TAPIGotChainEvent(int OType, int OPtr, const char *Sym, int Tot, TAPIOptionChainItem *Chain);
typedef void WINAPI TAPIGotChainWQEvent(int OType, int OPtr, const char *Sym, int Tot, TAPIOptionChainWQItem *Chain);

typedef void WINAPI TAPIGotMessageKeyEvent(int OType,int OPtr, const char *Acct, const char *MsgKey);
typedef void WINAPI TAPIAccountMessageEvent(int OType,int OPtr, const char *MsgType, const char *MsgKey, TAPIAccountMsg *Msg);
typedef void WINAPI TAPIGotAccountHistory(int OType,int OPtr, int NumRec, char *Acct, char *FromDate, char *ToDate, char *ReqType, TAPIAccountHistoryRecord *AccHist, const char *Err);
typedef void WINAPI TAPIGotMessageKeyEvent(int OType,int OPtr, const char *Acct, const char *MsgKey);
typedef void WINAPI TAPITimeSalesDataEvent(int OType,int OPtr, const char *Sym, TAPITimeSalesRecord TandS);


#pragma pack(pop) 
       
#endif //_TDATYPES_FILE

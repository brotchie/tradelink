using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerNxCore
{
        public enum nxST_EXCHANGE 
        {
             NQEX = 1,
             NQAD = 2,
             NYSE = 3,
             AMEX = 4,
             CBOE = 5,
             ISEX = 6,
             PACF = 7,
             CINC = 8,
             PHIL = 9,
             OPRA = 10,
             BOST = 11,
             NQNM = 12,
             NQSC = 13,
             NQBB = 14,
             NQPK = 15,
             NQAG = 16,
             CHIC = 17,
             TSE  = 18,
             CDNX = 19,
             CME  = 20,
             NYBT = 21,
             NYBA = 22,
             COMX = 23,
             CBOT = 24,
             NYMX = 25,
             KCBT = 26,
             MGEX = 27,
             WCE  = 28,
             ONEC = 29,
             DOWJ = 30,
             NNEX = 31,
             SIMX = 32,
             FTSE = 33,
             EURX = 34,
             ENXT = 35,
             DTN  = 36,
             LMT  = 37,
             LME  = 38,
             IPEX = 39,
             MSE  = 40,
             WSE  = 41,
             ISLD = 42,
             MDAM = 43,
             CLRP = 44,
             BARK = 45,
             TULL = 46,
             CQTS = 47,
             HOTS = 48,
             EUUS = 49,
             EUEU = 50,
             ENCM = 51,
             ENID = 52,
             ENIR = 53,
             CFE  = 54,
             PBOT = 55,
             HWTB = 56,
             NQNX = 57,
             BTRF = 58,
             NTRF = 59,
             BATS = 60
        }

        public enum nxST_TRADECONDITION
    {
         Regular             = 0,
         FormT               = 1,
         OutOfSeq            = 2,
         AvgPrc              = 3,
         AvgPrc_Nasdaq       = 4,
         OpenReportLate      = 5,
         OpenReportOutOfSeq  = 6,
         OpenReportInSeq     = 7,
         PriorReferencePrice = 8,
         NextDaySale         = 9,
         Bunched             = 10,
         CashSale            = 11,
         Seller              = 12,
         SoldLast            = 13,
         Rule127             = 14,
         BunchedSold         = 15,
         NonBoardLot         = 16,
         POSIT               = 17,
         AutoExecution       = 18,
         Halt                = 19,
         Delayed             = 20,
         Reopen              = 21,
         Acquisition         = 22,
         CashMarket          = 23,
         NextDayMarket       = 24,
         BurstBasket         = 25,
         OpenDetail          = 26,
         IntraDetail         = 27,
         BasketOnClose       = 28,
         Rule155             = 29,
         Distribution        = 30,
         Split               = 31,
         Reserved            = 32,
         CustomBasketCross   = 33,
         AdjTerms            = 34,
         Spread              = 35,
         Straddle            = 36,
         BuyWrite            = 37,
         Combo               = 38,
         STPD                = 39,
         CANC                = 40,
         CANCLAST            = 41,
         CANCOPEN            = 42,
         CANCONLY            = 43,
         CANCSTPD            = 44,
         MatchCross          = 45,
         FastMarket          = 46,
         Nominal             = 47,
         Cabinet             = 48,
         BlankPrice          = 49,
         NotSpecified        = 50,
         MCOfficialClose     = 51,
         SpecialTerms        = 52,
         ContingentOrder     = 53,
         InternalCross       = 54,
         StoppedRegular      = 55,
         StoppedSoldLast     = 56,
         StoppedOutOfSeq     = 57,
         Basis               = 58,
         VWAP                = 59,
         SpecialSession      = 60,
         NanexAdmin          = 61,
         OpenReport          = 62,
         MarketOnClose       = 63,
         NotDefined          = 64,
         OutOfSeqPreMkt      = 65,
         MCOfficialOpen      = 66,
         FuturesSpread       = 67,
         OpenRange           = 68,
         CloseRange          = 69,
         NominalCabinet      = 70,
         ChangingTrans       = 71,
         ChangingTransCab    = 72,
         NominalUpdate       = 73,
         PitSettlement       = 74,
         BlockTrade          = 75,
         ExgForPhysical      = 76,
         VolumeAdjustment    = 77,
         VolatilityTrade     = 78,
         YellowFlag          = 79,
         FloorPrice          = 80,
         OfficialPrice       = 81,
         UnofficialPrice     = 82,
         MidBidAskPrice      = 83,
         EndSessionHigh      = 84,
         EndSessionLow       = 85,
         Backwardation       = 86,
         Contango            = 87,
         Holiday             = 88,
         PreOpening          = 89,
         PostFull            = 90,
         PostRestricted      = 91,
         ClosingAuction      = 92,
         Batch               = 93,
         Trading             = 94,
         IntermarketSweep    = 95,
         Derivative          = 96,
         Reopening           = 97,
         Closing             = 98,
         CAPElection         = 99,
         SpotSettlement      = 100,
         BasisHigh           = 101,
         BasisLow            = 102,
         Yield               = 103,
         PriceVariation      = 104,
         StockOption         = 105,
         NewTc106            = 106,
         NewTc107            = 107
    }

        public enum nxST_QUOTECONDITION
    {
         Regular            = 0,
         BidAskAutoExec     = 1,
         Rotation           = 2,
         SpecialistAsk      = 3,
         SpecialistBid      = 4,
         Locked             = 5,
         FastMarket         = 6,
         SpecialistBidAsk   = 7,
         OneSide            = 8,
         OpeningQuote       = 9,
         ClosingQuote       = 10,
         MarketMakerClosed  = 11,
         DepthOnAsk         = 12,
         DepthOnBid         = 13,
         DepthOnBidAsk      = 14,
         Tier3              = 15,
         Crossed            = 16,
         Halted             = 17,
         OperationalHalt    = 18,
         News               = 19,
         NewsPending        = 20,
         NonFirm            = 21,
         DueToRelated       = 22,
         Resume             = 23,
         NoMarketMakers     = 24,
         OrderImbalance     = 25,
         OrderInflux        = 26,
         Indicated          = 27,
         PreOpen            = 28,
         InViewOfCommon     = 29,
         RelatedNewsPending = 30,
         RelatedNewsOut     = 31,
         AdditionalInfo     = 32,
         RelatedAddlInfo    = 33,
         NoOpenResume       = 34,
         Deleted            = 35,
         RegulatoryHalt     = 36,
         SECSuspension      = 37,
         NonComliance       = 38,
         FilingsNotCurrent  = 39,
         CATSHalted         = 40,
         CATS               = 41,
         ExDivOrSplit       = 42,
         Unassigned         = 43,
         InsideOpen         = 44,
         InsideClosed       = 45,
         OfferWanted        = 46,
         BidWanted          = 47,
         Cash               = 48,
         Inactive           = 49,
         NationalBBO        = 50,
         Nominal            = 51,
         Cabinet            = 52,
         NominalCabinet     = 53,
         BlankPrice         = 54,
         SlowBidAsk         = 55,
         SlowList           = 56,
         SlowBid            = 57,
         SlowAsk            = 58
    }

        public enum nxST_CATEGORY
    {
         NanexCategories_1          = 0,
         OptionContract             = 1,
         OptionSeries               = 2,
         SymbolClassification       = 3,
         EquitySymbolInfo           = 4,
         FuturesSymbolInfo          = 5,
         BondSymbolInfo             = 6,
         IndexSymbolInfo            = 7,
         FundSymbolInfo             = 8,
         EquityFundamental          = 9,
         DTNEquityFundamental       = 10,
         MutualFundFundamental      = 11,
         MoneyMarketFundamental     = 12,
         FiftyTwoWeekHiLo           = 13,
         YTDHiLo                    = 14,
         ContractHiLo               = 15,
         OHLC                       = 16,
         NxRefreshOverride          = 17,
         NxUncorrectedOHLCV         = 18,
         TradeReportSummary         = 20,
         FormTReportSummary         = 21,
         OpeningTradeReports        = 22,
         MCClosingTradeReports      = 23,
         TradeCorrection            = 24,
         BarCorrection              = 25,
         NxFirstTrade               = 26,
         NxLastTrade                = 27,
         NxFirstQuote               = 28,
         NxLastQuote                = 29,
         SymbolHistory              = 30,
         SymbolAlias                = 31,
         ActivityRank               = 33,
         FinancialStatus            = 34,
         NxNMsg                     = 39,
         OptionMultipleStrikeCodes  = 40,
         OptionMultipleStrikePrices = 41,
         TradingSessionHours        = 42,
         CTA_ClosingReport          = 50,
         OTCBB_ClosingReport        = 51,
         OTC_ClosingReport          = 52,
         Nasdaq_ClosingReport       = 54,
         Nasdaq_InsideClosingReport = 55,
         CCDF_ClosingReport         = 56,
         Split                      = 60,
         ExDividend                 = 61,
         DividendYield              = 62,
         Halt                       = 63,
         PriceIndication            = 64,
         OrderImbalance             = 65,
         ShortSaleRestricted        = 66,
         OpenInterest               = 67,
         VolOI                      = 68,
         MMakerStatus               = 69,
         RootSymbolInfo             = 70,
         FundPricing                = 71,
         FuturesContract            = 80,
         OpenRange                  = 81,
         CloseRange                 = 82,
         OtherRange                 = 83,
         Settlement                 = 84,
         FutureOptContract          = 86
    }

        public enum nxST_HALTSTATUS
    {
         Resume         = 0,
         OpenDelay      = 1,
         Halted         = 2,
         NoOpenNoResume = 3
    }

        public enum nxST_HALTREASONTYPE
    {
         News                  = 1,
         NewsDisseminated      = 2,
         OrderImbalance        = 3,
         EquipmentChange       = 4,
         PendingAdditionalInfo = 5,
         Suspended             = 6,
         SEC                   = 7,
         NotSpecified          = 8
    }

        public enum nxST_OPENINDICATIONTYPE
    {
         Clear      = 0,
         Normal     = 1,
         New        = 2,
         Correcting = 3,
         Cancel     = 4,
         Range      = 5
    }

        public enum nxST_ORDERIMBALANCETYPE
    {
         Clear           = 0,
         BuySide         = 1,
         SellSide        = 2,
         ClearClosing    = 3,
         BuySideClosing  = 4,
         SellSideClosing = 5,
         FastMarket      = 6
    }

    public enum nxST_TRADEREPORTREASON
    {
         InitialRefresh = 0,
         PostInsert     = 1,
         PostCancel     = 2,
         PostCorrection = 3,
         SyncRefresh    = 4,
         Final          = 5,
         CorrectedFinal = 6
    }

    public enum nxST_EXGCORRECTIONMAP
    {
        Correction = 0x00000001,
        Cancel = 0x00000002,
        Busted = 0x00000004,
        EntryError = 0x00000008,
        CancelBid = 0x00000010,
        CancelAsk = 0x00000020
    }
    public enum nxST_SYMBOLCHANGETYPE
    {
         Add    = 0,
         Delete = 1,
         Modify = 2
    }
    public enum nxST_OHLCFLAGS 
    {
         CloseIsSettle  = 0x00000001,
         CloseInClsRng  = 0x00000002,
         CloseSetByBid  = 0x00000004,
         CloseSetByAsk  = 0x00000008,
         HighBeforeLow  = 0x00000010,
         HighInserted   = 0x00000020,
         LowInserted    = 0x00000040,
         OpenInserted   = 0x00000080,
         HighSetByBid   = 0x00000100,
         LowSetByAsk    = 0x00000200,
         HighEqLow      = 0x00000400,
         LowAboveHigh   = 0x00000800,
         VolEstimated   = 0x00001000,
         OpenInOpnRng   = 0x00002000,
         OpenSetByBid   = 0x00004000,
         OpenSetByAsk   = 0x00008000,
         OpenEqLast     = 0x00010000,
         OpenEqHigh     = 0x00020000,
         OpenEqLow      = 0x00040000,
         OpenOutsideHL  = 0x00080000,
         Corrections    = 0x00100000,
         CloseEqHigh    = 0x00200000,
         CloseEqLow     = 0x00400000,
         CloseOutsideHL = 0x00800000,
         PrClIsSettle   = 0x01000000,
         PrClInClsRng   = 0x02000000,
         PrClSetByBid   = 0x04000000,
         PrClSetByAsk   = 0x08000000,
         VolumeX100     = 0x10000000
    }

    public enum nxST_EXDIVIDENDATTRIB
    {
         Canadian              = 0x00000001,
         Special               = 0x00000002,
         Extra                 = 0x00000004,
         ExtraOrdinary         = 0x00000008,
         Increase              = 0x00000010,
         Initial               = 0x00000020,
         SaleOfRights          = 0x00000040,
         Approximate           = 0x00000080,
         OrdinaryIncome        = 0x00000100,
         ExRights              = 0x00000200,
         PercentDistribution   = 0x00000400,
         ToBeAnnounced         = 0x00000800,
         Final                 = 0x00001000,
         Decrease              = 0x00002000,
         PaymentInStock        = 0x00004000,
         SemiAnnual            = 0x00008000,
         Annual                = 0x00010000,
         PaymentInKind         = 0x00020000,
         ShortTermCapitalGains = 0x00040000,
         CapitalGains          = 0x00080000,
         Equivalence           = 0x00100000,
         GrossAmount           = 0x00200000,
         LiquidityDistribution = 0x00400000,
         PaymentInCashOrStock  = 0x00800000,
         CorrectedAmount       = 0x01000000,
         Monthly               = 0x02000000,
         Multiple              = 0x04000000,
         Estimating            = 0x08000000,
         ReturnOfCapital       = 0x10000000
    }

    public enum nxST_OPRAEXCHANGELIST
    {
         NQEX  = 0x00000001,
         CBOE  = 0x00000002,
         ISEX  = 0x00000004,
         AMEX  = 0x00000008,
         PACF  = 0x00000010,
         PHIL  = 0x00000020,
         BOST  = 0x00000040,
         NQEX2 = 0x00000080,
         OEx8  = 0x00000100,
         OEx9  = 0x00000200,
         OExA  = 0x00000400,
         OExB  = 0x00000800,
         OExC  = 0x00001000,
         OExD  = 0x00002000,
         OExE  = 0x00004000,
         OExF  = 0x00008000
    }
    public enum nxST_MARKETMAKERTYPE 
    {
         Regular = 1,
         Primary = 2,
         Passive = 3
    }
    public enum nxST_MMQUOTETYPE
    {
        Regular = 1,
        Passive = 2,
        SyndicateBid = 3,
        PreSyndicateBid = 4,
        PenaltyBid = 5,
        DeletePosition = 6
    }

    public enum nxST_REFRESHTYPE
    {
         NotRefresh    = 0,
         ExgSourced    = 1,
         Mismatch      = 2,
         Synchronizing = 3
    }
    public enum nxST_EXPIRATIONCYCLE
    {
         Jan = 1,
         Feb = 2,
         Mar = 3,
         Wk1 = 4,
         Wk2 = 5,
         Wk3 = 6,
         Wk4 = 7,
         Wk5 = 8,
         Qtr = 9
    }

    public enum nxST_VOLUMETYPE
        {
         Incremental     = 0,
         NonIncremental  = 1,
         TotalVolume     = 2,
         TotalVolumeX100 = 3
    }

    public enum nxST_QTEMATCHTYPE
    {
        NoMatch = 0,
        AtBid = 1,
        AtAsk = 2,
        Inside = 3,
        BelowBid = 4,
        AboveAsk = 5
    }

    public enum nxST_QTEMATCHFLAGS
    {
         OlderMatch = 0x00000001,
         Crossed    = 0x00000002
    }

    public enum nxST_SIGHIGHLOWTYPE 
    {
         Even    = 0,
         NewLow  = 1,
         Low     = 2,
         High    = 3,
         NewHigh = 4
    }

    public enum nxST_BBOCHANGEFLAGS
    {
         BidExg   = 0x00000001,
         BidPrice = 0x00000002,
         BidSize  = 0x00000004,
         AskExg   = 0x00000010,
         AskPrice = 0x00000020,
         AskSize  = 0x00000040
    }

    public enum nxST_TRADECONDFLAGS
    {
        NoLast = 0x00000001,
        NoHigh = 0x00000002,
        NoLow = 0x00000004
    }
    public enum nxST_PRICEFLAGS
    {
         SetLast      = 0x00000001,
         SetHigh      = 0x00000002,
         SetLow       = 0x00000004,
         SetOpen      = 0x00000008,
         InsertRecord = 0x00000010,
         CancelRecord = 0x00000020,
         Settlement   = 0x00000040
    }
    public enum nxST_SETTLEFLAGMAP
    {
         OneSession  = 0x00000001,
         AllSessions = 0x00000002,
         Corrected   = 0x00000004,
         SettleOnly  = 0x00000008
    }

    public enum nxST_SYMBOLFNCLSTATUSMAP
    {
         RestoredGood = 0x00000001,
         Bankrupt     = 0x00000002,
         Deficient    = 0x00000004,
         Delinquent   = 0x00000008,
         Suspended    = 0x00000010
    }

    public enum nxST_EQUITYCLASSIFICATIONS
    {
        ETF = 0x00000001,
        NQGlobalSelect = 0x00000002,
        HasOptions = 0x00000010,
        HasSSFutures = 0x00000020,
        Dividend = 0x00000100,
        Split = 0x00000200,
        SymHist = 0x00000400,
        NewSymbol = 0x00000800,
        ClosedEndFund = 0x00001000,
        ADR = 0x00002000
    }

    public enum nxST_SYMBOLTYPES
    {
        Bond = 1,
        Currency = 2,
        Equity = 4,
        Futures = 5,
        Index = 8,
        MutualFund = 12,
        Option = 14,
        FuturesOption = 15,
        FuturesRoot = 17,
        SingleStockFutures = 18,
        Spread = 25
    }


}

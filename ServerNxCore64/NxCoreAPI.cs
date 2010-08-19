///////////////////////////////////////////////////////////////
//// CONTENTS COPYRIGHT 2010 by  Nanex, LLC, Winnetka, IL. ////
//// ALL RIGHTS RESERVED.					               ////  
//// YOU MAY NOT TRANSMIT THE CONTENTS OF THIS FILE        ////
//// WITHOUT EXPRESS WRITTEN PERMISSION FROM NANEX,LLC.    ////
//// !!! THIS FILE CONTAINS CONFIDENTIAL INFORMATION !!!   ////	
//// SUPPORT: email: nxcore@nanex.net		               ////
///////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////
//// NxCoreAPI.cs                                          ////
//// C# Wrapper for NxCore API                             ////
//// Author: Jeffrey Donovan                               ////
///////////////////////////////////////////////////////////////
//// To Read as Written:                                   ////
//// Tab Size: 4  Indent Size: 2, Keep Tabs                ////
///////////////////////////////////////////////////////////////
//// Last Modification Date: 01-08-2010                    ////
///////////////////////////////////////////////////////////////

using   System;
using   System.Runtime.InteropServices;
using   System.Text;

namespace NxCoreAPI
{
  #region NxCore Structures

    #region NxDate, NxTime, NxString
        
    // NxDate    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 16)]
    public struct NxDate
    {   // 16 bytes. aligns on 4 bytes.
      [FieldOffset(0)]
      public UInt32 NDays;        // julian date. Number of days since Jan 1, 1883.
      [FieldOffset(4)]
      public ushort Year;         // current year. 1999,2000,2001,2002,2003..
      [FieldOffset(6)]
      public byte Month;          // month of year.  1-12 (note this is not zero based)
      [FieldOffset(7)]
      public byte Day;            // day of month. 1-31 (not zero based)
      [FieldOffset(8)]
      public byte DSTIndicator;   // see NxDATE_DST_
      [FieldOffset(9)]
      public byte DayOfWeek;      // see NxDOW_
      [FieldOffset(10)]
      public ushort DayOfYear;
      [FieldOffset(12)]
      public UInt32 alignment;
    };
    
    // NxTime    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 16)]
    public struct NxTime
    {   // 16 bytes. aligns on 4 bytes.
      [FieldOffset(0)]
      public UInt32 MsOfDay;      // number of milliseconds since start of day -- midnight
      [FieldOffset(4)]
      public ushort Millisecond;  // millisecond of second 0-999
      [FieldOffset(6)]
      public byte Second;         // second of minute 0-59
      [FieldOffset(7)]
      public byte Minute;         // minute of hour 0-59
      [FieldOffset(8)]
      public byte Hour;           // hour of day. 0-23
      [FieldOffset(9)]
      public sbyte TimeZone;      // +/- hours to GMT
      [FieldOffset(10)]
      public ushort MsResolution;
      [FieldOffset(12)]
      private UInt32 alignment;
    };

	// NxString
    // NxString contains two 32-bit integer members that you can assign to each NxString, a unique "Atom" assigned by NxCoreAPI 
    // and a zero terminated ascii string.    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    public struct NxString
    {
      [FieldOffset(0)]
      public Int32 UserData1;// first  32-bit user data space. Valid for
      [FieldOffset(4)]
      public Int32 UserData2;// second 32-bit user data space.
      [FieldOffset(8)]
      public ushort Atom;    // unique/sequential/nonzero/constant 16 bit number assigned to the String.
      [FieldOffset(10)]
      public sbyte String;   // this is acually [strlen(String)+1] and directly follows Atom in memory.
      //(note String in pnxsDateAndStrike in NxOptionHdr is NOT null terminated -- it is exactly 2 bytes)
    };

	// NxStringDaS
    // NxStringDaS contains two 32-bit integer members that you can assign to each NxString, a unique "Atom" assigned by NxCoreAPI 
    // and a two-character ascii string.    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    public struct NxStringDaS
    {
      [FieldOffset(0)]
      public Int32 UserData1;// first  32-bit user data space. Valid for
      [FieldOffset(4)]
      public Int32 UserData2;// second 32-bit user data space.
      [FieldOffset(8)]
      public ushort Atom;    // unique/sequential/nonzero/constant 16 bit number assigned to the String.
      [FieldOffset(10)]
      public sbyte String;   // this is acually [strlen(String)] and directly follows Atom in memory -- it is exactly 2 bytes
    };

    #endregion  // NxDate, NxTime, NxString

    #region AccessStatus

    
    // NxAccessStatus    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 44)]
    public struct NxAccessStatus
    {
      [FieldOffset(0)]
      public UInt32 Version;
      [FieldOffset(4)]
      public Int32 AcctExpireDays;
      [FieldOffset(8)]
      public byte Status;
      [FieldOffset(9)]
      public byte AttnLevel;
      [FieldOffset(10)]
      public byte StatusMCS;          // when a multicast client -- shows status of NxCore Access Multicast Server
      [FieldOffset(11)]
      public byte AttnLevelMCS;
      [FieldOffset(12)]
      public ushort UDPPingTime;      // round-trip in milliseconds
      [FieldOffset(14)]
      public ushort UDPPktLossPct;    // # packet resends per 1,000
      [FieldOffset(16)]
      public Int32 NxClockDiff;
      [FieldOffset(20)]
	  public UInt32 NxClockReceived;
      [FieldOffset(24)]
      public Int32 SysClockDrift;
      [FieldOffset(28)]
	  public UInt32 SecondsRunning;   // count of seconds NxCore Access has been running
      [FieldOffset(32)]
	  public UInt32 RecvBytes;        // recent bandwidth in bytes/second (divide by 10000 to get kbps
      [FieldOffset(36)]
	  public UInt32 SendBytes;        // recent bandwidth in bytes/second (divide by 10000 to get kbps
      [FieldOffset(40)]
      public ushort MCClients;
      [FieldOffset(42)]
      public ushort MCCLicenses;
      [FieldOffset(44)]
      public byte ChatMsgIndc;
    };
    #endregion  // AccessStatus

    #region NxCoreSystem

	// NxCoreSystem
    // NxCoreSystem is first parameter passed to user callback function: int UserCallBackFunction(const NxCoreSystem*,const NxCoreMessage*);    
	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 76)]
    public struct NxCoreSystem
    {
      [FieldOffset(0)]
      public Int32 UserData;     // UserData value passed to NxCoreProcessTape
      [FieldOffset(4)]
	  public Int32 DLLVersion;   // Version of NxCoreAPI.dll
      [FieldOffset(8)]
      public NxDate nxDate;      // date of tape.
      [FieldOffset(24)]
      public NxTime nxTime;      // time of tape. This is the subsecond NxClock
      [FieldOffset(40)]
	  public Int32 ClockUpdateInterval; // indicates changes to nxTime
      [FieldOffset(44)]
	  public Int32 Status;              // processing status. see NxCORESTATUS_
      [FieldOffset(48)]
	  public Int32 StatusData;          // additional information depending on Status
      [FieldOffset(52)]
      unsafe public sbyte* StatusDisplay; // text display combining Status and StatusData
      [FieldOffset(56)]
      unsafe public sbyte* TapeDisplay;   // text from the header of the tape.
      [FieldOffset(60)]
      unsafe public void* Module;         // module of the dll for the calling process.
      [FieldOffset(64)]
      unsafe public void* ThreadInstance; // thread instance identifier
      [FieldOffset(68)]
      unsafe public sbyte* PermissionSet; // text of PermissionSet "AA", etc.
      [FieldOffset(72)]
      unsafe public NxAccessStatus* pNxAStatus;     // Ver 2.0 and higher (NxCoreAccess and NxCoreAPI.dll): during real-time processing, status of NxCoreAccess. Updated once per second.
    };
    #endregion  // NxCoreSystem

    #region NxCoreData
    	    
    // NxCoreQuote    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 40)]
    public struct NxCoreQuote
    {   //40 bytes. Aligns on 4 bytes.
      [FieldOffset(0)]
      public Int32 AskSize;        // Ask size
      [FieldOffset(4)]
	  public Int32 BidSize;        // Bid size
      [FieldOffset(8)]
	  public Int32 AskSizeChange;
      [FieldOffset(12)]
	  public Int32 BidSizeChange;
      [FieldOffset(16)]
	  public Int32 AskPrice;       // ask Price (nxPriceType)
      [FieldOffset(20)]
	  public Int32 BidPrice;       // bid Price (nxPriceType)
      [FieldOffset(24)]
	  public Int32 AskPriceChange;
      [FieldOffset(28)]
	  public Int32 BidPriceChange;
      [FieldOffset(32)]
      public byte PriceType;      // lAskPrice and lBidPrice always have the same price type.
      [FieldOffset(33)]
      public byte Refresh;        // set to non-zero if this message is a refresh as opposed to real-time.
      [FieldOffset(34)]
      public byte QuoteCondition; // quote condition assigned by exchange. Use NxCoreGetDefinedString(NxST_QUOTECONDITION,QuoteCondition) for text name
      [FieldOffset(35)]
      public byte NasdaqBidTick;  // for nasdaq stocks. nasdaqBidTick.
      [FieldOffset(36)]
      private UInt32 alignment;   // for alignment
    };
       
    // NxCoreExgQuote    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 84)] // must force the size, or it becomes 88
    public struct NxCoreExgQuote
    {   // 88 bytes.
      [FieldOffset(0)]
      public NxCoreQuote coreQuote;
      [FieldOffset(40)]
      public Int32 BestAskPrice;
      [FieldOffset(44)]
	  public Int32 BestBidPrice;
      [FieldOffset(48)]
	  public Int32 BestAskPriceChange;
      [FieldOffset(52)]
	  public Int32 BestBidPriceChange;
      [FieldOffset(56)]
	  public Int32 BestAskSize;
      [FieldOffset(60)]
	  public Int32 BestBidSize;
      [FieldOffset(64)]
	  public Int32 BestAskSizeChange;
      [FieldOffset(68)]
	  public Int32 BestBidSizeChange;
      [FieldOffset(72)]
      public ushort BestAskExg;
      [FieldOffset(74)]
      public ushort BestBidExg;
      [FieldOffset(76)]
      public ushort PrevBestAskExg;
      [FieldOffset(78)]
      public ushort PrevBestBidExg;
      [FieldOffset(80)]
      public byte BestAskCondition;
      [FieldOffset(81)]
      public byte BestBidCondition;
      [FieldOffset(82)]
      public byte BBOChangeFlags;
      [FieldOffset(83)]
      public byte ClosingQuoteFlag;
    };

    // NxCoreMMQuote   
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 48)]
    public struct NxCoreMMQuote
    {   // 48 bytes (52 bytes on 64bits)
      [FieldOffset(0)]
      public NxCoreQuote coreQuote;
      [FieldOffset(40)]
      unsafe public NxString* pnxStringMarketMaker;
      [FieldOffset(44)]
      public byte MarketMakerType;
      [FieldOffset(45)]
      public byte QuoteType;
    };
      
    // NxCoreTrade.nxAnalysis  
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 24)]
    public struct NxCTAnalysis
    {   // 24 bytes. aligns on 4 bytes.
      [FieldOffset(0)]
      public Int32 FilterThreshold;        // threshold for filter analysis
      [FieldOffset(4)]
      public byte Filtered;
      [FieldOffset(5)]
      public byte FilterLevel;
      [FieldOffset(6)]
      public byte SigHiLoType;
      [FieldOffset(7)]
      private byte alignment;
      [FieldOffset(8)]
      public UInt32 SigHiLoSeconds;
      [FieldOffset(12)]
	  public Int32 QteMatchDistanceRGN;
      [FieldOffset(16)]
	  public Int32 QteMatchDistanceBBO;
      [FieldOffset(20)]
      public byte QteMatchTypeRGN;
      [FieldOffset(21)]
      public byte QteMatchTypeBBO;
      [FieldOffset(22)]
      public byte QteMatchFlagsRGN;
      [FieldOffset(23)]
      public byte QteMatchFlagsBBO;
    };
      
    // NxCoreTrade
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 84)]   // must force the size, or it becomes 88
    public struct NxCoreTrade
    {   // 84 bytes.
      [FieldOffset(0)]
      public Int32 Price;           // price of trade report. Note: There is also a "Last" field in this structure. Not all Prices update the "Last". FormT (after hours) trades are one example.
      [FieldOffset(4)]
      public byte PriceType;        // Applies to Price,Open,High,Low,Last,Tick,Net Change,nxAnalysis.FilterThreshold,nxAnalysis.QteMatchDistanceRGN/BBO fields in this record.
      [FieldOffset(5)]
      public byte PriceFlags;       // flags indicating if trade is new high/low, etc. see defines beginning with NxTPF_ .
      [FieldOffset(6)]
      public byte TradeCondition;   // trade condition code assigned by exchange. Use NxCoreGetDefinedString(NxST_TRADECONDITION,TradeCondition) for text name
      [FieldOffset(7)]
      public byte ConditionFlags;   // converts TradeCondition (and BATE when present) to set eligibility flags (defines begin with NxTCF_ )
      [FieldOffset(8)]
      public byte VolumeType;       // indicates how the Size field changes (or not) the TotalVolume/TickVolume fields (defines begin with NxTVT_ )
      [FieldOffset(9)]
      public sbyte BATECode;        // 'B'id,'A'sk, or 'T'rade indicator for some commodities. ('E'xception not used -- historical artifact).
      [FieldOffset(10)]
      private ushort alignment;     // alignment
      [FieldOffset(12)]
      public UInt32 Size;
      [FieldOffset(16)]
	  public UInt32 ExgSequence;    // original exchange sequence number.
      [FieldOffset(20)]
	  public UInt32 RecordsBack;    // for inserts and deletes, indicates the number of records from the last record received the cancel/insert applies.
      [FieldOffset(24)]
      public NxCTAnalysis nxAnalysis;
      [FieldOffset(48)]
      public System.UInt64 TotalVolume;
      [FieldOffset(56)]
	  public UInt32 TickVolume;
      [FieldOffset(60)]
      public Int32 Open;
      [FieldOffset(64)]
	  public Int32 High;
      [FieldOffset(68)]
	  public Int32 Low;
      [FieldOffset(72)]
	  public Int32 Last;
      [FieldOffset(76)]
	  public Int32 Tick;
      [FieldOffset(80)]
	  public Int32 NetChange;
    };
      
    // NxPrice    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 8)]
    public struct NxPrice
    {   // 8 bytes. Aligns on 4 bytes.
      [FieldOffset(0)]
      public Int32 Price;
      [FieldOffset(4)]
      public byte PriceType;
    };
      
    // StringTableItem    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 8)]
    public struct StringTableItem
    {
      [FieldOffset(0)]
      public UInt32 ixTable;
      [FieldOffset(4)]
      public UInt32 idString;
    };
    
    // DataUnion is used inside of NxCategoryField  
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 16)]
    public struct DataUnion
    {
      [FieldOffset(0)]
      public System.Int64 i64Bit;     // 1: NxCFT_64BIT
      [FieldOffset(0)]
      public Int32 i32Bit;            // 2: NxCFT_32BIT
      [FieldOffset(0)]
      unsafe public sbyte* StringZ;   // 3: NxCFT_STRINGZ
      [FieldOffset(0)]
      public double Double;           // 4: NxCFT_DOUBLE
      [FieldOffset(0)]
      public NxPrice nxPrice;         // 5: NxCFT_PRICE
      [FieldOffset(0)]
      public NxDate nxDate;           // 6: NxCFT_DATE
      [FieldOffset(0)]
      public NxTime nxTime;           // 7: NxCFT_TIME
      [FieldOffset(0)]
      unsafe public NxString* pnxString;       // 8: NxCFT_NxSTRING
      [FieldOffset(0)]
      public StringTableItem stringTableItem;  // 9: NxCFT_STRING_IDX, 10:NxCFT_STRING_MAP
    };
      
    // NxCategoryField  
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 32)]
    public struct NxCategoryField
    {
      [FieldOffset(0)]
      unsafe public sbyte* FieldName;  //4 or 8 bytes (8 on 64bit)
      [FieldOffset(4)]
      unsafe public sbyte* FieldInfo;  //4 or 8 bytes (8 on 64bit)
      [FieldOffset(8)]
      public byte Set;
      [FieldOffset(9)]
      public byte FieldType;
      [FieldOffset(10)]
      public byte a1;     // alignment
      [FieldOffset(11)]
      public byte a2;     // alignment
      [FieldOffset(12)]
      public byte a3;     // alignment
      [FieldOffset(13)]
      public byte a4;     // alignment
      [FieldOffset(14)]
      public byte a5;     // alignment
      [FieldOffset(15)]
      public byte a6;     // alignment
      [FieldOffset(16)]
      public DataUnion data;
    };
    
    // NxCoreCategory
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 10)]
    public struct NxCoreCategory
    {
      [FieldOffset(0)]
      unsafe public NxString* pnxStringCategory;
      [FieldOffset(4)]
      unsafe public NxCategoryField* pnxFields;
      [FieldOffset(8)]
      public ushort NFields;
    };
    
    // NxOptionHdr
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 48)]
    public struct NxOptionHdr
    {   // 48 bytes
      [FieldOffset(0)]
      unsafe public NxStringDaS* pnxsDateAndStrike;  // the string member contains 2 alpha characters (expire code and strike code) -- and is NOT null terminated.
      [FieldOffset(4)]
      unsafe public NxString* pnxsUnderlying;        // This is the same pointer as pnxStringSymbol in the underlying symbol (unless Atom == 0, which is the case if you are not subscribed to the underlying's exchange).
      [FieldOffset(8)]
      unsafe public NxString* pnxsSeriesChain;       // All option series in the chain share the same pointer to this string.
      [FieldOffset(12)]
      unsafe public NxString* pnxsSpecialSettle;     // All option series with the same special settlement share the same pointer to this string
      [FieldOffset(16)]
      public ushort exgUnderlying;    // exchange code of the underlying symbol
      [FieldOffset(18)]
      public ushort contractUnit;     // default is 100 (shares,units, etc).
      [FieldOffset(20)]
      public Int32 strikePrice;       // OPRA sourced strike price. implied 3 decimal places. i.e.    35000 == 35.000 == 35
      [FieldOffset(24)]
      public Int32 strikePriceEst;    // if non-zero, the nxCore server includes the estimated strike price for this option contract.
      [FieldOffset(28)]
      public byte PutCall;            // the value 1 == put, 0 == call.
      [FieldOffset(29)]
      public byte expirationCycle;    // 1 == January cycle, 2 == February cycle, 3 == March cycle.
      [FieldOffset(30)]
      public byte oicStrikeAge;       // number from 0-7. Incremented each trading day the strikePrice is received from OPRA.
      [FieldOffset(31)]
      public byte nxStrikeMatch;      // number from 0-15 indicating the likelihood that the strikePrice is correct.
      [FieldOffset(32)]
      public NxDate nxExpirationDate; // The date this option contract expires.
    };

    // NxCoreSymbolChange    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 14)]
    public struct NxCoreSymbolChange
    {
      [FieldOffset(0)]
      public byte Status;   // see NxSS_
      [FieldOffset(1)]
      public sbyte StatusChar;   // 'A'dd,'D'elete,'M'odify. for humans
      [FieldOffset(2)]
      private ushort alignment;
      [FieldOffset(4)]
      unsafe public NxString* pnxsSymbolOld;
      [FieldOffset(8)]
      unsafe public NxOptionHdr* pnxOptionHdrOld;  // non-zero for option contract changes
      [FieldOffset(12)]
      public ushort ListedExgOld;
    };

	// SymbolSpin
    // Every SymbolSpin  contains the current iteration zero-based count (index) and the total number of symbols that will be iterated.
    // only options contracts use the "sub" count and limit fields.    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 20)]
    public struct NxCoreSymbolSpin
    {
      [FieldOffset(0)]
      public UInt32 SpinID;         // NxCoreAPI.dll sets this to 0 for the initial automatic SymbolSpin at the start of each tape. The value passed to the SymbolIterate function for the spinID will appear in this data member.
      [FieldOffset(4)]
	  public UInt32 IterLimit;      // for(IterCount = 0; IterCount < IterLimit; ++IterCount)
      [FieldOffset(8)]
	  public UInt32 SubIterLimit;   // for(SubIterCount = 0; SubIterCount < SubIterLimit; ++SubIterCount) => for option contracts within one option series.
      [FieldOffset(12)]
	  public UInt32 IterCount;
      [FieldOffset(16)]
	  public UInt32 SubIterCount;
    };
    
    // NxCoreData union    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 88)]
    public struct NxCoreData
    {
      [FieldOffset(0)]
      public NxCoreExgQuote ExgQuote;
      [FieldOffset(0)]
      public NxCoreMMQuote MMQuote;
      [FieldOffset(0)]
      public NxCoreTrade Trade;
      [FieldOffset(0)]
      public NxCoreCategory Category;
      [FieldOffset(0)]
      public NxCoreSymbolChange SymbolChange;
      [FieldOffset(0)]
      public NxCoreSymbolSpin SymbolSpin;
    };
    #endregion  // NxCoreData

    #region NxCoreHeader
    
    // NxCoreHeader    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 48)]
    public struct NxCoreHeader
    {   // 48 bytes
      [FieldOffset(0)]
      unsafe public NxString* pnxStringSymbol;
      [FieldOffset(4)]
      unsafe public NxOptionHdr* pnxOptionHdr;
      [FieldOffset(8)]
      public NxDate nxSessionDate;
      [FieldOffset(24)]
      public NxTime nxExgTimestamp;
      [FieldOffset(40)]
      public ushort ListedExg;        // Listed or Primary Exchange. Use NxCoreGetDefinedString(NxST_EXCHANGE,ListedExg) for text name
      [FieldOffset(42)]
      public ushort ReportingExg;     // Exchange sending quote/trade. Use NxCoreGetDefinedString(NxST_EXCHANGE,ReportingExg) for text name
      [FieldOffset(44)]
      public byte SessionID;
      [FieldOffset(45)]
      private byte alignment;
      [FieldOffset(46)]
      public ushort PermissionID;     // identifies the permissions (exchanges,etc.) in the tape. Use NxCoreGetDefinedString(NxST_PERMID,PermissionID) for text name
    };
    #endregion  // NxCoreHeader

    #region NxCoreMessage
    
    // NxCoreMessage    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 140)]
    public struct NxCoreMessage
    {
      [FieldOffset(0)]
      public NxCoreHeader coreHeader;
      [FieldOffset(48)]
      public NxCoreData coreData;
      [FieldOffset(136)]
      public UInt32 MessageType;
    };
    #endregion  // NxCoreMessage

    #region NxCoreState    

    // NxCoreStateExgQuote    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 20)]
    public struct NxCoreStateExgQuote
    {
      [FieldOffset(0)]
      public Int32 AskPrice;
      [FieldOffset(4)]
	  public Int32 BidPrice;
      [FieldOffset(8)]
	  public Int32 AskSize;
      [FieldOffset(12)]
	  public Int32 BidSize;
      [FieldOffset(16)]
      public ushort ReportingExg;
      [FieldOffset(18)]
      public byte QuoteCondition; // quote condition
    };
    
    // NxCoreStateMMQuote    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 24)]
    public struct NxCoreStateMMQuote
    {
      [FieldOffset(0)]
	  public Int32 AskPrice;
      [FieldOffset(4)]
	  public Int32 BidPrice;
      [FieldOffset(8)]
	  public Int32 AskSize;
      [FieldOffset(12)]
	  public Int32 BidSize;
      [FieldOffset(16)]
      unsafe public NxString* pnxStringMarketMaker;
      [FieldOffset(20)]
      public byte MarketMakerType;
      [FieldOffset(21)]
      public byte QuoteType;
    };
          
    // NxCoreStateExgQuotes    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 248)]
    public struct NxCoreStateExgQuotes
    {
      [FieldOffset(0)]
      public ushort StateQuoteCount;
      [FieldOffset(2)]
      public byte PriceType;
      [FieldOffset(3)]
      public byte NasdaqBidTick;  // for nasdaq stocks. nasdaqBidTick.
      [FieldOffset(4)]
      public ushort BestAskExg;
      [FieldOffset(6)]
      public ushort BestBidExg;
      [FieldOffset(8)]
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
      public NxCoreStateExgQuote[] StateExgQuotes;
    };
      
    // NxCoreStateMMQuotes    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 12292)]
    public struct NxCoreStateMMQuotes
    {
      [FieldOffset(0)]
      public ushort StateQuoteCount;
      [FieldOffset(2)]
      public byte PriceType;
      [FieldOffset(3)]
      public byte NasdaqBidTick;  // for nasdaq stocks. nasdaqBidTick.
      [FieldOffset(4)]
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
      public NxCoreStateMMQuote[] StateMMQuotes;
    };
    
    // NxCoreStateTrade    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 48)]
    public struct NxCoreStateTrade
    {
      [FieldOffset(0)]
      public System.UInt64 TotalVolume;
      [FieldOffset(8)]
      public UInt32 TickVolume;
      [FieldOffset(12)]
      public byte PriceType;
      [FieldOffset(13)]
      public byte PriceFlags;
      [FieldOffset(14)]
      public byte ConditionFlags;
      [FieldOffset(15)]
      public byte VolumeType;
      [FieldOffset(16)]
      public Int32 Open;
      [FieldOffset(20)]
	  public Int32 High;
      [FieldOffset(24)]
	  public Int32 Low;
      [FieldOffset(28)]
	  public Int32 Last;
      [FieldOffset(32)]
	  public Int32 NetChange;
      [FieldOffset(36)]
	  public Int32 Price;
      [FieldOffset(40)]
	  public Int32 Threshold;
      [FieldOffset(44)]
	  public Int32 Tick;
    };
    
    // NxCoreStateOHLCTrade  
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 52)]
    public struct NxCoreStateOHLCTrade
    {
      [FieldOffset(0)]
      public System.UInt64 TotalVolume;
      [FieldOffset(8)]
      public UInt32 TickVolume;
      [FieldOffset(12)]
      public byte PriceType;
      [FieldOffset(13)]
      public byte PriceFlags;
      [FieldOffset(14)]
      public byte ConditionFlags;
      [FieldOffset(15)]
      public byte VolumeType;
      [FieldOffset(16)]
	  public Int32 Open;
      [FieldOffset(20)]
	  public Int32 High;
      [FieldOffset(24)]
	  public Int32 Low;
      [FieldOffset(28)]
	  public Int32 Last;
      [FieldOffset(32)]
	  public Int32 NetChange;
      [FieldOffset(36)]
	  public Int32 Price;
      [FieldOffset(40)]
	  public Int32 Threshold;
      [FieldOffset(44)]
	  public Int32 Tick;
      [FieldOffset(48)]
	  public Int32 TradeSize;
    };
    #endregion  // NxCoreState

    #region NxCoreTapeFile
      
    // NxCoreTapeFile used with NxCoreListTapes  
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 304)]
    public struct NxCoreTapeFile
    {
      [FieldOffset(0)]
      public sbyte PathnameStrZ;   // Size = 260
      [FieldOffset(260)]
      public Int32 PathnameLen;
      [FieldOffset(264)]
      public Int32 FilenameLen;
      [FieldOffset(268)]
      public UInt32 FileAttributes;
      [FieldOffset(272)]
      public System.UInt64 FileSize;
      [FieldOffset(280)]
      public NxDate TapeDate;
      [FieldOffset(296)]
      public sbyte PermSet;        // Size = 8
    };
    #endregion  // NxCoreTapeFile

    #region NxCoreAPIDLLFile
    
    // NxCoreAPIDLLFile used with NxCoreListAPIDlls    
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 316)]
    public struct NxCoreAPIDLLFile
    {
      [FieldOffset(0)]
      public sbyte PathnameStrZ;   // Size = 260
      [FieldOffset(260)]
      public Int32 PathnameLen;
      [FieldOffset(264)]
      public Int32 FilenameLen;
      [FieldOffset(268)]
      public UInt32 FileAttributes;
      [FieldOffset(272)]
      public System.UInt64 FileSize;
      [FieldOffset(280)]
      public NxDate BuildDate;
      [FieldOffset(296)]
      public NxTime BuildTime;
      [FieldOffset(312)]
      public byte verMajor;
      [FieldOffset(313)]
      public byte verMinor;
      [FieldOffset(314)]
      public ushort verBuild;
    };
    #endregion  // NxCoreAPIDLLFile

#endregion //Structures


#region NxCore Class
	public class NxCore
  {
    /// <summary>
    /// Summary description for NxCoreAPI.
    /// </summary>
    #region Functions
	/// <summary>
	/// Process a tape file.
	/// </summary>
	/// <param name="pszFilename">
	/// Specifies the full path filename name of the NxCore Tape To Process.
	/// Pass zero or an empty string "" will select the active NxCore real-time stream.
	/// </param>
	/// <param name="pBaseMemory">
	/// Specifies the full path filename name of Symbol Key file that was previously saved.
	/// Note, it is recommended that you pass a value of zero and not use this feature at this time.
	/// </param>
	/// <param name="controlFlags">
	/// Specify zero or more of one of the control flags that will remain in effect for the entire processing of the NxCore Tape.
	/// Use enum controlFlags.
	/// </param>
	/// <param name="UserData">
	/// The value of user data is copied to NxCoreSystem.UserData and is available to reference or change during your callback function.
	/// </param>
	/// <param name="stdcallback">
	/// The callback function that will be called for all messages when processing the NxCore Tape.
	/// </param>
	/// <returns>
	/// Returns 0 if no errors and completed processing the NxCore Tape.
	/// Returns -1 if NxCALLBACKRETURN_STOP was returned by the callback function.
	/// Returns -2 if the KeyFile failed to load properly.
	/// Returns -3 if the file could not be opened.
	/// Returns a positive number if an exception error was encountered. The value is the same as the last NxCoreSyste.StatusData.
	/// </returns>
	
	// Callback Delegates
	unsafe public delegate int NxCoreCallback([In] IntPtr pSys, [In] IntPtr pMsg);
	unsafe public delegate int NxCoreCallbackTapeList(IntPtr pYourParam, [In] IntPtr pNcTF);
	unsafe public delegate int NxCoreCallbackAPIDLLsList(IntPtr pYourParam, [In] IntPtr pNcDF);

    [DllImport("NxCoreAPI.dll", EntryPoint="sNxCoreAPIVersion")]
    public static extern uint Version();
	     	
	// Functions
    [DllImport("NxCoreAPI.dll", EntryPoint="sNxCoreProcessTape")]
	public static extern int ProcessTape(
        [In, MarshalAs(UnmanagedType.LPStr)] string pszFilename,
        [In, MarshalAs(UnmanagedType.LPStr)] string pBaseMemory,
        uint controlFlags,
        uint UserData,
        [In] NxCoreCallback stdcallback);
	
	[DllImport("NxCoreAPI.dll", EntryPoint = "sNxCoreSetCallback")]
	public static extern NxCoreCallback SetCallback(		
		[In] NxCoreCallback stdcallback);

    [DllImport("NxCoreAPI.dll", EntryPoint="sNxCoreSpinSymbols")]
	public static extern int SpinSymbols(       // returns <0 on error
        ushort listedExchange,                  // use -1 to choose all listed exchanges on the tape
        uint iterateOptions,                    // 1 = options only, 0 = regular symbols, do it twice to get both
        uint spinID,                            // will be assigned to the NxCoreMessage.coreData.SymbolSpin.SpinID member
        [In] NxCoreCallback tempCallback,       // null to use existing callback, otherwise temporarily override with this
        int tempUserData);                      // pass a non-zero value to temporarily override the NxCoreSystem.UserData value
  
    [DllImport("NxCoreAPI.dll", EntryPoint="sNxCoreDateFromNDays")]
	unsafe public static extern int DateFromNDays( // returns nothing, fills a NxDate structure from the NDays member
	  [In, Out] ref NxDate pnxDate);// ref of an NxDate struct to be filled

    [DllImport("NxCoreAPI.dll", EntryPoint="sNxCoreDateFromYMD")]
	unsafe public static extern int DateFromYMD( // returns nothing, fills a NxDate structure from the YMD members
	  [In, Out] ref NxDate pnxDate);// ref of an NxDate struct to be filled

    [DllImport("NxCoreAPI.dll", EntryPoint="sNxCorePriceToDouble")]
	public static extern double PriceToDouble(       // returns a double that represents Price and PriceType combination
        int Price,                                   // integer representing the price to format
        byte PriceType);                             // The NxPriceType that Price is using

    [DllImport("NxCoreAPI.dll", EntryPoint="sNxCorePriceConvert")]
	public static extern int PriceConvert(       // returns the value of iPrice as converted into newPriceType
        int iPrice,                              // integer price to be converted
        byte orgPriceType,                       // the NxPriceType that iPrice is currently using
        byte newPriceType);                      // the NxPriceType to convert iPrice into

    [DllImport("NxCoreAPI.dll", EntryPoint="sNxCorePriceFormat")]
	public static extern int PriceFormat(       // returns length of formatted string in szBuff
        StringBuilder szBuff,                   // capacity must be at least 256 chars
        int Price,                              // integer representing the price to format
        byte PriceType,                         // the NxPriceType that Price is using
        int expandWidth,                        // set to a non-zero value to pad the buffer to at least this length
        uint bInsertCommas);                    // set to 1 to format the price using commas to separate thousands

    [DllImport("NxCoreAPI.dll", EntryPoint="sNxCoreGetDefinedString")]
	unsafe public static extern sbyte* GetDefinedString( // returns pointer to the string, or null if not found
        int ixTable,                            // set to one of the NxST_ values
        int ixString);                          // a zero-based number that indexes into one of the String Tables indexed

	[DllImport("NxCoreAPI.dll", EntryPoint = "sNxCoreGetTapeName")]
	unsafe public static extern int GetTapeName(    
	    [Out] char[] szBuffTapeName,	    
		int nBufferBytes);

    [DllImport("NxCoreAPI.dll", EntryPoint="sNxCoreStateGetLastTrade")]
	unsafe public static extern int StateGetLastTrade( // returns 0 if successful
        [In, Out] ref NxCoreStateTrade pStateTrade,    // ref to struct to be filled
        [In] System.IntPtr pnxsSymOrContract);         // pass null to choose current symbol for callback message
                                                       // or pointer to NxString obtained in previous callback from one of
                                                       // NxCoreMessage.coreHeader.pnxStringSymbol or 
                                                       // NxCoreMessage.coreheader.pnxOptionHdr->pnxsDateAndStrike

    [DllImport("NxCoreAPI.dll", EntryPoint="sNxCoreStateGetExgQuotes")]
	unsafe public static extern int StateGetExgQuotes(       // returns 0 if successful
		[In, Out] ref NxCoreStateExgQuotes pStateExgQuotes,  // NxCoreStateExgQuotes* ref to struct to be filled
        [In] System.IntPtr pnxsSymOrContract);               // pass null to choose current symbol for callback message
                                                             // or pointer to NxString obtained in previous callback from one of                                                                                
	                                                         // NxCoreMessage.coreHeader.pnxStringSymbol or 
	                                                         // NxCoreMessage.coreheader.pnxOptionHdr->pnxsDateAndStrike
  
    [DllImport("NxCoreAPI.dll", EntryPoint="sNxCoreStateGetMMQuotes")]
	unsafe public static extern int StateGetMMQuotes(       // returns 0 if successful
        ushort ReportingExg,                                // the Reporting Exchange to get MM Quotes from	    
		[In, Out] ref NxCoreStateMMQuotes pStateMMQuotes,   // NxCoreStateMMQuotes* ref to struct to be filled
	    [In] System.IntPtr pnxsSymOrContract);              // pass null to choose current symbol for callback message
                                                            // or pointer to NxString obtained in previous callback from one of
                                                            // NxCoreMessage.coreHeader.pnxStringSymbol or 
                                                            // NxCoreMessage.coreheader.pnxOptionHdr->pnxsDateAndStrike

    [DllImport("NxCoreAPI.dll", EntryPoint="sNxCoreSaveState")]
	public static extern int SaveState(                               // returns 0 if successful
        [In, MarshalAs(UnmanagedType.LPStr)] string szStateFilename,  // full pathname of file to save state in
        uint controlFlags);                                           // ?

    [DllImport("NxCoreAPI.dll", EntryPoint = "sNxCoreListTapes")]
	unsafe public static extern int ListTapes(        // returns number of tape files found
        uint controlFlags,                            // unused, set to 0
        [In] NxCoreCallbackTapeList stdcallback,      // callback function for receiving tape filenames
        [In] void* pYourParam);                       // pointer to be used for own purposes

	[DllImport("NxCoreAPI.dll", EntryPoint = "sNxCoreListAPIDLLs")]
	unsafe public static extern int ListAPIDLLs(      // returns number of tape files found
		uint controlFlags,                            // unused, set to 0
		[In] NxCoreCallbackAPIDLLsList stdcallback,   // callback function for receiving tape filenames
		[In] void* pYourParam);                       // pointer to be used for own purposes
	
    #endregion
	 
	#region Constants

	//return one of these values from your UserCallback function.
    public const byte NxCALLBACKRETURN_CONTINUE = 0; // continue normal processing
    public const byte NxCALLBACKRETURN_STOP = 1;	 // stop processing and return control to function that called NxCoreProcessTape
    public const byte NxCALLBACKRETURN_RESTART = 2;	 // restart processing from beginning of tape. UserData and strings are preserved.

    //NxCoreSystem.ClockUpdateInterval definitions
    public const byte NxCLOCK_NOCHANGE = 0;	// clock has not changed since last message.
    public const byte NxCLOCK_CLOCK = 1;	// NxCoreSystem.nxTime.Millisecond is longest interval that changed.
    public const byte NxCLOCK_SECOND = 2;	// NxCoreSystem.nxTime.Second is longest interval that changed.
    public const byte NxCLOCK_MINUTE = 3;	// NxCoreSystem.nxTime.Minute is longest interval that changed.
    public const byte NxCLOCK_HOUR = 4;		// NxCoreSystem.nxTime.Hour is longest interval that changed.

    //NxCoreSystem.Status definitions 
    public const byte NxCORESTATUS_RUNNING = 0;			  // system is running.
    public const byte NxCORESTATUS_INITIALIZING = 1;	  // first message sent when processing a tape.
    public const byte NxCORESTATUS_COMPLETE = 2;		  // last message sent when processing a tape, unless user aborts by returning a non-zero value (NxCALLBACKRETURN_STOP) from callback.
    public const byte NxCORESTATUS_SYNCHRONIZING = 3;	  // synchronization reset detected in the tape, continuing processing
    public const byte NxCORESTATUS_ERROR = 4;			  // an error has occurred and processing halted. NxCORESTATUS_COMPLETE message will follow
    public const byte NxCORESTATUS_WAITFORCOREACCESS = 5; // NxCoreAPI.dll is waiting for NxCore Access to connect.
    public const byte NxCORESTATUS_RESTARTING_TAPE = 6;	  // NxCoreAPI.dll is restarting the same tape from the beginning at users request
    public const byte NxCORESTATUS_LOADED_STATE = 7;	  // NxCoreAPI.dll initialized from state
    public const byte NxCORESTATUS_SAVING_STATE = 8;	  // NxCoreAPI.dll will capture the state after this message
    public const byte NxCORESTATUS_SYMBOLSPIN = 9;		  // signals system symbol spin state: StatusData == 0 when starting, 1 when complete
	public const byte NxCORESTATUS_TAPEOPEN = 10;         // signals system when tape header has been read, tape definition string and date are available for the first time.

    //StatusData member may be set to one of the following 
    //
    //when NxCoreSystem.Status == NxCORESTATUS_RUNNING
    public const byte NxCSRUNMODE_SRC_DISK_FILE = 0;	 // processing from a tape on disk
    public const byte NxCSRUNMODE_SRC_ACCESS_FILE = 1;	 // processing from NxCoreAccess's file -- dll has not yet reached memory buffers
    public const byte NxCSRUNMODE_SRC_ACCESS_MEMORY = 2; // processing from NxCoreAccess's memory buffers

    public const byte NxCSRUNMODE_SRC_ACCESS_MB_OLDEST = 2;	 // processing oldest of NxCoreAccess's memory buffers
    public const byte NxCSRUNMODE_SRC_ACCESS_MB_LOWER = 3;	 // processing older half of NxCoreAccess's memory buffers   
    public const byte NxCSRUNMODE_SRC_ACCESS_MB_UPPER = 4;	 // processing most recent half of NxCoreAccess's memory buffers
    public const byte NxCSRUNMODE_SRC_ACCESS_MB_SECOND = 5;	 // processing 2nd most recent.
    public const byte NxCSRUNMODE_SRC_ACCESS_MB_CURRENT = 6; // processing most recent of NxCoreAccess's memory buffers

    //when NxCoreSystem.Status == NxCORESTATUS_SAVING_STATE
    public const sbyte NxCSSAVESTATE_CAPTURE = 0;		 // the core state will be captured after you return from this callback
    public const sbyte NxCSSAVESTATE_COMPLETE = 1;		 // the save core state operation is complete
    public const sbyte NxCSSAVESTATE_ERR_CREATEFILE = -1; // failed to open the specified tape filename
    public const sbyte NxCSSAVESTATE_ERR_DISKSPACE = -2;	 // not enough disk space to complete the operation
    public const sbyte NxCSSAVESTATE_ERR_MEMORY = -3;	 // insufficient memory to complete the operation

    //when NxCoreSystem.Status == NxCORESTATUS_SYMBOLSPIN			  
    public const byte NxCSSYMBOLSPIN_STARTING = 0;	// the system symbol spin is starting -- note -- status will return NxCORESTATUS_RUNNING after this message
    public const byte NxCSSYMBOLSPIN_COMPLETE = 1;	// the system symbol spin has completed

    //String Table Indexes for NxCoreGetDefinedString
    public const byte NxST_PERMID = 2;
    public const byte NxST_EXCHANGE = 3;
    public const byte NxST_TRADECONDITION = 4;
    public const byte NxST_QUOTECONDITION = 5;
    public const byte NxST_CATEGORY = 6;
    public const byte NxST_HALTSTATUS = 7;
	public const byte NxST_HALTREASONTYPE = 8;
    public const byte NxST_OPENINDICATIONTYPE = 9;
    public const byte NxST_ORDERIMBALANCETYPE = 10;
    public const byte NxST_TRADEREPORTREASON = 11;
    public const byte NxST_EXGCORRECTIONMAP = 12;
    public const byte NxST_SYMBOLCHANGETYPE = 13;
    public const byte NxST_OHLCFLAGS = 14;
    public const byte NxST_EXDIVIDENDATTRIB = 15;
    public const byte NxST_OPRAEXCHANGELIST = 16;
    public const byte NxST_NASDBIDTICK = 17;
    public const byte NxST_MARKETMAKERTYPE = 18;
    public const byte NxST_MMQUOTETYPE = 19;
    public const byte NxST_REFRESHTYPE = 20;
    public const byte NxST_EXPIRATIONCYCLE = 21;
    public const byte NxST_VOLUMETYPE = 22;
    public const byte NxST_QTEMATCHTYPE = 23;
    public const byte NxST_QTEMATCHFLAGS = 24;
    public const byte NxST_SIGHIGHLOWTYPE = 25;
    public const byte NxST_BBOCHANGEFLAGS = 26;
    public const byte NxST_TRADECONDFLAGS = 27;
    public const byte NxST_PRICEFLAGS = 28;
    public const byte NxST_SETTLEFLAGMAP = 29;
    public const byte NxST_SYMBOLFNCLSTATUSMAP = 30;
    public const byte NxST_EQUITYCLASSIFICATIONS = 31;

    //control flag for NxCoreSaveState
    public const byte NxSAVESTATE_GRADUALLY = 0; //  Default saves gradually: writes out memory block to file in 1mb increments
    public const byte NxSAVESTATE_ONEPASS = 1;	 // save memory state in one pass -- save completes in one pass. Default saves gradually.
    public const byte NxSAVESTATE_CANCEL = 2;

    //error codes returned from NxCoreAPI functions
    public const sbyte NxAPIERR_NOT_CALLBACK_THREAD = -1;
    public const sbyte NxAPIERR_BAD_PARAMETERS = -2;
    public const sbyte NxAPIERR_EXCEPTION = -3;
    public const sbyte NxAPIERR_OPEN_TAPE_FILE_FAILED = -4;
    public const sbyte NxAPIERR_INITIALIZE_MEMORY_FAILED = -5;
    public const sbyte NxAPIERR_NOLISTED_EXG = -6;			  // symbol spin -- listed exchange does not exist in current tape
    public const sbyte NxAPIERR_NOSYMBOLS_FOR_LSTEXG = -7;	  // symbol spin -- no symbols of the type (options/not options) for the exchange specified
    public const sbyte NxAPIERR_NOSESSION_FOR_SYMBOL = -8;	  // symbol spin -- no session or session does not have data type
    public const sbyte NxAPIERR_NODATATYPE_FOR_SESSION = -9;	  // symbol spin -- There's a session, but no trade/quote/mmquote data for session
    public const sbyte NxAPIERR_NODATA_FOR_REPEXG = -10;		  // symbol spin -- MMQuotes. there is session with data, but no entry for the specified reporting exg
    public const sbyte NxAPIERR_ZEROED_DATA_FOR_SESSION = -11; // symbol spin -- there is a session, but data is all zero
    public const sbyte NxAPIERR_SAVE_STATE_IN_PROGRESS = -12;
    public const sbyte NxAPIERR_NOT_SUPPORTED = -13;

    //qteRefresh
    public const byte NxQTE_REFRESH_REALTIME = 0;	 // realtime quote, not a refresh
    public const byte NxQTE_REFRESH_SOURCE = 1;		 // The source of the data has refreshed quote. Only seen for Market Makers in morning reset
    public const byte NxQTE_REFRESH_MISMATCH = 2;	 // A mismatch detected in NxCore processors from last received quote information.
    public const byte NxQTE_REFRESH_SYNCHRONIZE = 3; // NxCore processors are resending last known quotes to ensure synchronization. Rare.
 
    //NxCoreExgQuote.BBOChangeFlags definitions
    public const byte NxBBOCHANGE_BIDEXG   = 0x01; // BestBidExg  != PrevBestBidExg
    public const byte NxBBOCHANGE_BIDPRICE = 0x02; // BestBidPriceChange != 0
    public const byte NxBBOCHANGE_BIDSIZE  = 0x04; // BestBidSizeChange != 0
    public const byte NxBBOCHANGE_ASKEXG   = 0x10; // BestAskExg  != PrevBestAskExg
    public const byte NxBBOCHANGE_ASKPRICE = 0x20; // BestAskPriceChange != 0
    public const byte NxBBOCHANGE_ASKSIZE  = 0x40; // BestAskSizeChange != 0

    //NxCoreQuote.NasdaqBidTick definitions   
    public const byte NxQTE_NASDBIDTICK_NA = 0;	  // bid tick is not applicable for symbol.
    public const byte NxQTE_NASDBIDTICK_UP = 1;	  // set when bid tick is up.
    public const byte NxQTE_NASDBIDTICK_DN = 2;	  // set when bid tick is down.
    public const byte NxQTE_NASDBIDTICK_NO = 3;	  // NASDAQ stock that does not carry a bid tick (small caps, etc)

    //NxCoreMMQuote.MarketMakerType definitions
    public const byte NxMMT_UNKNOWN = 0;   // not set, not applicable
    public const byte NxMMT_REGULAR = 1;   // regular market maker -- not primary, not passive.
    public const byte NxMMT_PRIMARY = 2;   // primary market maker.
    public const byte NxMMT_PASSIVE = 3;   // passive market maker.
 
    //NxCoreMMQuote.QuoteType definitions
    public const byte NxMMQT_UNKNOWN = 0;
    public const byte NxMMQT_REGULAR = 1;
    public const byte NxMMQT_PASSIVE = 2;   // passive quote. (not same thing as MMT_PASSIVE which is a type of market maker)
    public const byte NxMMQT_SYNDICATE = 3;	   // Syndicate        BID ONLY
    public const byte NxMMQT_PRESYNDICATE = 4; // Presyndicate BID ONLY
    public const byte NxMMQT_PENALTY = 5;	   // Penalty.     BID ONLY

    //NxCoreTrade.PriceFlags definitions.
    public const byte NxTPF_SETLAST   = 0x01;	// Update the 'last' price field with the trade price.
    public const byte NxTPF_SETHIGH   = 0x02;	// Update the session high price.
    public const byte NxTPF_SETLOW    = 0x04;	// Update the session low price.
    public const byte NxTPF_SETOPEN   = 0x08;	// Indicates trade report is a type of opening report. For snapshot indicies, this is the "open" field. See TradeConditions for the types that update this flag.
    public const byte NxTPF_EXGINSERT = 0x10;	// Trade report was inserted, not real-time. Often follows EXGCANCEL for trade report corrections.
    public const byte NxTPF_EXGCANCEL = 0x20;	// Cancel message. The data in this trade report reflects the state of the report when first sent, including the SETLAST/increment volume, etc flags.
    public const byte NxTPF_SETTLEMENT= 0x40;	// price is settlement

    //NxCoreTrade.ConditionFlags definitions.
    public const byte NxTCF_NOLAST = 0x01;	// not eligible to update last price.
    public const byte NxTCF_NOHIGH = 0x02;	// not eligible to update high price.
    public const byte NxTCF_NOLOW  = 0x04;	// not eligible to update low price.

    //NxCoreTrade.VolumeType definitions
    public const byte NxTVT_INCRVOL = 0;	  // incremental volume. UNSIGNED, increment tick volume and total volume. Note: dwSize maybe zero -- which updates the tick Volume and leaves total volume unchanged
    public const byte NxTVT_NONINCRVOL = 1;	  // non-incremental volume. Rarely used outside of indexes. Intraday and Open detail in NYSE stocks.
    public const byte NxTVT_TOTALVOL = 2;  // dwSize *is* the total volume -- used mainly in a few indexes
    public const byte NxTVT_TOTALVOLx100 = 3; // dwSize *is* the total volume/100. multiply by 100 for current total volume. RARE -- in DOW indexes

    //NxCoreTrade.nxAnalysis.QteMatchTypeRGN and
    //NxCoreTrade.nxAnalysis.QteMatchTypeBBO definitions.
    public const byte NxRTA_QMTYPE_NONE = 0;	// no recent quotes.
    public const byte NxRTA_QMTYPE_BID = 1;		// at bid or higher.
    public const byte NxRTA_QMTYPE_ASK = 2;		// at ask or lower
    public const byte NxRTA_QMTYPE_INSIDE = 3;	// exactly between bid/ask. 0 dist means locked.
    public const byte NxRTA_QMTYPE_BELOWBID = 4; // lower than bid
    public const byte NxRTA_QMTYPE_ABOVEASK = 5; // higher than ask

    //NxCoreTrade.nxAnalysis.QteMatchFlagsRGN and
    //NxCoreTrade.nxAnalysis.QteMatchFlagsBBO definitions.
    public const byte NxRTA_QTEMATCHFLAG_OLDER = 0x01;	  // matched a quote older than a previously matched quote, possible indication of staleness
    public const byte NxRTA_QTEMATCHFLAG_CROSSED = 0x02;

    //NxCoreTrade.nxAnalysis.SigHiLoType definitions.
    public const byte NxRTA_SIGHL_EQ = 0;
    public const byte NxRTA_SIGHL_LOWEST = 1;
    public const byte NxRTA_SIGHL_LOW = 2;
    public const byte NxRTA_SIGHL_HIGH = 3;
    public const byte NxRTA_SIGHL_HIGHEST = 4;

    //NxCategoryField.FieldType definitions.
    public const byte NxCFT_UNKNOWN = 0;
    public const byte NxCFT_64BIT = 1;
    public const byte NxCFT_32BIT = 2;
    public const byte NxCFT_STRINGZ = 3;
    public const byte NxCFT_DOUBLE = 4;
    public const byte NxCFT_PRICE = 5;
    public const byte NxCFT_DATE = 6;
    public const byte NxCFT_TIME = 7;
    public const byte NxCFT_NxSTRING = 8;
    public const byte NxCFT_STRING_IDX = 9;
    public const byte NxCFT_STRING_MAP = 10;

    //NxCoreSymbolChange.Status values
    public const byte NxSS_ADD = 0;	  // symbol has been added effective immediately.
    public const byte NxSS_DEL = 1;	  // symbol marked for deletion at end of session.
    public const byte NxSS_MOD = 2;	  // symbol changed, or moved from one ListedExg to another. pnxsSymbolOld, pnxOptionHdrOld, and ListedExgOld are set.
 
    //NxCoreMessage.MessageType
    public const byte NxMSG_STATUS = 0;
    public const byte NxMSG_EXGQUOTE = 1;
    public const byte NxMSG_MMQUOTE = 2;
    public const byte NxMSG_TRADE = 3;
    public const byte NxMSG_CATEGORY = 4;
    public const byte NxMSG_SYMBOLCHANGE = 5;
    public const byte NxMSG_SYMBOLSPIN = 6;

    public const ushort NxSPIN_ALL_EXGS = 65535;
    public const ushort NxSPIN_OPTIONS = 0x01;

    public const byte NxLTF_SEARCH_LOCAL = 0;
    public const byte NxLTF_SEARCH_NETWORK = 0x01;
    public const byte NxLTF_SEARCH_REMOVEABLE = 0x02;

    //controlFlags for NxCoreProcessTape
    public const byte NxCF_EXCLUDE_QUOTES = 0x01;		  // exclude Exchange Quotes
    public const byte NxCF_EXCLUDE_QUOTES2 = 0x02;		  // exclude MMQuotes
    public const byte NxCF_EXCLUDE_OPRA = 0x04;			  // exclude all option from OPRA (US)
    public const byte NxCF_FAVOR_NBBO = 0x08;
    public const long NxCF_MEMORY_ADDRESS = 0x4000000;	  // if set, memory is allocated (if possible) at the address specified by the 2nd parameter to NxCoreProcessTape (pBaseMemory)
    public const long NxCF_EXCLUDE_CRC_CHECK = 0x8000000; // exclude crc checking

	#endregion

	public NxCore()
    {
	}	
  } // CLASS Definition
#endregion

} // Namespace
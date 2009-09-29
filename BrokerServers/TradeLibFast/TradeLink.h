#pragma once

	const CString LIVEWINDOW = _T("TL-BROKER-LIVE");
	const CString SIMWINDOW = _T("TL-BROKER-SIMU");
	const CString HISTWINDOW = _T("TL-BROKER-HIST");
	const CString TESTWINDOW = _T("TL-BROKER-TEST");
	const CString SERVERWINDOW = _T("TradeLinkServer");
	const CString CLIENTWINDOW = _T("TradeLinkClient");

	enum TLTypes
	{
		NONE = 0,
        SIMBROKER = 2,
        LIVEBROKER = 4,
        HISTORICALBROKER = 8,
        TESTBROKER = 16,
		ANYSERVER = 255,
	};

	enum TL2 {
        // START TRADELINK STATUS MESSAGES
        ORDER_NOT_FOUND = -112,
        TLCLIENT_NOT_FOUND = -111,
        INVALID_ACCOUNT = -110,
        UNKNOWN_ERROR = -109,
        FEATURE_NOT_IMPLEMENTED = -108,
        CLIENTNOTREGISTERED = -107,
        EMPTY_ORDER = -106,
        UNKNOWN_MESSAGE = -105,
        UNKNOWN_SYMBOL = -104,
        BROKERSERVER_NOT_FOUND = -103,
        INVALID_ORDERSIZE = -102,
        DUPLICATE_ORDERID = -101,
        SYMBOL_NOT_LOADED = -100,
        OK = 0,
        // END STATUS MESSAGES
        // START CUSTOM MESSAGES
        CUSTOM1 = 1,
        CUSTOM2,
        CUSTOM3,
        CUSTOM4,
        CUSTOM5,
        CUSTOM6,
        CUSTOM7,
        CUSTOM8,
        CUSTOM9,
        CUSTOM10,
        CUSTOM11,
        CUSTOM12,
        CUSTOM13,
        CUSTOM14,
        CUSTOM15,
        CUSTOM16,
        CUSTOM17,
        CUSTOM18,
        CUSTOM19,
        CUSTOM20,
        ISSHORTABLE,
        VWAP,
        LASTTRADESIZE,
        LASTTRADEPRICE,
        LASTBID,
        LASTASK,
        BIDSIZE,
        ASKSIZE,
        DAYLOW,
        DAYHIGH,
        OPENPRICE,
        CLOSEPRICE,
        LRPBUY,
        LRPSELL,
        AMEXLASTTRADE,
        NASDAQLASTTRADE,
        NYSEBID,
        NYSEASK,
        NYSEDAYHIGH,
        NYSEDAYLOW,
        NYSELASTTRADE,
        NASDAQIMBALANCE,
        NASDAQPREVIOUSIMBALANCE,
        NYSEIMBALACE,
        NYSEPREVIOUSIMBALANCE,
        POSPRICEREQUEST,
        POSSIZEREQUEST,
        SENDORDEROCO,
        SENDORDEROSO,
        // END CUSTOM MESSAGES
        // START TRADELINK MESSAGES
		// requests
        SENDORDER = 5000,
        BROKERNAME,
        VERSION,
		REGISTERCLIENT,
        REGISTERSTOCK,
        CLEARSTOCKS,
        CLEARCLIENT,
        HEARTBEAT,
        ACCOUNTREQUEST,
        ORDERCANCELREQUEST,
        POSITIONREQUEST,
        FEATUREREQUEST,
        DOMREQUEST,
        IMBALANCEREQUEST,
		SENDORDERMARKET,
        SENDORDERLIMIT,
        SENDORDERSTOP,
        SENDORDERTRAIL,
        SENDORDERMARKETONCLOSE,
		// responses or acks
        TICKNOTIFY,
        EXECUTENOTIFY,
        ORDERNOTIFY,
        ACCOUNTRESPONSE,
        ORDERCANCELRESPONSE,
        FEATURERESPONSE,
        POSITIONRESPONSE,
        IMBALANCERESPONSE,
        DOMRESPONSE,
        LIVEDATA,
        LIVETRADING,
        SIMTRADING,
        HISTORICALDATA,
        HISTORICALTRADING,
        LOOPBACKSERVER,
        LOOPBACKCLIENT,
        STARTHISTORICALRUN,
        ENDHISTORICALRUN,
        // END TRADELINK MESSAGES
	};

	enum Brokers
	{
		Unknown = -1,
        TradeLink = 0,
        Assent,
        InteractiveBrokers,
        Genesis,
        Bright,
        Echo,
        Sterling,
        TDAmeritrade,
        Blackwood,
        MBTrading,
        HUBB,
        Tradespeed,
        REDI,
        eSignal,
        IQFeed,
        TrackData,
        TradingTechnologies,
        ZenFire,
        GAINCapital,
        FxCm,
        OpenEcry,
        DBFX
	};






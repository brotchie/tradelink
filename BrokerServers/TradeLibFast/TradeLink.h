#pragma once

	const CString LIVEWINDOW = _T("TL-BROKER-LIVE");
	const CString SIMWINDOW = _T("TL-BROKER-SIMU");
	const CString HISTWINDOW = _T("TL-BROKER-HIST");
	const CString TESTWINDOW = _T("TL-BROKER-TEST");

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
		OK = 0,
		SENDORDER = 1,
		BROKERNAME = 28,
		VERSION = 31,
		ISSHORTABLE,
		VWAP,	// GetVwap
		LASTTRADESIZE,	// GetLastTradeSize
		LASTTRADEPRICE,	// GetLastTradePrice
		LASTBID,				// GetBid
		LASTASK,				// GetAsk
		BIDSIZE,			// GetBidSize
		ASKSIZE,			// GetAskSize
		DAYLOW,			// GetDayLow
		DAYHIGH,			// GetDayHigh
		OPENPRICE,		// GetOpenPrice
		CLOSEPRICE,		// GetClosePrice - yesterday
		LRPBUY,				// GetLRP buy side
		LRPSELL,			// GetLRP sell side
		AMEXLASTTRADE,		// GetAmexLastTrade
		NASDAQLASTTRADE,		// GetNasdaqLastTrade
		NYSEBID,			// GetNyseBid
		NYSEASK,			// GetNyseAsk
		NYSEDAYHIGH,		// GetNyseDayHigh
		NYSEDAYLOW,			// GetNyseDayLow
		NYSELASTTRADE,		// GetNyseLastTrade
		NASDAQIMBALANCE,	// GetNasdaqImbalance
		NASDAQPREVIOUSIMBALANCE,	// GetNasdaqPreviousImbalance
		NYSEIMBALACE,		// GetNyseImbalance
		NYSEPREVIOUSIMBALANCE,	// GetNysePreviousImbalance
		TICKNOTIFY = 100,
		EXECUTENOTIFY,
		REGISTERCLIENT,
		REGISTERSTOCK,
		CLEARSTOCKS,
		CLEARCLIENT,
		HEARTBEAT,
		ORDERNOTIFY,
		ACCOUNTRESPONSE = 500,
		ACCOUNTREQUEST,
		ORDERCANCELREQUEST,
		ORDERCANCELRESPONSE,
		FEATUREREQUEST,
		FEATURERESPONSE,
		POSITIONREQUEST,
		POSITIONRESPONSE,
		FEATURE_NOT_IMPLEMENTED = 994,
		CLIENTNOTREGISTERED = 995,
		GOTNULLORDER = 996,
		UNKNOWNMSG,
		UNKNOWNSYM,
		TL_CONNECTOR_MISSING,
		BAD_PARAMETERS,
		DUPLICATE_ORDERID,
	};

	enum Brokers
	{
		UnknownBroker = -1,
		TradeLinkSimulation = 0,
		Assent,
		InteractiveBrokers,
		Genesis,
		Bright,
		Echo,
	};






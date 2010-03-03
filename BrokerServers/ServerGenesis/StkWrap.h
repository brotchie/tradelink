#pragma once
#include "GTStock.h"
#include "GTPending.h"
#include "ServerGenesis.h"
using namespace TradeLibFast;

// number of orders to maintain in GTAPI
#define MAXDEPTH 30

// number of aggregated price levels to send
#define SENDDEPTH 10

// server can send 10 miliseconds accuracy, but TradeLink doesn't like
#undef LONGTIME

// verbose mode
#undef VERB

class ServerGenesis;

class StkWrap : public GTStock  
{
public:


public:
	StkWrap(GTSession &session, LPCSTR pszStock,int symid);
	virtual ~StkWrap();
	int _symid;
	int _depth;

	double prevBids[SENDDEPTH];
	double prevAsks[SENDDEPTH];
	long prevBidShares[SENDDEPTH];
	long prevAskShares[SENDDEPTH];

	int prevTradeTime;
	int maxTime;

	void SendData();
	void SetupDepth(int depth);

public:
	ServerGenesis* tl;

protected:
	int date;
public:
	virtual int OnTick();

protected:
	virtual int OnGotQuoteLevel1(GTLevel1 *pRcd);
	virtual int OnGotLevel2Record(GTLevel2 *pRcd);
	virtual int OnGotQuotePrint(GTPrint *pRcd);
	virtual int OnBestBidPriceChanged();
	virtual int OnBestAskPriceChanged();
	virtual int OnExecMsgErrMsg(const GTErrMsg &err);
	virtual int OnExecMsgSending(const GTSending &sending);
	virtual int OnExecMsgPending(const GTPending &pending);
	virtual int OnExecMsgTrade(const GTTrade &trade);
	virtual int OnExecMsgOpenPosition(const GTOpenPosition &open);
	virtual int OnExecMsgCancel(const GTCancel & cancel);
	virtual int OnSendingOrder(const GTSending &gtsending);

};


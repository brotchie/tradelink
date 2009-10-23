#pragma once
#include "GTStock.h"
#include "GTPending.h"
#include "ServerGenesis.h"
using namespace TradeLibFast;

class ServerGenesis;

class StkWrap : public GTStock  
{
public:


public:
	StkWrap(GTSession &session, LPCSTR pszStock,int symid);
	virtual ~StkWrap();
	int _symid;

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


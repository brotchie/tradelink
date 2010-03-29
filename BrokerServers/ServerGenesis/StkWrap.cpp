
#include "stdafx.h"
#include "testapi.h"
#include "TestAPIDlg.h"
#include "StkWrap.h"

using namespace TradeLibFast;

StkWrap::StkWrap(GTSession &session, LPCSTR pszStock, int symid)
	: GTStock(session, pszStock)
{

	_symid = symid;
	tl = NULL;

	time_t now;
	time(&now);
	CTime ct(now);
	date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();


}

void StkWrap::SetupDepth(int depth)
{

	_depth = depth;
	for(int i = 0; i< depth; i++)
	{
		prevBids[i] = 0;
		prevAsks[i] = 0;
		prevBidShares[i] = 0;
		prevAskShares[i] = 0;
	}

	prevTradeTime = -42;
	maxTime = -42;


#ifdef UDP_QUOTE
	m_level2.m_bid.m_nMaxLevels = 10;
	m_level2.m_ask.m_nMaxLevels = 10;

	m_prints.SetMinMax(128, 256);
#endif

	m_level2.m_bid.m_nMaxLevels = depth;
	m_level2.m_ask.m_nMaxLevels = depth;
}

inline int GT2TL(GTime32 gtime)
{
#ifdef LONGTIME
		return GT2TLLONG(gtime);
#else
		return (gtime.chSec+gtime.chMin*100+gtime.chHour*10000);
#endif
}

inline int GT2TLLONG(GTime32 gtime)
{
	return (gtime.chSec+gtime.chMin*100+gtime.chHour*10000)*100+gtime.chSec100;
}


StkWrap::~StkWrap()
{
	tl = NULL;
}

int StkWrap::OnExecMsgErrMsg(const GTErrMsg &err)
{
	// ensure we have the sequence
	long oseq = err.dwOrderSeqNo;
	int index = tl->GetIDIndex(oseq,SEQ);
	if (index==NO_ID) return GTStock::OnExecMsgErrMsg(err);
	// get id
	int64 id = tl->orderids[index];
	CString sym = CString(err.szStock);
	CString errmsg = CString(err.szText);
	int ioseq = (int)oseq;
	CString m;
	m.Format("error: %s %s %i %i %u",sym,errmsg,err.nErrCode,ioseq,id);
	tl->D(m);
	return GTStock::OnExecMsgErrMsg(err);
}

int StkWrap::OnExecMsgCancel(const GTCancel &cancel)
{
	int idx = tl->GetIDIndex(cancel.dwTicketNo,TICKET);
	int64 id = tl->orderids[idx];
	tl->SrvGotCancel(id);


	return GTStock::OnExecMsgCancel(cancel);
}

int StkWrap::OnGotQuoteLevel1(GTLevel1 *pRcd)
{

#ifdef UDP_QUOTE
	tl->D("Processing UDP\n");
	return GTStock::OnGotQuoteLevel1(pRcd);
#else
	CString strStock;

	if(strStock != pRcd->szStock)
		return GTStock::OnGotQuoteLevel1(pRcd);

	return GTStock::OnGotQuoteLevel1(pRcd);
#endif	
}
#define SHAREUNITS 100
int StkWrap::OnGotLevel2Record(GTLevel2 *pRcd)
{

#ifdef UDP_QUOTE
	tl->D("Processing UDP\n");
	return GTStock::OnGotLevel2Record(pRcd);
#else
	CString strStock;

	int rc = GTStock::OnGotLevel2Record(pRcd);
	// make sure depth is enabled
	if (tl->_depth==0)
	{
		char chSide = pRcd->chSide;

        TLTick k;
        k.sym = CString(pRcd->szStock);
        k.symid = _symid;
        k.depth = 0;
        k.time = GT2TL(pRcd->gtime);
        k.date = date;
        
        if (pRcd->chSide=='B')
        {
                k.bid = pRcd->dblPrice;
                k.bs = pRcd->dwShares / SHAREUNITS;
                k.be = CString(CAST_MMID_TEXT(pRcd->mmid));
        }
        else
        {
                k.ask = pRcd->dblPrice;
                k.os = pRcd->dwShares / SHAREUNITS;
                k.oe = CString(CAST_MMID_TEXT(pRcd->mmid));

        }
        tl->SrvGotTick(k);
	}
	else 
	{
		if(pRcd->nOwnShares > 0)
		{
			char str[1000];
			// didn't work :(
			sprintf(str, "owned: %d", pRcd->nOwnShares);
			tl->D( str );
		}
		SendData();
	}




	SendData();
	return rc;

#endif
}

int StkWrap::OnTick()
{
	GTStock::OnTick();

	return 0;
}

int StkWrap::OnGotQuotePrint(GTPrint *pRcd)
{
	TLTick k;
	k.sym = CString(pRcd->szStock);
	k.trade = pRcd->dblPrice;
	k.size = (int)pRcd->dwShares;
	k.ex = CString(pRcd->chSource);
	k.time = GT2TL(pRcd->gtime);
	k.date = date;
	
#ifdef VERB
	// often trades arrive in the wrong order
	int myTime = GT2TLLONG(pRcd->gtime);
	if(myTime < prevTradeTime)
	{
		char str[1000];
		sprintf(str, "BAD TIMES Trade Time: %d, Prev Trade Time: %d", myTime, prevTradeTime);
		tl->D( str );
	}
	else
		prevTradeTime = myTime;
#endif

	tl->SrvGotTick(k);
	
	return GTStock::OnGotQuotePrint(pRcd);
}

int StkWrap::OnExecMsgSending(const GTSending &pRcd)
{
	return GTStock::OnExecMsgSending(pRcd);
}

int StkWrap::OnSendingOrder(const GTSending &pRcd)
{
	// ensure we have the order id
	long id = pRcd.dwUserData;
	int index = tl->GetIDIndex(id,ID);
	if (index==NO_ID) return GTStock::OnSendingOrder(pRcd);
	// save the sequence for the id
	tl->orderseq[index] =  pRcd.dwTraderSeqNo;
	return GTStock::OnSendingOrder(pRcd);
}

CString tifstring(int TIF)
{
	switch (TIF)
	{
		case TIF_DAY : return CString("DAY"); break;
		case TIF_MGTC : return CString("GTC"); break;
		case TIF_IOC : return CString("IOC"); break;
	}
	return CString("DAY");
}

int StkWrap::OnBestAskPriceChanged()
{
	int rc = GTStock::OnBestAskPriceChanged();
	return rc;
}

int StkWrap::OnBestBidPriceChanged()
{
	int rc = GTStock::OnBestBidPriceChanged();
	return rc;
}

void StkWrap::SendData()
{
	TLTick k;

	long pdwAskShares[SENDDEPTH] = {0};
	double pdblAskPrices[SENDDEPTH] = {0};
	long pdwBidShares[SENDDEPTH] = {0};
	double pdblBidPrices[SENDDEPTH] = {0};

	int askLevels = m_level2.m_ask.GetLevel(pdwAskShares, pdblAskPrices, SENDDEPTH);
	int bidLevels = m_level2.m_bid.GetLevel(pdwBidShares, pdblBidPrices, SENDDEPTH);

	//for(int i = 0; i < MAXDEPTH; i++)
	//{
	//	//char str[1000];
	//	//sprintf(str, "%d: %d@%f", i, pdwBidShares[i], pdblBidPrices[i]);
	//	//tl->D( str );
	//}

	for(int j = 0; j < SENDDEPTH; j++)
	{
		// note, senddepth is for aggregated number of levels, while here we iterate on orders
		// i.e. maxTime will be an estimation

		GTLevel2* item = m_level2.GetBidItem(j);
		if(item != NULL)
		{
			maxTime = max(maxTime, GT2TL(item->gtime));
		}
		//if(item != NULL && item->nOwnShares > 0)
		//{
		//	char str[1000];
		//	sprintf(str, "bid[%d] owned: %d", i, item->nOwnShares);
		//	tl->D( str );
		//}
		item = m_level2.GetAskItem(j);
		if(item != NULL)
		{
			maxTime = max(maxTime, GT2TL(item->gtime));
		}
	}

	bool gotSome = false; //whether change on the current level
	bool wasChange = false; //need to send depth 0, if there was a deeper change

	for(int i = SENDDEPTH-1; i >=0 ; i--)
	{
		gotSome = false;

		//GTLevel2* askItem = m_level2.GetAskItem(i);
		if( prevBids[i] != pdblBidPrices[i] || prevBidShares[i] != pdwBidShares[i] ||
		    prevAsks[i] != pdblAskPrices[i] || prevAskShares[i] != pdwAskShares[i] ) // send both, if send anything
		{
			gotSome = true;
			wasChange = true;
		}

		if( (wasChange && i == 0) || gotSome )
		{
			//if(askItem->nOwnShares > 0)
			//{
			//	char str[1000];
			//	sprintf(str, "ask[%d] owned: %d", i, askItem->nOwnShares);
			//	tl->D( str );
			//}
			k.ask = pdblAskPrices[i];//askItem->dblPrice;
			k.os = pdwAskShares[i];//askItem->dwShares;
#ifdef VERB
			if(i==0 && prevAsks[i] != k.ask)
			{
				char str[1000];
				sprintf(str, "ask[0]: %f", k.ask);
				tl->D( str );
			}
#endif
			// no exchange info because of aggregation (same problem with plain bid/ask)
			k.oe = "";//CString(m_level1.locAskExchangeCode);
			prevAsks[i] = pdblAskPrices[i];
			prevAskShares[i] = pdwAskShares[i];
		}

		//GTLevel2* bidItem = m_level2.GetBidItem(i);
		if( (wasChange && i == 0) || gotSome )
		{
			//if(bidItem->nOwnShares > 0)
			//{
			//	char str[1000];
			//	sprintf(str, "bid[%d] owned: %d", i, bidItem->nOwnShares);
			//	tl->D( str );
			//}
			k.bid = pdblBidPrices[i];//bidItem->dblPrice;
			k.bs = pdwBidShares[i];//bidItem->dwShares;
#ifdef VERB
			if(i==0 && prevBids[i] != k.bid)
			{
				char str[1000];
				sprintf(str, "bid[0]: %f", k.bid);
				tl->D( str );
			}
#endif

			k.be = "";//CString(m_level1.locBidExchangeCode);
			gotSome = true;
			prevBids[i] = pdblBidPrices[i];
			prevBidShares[i] = pdwBidShares[i];
		}


		if(gotSome)
		{
			k.sym = CString(m_szStock);
			k.symid = _symid;
			k.date = date;
			k.depth = i;
			k.time = maxTime;
			tl->SrvGotTick(k);
		}
	}

}




int StkWrap::OnExecMsgPending(const GTPending &pRcd)
{
	// ensure we have the sequence
	long seq = pRcd.dwTraderSeqNo;
	int index = tl->GetIDIndex(seq,SEQ);
	if (index==NO_ID) return GTStock::OnExecMsgPending(pRcd);
	// save the ticket
	tl->orderticket[index] = pRcd.dwTicketNo;
	// build the order
	TLOrder o;
	o.symbol = CString(pRcd.szStock);
	o.side = pRcd.chPendSide == 'B';
	o.size = pRcd.nEntryShares;
	o.TIF = tifstring(pRcd.nEntryTIF);
	o.time = pRcd.nPendTime;
	o.date = pRcd.nEntryDate;
	o.price = pRcd.dblEntryPrice;
	o.stop = pRcd.dblPendStopLimitPrice;
	o.id = tl->orderids[index];
	o.account = CString(pRcd.szAccountID);
	o.exchange = CAST_MMID_TEXT(pRcd.place);
	// send it
	tl->SrvGotOrder(o);

	return GTStock::OnExecMsgPending(pRcd);
}

int StkWrap::OnExecMsgTrade(const GTTrade &pRcd)
{
	// make sure we have the order for this trade
	long ticket = pRcd.dwTicketNo;
	int index = tl->GetIDIndex(ticket,TICKET);
	// if we don't, we're done
	if (index==NO_ID) return GTStock::OnExecMsgTrade(pRcd);
	// build trade object
	TLTrade t;
	t.symbol = CString(pRcd.szStock);
	t.account = CString(pRcd.szAccountID);
	t.id = tl->orderids[index];
	t.xdate = pRcd.nExecDate;
	t.xtime = pRcd.nExecTime;
	t.xprice = pRcd.dblExecPrice;
	t.xsize = pRcd.nExecShares;
	t.side = pRcd.chExecSide == 'B';
	t.exchange = CAST_MMID_TEXT(pRcd.execfirm);
	// send it
	tl->SrvGotFill(t);

	return GTStock::OnExecMsgTrade(pRcd);
}

int StkWrap::OnExecMsgOpenPosition(const GTOpenPosition &open)
{
	return GTStock::OnExecMsgOpenPosition(open);
}

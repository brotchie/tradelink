
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


#ifdef UDP_QUOTE
	m_level2.m_bid.m_nMaxLevels = 10;
	m_level2.m_ask.m_nMaxLevels = 10;

	m_prints.SetMinMax(128, 256);
#endif
}

inline int GT2TL(GTime32 gtime)
{
	return gtime.chSec+gtime.chMin*100+gtime.chHour*10000;
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
	uint id = tl->orderids[index];
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
	int id = tl->orderids[idx];
	tl->SrvGotCancel(id);


	return GTStock::OnExecMsgCancel(cancel);
}

int StkWrap::OnGotQuoteLevel1(GTLevel1 *pRcd)
{
#ifdef UDP_QUOTE
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
	return GTStock::OnGotLevel2Record(pRcd);
#else
	CString strStock;

	int rc = GTStock::OnGotLevel2Record(pRcd);
	// make sure depth is enabled
	if (tl->_depth==0) return rc;
	// make sure quote is good
	if (pRcd->szStock!=this->GetSymbolName()) return rc;
	// make sure depth is requested
	int depth = (int)(pRcd->dblLevelPrice * m_session.m_setting.m_nLevelRate);
	if (depth>tl->_depth) return rc;

	char chSide = pRcd->chSide;


	TLTick k;
	k.sym = CString(pRcd->szStock);
	k.symid = _symid;
	k.depth = depth;
	k.time = GT2TL(pRcd->gtime);
	k.date = date;
	
	if (pRcd->chSide=='B')
	{
		k.bid = pRcd->dblPrice;
		k.bs = pRcd->dwShares / SHAREUNITS;
		k.be = CAST_MMID_TEXT(pRcd->mmid);
	}
	else
	{
		k.ask = pRcd->dblPrice;
		k.os = pRcd->dwShares / SHAREUNITS;
		k.oe = CAST_MMID_TEXT(pRcd->mmid);

	}
	tl->SrvGotTick(k);

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
	TLTick k;
	k.sym = CString(m_szStock);
	k.symid = _symid;
	k.ask = m_level1.dblAskPrice;
	k.os = m_level1.nAskSize;
	k.oe = CString(m_level1.locAskExchangeCode);
	k.date = date;
	k.time = GT2TL(m_level1.gtime);

	tl->SrvGotTick(k);
	return OK;
}

int StkWrap::OnBestBidPriceChanged()
{
	TLTick k;
	k.sym = CString(m_szStock);
	k.symid = _symid;
	k.bid = m_level1.dblBidPrice;
	k.bs = m_level1.nBidSize;
	k.be = CString(m_level1.locBidExchangeCode);
	k.date = date;
	k.time = GT2TL(m_level1.gtime);

	tl->SrvGotTick(k);

	return OK;


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

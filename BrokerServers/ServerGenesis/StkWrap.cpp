/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// MyStock.cpp: implementation of the StkWrap class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "testapi.h"
#include "TestAPIDlg.h"
#include "StkWrap.h"


#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

StkWrap::StkWrap(GTSession &session, LPCSTR pszStock)
	: GTStock(session, pszStock)
{
	//m_pDlg = NULL;

#ifdef UDP_QUOTE
	m_level2.m_bid.m_nMaxLevels = 10;
	m_level2.m_ask.m_nMaxLevels = 10;

	m_prints.SetMinMax(128, 256);
#endif
}

StkWrap::~StkWrap()
{

}

int StkWrap::OnGotQuoteLevel1(GTLevel1 *pRcd)
{
#ifdef UDP_QUOTE
	return GTStock::OnGotQuoteLevel1(pRcd);
#else
	CString strStock;

	if(strStock != pRcd->szStock)
		return GTStock::OnGotQuoteLevel1(pRcd);

	//TRACE("%d %d %d %d %d %f %d %c %c %c %c %c\n", pRcd->locBidSize, pRcd->locAskSize, pRcd->nAskSize, pRcd->nBidSize, pRcd->nLastSize, pRcd->locAskPrice, pRcd->chSaleCondition, pRcd->chSaleCondition,
	//	pRcd->bboAskExchangeCode[0], pRcd->bboBidExchangeCode[0], pRcd->locAskExchangeCode[0], pRcd->locBidExchangeCode[0]
	//	);
	return GTStock::OnGotQuoteLevel1(pRcd);
#endif	
}

int StkWrap::OnGotLevel2Record(GTLevel2 *pRcd)
{
#ifdef UDP_QUOTE
	return GTStock::OnGotLevel2Record(pRcd);
#else
	CString strStock;
	//m_pDlg->GetDlgItemText(IDC_STOCK, strStock);

	if(strStock != pRcd->szStock)
		return GTStock::OnGotLevel2Record(pRcd);

	char chSide = pRcd->chSide;

	int rc = GTStock::OnGotLevel2Record(pRcd);
	//if(m_level2.GetBestBidPrice()>m_level2.GetBestAskPrice())
	//	TRACE("BID %lf, ASK %lf\n", m_level2.GetBestBidPrice(), m_level2.GetBestAskPrice()); 

	if(chSide == 'B')
		DisplayBidLevel();
	else
		DisplayAskLevel();

	return rc;
#endif
}

int StkWrap::DisplayBidLevel()
{
	long dwShares[2];
	double dblPrices[2];
	int nLevels = m_level2.GetBidLevel(dwShares, dblPrices, 2);

	int nShareIDs[] = {IDC_BID_SHARES1, IDC_BID_SHARES2};
	int nPriceIDs[] = {IDC_BID_PRICE1, IDC_BID_PRICE2};

	int i;
	for(i = 0; i < nLevels; ++i)
	{
		char ss[256];
		sprintf(ss, "%.2lf", dblPrices[i]);
		//m_pDlg->SetDlgItemText(nPriceIDs[i], ss);
		//m_pDlg->SetDlgItemInt(nShareIDs[i], dwShares[i]);
	}

	//for(; i < nLevels; ++i)
	//{
		//m_pDlg->SetDlgItemText(nPriceIDs[i], "");
		//m_pDlg->SetDlgItemInt(nShareIDs[i], 0);
	//}

	if(nLevels >= 1 && dblPrices[0] > 31.3)
	{
//		TRACE("Begin %.4lf\n", dblPrices[0]);
		for(int i = 0; i < 20; ++i)
		{
			GTLevel2 *pRcd = m_level2.GetBidItem(i);
			if(pRcd == NULL)
				break;

//			TRACE("%d\t%.4s\t%.4lf\n", pRcd->dwShares, &pRcd->mmid, pRcd->dblPrice);
		}
	}

	return 0;
}

int StkWrap::DisplayAskLevel()
{
	long dwShares[2];
	double dblPrices[2];
	int nLevels = m_level2.GetAskLevel(dwShares, dblPrices, 2);

	int nShareIDs[] = {IDC_ASK_SHARES1, IDC_ASK_SHARES2};
	int nPriceIDs[] = {IDC_ASK_PRICE1, IDC_ASK_PRICE2};

	int i;
	for(i = 0; i < nLevels; ++i)
	{
		char ss[256];
		sprintf(ss, "%.2lf", dblPrices[i]);
		//m_pDlg->SetDlgItemText(nPriceIDs[i], ss);
		//m_pDlg->SetDlgItemInt(nShareIDs[i], dwShares[i]);
	}

	//for(; i < nLevels; ++i)
	//{
	//	m_pDlg->SetDlgItemText(nPriceIDs[i], "");
	//	m_pDlg->SetDlgItemInt(nShareIDs[i], 0);
	//}

	return 0;
}

int StkWrap::OnTick()
{
	GTStock::OnTick();

	//TRACE("StkWrap::OnTick()\n");

	return 0;
}

int StkWrap::OnGotQuotePrint(GTPrint *pRcd)
{
	//TRACE("%s Ex=%.1s SC=%.1s Src=%.1s MMID=%d %d\n", pRcd->szStock, &pRcd->chExchangeCode, &pRcd->chSaleCondition, &pRcd->chSource, pRcd->mmidContraBrokerCode, sizeof(*pRcd));
	
	return GTStock::OnGotQuotePrint(pRcd);
}

int StkWrap::OnExecMsgSending(const GTSending &pRcd)
{
	char ss[256];

	sprintf(ss, "Sending %s %.1s %.4lf %5d", pRcd.szStock, &pRcd.chEntrySide, pRcd.dblEntryPrice, pRcd.nEntryShares);
	//m_pDlg->m_list.InsertString(0, ss);

//	int count = m_pDlg->m_list.GetCount();
	//if(count >= 1000)
	//{
	//	for(int i = count - 1; i >= 500; --i)
	//		;
			//m_pDlg->m_list.DeleteString(i);
	//}

	return GTStock::OnExecMsgSending(pRcd);
}

int StkWrap::OnExecMsgPending(const GTPending &pRcd)
{
	GTPending32 gPend = pRcd;
	char ss[256];

	sprintf(ss, "Pending %s %.1s %.4lf %5d", pRcd.szStock, &pRcd.chPendSide, pRcd.dblPendPrice, pRcd.nPendShares);

	//m_pDlg->m_list.InsertString(0, ss);

	//int count = m_pDlg->m_list.GetCount();
	//if(count >= 1000)
	//{
	//	for(int i = count - 1; i >= 500; --i)
	//		;
	//		m_pDlg->m_list.DeleteString(i);
	//}

	return GTStock::OnExecMsgPending(pRcd);
}

int StkWrap::OnExecMsgTrade(const GTTrade &pRcd)
{
	char ss[256];

	//TRACE("Trade %s %.1s %.4lf %5d\n", pRcd.szStock, &pRcd.chExecSide, pRcd.dblExecPrice, pRcd.nExecShares);
	char side;
	int share;
	double price;
	GetOpenPosition(side, share, price);
	TRACE("TradeOpenBefore %c %.4lf %5d\n", side, price, share);

	sprintf(ss, "Trade %s %.1s %.4lf %5d\n", pRcd.szStock, &pRcd.chExecSide, pRcd.dblExecPrice, pRcd.nExecShares);
	//m_pDlg->m_list.InsertString(0, ss);

	//int count = m_pDlg->m_list.GetCount();
	//if(count >= 1000)
	//{
	//	for(int i = count - 1; i >= 500; --i)
	//		m_pDlg->m_list.DeleteString(i);
	//}

	GTStock::OnExecMsgTrade(pRcd);
	GetOpenPosition(side, share, price);
	TRACE("TradeOpenAfter %c %.4lf %5d\n", side, price, share);
	return 0;
}

int StkWrap::OnExecMsgOpenPosition(const GTOpenPosition &open)
{
	TRACE("OPEN: %s %s %s %s %d %c\n", open.szAccountID, open.szAccountCode, open.szReconcileID, open.szStock, open.nOpenShares, open.chOpenSide);
	return GTStock::OnExecMsgOpenPosition(open);
}

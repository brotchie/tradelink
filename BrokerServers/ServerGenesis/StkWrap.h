/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// MyStock.h: interface for the CMyStock class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MYSTOCK_H__66F52ECC_BD30_4449_B973_55521242E873__INCLUDED_)
#define AFX_MYSTOCK_H__66F52ECC_BD30_4449_B973_55521242E873__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\inc\GTStock.h"
#include "GTPending.h"
class StkWrap : public GTStock  
{
public:
	//CTestAPIDlg *m_pDlg;

public:
	StkWrap(GTSession &session, LPCSTR pszStock);
	virtual ~StkWrap();

public:
	virtual int DisplayBidLevel();
	virtual int DisplayAskLevel();

protected:
public:
	virtual int OnTick();

protected:
	virtual int OnGotQuoteLevel1(GTLevel1 *pRcd);
	virtual int OnGotLevel2Record(GTLevel2 *pRcd);
	virtual int OnGotQuotePrint(GTPrint *pRcd);
	virtual int OnExecMsgSending(const GTSending &sending);
	virtual int OnExecMsgPending(const GTPending &pending);
	virtual int OnExecMsgTrade(const GTTrade &trade);
	virtual int OnExecMsgOpenPosition(const GTOpenPosition &open);
};

#endif // !defined(AFX_MYSTOCK_H__66F52ECC_BD30_4449_B973_55521242E873__INCLUDED_)

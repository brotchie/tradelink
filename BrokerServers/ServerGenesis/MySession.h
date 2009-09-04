/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

#if !defined(AFX_MySession_H__94295FEA_9865_4192_A8CF_A6F96779C4C7__INCLUDED_)
#define AFX_MySession_H__94295FEA_9865_4192_A8CF_A6F96779C4C7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// MySession.h : header file
//

#include "GTSession.h"

class CTestAPIDlg;

/////////////////////////////////////////////////////////////////////////////
// CMySession window

class CMySession : public GTSession
{
public:
	CString m_strAccountID;

// Construction
public:
	CMySession();

// Attributes
public:
	CTestAPIDlg *m_pDlg;

// Operations
public:
	virtual GTStock *OnCreateStock(LPCSTR pszStock);
	virtual void OnDeleteStock(GTStock *pStock);

	virtual int OnTick();

	virtual int OnExecConnected();
	virtual int OnExecDisconnected();
	virtual int OnExecMsgErrMsg(const GTErrMsg &errmsg);
	virtual int OnExecMsgLoggedin();
	virtual int OnExecMsgChat(const GTChat &chat);

	virtual int OnGotLevel2Connected();
	virtual int OnGotLevel2Disconnected();

	virtual int OnGotQuoteConnected();
	virtual int OnGotQuoteDisconnected();

	virtual int OnGotChartConnected();
	virtual int OnGotChartDisconnected();

	virtual void OnUDPQuoteMessage(int nCode, unsigned short nPort, const char *pszMsg);

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMySession)
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CMySession();

	// Generated message map functions
protected:
	//{{AFX_MSG(CMySession)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MySession_H__94295FEA_9865_4192_A8CF_A6F96779C4C7__INCLUDED_)

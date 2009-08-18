/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// MySession.cpp : implementation file
//

#include "stdafx.h"
#include "testapi.h"
#include "MySession.h"
#include "MyStock.h"
#include "TestAPIDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CMySession

CMySession::CMySession()
{
	m_pDlg = NULL;
}

CMySession::~CMySession()
{
}


BEGIN_MESSAGE_MAP(CMySession, GTSession)
	//{{AFX_MSG_MAP(CMySession)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CMySession message handlers

GTStock *CMySession::OnCreateStock(LPCSTR pszStock)
{
	CMyStock *pStock = new CMyStock(*this, pszStock);
	if(pStock == NULL)
		return NULL;

	pStock->m_pDlg = this->m_pDlg;
	return pStock;
}

void CMySession::OnDeleteStock(GTStock *pStock)
{
	GTSession::OnDeleteStock(pStock);
}

int CMySession::OnExecConnected()
{
	m_pDlg->GetDlgItem(IDC_START)->EnableWindow(FALSE);

	m_pDlg->m_list.InsertString(0, "Exec Connected");

	return GTSession::OnExecConnected();
}

int CMySession::OnExecDisconnected()
{
	m_pDlg->GetDlgItem(IDC_START)->EnableWindow(TRUE);

	m_pDlg->m_list.InsertString(0, "Exec Disconnected");

	return GTSession::OnExecDisconnected();
}

int CMySession::OnExecMsgLoggedin()
{
	m_pDlg->m_list.InsertString(0, "Logged in");

	return GTSession::OnExecMsgLoggedin();
}

int CMySession::OnExecMsgErrMsg(const GTErrMsg &errmsg)
{
	char msg[1024];
	LPCSTR pMsg = GetErrorMessage(errmsg.nErrCode, msg);

	m_pDlg->m_list.InsertString(0, pMsg);

	return GTSession::OnExecMsgErrMsg(errmsg);
}

int CMySession::OnExecMsgChat(const GTChat &chat)
{
	char msg[1024];
	sprintf(msg, "%s -> %s: %s", chat.szUserFm, chat.szUserTo, chat.szText);
	m_pDlg->m_list.InsertString(0, msg);

	return GTSession::OnExecMsgChat(chat);
}

int CMySession::OnGotLevel2Connected()
{
	m_pDlg->m_list.InsertString(0, "Level 2 Connected");
	
	return GTSession::OnGotLevel2Connected();
}

int CMySession::OnGotLevel2Disconnected()
{
	m_pDlg->m_list.InsertString(0, "Level 2 Disconnected");

	return GTSession::OnGotLevel2Disconnected();
}

int CMySession::OnGotQuoteConnected()
{
	m_pDlg->m_list.InsertString(0, "Quote Connected");

	return GTSession::OnGotQuoteConnected();
}

int CMySession::OnGotQuoteDisconnected()
{
	m_pDlg->m_list.InsertString(0, "Quote Disconnected");

	return GTSession::OnGotQuoteDisconnected();
}

////
int CMySession::OnGotChartConnected()
{
	m_pDlg->m_list.InsertString(0, "Chart Connected");

	return GTSession::OnGotChartConnected();
}

int CMySession::OnGotChartDisconnected()
{
	m_pDlg->m_list.InsertString(0, "Chart Disconnected");

	return GTSession::OnGotQuoteDisconnected();
}

int CMySession::OnTick()
{
	GTSession::OnTick();

	static int count = 0;
	
	if(count++ % 60 == 0)
	{
		// To Do here
	}

	m_pDlg->SetDlgItemInt(IDC_LEVEL2S, this->m_stocks.GetCount());

	return 0;
}

void CMySession::OnUDPQuoteMessage(int nCode, unsigned short nPort, const char *pszMsg)
{
	char ss[1024];
	_snprintf(ss, sizeof(ss), "%s, code=%d, port=%u", pszMsg, nCode, nPort);
	m_pDlg->m_list.InsertString(0, ss);

	GTSession::OnUDPQuoteMessage(nCode, nPort, pszMsg);
}

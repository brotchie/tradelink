#include "stdafx.h"
#include "GTWrap.h"

GTWrap::GTWrap()
{
	DeleteAllStocks();
	m_setting.m_hidden[MMID_NYB]=1;
}

GTWrap::~GTWrap()
{
	_sg = NULL;
}


BEGIN_MESSAGE_MAP(GTWrap, GTSession)
	//{{AFX_MSG_MAP(GTSess)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// GTSess message handlers

GTStock *GTWrap::OnCreateStock(LPCSTR pszStock)
{
	// get id for symbol
	int symid = _sg->FindSym(CString(pszStock));
	// make sure it's registered
	if (symid<0) 
		return GTSession::OnCreateStock(pszStock);
	StkWrap* stk = new StkWrap(*this,pszStock,symid);
	stk->tl = _sg;
	return stk;
}

void GTWrap::OnDeleteStock(GTStock *pStock)
{
	GTSession::OnDeleteStock(pStock);
}

void GTWrap::D(CString msg)
{
	if (_sg==NULL) return;
	_sg->D(msg);
}

int GTWrap::OnExecConnected()
{

	D("Exec Connected");

	return GTSession::OnExecConnected();
}

int GTWrap::OnExecDisconnected()
{
	D("Exec Disconnected");

	return GTSession::OnExecDisconnected();
}

int GTWrap::OnExecMsgLoggedin()
{
	D("Logged in");
	_sg->accounttest();
	return GTSession::OnExecMsgLoggedin();
}

int GTWrap::OnExecMsgErrMsg(const GTErrMsg &errmsg)
{
	char msg[1024];
	LPCSTR pMsg = GetErrorMessage(errmsg.nErrCode, msg);
	D(pMsg);

	return GTSession::OnExecMsgErrMsg(errmsg);
}

int GTWrap::OnExecMsgChat(const GTChat &chat)
{
	char msg[1024];
	sprintf(msg, "%s -> %s: %s", chat.szUserFm, chat.szUserTo, chat.szText);
	D(msg);

	return GTSession::OnExecMsgChat(chat);
}

int GTWrap::OnGotLevel2Connected()
{
	D("Level 2 Connected");
	
	return GTSession::OnGotLevel2Connected();
}

int GTWrap::OnGotLevel2Disconnected()
{
	D("Level 2 Disconnected");

	return GTSession::OnGotLevel2Disconnected();
}

int GTWrap::OnGotQuoteConnected()
{
	D("Quote Connected");

	return GTSession::OnGotQuoteConnected();
}

int GTWrap::OnGotQuoteDisconnected()
{
	D("Quote Disconnected");

	return GTSession::OnGotQuoteDisconnected();
}

////
int GTWrap::OnGotChartConnected()
{
	D("Chart Connected");

	return GTSession::OnGotChartConnected();
}

int GTWrap::OnGotChartDisconnected()
{
	D("Chart Disconnected");

	return GTSession::OnGotQuoteDisconnected();
}

int GTWrap::OnTick()
{
	GTSession::OnTick();

	static int count = 0;
	
	if(count++ % 60 == 0)
	{
		// To Do here
	}

	//m_pDlg->SetDlgItemInt(IDC_LEVEL2S, this->m_stocks.GetCount());

	return 0;
}

void GTWrap::OnUDPQuoteMessage(int nCode, unsigned short nPort, const char *pszMsg)
{
	char ss[1024];
	_snprintf(ss, sizeof(ss), "%s, code=%d, port=%u", pszMsg, nCode, nPort);
	D(ss);

	GTSession::OnUDPQuoteMessage(nCode, nPort, pszMsg);
}

#pragma once
#include "GTSession.h"
#include "ServerGenesis.h"

class ServerGenesis;

class GTWrap : 
	public GTSession
{

// Construction
public:
	GTWrap(void);
	~GTWrap();


// Attributes
public:
	ServerGenesis* _sg;	
protected:
	CString m_strAccountID;

// Operations
public:
	void D(CString msg);
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
	//{{AFX_VIRTUAL(GTSess)
	//}}AFX_VIRTUAL

	// Generated message map functions
protected:
	//{{AFX_MSG(GTSess)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.


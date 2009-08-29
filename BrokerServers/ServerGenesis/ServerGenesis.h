#pragma once
#include "TradeLibFast.h"
#include "GTSession.h"
#include <vector>


class ServerGenesis : 
	public GTSession, 
	public TradeLibFast::TLServer_WM
{
public :
	ServerGenesis();
	~ServerGenesis();

	//CTestAPIDlg *m_pDlg;

	// tradelink overrides
	void Start();
	int SendOrder(TradeLibFast::TLOrder order);
	int CancelRequest(long id);
	int BrokerName();
	std::vector<int> GetFeatures();

	// genesis overrides
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

private:
		CString m_strAccountID;
		std::vector<GTOrder> m_order;


};


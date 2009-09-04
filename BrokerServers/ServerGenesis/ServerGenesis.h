#pragma once
#include "TradeLibFast.h"
#include "GTOrder.h"
#include <vector>

class GTWrap;

class ServerGenesis : 
	public TradeLibFast::TLServer_WM
{
public :
	ServerGenesis();
	virtual ~ServerGenesis();

	// tradelink overrides
	void Start(void);
	void Start(LPCSTR user, LPCSTR pw);
	void Stop();
	int SendOrder(TradeLibFast::TLOrder order);
	int CancelRequest(long id);
	int BrokerName();
	std::vector<int> GetFeatures();

private:
		CString m_strAccountID;
		std::vector<GTOrder> m_order;
		// genesis overrides
		GTWrap * gtw;

	// Generated message map functions
protected:
	//{{AFX_MSG(ServerGenesis)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG
	//DECLARE_MESSAGE_MAP()



};


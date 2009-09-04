#pragma once
#include "TradeLibFast.h"
#include "GTOrder.h"
#include "GTWrap.h"
#include <vector>

class GTWrap;

class ServerGenesis :
	public TradeLibFast::TLServer_WM
{
public :
	ServerGenesis();
	~ServerGenesis();

	// tradelink overrides
	void Start(void);
	void Start(LPCSTR user, LPCSTR pw);
	void Stop();
	int SendOrder(TradeLibFast::TLOrder order);
	int CancelRequest(long id);
	int BrokerName();
	std::vector<int> GetFeatures();
	void accounttest();
	GTWrap* gtw;
	bool LoadConfig();
	bool Autologin();

private:
	bool autoattempt;
		std::vector<CString> m_accts;
		std::vector<GTOrder> m_order;
		CString un;
		CString pw;
		// genesis overrides


	// Generated message map functions
protected:
	//{{AFX_MSG(ServerGenesis)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG
	//DECLARE_MESSAGE_MAP()
};
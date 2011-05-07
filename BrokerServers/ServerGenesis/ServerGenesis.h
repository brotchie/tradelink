#pragma once
#include "TradeLibFast.h"
#include "GTOrder.h"
#include "GTWrap.h"
#include <vector>
#include "StkWrap.h"

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
	int DOMRequest(int depth);
	int SendOrder(TradeLibFast::TLOrder order);
	int CancelRequest(int64 id);
	int BrokerName();
	int AccountResponse(CString clientname);
	int RegisterStocks(CString client);
	int PositionResponse(CString account, CString client);
	std::vector<int> GetFeatures();
	void accounttest();
	GTWrap* gtw;
	bool LoadConfig();
	bool Autologin();
	bool IdIsUnique(int64 id);
	int _depth;
	std::vector<int64> orderids;
	std::vector<long> orderseq;
	std::vector<long> orderticket;
	std::vector<DWORD> gorderids;
	int GetIDIndex(int64 id, int type);

private:

	void SendPosition(int client, GTOpenPosition& p);
	bool autoattempt;
	bool subscribed(CString sym);
	std::vector<StkWrap*> m_stk;
		std::vector<CString> m_accts;
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

enum ORDERIDTYPE
{
	NO_ID = -1,
	ID,
	SEQ,
	TICKET,
	GENESIS,
};
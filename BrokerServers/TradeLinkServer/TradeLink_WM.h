#pragma once
#include "TLOrder.h"
#include "TLTrade.h"
#include "TLTick.h"
#include "TLSecurity.h"
#include <vector>
using namespace std;

namespace TradeLinkServer
{


	const CString LIVEWINDOW = _T("TL-BROKER-LIVE");
	const CString SIMWINDOW = _T("TL-BROKER-SIMU");

	// TradeLink_WM

	[event_source(native)]
	class AFX_EXT_CLASS  TradeLink_WM : public CWnd
	{
		DECLARE_DYNAMIC(TradeLink_WM)

	public:
		TradeLink_WM(void);
		~TradeLink_WM(void);
		bool TLDEBUG;
		bool ENABLED;
		__event void GotDebug(LPCTSTR msg);
		CString debugbuffer;

	protected:
		bool needStock(CString stock);
		int FindClient(CString clientname);

		vector <CString>client; // map client ids to name
		vector <time_t>heart; // map last contact to id
		typedef vector <CString> clientstocklist; // hold a single clients stocks
		vector < clientstocklist > stocks; // map stocklist to id
		int FindClientFromStock(CString stock);
		DECLARE_MESSAGE_MAP()

	public:
		afx_msg BOOL OnCopyData(CWnd* pWnd, COPYDATASTRUCT* pCopyDataStruct);
		int RegisterClient(CString  clientname);
		int ServiceMessage(int MessageType, CString msg);
		int HeartBeat(CString clientname);
		virtual int BrokerName(void);
		virtual int SendOrder(TLOrder order);
		virtual int AccountResponse(CString clientname);
		virtual int RegisterStocks(CString clientname);
		virtual int RegisterFutures(CString clientname);
		virtual std::vector<int> GetFeatures();
		int ClearClient(CString client);
		int ClearStocks(CString client);

		void TradeLink_WM::D(const CString & message);
		void SrvGotOrder(TLOrder order);
		void SrvGotFill(TLTrade trade);
		void SrvGotTick(TLTick tick);
		bool HaveSubscriber(CString stock);
		void SrvCancelNotify(int orderid);
		virtual void SrvCancelRequest(long order);
		virtual void Start(bool live = true);
	};


}

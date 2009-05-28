#pragma once
#include "TLSecurity.h"
#include "TLMarketBasket.h"
#include "TLOrder.h"
#include "TLTrade.h"
#include "TLTick.h"
#include "TradeLink.h"
#include "Util.h"
#include <vector>

namespace TradeLibFast
{
	class AFX_EXT_CLASS TLClient_WM : public CWnd
	{
	public:
		TLClient_WM(char* client = "tlclient");
		~TLClient_WM(void);
		long TLSend(int type,LPCTSTR msg);
		static long TLSend(int type,LPCTSTR msg, HWND dest);
		static long TLSend(int type,LPCTSTR msg,CString windname);
		vector<int> TLFound();
		void Mode();
		void Mode(int ProviderId);
		void Unsubscribe();
		void Disconnect();
		void Register();
		void Subscribe(TLMarketBasket mb);
		int SendOrder(TLOrder o);
		void CancelOrder(long id);
		void RequestDOM(int depth = 4);

		// handle these events
		virtual void gotOrder(TLOrder o) {};
		virtual void gotFill(TLTrade fill) {};
		virtual void gotTick(TLTick tick) {};


	protected:
		vector<int> servers;
		vector<CString> srvrname;
		afx_msg BOOL OnCopyData(CWnd* pWnd, COPYDATASTRUCT* pCopyDataStruct);
		HWND _himh;
		CString _him;
		CString _me;
		DECLARE_MESSAGE_MAP()
	};
}

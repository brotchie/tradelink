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
		static long TLSend(int type,LPCTSTR msg,CString windname);
		int TLFound(int mask);
		void Mode(int mode);
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
		afx_msg BOOL OnCopyData(CWnd* pWnd, COPYDATASTRUCT* pCopyDataStruct);
		CString _him;
		CString _me;
		DECLARE_MESSAGE_MAP()
	};
}

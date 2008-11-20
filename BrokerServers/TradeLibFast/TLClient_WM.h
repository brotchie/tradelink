#pragma once
#include "TradeLibFast.h"


namespace TradeLibFast
{
	class AFX_EXT_CLASS  TLClient_WM : public CWnd
	{
		DECLARE_DYNAMIC(TLClient_WM)

	public:
		TLClient_WM(CString client = CString("tlclient"));
		~TLClient_WM(void);
		void Register();
		void Subscribe(TLMarketBasket mb);
		int TLFound(int mask);
		void Mode(int mode);
		void Unsubscribe();
		void Disconnect();
		int SendOrder(TLOrder o);
		afx_msg BOOL OnCopyData(CWnd* pWnd, COPYDATASTRUCT* pCopyDataStruct);

		// handle these events
		virtual void gotOrder(TLOrder o) {};
		virtual void gotFill(TLTrade fill) {};
		virtual void gotTick(TLTick tick) {};

		




	protected:
		CString _him;
		CString _me;
		int TLSend(int type,LPCTSTR msg,CString windname);
		DECLARE_MESSAGE_MAP()
	};


}

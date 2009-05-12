#include "stdafx.h"
#include "TLClient_WM.h"


namespace TradeLibFast
{
	BEGIN_MESSAGE_MAP(TLClient_WM, CWnd)
		ON_WM_COPYDATA()
	END_MESSAGE_MAP()

	BOOL TLClient_WM::OnCopyData(CWnd* pWnd, COPYDATASTRUCT* pCopyDataStruct)
	{
		CString msg = (LPCTSTR)(pCopyDataStruct->lpData);
		int type = (int)pCopyDataStruct->dwData;

		// here is where we switch based upon message type,
		// create whatever objects we need from the message body (msg)
		// and pass them along
		switch (type)
		{
		case ORDERNOTIFY :
			{
				TLOrder o = TLOrder::Deserialize(msg);
				gotOrder(o);
			}
			break;
		case EXECUTENOTIFY :
			{
				TLTrade f = TLTrade::Deserialize(msg);
				gotFill(f);
			}
			break;
		case TICKNOTIFY :
			{
				TLTick t = TLTick::Deserialize(msg);
				gotTick(t);
			}
			break;
		}
		return true;
	}

	int TLClient_WM::SendOrder(TLOrder o)
	{
		return TLSend(SENDORDER,o.Serialize(),_him);
	}

	void TLClient_WM::CancelOrder(long id)
	{
		CString ids;
		ids.Format("%i",id);
		TLSend(ORDERCANCELREQUEST,ids,_him);
	}


	TLClient_WM::TLClient_WM(char* clientname)
	{
		// make sure window has a unique name
		int i = -1;
		CString name = clientname;
		while (true)
		{
			HWND dest = FindWindowA(NULL,(LPCSTR)(LPCTSTR)name)->GetSafeHwnd();
			if (!dest) break;
			name.Format("%s%i",clientname,i);
		}
		_me = name;
		this->Create(NULL, name, 0,CRect(0,0,20,20), CWnd::GetDesktopWindow(),NULL);
		this->ShowWindow(SW_HIDE); // hide our window
		Mode(TLFound(ANYSERVER));
	}

	TLClient_WM::~TLClient_WM()
	{
		Unsubscribe();
		Disconnect();
	}

	bool found(CString wind)
	{
		HWND dest = FindWindowA(NULL,(LPCSTR)(LPCTSTR)wind);
		if (dest) return true;
		return false;
	}

	int TLClient_WM::TLFound(int mask)
	{
		int fnd = NONE;
		if (found(::SIMWINDOW)) fnd += (int)SIMBROKER;
		if (found(::LIVEWINDOW)) fnd += (int)LIVEBROKER;
		if (found(::HISTWINDOW)) fnd += (int)HISTORICALBROKER;
		if (found(::TESTWINDOW)) fnd += (int)TESTBROKER;
		return fnd & mask;
	}

	void TLClient_WM::Mode(int mode)
	{
		if (mode==SIMBROKER)
			_him = ::SIMWINDOW;
		else if (mode== LIVEBROKER)
			_him = ::LIVEWINDOW;
		else if (mode == HISTORICALBROKER)
			_him = ::HISTWINDOW;
		else if (mode == TESTBROKER)
			_him = ::TESTWINDOW;
		else 
			_him = "TL-NOTFOUND";
		Register();
	}

	void TLClient_WM::Register()
	{
		TLSend(REGISTERCLIENT,_me,_him);
	}

	void TLClient_WM::Subscribe(TLMarketBasket mb)
	{
		CString basket = mb.Serialize();
		CString m;
		m.Format("%s+%s",_me,basket);
		TLSend(REGISTERSTOCK,m,_him);
	}

	void TLClient_WM::RequestDOM(int depth)
	{
		CString m;
		m.Format("%s+%i",_me,depth);
		TLSend(DOMREQUEST,_me,_him);
	}

	long TLClient_WM::TLSend(int type,LPCTSTR msg,CString windname) 
	{
		LRESULT result = BROKERSERVER_NOT_FOUND;
		HWND dest = FindWindowA(NULL,(LPCSTR)(LPCTSTR)windname)->GetSafeHwnd();
		
		if (dest) 
		{
			COPYDATASTRUCT CD;  // windows-provided structure for this purpose
			CD.dwData=type;		// stores type of message
			int len = 0;
			len = (int)strlen((char*)msg);

			CD.cbData = len+1;
			CD.lpData = (void*)msg;	//here's the data we're sending
			result = ::SendMessageA(dest,WM_COPYDATA,0,(LPARAM)&CD);
		} 
		return (long)result;
	}

	void TLClient_WM::Unsubscribe()
	{
		TLSend(CLEARSTOCKS,"",_him);
	}

	void TLClient_WM::Disconnect()
	{
		TLSend(CLEARCLIENT,"",_him);
	}


}
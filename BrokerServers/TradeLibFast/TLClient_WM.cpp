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
		case TICKNOTIFY :
			{
				TLTick t = TLTick::Deserialize(msg);
				gotTick(t);
			}
			break;
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
		}
		return true;
	}

	int TLClient_WM::SendOrder(TLOrder o)
	{
		return TLSend(SENDORDER,o.Serialize());
	}

	void TLClient_WM::CancelOrder(long id)
	{
		CString ids;
		ids.Format("%i",id);
		TLSend(ORDERCANCELREQUEST,ids);
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
		TLFound();
		Mode();
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

	vector<int> TLClient_WM::TLFound()
	{
		// clear initial lists
		servers.clear();
		srvrname.clear();
		// prepare legacy name search
		vector<CString> attempts;
		attempts.push_back(::SIMWINDOW);
		attempts.push_back(::LIVEWINDOW);
		attempts.push_back(::TESTWINDOW);
		attempts.push_back(::HISTWINDOW);
		attempts.push_back(::SERVERWINDOW);
		// prepare ordered name search 
		const int MAXSERVERS = 10;
		for (int i = 0; i<MAXSERVERS; i++)
		{
			CString m;
			m.Format("%s.%i",::SERVERWINDOW,i);
			attempts.push_back(m);
		}
		// see whats of our list is out there
		for (int i =0; i<(int)attempts.size(); i++)
		{
			if (found(attempts[i]))
			{
				int id = TLSend(BROKERNAME,"",attempts[i]);
				if (id!=-1)
				{
					servers.push_back(id);
					srvrname.push_back(attempts[i]);
				}
			}
		}
		return servers;
	}

	void TLClient_WM::Mode() { Mode(0); }
	void TLClient_WM::Mode(int ProviderId)
	{
		if ((ProviderId<0) || (ProviderId>=(int)servers.size()))
		{
			return;
		}
		_him = srvrname[ProviderId];
		Register();
	}

	void TLClient_WM::Register()
	{
		HWND dest = FindWindowA(NULL,(LPCSTR)(LPCTSTR)_him)->GetSafeHwnd();
		_himh = dest;
		TLSend(REGISTERCLIENT,_me,_himh);
	}

	void TLClient_WM::Subscribe(TLMarketBasket mb)
	{
		CString basket = mb.Serialize();
		CString m;
		m.Format("%s+%s",_me,basket);
		TLSend(REGISTERSTOCK,m);
	}

	void TLClient_WM::RequestDOM(int depth)
	{
		CString m;
		m.Format("%s+%i",_me,depth);
		TLSend(DOMREQUEST,_me);
	}

	long TLClient_WM::TLSend(int type,LPCTSTR msg) 
	{
		if (_himh)
			return TLSend(type,msg,_himh);
		return BROKERSERVER_NOT_FOUND;
	}
	long TLClient_WM::TLSend(int type,LPCTSTR msg,CString windname) 
	{
		HWND dest = FindWindowA(NULL,(LPCSTR)(LPCTSTR)windname)->GetSafeHwnd();
		return TLSend(type,msg,dest);
	}
	long TLClient_WM::TLSend(int type,LPCTSTR msg, HWND dest)
	{
		// set default result
		LRESULT result = BROKERSERVER_NOT_FOUND;
		
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
		TLSend(CLEARSTOCKS,"");
	}

	void TLClient_WM::Disconnect()
	{
		TLSend(CLEARCLIENT,"");
	}


}
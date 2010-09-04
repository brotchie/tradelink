#include "StdAfx.h"
#include "TLServer_IP.h"


// AsyncServerDlg.cpp : implementation file
// Rewritten to Best Practice by Joseph M. Newcomer, Mar 2007
//

#include "messages.h"
#include "port.h"
#include "Convert.h"
#include "UnicodeFont.h"
#include "ErrorString.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

namespace TradeLibFast
{

	/////////////////////////////////////////////////////////////////////////////
	// TLServer_IP dialog
	
	/****************************************************************************
	*                      TLServer_IP::TLServer_IP
	* Inputs:
	*       CWnd * parent:
	* Effect: 
	*       Constructor
	****************************************************************************/

	TLServer_IP::TLServer_IP() :
			m_open(0),
			m_close(0),
			m_running(FALSE),
			m_MainWndIsClosing(FALSE)
	{
			//{{AFX_DATA_INIT(TLServer_IP)
					// NOTE: the ClassWizard will add member initialization here
			//}}AFX_DATA_INIT
			// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
			PORT = DEFAULTPORT;
			MajorVer = 0.1;
			MinorVer = 0;
			TLDEBUG = true;
			ENABLED = false;
			debugbuffer = CString("");
			m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	}
	TLServer_IP::~TLServer_IP()
	{
		debugbuffer = "";
	}
	

	BEGIN_MESSAGE_MAP(TLServer_IP, CWnd)
			//{{AFX_MSG_MAP(TLServer_IP)
			ON_WM_SYSCOMMAND()
			ON_WM_PAINT()
			ON_WM_QUERYDRAGICON()
			ON_WM_CLOSE()
			ON_BN_CLICKED(IDC_CLOSE, OnBnClickedClose)
			ON_WM_SIZE()
			ON_BN_CLICKED(IDC_RUN, OnRun)
			ON_EN_CHANGE(IDC_PORT, OnChangePort)
			//}}AFX_MSG_MAP
			ON_MESSAGE(UWM_THREADSTART, OnThreadStart)
			ON_MESSAGE(UWM_THREADCLOSE, OnThreadClose)
			ON_MESSAGE(UWM_NETWORK_DATA, OnNetworkData)    
			ON_MESSAGE(UWM_INFO, OnInfo)
			ON_MESSAGE(UWM_NETWORK_ERROR, OnNetworkError)
			ON_MESSAGE(UWM_SEND_COMPLETE, OnSendComplete)
	END_MESSAGE_MAP()

	
	/****************************************************************************
	*                       TLServer_IP::CleanupThreads                      
	* Result: BOOL                                                               
	*       TRUE if there were no threads running                                
	*       FALSE if threads are being shut down                                 
	* Effect:                                                                    
	*       Initiates thread shutdown                                            
	****************************************************************************/

	BOOL TLServer_IP::CleanupThreads() 
	   {
		INT_PTR size = m_threadIDs.GetSize();
		if(size == 0)                      
		   return TRUE;                    

		for (INT_PTR i = 0; i < size; i++)
		   { /* scan threads */
			if (!::PostThreadMessage(m_threadIDs[i], UWM_TERM_THREAD, 0, 0))
			   { /* failed */
				TRACE(_T("%s: Thread 0x%02x possibly already terminated\n"), AfxGetApp()->m_pszAppName, m_threadIDs[i]);
				m_threadIDs.RemoveAt(i);
			   } /* failed */
		   } /* scan threads */

		// Note that if PostThreadMessage has failed and all the target threads have
		// been removed from the array, we are done

		if(m_threadIDs.GetSize() == 0)
		   return TRUE;
	    
		return FALSE;
	}

	/////////////////////////////////////////////////////////////////////////////
	// TLServer_IP message handlers
	
	/****************************************************************************
	*                       TLServer_IP::CreateListener
	* Result: BOOL
	*       TRUE if socket created
	*       FALSE if error
	* Effect: 
	*       Creates a listener socket
	****************************************************************************/

	BOOL TLServer_IP::CreateListener()
		{
		 if (!AfxSocketInit())
		 {
			 CString * s = new CString("IDP_SOCKETS_INIT_FAILED");
			PostMessage(UWM_INFO, (WPARAM)s, ::GetCurrentThreadId());
			 return FALSE;
		 }
		 // Create the listener socket
		 UINT portnum = PORT;
		 if(!m_listensoc.Create(portnum))
			{ /* failed to create */
			 DWORD err = ::GetLastError();
			 CString fmt;
			 fmt.LoadString(IDS_LISTEN_CREATE_FAILED);
			 CString * s = new CString;
			 s->Format(fmt, portnum);
			 PostMessage(UWM_INFO, (WPARAM)s, ::GetCurrentThreadId());
			 PostMessage(UWM_NETWORK_ERROR, (WPARAM)err, ::GetCurrentThreadId());
			 return FALSE;
			} /* failed to create */

		 { /* success */
		  CString fmt;
		  fmt.LoadString(IDS_LISTENER_CREATED);
		  CString * s = new CString;
		  s->Format(fmt, portnum);
		  PostMessage(UWM_INFO, (WPARAM)s,::GetCurrentThreadId());
		 } /* success */

		 m_listensoc.SetTarget(this);

		 m_listensoc.Listen();
		 return TRUE;
		} // TLServer_IP::CreateListener

	
	/****************************************************************************
	*                        TLServer_IP::OnSysCommand
	* Inputs:
	*       UINT nID: Menu ID
	*       LPARAM lParam: unused, pass to superclass
	* Result: void
	*       
	* Effect: 
	*       Processes a system menu click
	****************************************************************************/

	void TLServer_IP::OnSysCommand(UINT nID, LPARAM lParam)
	{
		CWnd::OnSysCommand(nID, lParam);
	}
	
	/****************************************************************************
	*                          TLServer_IP::OnPaint
	* Result: void
	*       
	* Effect: 
	*       If you add a minimize button to your dialog, you will need the code
	*       below to draw the icon.  For MFC applications using the document/view
	*       model, this is automatically done for you by the framework.
	****************************************************************************/

	void TLServer_IP::OnPaint() 
	{
			if (IsIconic())
			{
					CPaintDC dc(this); // device context for painting

					SendMessage(WM_ICONERASEBKGND, (WPARAM) dc.GetSafeHdc(), 0);

					// Center icon in client rectangle
					int cxIcon = GetSystemMetrics(SM_CXICON);
					int cyIcon = GetSystemMetrics(SM_CYICON);
					CRect rect;
					GetClientRect(&rect);
					int x = (rect.Width() - cxIcon + 1) / 2;
					int y = (rect.Height() - cyIcon + 1) / 2;

					// Draw the icon
					dc.DrawIcon(x, y, m_hIcon);
			}
			else
			{
					CWnd::OnPaint();
			}
	}
	
	/****************************************************************************
	*                      TLServer_IP::OnQueryDragIcon
	* Result: HCURSOR
	*       Handle of cursor to use as drag icon
	* Effect: 
	*       The system calls this to obtain the cursor to display while the user
	*       drags the minimized window.
	****************************************************************************/

	HCURSOR TLServer_IP::OnQueryDragIcon()
	{
			return (HCURSOR) m_hIcon;
	}
	
	/****************************************************************************
	*                       TLServer_IP::OnThreadStart
	* Inputs:
	*       WPARAM unused
	*       LPARAM lParam: (LPARAM)(DWORD) the thread ID
	* Result: LRESULT
	*       Logically void, 0, always
	* Effect: 
	*       Increments the open-thread count.  Adds the thread ID to the
	*       array of thread IDs
	****************************************************************************/

	LRESULT TLServer_IP::OnThreadStart(WPARAM, LPARAM lParam)
	   {
		m_open++;

		DWORD ThreadID = (DWORD)lParam;
		m_threadIDs.Add(ThreadID);  // save thread refs so we can post msg to quit later

		// Update the display
		ShowOpenConnections();
		return 0;
	   }
	
	/****************************************************************************
	*                    TLServer_IP::ShowOpenConnections
	* Result: void
	*       
	* Effect: 
	*       Shows the number of open connections in the dialog control  
	****************************************************************************/

	void TLServer_IP::ShowOpenConnections()
		{
		 CString s;
		 s.Format(_T("%d"), m_open);
		 D(s);
		} // TLServer_IP::ShowOpenConnections
	
	/****************************************************************************
	*                   TLServer_IP::ShowClosedConnections
	* Result: void
	*       
	* Effect: 
	*       Displays the count of closed connections
	****************************************************************************/

	void TLServer_IP::ShowClosedConnections()
		{
		 CString s;
		 s.Format(_T("%d"), m_close);
		 D(s);
		} // TLServer_IP::ShowClosedConnections
	
	/****************************************************************************
	*                       TLServer_IP::OnThreadClose
	* Inputs:
	*       WPARAM unused
	*       LPARAM: lParam: (LPARAM)(DWORD) thread ID
	* Result: LRESULT
	*       Logically void, 0, always
	* Effect: 
	*       Records that a thread has closed.  If a thread has closed and
	*       a close operation has been deferred, completes the close transaction
	****************************************************************************/

	LRESULT TLServer_IP::OnThreadClose(WPARAM, LPARAM lParam)
	   {
		m_open--;
		m_close++;
	        
		ShowOpenConnections();

		ShowClosedConnections();
	        
		// remove dwThreadID from m_threadIDs
		DWORD dwThreadID = (DWORD)lParam;
	        
		INT_PTR size = m_threadIDs.GetSize();
		for (INT_PTR i = 0; i < size; i++)
		   { /* scan threads */
			if (m_threadIDs[i] ==  dwThreadID)
			   {
				m_threadIDs.RemoveAt(i);
				TRACE(_T("%s: Thread 0x%02x is removed\n"), AfxGetApp()->m_pszAppName, dwThreadID);
				break;
			   }
		   } /* scan threads */

	   // If we got a WM_CLOSE and there were open threads,  
	   // we deferred the close until all the threads terminated
	   // Now we try again  
		if(m_MainWndIsClosing && m_open == 0) 
		   PostMessage(WM_CLOSE);  // try the close again
		return 0;
	   }
	
	/****************************************************************************
	*                        TLServer_IP::OnNetworkData
	* Inputs:
	*       WPARAM: (WPARAM)(CByteArray *) The data to display
	*       LPARAM: (LPARAM)(DWORD) the thread ID of the thread that sent it
	* Result: LRESULT
	*       Logically void, 0, always
	* Effect: 
	*       Displays the string received
	* Notes:
	*       Deletes the string
	****************************************************************************/

	LRESULT TLServer_IP::OnNetworkData(WPARAM wParam, LPARAM lParam)
	   {
		// a new message has been received. Update the UI
		CByteArray * data = (CByteArray *)wParam;
		
		CString s = ConvertReceivedDataToString(*data);
		D(s);

		CString msg;
	#define SERVER_STRING_INFO_LIMIT 63
		msg.Format(_T("%x: => \"%-*s\""), (DWORD)lParam, SERVER_STRING_INFO_LIMIT, s);
		D(msg);
		delete data;

		return 0;
	   }


	void TLServer_IP::SrvGotPacket(TLPacket packet)
	{
		ServiceMessage(packet.Intention,packet.Data);
	}



	int TLServer_IP::ServiceMessage(int MessageType, CString msg)
	{
		switch (MessageType)
		{
			case ORDERCANCELREQUEST :
				return CancelRequest((long)atoi(msg.GetBuffer()));
			case ACCOUNTREQUEST :
				return AccountResponse(msg);
			case CLEARCLIENT :
				return ClearClient(msg);
			case CLEARSTOCKS :
				return ClearStocks(msg);
			case REGISTERSTOCK :
				{
				vector<CString> rec;
				gsplit(msg,CString("+"),rec);
				CString client = rec[0];
				vector<CString> hisstocks;
				// make sure client sent a basket, otherwise clear the basket
				if (rec.size()!=2) return ClearStocks(client);
				// get the basket
				gsplit(rec[1],CString(","),hisstocks);
				// make sure we have the client
				unsigned int cid = FindClient(client); 
				if (cid==-1) return CLIENTNOTREGISTERED; //client not registered
				// save the basket
				stocks[cid] = hisstocks; 
				D(CString(_T("Client ")+client+_T(" registered: ")+gjoin(hisstocks,",")));
				HeartBeat(client);
				return RegisterStocks(client);
				}
			case POSITIONREQUEST :
				{
				vector<CString> r;
				gsplit(msg,CString("+"),r);
				if (r.size()!=2) return UNKNOWN_MESSAGE;
				return PositionResponse(r[1],r[0]);
				}
			case REGISTERCLIENT :
				return RegisterClient(msg);
			case HEARTBEATREQUEST :
				return HeartBeat(msg);
			case BROKERNAME :
				return BrokerName();
			case SENDORDER :
				return SendOrder(TLOrder::Deserialize(msg));
			case FEATUREREQUEST:
				{
					// get features supported by child class
					std::vector<int> stub = GetFeatures();
					// append basic feature we provide as parent
					stub.push_back(REGISTERCLIENT);
					stub.push_back(HEARTBEATREQUEST);
					stub.push_back(CLEARSTOCKS);
					stub.push_back(CLEARCLIENT);
					stub.push_back(VERSION);
					// send entire feature set back to client
					TLSend(FEATURERESPONSE,SerializeIntVec(stub),msg);
					return OK;
				}
			case VERSION :
					return MinorVer;
		}

		return UnknownMessage(MessageType,msg);
	}
	
	/****************************************************************************
	*                          TLServer_IP::OnClose
	* Result: void
	*       
	* Effect: 
	*       The program has received a WM_CLOSE message and needs to shut down,
	*       so this initiates a shutdown
	****************************************************************************/

	void TLServer_IP::OnClose() 
	   {
		m_MainWndIsClosing = TRUE;
		updateControls();

		if(!CleanupThreads())
		   { /* threads running */
			TRACE(_T("%s: TLServer_IP::OnClose: deferring close\n"), AfxGetApp()->m_pszAppName);
			return;
		   } /* threads running */

		//CWnd::OnOK();
	   }
	
	/****************************************************************************
	*                            TLServer_IP::OnOK
	*                          TLServer_IP::OnCancel
	* Result: void
	*       
	* Effect: 
	*       Does nothing.  This is so that an <enter> key will not accidentally
	*       terminate the program
	****************************************************************************/

	void TLServer_IP::OnOK() 
	   {
	   }

	void TLServer_IP::OnCancel() 
	   {
	   }
	
	/****************************************************************************
	*                       TLServer_IP::updateControls                      
	* Result: void                                                               
	*                                                                            
	* Effect:                                                                    
	*       Updates the controls                                                 
	****************************************************************************/

	void TLServer_IP::updateControls()
		{
		 //c_Run.EnableWindow(!m_running && portnum > 1023 && portnum <= 65535);

		 //c_Port.SetReadOnly(m_running);
		 //x_Port.EnableWindow(!m_running);
		 // If we are shutting down, disable the entire dialog!
		 EnableWindow(!m_MainWndIsClosing);
		} // TLServer_IP::updateControls
	
	/****************************************************************************
	*                      TLServer_IP::OnBnClickedClose
	* Result: void
	*       
	* Effect: 
	*       The Close button on the dialog has been hit.  Initiate a shutdown
	*       of the app
	****************************************************************************/

	void TLServer_IP::OnBnClickedClose()
	   {
		PostMessage(WM_CLOSE);
	   }
	
	/****************************************************************************
	*                           TLServer_IP::OnSize
	* Inputs:
	*       UINT nType:
	*       int cx:
	*       int cy:
	* Result: void
	*       
	* Effect: 
	*       Resizes the listbox
	****************************************************************************/

	void TLServer_IP::OnSize(UINT nType, int cx, int cy) 
	   {
		CWnd::OnSize(nType, cx, cy);
	        

	   }
	
	/****************************************************************************
	*                           TLServer_IP::OnInfo
	* Inputs:
	*       WPARAM: (WPARAM)(CString *) String to display
	*       LPARAM: (LPARAM)(DWORD) thread id of thread providing info
	* Result: LRESULT
	*       Logically void, 0, always
	* Effect: 
	*       Logs the info string
	****************************************************************************/

	LRESULT TLServer_IP::OnInfo(WPARAM wParam, LPARAM lParam)
		{
		 CString s = *((CString *)wParam);
		 CString id;
		 id.Format(_T("%x: "), (DWORD)lParam);
		 CString msg;
		 msg.Format("%s %s",id,s);
		 D(msg);

		 return 0;
		} // TLServer_IP::OnInfo
	
	/****************************************************************************
	*                           TLServer_IP::OnRun
	* Result: void
	*       
	* Effect: 
	*       Creates the listener socket
	****************************************************************************/

	void TLServer_IP::OnRun() 
	   {
		m_running = CreateListener();
		updateControls();
	   }
	
	/****************************************************************************
	*                        TLServer_IP::OnChangePort
	* Result: void
	*       
	* Effect: 
	*       Updates the controls based on the port # changing
	****************************************************************************/

	void TLServer_IP::OnChangePort() 
	   {
		updateControls();
	   }
	
	/****************************************************************************
	*                       TLServer_IP::OnNetworkError
	* Inputs:
	*       WPARAM: (WPARAM)(DWORD) error code
	*       LPARAM: (LPARAM)(DWORD) thread that issued the error
	* Result: LRESULT
	*       Logically void, 0, always
	* Effect: 
	*       Records the error
	****************************************************************************/

	LRESULT TLServer_IP::OnNetworkError(WPARAM wParam, LPARAM lParam)
		{
		 DWORD err = (DWORD)wParam;
		 CString msg = ErrorString(err);
		 CString id;
		 id.Format(_T("%x: "), (DWORD)lParam);
		 CString m;
		 m.Format("%s %s",id,msg);
		 D(m);
		 return 0;
		} // TLServer_IP::OnNetworkError
	
	/****************************************************************************
	*                       TLServer_IP::OnSendComplete
	* Inputs:
	*       WPARAM: unused
	*       LPARAM: (WPARAM)(DWORD) Thread ID of thread that sent data
	* Result: LRESULT
	*       Logically void, 0, always
	* Effect: 
	*       Logs the data-sent event
	****************************************************************************/

	LRESULT TLServer_IP::OnSendComplete(WPARAM, LPARAM lParam)
		{
		 DWORD threadid = (DWORD)lParam;
		 CString fmt;
		 fmt.LoadString(IDS_DATA_SENT);
		 CString s;
		 s.Format(fmt, threadid);
		 D(s);
		 return 0;
		} // TLServer_IP::OnSendComplete

	// start tradelink functions

	int TLServer_IP::UnknownMessage(int MessageType, CString msg)
	{
		return UNKNOWN_MESSAGE;
	}

	int TLServer_IP::HeartBeat(CString clientname)
	{
			int cid = FindClient(clientname);
			if (cid==-1) return -1;
			time_t now;
			time(&now);
			time_t then = heart[cid];
			double dif = difftime(now,then);
			heart[cid] = now;
			return (int)dif;
	}

	int TLServer_IP::RegisterStocks(CString clientname) { return OK; }
	std::vector<int> TLServer_IP::GetFeatures() { std::vector<int> blank; return blank; } 

	int TLServer_IP::AccountResponse(CString clientname)
	{
		return FEATURE_NOT_IMPLEMENTED;
	}

	int TLServer_IP::PositionResponse(CString account, CString clientname)
	{
		return FEATURE_NOT_IMPLEMENTED;
	}

	int TLServer_IP::BrokerName(void)
	{
		return TradeLink;
	}

	int TLServer_IP::SendOrder(TLOrder order)
	{
		return FEATURE_NOT_IMPLEMENTED;
	}

	int TLServer_IP::ClearClient(CString clientname)
	{
		int cid = FindClient(clientname);
		if (cid==-1) return CLIENTNOTREGISTERED;
		client[cid] = "";
		stocks[cid] = clientstocklist(0);
		heart[cid] = NULL;
		D(CString(_T("Client ")+clientname+_T(" disconnected.")));
		return OK;
	}
	int TLServer_IP::ClearStocks(CString clientname)
	{
		int cid = FindClient(clientname);
		if (cid==-1) return CLIENTNOTREGISTERED;
		stocks[cid] = clientstocklist(0);
		HeartBeat(clientname);
		D(CString(_T("Cleared stocks for ")+clientname));
		return OK;
	}


	void TLServer_IP::D(const CString & message)
	{

		if (this->TLDEBUG)
		{
			const CString NEWLINE = "\n";
			CString line;
			vector<int> now;
			TLTimeNow(now);
			line.Format("%i %s%s",now[TLtime],message,NEWLINE);
			debugbuffer.Append(line);
			__raise this->GotDebug(line);
		}
	}

	void TLServer_IP::SrvGotOrder(TLOrder order)
	{
		if (order.symbol=="") return;
		for (size_t i = 0; i<client.size(); i++)
			if (client[i]!="")
				TLSend(ORDERNOTIFY,order.Serialize(),client[i]);
	}

	void TLServer_IP::SrvGotFill(TLTrade trade)
	{
		if (!trade.isValid()) return;
		for (size_t i = 0; i<client.size(); i++)
			if (client[i]!="")
				TLSend(EXECUTENOTIFY,trade.Serialize(),client[i]);
	}

	void TLServer_IP::SrvGotTick(TLTick tick)
	{
		if (tick.sym=="") return;
		for (uint i = 0; i<stocks.size(); i++)
			for (uint j = 0; j<stocks[i].size(); j++)
			{
				if (stocks[i][j]==tick.sym)
					TLSend(TICKNOTIFY,tick.Serialize(),client[i]);
			}
	}

	void TLServer_IP::SrvGotCancel(int orderid)
	{
		CString id;
		id.Format(_T("%i"),orderid);
		for (size_t i = 0; i<client.size(); i++)
			if (client[i]!="")
				TLSend(ORDERCANCELRESPONSE,id,client[i]);
	}

	int TLServer_IP::CancelRequest(long order)
	{
		return FEATURE_NOT_IMPLEMENTED;
	}

	bool TLServer_IP::HaveSubscriber(CString stock)
	{
		for (size_t i = 0; i<stocks.size(); i++) // go through each client
			for (size_t j = 0; j<stocks[i].size(); j++) // and each stock
				if (stocks[i][j].CompareNoCase(stock)==0) 
					return true;
		return false;
	}


	void TLServer_IP::Start(bool live)
	{
		if (!ENABLED)
		{
			CString wind(live ? LIVEWINDOW : SIMWINDOW);
			this->Create(NULL, wind, 0,CRect(0,0,20,20), CWnd::GetDesktopWindow(),NULL);
			this->ShowWindow(SW_HIDE); // hide our window
			OnRun();
			CString msg;
			msg.Format("Started TL BrokerServer %s [ %.1f.%i]",wind,MajorVer,MinorVer);
			this->D(msg);
			ENABLED = true;
		}
	}

	int TLServer_IP::RegisterClient(CString  clientname)
	{
		if (FindClient(clientname)!=-1) return OK;
		client.push_back(clientname);
		time_t now;
		time(&now);
		heart.push_back(now); // save heartbeat at client index
		clientstocklist my = clientstocklist(0);
		stocks.push_back(my);
		D(CString(_T("Client ")+clientname+_T(" connected.")));
		return OK;
	}

	unsigned int TLServer_IP::FindClientFromStock(CString stock)
	{
		for (unsigned int i = 0; i<client.size(); i++)
			for (unsigned int j = 0; j<stocks[i].size(); j++)
			{
				if (stocks[i][j].CompareNoCase(stock)==0)
					return i;
			}
		return -1;
	}

	bool TLServer_IP::needStock(CString stock)
	{
		for (size_t i = 0; i<stocks.size(); i++)
			for (size_t j = 0; j<stocks[i].size(); j++)
			{
				if (stocks[i][j]==stock) return true;
			}
		return false;
	}

	int TLServer_IP::FindClient(CString cwind)
	{
		size_t len = client.size();
		for (size_t i = 0; i<len; i++) if (client[i]==cwind) return (int)i;
		return -1;
	}


	long TLServer_IP::TLSend(int type,LPCTSTR msg,CString client) 
	{
		LRESULT result = TLCLIENT_NOT_FOUND;
		return (long)result;
	}


}

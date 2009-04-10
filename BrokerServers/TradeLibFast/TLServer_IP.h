#pragma once
#include "afxsock.h"
#include "Listens.h"    // Added by ClassView
#include "ASresource.h"
#include "Log.h"
#include "TradeLibFast.h"
using namespace std;

/////////////////////////////////////////////////////////////////////////////
// TLServer_IP dialog

namespace TradeLibFast
{

	[event_source(native)]
	class AFX_EXT_CLASS TLServer_IP : public CWnd
	{
	// Construction
		public:
		   TLServer_IP();  // standard constructor
		   // socket member that listens for new connections
		   CListensoc m_listensoc;
		   static const uint DEFAULTPORT = 3000;
		   uint PORT;


		   // tradelink functions
			~TLServer_IP(void);
			bool TLDEBUG;
			bool ENABLED;
			__event void GotDebug(LPCTSTR msg);
			CString debugbuffer;
			static long TLSend(int type,LPCTSTR msg,CString client);
			void SrvGotOrder(TLOrder order);
			void SrvGotFill(TLTrade trade);
			void SrvGotTick(TLTick tick);
			void SrvGotCancel(int orderid);
			void SrvGotPacket(TLPacket packet);
			CString Version();

			int RegisterClient(CString  clientname);
			int ServiceMessage(int MessageType, CString msg);
			int HeartBeat(CString clientname);
			virtual int UnknownMessage(int MessageType, CString msg);
			virtual int BrokerName(void);
			virtual int SendOrder(TLOrder order);
			virtual int AccountResponse(CString clientname);
			virtual int PositionResponse(CString account, CString clientname);
			virtual int RegisterStocks(CString clientname);
			virtual std::vector<int> GetFeatures();
			virtual int ClearClient(CString client);
			virtual int ClearStocks(CString client);
			virtual int CancelRequest(long order);
			virtual void Start(bool live = true);

			void D(const CString & message);


		bool HaveSubscriber(CString stock);

		protected:

			// tradelink protected stuff
			bool needStock(CString stock);
			int FindClient(CString clientname);
			double MajorVer;
			int MinorVer;

			vector <CString>client; // map client ids to name
			vector <time_t>heart; // map last contact to id
			typedef vector <CString> clientstocklist; // hold a single clients stocks
			vector < clientstocklist > stocks; // map stocklist to id
			unsigned int FindClientFromStock(CString stock);

		   CFont font;
		   int m_close;

		   BOOL m_running;  // listener has been created
		   BOOL CreateListener();
		   // a count of the number of open connections
		   int m_open;

		   CDWordArray m_threadIDs;
		   BOOL m_MainWndIsClosing; 

	// Dialog Data
			//{{AFX_DATA(TLServer_IP)
			enum { IDD = IDD_ASYNCSERVER_DIALOG };
			/*
			CStatic x_Port;
			CButton c_Run;
			CEdit   c_Port;
			CEdit   c_ClosedConnections;
			CEdit   c_OpenConnections;
			CButton c_Close;
			CEdit   c_LastString;
			CLog        c_Record;
			*/
			//}}AFX_DATA


	// Implementation
	protected:
			HICON m_hIcon;
			BOOL CleanupThreads();

			void updateControls();
			void ShowOpenConnections();
			void ShowClosedConnections();
	        
			// Generated message map functions
			//{{AFX_MSG(TLServer_IP)
			afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
			afx_msg void OnPaint();
			afx_msg HCURSOR OnQueryDragIcon();
			afx_msg void OnClose();
			virtual void OnOK();
			virtual void OnCancel();
			afx_msg void OnBnClickedClose();
			afx_msg void OnSize(UINT nType, int cx, int cy);
			afx_msg void OnRun();
			afx_msg void OnChangePort();
			//}}AFX_MSG

			LRESULT OnInfo(WPARAM, LPARAM);
			LRESULT OnThreadStart(WPARAM, LPARAM);  
			LRESULT OnThreadClose(WPARAM, LPARAM);
			LRESULT OnNetworkData(WPARAM, LPARAM);
			LRESULT OnNetworkError(WPARAM, LPARAM);
			LRESULT OnSendComplete(WPARAM, LPARAM);

			DECLARE_MESSAGE_MAP()
	};
}

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.



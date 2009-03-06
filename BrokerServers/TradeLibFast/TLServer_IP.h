#pragma once
#include "afxsock.h"
#include "Listens.h"    // Added by ClassView
#include "ASresource.h"
#include "Log.h"

/////////////////////////////////////////////////////////////////////////////
// TLServer_IP dialog

namespace TradeLibFast
{

	class AFX_EXT_CLASS TLServer_IP : public CWnd
	{
	// Construction
		public:
		   TLServer_IP();  // standard constructor
		   // socket member that listens for new connections
		   CListensoc m_listensoc;

		protected:
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
			CStatic x_Port;
			CButton c_Run;
			CEdit   c_Port;
			CEdit   c_ClosedConnections;
			CEdit   c_OpenConnections;
			CButton c_Close;
			CEdit   c_LastString;
			CLog        c_Record;
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



// TWSServerDlg.h : header file
//

#pragma once
#include "afxwin.h"
#include "TWS_TLServer.h"


// CTWSServerDlg dialog
[event_receiver(native)]
class CTWSServerDlg : public CDialog
{
// Construction
public:
	CTWSServerDlg(CWnd* pParent = NULL);	// standard constructor
	~CTWSServerDlg();

// Dialog Data
	enum { IDD = IDD_TWSSERVER_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;


	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
	CEdit m_status;
	TradeLibFast::TWS_TLServer* tl;
	int deblines;
public:
	void cstat(CString msg);
	void status(LPCTSTR msg);

};

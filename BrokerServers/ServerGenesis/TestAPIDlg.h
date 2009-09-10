/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// TestAPIDlg.h : header file
//

#if !defined(AFX_TESTAPIDLG_H__C7C7D67A_65B4_4F8D_B6C6_5112E3F90AA7__INCLUDED_)
#define AFX_TESTAPIDLG_H__C7C7D67A_65B4_4F8D_B6C6_5112E3F90AA7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "ServerGenesis.h"



/////////////////////////////////////////////////////////////////////////////
// CTestAPIDlg dialog

class CTestAPIDlg : public CDialog
{
// Construction
public:
	CTestAPIDlg(CWnd* pParent = NULL);	// standard constructor
	~CTestAPIDlg(void);


	ServerGenesis* tl;


// Dialog Data
	//{{AFX_DATA(CTestAPIDlg)
	enum { IDD = IDD_TESTAPI_DIALOG };
	CListBox	m_list;
	CButton	m_start;
	CString	m_strPassword;
	CString	m_strUserName;
	void status(LPCTSTR m);
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTestAPIDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL
// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CTestAPIDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	virtual void OnOK();
	virtual void OnCancel();
	afx_msg void OnClose();
	afx_msg void OnStart();
	afx_msg void OnStop();
	afx_msg void OnDump();
	afx_msg LRESULT AttemptLogin(WPARAM, LPARAM);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
public:

	afx_msg void OnEnChangePassword();
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TESTAPIDLG_H__C7C7D67A_65B4_4F8D_B6C6_5112E3F90AA7__INCLUDED_)

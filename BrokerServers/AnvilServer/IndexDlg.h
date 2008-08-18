#if !defined(AFX_INDEXDLG_H__092E8CC7_4619_4C93_AA85_ADE09A83BA75__INCLUDED_)
#define AFX_INDEXDLG_H__092E8CC7_4619_4C93_AA85_ADE09A83BA75__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// IndexDlg.h : header file
//

#include "BaseDlg.h"
#include "ObserverApi.h"
#include "BusinessApi.h"
/////////////////////////////////////////////////////////////////////////////
// IndexDlg dialog

class IndexDlg : public BaseDlg, public Observer
{
// Construction
public:
	IndexDlg(CWnd* pParent = NULL);   // standard constructor
//    virtual void OnCancel();

// Dialog Data
	//{{AFX_DATA(IndexDlg)
	enum { IDD = IDD_INDEX };
	CButton	m_CheckBoxDynamicUpdate;
	CStatic	m_StaticSymbol;
	CStatic	m_StaticValue;
	CStatic	m_StaticTick;
	CStatic	m_StaticOpen;
	CStatic	m_StaticNetChange;
	CStatic	m_StaticMarketOpen;
	CStatic	m_StaticLow;
	CStatic	m_StaticHigh;
	CStatic	m_StaticClose;
	CButton	m_ButtonOk;
	CEdit	m_EditSymbol;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(IndexDlg)
	protected:
    virtual void OnOK();
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(IndexDlg)
	afx_msg void OnChangeIndexSymbol();
	afx_msg void OnDynamicUpdate();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
    void FillInfo();

    MarketIndex* m_index;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_INDEXDLG_H__092E8CC7_4619_4C93_AA85_ADE09A83BA75__INCLUDED_)

#if !defined(AFX_CANCELORDERDLG_H__BF18AE15_1899_4373_BD16_1D5FB36AD142__INCLUDED_)
#define AFX_CANCELORDERDLG_H__BF18AE15_1899_4373_BD16_1D5FB36AD142__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CancelOrderDlg.h : header file
//

#include "BaseDlg.h"
#include "ObserverApi.h"
/////////////////////////////////////////////////////////////////////////////
// CancelOrderDlg dialog

class CancelOrderDlg : public BaseDlg
{
// Construction
public:
	CancelOrderDlg(CWnd* pParent = NULL);   // standard constructor
//    virtual void OnCancel();

// Dialog Data
	//{{AFX_DATA(CancelOrderDlg)
	enum { IDD = IDD_CANCELORDER };
	CButton	m_CheckBoxIncludeSmartOrders;
	CButton	m_RadioSingleDestination;
	CButton	m_RadioAllDestinations;
	CEdit	m_EditSymbol;
	CEdit	m_EditMmid;
	CEdit	m_EditOrderId;
	CButton	m_CheckBoxTifNonDay;
	CButton	m_CheckBoxTifDay;
	CButton	m_CheckBoxSideSell;
	CButton	m_CheckBoxSideBuy;
	CButton	m_RadioSingleStock;
	CButton	m_RadioAllStocks;
	CButton	m_RadioSingleOrder;
	CButton	m_RadioMultipleOrders;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CancelOrderDlg)
	protected:
    virtual void OnOK();
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CancelOrderDlg)
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
    Observable* m_account;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CANCELORDERDLG_H__BF18AE15_1899_4373_BD16_1D5FB36AD142__INCLUDED_)

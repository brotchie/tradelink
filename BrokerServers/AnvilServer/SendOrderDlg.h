#if !defined(AFX_SENDORDERDLG_H__B7A475BD_B598_47B7_B5C3_BEBE5041127A__INCLUDED_)
#define AFX_SENDORDERDLG_H__B7A475BD_B598_47B7_B5C3_BEBE5041127A__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// SendOrderDlg.h : header file
//

#include "SendOrderBaseDlg.h"
/////////////////////////////////////////////////////////////////////////////
// SendOrderDlg dialog

class OrderParamsSimple : public OrderParams
{
public:
//    char m_side;
    char m_destination[50];
//    Money m_price;
//    unsigned int m_size;
    unsigned int m_visibilityMode;
    unsigned int m_visibleSize;
    unsigned int m_listedDestinationExchange;
};

class SendOrderDlg : public SendOrderBaseDlg
{
// Construction
public:
	SendOrderDlg(CWnd* pParent = NULL);   // standard constructor
    static const char* GetOrderError(unsigned int error);
//    virtual void OnCancel();

// Dialog Data
	//{{AFX_DATA(SendOrderDlg)
	enum { IDD = IDD_SENDORDER };
	CButton	m_RadioDefault;
//	CButton	m_RadioNyseNX;
	CButton	m_RadioNyse;
	CButton	m_RadioAmex;
	CStatic	m_StaticVisibleSize;
	CEdit	m_EditVisibleSize;
	CEdit	m_EditDestination;
	CButton	m_CheckBoxHidden;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(SendOrderDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(SendOrderDlg)
	afx_msg void OnSendOrderHidden();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
    virtual unsigned int SendOrder(const StockBase* stockHandle, const OrderParams& orderParams, Order*& orderSent);
    virtual OrderParams* CreateOrderParams() const;
    unsigned int FillOrderParams(OrderParams* op);
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SENDORDERDLG_H__B7A475BD_B598_47B7_B5C3_BEBE5041127A__INCLUDED_)

#if !defined(AFX_SENDSTOPORDERDLG_H__02EB4156_D27E_4B41_A16E_07BD7F05D8B1__INCLUDED_)
#define AFX_SENDSTOPORDERDLG_H__02EB4156_D27E_4B41_A16E_07BD7F05D8B1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// SendStopOrderDlg.h : header file
//

#include "SendOrderBaseDlg.h"
#include "ObserverApi.h"
#include "BusinessApi.h"

class OrderParamsStop : public OrderParams
{
public:
//    char m_side;
//    Money m_price;
//    unsigned int m_size;
//    unsigned int m_tif;
    Money m_stopPrice;
    char m_postQuoteDestination[50];
    StopTriggerType m_triggerType;
	bool m_stopLossOnly;
};


/////////////////////////////////////////////////////////////////////////////
// SendStopOrderDlg dialog

class SendStopOrderDlg : public SendOrderBaseDlg
{
// Construction
public:
	SendStopOrderDlg(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(SendStopOrderDlg)
	enum { IDD = IDD_SENDSTOPORDER };
	CButton m_CheckBoxStopLossOnly;
	CEdit	m_EditStopPriceTenths;
	CEdit	m_EditStopPriceDollars;
	CEdit	m_EditStopPriceCents;
	CButton	m_RadioPostQuoteDestinationSize;
	CButton	m_RadioPostQuoteDestinationIsld;
	CButton	m_RadioTriggerPrice;
	CButton	m_RadioTriggerSameSideQuote;
	CButton	m_RadioTriggerOppositeSideQuote;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(SendStopOrderDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(SendStopOrderDlg)
		// NOTE: the ClassWizard will add member functions here
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
    virtual unsigned int SendOrder(const StockBase* stockHandle, const OrderParams& orderParams, Order*& orderSent);
    virtual OrderParams* CreateOrderParams() const;
    unsigned int FillOrderParams(OrderParams* op);
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SENDSTOPORDERDLG_H__02EB4156_D27E_4B41_A16E_07BD7F05D8B1__INCLUDED_)

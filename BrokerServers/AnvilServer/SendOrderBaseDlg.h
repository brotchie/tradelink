#if !defined(AFX_SENDORDERBASEDLG_H__C922A485_E3C2_4810_94F0_0A3EEB0E6327__INCLUDED_)
#define AFX_SENDORDERBASEDLG_H__C922A485_E3C2_4810_94F0_0A3EEB0E6327__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// SendOrderBaseDlg.h : header file
//

#include "BaseDlg.h"
#include "ObserverApi.h"
#include "BusinessApi.h"

class OrderParams
{
public:
    virtual ~OrderParams(){}

    char m_side;
//    char m_destination[50];
    Money m_price;
    unsigned int m_size;
//    unsigned int m_visibilityMode;
//    unsigned int m_visibleSize;
    unsigned int m_tif;
};

class OrderParamsList
{
public:
    virtual ~OrderParamsList();

    const StockBase* m_stockHandle;
    typedef std::list<OrderParams*> OrderList;
    OrderList m_orderList;
};

/////////////////////////////////////////////////////////////////////////////
// SendOrderBaseDlg dialog

class SendOrderBaseDlg : public BaseDlg, public Observer
{
// Construction
public:
	SendOrderBaseDlg(unsigned int id, CWnd* pParent = NULL);   // standard constructor
    virtual void OnOK();

    typedef std::map<std::string, OrderParamsList> OrdersToSend;

// Dialog Data
	//{{AFX_DATA(SendOrderBaseDlg)
//	enum { IDD = _UNKNOWN_RESOURCE_ID_ };
	CEdit	m_EditSymbol;
	CButton	m_RadioSideSell;
	CButton	m_RadioSideBuy;
	CEdit	m_EditSize;
	CEdit	m_EditPriceTenths;
	CEdit	m_EditPriceDollars;
	CEdit	m_EditPriceCents;
	CEdit	m_EditTifMinutes;
	CComboBox	m_ComboBoxTif;
	CStatic	m_StaticTifMinutes;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(SendOrderBaseDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(SendOrderBaseDlg)
	afx_msg void OnSelchangeSendOrderTif();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
    virtual unsigned int SendOrder(const StockBase* stockHandle, const OrderParams& orderParams, Order*& orderSent) = 0;
    virtual OrderParams* CreateOrderParams() const = 0;
    virtual unsigned int FillOrderParams(OrderParams* op);
    Observable* m_account;

    OrdersToSend m_ordersToSend;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SENDORDERBASEDLG_H__C922A485_E3C2_4810_94F0_0A3EEB0E6327__INCLUDED_)

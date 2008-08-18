#if !defined(AFX_LOADDLG_H__46AC26E3_2540_49D3_B1E3_6EF1FE6355E9__INCLUDED_)
#define AFX_LOADDLG_H__46AC26E3_2540_49D3_B1E3_6EF1FE6355E9__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// LoadDlg.h : header file
//

#include "BaseDlg.h"
#include "ListBoxFocus.h"

#include "Messages.h"
#include "ObserverApi.h"

class Stock;

class ListBoxStock : public ListBoxNoItemColor
{
// Construction
public:
    ListBoxStock(){}
    bool AddStock(Stock* stock);
    virtual void DeleteItem(LPDELETEITEMSTRUCT lpDeleteItemStruct);
    virtual int SearchCompare(const void* item1, const void* item2) const;
	void IncrementSubscriptionCount(int count);
	void IncrementLoadedCount(int count, bool subscribed);
protected:
    virtual void DoDrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
};

/////////////////////////////////////////////////////////////////////////////
// LoadDlg dialog

class LoadDlg : public BaseDlg, public Observer
{
// Construction
public:
	LoadDlg(CWnd* pParent = NULL);   // standard constructor
    virtual void OnOK();
//    virtual void OnCancel();

    virtual void MarketSummaryPopulationDone(bool done);

// Dialog Data
	//{{AFX_DATA(LoadDlg)
	enum { IDD = IDD_LOAD };
	CStatic	m_StaticStocksLoaded;
	CStatic	m_StaticStocksSubscribed;
	ListBoxStock	m_ListBoxStocks;
	CSpinButtonCtrl	m_SpinPriceDollarsTo;
	CSpinButtonCtrl	m_SpinPriceCentsTo;
	CSpinButtonCtrl	m_SpinPriceDollarsFrom;
	CSpinButtonCtrl	m_SpinPriceCentsFrom;
	CEdit	m_EditPriceDollarsTo;
	CEdit	m_EditPriceCentsTo;
	CEdit	m_EditPriceDollarsFrom;
	CEdit	m_EditPriceCentsFrom;
	CStatic	m_StaticMarketLoaded;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(LoadDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(LoadDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnChangePriceFromCents();
	afx_msg void OnChangePriceFromDollars();
	afx_msg void OnChangePriceToCents();
	afx_msg void OnChangePriceToDollars();
	afx_msg void OnKillfocusPriceFromCents();
	afx_msg void OnKillfocusPriceFromDollars();
	afx_msg void OnKillfocusPriceToCents();
	afx_msg void OnKillfocusPriceToDollars();
	//}}AFX_MSG
    afx_msg LRESULT OnSubscriptionCountIncrement(WPARAM count, LPARAM);
	afx_msg LRESULT OnLoadedCountIncrement(WPARAM count, LPARAM loaded);
	void UpdateSubscriptionCount();
	DECLARE_MESSAGE_MAP()
    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);

	unsigned int m_subscribeCount;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_LOADDLG_H__46AC26E3_2540_49D3_B1E3_6EF1FE6355E9__INCLUDED_)

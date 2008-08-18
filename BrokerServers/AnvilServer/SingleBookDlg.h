#if !defined(AFX_SINGLEBOOKDLG_H__CE26D129_A0CA_4D43_9465_97A56483D009__INCLUDED_)
#define AFX_SINGLEBOOKDLG_H__CE26D129_A0CA_4D43_9465_97A56483D009__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// SingleBookDlg.h : header file
//
#include "BaseDlg.h"
#include "ObserverApi.h"

class StockBase;

/////////////////////////////////////////////////////////////////////////////
// SingleBookDlg dialog

class SingleBookDlg : public BaseDlg, public Observer
{
// Construction
public:
	SingleBookDlg(CWnd* pParent = NULL);   // standard constructor

    virtual void DoCancel();
// Dialog Data
	//{{AFX_DATA(SingleBookDlg)
	enum { IDD = IDD_SINGLE_BOOK };
	CStatic	m_bid6;
	CStatic	m_bid5;
	CStatic	m_bid4;
	CStatic	m_bid3;
	CStatic	m_bid2;
	CStatic	m_bid1;
	CStatic	m_ask6;
	CStatic	m_ask5;
	CStatic	m_ask4;
	CStatic	m_ask3;
	CStatic	m_ask2;
	CStatic	m_ask1;
	CEdit	m_EditStock;
	CButton	m_CheckBoxDynamic;
	CButton	m_RadioNyse;
	CButton	m_RadioIsld;
	CButton	m_RadioBats;
	CButton	m_RadioArca;
	CButton	m_RadioExpanded;
	CButton	m_RadioAggregated;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(SingleBookDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
    virtual void OnOK();
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(SingleBookDlg)
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
    void Clear();
    void ClearControls();
    void FillQuotes(bool bid);

    QuoteControls m_bids;
    QuoteControls m_asks;
    const StockBase* m_stockHandle;
    Observable* m_book;

    void* m_bidIterator;
    void* m_askIterator;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SINGLEBOOKDLG_H__CE26D129_A0CA_4D43_9465_97A56483D009__INCLUDED_)

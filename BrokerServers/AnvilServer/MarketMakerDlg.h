#if !defined(AFX_MARKETMAKERDLG_H__C6A26F0E_92F8_46C8_BA19_AB7CB40F12DD__INCLUDED_)
#define AFX_MARKETMAKERDLG_H__C6A26F0E_92F8_46C8_BA19_AB7CB40F12DD__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// MarketMakerDlg.h : header file
//

#include "BaseDlg.h"
#include "Messages.h"
#include "ObserverApi.h"

class StockBase;

/////////////////////////////////////////////////////////////////////////////
// MarketMakerDlg dialog

class MarketMakerDlg : public BaseDlg, public Observer
{
// Construction
public:
	MarketMakerDlg(CWnd* pParent = NULL);   // standard constructor
    virtual ~MarketMakerDlg();

//    virtual void OnCancel();
    virtual void DoCancel();
// Dialog Data
	//{{AFX_DATA(MarketMakerDlg)
	enum { IDD = IDD_MARKETMAKER };
	CStatic	m_l1BidTick;
	CStatic	m_print6;
	CStatic	m_print5;
	CStatic	m_print4;
	CStatic	m_print3;
	CStatic	m_print2;
	CStatic	m_print1;
	CStatic	m_l1Volume;
	CStatic	m_l1Time;
	CStatic	m_l1Size;
	CStatic	m_l1Open;
	CStatic	m_l1Net;
	CStatic	m_l1Lo;
	CStatic	m_l1Last;
	CStatic	m_l1Hi;
	CStatic	m_l1Close;
	CStatic	m_l1BXA;
	CEdit	m_EditStock;
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
	//}}AFX_DATA

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(MarketMakerDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
    virtual void OnOK();
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(MarketMakerDlg)
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
    void Clear();
    void ClearControls();
    void FillQuotes(bool bid);
    void FillLevel1();
    void FillPrints();

    QuoteControls m_bids;
    QuoteControls m_asks;
    QuoteControls m_prints;

    const StockBase* m_stockHandle;
    Observable* m_level1;
    Observable* m_level2;
    Observable* m_print;
    Observable* m_book;

    void* m_bidIterator;
    void* m_askIterator;
    void* m_printsIterator;

    unsigned int m_linesIntegrated[MAX_BOOKS];
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MARKETMAKERDLG_H__C6A26F0E_92F8_46C8_BA19_AB7CB40F12DD__INCLUDED_)

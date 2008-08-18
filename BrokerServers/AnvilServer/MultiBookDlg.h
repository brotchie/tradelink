#if !defined(AFX_MULTIBOOKDLG_H__6F7F2FFB_CA90_4713_A990_1ED972B277A2__INCLUDED_)
#define AFX_MULTIBOOKDLG_H__6F7F2FFB_CA90_4713_A990_1ED972B277A2__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// MultiBookDlg.h : header file
//

#include "BaseDlg.h"
#include "ObserverApi.h"

class StockBase;
/////////////////////////////////////////////////////////////////////////////
// MultiBookDlg dialog

class MultiBookDlg : public BaseDlg, public Observer
{
// Construction
public:
	MultiBookDlg(CWnd* pParent = NULL);   // standard constructor

    virtual void DoCancel();
// Dialog Data
	//{{AFX_DATA(MultiBookDlg)
	enum { IDD = IDD_MULTI_BOOK };
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
	CButton	m_RadioExpanded;
	CButton	m_RadioAggregated;
	CButton	m_CheckBoxNyse;
	CButton	m_CheckBoxIsld;
	CButton	m_CheckBoxBats;
	CButton	m_CheckBoxArca;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(MultiBookDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
    virtual void OnOK();
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(MultiBookDlg)
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
    Observable* m_level1;

    void* m_bidIterator;
    void* m_askIterator;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MULTIBOOKDLG_H__6F7F2FFB_CA90_4713_A990_1ED972B277A2__INCLUDED_)

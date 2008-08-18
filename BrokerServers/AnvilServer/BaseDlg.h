#if !defined(AFX_BASEDLG_H__8106EAF8_BB4C_4217_9494_0C2C29C6B7FB__INCLUDED_)
#define AFX_BASEDLG_H__8106EAF8_BB4C_4217_9494_0C2C29C6B7FB__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// BaseDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// BaseDlg dialog

class BaseDlg : public CDialog
{
// Construction
public:
	BaseDlg(unsigned int id, CWnd* pParent = NULL);   // standard constructor
    BOOL CreateTradeDialog();
    virtual void OnOK(){}
    virtual void OnCancel();

    virtual void MarketSummaryPopulationDone(bool done){}
    virtual void DoCancel(){}

    typedef std::vector<CStatic*> QuoteControls;
    static void Fill(QuoteControls& quoteControls, void* iterator);
// Dialog Data
	//{{AFX_DATA(BaseDlg)
//	enum { IDD = _UNKNOWN_RESOURCE_ID_ };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(BaseDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(BaseDlg)
		// NOTE: the ClassWizard will add member functions here
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
    unsigned int m_dialogId;
    CWnd* m_parentWindow;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_BASEDLG_H__8106EAF8_BB4C_4217_9494_0C2C29C6B7FB__INCLUDED_)

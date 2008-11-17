#if !defined(AFX_EXTFRAME_H__0E7D4985_B30B_4067_BFC0_0DB7C6D9C54C__INCLUDED_)
#define AFX_EXTFRAME_H__0E7D4985_B30B_4067_BFC0_0DB7C6D9C54C__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// ExtFrame.h : header file
//

#include "Messages.h"
#include "ObserverApi.h"
#include "Monitor.h"
#include "TradeLibFast.h"

class ExtFrame : public CFrameWnd, public Observer
{
//	DECLARE_DYNCREATE(ExtFrame)
public:
	ExtFrame();           // protected constructor used by dynamic creation
	virtual ~ExtFrame();
    void ShowWindowAndChildren(int show);
	Monitor* monitor;
public:
    static ExtFrame* GetInstance(){return instance;}
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(ExtFrame)
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(ExtFrame)
	afx_msg void OnDestroy();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg BOOL OnCopyData(CWnd* sWnd,COPYDATASTRUCT* pCD);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);

    static ExtFrame* instance;

//    std::string m_version;
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_EXTFRAME_H__0E7D4985_B30B_4067_BFC0_0DB7C6D9C54C__INCLUDED_)

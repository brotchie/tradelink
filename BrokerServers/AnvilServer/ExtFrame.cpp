// ExtFrame.cpp : implementation file
//

#include "stdafx.h"
#include "Resource.h"
#include "ExtFrame.h"
#include "TradeLink.h"
#include "TLAnvil.h"
#include "Monitor.h"

#include "ObserverApi.h"
#include "BusinessApi.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

ExtFrame* ExtFrame::instance = NULL;


ExtFrame::ExtFrame()
{
    instance = this;
	monitor = new Monitor();
	B_GetAdminObservable()->Add(monitor);
    B_GetAdminObservable()->Add(this);

}

ExtFrame::~ExtFrame()
{
	TLUnload();
	B_GetAdminObservable()->Remove(monitor);
	delete monitor;
    instance = NULL;
}


BEGIN_MESSAGE_MAP(ExtFrame, CFrameWnd)
	//{{AFX_MSG_MAP(ExtFrame)
	ON_WM_COPYDATA()
	ON_WM_DESTROY()
	ON_WM_SYSCOMMAND()

	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// ExtFrame message handlers



void ExtFrame::OnDestroy()
{
	CFrameWnd::OnDestroy();
    instance = NULL;	
}

void ExtFrame::ShowWindowAndChildren(int show)
{
    ShowWindow(show);
}

void ExtFrame::OnSysCommand(UINT nID, LPARAM lParam)
{
    switch(nID)
    {
		case SC_CLOSE:
        ShowWindowAndChildren(SW_HIDE);
        break;

        default:
        CFrameWnd::OnSysCommand(nID, lParam);
        break;
	}
}


void ExtFrame::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
	
}



BOOL ExtFrame::OnCopyData(CWnd* sWnd, COPYDATASTRUCT* CD) 
{
	CString gotMsg = (LPCTSTR)(CD->lpData);
	int gotType = (int)(CD->dwData);
	return ServiceMsg(gotType,gotMsg);

}

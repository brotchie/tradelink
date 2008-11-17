// ExtFrame.cpp : implementation file
//

#include "stdafx.h"
#include "Resource.h"
#include "ExtFrame.h"

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
	tl = new AVL_TLWM();
	B_GetAdminObservable()->Add(tl);
    B_GetAdminObservable()->Add(this);

}

ExtFrame::~ExtFrame()
{
	//TLUnload();
	B_GetAdminObservable()->Remove(tl);
	delete tl;
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


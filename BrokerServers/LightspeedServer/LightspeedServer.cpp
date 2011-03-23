// GrayBoxSample.cpp : Defines the initialization routines for the DLL.
//

#include "stdafx.h"
#include "GrayBoxSample.h"
#include "MainDlg.h"
#include "LS_TLWM.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#endif

std::string version;
void CGrayBoxSampleApp::StartExtension()
{
	
}
void CGrayBoxSampleApp::StopExtension()
{
	
	
}

/*
void CGrayBoxSampleApp::StartExtension()
{
	
	MainDlg *pDlg = new MainDlg;
	if (pDlg->Create(IDD_MAINDLG, CWnd::FromHandle(L_GetMainWnd())))
	{
		m_pMainWnd = pDlg;
	}
	else
	{
		delete pDlg;
	}
}
void CGrayBoxSampleApp::StopExtension()
{
	
	if (m_pMainWnd != 0)
	{
		m_pMainWnd->DestroyWindow();
		m_pMainWnd = 0;
	}
}*/


//
//TODO: If this DLL is dynamically linked against the MFC DLLs,
//		any functions exported from this DLL which call into
//		MFC must have the AFX_MANAGE_STATE macro added at the
//		very beginning of the function.
//
//		For example:
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// normal function body here
//		}
//
//		It is very important that this macro appear in each
//		function, prior to any calls into MFC.  This means that
//		it must appear as the first statement within the 
//		function, even before any object variable declarations
//		as their constructors may generate calls into the MFC
//		DLL.
//
//		Please see MFC Technical Notes 33 and 58 for additional
//		details.
//


// CGrayBoxSampleApp

BEGIN_MESSAGE_MAP(CGrayBoxSampleApp, CWinApp)
END_MESSAGE_MAP()


// CGrayBoxSampleApp construction

CGrayBoxSampleApp::CGrayBoxSampleApp()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}


// The one and only CGrayBoxSampleApp object

CGrayBoxSampleApp theApp;

LSEXPORT void LSInitInstance()
{
	LS_TLWM* frame = LS_TLWM::GetInstance();


	if (!frame)
	{
		frame = new LS_TLWM();
		version += frame->Version();
	}
			
	frame->Start();
	CString tmp;
	tmp.Format("TradeLink LightspeedConnector %s",frame->Version());
	frame->D(tmp);
}
LSEXPORT void LSExitInstance()
{
	LS_TLWM* frame = LS_TLWM::GetInstance();
    if(frame)
    {
        frame->DestroyWindow();
    }
	delete frame;
	frame = NULL;
}
LSEXPORT BOOL LSPreTranslateMessage(MSG *pMsg)
{
	return theApp.PreTranslateMessage(pMsg);
}

// CGrayBoxSampleApp initialization

BOOL CGrayBoxSampleApp::InitInstance()
{
	CWinApp::InitInstance();

	return TRUE;
}

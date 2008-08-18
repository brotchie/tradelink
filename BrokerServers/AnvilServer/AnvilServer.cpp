// AnvilExtTest.cpp : Defines the initialization routines for the DLL.
//

#include "stdafx.h"
#include "Resource.h"
#include "BusinessApi.h"
#include <afxdllx.h>
#include "SendOrderBaseDlg.h"
#include "ExtFrame.h"
#include <fstream>
#include "PosOrdersExecsDlg.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif


HINSTANCE module_instance = NULL;

static AFX_EXTENSION_MODULE TradeLinkDLL = { NULL, NULL };

extern "C" int APIENTRY
DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved)
{
	// Remove this if you use lpReserved
	UNREFERENCED_PARAMETER(lpReserved);

	if (dwReason == DLL_PROCESS_ATTACH)
	{
        module_instance = hInstance;

		TRACE0("AnvilServer.DLL Initializing!\n");
		
		// Extension DLL one-time initialization
		if (!AfxInitExtensionModule(TradeLinkDLL, hInstance))
			return 0;

		// Insert this DLL into the resource chain
		// NOTE: If this Extension DLL is being implicitly linked to by
		//  an MFC Regular DLL (such as an ActiveX Control)
		//  instead of an MFC application, then you will want to
		//  remove this line from DllMain and put it in a separate
		//  function exported from this Extension DLL.  The Regular DLL
		//  that uses this Extension DLL should then explicitly call that
		//  function to initialize this Extension DLL.  Otherwise,
		//  the CDynLinkLibrary object will not be attached to the
		//  Regular DLL's resource chain, and serious problems will
		//  result.

		new CDynLinkLibrary(TradeLinkDLL);
	}
	else if (dwReason == DLL_PROCESS_DETACH)
	{

		TRACE0("AnvilServer.DLL Terminating!\n");
		// Terminate the library before destructors are called
		AfxTermExtensionModule(TradeLinkDLL);
	}
	return 1;   // ok
}


#ifdef __cplusplus
extern "C" {
#endif

void WINAPI TerminateAnvilExtension()
{
    ExtFrame* frame = ExtFrame::GetInstance();
    if(frame)
    {
        frame->DestroyWindow();
//        delete frame;
    }

}

const char* WINAPI GetAnvilExtensionDescription()
{
    return "TradeLink AnvilServer";
}

const char* WINAPI GetAnvilExtensionMenu()
{
    static char menu[30];
    const char* ms = "Show Window";
    const char* mh = "Hide Window";

    char* cursor = menu;
	unsigned int len = sizeof(menu);
	unsigned int le;

	strcpy_s(cursor, len, ms);
	le = (unsigned int)strlen(cursor) + 1;
	if(len <= le)
	{
		*menu = '\0';
		return menu;
	}
	len -= le;
    cursor += le;

    strcpy_s(cursor, len, mh);
	le = (unsigned int)strlen(cursor) + 1;
	if(len <= le)
	{
		*menu = '\0';
		return menu;
	}
	len -= le;
    cursor += le;
    *cursor = '\0';
    return menu;
}


void ShowMainWindow()
{
    ExtFrame* frame = ExtFrame::GetInstance();
	Observable* m_account = B_GetCurrentAccount();
	BOOL isSim = B_IsAccountSimulation(m_account);

    if(!frame)
    {
		LPCTSTR windname = "TL-BROKER-SIMU";
		if (!isSim) windname = "TL-BROKER-LIVE";

        frame = new ExtFrame();

        CRect rect(200, 200, 600, 400);
	// create and load the frame with its resources
//	pFrame->Create(NULL, "", WS_THICKFRAME|WS_POPUP|WS_CLIPCHILDREN|WS_SYSMENU|WS_MINIMIZEBOX|WS_CAPTION, CRect(0, 0, 200, 100));
        CWnd* parent = AfxGetMainWnd();
//HINSTANCE resource = AfxGetResourceHandle();
//AfxSetResourceHandle(module_instance);
	    frame->Create(NULL, windname, WS_OVERLAPPEDWINDOW, rect, parent, MAKEINTRESOURCE(IDR_MAINFRAME));//, WS_VISIBLE|WS_POPUP|WS_CLIPCHILDREN|WS_THICKFRAME|WS_SYSMENU|WS_MINIMIZEBOX, rect);
//        frame->ShowWindow(SW_SHOW);
        //frame->ShowWindowAndChildren(SW_SHOW);
    }
    else
    {
//        frame->ShowWindow(SW_SHOW);
        frame->ShowWindowAndChildren(SW_SHOW);
        frame->SetForegroundWindow();
    }
}

std::string version;
const char* VERFILE = "c:\\progra~1\\tradelink\\brokerserver\\VERSION.txt";

void WINAPI InitializeAnvilExtension()
{
	CString major = "2.2";
	CString minor("$Rev: 197 $");
	std::ifstream file;
	file.open(VERFILE);
	if (file.is_open())
	{
		char data[8];
		file.getline(data,8);
		minor = CString(data);
		file.close();
	}
	else
	{
		minor.Replace("$Rev: ","");
		minor.TrimRight(" $");
	}
	CString ver;
	ver.Format("%s.%s",major,minor);
	version += ver;
    ShowMainWindow();

}

bool WINAPI DoAnvilExtensionCommand(unsigned int id)
{
    switch(id)
    {
        case 0:
        ShowMainWindow();
        break;

        case 1:
        {
            ExtFrame* frame = ExtFrame::GetInstance();
            if(frame)
            {
                frame->SendMessage(WM_SYSCOMMAND, SC_CLOSE, 0);
            }
        }
        break;

        default:
        return false;
    }
    return true;
}

const char* WINAPI GetAnvilExtensionVersion()
{
	return version.c_str();
}

bool WINAPI IsAnvilExtensionDebug()
{
#ifdef _DEBUG
    return true;
#else
    return false;
#endif
}


const char* WINAPI GetReceiverHeaderVersion()
{
    return ReceiverHeaderVersion;
}

const char* WINAPI GetObserverHeaderVersion()
{
    return ObserverHeaderVersion;
}

const char* WINAPI GetBusinessHeaderVersion()
{
    return BusinessHeaderVersion;
}





#ifdef __cplusplus
} //extern "C"
#endif

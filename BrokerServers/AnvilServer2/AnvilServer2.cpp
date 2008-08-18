// AnvilServer2.cpp : Defines the initialization routines for the DLL.
//

#include "stdafx.h"
#include "Messages.h"
#include "BusinessApi.h"
#include <afxdllx.h>
#include "AVL_TLWM.h"
#include "Util.h"
#include "AnvilServerStatus.h"

#ifdef _MANAGED
#error Please read instructions in AnvilServer2.cpp to compile with /clr
// If you want to add /clr to your project you must do the following:
//	1. Remove the above include for afxdllx.h
//	2. Add a .cpp file to your project that does not have /clr thrown and has
//	   Precompiled headers disabled, with the following text:
//			#include <afxwin.h>
//			#include <afxdllx.h>
#endif

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


static AFX_EXTENSION_MODULE AnvilServer2DLL = { NULL, NULL };

#ifdef _MANAGED
#pragma managed(push, off)
#endif

extern "C" int APIENTRY
DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved)
{
	// Remove this if you use lpReserved
	UNREFERENCED_PARAMETER(lpReserved);

	if (dwReason == DLL_PROCESS_ATTACH)
	{
		TRACE0("AnvilServer2.DLL Initializing!\n");
		
		// Extension DLL one-time initialization
		if (!AfxInitExtensionModule(AnvilServer2DLL, hInstance))
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

		new CDynLinkLibrary(AnvilServer2DLL);

	}
	else if (dwReason == DLL_PROCESS_DETACH)
	{
		TRACE0("AnvilServer2.DLL Terminating!\n");

		// Terminate the library before destructors are called
		AfxTermExtensionModule(AnvilServer2DLL);
	}
	return 1;   // ok
}


#ifdef __cplusplus
extern "C" {
#endif

	
using namespace TradeLinkServer;

AVL_TLWM* tl = NULL;

void WINAPI InitializeAnvilExtension()
{
		Observable* m_account = B_GetCurrentAccount();
		bool isSim = B_IsAccountSimulation(m_account);
		tl = new AVL_TLWM();
		tl->Start(!isSim);
}

void WINAPI TerminateAnvilExtension()
{
	// run by anvil at dll termination
	delete tl;
}



bool WINAPI DoAnvilExtensionCommand(unsigned int id)
{
	// this is called when the AnvilExtensionMenu is selected, with the ID of the menu item chosen
	AnvilServerStatus dlg = new AnvilServerStatus(AfxGetMainWnd());
	dlg.loadfrom = tl->debugbuffer;
	dlg.DoModal();
	delete dlg;
    return true;
}

const char* WINAPI GetAnvilExtensionDescription()
{
    return "Anvil BrokerServer";
}

const char* WINAPI GetAnvilExtensionMenu()
{
    static char menu[30];
    const char* ms = "Show Debugs";

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
    *cursor = '\0';
    return menu;
}


const char* WINAPI GetAnvilExtensionVersion()
{
	CString major = "2.2";
	char* minor = cleansvnrev("$Rev: 101 $");
	CString ver;
	ver.Format("%s.%s",major,minor);
	return ver.GetBuffer();
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

bool WINAPI IsAnvilExtensionDebug()
{
#ifdef _DEBUG
    return true;
#else
    return false;
#endif
}

#ifdef __cplusplus
} //extern "C"
#endif




#ifdef _MANAGED
#pragma managed(pop)
#endif


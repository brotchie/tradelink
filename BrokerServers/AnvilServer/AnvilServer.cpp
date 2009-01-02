// AnvilExtTest.cpp : Defines the initialization routines for the DLL.
//

#include "stdafx.h"
#include "BusinessApi.h"
#include <afxdllx.h>
#include "AVL_TLWM.h"
#include <fstream>
#include "Messages.h"
using namespace TradeLibFast;


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
    AVL_TLWM* frame = AVL_TLWM::GetInstance();
    if(frame)
    {
        frame->DestroyWindow();
    }
	delete frame;
	frame = NULL;
}

const char* WINAPI GetAnvilExtensionDescription()
{
    return "Anvil+BrokerServer";
}

const char* WINAPI GetAnvilExtensionMenu()
{
    static char menu[30];
	return menu;

}

std::string version;


void ShowMainWindow()
{
    AVL_TLWM* frame = AVL_TLWM::GetInstance();
	Observable* m_account = B_GetCurrentAccount();
	BOOL isSim = B_IsAccountSimulation(m_account);
	if (!frame)
	{
		frame = new AVL_TLWM();
		version += frame->Version();
	}
	frame->Start(!isSim);
}


void WINAPI InitializeAnvilExtension()
{
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
            AVL_TLWM* frame = AVL_TLWM::GetInstance();
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

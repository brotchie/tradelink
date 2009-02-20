// TradeLibFast.cpp : Defines the initialization routines for the DLL.
//

#include "stdafx.h"
#include <afxdllx.h>
#include <string>
#include "TradeLibFast.h"
using namespace TradeLibFast;
#ifdef _MANAGED
#error Please read instructions in TradeLibFast.cpp to compile with /clr
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


static AFX_EXTENSION_MODULE TradeLibFastDLL = { NULL, NULL };

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
		TRACE0("TradeLibFast.DLL Initializing!\n");
		
		// Extension DLL one-time initialization
		if (!AfxInitExtensionModule(TradeLibFastDLL, hInstance))
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

		new CDynLinkLibrary(TradeLibFastDLL);

	}
	else if (dwReason == DLL_PROCESS_DETACH)
	{
		TRACE0("TradeLibFast.DLL Terminating!\n");

		// Terminate the library before destructors are called
		AfxTermExtensionModule(TradeLibFastDLL);
	}
	return 1;   // ok
}

#ifdef __cplusplus
extern "C"  {
#endif

	int __stdcall TLSENDORDER(LPSTR sym, BOOL side, int size, double price, double stop, int id, LPSTR account, LPSTR exchange)
	{
		// we need this line here in order to create windows from this dll
		if (!AfxWinInit(::GetModuleHandle(NULL), NULL, ::GetCommandLine(), 0))
			return -1;

		TLClient_WM tl;
		TLOrder o;
		o.symbol = sym;
		o.side = side != 0;
		o.size = abs(size);
		o.price = price;
		o.stop = stop;
		o.id = id;
		o.account = account;
		CString ex = exchange;
		if (ex!="")
			o.exchange = ex;
		int error = tl.SendOrder(o);
		return error;
	}
	void __stdcall TLSENDCANCEL(int orderid)
	{
		// we need this line here in order to create windows from this dll
		if (!AfxWinInit(::GetModuleHandle(NULL), NULL, ::GetCommandLine(), 0))
			return;

		TLClient_WM tl;
		tl.CancelOrder(orderid);
	}

#ifdef __cplusplus
} //extern "C"
#endif

#ifdef _MANAGED
#pragma managed(pop)
#endif


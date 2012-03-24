// GrayBoxSample.h : main header file for the GrayBoxSample DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CGrayBoxSampleApp
// See GrayBoxSample.cpp for the implementation of this class
//

class CGrayBoxSampleApp : public CWinApp
{
public:
	CGrayBoxSampleApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};

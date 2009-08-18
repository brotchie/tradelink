/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// TestAPI.h : main header file for the TESTAPI application
//

#if !defined(AFX_TESTAPI_H__1445C8D4_F507_4D15_A048_1C4DA49522A7__INCLUDED_)
#define AFX_TESTAPI_H__1445C8D4_F507_4D15_A048_1C4DA49522A7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

#include "GTAPI.h"

/////////////////////////////////////////////////////////////////////////////
// CTestAPIApp:
// See TestAPI.cpp for the implementation of this class
//

class CTestAPIApp : public CWinApp
{
public:
	CTestAPIApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTestAPIApp)
	public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CTestAPIApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TESTAPI_H__1445C8D4_F507_4D15_A048_1C4DA49522A7__INCLUDED_)

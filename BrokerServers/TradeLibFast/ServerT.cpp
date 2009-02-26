// ServerT.cpp : implementation file
//
// of Microsoft Technical Support, Developer Support
// Copyright (c) 1998 Microsoft Corporation. All rights reserved.
// Rewritten to Best Practice by Joseph M. Newcomer, Mar 2007

#include "stdafx.h"
#include "ServerT.h"
#include "messages.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CServerThread

IMPLEMENT_DYNCREATE(CServerThread, CSocketThread)

CServerThread::CServerThread() : CSocketThread()
{
}

CServerThread::~CServerThread()
{
}

/****************************************************************************
*                         CServerThread::InitInstance
* Inputs:
*       ¶
* Result: BOOL
*       TRUE if successful; thread will continue execution
*       FALSE if failure; thread is aborted
* Effect: 
*       Does nothing for now except the superclass initialization
****************************************************************************/

BOOL CServerThread::InitInstance()
   {
    if(!CSocketThread::InitInstance())
       return FALSE;
    // TODO: any additional initialization would go here
    return TRUE;
   }

/****************************************************************************
*                         CServerThread::ExitInstance
* Result: int
*       Desired return code
* Effect: 
*       Notifies the target that the thread has closed
****************************************************************************/

int CServerThread::ExitInstance()
   {
    return CSocketThread::ExitInstance();
   }

/****************************************************************************
*                                 Message Map
****************************************************************************/

BEGIN_MESSAGE_MAP(CServerThread, CSocketThread)
        //{{AFX_MSG_MAP(CServerThread)
                // NOTE - the ClassWizard will add and remove mapping macros here.
        //}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CServerThread message handlers

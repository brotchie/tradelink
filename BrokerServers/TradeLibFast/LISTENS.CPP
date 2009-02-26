// Listens.cpp : implementation file
//
// of Microsoft Technical Support, Developer Support
// Copyright (c) 1998 Microsoft Corporation. All rights reserved.
// Rewritten to Best Practice by Joseph M. Newcomer, Mar 2007

#include "stdafx.h"
//#include "AsyncServerDlg.h"
#include "Listens.h"
#include "ServerT.h"
#include "ErrorString.h"
#include "Messages.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif




/////////////////////////////////////////////////////////////////////////////
// CListensoc
CListensoc::CListensoc() :
   target(NULL)
{
}
CListensoc::~CListensoc()
{
}


// Do not edit the following lines, which are needed by ClassWizard.
#if 0
BEGIN_MESSAGE_MAP(CListensoc, CAsyncSocket)
        //{{AFX_MSG_MAP(CListensoc)
        //}}AFX_MSG_MAP
END_MESSAGE_MAP()
#endif  // 0

/////////////////////////////////////////////////////////////////////////////
// CListensoc member functions

void CListensoc::OnAccept(int nErrorCode) 
   {
    TRACE(_T("%s: CListensoc::OnAccept(%d)\n"), AfxGetApp()->m_pszAppName, nErrorCode);
    if(nErrorCode != ERROR_SUCCESS)
       { /* had an error */
        ASSERT(FALSE);
        ASSERT(target != NULL);
        if(target != NULL)
           target->PostMessage(UWM_NETWORK_ERROR, (WPARAM)nErrorCode, (LPARAM)::GetCurrentThreadId());
        return;
       } /* had an error */
    // New connection is being established

    CAsyncSocket soc;

    // Accept the connection using a temp CAsyncSocket object.
    if(!Accept(soc))
       { /* accept failed */
        DWORD err = ::GetLastError();
        ASSERT(target != NULL);
        if(target != NULL)
           target->PostMessage(UWM_NETWORK_ERROR, (WPARAM)err, (LPARAM)::GetCurrentThreadId());
        return;
       } /* accept failed */

    // Create a thread to handle the connection. The thread is created suspended so that we can
    // set variables in CServerThread before it starts executing.
    CServerThread* pThread = (CServerThread*)AfxBeginThread(RUNTIME_CLASS(CServerThread),
                                                            THREAD_PRIORITY_NORMAL,
                                                            0,
                                                            CREATE_SUSPENDED);
    if (pThread == NULL)
        {
         DWORD err = ::GetLastError();
         ASSERT(FALSE);  // help in debugging
         soc.Close();
         TRACE(_T("%s: CListenSoc::OnAccept: Could not create thread: %s\n"), AfxGetApp()->m_pszAppName, ErrorString(err));
         ASSERT(target != NULL);
         if(target != NULL)
            target->PostMessage(UWM_NETWORK_ERROR, (WPARAM)err, (LPARAM)::GetCurrentThreadId());
         return;
        }

    // Pass the socket to the thread by passing the socket handle. You cannot pass
    // a CSocket object across threads.
    pThread->SetSocket(soc.Detach());
    pThread->SetTarget(target);

    // Now start the thread.
    pThread->ResumeThread();
        
    CAsyncSocket::OnAccept(nErrorCode);
   }

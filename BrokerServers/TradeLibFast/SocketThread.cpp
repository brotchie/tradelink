// SocketThread.cpp : implementation file
//
// of Microsoft Technical Support, Developer Support
// Copyright (c) 1998 Microsoft Corporation. All rights reserved.
// Rewritten to Best Practice by Joseph M. Newcomer, Mar 2007

#include "stdafx.h"
#include "SocketThread.h"
#include "messages.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CSocketThreadhread

IMPLEMENT_DYNCREATE(CSocketThread, CWinThread)

CSocketThread::CSocketThread() :
   socket(NULL),
   target(NULL)
{
}

CSocketThread::~CSocketThread()
{
}

/****************************************************************************
*                         CSocketThread::InitInstance
* Result: BOOL
*       TRUE if successful, let thread execute
*       FALSE if falure
* Effect: 
*       ¶
****************************************************************************/

BOOL CSocketThread::InitInstance()
   {
    CConnectSoc * sock = GetSocket();
    ASSERT(sock != NULL);
    if(sock == NULL)
       { /* failed */
        return FALSE;
       } /* failed */

    // Post a message to the main thread so that it can update the number of
    // open connections
    ASSERT(target != NULL);
    if(target == NULL)
       return FALSE;
    
    target->PostMessage(UWM_THREADSTART, 0, (LPARAM)m_nThreadID);

    sock->SetTarget(target);
    sock->SetInfoRequest(TRUE);

    if(socket != NULL)
       { /* attach socket */
        sock->Attach(socket);
        sock->TraceAttach();
       } /* attach socket */
    return TRUE;
   }

/****************************************************************************
*                         CSocketThread::ExitInstance
* Result: int
*       Desired return code
* Effect: 
*       Notifies the target that the thread has closed
****************************************************************************/

int CSocketThread::ExitInstance()
   {
    ASSERT(target != NULL);
    if(target != NULL)
       target->PostMessage(UWM_THREADCLOSE, 0, (LPARAM)m_nThreadID);
    return CWinThread::ExitInstance();
   }

/****************************************************************************
*                                 Message Map
****************************************************************************/

BEGIN_MESSAGE_MAP(CSocketThread, CWinThread)
        //{{AFX_MSG_MAP(CSocketThread)
                // NOTE - the ClassWizard will add and remove mapping macros here.
        //}}AFX_MSG_MAP
        ON_THREAD_MESSAGE(UWM_TERM_THREAD, OnTermThread)
        ON_THREAD_MESSAGE(UWM_SEND_DATA, OnSendData)
        ON_THREAD_MESSAGE(UWM_START_NEXT_PACKET, OnStartNextPacket)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CSocketThread message handlers

/****************************************************************************
*                         CSocketThread::OnTermThread
* Inputs:
*       WPARAM: ignored
*       LPARAM: ignored
* Result: void
*       
* Effect: 
*       Requests the thread shut down
****************************************************************************/

void CSocketThread::OnTermThread(WPARAM, LPARAM)
   {
    CConnectSoc * sock = GetSocket();
    ASSERT(sock != NULL);
    if(sock != NULL)
       sock->DoClose();
    ::PostQuitMessage(0);
   }

/****************************************************************************
*                          CSocketThread::OnSendData
* Inputs:
*       WPARAM: (WPARAM)(CByteArray *) data to send
*       LPARAM: unused, but passed to handler
* Result: void
*       
* Effect: 
*       Requests an explicit send of the packet
****************************************************************************/

void CSocketThread::OnSendData(WPARAM wParam, LPARAM lParam)
    {
     CConnectSoc * sock = GetSocket();
     ASSERT(sock != NULL);
     if(sock == NULL)
        return;
     sock->DoSendData(wParam, lParam);
    } // CSocketThread::OnSendData

/****************************************************************************
*                      CSocketThread::OnStartNextPacket
* Inputs:
*       WPARAM: unused
*       LPARAM: unused
* Result: void
*       
* Effect: 
*       This is posted by the network layer to dequeue the next send packet
****************************************************************************/

void CSocketThread::OnStartNextPacket(WPARAM, LPARAM)
   {
    CConnectSoc * sock = GetSocket();
    ASSERT(sock != NULL);
    if(sock == NULL)
       return;
    sock->DoStartNextPacket();
   } // CSocketThread::OnStartNextPacket

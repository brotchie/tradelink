// ServerSocket.cpp : implementation file
//
// Rewritten to Best Practice by Joseph M. Newcomer, Mar 2007

#include "stdafx.h"
#include "ServerSocket.h"
#include "Convert.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CServerSocket

CServerSocket::CServerSocket()
{
}

CServerSocket::~CServerSocket()
{
}


// Do not edit the following lines, which are needed by ClassWizard.
#if 0
BEGIN_MESSAGE_MAP(CServerSocket, CAsyncSocket)
        //{{AFX_MSG_MAP(CServerSocket)
        //}}AFX_MSG_MAP
END_MESSAGE_MAP()
#endif  // 0

/////////////////////////////////////////////////////////////////////////////
// CServerSocket member functions

/****************************************************************************
*                        CServerSocket::ProcessReceive
* Inputs:
*       CByteArray & data: Raw data received
* Result: void
*       
* Effect: 
*       This processes the received data
****************************************************************************/

void CServerSocket::ProcessReceive(CByteArray & data)
    {
     SendDebugInfoToOwner(data);
     SendDataToOwner(data);
     
     CString s = ConvertReceivedDataToString(data);
     s = _T(">>>") + s + _T("<<<");

     //CByteArray msg;
     //ConvertStringToSendData(s, msg);
     //Send(msg, ::GetCurrentThreadId());
    } // CServerSocket::ProcessReceive

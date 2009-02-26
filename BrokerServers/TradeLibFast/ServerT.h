// ServerT.h : header file
//
// Written by Sridhar S Madhugiri
// of Microsoft Technical Support, Developer Support
// Copyright (c) 1997 Microsoft Corporation. All rights reserved.
// Rewritten to Best Practice by Joseph M. Newcomer, Mar 2007

#if !defined(AFX_SERVERT_H__B7C54BD2_A555_11D0_8996_00AA00B92B2E__INCLUDED_)
#define AFX_SERVERT_H__B7C54BD2_A555_11D0_8996_00AA00B92B2E__INCLUDED_

#include "SocketThread.h"
#include "ServerSocket.h"

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

/////////////////////////////////////////////////////////////////////////////
// CServerThread thread

class CServerThread : public CSocketThread
{
        DECLARE_DYNCREATE(CServerThread)
protected:
        CServerThread();           // protected constructor used by dynamic creation

// Attributes
public:

// Operations
protected:
        virtual CConnectSoc * GetSocket() { return &m_socket; }
        // CSocket derived class that handles the connection.
        CServerSocket m_socket;

// Overrides
        // ClassWizard generated virtual function overrides
        //{{AFX_VIRTUAL(CServerThread)
protected:
        virtual BOOL InitInstance();
        virtual int ExitInstance();
        //}}AFX_VIRTUAL

// Implementation
protected:
        virtual ~CServerThread();

        // Generated message map functions
        //{{AFX_MSG(CServerThread)
                // NOTE - the ClassWizard will add and remove member functions here.
        //}}AFX_MSG
        DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SERVERT_H__B7C54BD2_A555_11D0_8996_00AA00B92B2E__INCLUDED_)

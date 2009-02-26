#if !defined(AFX_SERVERSOCKET_H__50DD2836_D1A3_4D8E_B696_6B76684F6572__INCLUDED_)
#define AFX_SERVERSOCKET_H__50DD2836_D1A3_4D8E_B696_6B76684F6572__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// ServerSocket.h : header file
//

#include "Connects.h"

/////////////////////////////////////////////////////////////////////////////
// CServerSocket command target

class CServerSocket : public CConnectSoc
{
public:
        CServerSocket();
        virtual ~CServerSocket();
protected:
        virtual void ProcessReceive(CByteArray & data);
// Overrides
protected:
        // ClassWizard generated virtual function overrides
        //{{AFX_VIRTUAL(CServerSocket)
        //}}AFX_VIRTUAL

        // Generated message map functions
        //{{AFX_MSG(CServerSocket)
                // NOTE - the ClassWizard will add and remove member functions here.
        //}}AFX_MSG
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SERVERSOCKET_H__50DD2836_D1A3_4D8E_B696_6B76684F6572__INCLUDED_)

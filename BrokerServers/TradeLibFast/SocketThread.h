#pragma once
#include "ConnectS.h"

class CSocketThread : public CWinThread {
    protected:
       DECLARE_DYNCREATE(CSocketThread)
    public:
       CSocketThread();
       void Send(CByteArray & data) { CConnectSoc * sock = GetSocket();
                                      ASSERT(sock != NULL);
                                      if(sock != NULL)
                                         sock->Send(data, m_nThreadID);
                                    }
       void SetSocket(SOCKET h) { socket = h; }
       void SetTarget(CWnd * w) { target = w; }
    protected:
       virtual CConnectSoc * GetSocket() {ASSERT(FALSE); return NULL; } // implement in subclasses

       // Used to pass the socket handle from the main thread to this thread.
       SOCKET socket;
       // Target of messages which are posted
       CWnd * target;
       //{{AFX_VIRTUAL(CServerThread)
       virtual BOOL InitInstance();
       virtual int ExitInstance();
        //}}AFX_VIRTUAL
    protected:
       virtual ~CSocketThread();
        //{{AFX_MSG(CServerThread)
        // NOTE - the ClassWizard will add and remove member functions here.
        //}}AFX_MSG
       afx_msg void OnTermThread(WPARAM, LPARAM);
       afx_msg void OnSendData(WPARAM, LPARAM);
       afx_msg void OnStartNextPacket(WPARAM, LPARAM);
       DECLARE_MESSAGE_MAP()
};

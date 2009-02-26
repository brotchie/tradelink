#pragma once
#include "afxsock.h"
/////////////////////////////////////////////////////////////////////////////
// CConnectSoc command target

class CConnectSoc : public CAsyncSocket
{
// Attributes
public:

// Operations
public:
        CConnectSoc();
        virtual ~CConnectSoc();
        void SetTarget(CWnd * w) { target = w; }
        CWnd * GetTarget() { return target; }

        void SetInfoRequest(BOOL b) { WantPeerInfo = b; }
        BOOL GetInfoRequest() { return WantPeerInfo; }
        void Send(CByteArray & data, DWORD threadID);
        void TraceAttach();
        void DoClose();
        void DoSendData(WPARAM, LPARAM);
        void DoStartNextPacket();
protected:
        CByteArray m_SendBuff;
        INT_PTR bytesSent;
        CWnd * target;

        CList<CByteArray *, CByteArray *> Queue;
        void FlushQueue();
        
        BOOL Sending;
        CByteArray m_recvBuff;

        typedef enum {L0, L1, L2, L3, DATA, ERRORSTATE} ReceiveState;
        ReceiveState state;

        BOOL WantPeerInfo;

        union {
           int ByteCount;
           BYTE data[4];
        } Header;

        INT_PTR bytesRead;

        void AbortConnection(DWORD err);
        void SetDataState();
        CString GetPeerPrefix();
        void NotifyOwnerAboutPacket();
        void SendDataToOwner(CByteArray & data);
        void SendDebugInfoToOwner(CByteArray & data);
        void DoSendComplete();
        void StartNextPacket();

        virtual void ProcessReceive(CByteArray & ) PURE; // define in subclasses

        // ClassWizard generated virtual function overrides
        //{{AFX_VIRTUAL(CConnectSoc)
        protected:
        virtual void OnConnect(int nErrorCode);
        virtual void OnClose(int nErrorCode);
        virtual void OnReceive(int nErrorCode);
        virtual void OnSend(int nErrorCode);
        virtual int Receive(void* lpBuf, int nBufLen, int nFlags = 0);
        virtual int Send(const void* lpBuf, int nBufLen, int nFlags = 0);
        //}}AFX_VIRTUAL

        // Generated message map functions
        //{{AFX_MSG(CConnectSoc)
                // NOTE - the ClassWizard will add and remove member functions here.
        //}}AFX_MSG

// Implementation
protected:
        void            DoAsyncSendBuff();
};


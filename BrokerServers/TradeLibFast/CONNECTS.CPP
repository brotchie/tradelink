// ConnectS.cpp : implementation file
//
// Microsoft Technical Support, Developer Support
// Copyright (c) 1998 Microsoft Corporation. All rights reserved.
// Rewritten to Best Practice by Joseph M. Newcomer, Mar 2007


#include "stdafx.h"
#include "ConnectS.h"
#include "messages.h"
//#include "resource.h"
#include "TracePacket.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CConnectSoc

CConnectSoc::CConnectSoc() :
   state(L0),
   Sending(FALSE),
   target(NULL),
   WantPeerInfo(FALSE),
   bytesSent(0)
   {
   }

CConnectSoc::~CConnectSoc()
   {
   }


// Do not edit the following lines, which are needed by ClassWizard.
#if 0
BEGIN_MESSAGE_MAP(CConnectSoc, CAsyncSocket)
        //{{AFX_MSG_MAP(CConnectSoc)
        //}}AFX_MSG_MAP
END_MESSAGE_MAP()
#endif  // 0


/////////////////////////////////////////////////////////////////////////////
// CConnectSoc member functions

/****************************************************************************
*                            CConnectSoc::OnClose
* Inputs:
*       int nErrorCode:
* Result: void
*       
* Effect: 
*       Handles the close notfication from the socket
*       send WM_QUIT message to the thread containing the socket
*       to shutdown once the connection is closed.
****************************************************************************/

void CConnectSoc::OnClose(int nErrorCode) 
   {
    TRACE(_T("%s: CConnectSoc::OnClose(%d)\n"), AfxGetApp()->m_pszAppName, nErrorCode);
    
    if(nErrorCode != ERROR_SUCCESS)
       { /* report error */
        ASSERT(target != NULL);
        if(target != NULL)
           { /* send error */
            TRACE(_T("%s: CConnectSoc::OnClose: sent UWM_NETWORK_ERROR(%d, 0x%x)\n"), AfxGetApp()->m_pszAppName, nErrorCode, ::GetCurrentThreadId());
            target->PostMessage(UWM_NETWORK_ERROR, (WPARAM)nErrorCode, (LPARAM)::GetCurrentThreadId());
           } /* send error */
       } /* report error */

    CAsyncSocket::OnClose(nErrorCode);
    // passive close        
    DoClose();
   }

/****************************************************************************
*                         CConnectSoc::GetPeerPrefix
* Result: CString
*       A printable representation of the form "x.x.x.x [port]"
* Notes: 
*       if GetPeerName() returns an empty string, returns "?.?.?.? [?]"
****************************************************************************/

CString CConnectSoc::GetPeerPrefix()
    {
     CString ip;
     UINT port;
     GetPeerName(ip, port);
     if(ip.IsEmpty())
         { /* no name */
          return _T("?.?.?.? [?]");
         } /* no name */
     CString s;
     s.Format(_T("%s [%u]"), ip, port);
     return s;
    } // CConnectSoc::GetPeerPrefix

/****************************************************************************
*                      CConnectSoc::SendDebugInfoToOwner
* Inputs:
*       CByteArray & data: Data to send
* Result: void
*       
* Effect: 
*       Creates a debug trace and sends it to the parent window
****************************************************************************/

void CConnectSoc::SendDebugInfoToOwner(CByteArray & data)
    {
     if(WantPeerInfo)                                                           
        { /* send peer info */
         ASSERT(target != NULL);
         if(target != NULL)
            { /* can send */
#define INFO_LIMIT 63
             CString * info = new CString;                                      
             info->Format(_T("%s (%d)"), GetPeerPrefix(), (int)data.GetSize()); 
             for(int i = 0; i < min((int)data.GetSize(), INFO_LIMIT); i++)
                { /* add bytes */
                 CString t;
                 t.Format(_T(" %02x"), data[i]);
                 *info += t;
                } /* add bytes */

             TRACE(_T("%s: CConnectSoc::SendDebugInfoToOwner: sent UWM_INFO(%p, 0x%x)\n"), AfxGetApp()->m_pszAppName, info, ::GetCurrentThreadId());
             if(!target->PostMessage(UWM_INFO, (WPARAM)info, (LPARAM)::GetCurrentThreadId()))
                { /* failed */
                 ASSERT(FALSE);
                 delete info;
                } /* failed */
            } /* can send */
        } /* send peer info */
    } // CConnectSoc::SendDebugInfoToOwner

/****************************************************************************
*                          CConnectSoc::SendDataToOwner
* Result: void
*       
* Effect: 
*       This will send the received data to the owner of the thread
****************************************************************************/

void CConnectSoc::SendDataToOwner(CByteArray & data)
    {
     ASSERT(target != NULL);

     if(target != NULL)
        { /* can send */
         CByteArray * result = new CByteArray;
         result->Copy(data);            

         TRACE(_T("%s: CConnectSoc::SendDataToOwner: sent UWM_NETWORK_DATA(%p, 0x%x)\n"), AfxGetApp()->m_pszAppName, result, ::GetCurrentThreadId());
         if(!target->PostMessage(UWM_NETWORK_DATA, (WPARAM)result, (LPARAM)::GetCurrentThreadId()))
            { /* failed to send */
             ASSERT(FALSE);
             delete result;
            } /* failed to send */
        } /* can send */
    } // CConnectSoc::SendDataToOwner

/****************************************************************************
*                         CConnectSoc::NotifyOwnerAboutPacket
* Result: void
*       
* Effect: 
*       Handles the transmission of the data packet to the owner thread.
* Notes:
*       This allows for a partial transmission of the last packet after
*       a shutdown
****************************************************************************/

void CConnectSoc::NotifyOwnerAboutPacket()
    {
     if(Header.ByteCount > 0)
        { /* send partial packet */
         // This will truncate the buffer if we have a partial receipt
         m_recvBuff.SetSize(m_recvBuff.GetSize() - Header.ByteCount);
        } /* send partial packet */

     if(m_recvBuff.GetSize() == 0)
        return; // nothing to send

     ProcessReceive(m_recvBuff);

     m_recvBuff.SetSize(0);
     Header.ByteCount = 0;
    } // CConnectSoc::NotifyOwnerAboutPacket

/****************************************************************************
*                           CConnectSoc::OnReceive
* Inputs:
*       int nErrorCode: error code from dispatcher
* Result: void
*       
* Effect: 
*       Receives data of the form
*
*       +--+--+--+--+--+--+--+--+--+--...--+
*       |    len    |   data               |
*       +--+--+--+--+--+--+--+--+--+--...--+
*                    \__________ __________/
*                               V
*                              len bytes
*
*       The length is an integer length in Network Standard Byte Order
*       The data is len bytes long
****************************************************************************/

void CConnectSoc::OnReceive(int nErrorCode) 
   {
    TRACE(_T("%s: CConnectSoc::OnReceive(%d)\n"), AfxGetApp()->m_pszAppName, nErrorCode);

    if(nErrorCode != ERROR_SUCCESS)
       { /* had error */
        AbortConnection(nErrorCode);
        return;
       } /* had error */

	m_recvBuff.SetSize(1024);
    int nRead = Receive(m_recvBuff.GetData(),1024);
	if (nRead==SOCKET_ERROR)
	{
		AbortConnection(::GetLastError());
		return;
	}
	NotifyOwnerAboutPacket();
	return;

    switch(state)  
       { /* state */
        case ERRORSTATE: 
           break;
        case L0: 
           nRead = Receive(&Header.data[0], sizeof(int));
           switch(nRead)
              { /* L0 next state */
               case SOCKET_ERROR:
                  AbortConnection(::GetLastError());
                  break;            
               case 0:              
                  break;            
               case 1:              
                  state = L1;       
                  break;            
               case 2:              
                  state = L2;       
                  break;            
               case 3:              
                  state = L3;       
                  break;            
               case 4:              
                  SetDataState();   
                  break;            
              } /* L0 next state */ 
           break;                   
        case L1: // one byte already read  
           nRead = Receive(&Header.data[1], sizeof(int) - 1);  
           switch(nRead)            
              { /* L1 next state */ 
               case SOCKET_ERROR:   
                  AbortConnection(::GetLastError());
                  break;            
               case 0:              
                  break;            
               case 1:              
                  state = L2;       
                  break;            
               case 2:              
                  state = L3;       
                  break;            
               case 3:              
                  SetDataState();   
                  break;            
              } /* L1 next state */ 
           break;                   
        case L2:                    
           nRead = Receive(&Header.data[2], sizeof(int) - 2);
           switch(nRead)
              { /* L2 next state */
               case SOCKET_ERROR:  
                  AbortConnection(::GetLastError());
                  break;            
               case 0:              
                  break;            
               case 1:              
                  state = L3;       
                  break;            
               case 2:              
                  SetDataState();   
                  break;            
              } /* L2 next state */ 
           break;                   
        case L3:                    
           nRead = Receive(&Header.data[3], sizeof(int) - 3);
           switch(nRead)            
              { /* L3 next state */ 
               case SOCKET_ERROR:   
                  AbortConnection(::GetLastError());
                  break;            
               case 0:              
                  break;            
               case 1:              
                  SetDataState();   
                  break;            
              } /* L3 next state */ 
           break;                   
        case DATA:                  
           nRead = Receive(m_recvBuff.GetData() + bytesRead, Header.ByteCount);
           if(nRead == SOCKET_ERROR)       
              { /* unrecoverable error */ 
               AbortConnection(::GetLastError());         
               break;                     
              } /* unrecoverable error */ 
           Header.ByteCount -= nRead;     
           if(Header.ByteCount == 0)      
              { /* all read */            
               NotifyOwnerAboutPacket();
               state = L0; 
               break;      
              } /* all read */ 
           break;              
       } /* state */           
}

/****************************************************************************
*                             CConnectSoc::OnSend
* Inputs:
*       int nErrorCode:
* Result: void
*       
* Effect: 
*       It is possible to send data on the async connection
****************************************************************************/

void CConnectSoc::OnSend(int nErrorCode) 
   {
    TRACE(_T("%s: CConnectSoc:OnSend(%d)\n"), AfxGetApp()->m_pszAppName, nErrorCode);

    if(nErrorCode != ERROR_SUCCESS)
       { /* had error */
        AbortConnection(nErrorCode);
        return;
       } /* had error */

    DoAsyncSendBuff();      
    CAsyncSocket::OnSend(nErrorCode);
   }

/****************************************************************************
*                              CConnectSoc::Send
* Inputs:
*       CByteArray & data: Reference to data to send
*       DWORD threadID: thread ID of service thread
* Result: void
*       
* Effect: 
*       Makes a copy of the data and queues it for sending
* Notes:
*       This is called in the context of the originating thread, not
*       the service thread
****************************************************************************/

void CConnectSoc::Send(CByteArray & data, DWORD threadID)
    {
     CByteArray * packet = new CByteArray;
     ASSERT(packet != NULL);
     if(packet == NULL)
        return;
     packet->Copy(data);
     TRACE(_T("%s: CConnectSoc::Send: sent UWM_SEND_DATA(%p)\n"), AfxGetApp()->m_pszAppName, packet);
     if(!::PostThreadMessage(threadID, UWM_SEND_DATA, (WPARAM)packet,0))
        { /* post failed */
         ASSERT(FALSE);
         delete packet;
         return;
        } /* post failed */
    } // CConnectSoc::Send

/****************************************************************************
*                           CConnectSoc::DoSendData
* Inputs:
*       WPARAM: (WPARAM)(CByteArray *): Data to send
*       LPARAM: unused
* Result: LRESULT
*       Logically void, 0, always
* Effect: 
*       Either sends the packet or queues it up for later processign
****************************************************************************/

void CConnectSoc::DoSendData(WPARAM wParam, LPARAM)
    {
     CByteArray * data = (CByteArray *)wParam;
     ASSERT(data != NULL);
     if(data == NULL)
        return; // can't do it, get out

     TRACE(_T("%s: CConnectSoc::DoSendData(%p),0) [%d]\n"), AfxGetApp()->m_pszAppName, data, (int)data->GetSize());

     Queue.AddTail(data);

     if(!Sending)
         StartNextPacket();
    } // CConnectSoc::DoSendData

/****************************************************************************
*                        CConnectSoc::StartNextPacket
* Result: void
*       
* Effect: 
*       Starts the next packet in the queue.  Does nothing if the queue is empty
*       except clear the Sending flag
****************************************************************************/

void CConnectSoc::StartNextPacket()
    {
     TRACE(_T("%s: CConnectSoc::StartNextPacket()\n"), AfxGetApp()->m_pszAppName);
     if(Queue.IsEmpty())
        { /* nothing to send */
         TRACE(_T("%s: CConnectSoc::StartNextPacket: Queue empty\n"), AfxGetApp()->m_pszAppName);
         Sending = FALSE;
         return;
        } /* nothing to send */

     CByteArray * data = Queue.RemoveHead();
     
     try {
          m_SendBuff.SetSize(data->GetSize() + sizeof(UINT));
         }
     catch(CMemoryException * e)
        { /* memory error */
#ifdef _DEBUG
         TCHAR msg[MAX_PATH];
         e->GetErrorMessage(msg, MAX_PATH);
         TRACE(_T("%s: Allocation error: %s\n"), AfxGetApp()->m_pszAppName, msg);
#endif
         e->Delete();
         // Lump all storage errors into "out of memory"
         AbortConnection(ERROR_OUTOFMEMORY);
         return;
        } /* memory error */
     UINT len = (UINT)data->GetSize();
     len = htonl(len);
     memcpy(m_SendBuff.GetData(), &len, sizeof(int));
     memcpy(m_SendBuff.GetData() + sizeof(int), data->GetData(), data->GetSize());
     bytesSent = 0;
     Sending = TRUE;
     delete data;
     DoAsyncSendBuff();
    } // CConnectSoc::StartNextPacket

/****************************************************************************
*                         CConnectSoc::DoSendComplete
* Result: void
*       
* Effect: 
*       A packet has been completely sent.  Clear the send-in-progress flag
*       and notify the owner
****************************************************************************/

void CConnectSoc::DoSendComplete()
    {
     if(!Sending)
        return;
     ASSERT(target != NULL);

     if(target != NULL)
        { /* send complete */
         TRACE(_T("%s: CConnectSoc::DoSendComplete: sent UWM_SEND_COMPLETE(0, 0x%x)\n"), AfxGetApp()->m_pszAppName, ::GetCurrentThreadId());
         target->PostMessage(UWM_SEND_COMPLETE, 0, (LPARAM)::GetCurrentThreadId());
        } /* send complete */
     TRACE(_T("%s: CConnectSoc::DoSendComplete: sent UWM_START_NEXT_PACKET\n"), AfxGetApp()->m_pszAppName);
     ::PostThreadMessage(::GetCurrentThreadId(), UWM_START_NEXT_PACKET, 0, 0);
    } // CConnectSoc::DoSendComplete

/****************************************************************************
*                       CConnectSoc::DoStartNextPacket
* Result: void
*       
* Effect: 
*       Starts the next packet
****************************************************************************/

void CConnectSoc::DoStartNextPacket()
    {
     StartNextPacket();
    } // CConnectSoc::DoStartNextPacket

/****************************************************************************
*                        CConnectSoc::DoAsyncSendBuff
* Result: void
*       
* Effect: 
*       This sends the next sequence of bytes in the data buffer
****************************************************************************/

void CConnectSoc::DoAsyncSendBuff()
   {
    TRACE(_T("%s: CConnectSoc::DoAsyncSendBuff() bytesent=%d\n"), AfxGetApp()->m_pszAppName, bytesSent);

    if(bytesSent == m_SendBuff.GetSize())
       { /* all sent */
        DoSendComplete();
        TRACE(_T("%s: CConnectSoc::DoAsyncSendBuff(): nothing to send\n"), AfxGetApp()->m_pszAppName);
        return;
       } /* all sent */

    ASSERT(bytesSent >= 0);

    LPBYTE p = m_SendBuff.GetData() + bytesSent;
    INT_PTR len = (int)(m_SendBuff.GetSize() - bytesSent);
    TRACE(_T("%s: ConnectSoc::Send(%p + %d (=%p),  %d - %d (=%d), 0)\n"),
                   AfxGetApp()->m_pszAppName,
                   m_SendBuff.GetData(), bytesSent, p,
                   (int)m_SendBuff.GetSize(),
                   (int)bytesSent,
                   len);

    INT_PTR result = Send(p, (int)len, 0);
    if(result == SOCKET_ERROR)
       { /* failed */
        DWORD err = ::GetLastError();
        if(err != WSAEWOULDBLOCK)
           { /* couldn't send */
            AbortConnection(err);
            return; // done for now, wait for next call
           } /* couldn't send */
       } /* failed */
    else
        { /* succeeded */
         TRACE(_T("%s : Send()=>%d\n"), AfxGetApp()->m_pszAppName, result);
         bytesSent += result;
         if(bytesSent == m_SendBuff.GetSize())
            DoSendComplete();
        } /* succeeded */
   } // CConnectSoc::DoAsyncSendBuff

/****************************************************************************
*                            CConnectSoc::Receive
* Inputs:
*       LPVOID Buf: Pointer to buffer segment to fill
*       int nBufLen: Buffer segment length
*       int nFlags: Flags
* Result: int
*       Number of bytes received
* Effect: 
*       Receives the data
****************************************************************************/

int CConnectSoc::Receive(LPVOID Buf, int nBufLen, int nFlags) 
   {
    TRACE(_T("%s: CConnectSoc::Receive(%p, %d, %x)\n"), AfxGetApp()->m_pszAppName, Buf, nBufLen, nFlags);
    int result = CAsyncSocket::Receive(Buf, nBufLen, nFlags);
    TRACE(_T("%s: CConnectSoc::Receive() got %d bytes\n"), AfxGetApp()->m_pszAppName, result);
#ifdef _DEBUG
    TracePacket((const LPBYTE)Buf, result);
#endif
    return result;
   }

/****************************************************************************
*                              CConnectSoc::Send
* Inputs:
*       LPCVOID Buf: Buffer to send
*       int nBufLen: Length to send
*       int nFlags:
* Result: int
*       Number of bytes sent
* Effect: 
*       Sends the bytes
****************************************************************************/

int CConnectSoc::Send(LPCVOID Buf, int nBufLen, int nFlags) 
   {
    TRACE(_T("%s: CConnectSoc::Send(%p, %d, %x)\n"), AfxGetApp()->m_pszAppName, Buf, nBufLen, nFlags);
#ifdef _DEBUG
    TracePacket((const LPBYTE)Buf, nBufLen);
#endif
    return CAsyncSocket::Send(Buf, nBufLen, nFlags);
   }

/****************************************************************************
*                        CAsyncSocket::AbortConnection
* Inputs:
*       DWORD err: Error code.  If other than ERROR_SUCCESS, sends an error
*                       notification to the owner
* Result: void
*       
* Effect: 
*       Aborts the socket connection after an error
****************************************************************************/

void CConnectSoc::AbortConnection(DWORD err)
    {                              
     if(err != ERROR_SUCCESS)
        { /* report error */
         ASSERT(target != NULL);
         if(target != NULL)
            { /* send error */
             TRACE(_T("%s: CConnectSoc::AbortConnection: sent UWM_NETWORK_ERROR(%d, 0x%x)\n"), AfxGetApp()->m_pszAppName, err, ::GetCurrentThreadId());
             target->PostMessage(UWM_NETWORK_ERROR, (WPARAM)err, (LPARAM)::GetCurrentThreadId());
            } /* send error */
        } /* report error */
     NotifyOwnerAboutPacket();
     state = ERRORSTATE;
     DoClose();                      
    } // CConnectSoc::AbortConnection

/****************************************************************************
*                          CConnectSoc::SetDataState
* Result: void
*       
* Effect: 
*       Sets DATA state. Converts the byte count from Network Standard
*       Byte Order to Host Byte Order
****************************************************************************/

void CConnectSoc::SetDataState()
    {                                     
     Header.ByteCount = ntohl(Header.ByteCount);
     TRACE(_T("%s: CConnectSoc::SetDataState: ByteCount %d (0x%08x)\n"), AfxGetApp()->m_pszAppName, Header.ByteCount, Header.ByteCount);
     m_recvBuff.SetSize(Header.ByteCount);
     bytesRead = 0;                       
     state = DATA;                        
    } // CConnectSoc::SetDataState        

/****************************************************************************
*                             CConnectSoc::DoClose
* Result: void
*       
* Effect: 
*       Closes the socket
****************************************************************************/

void CConnectSoc::DoClose()
    {
     FlushQueue();
     TRACE(_T("%s: CConnectSoc::DoClose\n"), AfxGetApp()->m_pszAppName);
     if(m_hSocket == INVALID_SOCKET)
        { /* not connected */
         ::PostQuitMessage(0);
         return;
        } /* not connected */
     
     ShutDown();
     Close();

     ASSERT(target != NULL);
     if(target != NULL)
        { /* send notification */
         TRACE(_T("%s: CConnectSoc::DoClose: sent UWM_CONNECTIONCLOSE(0, 0x%x)\n"), AfxGetApp()->m_pszAppName, ::GetCurrentThreadId());
         target->PostMessage(UWM_CONNECTIONCLOSE, 0, (LPARAM)::GetCurrentThreadId());
        } /* send notification */

     if(WantPeerInfo)
        { /* show close notice */
         CString closed;
         //closed.LoadString(IDS_CLOSED);
         ASSERT(target != NULL);
         if(target != NULL)
            { /* send it */
             CString * s = new CString;
             s->Format(_T("%s %s"), (LPCTSTR)GetPeerPrefix(), closed);
             TRACE(_T("%s: CConnectSoc::DoClose: sent UWM_INFO(%p, 0x%x)\n"), AfxGetApp()->m_pszAppName, s, ::GetCurrentThreadId());
             if(!target->PostMessage(UWM_INFO, (WPARAM)s, (LPARAM)::GetCurrentThreadId()))
                { /* failed to send */
                 ASSERT(FALSE);
                 delete s;
                } /* failed to send */
            } /* send it */
        } /* show close notice */

     ::PostQuitMessage(0);
    } // CConnectSoc::DoClose

/****************************************************************************
*                      CConnectSoc::FlushQueue
* Result: void
*       
* Effect: 
*       Removes all pending buffers from the queue and frees their storage
****************************************************************************/

void CConnectSoc::FlushQueue()
    {
     while(!Queue.IsEmpty())
        { /* kill buffer */
         CByteArray * data = Queue.RemoveHead();
         TRACE(_T("%s: Purging buffer at %p\n"), AfxGetApp()->m_pszAppName, data);
         delete data;
        } /* kill buffer */
    } // CConnectSoc::FlushQueue

/****************************************************************************
*                           CConnectSoc::OnConnect
* Inputs:
*       int nErrorCode: error code, ERROR_SUCCESS if connected
* Result: void
*       
* Effect: 
*       Notifies the main window that the connection has completed
****************************************************************************/

void CConnectSoc::OnConnect(int nErrorCode) 
   {
    TRACE(_T("%s: CConnectSoc::OnConnect(%d)\n"), AfxGetApp()->m_pszAppName, nErrorCode);

    if(nErrorCode != ERROR_SUCCESS)
        { /* failed */
         AbortConnection(nErrorCode);
         return;
        } /* failed */

    CAsyncSocket::OnConnect(nErrorCode);

    ASSERT(target != NULL);
    if(target != NULL)
       { /* connection made */
        TRACE(_T("%s: CConnectSoc::OnConnect: sent UWM_CONNECTIONMADE(0, 0x%x)\n"), AfxGetApp()->m_pszAppName, ::GetCurrentThreadId());
        target->PostMessage(UWM_CONNECTIONMADE, 0, (LPARAM)::GetCurrentThreadId());
       } /* connection made */
   }

/****************************************************************************
*                          CConnectSoc::TraceAttach
* Result: void
*       
* Effect: 
*       Emits a trace message if requested
****************************************************************************/

void CConnectSoc::TraceAttach()
    {
     if(WantPeerInfo)
        { /* show attach notice */
         ASSERT(target != NULL);
         if(target != NULL)
            { /* can send */
             CString attached;
//             attached.LoadString(IDS_ATTACHED);

             CString * s = new CString;
             s->Format(_T("%s %s"), (LPCTSTR)GetPeerPrefix(), attached);
             TRACE(_T("%s: CConnectSoc::TraceAttach: sent UWM_INFO(%p, 0x%x)\n"), AfxGetApp()->m_pszAppName, s, ::GetCurrentThreadId());
             if(!target->PostMessage(UWM_INFO, (WPARAM)s, (LPARAM)::GetCurrentThreadId()))
                { /* send failed */
                 ASSERT(FALSE);
                 delete s;
                } /* send failed */
            } /* can send */
        } /* show close notice */
    } // CConnectSoc::TraceAttach

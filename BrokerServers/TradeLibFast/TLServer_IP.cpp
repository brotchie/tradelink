#include "StdAfx.h"
#include "TLServer_IP.h"

TLServer_IP::TLServer_IP(void)
{
	       // Just specify input PORT#, local machine is assumed

       //BOOL bRet = Create(9122,SOCK_DGRAM,FD_READ);
	BOOL bRet = Create(9122,SOCK_STREAM,FD_READ);
	// BOOL bRet = Create(0,SOCK_DGRAM,FD_WRITE);
	bRet &= Create(0,SOCK_STREAM,FD_WRITE);


       if (bRet != TRUE)

       {

              UINT uErr = GetLastError();

       }
}

TLServer_IP::~TLServer_IP(void)
{
}

void TLServer_IP::OnReceive(int Error)
{
	static int i=0;
   i++;

   TCHAR buff[1024];
   int nRead;
   CString strSendersIp;
   UINT uSendersPort;

   // Could use Receive here if you don’t need the senders address & port
   nRead = ReceiveFromEx(buff, 1024, strSendersIp, uSendersPort); 

   switch (nRead)
   {
   case 0:       // Connection was closed.
      Close();      
      break;
   case SOCKET_ERROR:
      if (GetLastError() != WSAEWOULDBLOCK) 
      {
         AfxMessageBox (_T("Error occurred"));
         Close();
      }
      break;
   default: // Normal case: Receive() returned the # of bytes received.
      buff[nRead] = 0; //terminate the string (assuming a string for this example)
      CString strReceivedData(buff);       // This is the input data    
   }
   CAsyncSocket::OnReceive(Error);
}


void TLServer_IP::OnSend(int nErrorCode)

{
   m_bReadyToSend = true;    // The socket is now ready to send
   CAsyncSocket::OnSend(nErrorCode);
}

bool TLServer_IP::Send(const void* lpBuf, int nBufLen)
{
       if ( ! m_bReadyToSend )
              return(false);
       m_bReadyToSend = false;
       int dwBytes;
       CAsyncSocket *paSocket = this;
       // Specify destination here (IP number obscured - could use computer name instead)

       if ((dwBytes = CAsyncSocket::SendToEx((LPCTSTR)lpBuf,nBufLen,9122,_T("172.XX.XX.XXX"))) == SOCKET_ERROR)
       {

              UINT uErr = GetLastError();

              if (uErr != WSAEWOULDBLOCK) 

              {
					AfxMessageBox (_T("Error occurred"));
              }
              return(false);
       }
       return(true);

}

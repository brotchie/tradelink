#include "StdAfx.h"
#include "TLServer_IP.h"

// AsyncServerDlg.cpp : implementation file
// Rewritten to Best Practice by Joseph M. Newcomer, Mar 2007
//

#include "messages.h"
#include "port.h"
#include "Convert.h"
#include "UnicodeFont.h"
#include "ErrorString.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// TLServer_IP dialog

/****************************************************************************
*                      TLServer_IP::TLServer_IP
* Inputs:
*       CWnd * parent:
* Effect: 
*       Constructor
****************************************************************************/

TLServer_IP::TLServer_IP() :
        m_open(0),
        m_close(0),
        m_running(FALSE),
        m_MainWndIsClosing(FALSE)
{
        //{{AFX_DATA_INIT(TLServer_IP)
                // NOTE: the ClassWizard will add member initialization here
        //}}AFX_DATA_INIT
        // Note that LoadIcon does not require a subsequent DestroyIcon in Win32
        m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}


BEGIN_MESSAGE_MAP(TLServer_IP, CWnd)
        //{{AFX_MSG_MAP(TLServer_IP)
        ON_WM_SYSCOMMAND()
        ON_WM_PAINT()
        ON_WM_QUERYDRAGICON()
        ON_WM_CLOSE()
        ON_BN_CLICKED(IDC_CLOSE, OnBnClickedClose)
        ON_WM_SIZE()
        ON_BN_CLICKED(IDC_RUN, OnRun)
        ON_EN_CHANGE(IDC_PORT, OnChangePort)
        //}}AFX_MSG_MAP
        ON_MESSAGE(UWM_THREADSTART, OnThreadStart)
        ON_MESSAGE(UWM_THREADCLOSE, OnThreadClose)
        ON_MESSAGE(UWM_NETWORK_DATA, OnNetworkData)    
        ON_MESSAGE(UWM_INFO, OnInfo)
        ON_MESSAGE(UWM_NETWORK_ERROR, OnNetworkError)
        ON_MESSAGE(UWM_SEND_COMPLETE, OnSendComplete)
END_MESSAGE_MAP()


/****************************************************************************
*                       TLServer_IP::CleanupThreads                      
* Result: BOOL                                                               
*       TRUE if there were no threads running                                
*       FALSE if threads are being shut down                                 
* Effect:                                                                    
*       Initiates thread shutdown                                            
****************************************************************************/

BOOL TLServer_IP::CleanupThreads() 
   {
    INT_PTR size = m_threadIDs.GetSize();
    if(size == 0)                      
       return TRUE;                    

    for (INT_PTR i = 0; i < size; i++)
       { /* scan threads */
        if (!::PostThreadMessage(m_threadIDs[i], UWM_TERM_THREAD, 0, 0))
           { /* failed */
            TRACE(_T("%s: Thread 0x%02x possibly already terminated\n"), AfxGetApp()->m_pszAppName, m_threadIDs[i]);
            m_threadIDs.RemoveAt(i);
           } /* failed */
       } /* scan threads */

    // Note that if PostThreadMessage has failed and all the target threads have
    // been removed from the array, we are done

    if(m_threadIDs.GetSize() == 0)
       return TRUE;
    
    return FALSE;
}

/////////////////////////////////////////////////////////////////////////////
// TLServer_IP message handlers

/****************************************************************************
*                       TLServer_IP::CreateListener
* Result: BOOL
*       TRUE if socket created
*       FALSE if error
* Effect: 
*       Creates a listener socket
****************************************************************************/

BOOL TLServer_IP::CreateListener()
    {
     // Create the listener socket
     CString port;
     c_Port.GetWindowText(port);
     UINT portnum = _tcstoul(port, NULL, 0);

     if(portnum == 0)
        portnum = PORT_NUM;

     if(!m_listensoc.Create(portnum))
        { /* failed to create */
         DWORD err = ::GetLastError();
         CString fmt;
         fmt.LoadString(IDS_LISTEN_CREATE_FAILED);
         CString * s = new CString;
         s->Format(fmt, portnum);
         PostMessage(UWM_INFO, (WPARAM)s, ::GetCurrentThreadId());
         PostMessage(UWM_NETWORK_ERROR, (WPARAM)err, ::GetCurrentThreadId());
         return FALSE;
        } /* failed to create */

     { /* success */
      CString fmt;
      fmt.LoadString(IDS_LISTENER_CREATED);
      CString * s = new CString;
      s->Format(fmt, portnum);
      PostMessage(UWM_INFO, (WPARAM)s,::GetCurrentThreadId());
     } /* success */

     m_listensoc.SetTarget(this);

     m_listensoc.Listen();
     return TRUE;
    } // TLServer_IP::CreateListener


/****************************************************************************
*                        TLServer_IP::OnSysCommand
* Inputs:
*       UINT nID: Menu ID
*       LPARAM lParam: unused, pass to superclass
* Result: void
*       
* Effect: 
*       Processes a system menu click
****************************************************************************/

void TLServer_IP::OnSysCommand(UINT nID, LPARAM lParam)
{
	CWnd::OnSysCommand(nID, lParam);
}

/****************************************************************************
*                          TLServer_IP::OnPaint
* Result: void
*       
* Effect: 
*       If you add a minimize button to your dialog, you will need the code
*       below to draw the icon.  For MFC applications using the document/view
*       model, this is automatically done for you by the framework.
****************************************************************************/

void TLServer_IP::OnPaint() 
{
        if (IsIconic())
        {
                CPaintDC dc(this); // device context for painting

                SendMessage(WM_ICONERASEBKGND, (WPARAM) dc.GetSafeHdc(), 0);

                // Center icon in client rectangle
                int cxIcon = GetSystemMetrics(SM_CXICON);
                int cyIcon = GetSystemMetrics(SM_CYICON);
                CRect rect;
                GetClientRect(&rect);
                int x = (rect.Width() - cxIcon + 1) / 2;
                int y = (rect.Height() - cyIcon + 1) / 2;

                // Draw the icon
                dc.DrawIcon(x, y, m_hIcon);
        }
        else
        {
                CWnd::OnPaint();
        }
}

/****************************************************************************
*                      TLServer_IP::OnQueryDragIcon
* Result: HCURSOR
*       Handle of cursor to use as drag icon
* Effect: 
*       The system calls this to obtain the cursor to display while the user
*       drags the minimized window.
****************************************************************************/

HCURSOR TLServer_IP::OnQueryDragIcon()
{
        return (HCURSOR) m_hIcon;
}

/****************************************************************************
*                       TLServer_IP::OnThreadStart
* Inputs:
*       WPARAM unused
*       LPARAM lParam: (LPARAM)(DWORD) the thread ID
* Result: LRESULT
*       Logically void, 0, always
* Effect: 
*       Increments the open-thread count.  Adds the thread ID to the
*       array of thread IDs
****************************************************************************/

LRESULT TLServer_IP::OnThreadStart(WPARAM, LPARAM lParam)
   {
    m_open++;

    DWORD ThreadID = (DWORD)lParam;
    m_threadIDs.Add(ThreadID);  // save thread refs so we can post msg to quit later

    // Update the display
    ShowOpenConnections();
    return 0;
   }

/****************************************************************************
*                    TLServer_IP::ShowOpenConnections
* Result: void
*       
* Effect: 
*       Shows the number of open connections in the dialog control  
****************************************************************************/

void TLServer_IP::ShowOpenConnections()
    {
     CString s;
     s.Format(_T("%d"), m_open);
     c_OpenConnections.SetWindowText(s);
    } // TLServer_IP::ShowOpenConnections

/****************************************************************************
*                   TLServer_IP::ShowClosedConnections
* Result: void
*       
* Effect: 
*       Displays the count of closed connections
****************************************************************************/

void TLServer_IP::ShowClosedConnections()
    {
     CString s;
     s.Format(_T("%d"), m_close);
     c_ClosedConnections.SetWindowText(s);
    } // TLServer_IP::ShowClosedConnections

/****************************************************************************
*                       TLServer_IP::OnThreadClose
* Inputs:
*       WPARAM unused
*       LPARAM: lParam: (LPARAM)(DWORD) thread ID
* Result: LRESULT
*       Logically void, 0, always
* Effect: 
*       Records that a thread has closed.  If a thread has closed and
*       a close operation has been deferred, completes the close transaction
****************************************************************************/

LRESULT TLServer_IP::OnThreadClose(WPARAM, LPARAM lParam)
   {
    m_open--;
    m_close++;
        
    ShowOpenConnections();

    ShowClosedConnections();
        
    // remove dwThreadID from m_threadIDs
    DWORD dwThreadID = (DWORD)lParam;
        
    INT_PTR size = m_threadIDs.GetSize();
    for (INT_PTR i = 0; i < size; i++)
       { /* scan threads */
        if (m_threadIDs[i] ==  dwThreadID)
           {
            m_threadIDs.RemoveAt(i);
            TRACE(_T("%s: Thread 0x%02x is removed\n"), AfxGetApp()->m_pszAppName, dwThreadID);
            break;
           }
       } /* scan threads */

   // If we got a WM_CLOSE and there were open threads,  
   // we deferred the close until all the threads terminated
   // Now we try again  
    if(m_MainWndIsClosing && m_open == 0) 
       PostMessage(WM_CLOSE);  // try the close again
    return 0;
   }

/****************************************************************************
*                        TLServer_IP::OnNetworkData
* Inputs:
*       WPARAM: (WPARAM)(CByteArray *) The data to display
*       LPARAM: (LPARAM)(DWORD) the thread ID of the thread that sent it
* Result: LRESULT
*       Logically void, 0, always
* Effect: 
*       Displays the string received
* Notes:
*       Deletes the string
****************************************************************************/

LRESULT TLServer_IP::OnNetworkData(WPARAM wParam, LPARAM lParam)
   {
    // a new message has been received. Update the UI
    CByteArray * data = (CByteArray *)wParam;
    
    CString s = ConvertReceivedDataToString(*data);
    c_LastString.SetWindowText(s);

    CString msg;
#define SERVER_STRING_INFO_LIMIT 63
    msg.Format(_T("%x: => \"%-*s\""), (DWORD)lParam, SERVER_STRING_INFO_LIMIT, s);
    c_Record.AddString(msg);
    delete data;

    return 0;
   }

/****************************************************************************
*                          TLServer_IP::OnClose
* Result: void
*       
* Effect: 
*       The program has received a WM_CLOSE message and needs to shut down,
*       so this initiates a shutdown
****************************************************************************/

void TLServer_IP::OnClose() 
   {
    m_MainWndIsClosing = TRUE;
    updateControls();

    if(!CleanupThreads())
       { /* threads running */
        TRACE(_T("%s: TLServer_IP::OnClose: deferring close\n"), AfxGetApp()->m_pszAppName);
        return;
       } /* threads running */

    //CWnd::OnOK();
   }

/****************************************************************************
*                            TLServer_IP::OnOK
*                          TLServer_IP::OnCancel
* Result: void
*       
* Effect: 
*       Does nothing.  This is so that an <enter> key will not accidentally
*       terminate the program
****************************************************************************/

void TLServer_IP::OnOK() 
   {
   }

void TLServer_IP::OnCancel() 
   {
   }

/****************************************************************************
*                       TLServer_IP::updateControls                      
* Result: void                                                               
*                                                                            
* Effect:                                                                    
*       Updates the controls                                                 
****************************************************************************/

void TLServer_IP::updateControls()
    {
     CString port;
     c_Port.GetWindowText(port);
     UINT portnum = _tcstoul(port, NULL, 0);
     c_Run.EnableWindow(!m_running && portnum > 1023 && portnum <= 65535);

     c_Port.SetReadOnly(m_running);
     x_Port.EnableWindow(!m_running);
     // If we are shutting down, disable the entire dialog!
     EnableWindow(!m_MainWndIsClosing);
    } // TLServer_IP::updateControls

/****************************************************************************
*                      TLServer_IP::OnBnClickedClose
* Result: void
*       
* Effect: 
*       The Close button on the dialog has been hit.  Initiate a shutdown
*       of the app
****************************************************************************/

void TLServer_IP::OnBnClickedClose()
   {
    PostMessage(WM_CLOSE);
   }

/****************************************************************************
*                           TLServer_IP::OnSize
* Inputs:
*       UINT nType:
*       int cx:
*       int cy:
* Result: void
*       
* Effect: 
*       Resizes the listbox
****************************************************************************/

void TLServer_IP::OnSize(UINT nType, int cx, int cy) 
   {
    CWnd::OnSize(nType, cx, cy);
        
    if(c_Record.GetSafeHwnd() != NULL)
       { /* resize it */
        CRect r;
        c_Record.GetWindowRect(&r);
        ScreenToClient(&r);
        c_Record.SetWindowPos(NULL, 0, 0,
                              cx - r.left,
                              cy - r.top,
                              SWP_NOMOVE | SWP_NOZORDER);
       } /* resize it */
   }

/****************************************************************************
*                           TLServer_IP::OnInfo
* Inputs:
*       WPARAM: (WPARAM)(CString *) String to display
*       LPARAM: (LPARAM)(DWORD) thread id of thread providing info
* Result: LRESULT
*       Logically void, 0, always
* Effect: 
*       Logs the info string
****************************************************************************/

LRESULT TLServer_IP::OnInfo(WPARAM wParam, LPARAM lParam)
    {
     CString * s = (CString *)wParam;
     CString id;
     id.Format(_T("%x: "), (DWORD)lParam);
     *s = id + *s;
     c_Record.AddString(*s);
     delete s;

     return 0;
    } // TLServer_IP::OnInfo

/****************************************************************************
*                           TLServer_IP::OnRun
* Result: void
*       
* Effect: 
*       Creates the listener socket
****************************************************************************/

void TLServer_IP::OnRun() 
   {
    m_running = CreateListener();
    updateControls();
   }

/****************************************************************************
*                        TLServer_IP::OnChangePort
* Result: void
*       
* Effect: 
*       Updates the controls based on the port # changing
****************************************************************************/

void TLServer_IP::OnChangePort() 
   {
    updateControls();
   }

/****************************************************************************
*                       TLServer_IP::OnNetworkError
* Inputs:
*       WPARAM: (WPARAM)(DWORD) error code
*       LPARAM: (LPARAM)(DWORD) thread that issued the error
* Result: LRESULT
*       Logically void, 0, always
* Effect: 
*       Records the error
****************************************************************************/

LRESULT TLServer_IP::OnNetworkError(WPARAM wParam, LPARAM lParam)
    {
     DWORD err = (DWORD)wParam;
     CString msg = ErrorString(err);
     CString id;
     id.Format(_T("%x: "), (DWORD)lParam);
     c_Record.AddString(id + msg);
     return 0;
    } // TLServer_IP::OnNetworkError

/****************************************************************************
*                       TLServer_IP::OnSendComplete
* Inputs:
*       WPARAM: unused
*       LPARAM: (WPARAM)(DWORD) Thread ID of thread that sent data
* Result: LRESULT
*       Logically void, 0, always
* Effect: 
*       Logs the data-sent event
****************************************************************************/

LRESULT TLServer_IP::OnSendComplete(WPARAM, LPARAM lParam)
    {
     DWORD threadid = (DWORD)lParam;
     CString fmt;
     fmt.LoadString(IDS_DATA_SENT);
     CString s;
     s.Format(fmt, threadid);
     c_Record.AddString(s);
     return 0;
    } // TLServer_IP::OnSendComplete


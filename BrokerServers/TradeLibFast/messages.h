/****************************************************************************
*                               UWM_THREADSTART                              
* Inputs:                                                                    
*       WPARAM: unused                                                       
*       LPARAM: (WPARAM)(DWORD) Thread ID
* Result: LRESULT                                                            
*       Logically void, 0, always                                            
* Effect:                                                                    
*       Notifies the main GUI thread that the child thread has started       
****************************************************************************/
#define UWM_THREADSTART  (WM_APP+200)                                        

/****************************************************************************
*                               UWM_THREADCLOSE                              
* Inputs:                                                                    
*       WPARAM: unused                                                       
*       LPARAM: (LPARAM)(DWORD) thread ID of closing thread
* Result: LRESULT                                                            
*       Logically void, 0, always                                            
* Effect:                                                                    
*       Notifies the main thread that a secondary thread has terminated      
****************************************************************************/

#define UWM_THREADCLOSE  (WM_APP+201)                                        

/****************************************************************************
*                                UWM_NETWORK_DATA                               
* Inputs:                                                                    
*       WPARAM: (WPARAM)(CByteArray *) Pointer to the data that was received           
*       LPARAM: (LPARAM)(DWORD) thread ID of thread that received the data
* Result: LRESULT                                                            
*       Logically void, 0, always                                            
* Effect:                                                                    
*       Passes data to the main thread to be processed and/or displayed
* Notes:                                                                     
*       It is the responsibility of the recipient to delete the CString      
****************************************************************************/

#define UWM_NETWORK_DATA    (WM_APP+202)                                        

/****************************************************************************
*                               UWM_TERM_THREAD                              
* Inputs:                                                                    
*       WPARAM: unused                                                       
*       LPARAM: unused                                                       
* Result: LRESULT                                                            
*       Logically void, 0, always                                            
* Effect:                                                                    
*       Sent via PostThreadMessage to the child thread to shut it down       
****************************************************************************/

#define UWM_TERM_THREAD  (WM_APP+203)                                        


/****************************************************************************
*                            UWM_CONNECTIONCLOSE
* Inputs:
*       WPARAM: unused
*       LPARAM lParam: (LPARAM)(DWORD) thread ID of thread that closed connection
* Result: LRESULT
*       Logically void, 0, always
* Effect: 
*       Notifies the main thread that the connection has closed
****************************************************************************/

#define UWM_CONNECTIONCLOSE (WM_APP + 204)


/****************************************************************************
*                             UWM_CONNECTIONMADE
* Inputs:
*       WPARAM: unused
*       LPARAM: unused
* Result: LRESULT
*       Logically void, 0, always
* Effect: 
*       Notifies the main thread that a connection has succeeded
****************************************************************************/

#define UWM_CONNECTIONMADE (WM_APP + 205)

/****************************************************************************
*                                  UWM_INFO
* Inputs:
*       WPARAM: (WPARAM)(CString *) Connection information message
*       LPARAM: (LPARAM)(DWORD) thread ID of thread that is logging info
* Result: LRESULT
*       Logically void, 0, always
* Effect: 
*       Used to post trace info about message receipt 
* Notes:
*       This only applies when the socket is
*       set to do this (see CConnectSoc::SetInfoRequest)
*       The recipient is responsible for the deletion of the string
****************************************************************************/

#define UWM_INFO (WM_APP + 206)

/****************************************************************************
*                              UWM_NETWORK_ERROR
* Inputs:
*       WPARAM: (WPARAM)(DWORD) error code
*       LPARAM: (LPARAM)(DWORD) thread ID of thread that had the error
* Result: LRESULT
*       Logically void, 0, always
* Effect: 
*       Notifies the main thread that a network error has occurred
****************************************************************************/

#define UWM_NETWORK_ERROR (WM_APP+207)

/****************************************************************************
*                              UWM_SEND_COMPLETE
* Inputs:
*       WPARAM: unused
*       LPARAM: (LPARAM)(DWORD) thread ID of thread that completed sending
* Result: LRESULT
*       Logically void, 0, always
* Effect: 
*       Notifies the owner that the last Send has completed
****************************************************************************/

#define UWM_SEND_COMPLETE (WM_APP+208)

/****************************************************************************
*                                UWM_SEND_DATA
* Inputs:
*       WPARAM: (WPARAM)(CByteArray*) Reference to a CByteArray to be sent
*       LPARAM: unused
* Result: LRESULT
*       Logically void, 0, always
* Effect: 
*       Enqueues a send request
* Notes:
*       This is sent to the thread via PostThreadMessage
*       The sender must allocate the CByteArray on the heap.  The network
*       layers will delete this object when it has been sent
****************************************************************************/

#define UWM_SEND_DATA (WM_APP + 209)

/****************************************************************************
*                            UWM_START_NEXT_PACKET
* Inputs:
*       WPARAM: unused
*       LPARAM: unused
* Result: void
*       
* Effect: 
*       This is sent via PostThreadMessage from the network thread to itself
*       to initiate the asynchronous dequeue of the next pending packet
****************************************************************************/

#define UWM_START_NEXT_PACKET (WM_APP + 210)

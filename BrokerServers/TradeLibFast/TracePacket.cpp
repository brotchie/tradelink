#include "stdafx.h"
#include "TracePacket.h"


/****************************************************************************
*                                 TracePacket
* Inputs:
*       LPBYTE buf: Buffer whose contents are traced
*       UINT_PTR len: Length of buffer
* Result: void
*       
* Effect: 
*       Traces the data packet
****************************************************************************/

void TracePacket(const LPBYTE Buf, INT_PTR len)
   {
    INT_PTR i;  // declare here for compatibility with VS6
    TRACE(_T("%s: Packet [%d]\n"), AfxGetApp()->m_pszAppName, len);
    TRACE(_T("%s: "), AfxGetApp()->m_pszAppName);
    for(i = 0; i < min(len, 16); i++)
       { /* show data */
        TRACE(_T("%02x "), Buf[i]);
       } /* show data */

    if(len > 32)
       { /* tail */
        TRACE(_T("..."));
        for(INT_PTR i = len - 16; i < len; i++)
           TRACE(_T("%02x "), ((LPBYTE)Buf)[i]);
       } /* tail */
    TRACE(_T("\n"));

    TRACE(_T("%s: "), AfxGetApp()->m_pszAppName);
    for(i = 0; i < min(len, 16); i++)
       { /* show data */
        BYTE b = Buf[i];
        if(b < ' ')
           b = '.';
        CString t((char)b);
        TRACE(_T(" %s "), t);
       } /* show data */

    if(len > 32)
       { /* tail */
        TRACE(_T("..."));
        for(i = len - 16; i < len; i++)
           { /* show tail */
            BYTE b = Buf[i];
            if(b < ' ')
               b = '.';
            CString t((char)b);
            TRACE(_T(" %s "), t);
           } /* show tail */
       } /* tail */
    TRACE(_T("\n"));
} // TracePacket

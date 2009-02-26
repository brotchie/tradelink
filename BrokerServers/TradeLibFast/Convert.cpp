#include "stdafx.h"
#include "Convert.h"
#pragma warning(disable:4189) // allow DWORD err = ::GetLastError() even though err is not used

/****************************************************************************
*                          ConvertReceivedDataToString
* Inputs:
*       CByteArray & data: Raw data in UTF-8 format
* Result: CString
*       A string representing the data
* Effect: 
*       Converts the data from UTF-8 to ANSI or Unicode string
* Notes:
*       To convert to ANSI, it is first turned into Unicode using CP_UTF8
*       as the source page, then converted from Unicode to ANSI by using
*       CP_ACP as the target page
****************************************************************************/

CString ConvertReceivedDataToString(CByteArray & data)
   {
     // data is UTF-8 encoded
    CArray<WCHAR, WCHAR> wc;
     // First, compute the amount of space required.  n will include the
     // space for the terminal NUL character
    INT_PTR n = ::MultiByteToWideChar(CP_UTF8, 0, (LPCSTR)data.GetData(), (int)data.GetSize(), NULL, 0);
    if(n == 0)
       { /* failed */
        DWORD err = ::GetLastError();
        TRACE(_T("%s: MultiByteToWideChar (1) returned error %d\n"), AfxGetApp()->m_pszAppName, err);
        return CString(_T(""));
       } /* failed */
    else
       { /* success */
        wc.SetSize(n);
        n = ::MultiByteToWideChar(CP_UTF8, 0, (LPCSTR)data.GetData(), (int)data.GetSize(), (LPWSTR)wc.GetData(), (int)n);
        if(n == 0)
           { /* failed */
            DWORD err = ::GetLastError();
            TRACE(_T("%s: MultiByteToWideChar (2) returned error %d\n"), AfxGetApp()->m_pszAppName, err);
            return CString(_T(""));
           } /* failed */
       } /* success */     

     // Data is now in Unicode
     // If we are a Unicode app we are done
     // If we are an ANSI app, convert it back to ANSI

#ifdef _UNICODE
     // If this is a Unicode app we are done
    return CString(wc.GetData(), (int)wc.GetSize());
#else // ANSI
    // Invert back to ANSI
    CString s;
    n = ::WideCharToMultiByte(CP_ACP, 0, (LPCWSTR)wc.GetData(), (int)wc.GetSize(), NULL, 0, NULL, NULL);
    if(n == 0)
       { /* failed */
        DWORD err = ::GetLastError();
        TRACE(_T("%s: WideCharToMultiByte (1) returned error %d\n"), AfxGetApp()->m_pszAppName, err);
        return CString("");
       } /* failed */
    else
       { /* success */
        LPSTR p = s.GetBuffer((int)n);
        n = ::WideCharToMultiByte(CP_ACP, 0, wc.GetData(), (int)wc.GetSize(), p, (int)n, NULL, NULL);
        if(n == 0)
           { /* conversion failed */
            DWORD err = ::GetLastError();
            TRACE(_T("%s: WideCharToMultiByte (2) returned error %d\n"), AfxGetApp()->m_pszAppName, err);
            s.ReleaseBuffer();
            return CString("");
           } /* conversion failed */
        s.ReleaseBuffer();
        return s;
       } /* success */
#endif
   } // ConvertReceivedDataToString

/****************************************************************************
*                           ConvertStringToSendData
* Inputs:
*       const CString & s: String to send
*       CByteArray & msg: Place to format message
* Result: BOOL
*       TRUE if successful
*       FALSE if error
* Effect: 
*       Converts the data to a byte stream for transmission
****************************************************************************/

BOOL ConvertStringToSendData(const CString & s, CByteArray & msg)
    {
#ifdef _UNICODE
     int n = ::WideCharToMultiByte(CP_UTF8, 0, s, -1, NULL, 0, NULL, NULL);
     if(n == 0)
        { /* failed */
         //DWORD err = ::GetLastError();
         msg.SetSize(0);
         return FALSE;
        } /* failed */
     else
        { /* success */
         msg.SetSize(n);
         n = ::WideCharToMultiByte(CP_UTF8, 0, s, -1, (LPSTR)msg.GetData(), n, NULL, NULL);
         if(n == 0)
            { /* conversion failed */
             DWORD err = ::GetLastError();
             msg.SetSize(0);
             return FALSE;
            } /* conversion failed */
         else
            { /* use multibyte string */
             msg.SetSize(n - 1);
             return TRUE;
            } /* use multibyte string */
        } /* success */
#else // ANSI
     CArray<WCHAR, WCHAR> wc;

     int n = ::MultiByteToWideChar(CP_ACP, 0, s, -1, NULL, 0);
     if(n == 0)
        { /* failed */
         DWORD err = ::GetLastError();
         msg.SetSize(0);
         return FALSE;
        } /* failed */
     else
        { /* success */
         wc.SetSize(n);
         n = ::MultiByteToWideChar(CP_ACP, 0, s, -1, wc.GetData(), n);
        } /* success */     

     n = ::WideCharToMultiByte(CP_UTF8, 0, wc.GetData(), -1, NULL, 0, NULL, NULL);
     if(n == 0)
        { /* failed */
         DWORD err = ::GetLastError();
         msg.SetSize(0);
         return FALSE;
        } /* failed */
     else
        { /* success */
         msg.SetSize(n);
         n = ::WideCharToMultiByte(CP_UTF8, 0, wc.GetData(), -1, (LPSTR)msg.GetData(), n, NULL, NULL);
         if(n == 0)
            { /* conversion failed */
             DWORD err = ::GetLastError();
             msg.SetSize(0);
             return FALSE;
            } /* conversion failed */
         else
            { /* use multibyte string */
             msg.SetSize(n - 1);
             return TRUE;
            } /* use multibyte string */
        } /* success */
#endif
    } // ConvertStringToSendData

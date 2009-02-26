#include "stdafx.h"
#include "ErrorString.h"
#include "ASresource.h"


/****************************************************************************
*                                DefaultError
* Inputs:
*       DWORD err: Error code
* Result: CString
*       The default error string
****************************************************************************/

static CString DefaultError(DWORD err)
    {
     CString fmt;
     fmt.LoadString(IDS_DEFAULT_ERROR);
     CString s;
     s.Format(fmt, err, err);
     return s;
    } // DefaultError

/****************************************************************************
*                               ErrorString
* Inputs:
*       DWORD err: Error value
* Result: CString
*       Error string
* Effect: 
*       Converts the error to a WinSock error code
****************************************************************************/

CString ErrorString(DWORD err)
    {
     DWORD flags = 0;
     CString path;
     HMODULE lib = NULL;

     LPTSTR p = path.GetBuffer(MAX_PATH);
     HRESULT hr = ::SHGetFolderPath(NULL, CSIDL_SYSTEM, NULL, SHGFP_TYPE_CURRENT, p);
     if(SUCCEEDED(hr))
        { /* succeeded */
         path.ReleaseBuffer();
         if(path.Right(1) != _T("\\"))
            path += _T("\\");
         /*****************************************************************************
          * While this seems odd that this is hardwired to
          * what appears to be a 32-bit DLL, in fact the
          * 64-bit version is ALSO called WSOCK32.DLL as evidenced by
          * this dump from dumpbin
          *
          * Microsoft (R) COFF/PE Dumper Version 8.00.50727.42
          *     Copyright (C) Microsoft Corporation.  All rights reserved.
          * 
          *     Dump of file c:\windows\system32\wsock32.dll
          *
          *     PE signature found
          *
          *     File Type: DLL
          *
          *     FILE HEADER VALUES
          *     8664 machine (x64)  <===== Note x64 version!
          *     5 number of sections
          *     42438B57 time date stamp Thu Mar 24 23:53:59 2005
          *     0 file pointer to symbol table
          *     0 number of symbols
          *     F0 size of optional header
          *     2022 characteristics
          *     Executable
          *     Application can handle large (>2GB) addresses
          *     DLL
          *****************************************************************************/
         path += _T("WSOCK32.DLL");
         lib = ::LoadLibrary(path);
         if(lib != NULL)
            flags |= FORMAT_MESSAGE_FROM_HMODULE;
        } /* succeeded */
     else
        { /* failed */
         path.ReleaseBuffer();
        } /* failed */     

     LPTSTR msg;
     if(::FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER |
                        FORMAT_MESSAGE_FROM_SYSTEM |
                        flags,
                        (LPCVOID)lib,
                        err,
                        0, // language ID
                        (LPTSTR)&msg,
                        0, // size ignored
                        NULL) // arglist
                              == 0)
        { /* not found */
         return DefaultError(err);
        } /* not found */

     LPTSTR eol = _tcsrchr(msg, _T('\r'));
     if(eol != NULL)
        *eol = _T('\0');
     
     CString s = msg;
     
     if(lib != NULL)
        ::FreeLibrary(lib);
     
     LocalFree(msg);
     return s;
    } // ErrorString

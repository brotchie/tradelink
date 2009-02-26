// Listens.h : header file
//
// of Microsoft Technical Support, Developer Support
// Copyright (c) 1998 Microsoft Corporation. All rights reserved.
// Rewritten to Best Practice by Joseph M. Newcomer, Mar 2007

#if !defined(AFX_LISTENS_H__B7C54BD1_A555_11D0_8996_00AA00B92B2E__INCLUDED_)
#define AFX_LISTENS_H__B7C54BD1_A555_11D0_8996_00AA00B92B2E__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
#include "afxsock.h"
/////////////////////////////////////////////////////////////////////////////
// CListensoc command target

class CListensoc : public CAsyncSocket
{
// Attributes
public:

// Operations
public:
        CListensoc();
        virtual ~CListensoc();
        void SetTarget(CWnd * w) { target = w; }
// Overrides
protected:
        // ClassWizard generated virtual function overrides
        //{{AFX_VIRTUAL(CListensoc)
        protected:
        virtual void OnAccept(int nErrorCode);
        //}}AFX_VIRTUAL

        // Generated message map functions
        //{{AFX_MSG(CListensoc)
                // NOTE - the ClassWizard will add and remove member functions here.
        //}}AFX_MSG

// Implementation
protected:
   CDWordArray m_threadIDs;
   CWnd * target;
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_LISTENS_H__B7C54BD1_A555_11D0_8996_00AA00B92B2E__INCLUDED_)

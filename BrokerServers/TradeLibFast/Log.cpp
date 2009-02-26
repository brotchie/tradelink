// Log.cpp : implementation file
//

#include "stdafx.h"
#include "Log.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CLog

CLog::CLog()
{
}

CLog::~CLog()
{
}


BEGIN_MESSAGE_MAP(CLog, CListBox)
        //{{AFX_MSG_MAP(CLog)
                // NOTE - the ClassWizard will add and remove mapping macros here.
        //}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CLog message handlers

/****************************************************************************
*                               CLog::AddString
* Inputs:
*       const CString & s: String to add
* Result: int
*       Location where it was added
* Effect: 
*       Adds the string and scrolls the control
****************************************************************************/

int CLog::AddString(const CString & s)
    {
     CRect lastRect;
     int count = CListBox::GetCount();
     if(count > 0)
        CListBox::GetItemRect(count - 1, &lastRect);

     int n = CListBox::AddString(s);
     if(n < 0)
        return n;
     
     CRect c;
     CListBox::GetClientRect(&c);

     // If the last item was already off the window, don't scroll
     if(count > 0)
        { /* had items */
         CRect item;
         CListBox::GetItemRect(n, &item);
         if(lastRect.top > c.bottom)
            { /* was invisible */
             return n;
            } /* was invisible */
        } /* had items */

     while(TRUE)
        { /* scan and adjust */
         int top = CListBox::GetTopIndex();

         CRect item;
         CListBox::GetItemRect(n, &item);
         if(item.bottom > c.bottom)
            { /* hidden */
             CListBox::SetTopIndex(top + 1);
             continue;
            } /* hidden */
         break;
        } /* scan and adjust */
     return n;
    } // CLog::AddString

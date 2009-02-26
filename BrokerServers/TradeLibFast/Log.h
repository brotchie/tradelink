#if !defined(AFX_LOG_H__3D3E1DE5_49BE_42B5_95D7_0873F524FA64__INCLUDED_)
#define AFX_LOG_H__3D3E1DE5_49BE_42B5_95D7_0873F524FA64__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// Log.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CLog window

class CLog : public CListBox
{
// Construction
public:
        CLog();
        int AddString(const CString & s);
// Attributes
public:

// Operations
public:

// Overrides
        // ClassWizard generated virtual function overrides
        //{{AFX_VIRTUAL(CLog)
        //}}AFX_VIRTUAL

// Implementation
public:
        virtual ~CLog();

        // Generated message map functions
protected:
        //{{AFX_MSG(CLog)
                // NOTE - the ClassWizard will add and remove member functions here.
        //}}AFX_MSG

        DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_LOG_H__3D3E1DE5_49BE_42B5_95D7_0873F524FA64__INCLUDED_)

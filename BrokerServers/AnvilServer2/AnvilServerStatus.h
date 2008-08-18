#pragma once
#include "afxwin.h"
#include "resource.h"


// AnvilServerStatus dialog
[event_receiver(native)]
class AnvilServerStatus : public CDialog
{
	DECLARE_DYNAMIC(AnvilServerStatus)

public:
	AnvilServerStatus(CWnd* pParent = NULL);   // standard constructor
	virtual ~AnvilServerStatus();

// Dialog Data
	enum { IDD = IDD_ANVILSTATUS };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()
public:
	BOOL AnvilServerStatus::OnInitDialog();
	CString loadfrom;
	CEdit m_mydebug;
	void status(LPCTSTR statusmsg);
};

#include "stdAfx.h"
#include "AnvilServerStatus.h"

// AnvilServerStatus.cpp : implementation file
//


// AnvilServerStatus dialog

IMPLEMENT_DYNAMIC(AnvilServerStatus, CDialog)

AnvilServerStatus::AnvilServerStatus(CWnd* pParent /*=NULL*/)
	: CDialog(AnvilServerStatus::IDD, pParent), loadfrom("")
{

}

AnvilServerStatus::~AnvilServerStatus()
{
}

void AnvilServerStatus::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_MYDEBUG, m_mydebug);
}

BOOL AnvilServerStatus::OnInitDialog()
{
	CDialog::OnInitDialog();
	m_mydebug.SetWindowTextA(this->loadfrom);
	return true;
}

void AnvilServerStatus::status(LPCTSTR statusmsg)
{
	CString msg(statusmsg);
	const CString NEWLINE = "\r\n";
	msg.Append(NEWLINE);
	CString stat;
	m_mydebug.GetWindowTextA(stat);
	stat.Append(msg);
	m_mydebug.SetWindowTextA(stat);
}

BEGIN_MESSAGE_MAP(AnvilServerStatus, CDialog)
END_MESSAGE_MAP()


// AnvilServerStatus message handlers

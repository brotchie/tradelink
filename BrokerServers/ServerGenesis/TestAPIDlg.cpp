/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// TestAPIDlg.cpp : implementation file
//

#include "stdafx.h"
#include "TestAPI.h"
#include "TestAPIDlg.h"
//#include ".\testapidlg.h"

#include <sstream>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTestAPIDlg dialog

CTestAPIDlg::CTestAPIDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CTestAPIDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CTestAPIDlg)
	m_strPassword = _T("");
	m_strUserName = _T("");
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

	m_strUserName = _T("");
	m_strPassword = _T("");
	tl = new ServerGenesis();
}

CTestAPIDlg::~CTestAPIDlg()
{
	__unhook(&ServerGenesis::GotDebug,tl,&CTestAPIDlg::status);
	delete tl;
}



void CTestAPIDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CTestAPIDlg)
	DDX_Control(pDX, IDC_LIST, m_list);
	DDX_Control(pDX, IDC_START, m_start);
	//DDX_Control(pDX, IDC_SESSION, m_session );
	DDX_Text(pDX, IDC_PASSWORD, m_strPassword);
	DDX_Text(pDX, IDC_USERNAME, m_strUserName);
	//}}AFX_DATA_MAP

}

void CTestAPIDlg::status(LPCTSTR m)
{
	m_list.AddString(m);
	int last = m_list.GetCount()-1;
	m_list.SetCurSel(last);
}
#define ATTEMPTLOGIN WM_USER+0X02
BEGIN_MESSAGE_MAP(CTestAPIDlg, CDialog)
	//{{AFX_MSG_MAP(CTestAPIDlg)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDC_START, OnStart)
	ON_BN_CLICKED(IDC_STOP, OnStop)
	//}}AFX_MSG_MAP
//	ON_BN_CLICKED(IDC_HIDEMM, OnBnClickedHidemm)
ON_MESSAGE(ATTEMPTLOGIN,AttemptLogin)
ON_EN_CHANGE(IDC_PASSWORD, &CTestAPIDlg::OnEnChangePassword)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTestAPIDlg message handlers

BOOL CTestAPIDlg::OnInitDialog()
{
	CDialog::OnInitDialog();
	tl->gtw->SubclassDlgItem(IDC_SESSION,this);

	__hook(&ServerGenesis::GotDebug,tl,&CTestAPIDlg::status);
	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
		
	PostMessage(ATTEMPTLOGIN,0,0);
	ShowWindow(SW_MINIMIZE);
	return TRUE;  // return TRUE  unless you set the focus to a control
}

LRESULT CTestAPIDlg::AttemptLogin(WPARAM w, LPARAM l)
{
	tl->Start();
	return 0;
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CTestAPIDlg::OnPaint() 
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
		CDialog::OnPaint();
	}
}

// The system calls this to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CTestAPIDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void CTestAPIDlg::OnOK() 
{
}

void CTestAPIDlg::OnCancel() 
{
	EndDialog(IDCANCEL);
}

void CTestAPIDlg::OnClose() 
{
	EndDialog(IDCANCEL);
}

void CTestAPIDlg::OnStart() 
{
	if(UpdateData() == FALSE)
		return;

	tl->Start(m_strUserName, m_strPassword);
}

void CTestAPIDlg::OnStop() 
{
	tl->Stop();
}


void CTestAPIDlg::OnEnChangePassword()
{
	// TODO:  If this is a RICHEDIT control, the control will not
	// send this notification unless you override the CDialog::OnInitDialog()
	// function and call CRichEditCtrl().SetEventMask()
	// with the ENM_CHANGE flag ORed into the mask.

	// TODO:  Add your control notification handler code here
}

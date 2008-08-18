// TWSServerDlg.cpp : implementation file
//

#include "stdafx.h"
#include "TWSServer.h"
#include "TWSServerDlg.h"
#include "EClientSocket.h"   // C:\JTS\SocketClient\include must be added to include path
#include "TwsSocketClientErrors.h"   // C:\JTS\SocketClient\include must be added to include path

#ifdef _DEBUG
#define new DEBUG_NEW
#endif
using namespace TradeLinkServer;

// CTWSServerDlg dialog


CTWSServerDlg::CTWSServerDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CTWSServerDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CTWSServerDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_STATUS, m_status);
}

BEGIN_MESSAGE_MAP(CTWSServerDlg, CDialog)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


// CTWSServerDlg message handlers

BOOL CTWSServerDlg::OnInitDialog()
{
	CDialog::OnInitDialog();


	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	ShowWindow(SW_MINIMIZE);

	cstat("Starting tradelink broker server...");
	tl = new TWS_TLWM();
	__hook(&TradeLink_WM::GotDebug,tl,&CTWSServerDlg::status);
	tl->Start();

	return TRUE;  // return TRUE  unless you set the focus to a control
}

CTWSServerDlg::~CTWSServerDlg()
{
	__unhook(&TradeLink_WM::GotDebug,tl,&CTWSServerDlg::status);
	delete tl;
}

void CTWSServerDlg::OnSysCommand(UINT nID, LPARAM lParam)
{

		CDialog::OnSysCommand(nID, lParam);
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CTWSServerDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

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

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CTWSServerDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

void CTWSServerDlg::cstat(CString msg)
{
	const CString NEWLINE = "\r\n";
	CString stat;
	m_status.GetWindowTextA(stat);
	stat.Append(msg+NEWLINE);
	m_status.SetWindowTextA(stat);
}


void CTWSServerDlg::status(LPCTSTR m)
{
	CString msg(m);
	const CString NEWLINE = "\r\n";
	msg.Append(NEWLINE);
	CString stat;
	m_status.GetWindowTextA(stat);
	stat.Append(msg);
	m_status.SetWindowTextA(stat);
}

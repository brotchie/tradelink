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
	m_strStock = _T("");
	m_strPassword = _T("");
	m_strUserName = _T("");
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

	m_strStock = "ZVZZT";
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
	DDX_Text(pDX, IDC_STOCK, m_strStock);
	DDV_MaxChars(pDX, m_strStock, 10);
	DDX_Text(pDX, IDC_PASSWORD, m_strPassword);
	DDX_Text(pDX, IDC_USERNAME, m_strUserName);
	//}}AFX_DATA_MAP

	if(pDX->m_bSaveAndValidate)
	{
		m_strStock.TrimLeft();
		m_strStock.TrimRight();

		if(m_strStock.GetLength() <= 0)
		{
			pDX->m_idLastControl = IDC_STOCK;
			MessageBox("Please input stock!", NULL, MB_ICONSTOP);
			pDX->Fail();
		}
	}
	DDX_Control(pDX, IDC_HIDEMM, m_hidemm);
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
	ON_BN_CLICKED(IDC_BID, OnBid)
	ON_BN_CLICKED(IDC_ASK, OnAsk)
	ON_BN_CLICKED(IDC_BUY, OnBuy)
	ON_BN_CLICKED(IDC_SELL, OnSell)
	ON_BN_CLICKED(IDC_CANCEL_BID, OnCancelBid)
	ON_BN_CLICKED(IDC_CANCEL_OFFER, OnCancelOffer)
	ON_BN_CLICKED(IDC_CANCEL_BID2, OnCancelBid2)
	ON_BN_CLICKED(IDC_DUMP, OnDump)
	//}}AFX_MSG_MAP
//	ON_BN_CLICKED(IDC_HIDEMM, OnBnClickedHidemm)
ON_BN_CLICKED(IDC_HIDEMM, OnBnClickedHidemm)
ON_BN_CLICKED(IDC_MARKETBUY, OnBnClickedMarketbuy)
ON_BN_CLICKED(IDC_MARKETSELL, OnBnClickedMarketsell)
ON_BN_CLICKED(IDC_REQUESTCHIAN, OnBnClickedRequestchian)
ON_BN_CLICKED(IDC_TEST, OnBnClickedTest)
ON_BN_CLICKED(IDC_BUTTON_DISPLAY_ACCOUNTS, OnBnClickedButtonDisplayAccounts)
ON_CBN_SELCHANGE(IDC_COMBO_ACCOUNTS, OnCbnSelchangeComboAccounts)
ON_CBN_DROPDOWN(IDC_COMBO_ACCOUNTS, &CTestAPIDlg::OnCbnDropdownComboAccounts)
ON_MESSAGE(ATTEMPTLOGIN,AttemptLogin)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTestAPIDlg message handlers

BOOL CTestAPIDlg::OnInitDialog()
{
	m_session.m_pDlg = this;

	CDialog::OnInitDialog();
	tl->gtw->SubclassDlgItem(IDC_SESSION,this);

	__hook(&ServerGenesis::GotDebug,tl,&CTestAPIDlg::status);
	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
	PostMessage(ATTEMPTLOGIN,0,0);
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
	if(m_session.CanClose() == FALSE)
	{
		MessageBox("Please logout before exit!", NULL, MB_ICONEXCLAMATION);
		return;
	}

	if(MessageBox("Do you want to exit?", NULL, MB_ICONQUESTION | MB_YESNOCANCEL) != IDYES)
	{
		return;
	}

	EndDialog(IDCANCEL);
}

void CTestAPIDlg::OnClose() 
{

}

void CTestAPIDlg::OnStart() 
{
	if(UpdateData() == FALSE)
		return;
	m_session.m_setting.SetExecAddress("69.64.202.155", 15805);
	m_session.m_setting.SetQuoteAddress("69.64.202.155", 15805);
	m_session.m_setting.SetLevel2Address("69.64.202.155", 15805);


	tl->Start(m_strUserName, m_strPassword);
	//m_session.Login(m_strUserName, m_strPassword);
}

void CTestAPIDlg::OnStop() 
{
	tl->Stop();
	//m_session.Logout();
	//m_session.TryClose();
}

void CTestAPIDlg::OnBid() 
{
	BOOL v = m_session.IsLoggedIn();
	if( v == FALSE)
		return;
	
	GTStock *pStock = m_session.GetStock(m_strStock);
	if(pStock == NULL)
		return;

	GTOrder order = pStock->m_defOrder;
	double price = pStock->m_level2.GetBestBidPrice();

	pStock->Bid(order, 100, pStock->m_level2.GetBestBidPrice() - 0.01, METHOD_CRSS);

	pStock->PlaceOrder(order);
}

void CTestAPIDlg::OnAsk() 
{
	if(m_session.IsLoggedIn() == FALSE)
		return;
	
	GTStock *pStock = m_session.GetStock(m_strStock);
	if(pStock == NULL)
		return;

	GTOrder order = pStock->m_defOrder;

	pStock->Ask(order, 100, pStock->m_level2.GetBestAskPrice() + 0.01, METHOD_ISLD);

	pStock->PlaceOrder(order);
}

void CTestAPIDlg::OnBuy() 
{
	if(m_session.IsLoggedIn() == FALSE)
		return;
	
	GTStock *pStock = m_session.GetStock(m_strStock);
	if(pStock == NULL)
		return;

	pStock->Buy(100, pStock->m_level2.GetBestAskPrice(), METHOD_ISLD);	
}

void CTestAPIDlg::OnSell() 
{
	if(m_session.IsLoggedIn() == FALSE)
		return;
	
	GTStock *pStock = m_session.GetStock(m_strStock);
	if(pStock == NULL)
		return;

	pStock->Sell(100, pStock->m_level2.GetBestBidPrice(), METHOD_ISLD);
}

void CTestAPIDlg::OnCancelBid() 
{
	GTStock *pStock = m_session.GetStock(m_strStock);
	if(pStock == NULL)
		return;

	pStock->CancelOrder('B');
}

void CTestAPIDlg::OnCancelOffer() 
{
	GTStock *pStock = m_session.GetStock(m_strStock);
	if(pStock == NULL)
		return;

	pStock->CancelOrder('S');
	pStock->CancelOrder('T');
}

void CTestAPIDlg::OnCancelBid2() 
{
	GTStock *pStock = m_session.GetStock(m_strStock);
	if(pStock == NULL)
		return;

	pStock->CancelAllOrders();
}

void CTestAPIDlg::OnDump() 
{
#if 1
	FILE *fp = fopen("C:\\temp\\Dump.txt", "w");
	if(fp == NULL)
		return;
	
	m_session.Dump(fp, 0);
	
	fclose(fp);
#else
	CString strStock;
	GetDlgItemText(IDC_STOCK, strStock);

	GTStock *pStock = m_session.GetStock(strStock);
	if(pStock == NULL)
		return;

	for(int i = 0; i <= 10; ++i)
	{
		GTLevel2 *pBid = pStock->m_level2.GetBidItem(i);
		if(pBid == NULL)
			break;

		GTLevel2 *pAsk = pStock->m_level2.GetAskItem(i);
		if(pAsk == NULL)
			break;

		TRACE("%3d  "
			"%6d	%.4s	%.4lf		"
			"%6d	%.4s	%.4lf\n"
			, i
			, pBid->dwShares, &pBid->mmid, pBid->dblPrice
			, pAsk->dwShares, &pAsk->mmid, pAsk->dblPrice
			);
	}
#endif
}

void CTestAPIDlg::OnBnClickedHidemm()
{
	CString strStock;
	GetDlgItemText(IDC_STOCK, strStock);

	GTStock *pStock = m_session.GetStock(strStock);
	if(pStock == NULL)
		return;

	pStock->m_level2.HideMarketMaker(m_hidemm.GetCheck() == 1);
}

void CTestAPIDlg::OnBnClickedMarketbuy()
{
	// TODO: Add your control notification handler code here

	if(m_session.IsLoggedIn() == FALSE)
		return;
	
	GTStock *pStock = m_session.GetStock(m_strStock);
	if(pStock == NULL)
		return;

	GTOrder32 order;
	order = pStock->m_defOrder;
	if(pStock->PlaceOrder(order, 'B', 1, 0, METHOD_ISEO, 0)==0)
	{
		order.chPriceIndicator = PRICE_INDICATOR_MARKET;
		pStock->PlaceOrder(order);
	}
}

void CTestAPIDlg::OnBnClickedMarketsell()
{
	// TODO: Add your control notification handler code here
	if(m_session.IsLoggedIn() == FALSE)
		return;
	
	GTStock *pStock = m_session.GetStock(m_strStock);
	if(pStock == NULL)
		return;

	GTOrder32 order;
	order = pStock->m_defOrder;
	if(pStock->PlaceOrder(order, 'S', 1, 0, METHOD_ISEO, 0)==0)
	{
		order.chPriceIndicator = PRICE_INDICATOR_MARKET;
		pStock->PlaceOrder(order);
	}
}

void CTestAPIDlg::OnBnClickedRequestchian()
{
	// TODO: Check the Option API later

	//m_session.RequestChain("GOOG");
	//m_session.RequestChain("A");
}

void CTestAPIDlg::OnBnClickedTest()
{
	// TODO: Check the Option API later
/*
	char symbol[16];
	symbol[0] = 0;
	int openint = 0;
	int rc = m_session.GetOptionSymbol("A", 2006, 5, 25, symbol, &openint);
	TRACE("%d: %s %d", rc, symbol, openint);

	int nYear = 0;
	int nMonth = 0;
	double dblStrike;
	char szOption[17];
	int nOpenInt = 0;
	UINT pos = m_session.GetFirstChainElement("A", &nYear, &nMonth, &dblStrike, szOption, &nOpenInt);
	while(pos)
	{
		TRACE("%s %4d %02d %6.2lf %05d\n", szOption, nYear, nMonth, dblStrike, nOpenInt);
		pos = m_session.GetNextChainElement("A", pos, &nYear, &nMonth, &dblStrike, szOption, &nOpenInt);
	}

	pos = m_session.GetFirstChainElement("GOOG", &nYear, &nMonth, &dblStrike, szOption, &nOpenInt);
	while(pos)
	{
		TRACE("%s %4d %02d %6.2lf %05d\n", szOption, nYear, nMonth, dblStrike, nOpenInt);
		pos = m_session.GetNextChainElement("GOOG", pos, &nYear, &nMonth, &dblStrike, szOption, &nOpenInt);
	}
	*/
}

void CTestAPIDlg::UpdateAccountInfo(std::list<std::string>& lstAccounts)
{
	// TODO: Add your control notification handler code here
	int n = 0;
	CComboBox* comb = (CComboBox*)GetDlgItem(IDC_COMBO_ACCOUNTS);
	CString strOldSelected;
	int nID = -1;

	if(comb)
	{
		nID = comb->GetCurSel();
		if(nID >= 0)
		{
			comb->GetLBText(nID, strOldSelected);
		}
		comb->ResetContent();
		comb->Clear();
	}

	std::list<std::string>::iterator it;
	for(n = 0, it = lstAccounts.begin(); lstAccounts.end()!=it; ++it, ++n)
	{
		if(comb)
		{
			comb->InsertString(n, it->c_str());
		}
	}

	// select
	if(nID >= 0)
	{
		CString strNew;
		comb->GetLBText(nID, strNew);
		
		if(!_stricmp(strOldSelected.GetString(), strNew.GetString()))
		{
			comb->SetCurSel(nID);
		}
		else
			comb->SetCurSel(-1);	// none selected
	}
}

void CTestAPIDlg::OnBnClickedButtonDisplayAccounts()
{
	int n = 0;
	std::list<std::string> lstAccounts;
	m_session.GetAllAccountName(lstAccounts);
	UpdateAccountInfo(lstAccounts);
	
	std::list<std::string>::const_iterator it;
	std::ostringstream os;
	os << "User " << m_session.m_user.szUserName << " [ID: " << m_session.m_user.szUserID << "] has " <<  lstAccounts.size() << " accounts. \r\n";
	for(it = lstAccounts.begin(); lstAccounts.end()!=it; ++it, ++n)
	{
		os << "Account " << n+1 << ": " << *it << ";\r\n";
	}

	MessageBox(os.str().c_str(), "MY ACCOUNTS!");
}

void CTestAPIDlg::OnCbnSelchangeComboAccounts()
{
	// TODO: Add your control notification handler code here
	CComboBox* comb = (CComboBox*)GetDlgItem(IDC_COMBO_ACCOUNTS);
	int nID = -1;

	if(comb)
	{
		nID = comb->GetCurSel();
		if(nID >= 0)
		{
			CString str ;
			comb->GetLBText(nID, str);

			m_session.SetCurrentAccount(str.GetString());
		}
	}
}

void CTestAPIDlg::OnCbnDropdownComboAccounts()
{
	// TODO: Add your control notification handler code here
	std::list<std::string> lstAccounts;
	m_session.GetAllAccountName(lstAccounts);
	UpdateAccountInfo(lstAccounts);
}

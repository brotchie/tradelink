// MainDlg.cpp : implementation file
//

#include "stdafx.h"
#include "GrayBoxSample.h"
#include "MainDlg.h"


// MainDlg dialog

IMPLEMENT_DYNAMIC(MainDlg, CDialog)

MainDlg::MainDlg(CWnd* pParent /*=NULL*/)
	: CDialog(MainDlg::IDD, pParent)
	, account(0)
	, summary(0)
{

}

MainDlg::~MainDlg()
{
	if (summary)
	{
		summary->L_Detach(this);
		L_DestroySummary(summary);
	}
	if (account)
	{
		account->L_Detach(this);
	}
}

void MainDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}


BEGIN_MESSAGE_MAP(MainDlg, CDialog)
	ON_BN_CLICKED(IDC_SENDORDER, &MainDlg::OnBnClickedSendOrder)
	ON_WM_NCDESTROY()
	ON_WM_CLOSE()
END_MESSAGE_MAP()


void MainDlg::OnNcDestroy()
{
	delete this;
}

void MainDlg::OnClose()
{
	L_ExitLightspeedExtension(2);
}

BOOL MainDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	account = L_GetAccount();
	account->L_Attach(this);
	summary = L_CreateSummary("ZXZZT");
	summary->L_Attach(this);
	SetDlgItemText(IDC_TRADERID, account->L_TraderId());
	SetDlgItemInt(IDC_PENDINGORDERS, account->L_PendingOrdersCount());

	return TRUE;
}

void MainDlg::HandleMessage(L_Message const *msg)
{
	switch (msg->L_Type())
	{
	case L_MsgOrderChange::id:
		SetDlgItemInt(IDC_PENDINGORDERS, account->L_PendingOrdersCount());
		break;
	case L_MsgL1::id:
	case L_MsgL1Update::id:
		{
			double bid = summary->L_Bid();
			double ask = summary->L_Ask();
			CString buf;
			buf.Format("%.2f", bid);
			SetDlgItemText(IDC_BID, buf);
			buf.Format("%.2f", ask);
			SetDlgItemText(IDC_ASK, buf);
		}
		break;
	}
}

void MainDlg::OnBnClickedSendOrder()
{
	account->L_SendOrder(
				summary,
				L_OrderType::LIMIT,
				L_Side::BUY,
				100,
				summary->L_Bid() - 0.01,
				"NSDQ",
				L_TIF::DAY
				);
	L_AddMessageToExtensionWnd("OnBnClickedSendOrder");
}


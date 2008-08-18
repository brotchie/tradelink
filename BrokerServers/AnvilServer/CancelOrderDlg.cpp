// CancelOrderDlg.cpp : implementation file
//

#include "stdafx.h"
#include "Resource.h"
#include "CancelOrderDlg.h"
#include "ExtFrame.h"
#include "BusinessApi.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CancelOrderDlg dialog


CancelOrderDlg::CancelOrderDlg(CWnd* pParent /*=NULL*/):
    BaseDlg(CancelOrderDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CancelOrderDlg)
	//}}AFX_DATA_INIT
}


void CancelOrderDlg::DoDataExchange(CDataExchange* pDX)
{
	BaseDlg::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CancelOrderDlg)
	DDX_Control(pDX, IDB_CANCELORDER_INCLUDE_SMART, m_CheckBoxIncludeSmartOrders);
	DDX_Control(pDX, IDB_CANCELORDER_SINGLE_DESTINATION, m_RadioSingleDestination);
	DDX_Control(pDX, IDB_CANCELORDER_ALL_DESTINATIONS, m_RadioAllDestinations);
	DDX_Control(pDX, IDE_CANCELORDER_STOCKSYMBOL, m_EditSymbol);
	DDX_Control(pDX, IDE_CANCELORDER_MMID, m_EditMmid);
	DDX_Control(pDX, IDE_CANCELORDER_ID, m_EditOrderId);
	DDX_Control(pDX, IDB_CANCELORDER_NONDAY, m_CheckBoxTifNonDay);
	DDX_Control(pDX, IDB_CANCELORDER_DAY, m_CheckBoxTifDay);
	DDX_Control(pDX, IDB_CANCELORDER_SELL, m_CheckBoxSideSell);
	DDX_Control(pDX, IDB_CANCELORDER_BUY, m_CheckBoxSideBuy);
	DDX_Control(pDX, IDB_CANCELORDER_SINGLE_STOCK, m_RadioSingleStock);
	DDX_Control(pDX, IDB_CANCELORDER_ALL_STOCKS, m_RadioAllStocks);
	DDX_Control(pDX, IDB_CANCELORDER_SINGLE, m_RadioSingleOrder);
	DDX_Control(pDX, IDB_CANCELORDER_MULTIPLE, m_RadioMultipleOrders);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CancelOrderDlg, BaseDlg)
	//{{AFX_MSG_MAP(CancelOrderDlg)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CancelOrderDlg message handlers

BOOL CancelOrderDlg::OnInitDialog() 
{
	BaseDlg::OnInitDialog();
	
	// TODO: Add extra initialization here
    m_account = B_GetCurrentAccount();
    if(m_account)
    {
        std::string title("SendOrder - ");
        title += B_GetAccountName(m_account);
        SetWindowText(title.c_str());
    }

    m_EditSymbol.SetLimitText(7);
    m_EditMmid.SetLimitText(4);
    m_EditOrderId.SetLimitText(10);

	m_RadioAllStocks.SetCheck(BST_CHECKED);
    m_RadioAllDestinations.SetCheck(BST_CHECKED);
    m_RadioMultipleOrders.SetCheck(BST_CHECKED);
	return TRUE;
}
/*
void CancelOrderDlg::OnCancel()
{
    ExtFrame::GetInstance()->DestroyCancelOrderDlg(this);
}
*/
void CancelOrderDlg::OnOK()
{
    char buf[50];
//SINGLE ORDER ?
    unsigned int orderId;
    if(m_RadioSingleOrder.GetCheck() == BST_CHECKED)
    {
        m_EditOrderId.GetWindowText(buf, sizeof(buf));
        if(*buf)
        {
            orderId = atoi(buf);
        }
        else
        {
            return;
        }
    }
    else
    {
        orderId = 0;
    }


//SYMBOL
    char symbol[50];
    bool allStocks = m_RadioAllStocks.GetCheck() == BST_CHECKED;
    if(allStocks)
    {
        *symbol = '\0';
    }
    else
    {
        m_EditSymbol.GetWindowText(symbol, sizeof(symbol));

        if(!*symbol)
        {
            return;
        }
    }
//DESTINATION
    char* destination;
    char destinationBuf[50];
    bool allDestinations = m_RadioAllDestinations.GetCheck() == BST_CHECKED;
    if(allDestinations)
    {
        destination = NULL;
    }
    else
    {
        m_EditMmid.GetWindowText(destinationBuf, sizeof(destinationBuf));

        if(!*destinationBuf)
        {
            return;
        }
        destination = destinationBuf;
    }

    unsigned int flags = 0;
    if(m_CheckBoxSideBuy.GetCheck() == BST_CHECKED)
    {
        flags |= CO_BUY;
    }
    if(m_CheckBoxSideSell.GetCheck() == BST_CHECKED)
    {
        flags |= CO_SELL;
    }
    if(m_CheckBoxTifDay.GetCheck() == BST_CHECKED)
    {
        flags |= CO_DAY;
    }
    if(m_CheckBoxTifNonDay.GetCheck() == BST_CHECKED)
    {
        flags |= CO_NONDAY;
    }

    bool includeSmartOrders = m_CheckBoxIncludeSmartOrders.GetCheck() == BST_CHECKED;
    if(orderId == 0)
    {
        if(allStocks)
        {
            B_CancelAllOrders(flags,
				(1 << ST_LAST) - 1,//all kinds of securities - stocks, options, futures
                destination,
                includeSmartOrders,
				true,
                m_account);
        }
        else
        {
            B_CancelAllStockOrders(symbol,
                flags,
                destination,
                includeSmartOrders,
				true,
                m_account);
        }
//void WINAPI B_CancelAllStockEcnOrders(const char* symbol, unsigned int flags, bool includeSmartOrders = false, const Observable* account = NULL);
    }
    else
    {
        Order* order = B_FindOrder(orderId, m_account);
        if(order)
        {
            order->Cancel();
        }
    }
}

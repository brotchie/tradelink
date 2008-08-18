// SendOrderBaseDlg.cpp : implementation file
//

#include "stdafx.h"
#include "Resource.h"
#include "SendOrderBaseDlg.h"
#include "Messages.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif


OrderParamsList::~OrderParamsList()
{
    OrderList::iterator it;
    OrderList::iterator itend = m_orderList.end();
    for(it = m_orderList.begin(); it != itend; ++it)
    {
        delete *it;
    }
    m_orderList.clear();
}

/////////////////////////////////////////////////////////////////////////////
// SendOrderBaseDlg dialog


SendOrderBaseDlg::SendOrderBaseDlg(unsigned int id, CWnd* pParent /*=NULL*/):
    BaseDlg(id, pParent)
{
	//{{AFX_DATA_INIT(SendOrderBaseDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void SendOrderBaseDlg::DoDataExchange(CDataExchange* pDX)
{
	BaseDlg::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(SendOrderBaseDlg)
	DDX_Control(pDX, IDE_SENDORDER_SYMBOL, m_EditSymbol);
	DDX_Control(pDX, IDB_SENDORDER_SELL, m_RadioSideSell);
	DDX_Control(pDX, IDB_SENDORDER_BUY, m_RadioSideBuy);
	DDX_Control(pDX, IDE_SENDORDER_SIZE, m_EditSize);
	DDX_Control(pDX, IDE_SENDORDER_PRICE_TENTHS, m_EditPriceTenths);
	DDX_Control(pDX, IDE_SENDORDER_PRICE_DOLLARS, m_EditPriceDollars);
	DDX_Control(pDX, IDE_SENDORDER_PRICE_CENTS, m_EditPriceCents);
	DDX_Control(pDX, IDE_SENDORDER_TIF_MINUTES, m_EditTifMinutes);
	DDX_Control(pDX, IDC_SENDORDER_TIF, m_ComboBoxTif);
	DDX_Control(pDX, IDS_SENDORDER_TIF_MINUTES, m_StaticTifMinutes);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(SendOrderBaseDlg, BaseDlg)
	//{{AFX_MSG_MAP(SendOrderBaseDlg)
	ON_CBN_SELCHANGE(IDC_SENDORDER_TIF, OnSelchangeSendOrderTif)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// SendOrderBaseDlg message handlers



void SendOrderBaseDlg::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
    switch(message->GetType())
    {
        case M_RESP_REFRESH_SYMBOL:
//This message comes when loading of a stock from the server is completed.
//Only after this a stock is marked as "valid" and function B_IsStockValid(unsigned int stockHandle) will return true.
//This message comes from all Observables.
//        m_level1
//        m_level2
//        m_prints
//        m_aggregatedBook
//        m_expandedBook
//But we can Invalidate the list box entry only once. That's why we have the if statement here.
//The Invalidate() will only change the stock's color.
//        if(from == m_level1)
        {
            std::string sym(((MsgRefreshSymbol*)message)->m_symbol);
            OrdersToSend::iterator found = m_ordersToSend.find(sym);
            if(found != m_ordersToSend.end())
            {
                Order* orderSent;
                unsigned int error;
                const StockBase* stockHandle = found->second.m_stockHandle;
                std::list<OrderParams*>& orderList = found->second.m_orderList;
                std::list<OrderParams*>::iterator it;
                std::list<OrderParams*>::iterator itend = orderList.end();
                for(it = orderList.begin(); it != itend; ++it)
                {
                    error = SendOrder(stockHandle, *(*it), orderSent);
                    if(error != SO_OK)//error occured
                    {
                        MessageBox(B_GetOrderErrorMessage(error), "Send Delayed Order Error", MB_OK|MB_ICONERROR);
                    }
                }
                B_GetLevel1(stockHandle)->Remove(this);
                m_ordersToSend.erase(found);
            }

        }
        break;

        case M_RESP_REFRESH_SYMBOL_FAILED:
//This message comes if stock does not exist. The internal object representing the stock will be destroyed immediately after this message is sent out.
//We must remove any reference to the stock and not use it otherwise it will cause a crash.
//For example, all the following pointers become invalid (objects pointed by them are destroyed)
//        m_level1
//        m_level2
//        m_prints
//        m_aggregatedBook
//        m_expandedBook

//        B_GetLevel1(stockHandle)->Remove(this);
        
        {
            std::string sym(((MsgRefreshSymbolFailed*)message)->m_symbol);
            OrdersToSend::iterator found = m_ordersToSend.find(sym);
            if(found != m_ordersToSend.end())
            {
                const StockBase* stockHandle = found->second.m_stockHandle;
                B_GetLevel1(stockHandle)->Remove(this);
                m_ordersToSend.erase(found);
            }
        }
        break;
    }
}

BOOL SendOrderBaseDlg::OnInitDialog() 
{
	BaseDlg::OnInitDialog();
	
    m_account = B_GetCurrentAccount();

    if(m_account)
    {
        CString text;
        GetWindowText(text);
        std::string title(text);
        title += " - ";
        title += B_GetAccountName(m_account);
        SetWindowText(title.c_str());
        B_AddUserOrderDescription(101, "Ext. Order", m_account);
    }

    m_EditSymbol.SetLimitText(7);
    m_EditPriceDollars.SetLimitText(4);
    m_EditPriceCents.SetLimitText(2);
    m_EditPriceTenths.SetLimitText(1);
    m_EditSize.SetLimitText(5);

    m_ComboBoxTif.AddString("Seconds");
    m_ComboBoxTif.AddString("IOC");
    m_ComboBoxTif.AddString("DAY");
    m_ComboBoxTif.SetCurSel(0);

    m_RadioSideBuy.SetCheck(BST_CHECKED);
	return TRUE;
}

unsigned int SendOrderBaseDlg::FillOrderParams(OrderParams* op)
{
//SIDE
    char side;
    if(m_RadioSideBuy.GetCheck() == BST_CHECKED)
    {
        side = 'B';
    }
    else
    {
        side = 'S';
    }

//PRICE
    char buf[50];
    m_EditPriceDollars.GetWindowText(buf, sizeof(buf));
    unsigned int dollars = atoi(buf);
    m_EditPriceCents.GetWindowText(buf, sizeof(buf));
    unsigned short cents = atoi(buf);
    m_EditPriceTenths.GetWindowText(buf, sizeof(buf));
    unsigned short tenths = atoi(buf);

    Money price(dollars, cents * 10 + tenths);

//SIZE
    m_EditSize.GetWindowText(buf, sizeof(buf));
    unsigned int size = atoi(buf);
    if(size == 0)
    {
        return 1;
    }

//TIF
    unsigned int tif;
    int sel = m_ComboBoxTif.GetCurSel();
    if(sel == 0)
    {
        m_EditTifMinutes.GetWindowText(buf, sizeof(buf));
        if(*buf)
        {
            tif = atoi(buf);
        }
        else
        {
            tif = TIF_IOC;
        }
    }
    else if(sel == 1)
    {
        tif = TIF_IOC;
    }
    else
    {
        tif = TIF_DAY;
    }

    op->m_tif = tif;
    op->m_side = side;
    op->m_price = price;
    op->m_size = size;
    
    return 0;
}

void SendOrderBaseDlg::OnOK()
{
//SYMBOL
    char symbol[50];
    m_EditSymbol.GetWindowText(symbol, sizeof(symbol));
    if(!*symbol)
    {
        return;
    }

    OrderParams* op = CreateOrderParams();

    if(FillOrderParams(op) == 0)
    {

        //Subscribe to Stock
        const StockBase* stockHandle = B_GetStockHandle(symbol);
        if(B_IsStockLoaded(stockHandle))
        {
    //Now we collected all the info.
    //Let's send the order
            Order* orderSent;//output parameter. If the order is sent successfully you will get the pointer to the order sent
            unsigned int error = SendOrder(stockHandle, *op, orderSent);
            delete op;
            if(error != SO_OK)//error occured
            {
                MessageBox(B_GetOrderErrorMessage(error), "Send Order Error", MB_OK|MB_ICONERROR);
            }
        }
        else
        {
    //we have to wait until the stock is loaded
            B_GetLevel1(stockHandle)->Add(this);

            std::string sym(symbol);
            OrdersToSend::iterator found = m_ordersToSend.find(sym);
            if(found == m_ordersToSend.end())
            {
                found = m_ordersToSend.insert(OrdersToSend::value_type(std::string(symbol), OrderParamsList())).first;
            }
        
            OrderParamsList& orderParamsList = found->second;
            orderParamsList.m_stockHandle = stockHandle;
            orderParamsList.m_orderList.push_back(op);
        }
    }
}



void SendOrderBaseDlg::OnSelchangeSendOrderTif() 
{
    int sel = m_ComboBoxTif.GetCurSel();
    if(sel == 0)
    {
        m_StaticTifMinutes.ShowWindow(SW_SHOW);
        m_EditTifMinutes.ShowWindow(SW_SHOW);
    }
    else
    {
        m_StaticTifMinutes.ShowWindow(SW_HIDE);
        m_EditTifMinutes.ShowWindow(SW_HIDE);
    }
}


// SendOrderDlg.cpp : implementation file
//

#include "stdafx.h"
#include "Resource.h"
#include "SendOrderDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// SendOrderDlg dialog


SendOrderDlg::SendOrderDlg(CWnd* pParent /*=NULL*/):
	SendOrderBaseDlg(SendOrderDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(SendOrderDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void SendOrderDlg::DoDataExchange(CDataExchange* pDX)
{
	SendOrderBaseDlg::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(SendOrderDlg)
	DDX_Control(pDX, IDB_SENDORDER_LISTED_DEFAULT, m_RadioDefault);
//	DDX_Control(pDX, IDB_SENDORDER_LISTED_NYSE_NX, m_RadioNyseNX);
	DDX_Control(pDX, IDB_SENDORDER_LISTED_NYSE, m_RadioNyse);
	DDX_Control(pDX, IDB_SENDORDER_LISTED_AMEX, m_RadioAmex);
	DDX_Control(pDX, IDS_SENDORDER_VISIBLESIZE, m_StaticVisibleSize);
	DDX_Control(pDX, IDE_SENDORDER_VISIBLESIZE, m_EditVisibleSize);
	DDX_Control(pDX, IDE_SENDORDER_DESTINATION, m_EditDestination);
	DDX_Control(pDX, IDB_SENDORDER_HIDDEN, m_CheckBoxHidden);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(SendOrderDlg, SendOrderBaseDlg)
	//{{AFX_MSG_MAP(SendOrderDlg)
	ON_BN_CLICKED(IDB_SENDORDER_HIDDEN, OnSendOrderHidden)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// SendOrderDlg message handlers

void SendOrderDlg::OnSendOrderHidden() 
{
    if(m_CheckBoxHidden.GetCheck() == BST_CHECKED)
    {
	    m_StaticVisibleSize.ShowWindow(SW_HIDE);
	    m_EditVisibleSize.ShowWindow(SW_HIDE);
    }
    else
    {
	    m_StaticVisibleSize.ShowWindow(SW_SHOW);
	    m_EditVisibleSize.ShowWindow(SW_SHOW);
    }
}


/*
void SendOrderDlg::OnCancel()
{
    ExtFrame::GetInstance()->DestroySendOrderDlg(this);
}
*/

unsigned int SendOrderDlg::FillOrderParams(OrderParams* o)
{
    unsigned int ret = SendOrderBaseDlg::FillOrderParams(o);
    if(ret != 0)
    {
        return ret;
    }

//DESTINATION
    char destination[50];
    m_EditDestination.GetWindowText(destination, sizeof(destination));
    if(!*destination)
    {
        return 2;
    }

    char buf[50];

//VISIBILITY
    unsigned int visibilityMode;
    unsigned int visibleSize;
    if(m_CheckBoxHidden.GetCheck() == BST_CHECKED)
    {
        visibilityMode = OVM_HIDDEN;
        visibleSize = 0;
    }
    else
    {
        visibilityMode = OVM_VISIBLE;
        m_EditVisibleSize.GetWindowText(buf, sizeof(buf));
        if(*buf)
        {
            visibleSize = atoi(buf);
        }
        else
        {
            m_EditSize.GetWindowText(buf, sizeof(buf));
            visibleSize = atoi(buf);
        }
    }


    OrderParamsSimple* op = (OrderParamsSimple*)o;
    strcpy_s(op->m_destination, sizeof(op->m_destination), destination);
    op->m_visibilityMode = visibilityMode;
    op->m_visibleSize = visibleSize;
    if(m_RadioNyse.GetCheck() == BST_CHECKED)
    {
        op->m_listedDestinationExchange = DE_NYSE;
    }
/*
    else if(m_RadioNyseNX.GetCheck() == BST_CHECKED)
    {
        op->m_listedDestinationExchange = DE_NYSE_NX;
    }
*/
    else if(m_RadioAmex.GetCheck() == BST_CHECKED)
    {
        op->m_listedDestinationExchange = DE_AMEX;
    }
    else
    {
        op->m_listedDestinationExchange = DE_DEFAULT;
    }
    return 0;
}
/*
void SendOrderDlg::OnOK()
{

//DESTINATION
    char destination[50];
    m_EditDestination.GetWindowText(destination, sizeof(destination));
    if(!*destination)
    {
        return;
    }

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
        return;
    }


//VISIBILITY
    unsigned int visibilityMode;
    unsigned int visibleSize;
    if(m_CheckBoxHidden.GetCheck() == BST_CHECKED)
    {
        visibilityMode = OVM_HIDDEN;
        visibleSize = 0;
    }
    else
    {
        visibilityMode = OVM_VISIBLE;
        m_EditVisibleSize.GetWindowText(buf, sizeof(buf));
        if(*buf)
        {
            visibleSize = atoi(buf);
        }
        else
        {
            visibleSize = size;
        }
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


//Now we collected all the info.
//Let's send the order

    
    //Subscribe to Stock
    unsigned int stockHandle = B_GetStockHandle(symbol);
    if(B_IsStockLoaded(stockHandle))
    {

        const Order* orderSent;//output parameter. If the order is sent successfully you will get the pointer to the order sent

        unsigned int error = B_SendOrder(stockHandle,
            side,
            destination,
            size,
            visibilityMode,
            visibleSize,
            price,//const Money& price,//0 for Market
            NULL,//const Money* stopPrice,
            NULL,//const Money* discrtetionaryPrice,
            tif,
            false,//bool proactive,
            true,//bool principalOrAgency, //principal - true, agency - false
            SUMO_ALG_UNKNOWN,//char superMontageAlgorithm,
            false,//bool delayShortTillUptick,
            DE_DEFAULT,
            &orderSent,
            m_account);

        if(error != SO_OK)//error occured
        {
            MessageBox(GetOrderError(error), "Send Order Error", MB_OK|MB_ICONERROR);
        }
        else
        {
        }
    }

    else
    {
        B_GetLevel1(stockHandle)->Add(this);

        std::string sym(symbol);
        OrdersToSend::iterator found = m_ordersToSend.find(sym);
        if(found == m_ordersToSend.end())
        {
            found = m_ordersToSend.insert(OrdersToSend::value_type(std::string(symbol), OrderParamsList())).first;
        }
        
        OrderParamsList& orderParamsList = found->second;
        orderParamsList.m_stockHandle = stockHandle;

        OrderParamsSimple* op = new OrderParamsSimple;
        op->m_side = side;
        strcpy(op->m_destination, destination);
        op->m_price = price;
        op->m_size = size;
        op->m_visibilityMode = visibilityMode;
        op->m_visibleSize = visibleSize;
        op->m_tif = tif;
        orderParamsList.m_orderList.push_back(op);

    }
}
*/
OrderParams* SendOrderDlg::CreateOrderParams() const
{
    return new OrderParamsSimple;
}

unsigned int SendOrderDlg::SendOrder(const StockBase* stockHandle, const OrderParams& o, Order*& orderSent)
{
//    const Order* orderSent;//output parameter. If the order is sent successfully you will get the pointer to the order sent

    const OrderParamsSimple& op = (const OrderParamsSimple&)o;
    unsigned int error = B_SendOrder(stockHandle,
        op.m_side,
        op.m_destination,
        op.m_size,
        op.m_visibilityMode,
        op.m_visibleSize,
        op.m_price,//const Money& price,//0 for Market
        NULL,//const Money* stopPrice,
        NULL,//const Money* discrtetionaryPrice,
        op.m_tif,
        false,//bool proactive,
        true,//bool principalOrAgency, //principal - true, agency - false
        SUMO_ALG_UNKNOWN,//char superMontageAlgorithm,
        OS_RESIZE,
        //false,//bool delayShortTillUptick,
        op.m_listedDestinationExchange,
        &orderSent,
        m_account,
        0,
        false,
        101, NULL);
/*
    if(orderSent)
    {
//Set user description of the order if you want to.
//Look at function call in AnvilExtTest.cpp: B_AddUserOrderDescriptions(101, "Ext. Order");
//The "Ext. Order" string will be visible in Anvil's "User Data" column in "Orders" window.
        orderSent->SetUserDescription(101);
    }
*/
    return error;
}

BOOL SendOrderDlg::OnInitDialog() 
{
	SendOrderBaseDlg::OnInitDialog();
	
	// TODO: Add extra initialization here
    m_EditDestination.SetLimitText(4);
    m_EditVisibleSize.SetLimitText(5);

    m_RadioDefault.SetCheck(BST_CHECKED);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}


// SendStopOrderDlg.cpp : implementation file
//

#include "stdafx.h"
#include "Resource.h"
#include "SendStopOrderDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// SendStopOrderDlg dialog


SendStopOrderDlg::SendStopOrderDlg(CWnd* pParent):
    SendOrderBaseDlg(SendStopOrderDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(SendStopOrderDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}

OrderParams* SendStopOrderDlg::CreateOrderParams() const
{
    return new OrderParamsStop;
}

void SendStopOrderDlg::DoDataExchange(CDataExchange* pDX)
{
	SendOrderBaseDlg::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(SendStopOrderDlg)
	DDX_Control(pDX, IDB_STOPLOSS_ONLY, m_CheckBoxStopLossOnly);
	DDX_Control(pDX, IDE_SENDORDER_STOPPRICE_TENTHS, m_EditStopPriceTenths);
	DDX_Control(pDX, IDE_SENDORDER_STOPPRICE_DOLLARS, m_EditStopPriceDollars);
	DDX_Control(pDX, IDE_SENDORDER_STOPPRICE_CENTS, m_EditStopPriceCents);
	DDX_Control(pDX, IDB_SENDSTOPORDER_PQD_SIZE, m_RadioPostQuoteDestinationSize);
	DDX_Control(pDX, IDB_SENDSTOPORDER_PQD_ISLD, m_RadioPostQuoteDestinationIsld);
	DDX_Control(pDX, IDB_SENDSTOPORDER_PRICE, m_RadioTriggerPrice);
	DDX_Control(pDX, IDB_SENDSTOPORDER_SAMESIDEQUOTE, m_RadioTriggerSameSideQuote);
	DDX_Control(pDX, IDB_SENDSTOPORDER_OPPOSITESIDEQUOTE, m_RadioTriggerOppositeSideQuote);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(SendStopOrderDlg, SendOrderBaseDlg)
	//{{AFX_MSG_MAP(SendStopOrderDlg)
		// NOTE: the ClassWizard will add message map macros here
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// SendStopOrderDlg message handlers
unsigned int SendStopOrderDlg::FillOrderParams(OrderParams* o)
{
    unsigned int ret = SendOrderBaseDlg::FillOrderParams(o);
    if(ret != 0)
    {
        return ret;
    }
/*
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

    strcpy(op->m_destination, destination);
    op->m_visibilityMode = visibilityMode;
    op->m_visibleSize = visibleSize;
    op->m_tif = tif;
*/

//STOPPRICE
    char buf[50];
    m_EditStopPriceDollars.GetWindowText(buf, sizeof(buf));
    unsigned int dollars = atoi(buf);
    m_EditStopPriceCents.GetWindowText(buf, sizeof(buf));
    unsigned short cents = atoi(buf);
    m_EditStopPriceTenths.GetWindowText(buf, sizeof(buf));
    unsigned short tenths = atoi(buf);

    Money stopPrice(dollars, cents * 10 + tenths);
    if(stopPrice.isZero())
    {
        return 5;
    }

    StopTriggerType triggerType;
    if(m_RadioTriggerSameSideQuote.GetCheck() == BST_CHECKED)
    {
        triggerType = TT_SAMESIDEQUOTE;
    }
    else if(m_RadioTriggerOppositeSideQuote.GetCheck() == BST_CHECKED)
    {
        triggerType = TT_OPPOSITESIDEQUOTE;
    }
    else if(m_RadioTriggerPrice.GetCheck() == BST_CHECKED)
    {
        triggerType = TT_PRICE;
    }
    else
    {
        return 4;
    }

    const char* postQuoteDestionation;
    if(m_RadioPostQuoteDestinationSize.GetCheck() == BST_CHECKED)
    {
        postQuoteDestionation = "SIZE";
    }
    else
    {
        postQuoteDestionation = "ISLD";
    }

    OrderParamsStop* op = (OrderParamsStop*)o;
    op->m_stopPrice = stopPrice;
    strcpy_s(op->m_postQuoteDestination, sizeof(op->m_postQuoteDestination), postQuoteDestionation);
    op->m_triggerType = triggerType;
    
	op->m_stopLossOnly = m_CheckBoxStopLossOnly.GetCheck() == BST_CHECKED;

    return 0;
}

unsigned int SendStopOrderDlg::SendOrder(const StockBase* stockHandle, const OrderParams& o, Order*& orderSent)
{
//    const Order* orderSent;//output parameter. If the order is sent successfully you will get the pointer to the order sent
/*
    return B_SendOrder(stockHandle,
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
        false,//bool delayShortTillUptick,
        DE_DEFAULT,
        &orderSent,
        m_account);
*/

    bool price2DecPlaces = true;
    const OrderParamsStop& op = (const OrderParamsStop&)o;
    Money insideMarketQuote;

    if(B_GetSafeInsidePrice(
        stockHandle,
        op.m_side == 'B',//bool buy
        op.m_side != 'B',//bool side
		false,//ecnsOnly
        insideMarketQuote))
    {
        Money limitPriceOffset;
        const Money* limitPriceOffsetPtr;
        if(op.m_price.isZero())
        {
            limitPriceOffsetPtr = NULL;//Stop Market order
        }
        else
        {
            if(op.m_side == 'B')
            {
                limitPriceOffset = op.m_price - insideMarketQuote;
            }
            else
            {
                limitPriceOffset = insideMarketQuote - op.m_price;
            }
            limitPriceOffsetPtr = &limitPriceOffset;
        }

        Money stopPriceOffset;
        if(op.m_side == 'B')
        {
            stopPriceOffset = op.m_stopPrice - insideMarketQuote;
        }
        else
        {
            stopPriceOffset = insideMarketQuote - op.m_stopPrice;
        }

        B_SendSmartStopOrder(stockHandle,
            op.m_side,
            op.m_size,
            limitPriceOffsetPtr,//const Money* priceOffset,//NULL for Stop Market
            stopPriceOffset,
            price2DecPlaces,
            true,//bool ecnsOnlyBeforeAfterMarket,
            false,//bool mmsBasedForNyse,
            TIF_DAY,//unsigned int stopTimeInForce,
            op.m_tif,//unsigned int timeInForceAfterStopReached,
            op.m_postQuoteDestination,
            NULL,//const char* redirection,
            false,//bool proactive,
            true,//bool principalOrAgency, //principal - true, agency - false
            SUMO_ALG_UNKNOWN,//char superMontageAlgorithm,
            OS_RESIZE,
//            false,//bool delayShortTillUptick,
            DE_DEFAULT,//unsigned int destinationExchange,
            op.m_triggerType,//StopTriggerType triggerType,
            false,
            0, "Ext. Stop Order",
			NULL,//const char* regionalProactiveDestination,
			STPT_ALL,
			Money(0, 200),
			op.m_stopLossOnly,
            &orderSent,
            m_account);
/*
        if(orderSent)
        {
//Set user description of the order if you want to.
//The "Ext. Stop Order" string will be visible in Anvil's "User Data" column in "Orders" window.
            orderSent->SetUserDescription(0, "Ext. Stop Order");
        }
*/
    }
    return SO_OK;
}


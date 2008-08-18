// PosOrdersExecsDlg.cpp : implementation file
//

#include "stdafx.h"
#include "Resource.h"
#include "PosOrdersExecsDlg.h"
#include "ExtFrame.h"
#include "BusinessApi.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif




int ListBoxPosition::SearchCompare(const void* item1, const void* item2) const
{
    const Position* pos1 = (const Position*)item1;
    const Position* pos2 = (const Position*)item2;
    int result = strcmp(pos1->GetSymbol(), pos2->GetSymbol());
    return result < 0 ? -1 : result > 0 ? 1 : 0;
}


//Draw a Position
void ListBoxPosition::DoDrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct)
{
    CDC* dc = CDC::FromHandle(lpDrawItemStruct->hDC);

    const Position* position = (const Position*)lpDrawItemStruct->itemData;
    int posSize = position->GetSize();
    COLORREF color;
    if(posSize == 0)
    {
        color = m_textColor;
    }
    else if(posSize > 0)
    {
        color = RGB(0, 128, 0);
    }
    else
    {
        color = RGB(128, 0, 0);
    }

	if(lpDrawItemStruct->itemState & ODS_SELECTED)
    {
        dc->FillSolidRect(&lpDrawItemStruct->rcItem, color);//GetSysColor(COLOR_HIGHLIGHT));
        dc->SetTextColor(m_bkColor);
    }
    else
    {
        dc->FillSolidRect(&lpDrawItemStruct->rcItem, m_bkColor);
        dc->SetTextColor(color);
    }
    dc->SetBkMode(TRANSPARENT);

    CRect r(lpDrawItemStruct->rcItem);
    r.left += 3;
    r.right = r.left + 40;
    dc->DrawText(position->GetSymbol(), -1, &r, DT_LEFT|DT_VCENTER|DT_SINGLELINE);

    char num[33];
    r.left = r.right;
    r.right = r.left + 50;
	_itoa_s(position->GetSize(), num, sizeof(num), 10);
    dc->DrawText(num, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 50;
    CString str = "";
    ExtFrame::FormatMoney(str, position->GetOpenPnl());
    dc->DrawText(str, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 50;
    str = "";
    ExtFrame::FormatMoney(str, position->GetClosedPnl());
    dc->DrawText(str, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 30;
	_ultoa_s(position->GetCountOrdersPendingBuy(), num, sizeof(num), 10);
    dc->DrawText(num, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 30;
	_ultoa_s(position->GetCountOrdersPendingSell(), num, sizeof(num), 10);
    dc->DrawText(num, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);
}

int ListBoxOrder::SearchCompare(const void* item1, const void* item2) const
{
    const Order* order1 = (const Order*)item1;
    const Order* order2 = (const Order*)item2;
    unsigned int chron1 = order1->GetChronologyOrdinal();
    unsigned int chron2 = order2->GetChronologyOrdinal();
    return chron1 < chron2 ? 1 : chron1 > chron2 ? -1 : 0;
}

//Draw an Order
void ListBoxOrder::DoDrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct)
{
    CDC* dc = CDC::FromHandle(lpDrawItemStruct->hDC);

    const Order* order = (const Order*)lpDrawItemStruct->itemData;
    char side = order->GetSide();
    COLORREF color;
    if(order->isDead())
    {
        if(order->GetSize() == order->GetExecutedSize())
        {
            color = RGB(0, 128, 0);
        }
        else
        {
            color = RGB(128, 0, 0);
        }
    }
    else if(order->GetExecutedSize() == 0)
    {
        color = RGB(0, 0, 0);
    }
    else
    {
        color = RGB(128, 128, 0);
    }

	if(lpDrawItemStruct->itemState & ODS_SELECTED)
    {
        dc->FillSolidRect(&lpDrawItemStruct->rcItem, color);//, GetSysColor(COLOR_HIGHLIGHT));
        dc->SetTextColor(m_bkColor);
    }
    else
    {
        dc->FillSolidRect(&lpDrawItemStruct->rcItem, m_bkColor);
        dc->SetTextColor(color);
    }
    dc->SetBkMode(TRANSPARENT);

    CRect r(lpDrawItemStruct->rcItem);
    r.left += 3;
    r.right = r.left + 40;
    dc->DrawText(order->GetSymbol(), -1, &r, DT_LEFT|DT_VCENTER|DT_SINGLELINE);


    CTime ct(order->GetTimeEntered());
    r.left = r.right;
    r.right = r.left + 70;
    dc->DrawText(ct.Format("%H:%M:%S"), -1, &r, DT_LEFT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 30;
    dc->DrawText(&side, 1, &r, DT_CENTER|DT_VCENTER|DT_SINGLELINE);

    char num[33];
    r.left = r.right;
    r.right = r.left + 80;
	_ultoa_s(order->GetId(), num, sizeof(num), 10);
    dc->DrawText(num, -1, &r, DT_LEFT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 40;
	_ultoa_s(order->GetSize(), num, sizeof(num), 10);
    dc->DrawText(num, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 40;
	_ultoa_s(order->GetExecutedSize(), num, sizeof(num), 10);
    dc->DrawText(num, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 50;
    CString str = "";
    if(order->isMarketOrder())
    {
        str = "Market";
    }
    else
    {
        ExtFrame::FormatMoney(str, *order);
    }
    dc->DrawText(str, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 50;
    str = "";
    ExtFrame::FormatMoney(str, order->GetExecutedPrice());
    dc->DrawText(str, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 40;
    dc->DrawText(order->GetDestination(), -1, &r, DT_CENTER|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 40;
    std::string text;
    ExtFrame::FormatTif(order->GetTimeInForce(), text);
    dc->DrawText(text.c_str(), -1, &r, DT_CENTER|DT_VCENTER|DT_SINGLELINE);
}


int ListBoxExecution::SearchCompare(const void* item1, const void* item2) const
{
    const Execution* exec1 = (const Execution*)item1;
    const Execution* exec2 = (const Execution*)item2;
    unsigned int chron1 = exec1->GetChronologyOrdinal();
    unsigned int chron2 = exec2->GetChronologyOrdinal();
    return chron1 < chron2 ? 1 : chron1 > chron2 ? -1 : 0;
}

//Draw an Execution
void ListBoxExecution::DoDrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct)
{
    CDC* dc = CDC::FromHandle(lpDrawItemStruct->hDC);

    const Execution* exec = (const Execution*)lpDrawItemStruct->itemData;
    char side = exec->GetSide();
    COLORREF color;
    if(side == 'B')
    {
        color = RGB(0, 128, 0);
    }
    else
    {
        color = RGB(128, 0, 0);
    }

	if(lpDrawItemStruct->itemState & ODS_SELECTED)
    {
        dc->FillSolidRect(&lpDrawItemStruct->rcItem, color);//, GetSysColor(COLOR_HIGHLIGHT));
        dc->SetTextColor(m_bkColor);
    }
    else
    {
        dc->FillSolidRect(&lpDrawItemStruct->rcItem, m_bkColor);
        dc->SetTextColor(color);
    }
    dc->SetBkMode(TRANSPARENT);

    CRect r(lpDrawItemStruct->rcItem);
    r.left += 3;
    r.right = r.left + 40;
    dc->DrawText(exec->GetSymbol(), -1, &r, DT_LEFT|DT_VCENTER|DT_SINGLELINE);

    CTime ct(exec->GetTimeEntered());
    r.left = r.right;
    r.right = r.left + 70;
    dc->DrawText(ct.Format("%H:%M:%S"), -1, &r, DT_LEFT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 50;
    CString str;
    str = "";
    ExtFrame::FormatMoney(str, exec->GetOrderPriceSize());
    dc->DrawText(str, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 50;
    str = "";
    ExtFrame::FormatMoney(str, *exec);
    dc->DrawText(str, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    char num[33];
    r.left = r.right;
    r.right = r.left + 40;
	_ultoa_s(exec->GetSize(), num, sizeof(num), 10);
    dc->DrawText(num, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 80;
    r.left += 4;
	_ultoa_s(exec->GetId(), num, sizeof(num), 10);
    dc->DrawText(num, -1, &r, DT_CENTER|DT_VCENTER|DT_SINGLELINE);
}

/////////////////////////////////////////////////////////////////////////////
// PosOrdersExecsDlg dialog


PosOrdersExecsDlg::PosOrdersExecsDlg(CWnd* pParent):
    BaseDlg(PosOrdersExecsDlg::IDD, pParent),
    m_staticHeight(0),
    m_allStatisHeight(0),
    m_lbTitleHeight(0),
    m_vertGap(3),
    m_account(NULL)
{
	//{{AFX_DATA_INIT(PosOrdersExecsDlg)
	//}}AFX_DATA_INIT

//
    m_positionTitle.push_back(WindowLocation(&m_StaticPosSymbol, 40, DT_LEFT));
    m_positionTitle.push_back(WindowLocation(&m_StaticPosSize, 50, DT_RIGHT));
    m_positionTitle.push_back(WindowLocation(&m_StaticPosOpenPnl, 50, DT_RIGHT));
    m_positionTitle.push_back(WindowLocation(&m_StaticPosClosedPnl, 50, DT_RIGHT));
    m_positionTitle.push_back(WindowLocation(&m_StaticPosPendingBuyOrderCount, 30, DT_RIGHT));
    m_positionTitle.push_back(WindowLocation(&m_StaticPosPendingSellOrderCount, 30, DT_RIGHT));

    m_orderTitle.push_back(WindowLocation(&m_StaticOrderSymbol, 40, DT_LEFT));
	m_orderTitle.push_back(WindowLocation(&m_StaticOrderTime, 70, DT_LEFT));
    m_orderTitle.push_back(WindowLocation(&m_StaticOrderSide, 30, DT_CENTER));
    m_orderTitle.push_back(WindowLocation(&m_StaticOrderId, 80, DT_CENTER));
    m_orderTitle.push_back(WindowLocation(&m_StaticOrderSize, 40, DT_RIGHT));
	m_orderTitle.push_back(WindowLocation(&m_StaticOrderExecSize, 40, DT_RIGHT));
	m_orderTitle.push_back(WindowLocation(&m_StaticOrderPrice, 50, DT_RIGHT));
	m_orderTitle.push_back(WindowLocation(&m_StaticOrderExecPrice, 50, DT_RIGHT));
	m_orderTitle.push_back(WindowLocation(&m_StaticOrderDestination, 40, DT_CENTER));
	m_orderTitle.push_back(WindowLocation(&m_StaticOrderTif, 40, DT_CENTER));

    m_executionTitle.push_back(WindowLocation(&m_StaticExecSymbol, 40, DT_LEFT));
	m_executionTitle.push_back(WindowLocation(&m_StaticExecTime, 70, DT_LEFT));
    m_executionTitle.push_back(WindowLocation(&m_StaticExecOrderPrice, 50, DT_RIGHT));
    m_executionTitle.push_back(WindowLocation(&m_StaticExecPrice, 50, DT_RIGHT));
    m_executionTitle.push_back(WindowLocation(&m_StaticExecSize, 50, DT_RIGHT));
	m_executionTitle.push_back(WindowLocation(&m_StaticExecOrderId, 80, DT_CENTER));
}


void PosOrdersExecsDlg::DoDataExchange(CDataExchange* pDX)
{
	BaseDlg::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(PosOrdersExecsDlg)
	DDX_Control(pDX, IDS_ORDER_SIDE, m_StaticOrderSide);
	DDX_Control(pDX, IDS_EXECUTION_ORDER_PRICE, m_StaticExecOrderPrice);
	DDX_Control(pDX, IDS_EXECUTION_PRICE, m_StaticExecPrice);
	DDX_Control(pDX, IDS_ORDER_TIME, m_StaticOrderTime);
	DDX_Control(pDX, IDS_EXECUTION_TIME, m_StaticExecTime);
	DDX_Control(pDX, IDS_EXEC_ORDER_ID, m_StaticExecOrderId);
	DDX_Control(pDX, IDS_ORDER_TIF, m_StaticOrderTif);
	DDX_Control(pDX, IDS_ORDER_PRICE, m_StaticOrderPrice);
	DDX_Control(pDX, IDS_ORDER_EXEC_SIZE, m_StaticOrderExecSize);
	DDX_Control(pDX, IDS_ORDER_EXEC_PRICE, m_StaticOrderExecPrice);
	DDX_Control(pDX, IDS_ORDER_DESTINATION, m_StaticOrderDestination);
	DDX_Control(pDX, IDS_POSITION_PENDING_SELL_ORDER_COUNT, m_StaticPosPendingSellOrderCount);
	DDX_Control(pDX, IDS_POSITION_PENDING_BUY_ORDER_COUNT, m_StaticPosPendingBuyOrderCount);
	DDX_Control(pDX, IDS_POSITION_CLOSED_PNL, m_StaticPosClosedPnl);
	DDX_Control(pDX, IDS_POSITION_SYMBOL, m_StaticPosSymbol);
	DDX_Control(pDX, IDS_POSITION_SIZE, m_StaticPosSize);
	DDX_Control(pDX, IDS_POSITION_OPEN_PNL, m_StaticPosOpenPnl);
	DDX_Control(pDX, IDS_ORDER_SYMBOL, m_StaticOrderSymbol);
	DDX_Control(pDX, IDS_ORDER_SIZE, m_StaticOrderSize);
	DDX_Control(pDX, IDS_ORDER_ID, m_StaticOrderId);
	DDX_Control(pDX, IDS_EXECUTION_SYMBOL, m_StaticExecSymbol);
	DDX_Control(pDX, IDS_EXECUTION_SIZE, m_StaticExecSize);
	DDX_Control(pDX, IDS_POSITIONS_NAME, m_StaticPositionsName);
	DDX_Control(pDX, IDS_EXECUTIONS_NAME, m_StaticExecutionsName);
	DDX_Control(pDX, IDS_ORDERS_NAME, m_StaticOrdersName);
	DDX_Control(pDX, IDL_POSITIONS, m_ListBoxPositions);
	DDX_Control(pDX, IDL_ORDERS, m_ListBoxOrders);
	DDX_Control(pDX, IDL_EXECUTIONS, m_ListBoxExecutions);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(PosOrdersExecsDlg, BaseDlg)
	//{{AFX_MSG_MAP(PosOrdersExecsDlg)
	ON_WM_SIZE()
	//}}AFX_MSG_MAP
	ON_MESSAGE(WM_EXITSIZEMOVE, OnExitSizeMove)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// PosOrdersExecsDlg message handlers

LRESULT PosOrdersExecsDlg::OnExitSizeMove(WPARAM w, LPARAM l) 
{
//    RepaintTitle(m_positionTitle);
    RepaintTitle(m_orderTitle);
    RepaintTitle(m_executionTitle);
	m_StaticExecutionsName.Invalidate();
	m_StaticOrdersName.Invalidate();
	m_ListBoxPositions.Invalidate();
	m_ListBoxOrders.Invalidate();
	m_ListBoxExecutions.Invalidate();
	return 0;
}

BOOL PosOrdersExecsDlg::OnInitDialog()
{
	BaseDlg::OnInitDialog();

//Getthe current account and use the object to iterate through existing Orders, Positions, Executions
    m_account = B_GetCurrentAccount();

    if(m_account)
    {
        CString text;
        GetWindowText(text);
        std::string title(text);
        title += " - ";
        title += B_GetAccountName(m_account);
        SetWindowText(title.c_str());

//Add this Dialog as an Observer to the account. Account will notify this dialog about new Orders, Positions, Executions or about
//changes in existing ones. The notification is done through a virtual function Process (see below).
//Whenever anOrder is created or updated or an Execution is created, function Process is called
        m_account->Add(this);

//Initialize contents of  the list boxes:
//When we create the Dialog box we should fill the list boxes with existing Orders, Positions, Executions. 
        void* iterator = B_CreatePositionIterator(POSITION_FLAT|POSITION_LONG|POSITION_SHORT, (1 << ST_LAST) - 1,m_account);
        B_StartIteration(iterator);
        const Position* pos;
        while(pos = B_GetNextPosition(iterator))
        {
            m_ListBoxPositions.AddString((const char*)pos);
        }
        B_DestroyIterator(iterator);

        iterator = B_CreateOrderIterator(OS_CANCELLED|OS_FILLED|OS_PENDINGLONG|OS_PENDINGSHORT, (1 << ST_LAST) - 1,m_account);
        B_StartIteration(iterator);
        const Order* order;
        while(order = B_GetNextOrder(iterator))
        {
            m_ListBoxOrders.AddString((const char*)order);
        }
        B_DestroyIterator(iterator);

        iterator = B_CreateExecutionIterator(m_account);
        B_StartIteration(iterator);
        const Execution* exec;
        while(exec = B_GetNextExecution(iterator))
        {
            m_ListBoxExecutions.AddString((const char*)exec);
        }
        B_DestroyIterator(iterator);
    }

    CRect r;
    m_StaticPositionsName.GetWindowRect(&r);
    m_staticHeight = r.Height();
    m_lbTitleHeight = 2 * m_staticHeight + 3 * m_vertGap;
    m_allStatisHeight = 3 * m_lbTitleHeight;
    m_StaticPositionsName.SetWindowPos(NULL, 0, m_vertGap, 0, 0, SWP_NOZORDER|SWP_NOSIZE);

    ArrangeTitle(m_positionTitle, m_vertGap + m_staticHeight + m_vertGap);
    Resize(true);
	return TRUE;
}

void PosOrdersExecsDlg::ArrangeTitle(ControlList& controlList, int y)
{
    ControlList::iterator it;
    ControlList::iterator itend = controlList.end();
    CRect r;
    CWnd* window;
    int offset;
    int currentLocation = 5;
    for(it = controlList.begin(); it != itend; ++it)
    {
        window = it->m_window;
        if(it->m_alignment == DT_LEFT)
        {
            window->SetWindowPos(NULL, currentLocation, y, 0, 0, SWP_NOZORDER|SWP_NOSIZE);
        }
        else
        {
            window->GetWindowRect(&r);
            offset = it->m_width - r.Width();
            if(offset < 0)
            {
                offset = 0;
            }
            else if(it->m_alignment == DT_CENTER)
            {
                offset >>= 1;
            }
            window->SetWindowPos(NULL, currentLocation + offset, y, 0, 0, SWP_NOZORDER|SWP_NOSIZE);
        }
        currentLocation += it->m_width;
    }
}

void PosOrdersExecsDlg::RepaintTitle(ControlList& controlList)
{
    ControlList::iterator it;
    ControlList::iterator itend = controlList.end();
    for(it = controlList.begin(); it != itend; ++it)
    {
        it->m_window->Invalidate();
    }
}

void PosOrdersExecsDlg::Resize(bool vertChanged)
{
	if(m_ListBoxPositions.m_hWnd != NULL)
    {
        int allLbHeight = m_dimensions.cy - m_allStatisHeight;
        int lbHeight = allLbHeight / 3;
        int lastLbHeight = allLbHeight - 2 * lbHeight;
	    m_ListBoxPositions.SetWindowPos(NULL, 0, m_lbTitleHeight, m_dimensions.cx, lbHeight, SWP_NOZORDER);
	    m_ListBoxOrders.SetWindowPos(NULL, 0, 2 * m_lbTitleHeight + lbHeight, m_dimensions.cx, lbHeight, SWP_NOZORDER);
	    m_ListBoxExecutions.SetWindowPos(NULL, 0, 3 * m_lbTitleHeight + 2 * lbHeight, m_dimensions.cx, allLbHeight - 2 * lbHeight, SWP_NOZORDER);

        if(vertChanged)
        {
            m_StaticOrdersName.SetWindowPos(NULL, 0, m_lbTitleHeight + lbHeight + m_vertGap, 0, 0, SWP_NOZORDER|SWP_NOSIZE);
            ArrangeTitle(m_orderTitle, m_lbTitleHeight + lbHeight + 2 * m_vertGap + m_staticHeight);

            m_StaticExecutionsName.SetWindowPos(NULL, 0, 2 * m_lbTitleHeight + 2 * lbHeight + m_vertGap, 0, 0, SWP_NOZORDER|SWP_NOSIZE);
            ArrangeTitle(m_executionTitle, 2 * m_lbTitleHeight + 2 * lbHeight + 2 * m_vertGap + m_staticHeight);
        }

    }
}

void PosOrdersExecsDlg::OnSize(UINT nType, int cx, int cy)
{
	BaseDlg::OnSize(nType, cx, cy);
    bool vertChanged = m_dimensions.cy != cy;
    m_dimensions.cx = cx;
    m_dimensions.cy = cy;
    Resize(vertChanged);
}



void PosOrdersExecsDlg::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
    switch(message->GetType())
    {
        case M_POOL_EXECUTION:
//Notification about a new execution.
        if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_EXECUTION)
        {
//this is the original structure coming from the server:
/*
class MsgPoolExecution : public Message
{
public:
	MsgPoolExecution() : Message(M_POOL_EXECUTION, sizeof(MsgPoolExecution)), x_BranchSeqId(0), m_bDecimal(1){}

	int 		    x_ExecutionPrice;
	long		    x_NumberOfShares;
	long		    x_NumberOfSharesLeft;
	bool		    x_bMoreShares;
	time_t		    x_Time;
    char		    x_UserId[LENGTH_SYMBOL];
	char		    x_Symbol[LENGTH_SYMBOL];
	unsigned int	x_OrderId;
	char		    x_CounterParty[LENGTH_SYMBOL];
	char		    x_ExecutionType[4]; // SOEE for SOES, EEXO for Selectnet.
	int			    x_BranchSeqId;
	char		    m_achReference[15];
	char		    m_bDecimal;

};
*/
            MsgPoolExecution* msg = (MsgPoolExecution*)message;//to get the structure, just cast Message* to  MsgPoolExecution* (not used here)

//This is additional info structure prepared by Business.dll. It contains updated objects Position, Order Execution (look in BusinessApi.h).
//You can access objects' fields, but it is not recommended to change them (The fields are protected and you should not play any tricks to modify the fields. It will cause unpredictable results)
            AIMsgExecution* info = (AIMsgExecution*)additionalInfo;
            Order* order = info->m_order;
            const Position* position = info->m_position;
            const Execution* exec = info->m_execution;

//The objects might be in the List Boxes already. If we find the object in the List Box we just repaint it, otherwise add the object to the List Box.
//All this dialog does is displaying the updated objects in the list boxes.
//You can put the objects in other collections and do some decision making bsed on the object updates.
            {
                int index = m_ListBoxExecutions.FindItem(exec);
                if(index < 0)
                {
                    index = m_ListBoxExecutions.AddString((const char*)exec);
                    if(index >= 0)
                    {
                        m_ListBoxExecutions.SetCurSel(index);
                    }
                }
            }
            if(order)//can be NULL for foreign executions
            {
                int index = m_ListBoxOrders.FindItem(order);
                if(index >= 0)
                {
                    m_ListBoxOrders.InvalidateItem(index);
                }
            }
            if(position)
            {
                int index = m_ListBoxPositions.FindItem(position);
                if(index >= 0)
                {
                    m_ListBoxPositions.InvalidateItem(index);
                }
                else
                {
                    index = m_ListBoxPositions.AddString((const char*)position);
                    if(index >= 0)
                    {
                        m_ListBoxPositions.SetCurSel(index);
                    }
                }
            }
        }
        break;

//The following messages are about the new orders or changes to existing orders.
//See the message structures in Messages.h file
//We try to find the order in the List Box and repaint it or add a new item if not found.
        case M_REQ_NEW_ORDER://New Order created and sent out by Business.dll (thriugh function call B_SendOrder).
        case M_POOL_ASSIGN_ORDER_ID://Original order sent has a unigue generated id. The server sends this message to notify you that the order was assigned a new id different from the original. Both ids are part of this notification structure. This message can come 1 or 2 times.
        case M_POOL_UPDATE_ORDER:// Order status is modified
        if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_ORDER)
        {




            AIMsgOrder* info = (AIMsgOrder*)additionalInfo;
            Order* order = info->m_order;
            const Position* position = info->m_position;



            {
                int index = m_ListBoxOrders.FindItem(order);
                if(index >= 0)
                {
                    m_ListBoxOrders.InvalidateItem(index);
                }
                else
                {
                    index = m_ListBoxOrders.AddString((const char*)order);
                    if(index >= 0)
                    {
                        m_ListBoxOrders.SetCurSel(index);
                    }
                }
            }
            
            if(!position)
            {
                position = B_FindPosition(order->GetSymbol(), m_account);
            }
            if(position)
            {
                int index = m_ListBoxPositions.FindItem(position);
                if(index >= 0)
                {
                    m_ListBoxPositions.InvalidateItem(index);
                }
                else
                {
                    index = m_ListBoxPositions.AddString((const char*)position);
                    if(index >= 0)
                    {
                        m_ListBoxPositions.SetCurSel(index);
                    }
                }
            }
        }
        break;

//An Anvil option allows cancelled unexecuted orders to be destroyed.
//We must remove such an order from the List Box.
        case M_ORDER_DELETED:
        {
            Order* order = (Order*)((MsgOrderChange*)message)->m_order;
            int index = m_ListBoxOrders.FindItem(order);
            if(index >= 0)
            {
                m_ListBoxOrders.DeleteString(index);
            }
        }
        break;

    }
}


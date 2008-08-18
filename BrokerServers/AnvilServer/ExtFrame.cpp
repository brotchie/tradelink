// ExtFrame.cpp : implementation file
//

#include "stdafx.h"
#include "Resource.h"
#include "ExtFrame.h"
#include "MarketMakerDlg.h"
#include "LoadDlg.h"
#include "SendOrderDlg.h"
#include "SendStopOrderDlg.h"
#include "CancelOrderDlg.h"
#include "IndexDlg.h"
#include "PosOrdersExecsDlg.h"
#include "SingleBookDlg.h"
#include "MultiBookDlg.h"
#include "TradeLink.h"
#include "Monitor.h"

#include "ObserverApi.h"
#include "BusinessApi.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

ExtFrame* ExtFrame::instance = NULL;


ExtFrame::ExtFrame()
{
    instance = this;
	monitor = new Monitor();
	B_GetAdminObservable()->Add(monitor);
    B_GetAdminObservable()->Add(this);
    m_marketSummaryPopulationDone = B_IsMarketSummaryPopulationDone();
}

ExtFrame::~ExtFrame()
{
	TLUnload();
	delete monitor;
	monitor = NULL;
    instance = NULL;
}

void ExtFrame::ClearDialogs()
{
    for(DlgSet::iterator it = m_dlgSet.begin(); it != m_dlgSet.end(); it = m_dlgSet.begin())
    {
        (*it)->OnCancel();
    }
}

void ExtFrame::RegChannels()
{ // set property "tradelink" on this window so we can find it easily
	CString caption;
	GetWindowTextA(caption);
	HWND h = (HWND)FindWindowA(NULL,caption);
	if (h)
	{
		SetProp(h,"TradeLink",(HANDLE)h);
	}
}

void ExtFrame::DestroyDlg(BaseDlg* dlg)
{
    m_dlgSet.erase(dlg);
    dlg->DestroyWindow();
    delete dlg;
}

BaseDlg* ExtFrame::CreateDlg(unsigned int id)
{
    BaseDlg* dlg;
    switch(id)
    {
        case IDD_SENDSTOPORDER:
        dlg = new SendStopOrderDlg(this);
        break;

        case IDD_SENDORDER:
        dlg = new SendOrderDlg(this);
        break;

        case IDD_CANCELORDER:
        dlg = new CancelOrderDlg(this);
        break;

        case IDD_INDEX:
        dlg = new IndexDlg(this);
        break;

        case IDD_LOAD:
        dlg = new LoadDlg(this);
        break;

        case IDD_MARKETMAKER:
        dlg = new MarketMakerDlg(this);
        break;

        case IDD_POS_ORDERS_EXECS:
        dlg = new PosOrdersExecsDlg(this);
        break;

        case IDD_SINGLE_BOOK:
        dlg = new SingleBookDlg(this);
        break;

        case IDD_MULTI_BOOK:
        dlg = new MultiBookDlg(this);
        break;

		
		default:
        dlg = NULL;
        break;
    }
    if(dlg)
    {
        if(dlg->CreateTradeDialog() == TRUE)
        {
            m_dlgSet.insert(dlg);
        }
    }
    return dlg;
}
BEGIN_MESSAGE_MAP(ExtFrame, CFrameWnd)
	//{{AFX_MSG_MAP(ExtFrame)
	ON_COMMAND(IDM_ACCOUNT, OnAccount)
	ON_COMMAND(IDM_POSITIONS, OnPositions)
	ON_COMMAND(IDM_ORDERS, OnOrders)
	ON_COMMAND(IDM_QUOTES, OnQuotes)
	ON_COMMAND(IDM_BOOKQUOTES, OnBookQuotes)
	ON_COMMAND(IDM_AGRBOOKQUOTES, OnAgrBookQuotes)
	ON_COMMAND(IDM_LEVEL1, OnLevel1)
	ON_COMMAND(IDM_SUBSCRIPTION, OnSubscription)
	ON_WM_COPYDATA()
	ON_WM_DESTROY()
	ON_WM_SYSCOMMAND()
	ON_COMMAND(IDM_LOADSTOCKS, OnLoadStocks)
	ON_COMMAND(IDM_SENDORDER, OnSendOrder)
	ON_COMMAND(IDM_CANCELORDER, OnCancelOrder)
	ON_COMMAND(IDM_INDEX, OnIndex)
	ON_COMMAND(IDM_SENDSTOPORDER, OnSendStopOrder)
	ON_COMMAND(IDM_POS_ORDERS_EXECS, OnPositionsOrdersExecutions)
	ON_COMMAND(IDM_SINGLEBOOK, OnSingleBook)
	ON_COMMAND(IDM_MULTIBOOK, OnMultiBook)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// ExtFrame message handlers

void ExtFrame::FormatTimeToken(unsigned int i, std::string& str)
{
    int j = i / 10;
    str += '0' + j;
    str += '0' + i - j * 10;
}

void ExtFrame::FormatTif(unsigned int tif, std::string& text)
{
    switch(tif)
    {
        case TIF_IOC:
        text = "IOC";
        break;

        case TIF_ISLAND_DAY:
        text = "Day";
        break;

        case TIF_ISLAND_EXTENDED_DAY:
        text = "Ext";
        break;

        case TIF_GTC:
        text = "GTC";
        break;

        case TIF_OPENING:
        text = "OPEN";
        break;

        case TIF_DAY:
        text = "DAY";
        break;

        default:
        {
            char num[33];
            unsigned int hours = tif / 3600;
            tif -= 3600 * hours;
            unsigned int minutes = tif / 60;
            tif -= 60 * minutes;
            text = "";
            if(hours)
            {
                if(hours < 10)
                {
                    text += '0';
                }
				_ultoa_s(hours, num, sizeof(num), 10);
                text += num;
                text += ':';
            }
            if(minutes)
            {
                FormatTimeToken(minutes, text);
                text += ':';
            }
            else if(hours)
            {
                text += "00:";
            }
            FormatTimeToken(tif, text);
        }
        break;
    }
}

void ExtFrame::FormatMoney(CString& dest, const Money& money)
{
    FormatDollarsAndCents(dest, money.GetWhole(), money.GetThousandsFraction());
}

void ExtFrame::FormatDollarsAndCents(CString& dest, int dollars, int cents)
{
    char num[33]; 
	_itoa_s(dollars, num, sizeof(num), 10);
    dest += num;
    dest += ".";
    if(cents < 0)
    {
        cents = -cents;
    }
    _itoa_s(cents, num, sizeof(num), 10);
    for(unsigned int i = (unsigned int)strlen(num); i < 3; i++)
    {
        dest += "0";
    }
    dest += num;
}

void ExtFrame::FormatMoneySize(CString& dest, const MoneySize& money)
{
    FormatMoney(dest, money);
    char num[33];
    dest += "  ";
	_itoa_s(money.GetSize(), num, sizeof(num), 10);
    dest += num;
}

void ExtFrame::OnAccount() 
{
    Observable* account = B_GetCurrentAccount();
    if(account)
    {
        char num[33];
        CString accountInfo;
        accountInfo += "Name ";
        accountInfo += B_GetAccountName(account);

        accountInfo += "\nFirm ";
        accountInfo += B_GetAccountFirm(account);
        
        accountInfo += "\nSimulation ";
        accountInfo += B_IsAccountSimulation(account) ? "Yes" : "No";
        
        accountInfo += "\nMaxOpenPosMoney ";
        FormatMoney(accountInfo, B_GetAccountMaxOpenPositionValue(account));
        
        accountInfo += "\nMaxOpenPosSize ";
		_itoa_s(B_GetMaxOpenPositionSize(account), num, sizeof(num), 10);
        accountInfo += num;
        
        accountInfo += "\nMaxOrderSize ";
		_itoa_s(B_GetMaxOrderSize(account), num, sizeof(num), 10);
        accountInfo += num;
        
        accountInfo += "\nBuyingPower ";
        FormatMoney(accountInfo, B_GetAccountBuyingPower(account));

        accountInfo += "\nBuyingPower in use ";
        FormatMoney(accountInfo, B_GetAccountBuyingPowerInUse(account));

        accountInfo += "\nOpen P&&L ";
        FormatMoney(accountInfo, B_GetAccountOpenPnl(account));

        accountInfo += "\nClosed P&&L ";
        FormatMoney(accountInfo, B_GetAccountClosedPnl(account));

        accountInfo += "\nSharesTraded ";
		_itoa_s(B_GetAccountSharesTraded(account), num, sizeof(num), 10);
        accountInfo += num;
        
        accountInfo += "\nLongPositions ";
		_itoa_s(B_GetLongPositionsCount(account), num, sizeof(num), 10);
        accountInfo += num;
        
        accountInfo += "\nShortPositions ";
		_itoa_s(B_GetShortPositionsCount(account), num, sizeof(num), 10);
        accountInfo += num;
        
        accountInfo += "\nMoney Invested ";
        FormatMoney(accountInfo, B_GetAccountMoneyInvested(account));

        accountInfo += "\nMoney Traded Long ";
        FormatMoney(accountInfo, B_GetAccountMoneyTradedLong(account));

        accountInfo += "\nMoney Traded Short ";
        FormatMoney(accountInfo, B_GetAccountMoneyTradedShort(account));

        accountInfo += "\nMoney Traded ";
        FormatMoney(accountInfo, B_GetAccountMoneyTraded(account));

        AfxGetMainWnd()->MessageBox(accountInfo, "Account", MB_OK);
    }
/*
Observable* WINAPI B_SetCurrentAccount(const char* name);
Observable* WINAPI B_GetAccount(const char* name);
Order* WINAPI B_CancelLastOrder(CO_BUY | CO_SELL | CO_NONDAY | CO_DAY, const Observable* account = NULL);
void WINAPI B_CancelAllOrders(CO_BUY | CO_SELL | CO_NONDAY | CO_DAY, const Observable* account = NULL);
void WINAPI B_CancelAllStockOrders(const char* symbol, CO_BUY | CO_SELL | CO_NONDAY | CO_DAY, const Observable* account = NULL);
*/
}

void ExtFrame::OnPositions() 
{
    Observable* account = B_GetCurrentAccount();
    if(!account)
    {
        return;
    }
    void* iterator = B_CreatePositionIterator(POSITION_FLAT|POSITION_LONG|POSITION_SHORT,(1 << ST_LAST) - 1, account);
    B_StartIteration(iterator);
    const Position* pos;
    CString posstr;
    char num[33];
    while(pos = B_GetNextPosition(iterator))
    {
        posstr += "\n";
        posstr += pos->GetSymbol();
        posstr += " Size=";
		_itoa_s(pos->GetSize(), num, sizeof(num), 10);
        posstr += num;
        posstr += " PendingLong=";
		_itoa_s(pos->GetSharesPendingLong(), num, sizeof(num), 10);
        posstr += num;
        posstr += " PendingShort=";
		_itoa_s(pos->GetSharesPendingShort(), num, sizeof(num), 10);
        posstr += num;
        posstr += " PendingOrdersLong=";
		_itoa_s(pos->GetCountOrdersPendingBuy(), num, sizeof(num), 10);
        posstr += num;
        posstr += " PendingOrdersShort=";
		_itoa_s(pos->GetCountOrdersPendingSell(), num, sizeof(num), 10);
        posstr += num;
        posstr += " Bullets=";
		_itoa_s(pos->GetBullets(), num, sizeof(num), 10);
        posstr += num;
        posstr += " Traded=";
		_itoa_s(pos->GetSharesTraded(), num, sizeof(num), 10);
        posstr += num;
/* and so on
    const Money& GetMoneyInvested() const{return m_moneyInvested;}
    const Money& GetMoneyPendingLong() const{return m_moneyPendingLong;}
    const Money& GetMoneyPendingShort() const{return m_moneyPendingShort;}
    const Money& GetMoneyTradedLong() const{return m_moneyTradedLong;}
    const Money& GetMoneyTradedShort() const{return m_moneyTradedShort;}
    const Money& GetAveragePrice() const{return m_averagePrice;}
    const Money& GetClosedPnl() const{return m_closedPnl;}
    const Money& GetOpenPnl() const{return m_openPnl;}
    const MoneySize& GetInsideBid() const{return m_insideBid;}
    const MoneySize& GetInsideAsk() const{return m_insideAsk;}
    const MoneySize& GetLastTrade() const{return m_lastTrade;}
    Money GetMoneyInUse(const Money& moneyPendingLong, int sharesPendingLong, const Money& moneyPendingShort, int sharesPendingShort) const;
    const Money& GetMoneyInUse() const{return m_moneyInUse;}
    Money GetProjectedMoneyInUseIncrease(bool side, const Money& price, int size) const;
    int GetPhantomSize() const{return m_phantomSize;}
    void SetPhantomSize(int size);
    void IncrementPhantomSize(int size){SetPhantomSize(m_phantomSize + size);}
    bool isOvernight() const{return m_overnight;}
*/
    }
    B_DestroyIterator(iterator);
    AfxGetMainWnd()->MessageBox(posstr, "Positions", MB_OK);
}

void ExtFrame::OnOrders() 
{
	// TODO: Add your command handler code here
    Observable* account = B_GetCurrentAccount();
    if(!account)
    {
        return;
    }
    CString str;
    char num[33];
    void* iterator = B_CreateOrderIterator(OS_CANCELLED|OS_FILLED|OS_PENDINGLONG|OS_PENDINGSHORT,(1 << ST_LAST) - 1, account);
    B_StartIteration(iterator);
    const Order* order;
    while(order = B_GetNextOrder(iterator))
    {
        str += "\n";
        str += order->GetSymbol();
        str += " Id=";
		_ultoa_s(order->GetId(), num, sizeof(num), 10);
        str += num;
        str += " Side=";
        str += order->GetSide();
        str += " P&&L=";
        FormatMoney(str, order->GetPnl());
        str += " Market=";
        str += order->GetDestination();
        str += " Contra=";
        str += order->GetCounterparty();
        str += " Price=";
        FormatMoney(str, *order);
        str += " Size=";
		_ultoa_s(order->GetSize(), num, sizeof(num), 10);
        str += num;
        str += " ExecPrice=";
        FormatMoney(str, order->GetExecutedPrice());
        str += " ExecSize=";
		_ultoa_s(order->GetExecutedSize(), num, sizeof(num), 10);
        str += num;
        str += " Pending=";
		_ultoa_s(order->GetRemainingSize(), num, sizeof(num), 10);
        str += num;
        str += " Confirmed=";
        str += order->isConfirmed() ? "Yes" : "No";
        str += " BeingCanceled=";
        str += order->isBeingCanceled() ? "Yes" : "No";
        str += " MarketOrder=";
        str += order->isMarketOrder() ? "Yes" : "No";
/*
    time_t GetTimeCanceled() const{return m_timeCanceled;}
    unsigned int GetTimeInForce() const{return m_tif;}
    virtual void Cancel() = 0;
*/        
    }
    B_DestroyIterator(iterator);
    AfxGetMainWnd()->MessageBox(str, "Orders", MB_OK);
}

void ExtFrame::OnQuotes() 
{
	// TODO: Add your command handler code here
    const StockBase* stockHandle = B_GetStockHandle("INTC");
    CString str;
    if(B_IsStockValid(stockHandle))
    {
        const BookEntry* be;
        const unsigned int entries = 6;
        unsigned int count;
        char num[33];

        
        Observable* level2 = B_GetLevel2(stockHandle);
        void* m_bidIterator = B_CreateBookIterator(level2, true);

        str += "BIDS\n";
        count = 0;
        B_StartIteration(m_bidIterator);
        while((be = B_GetNextBookEntry(m_bidIterator)) && count < entries)
        {
            count++;
            str += be->GetMmid();
            str += "  ";
            FormatMoney(str, *be);
            str += "    ";
			_itoa_s(be->GetSize(), num, sizeof(num), 10);
            str += num;
            str += "\n";
        }
        B_DestroyIterator(m_bidIterator);

        str += "\nASKS\n";
        count = 0;
        void* m_askIterator = B_CreateBookIterator(level2, false);
        B_StartIteration(m_askIterator);
        while((be = B_GetNextBookEntry(m_askIterator)) && count < entries)
        {
            count++;
            str += be->GetMmid();
            str += "  ";
            FormatMoney(str, *be);
            str += "    ";
			_itoa_s(be->GetSize(), num, sizeof(num), 10);
            str += num;
            str += "\n";
        }
        B_DestroyIterator(m_askIterator);

    }
	else
    {
        str += "Stock is not loaded yet. Try again";
    }
	
    AfxGetMainWnd()->MessageBox(str, "INTC Level 2", MB_OK);
}

void ExtFrame::OnBookQuotes() 
{
	// TODO: Add your command handler code here
    const StockBase* stockHandle = B_GetStockHandle("INTC");
    CString str;
    if(B_IsStockValid(stockHandle))
    {
        const BookEntry* be;
        const unsigned int entries = 6;
        unsigned int count;
        char num[33];

        
//        Observable* book = B_GetExpandedBook(stockHandle);
        unsigned int filter = 0xFFFFFFFF & ~(1 << NYSE_BOOK);
		unsigned int mmLines = 0xFFFFFFFF;
        void* bidIterator = B_CreateMultiBookIterator(stockHandle, true, false, filter, mmLines, false, NULL);//m_aggregatedView, this);

        str += "BIDS\n";
        count = 0;
        B_StartIteration(bidIterator);
        while((be = B_GetNextBookEntry(bidIterator)) && count < entries)
        {
            count++;
            str += be->GetMmid();
            str += "  ";
//            FormatDollarsAndCents(str, be->GetPriceWhole(), be->GetPriceThousandsFraction());
            FormatMoney(str, *be);
            str += "  ";
			_itoa_s(be->GetSize(), num, sizeof(num), 10);
            str += num;
            str += "\n";
        }
        B_DestroyIterator(bidIterator);

        str += "\nASKS\n";
        count = 0;
//        unsigned int m_askIterator = B_CreateBookIterator(book, false);
        void* askIterator = B_CreateMultiBookIterator(stockHandle, false, false, filter, mmLines, false, NULL);//m_aggregatedView, this);
        B_StartIteration(askIterator);
        while((be = B_GetNextBookEntry(askIterator)) && count < entries)
        {
            count++;
            str += be->GetMmid();
            str += "  ";
//            FormatDollarsAndCents(str, be->GetPriceWhole(), be->GetPriceThousandsFraction());
            FormatMoney(str, *be);
            str += "  ";
			_itoa_s(be->GetSize(), num, sizeof(num), 10);
            str += num;
            str += "\n";
        }
        B_DestroyIterator(askIterator);

    }
	else
    {
        str += "Stock is not loaded yet. Try again";
    }
	
    AfxGetMainWnd()->MessageBox(str, "INTC Expanded Book", MB_OK);
}

void ExtFrame::OnAgrBookQuotes() 
{
    const StockBase* stockHandle = B_GetStockHandle("INTC");
    CString str;
    if(B_IsStockValid(stockHandle))
    {
        const BookEntry* be;
        const unsigned int entries = 6;
        unsigned int count;
        char num[33];

        
        unsigned int filter = 0xFFFFFFFF & ~(1 << NYSE_BOOK);
		unsigned int mmLines = 0xFFFFFFFF;
        void* bidIterator = B_CreateMultiBookIterator(stockHandle, true, false, filter, mmLines, true, NULL);//m_aggregatedView, this);

        str += "BIDS\n";
        count = 0;
        B_StartIteration(bidIterator);
        while((be = B_GetNextBookEntry(bidIterator)) && count < entries)
        {
            count++;
            str += be->GetMmid();
            str += "  ";
            FormatMoney(str, *be);
            str += "  ";
			_itoa_s(be->GetSize(), num, sizeof(num), 10);
            str += num;
            str += "\n";
        }
        B_DestroyIterator(bidIterator);

        str += "\nASKS\n";
        count = 0;
        void* askIterator = B_CreateMultiBookIterator(stockHandle, false, false, filter, mmLines, true, NULL);//m_aggregatedView, this);
        B_StartIteration(askIterator);
        while((be = B_GetNextBookEntry(askIterator)) && count < entries)
        {
            count++;
            str += be->GetMmid();
            str += "  ";
            FormatMoney(str, *be);
            str += "  ";
			_itoa_s(be->GetSize(), num, sizeof(num), 10);
            str += num;
            str += "\n";
        }
        B_DestroyIterator(askIterator);

    }
	else
    {
        str += "Stock is not loaded yet. Try again";
    }
    AfxGetMainWnd()->MessageBox(str, "INTC Expanded Book", MB_OK);
}

void ExtFrame::OnLevel1()
{
    const StockBase* stockHandle = B_GetStockHandle("AMD");
    CString str;
    if(B_IsStockValid(stockHandle))
    {
        char num[33];
        Observable* level1 = B_GetLevel1(stockHandle);
        str += "BidTick ";
        switch(B_GetBidTickStatus(level1))
        {
            case NOTICK:
            str += "NO";
            break;

            case UPTICK:
            str += "UP";
            break;

            case DOWNTICK:
            str += "DOWN";
            break;
        }

        str += "\nBid ";
        FormatMoneySize(str, B_GetBid(level1));

        str += "\nAsk ";
        FormatMoneySize(str, B_GetAsk(level1));
        
        str += "\nLastTrade ";
        FormatMoneySize(str, B_GetLastTrade(level1));
        
        str += "\nLastTradeTime ";
        CTime t(B_GetLastTradeTime(level1));
        str += t.Format("%H:%M:%S");
        
        str += "\nVolume ";
		_i64toa_s(B_GetVolume(level1), num, sizeof(num), 10);
        str += num;
        
        str += "\nOpen Price ";
        FormatMoney(str, B_GetOpenPrice(level1));

        str += "\nClose Price ";
        FormatMoney(str, B_GetClosePrice(level1));

        str += "\nHi Price ";
        FormatMoney(str, B_GetIntraDayHigh(level1));

        str += "\nLo Price ";
        FormatMoney(str, B_GetIntraDayLow(level1));

        str += "\nNet Change ";
        FormatMoney(str, B_GetNetChange(level1));
    }	
	else
    {
        str += "Stock is not loaded yet. Try again";
    }
    AfxGetMainWnd()->MessageBox(str, "INTC Level 1", MB_OK);
}

void ExtFrame::OnDestroy()
{
    ClearDialogs();
	CFrameWnd::OnDestroy();
    instance = NULL;	
	// TODO: Add your message handler code here
	
}

void ExtFrame::ShowWindowAndChildren(int show)
{
    ShowWindow(show);
    DlgSet::iterator it;
    DlgSet::iterator itend = m_dlgSet.end();
    for(it = m_dlgSet.begin(); it != itend; ++it)
    {
        (*it)->ShowWindow(show);
    }
}

void ExtFrame::OnSysCommand(UINT nID, LPARAM lParam)
{
    switch(nID)
    {
		case SC_CLOSE:
        ShowWindowAndChildren(SW_HIDE);
        break;

        default:
        CFrameWnd::OnSysCommand(nID, lParam);
        break;
	}
}

unsigned int ExtFrame::SetDollarsAndCents(CSpinButtonCtrl& spinDollars, CEdit& editDollars,
    CSpinButtonCtrl& spinCents, CEdit& editCents,
    unsigned int val, unsigned int maxDollars)
{
    spinDollars.SetRange(0, maxDollars);
    spinCents.SetRange(0, 99);
    char num[33];
    _ultoa_s(maxDollars, num, sizeof(num), 10);

    editDollars.SetLimitText((unsigned int)strlen(num));
    editCents.SetLimitText(2);

    unsigned int dollars = val / 1000;
    SetSpinValue(spinDollars, editDollars, dollars, false);
    unsigned int tenthcents = val - 1000 * dollars;
    unsigned int cents = tenthcents / 10;
    SetSpinValue(spinCents, editCents, cents, true);
    return tenthcents - 10 * cents;
}

void ExtFrame::SetSpinValue(CSpinButtonCtrl& spin, CEdit& edit, int val, bool two)
{
    int rangeMin;
    int rangeMax;
    spin.GetRange32(rangeMin, rangeMax);
    if(val < rangeMin)
    {
        val = rangeMin;
    }
    else if(val > rangeMax)
    {
        val = rangeMax;
    }
    char num[33];
    if(two && val < 10)
    {
        *num = '0';
        _itoa_s(val, num + 1, sizeof(num) - 1, 10);
    }
    else
    {
        _itoa_s(val, num, sizeof(num), 10);
    }
    spin.SetPos(val);

    edit.SetWindowText(num);
}

void ExtFrame::SpinChanged(CSpinButtonCtrl& spin, CEdit& edit)
{
    CString str;
    edit.GetWindowText(str);
    if(str.GetLength() == 0)
    {
        spin.SetPos(0);
    }
    else
    {
        spin.SetPos(atoi(str));
    }
}

void ExtFrame::IncrementSpin(CSpinButtonCtrl& spin, CEdit& edit, unsigned int val)
{
    char num[33];
    _ultoa_s(val, num, sizeof(num), 10);
    spin.SetPos(val);
    edit.SetWindowText(num);
    ScrollEnd(edit);
}   

void ExtFrame::ScrollEnd(CEdit& edit)
{
    int length = edit.GetWindowTextLength();
    edit.PostMessage(EM_SETSEL, length, length);
}


void ExtFrame::AdjustEdit(CSpinButtonCtrl& spin, CEdit& edit, bool two)
{
    int rangeMin;
    int rangeMax;
    spin.GetRange32(rangeMin, rangeMax);
    if(rangeMin <= rangeMax)
    {
        int length = edit.GetWindowTextLength();
        char* temp = new char[length + 1];
        edit.GetWindowText(temp, length + 1);
        int value;
        bool changed = false;
        if(length == 0)
        {
            value = rangeMin;
            changed = true;
        }
        else
        {
            value = atoi(temp);
            if(value < rangeMin)
            {
                value = rangeMin;
                changed = true;
            }
            else if(value > rangeMax)
            {
                value = rangeMax;
                changed = true;
            }
        }
        char numValue[33];
        _itoa_s(value, numValue, 10);
        length = (int)strlen(numValue);

        if(length == 0)
        {
            edit.SetWindowText(two ? "00" : "0");
        }
        else if(two)
        {
            if(length == 1)
            {
                char num[33];
                *num = '0';
                strcpy_s(num + 1, sizeof(num) - 1, numValue);
                edit.SetWindowText(num);
            }
            else if(changed)
            {
                edit.SetWindowText(numValue);
            }
        }
        else
        {
            if(length == 2)
            {
                if(*numValue == '0')
                {
                    edit.SetWindowText(numValue + 1);
                }
                else if(changed)
                {
                    edit.SetWindowText(numValue);
                }
            }
            else if(changed)
            {
                edit.SetWindowText(numValue);
            }
        }
        delete[] temp;
    }
}




void ExtFrame::OnLoadStocks() 
{
	// TODO: Add your command handler code here
    CreateDlg(IDD_LOAD);
	
}

void ExtFrame::MarketSummaryPopulationDone(bool done)
{
    m_marketSummaryPopulationDone = done;

    DlgSet::iterator it;
    DlgSet::iterator itend = m_dlgSet.end();
    for(it = m_dlgSet.begin(); it != itend; ++it)
    {
        (*it)->MarketSummaryPopulationDone(done);
    }
}

void ExtFrame::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
	TLKillDead(600);
	
    switch(message->GetType())
    {
        case MS_RESP_SYMBOL_SORTABLE_POPULATION_DONE:
        MarketSummaryPopulationDone(true);
        break;

        case MSGID_CONNECTION_LOST:
        if(from == B_GetMarketSummaryReceiver())
        {
            MarketSummaryPopulationDone(false);
        }
        break;
    }

}

void ExtFrame::OnSubscription()
{
    CreateDlg(IDD_MARKETMAKER);
}

void ExtFrame::OnSendOrder() 
{
    CreateDlg(IDD_SENDORDER);
}

void ExtFrame::OnCancelOrder() 
{
    CreateDlg(IDD_CANCELORDER);
}

void ExtFrame::OnIndex() 
{
    CreateDlg(IDD_INDEX);
}

void ExtFrame::OnSendStopOrder() 
{
    CreateDlg(IDD_SENDSTOPORDER);
}

void ExtFrame::OnPositionsOrdersExecutions() 
{
    CreateDlg(IDD_POS_ORDERS_EXECS);
}

void ExtFrame::OnSingleBook() 
{
    CreateDlg(IDD_SINGLE_BOOK);
}

void ExtFrame::OnMultiBook() 
{
    CreateDlg(IDD_MULTI_BOOK);
}



BOOL ExtFrame::OnCopyData(CWnd* sWnd, COPYDATASTRUCT* CD) 
{
	TRACE0("got COPYDATA");
	CString gotMsg = (LPCTSTR)(CD->lpData);
	int gotType = (int)(CD->dwData);
	return ServiceMsg(gotType,gotMsg);

}

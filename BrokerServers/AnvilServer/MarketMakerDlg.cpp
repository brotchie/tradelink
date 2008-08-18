// MarketMakerDlg.cpp : implementation file
//

#include "stdafx.h"
#include "Resource.h"
#include "MarketMakerDlg.h"
#include "ExtFrame.h"

//#include "ReceiverApi.h"
#include "BusinessApi.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// MarketMakerDlg dialog


MarketMakerDlg::MarketMakerDlg(CWnd* pParent):
    BaseDlg(MarketMakerDlg::IDD, pParent),
    m_stockHandle(0),
    m_level1(NULL),
    m_level2(NULL),
    m_print(NULL),
    m_book(NULL),
    m_bidIterator(NULL),
    m_askIterator(NULL),
    m_printsIterator(NULL)
{
	//{{AFX_DATA_INIT(MarketMakerDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT

    Clear();

    m_bids.push_back(&m_bid1);
    m_bids.push_back(&m_bid2);
    m_bids.push_back(&m_bid3);
    m_bids.push_back(&m_bid4);
    m_bids.push_back(&m_bid5);
    m_bids.push_back(&m_bid6);

    m_asks.push_back(&m_ask1);
    m_asks.push_back(&m_ask2);
    m_asks.push_back(&m_ask3);
    m_asks.push_back(&m_ask4);
    m_asks.push_back(&m_ask5);
    m_asks.push_back(&m_ask6);

    m_prints.push_back(&m_print1);
    m_prints.push_back(&m_print2);
    m_prints.push_back(&m_print3);
    m_prints.push_back(&m_print4);
    m_prints.push_back(&m_print5);
    m_prints.push_back(&m_print6);

    for(unsigned int i = 0; i < MAX_BOOKS; i++)
    {
        m_linesIntegrated[i] = 1;//number of book lines integrated in Level2
    }
}

MarketMakerDlg::~MarketMakerDlg()
{
}

void MarketMakerDlg::Clear()
{
    if(m_bidIterator)
    {
        B_DestroyIterator(m_bidIterator);
        m_bidIterator = NULL;
    }
    if(m_askIterator)
    {
        B_DestroyIterator(m_askIterator);
        m_askIterator = NULL;
    }
    if(m_printsIterator)
    {
        B_DestroyIterator(m_printsIterator);
        m_printsIterator = NULL;
    }

    if(m_level1)
    {
        m_level1->Remove(this);
        m_level1 = NULL;
    }
    if(m_level2)
    {
        m_level2->Remove(this);
        m_level2 = NULL;
    }
    if(m_book)
    {
        m_book->Remove(this);
        m_book = NULL;
    }
    if(m_print)
    {
        m_print->Remove(this);
        m_print = NULL;
    }
    
    m_stockHandle = 0;
}

void MarketMakerDlg::ClearControls()
{
    QuoteControls::iterator it;
    QuoteControls::iterator itend = m_bids.end();
    for(it = m_bids.begin(); it != itend; it++)
    {
        (*it)->SetWindowText("");
    }
    itend = m_asks.end();
    for(it = m_asks.begin(); it != itend; it++)
    {
        (*it)->SetWindowText("");
    }
    itend = m_prints.end();
    for(it = m_prints.begin(); it != itend; it++)
    {
        (*it)->SetWindowText("");
    }

	m_l1Volume.SetWindowText("");
	m_l1Time.SetWindowText("");
	m_l1Size.SetWindowText("");
	m_l1Open.SetWindowText("");
	m_l1Net.SetWindowText("");
	m_l1Lo.SetWindowText("");
	m_l1Last.SetWindowText("");
	m_l1Hi.SetWindowText("");
	m_l1Close.SetWindowText("");
	m_l1BXA.SetWindowText("");
    m_l1BidTick.SetWindowText("");


    if(m_hWnd)
    {
        SetWindowText("");
    }
}

void MarketMakerDlg::DoDataExchange(CDataExchange* pDX)
{
	BaseDlg::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(MarketMakerDlg)
	DDX_Control(pDX, IDS_BIDTICK_VALUE, m_l1BidTick);
	DDX_Control(pDX, IDS_PRINT_6, m_print6);
	DDX_Control(pDX, IDS_PRINT_5, m_print5);
	DDX_Control(pDX, IDS_PRINT_4, m_print4);
	DDX_Control(pDX, IDS_PRINT_3, m_print3);
	DDX_Control(pDX, IDS_PRINT_2, m_print2);
	DDX_Control(pDX, IDS_PRINT_1, m_print1);
	DDX_Control(pDX, IDS_VOLUME_VALUE, m_l1Volume);
	DDX_Control(pDX, IDS_TIME_VALUE, m_l1Time);
	DDX_Control(pDX, IDS_SIZE_VALUE, m_l1Size);
	DDX_Control(pDX, IDS_OPEN_VALUE, m_l1Open);
	DDX_Control(pDX, IDS_NET_VALUE, m_l1Net);
	DDX_Control(pDX, IDS_LO_VALUE, m_l1Lo);
	DDX_Control(pDX, IDS_LAST_VALUE, m_l1Last);
	DDX_Control(pDX, IDS_HI_VALUE, m_l1Hi);
	DDX_Control(pDX, IDS_CLOSE_VALUE, m_l1Close);
	DDX_Control(pDX, IDS_BXA_VALUE, m_l1BXA);
	DDX_Control(pDX, IDE_STOCK, m_EditStock);
	DDX_Control(pDX, IDS_BID_6, m_bid6);
	DDX_Control(pDX, IDS_BID_5, m_bid5);
	DDX_Control(pDX, IDS_BID_4, m_bid4);
	DDX_Control(pDX, IDS_BID_3, m_bid3);
	DDX_Control(pDX, IDS_BID_2, m_bid2);
	DDX_Control(pDX, IDS_BID_1, m_bid1);
	DDX_Control(pDX, IDS_ASK_6, m_ask6);
	DDX_Control(pDX, IDS_ASK_5, m_ask5);
	DDX_Control(pDX, IDS_ASK_4, m_ask4);
	DDX_Control(pDX, IDS_ASK_3, m_ask3);
	DDX_Control(pDX, IDS_ASK_2, m_ask2);
	DDX_Control(pDX, IDS_ASK_1, m_ask1);
	//}}AFX_DATA_MAP
}


void MarketMakerDlg::DoCancel()
{
    Clear();
}

BEGIN_MESSAGE_MAP(MarketMakerDlg, BaseDlg)
	//{{AFX_MSG_MAP(MarketMakerDlg)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()



/////////////////////////////////////////////////////////////////////////////
// MarketMakerDlg message handlers
/*
void MarketMakerDlg::OnCancel()
{
    Clear();
    ExtFrame::GetInstance()->DestroyMMDlg(this);
}
*/
void MarketMakerDlg::OnOK()
{
    CString symbol;
    m_EditStock.GetWindowText(symbol);
    m_EditStock.SetWindowText("");
    Clear();
    ClearControls();
    m_stockHandle = B_GetStockHandle(symbol);
    if(m_stockHandle)
    {
        m_level1 = B_GetLevel1(m_stockHandle);
        m_level2 = B_GetLevel2(m_stockHandle);
//        m_book = B_GetAggregatedBook(m_stockHandle);
        m_print = B_GetPrints(m_stockHandle);

        m_level1->Add(this);
//        m_level2->Add(this);
//        m_book->Add(this);
        m_print->Add(this);
        bool ecnsOnly = false;
        bool twoDecPlaces = false;
		unsigned int mmLines = 1;//0xFFFFFFFF;

        m_bidIterator = B_CreateLevel2AndBookIterator(m_stockHandle, true, ecnsOnly, twoDecPlaces, m_linesIntegrated, mmLines, this);
        m_askIterator = B_CreateLevel2AndBookIterator(m_stockHandle, false, ecnsOnly, twoDecPlaces, m_linesIntegrated, mmLines, this);

//        m_bidIterator = B_CreateLevel2AndBookIterator(m_level2, m_book, true, false, false, m_linesIntegrated, MAX_BOOKS);
//        m_askIterator = B_CreateLevel2AndBookIterator(m_level2, m_book, false, false, false, m_linesIntegrated, MAX_BOOKS);
        m_printsIterator = B_CreatePrintsIterator(m_stockHandle, true);
		B_TransactionIteratorSetSourceFilter(m_printsIterator, ((1 << PS_LAST) - 1), NULL);

        SetWindowText(symbol);

        if(B_IsStockValid(m_stockHandle))
        {
            FillLevel1();
            FillPrints();
            FillQuotes(true);
            FillQuotes(false);
        }
    }
}

void MarketMakerDlg::FillPrints()
{
    const Transaction* t;
    unsigned int entries = (unsigned int)m_prints.size();
    unsigned int count = 0;
    char num[33];
    CString str;        
    B_StartIteration(m_printsIterator);
    while((t = B_GetNextPrintsEntry(m_printsIterator)) && count < entries)
    {
        str = "";
//        ExtFrame::FormatDollarsAndCents(str, t->GetPriceWhole(), t->GetPriceThousandsFraction());
        ExtFrame::FormatMoney(str, *t);
        str += "      ";
		_ultoa_s(t->GetSize(), num, sizeof(num), 10);
        str += num;
        str += "      ";
        CTime t(t->GetTime());
        str += t.Format("%H:%M:%S");
/*
        switch(t->GetStatus())
        {
            case TRADE_GREATERTHANASK:
            case TRADE_LESSTHANBID:
            break;

            default:
            break;
        }
*/
        m_prints[count]->SetWindowText(str);
        count++;
    }
    for(; count < entries; count++)
    {
        m_prints[count]->SetWindowText("");
    }
}

void MarketMakerDlg::FillLevel1()
{
    CString str;
    char num[33];
    switch(B_GetBidTickStatus(m_level1))
    {
        case NOTICK:
        str = "NO";
        break;

        case UPTICK:
        str = "UP";
        break;

        case DOWNTICK:
        str = "DOWN";
        break;
    }
	m_l1BidTick.SetWindowText(str);

	_i64toa_s(B_GetVolume(m_level1), num, sizeof(num), 10);
	m_l1Volume.SetWindowText(num);
    
    CTime t(B_GetLastTradeTime(m_level1));
    m_l1Time.SetWindowText(t.Format("%H:%M:%S"));
	
	_ultoa_s(B_GetLastTrade(m_level1).GetSize(), num, sizeof(num), 10);
    m_l1Size.SetWindowText(num);

    str = "";
    ExtFrame::FormatMoney(str, B_GetOpenPrice(m_level1));
	m_l1Open.SetWindowText(str);

    str = "";
    ExtFrame::FormatMoney(str, B_GetNetChange(m_level1));
	m_l1Net.SetWindowText(str);

    str = "";
    ExtFrame::FormatMoney(str, B_GetIntraDayLow(m_level1));
	m_l1Lo.SetWindowText(str);

    str = "";
    ExtFrame::FormatMoney(str, B_GetLastTrade(m_level1));
	m_l1Last.SetWindowText(str);

    str = "";
    ExtFrame::FormatMoney(str, B_GetIntraDayHigh(m_level1));
	m_l1Hi.SetWindowText(str);

    str = "";
    ExtFrame::FormatMoney(str, B_GetClosePrice(m_level1));
	m_l1Close.SetWindowText(str);

	_ultoa_s(B_GetBid(m_level1).GetSize(), num, sizeof(num), 10);
    str = num;
    str += "x";
	_ultoa_s(B_GetAsk(m_level1).GetSize(), num, sizeof(num), 10);
    str += num;
    m_l1BXA.SetWindowText(str);
}

void MarketMakerDlg::FillQuotes(bool bid)
{
    if(bid)
    {
        Fill(m_bids, m_bidIterator);
    }
    else
    {
        Fill(m_asks, m_askIterator);
    }
}

void MarketMakerDlg::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
    switch(message->GetType())
    {
        case M_RESP_REFRESH_SYMBOL:
        if(from == m_level1)
        {
            FillLevel1();
        }
        else if(from == m_level2 || from == m_book)
        {
            FillQuotes(true);
            FillQuotes(false);
        }
        else if(from == m_print)
        {
            FillPrints();
        }

        break;

        case M_RESP_REFRESH_SYMBOL_FAILED:
//        Clear();
        m_level1 = NULL;
        m_level2 = NULL;
        m_print = NULL;
        m_book = NULL;
        Clear();
        ClearControls();
        break;

    
        case M_NW2_MM_QUOTE:
        case M_LEVEL2_QUOTE:
        if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_LEVEL2_QUOTE)
        {
            AIMsgMMLevel2Quote* info = (AIMsgMMLevel2Quote*)additionalInfo;
            if(!info->isDirectEcn || m_linesIntegrated[info->bookId] == 0)
            {
                if(info->bidChanged)
                {
                    FillQuotes(true);
                }
                if(info->askChanged)
                {
                    FillQuotes(false);
                }
            }
        }
        else //should not happen
        {
            FillQuotes(true);
            FillQuotes(false);
        }
        break;
/*
        case M_ITCH_MODIFY_ORDER:
        FillQuotes(((MsgItchModifyOrder*)message)->x_BuySellIndicator == 'B');
        break;

		case M_ITCH_ADD_ORDER:
        FillQuotes(((MsgItchAddOrder*)message)->x_BuySellIndicator == 'B');
        break;
*/
		case M_ITCH_1_00_NewVisibleOrder:
        FillQuotes(((MsgItch100AddOrder*)message)->m_BuySellIndicator == 'B');
        break;

		case M_ITCH_1_00_VisibleOrderExecution:
        FillQuotes(((MsgItch100VisibleOrderExecution*)message)->m_BuySellIndicator == 'B');
        break;

		case M_ITCH_1_00_CanceledOrder:
        FillQuotes(((MsgItch100CancelOrder*)message)->m_BuySellIndicator == 'B');
        break;

		case M_BOOK_NEW_ORDER:
        FillQuotes(((MsgBookNewOrder*)message)->m_Side == SIDE_BUY);
        break;

		case M_BOOK_MODIFY_ORDER:
        FillQuotes(((MsgBookModifyOrder*)message)->m_Side == SIDE_BUY);
        break;

		case M_BOOK_DELETE_ORDER:
        FillQuotes(((MsgBookDeleteOrder*)message)->m_Side == SIDE_BUY);
        break;

		case M_ITCH_1_00_NewVisibleAttributedOrder:
		{
			const MsgItch100AddAttributedOrder* msg = (const MsgItch100AddAttributedOrder*)message;
			unsigned short ecnId = B_GetEcnIdByNameId(msg->m_mmid);
			if(ecnId < MAX_BOOKS)//Ecn Book
			{
				if(B_IsEcnBook(m_stockHandle, from, true))
				{
					if(additionalInfo && additionalInfo->GetType() == M_AI_BOOK_QUOTE)
					{
						AIMsgBookQuote* info = (AIMsgBookQuote*)additionalInfo;
				        FillQuotes(msg->m_BuySellIndicator == 'B');
					}
				}
			}
			else// if(from != m_position)//MarketMaker book// if(m_mmLines != 0)
			{
		        FillQuotes(msg->m_BuySellIndicator == 'B');
			}
		}

        break;

		case M_ITCH_1_00_VisibleAttributedOrderExecution:
		{
			const MsgItch100VisibleAttributedOrderExecution* msg = (const MsgItch100VisibleAttributedOrderExecution*)message;
			unsigned short ecnId = B_GetEcnIdByNameId(msg->m_mmid);
			if(ecnId < MAX_BOOKS)
			{
				if(B_IsEcnBook(m_stockHandle, from, true))
				{
					if(additionalInfo && additionalInfo->GetType() == M_AI_BOOK_QUOTE)
					{
						AIMsgBookQuote* info = (AIMsgBookQuote*)additionalInfo;
				        FillQuotes(msg->m_BuySellIndicator == 'B');
					}
				}
		        else// if(from != m_position)
				{
//					AddPrint();
				}
			}
			else// if(from != m_position)// if(m_mmLines != 0)
			{
		        FillQuotes(msg->m_BuySellIndicator == 'B');
			}
		}
        break;

		case M_ITCH_1_00_ATTRIBUTED_CanceledOrder:
		{
			const MsgItch100AttributedCancelOrder* msg = (const MsgItch100AttributedCancelOrder*)message;
			unsigned short ecnId = B_GetEcnIdByNameId(msg->m_mmid);
			if(ecnId < MAX_BOOKS)
			{
				if(B_IsEcnBook(m_stockHandle, from, true))
				{
					if(additionalInfo && additionalInfo->GetType() == M_AI_BOOK_QUOTE)
					{
						AIMsgBookQuote* info = (AIMsgBookQuote*)additionalInfo;
				        FillQuotes(msg->m_BuySellIndicator == 'B');
					}
				}
			}
			else// if(from != m_position)// if(m_mmLines != 0)
			{
		        FillQuotes(msg->m_BuySellIndicator == 'B');
			}
		}
        break;
/*
		case M_AI_NEW_MM_BOOK:
		if(m_bidIterator)
		{
			unsigned int mmid = ((const MsgNewMmBook*)message)->m_mmid;
			B_MultiBookIteratorMmBookAdded(m_bidIterator, mmid);
			B_MultiBookIteratorMmBookAdded(m_askIterator, mmid);
		}
		break;
*/
		case M_BOOK_NYSE_OPEN_BOOK:
        {
            MsgNyseOpenBook* msg = (MsgNyseOpenBook*)message;
            if(msg->m_uNumberOfBuys > 0)
            {
                FillQuotes(true);
            }
            if(msg->m_uNumberOfSells > 0)
            {
                FillQuotes(false);
            }
        }
        break;

		case M_FLUSH_ALL:
        case M_FLUSH_ALL_OPEN_BOOKS:
        case M_FLUSH_BOOK_FOR_STOCK:
        FillQuotes(true);
        FillQuotes(false);
        break;


        case M_NW2_INSIDE_QUOTE:
        if(from == m_level1)
        {
            FillLevel1();
        }
        break;

//        case M_NW2_LAST_TRADE:
        case M_LAST_TRADE_SHORT:
        case M_TAL_LAST_TRADE:
        if(from == m_print)
        {
            FillPrints();
        }
        else if(from == m_level1)
        {
            FillLevel1();
        }
        break;
    }
}

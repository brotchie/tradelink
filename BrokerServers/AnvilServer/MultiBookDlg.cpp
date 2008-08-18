// MultiBookDlg.cpp : implementation file
//

#include "stdafx.h"
#include "Resource.h"
#include "MultiBookDlg.h"
#include "BusinessApi.h"
#include "Messages.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// MultiBookDlg dialog


MultiBookDlg::MultiBookDlg(CWnd* pParent):
    BaseDlg(MultiBookDlg::IDD, pParent),
    m_stockHandle(NULL),
    m_level1(NULL),
    m_bidIterator(NULL),
    m_askIterator(NULL)
{
	//{{AFX_DATA_INIT(MultiBookDlg)
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
}

void MultiBookDlg::Clear()
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
    if(m_level1)
    {
        m_level1->Remove(this);
        m_level1 = NULL;
    }
    m_stockHandle = NULL;
}

void MultiBookDlg::ClearControls()
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

    if(m_hWnd)
    {
        SetWindowText("");
    }
}


void MultiBookDlg::DoDataExchange(CDataExchange* pDX)
{
	BaseDlg::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(MultiBookDlg)
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
	DDX_Control(pDX, IDE_STOCK, m_EditStock);
	DDX_Control(pDX, IDB_SINGLEBOOK_DYNAMIC, m_CheckBoxDynamic);
	DDX_Control(pDX, IDB_SINGLEBOOK_EXPANDED, m_RadioExpanded);
	DDX_Control(pDX, IDB_SINGLEBOOK_AGGREGATED, m_RadioAggregated);
	DDX_Control(pDX, IDB_MULTIBOOK_NYSE, m_CheckBoxNyse);
	DDX_Control(pDX, IDB_MULTIBOOK_ISLD, m_CheckBoxIsld);
	DDX_Control(pDX, IDB_MULTIBOOK_BATS, m_CheckBoxBats);
	DDX_Control(pDX, IDB_MULTIBOOK_ARCA, m_CheckBoxArca);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(MultiBookDlg, BaseDlg)
	//{{AFX_MSG_MAP(MultiBookDlg)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// MultiBookDlg message handlers
void MultiBookDlg::DoCancel()
{
    Clear();
}

void MultiBookDlg::OnOK()
{
    CString symbol;
    m_EditStock.GetWindowText(symbol);
//    m_EditStock.SetWindowText("");
    Clear();
    ClearControls();
    m_stockHandle = B_GetStockHandle(symbol);
    if(m_stockHandle)
    {
        m_level1 = B_GetLevel1(m_stockHandle);
        unsigned short bookFilter = 0;
        if(m_CheckBoxIsld.GetCheck() == BST_CHECKED)
        {
            bookFilter |= (1 << ISLD_BOOK);
        }
        if(m_CheckBoxBats.GetCheck() == BST_CHECKED)
        {
            bookFilter |= (1 << BATS_BOOK);
        }
        if(m_CheckBoxArca.GetCheck() == BST_CHECKED)
        {
            bookFilter |= (1 << ARCA_BOOK);
        }
        if(m_CheckBoxNyse.GetCheck() == BST_CHECKED)
        {
            bookFilter |= (1 << NYSE_BOOK);
        }
        bool aggregated = m_RadioAggregated.GetCheck() == BST_CHECKED;
        bool dynamic = m_CheckBoxDynamic.GetCheck() == BST_CHECKED;
		unsigned int mmLines = 0xFFFFFFFF;
//unsigned int WINAPI B_CreateMultiBookIterator(unsigned int stockHandle, bool side, bool ecnsOnly, unsigned int ecnFilter, bool aggregated, Observer* observerToAddToBooks);
        m_bidIterator = B_CreateMultiBookIterator(m_stockHandle, true, false, bookFilter, mmLines, aggregated, dynamic ? this : NULL);
        m_askIterator = B_CreateMultiBookIterator(m_stockHandle, false, false, bookFilter, mmLines, aggregated, dynamic ? this : NULL);

        SetWindowText(symbol);

        if(B_IsStockValid(m_stockHandle))
        {
            FillQuotes(true);
            FillQuotes(false);
        }
        else
        {
            m_level1->Add(this);
        }
    }
}

void MultiBookDlg::FillQuotes(bool bid)
{
//Look at function "Fill" in BaseDlg class
    if(bid)
    {
        Fill(m_bids, m_bidIterator);
    }
    else
    {
        Fill(m_asks, m_askIterator);
    }
}

void MultiBookDlg::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
    switch(message->GetType())
    {
        case M_RESP_REFRESH_SYMBOL:
        if(from == m_level1)
        {
            FillQuotes(true);
            FillQuotes(false);
            if(m_CheckBoxDynamic.GetCheck() != BST_CHECKED)
            {
                m_level1->Remove(this);
            }
        }

        break;

        case M_RESP_REFRESH_SYMBOL_FAILED:
        m_level1 = NULL;
        Clear();
        ClearControls();
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
    }
}

BOOL MultiBookDlg::OnInitDialog() 
{
	BaseDlg::OnInitDialog();
//	m_CheckBoxIsld.SetCheck(BST_CHECKED);
	m_RadioAggregated.SetCheck(BST_CHECKED);
	return TRUE;
}

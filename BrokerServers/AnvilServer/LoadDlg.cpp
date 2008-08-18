// LoadDlg.cpp : implementation file
//

#include "stdafx.h"
#include "Resource.h"
#include "LoadDlg.h"
#include "ExtFrame.h"

//#include <ReceiverApi.h>
#include "BusinessApi.h"


class Stock : public Observer//we derive this object from Obsererver to be able to get info about dynamic changes of the stock
                            //every Observer must have function virtual void Process(const Message* message, Observable* from, const Message* additionalInfo); which will be called when a stock update is received from the server.
{
public:
    Stock(const StockMovement* stock, bool load = true):
		m_symbol(stock->GetSymbol()),
        m_stockHandle(NULL),
        m_level1(NULL),
//        m_level2(NULL),
        m_prints(NULL),
//        m_aggregatedBook(NULL),
//        m_expandedBook(NULL),
        m_owningListBox(NULL),
        m_loaded(false),
        m_index(0xFFFFFFFF),
		m_volume(0),
		m_nysVolume(0),
        m_bidIterator(NULL),
        m_askIterator(NULL)
    {
        if(load)
        {
			m_volume = stock->GetVolume();
			m_nysVolume = stock->GetNyseVolume();

			m_stockHandle = B_GetStockHandle(m_symbol.c_str());//This will load the stock from the Hammer server. The stock will not be available immediately for monitoring and trading, only after Hammer server sends all the info about the stock.
            m_level1 = B_GetLevel1(m_stockHandle);
//            m_level2 = B_GetLevel2(m_stockHandle);
            m_prints = B_GetPrints(m_stockHandle);
//            m_aggregatedBook = B_GetAggregatedBook(m_stockHandle);
//            m_expandedBook = B_GetExpandedBook(m_stockHandle);

//Add 'this' as an Observer to a bunch of Observables.
            m_level1->Add(this);
//            m_level2->Add(this);
            m_prints->Add(this);

            bool ecnsOnly = false;
            bool twoDecPlaces = false;

//how many lines for each book you want integrated in the level 2 quotes
//for the purpose of getting the best quote we need one line per book.
            unsigned int bookQuotesIntegrated[MAX_BOOKS];
            for(unsigned int i = 0; i < MAX_BOOKS; i++)
            {
                bookQuotesIntegrated[i] = 1;//number of book lines integrated in Level2
            }
			unsigned int mmLines = 1;//0xFFFFFFFF - all

            m_bidIterator = B_CreateLevel2AndBookIterator(m_stockHandle, true, ecnsOnly, twoDecPlaces, bookQuotesIntegrated, mmLines, this);
            m_askIterator = B_CreateLevel2AndBookIterator(m_stockHandle, false, ecnsOnly, twoDecPlaces, bookQuotesIntegrated, mmLines, this);

//            m_aggregatedBook->Add(this);
//            m_expandedBook->Add(this);

            m_loaded = B_IsStockValid(m_stockHandle);
//When an Observable has some new data it will notify all its Observers by means of calling function virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
        }
    }

    virtual ~Stock()
    {
        if(m_bidIterator)
        {
            B_DestroyIterator(m_bidIterator);
        }
        if(m_askIterator)
        {
            B_DestroyIterator(m_askIterator);
        }
    }

    void SetLoaded(bool loaded){m_loaded = loaded;}
    bool isLoaded() const{return m_loaded;}

    void RemoveFromListBox()
    {
        if(m_owningListBox && m_index != 0xFFFFFFFF)
        {
            unsigned int count = m_owningListBox->GetCount();
            for(unsigned int i = m_index + 1; i < count; ++i)
            {
                ((Stock*)m_owningListBox->GetItemDataPtr(i))->SetIndex(i - 1);
            }
            m_owningListBox->DeleteString(m_index);
			m_owningListBox->IncrementLoadedCount(-1, m_loaded);
            m_owningListBox = NULL;
        }
    }

	unsigned __int64 GetVolume() const{return m_volume;}
	unsigned __int64 GetNysVolume() const{return m_nysVolume;}

    void SetIndex(unsigned int index){m_index = index;}
    unsigned int GetIndex() const{return m_index;}

    void Invalidate()
    {
        if(m_owningListBox && m_index != 0xFFFFFFFF)
        {
            m_owningListBox->InvalidateItem(m_index);
        }
    }

    const std::string& GetSymbol() const{return m_symbol;}

    const MoneySize& GetLastTrade() const
    {
        return B_GetLastTrade((Observable*)m_level1);
    }
    const Money& GetClosePrice() const
    {
        return B_GetClosePrice((Observable*)m_level1);
    }
    char GetBidTick() const
    {
        unsigned int tick = B_GetBidTickStatus((Observable*)m_level1);
        return tick == UPTICK ? 'U' : tick == DOWNTICK ? 'D' : ' ';
    }

    Observable* GetLevel1(){return m_level1;}
//    Observable* GetLevel2(){return m_level2;}
    Observable* GetPrints(){return m_prints;}
//    Observable* GetAggregatedBook(){return m_aggregatedBook;}
//    Observable* GetExpandedBook(){return m_expandedBook;}
    void* GetBidIterator() const{return m_bidIterator;}
    void* GetAskIterator() const{return m_askIterator;}
    void SetOwner(ListBoxStock* lb){m_owningListBox = lb;}
protected:
    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
    std::string m_symbol;
    const StockBase* m_stockHandle;
    Observable* m_level1;
//    Observable* m_level2;
    Observable* m_prints;
//    Observable* m_aggregatedBook;
//    Observable* m_expandedBook;

	unsigned __int64 m_volume;
	unsigned __int64 m_nysVolume;

    void* m_bidIterator;
    void* m_askIterator;


    ListBoxStock* m_owningListBox;//we will be updating the list box items, so we need a pointer to the list box owning the stock

    bool m_loaded;
    unsigned int m_index;
};


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

bool ListBoxStock::AddStock(Stock* stock)
{
    int found = FindItem(stock);
    if(found < 0)
    {
        if(AddString((const char*)stock) >= 0)
        {
            stock->SetOwner(this);
            return true;
        }
    }
    return false;
}

void ListBoxStock::DeleteItem(LPDELETEITEMSTRUCT lpDeleteItemStruct)
{
    delete (Stock*)lpDeleteItemStruct->itemData;
}

int ListBoxStock::SearchCompare(const void* item1, const void* item2) const
{
    const Stock* stock1 = (const Stock*)item1;
    const Stock* stock2 = (const Stock*)item2;
    int result = strcmp(stock1->GetSymbol().c_str(), stock2->GetSymbol().c_str());
    return result < 0 ? -1 : result > 0 ? 1 : 0;
}

void FormatUnsignedInteger64WithCommas(unsigned __int64 n, std::string& str)
{
    char num[64];
    _ui64toa_s(n, num, sizeof(num), 10);
    unsigned int len = (unsigned int)strlen(num);
    unsigned int i3 = len / 3 + 1;
    unsigned int i = i3 * 3 - len;
    if(i == 3)
    {
        i = 0;
    }
    char* buf = new char[len + i3 + 1];
    char* cursorOut = buf;
    const char* cursorIn = num;
    while(true)
    {
		*cursorOut++ = *cursorIn++;
		if(*cursorIn)
        {
			if(++i == 3)
            {
				*cursorOut++ = ',';
				i = 0;
			}
		}
		else
        {
			break;
		}
	}
	*cursorOut = '\0';
	str = buf;
	delete[] buf;
}

void ListBoxStock::IncrementSubscriptionCount(int count)
{
	::SendMessage(::GetParent(m_hWnd), WM_USER + 200, (WPARAM) count, 0);
}

void ListBoxStock::IncrementLoadedCount(int count, bool subscribed)
{
	::SendMessage(::GetParent(m_hWnd), WM_USER + 201, (WPARAM) count, subscribed ? 1 : 0);
}

void ListBoxStock::DoDrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct)
{
    CDC* dc = CDC::FromHandle(lpDrawItemStruct->hDC);

    Stock* stock = (Stock*)lpDrawItemStruct->itemData;
    COLORREF color;
    if(stock->isLoaded())
    {
        color = m_textColor;
    }
    else
    {
        color = RGB(164, 0, 0);
    }

	if(lpDrawItemStruct->itemState & ODS_SELECTED)
    {
        dc->FillSolidRect(&lpDrawItemStruct->rcItem, GetSysColor(COLOR_HIGHLIGHT));
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
    r.right = r.left + 30;
    dc->DrawText(stock->GetSymbol().c_str(), -1, &r, DT_LEFT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 40;
    const MoneySize& trade = stock->GetLastTrade();
//    CString price = FormatPrice(trade);
    CString price;
    ExtFrame::FormatMoney(price, trade);
    dc->DrawText(price, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    char num[33];
    r.left = r.right;
    r.right = r.left + 40;
	_ultoa_s(trade.GetSize(), num, sizeof(num), 10);
    dc->DrawText(num, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 40;
//    price = FormatPrice(stock->GetClosePrice());
    price = "";
    ExtFrame::FormatMoney(price, stock->GetClosePrice());
    dc->DrawText(price, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

/*
    bool ecnsOnly = false;
    bool twoDecPlaces = false;

//how many lines for each book you want integrated in the level 2 quotes
//for the purpose of getting the best quote we need one line per book.
    unsigned int bookQuotesIntegrated[MAX_BOOKS];
    for(unsigned int i = 0; i < MAX_BOOKS; i++)
    {
        bookQuotesIntegrated[i] = 1;//number of book lines integrated in Level2
    }
*/
    const BookEntry* quote;

    r.left = r.right;
    r.right = r.left + 50;
//    unsigned int bidIterator = B_CreateLevel2AndBookIterator(stock->GetLevel2(), stock->GetAggregatedBook(), true, ecnsOnly, twoDecPlaces, bookQuotesIntegrated, MAX_BOOKS);
    void* bidIterator = stock->GetBidIterator();
    B_StartIteration(bidIterator);
    quote = B_GetNextBookEntry(bidIterator);
    if(quote)
    {
        price = "";
        ExtFrame::FormatMoney(price, *quote);
//        price = FormatPrice(*quote);
    }
    else
    {
        price = "0.000";
    }
//    B_DestroyIterator(bidIterator);
    dc->DrawText(price, -1, &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 15;
    dc->DrawText("X", 1, &r, DT_CENTER|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 50;
//    unsigned int askIterator = B_CreateLevel2AndBookIterator(stock->GetLevel2(), stock->GetAggregatedBook(), false, ecnsOnly, twoDecPlaces, bookQuotesIntegrated, MAX_BOOKS);
    void* askIterator = stock->GetAskIterator();
    B_StartIteration(askIterator);
    quote = B_GetNextBookEntry(askIterator);
    if(quote)
    {
//        price = FormatPrice(*quote);
        price = "";
        ExtFrame::FormatMoney(price, *quote);
    }
    else
    {
        price = "0.000";
    }
//    B_DestroyIterator(askIterator);
    dc->DrawText(price, -1, &r, DT_LEFT|DT_VCENTER|DT_SINGLELINE);


    r.left = r.right;
    r.right = r.left + 15;
    char bidtick = stock->GetBidTick();
    dc->DrawText(&bidtick, 1, &r, DT_LEFT|DT_VCENTER|DT_SINGLELINE);

	std::string str;
    r.left = r.right;
    r.right = r.left + 120;
	FormatUnsignedInteger64WithCommas(stock->GetNysVolume(), str);
	dc->DrawText(str.c_str(), (int)str.length(), &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);

    r.left = r.right;
    r.right = r.left + 120;
	str = "";
	FormatUnsignedInteger64WithCommas(stock->GetVolume(), str);
	dc->DrawText(str.c_str(), (int)str.length(), &r, DT_RIGHT|DT_VCENTER|DT_SINGLELINE);
}

void Stock::Process(const Message* message, Observable* from, const Message* additionalInfo)
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
        if(from == m_level1)
        {
			m_owningListBox->IncrementSubscriptionCount(1);
            m_loaded = true;
            Invalidate();
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

        if(from == m_level1)
        {
            RemoveFromListBox();
        }
        break;

        case M_NW2_INSIDE_QUOTE:
//This message comes from m_level1, when inside bid or ask change, or bid tick changes
//You can cast message to MsgInsideQuote*
//MsgInsideQuote* msg = (MsgInsideQuote*)message;
//See Messages.h
        Invalidate();
        break;

//
        case M_NW2_MM_QUOTE:
//This message comes from m_level2, when one or more level 2 quotes are added, removed or modified.
//You can cast message to MsgMMQuote*
//MsgMMQuote* msg = (MsgMMQuote*)message;
//See Messages.h
//This message may be obsolete, replaced by M_LEVEL2_QUOTE.
        if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_LEVEL2_QUOTE)
        {
            AIMsgMMLevel2Quote* info = (AIMsgMMLevel2Quote*)additionalInfo;
            Invalidate();
        }
        break;

        case M_LEVEL2_QUOTE:
//This message comes from m_level2, when a level 2 quote is added, removed or modified.
//You can cast message to MsgLevel2Quote*
//MsgLevel2Quote* msg = (MsgLevel2Quote*)message;
//See Messages.h
        if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_LEVEL2_QUOTE)
        {
            AIMsgMMLevel2Quote* info = (AIMsgMMLevel2Quote*)additionalInfo;
            Invalidate();
        }
        break;
/*
        case M_ITCH_MODIFY_ORDER:
//This message comes from m_expandedBook and m_aggregatedBook (in this order), when BRUT quote is removed or modified
//This is a direct notification from BRUT
//You can cast message to MsgItchModifyOrder*
//MsgItchModifyOrder* msg = (MsgItchModifyOrder*)message;
//See Messages.h
        if(B_IsEcnBook(m_stockHandle, from, true))
//        if(from == m_aggregatedBook)// do it once, the message comes 2 times, because we attached the Observer to both books
        {
            Invalidate();
        }
        break;

		case M_ITCH_ADD_ORDER:
//This message comes from m_expandedBook and m_aggregatedBook (in this order), when BRUT quote is added
//This is a direct notification from BRUT
//You can cast message to MsgItchAddOrder*
//MsgItchAddOrder* msg = (MsgItchAddOrder*)message;
//See Messages.h
        if(B_IsEcnBook(m_stockHandle, from, true))
//        if(from == m_aggregatedBook)// do it once, the message comes 2 times, because we attached the Observer to both books
        {
            Invalidate();
        }
        break;
*/
		case M_ITCH_1_00_NewVisibleOrder:
//This message comes from m_expandedBook and m_aggregatedBook (in this order), when ISLD quote is added
//This is a direct notification from ISLD
//You can cast message to MsgItch100AddOrder*
//MsgItch100AddOrder* msg = (MsgItch100AddOrder*)message;
//See Messages.h
        if(B_IsEcnBook(m_stockHandle, from, true))
//        if(from == m_aggregatedBook)// do it once, the message comes 2 times, because we attached the Observer to both books
        {
            Invalidate();
        }
        break;

		case M_ITCH_1_00_VisibleOrderExecution:
//This message comes from m_expandedBook and m_aggregatedBook (in this order), when ISLD quote is decremented in size or removed because of an execution
//This is a direct notification from ISLD
//You can cast message to MsgItch100VisibleOrderExecution*
//MsgItch100VisibleOrderExecution* msg = (MsgItch100VisibleOrderExecution*)message;
//See Messages.h
        if(B_IsEcnBook(m_stockHandle, from, true))
//        if(from == m_aggregatedBook)// do it once, the message comes 2 times, because we attached the Observer to both books
        {
            Invalidate();
        }
        break;

		case M_ITCH_1_00_CanceledOrder:
//This message comes from m_expandedBook and m_aggregatedBook (in this order), when ISLD quote is decremented in size or removed because of a cancel
//This is a direct notification from ISLD
//You can cast message to MsgItch100CancelOrder*
//MsgItch100CancelOrder* msg = (MsgItch100CancelOrder*)message;
//See Messages.h
        if(B_IsEcnBook(m_stockHandle, from, true))
//        if(from == m_aggregatedBook)// do it once, the message comes 2 times, because we attached the Observer to both books
        {
            Invalidate();
        }
        break;

		case M_BOOK_NEW_ORDER:
//This message comes from m_expandedBook and m_aggregatedBook (in this order), when an ECN (other than ISLD or BRUT) quote is added
//This is a direct notification from an ECN
//You can cast message to MsgBookNewOrder*
//MsgBookNewOrder* msg = (MsgBookNewOrder*)message;
//See Messages.h
        if(B_IsEcnBook(m_stockHandle, from, true))
//        if(from == m_aggregatedBook)// do it once, the message comes 2 times, because we attached the Observer to both books
        {
            Invalidate();
        }
        break;

		case M_BOOK_MODIFY_ORDER:
//This message comes from m_expandedBook and m_aggregatedBook (in this order), when an ECN (other than ISLD or BRUT) quote is modified
//This is a direct notification from an ECN
//You can cast message to MsgBookModifyOrder*
//MsgBookModifyOrder* msg = (MsgBookModifyOrder*)message;
//See Messages.h
        if(B_IsEcnBook(m_stockHandle, from, true))
//        if(from == m_aggregatedBook)// do it once, the message comes 2 times, because we attached the Observer to both books
        {
            Invalidate();
        }
        break;

		case M_BOOK_DELETE_ORDER:
//This message comes from m_expandedBook and m_aggregatedBook (in this order), when an ECN (other than ISLD or BRUT) quote is removed
//This is a direct notification from an ECN
//You can cast message to MsgBookDeleteOrder*
//MsgBookDeleteOrder* msg = (MsgBookDeleteOrder*)message;
//See Messages.h
        if(B_IsEcnBook(m_stockHandle, from, true))
//        if(from == m_aggregatedBook)// do it once, the message comes 2 times, because we attached the Observer to both books
        {
            Invalidate();
        }
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
			            Invalidate();
					}
				}
			}
			else// if(from != m_position)//MarketMaker book// if(m_mmLines != 0)
			{
	            Invalidate();
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
			            Invalidate();
					}
				}
		        else// if(from != m_position)
				{
//					AddPrint();
				}
			}
			else// if(from != m_position)// if(m_mmLines != 0)
			{
	            Invalidate();
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
			            Invalidate();
					}
				}
			}
			else// if(from != m_position)// if(m_mmLines != 0)
			{
				Invalidate();
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
/*
		case M_ITCH_1_00_HiddenAttributedOrderExecution:
        break;
*/

		case M_BOOK_NYSE_OPEN_BOOK:
//This message comes from m_expandedBook and m_aggregatedBook (in this order), when one or more NYSE quotes are added removed or modified
//This is a direct notification from NYSE
//You can cast message to MsgNyseOpenBook*
//MsgNyseOpenBook* msg = (MsgNyseOpenBook*)message;
//See Messages.h
        if(B_IsEcnBook(m_stockHandle, from, true))
//        if(from == m_aggregatedBook)// do it once, the message comes 2 times, because we attached the Observer to both books
        {
            MsgNyseOpenBook* msg = (MsgNyseOpenBook*)message;
            Invalidate();
        }
        break;

		case M_FLUSH_ALL:
//This message comes from m_expandedBook m_aggregatedBook, m_level1 and m_level2 (in this order), when Hammer server has a problem and requests that you dismiss all the quotes and Level 1 info for a stock. 
//You can cast message to MsgFlushAll*
//MsgFlushAll* msg = (MsgFlushAll*)message;
//See Messages.h
        case M_FLUSH_ALL_OPEN_BOOKS:
//This message comes from m_expandedBook m_aggregatedBook (in this order), when Hammer server has a problem and requests that you dismiss all the book quotes for a stock.
//You can cast message to MsgFlushAllOpenBooks*
//MsgFlushAllOpenBooks* msg = (MsgFlushAllOpenBooks*)message;
//See Messages.h
        case M_FLUSH_BOOK_FOR_STOCK:
//This message comes from m_expandedBook m_aggregatedBook (in this order), when Hammer server has a problem and requests that you dismiss all the quotes of a specific book for a stock.
//You can cast message to MsgFlushBookForStock*
//MsgFlushBookForStock* msg = (MsgFlushBookForStock*)message;
//See Messages.h
        Invalidate();
        break;


        case M_LAST_TRADE_SHORT:
//This message comes from m_level1 and m_prints (in this order), when there is a new trade reported
//You can cast message to MsgLastTradeShort*
//MsgLastTradeShort* msg = (MsgLastTradeShort*)message;
//See Messages.h
/*
        case M_TAL_LAST_TRADE:
//This message comes from m_level1 and m_prints (in this order).
//You can cast message to MsgTalLastTrade*
//MsgTalLastTrade* msg = (MsgTalLastTrade*)message;
//See Messages.h
*/
        if(from == m_prints)
        {
			const MsgLastTradeShort* msg = (const MsgLastTradeShort*)message;
			if(msg->m_ExecutionExchange == ExecExch_NYS)
			{
				m_nysVolume += msg->m_LastTradeVolume;
			}
			m_volume += msg->m_LastTradeVolume;
            Invalidate();
        }
        break;

		case M_RESP_TO_CLIENT_SYMBOL_UNSUBSCRIBED:
        if(from == m_level1)
        {
			m_owningListBox->IncrementSubscriptionCount(-1);
			m_loaded = false;
		}
		break;
    }
}

/////////////////////////////////////////////////////////////////////////////
// LoadDlg dialog


LoadDlg::LoadDlg(CWnd* pParent):
    BaseDlg(LoadDlg::IDD, pParent),
	m_subscribeCount(0)
{
	//{{AFX_DATA_INIT(LoadDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	B_GetAdminObservable()->Add(this);
}

/*
void LoadDlg::OnCancel()
{
//    Clear();
    ExtFrame::GetInstance()->DestroyLoadDlg(this);
}
*/
void LoadDlg::OnOK()
{
    if(!B_IsMarketSummaryPopulationDone())
    {
        MessageBox("Market is not loaded yet", "Wait");
        return;
    }

//    m_ListBoxStocks.SetRedraw(FALSE);
    m_ListBoxStocks.ResetContent();

    Money from(LOWORD(m_SpinPriceDollarsFrom.GetPos()), LOWORD(m_SpinPriceCentsFrom.GetPos()) * 10);
    Money to(LOWORD(m_SpinPriceDollarsTo.GetPos()), LOWORD(m_SpinPriceCentsTo.GetPos()) * 10);

    void* iterator = B_CreateStockMovementIterator();
    B_StartIteration(iterator);

//StockMovement is a class declared in BusinessApi.h
//The name might be misleading. StockMovement is just an object containing some info about a stock.
//Using that info we will decide which stocks we want to load from the Hammer server in entirety (in this case - last price).
    const StockMovement* stock;
    const Money* lastPrice;
    char num[33];

//    std::set<std::string> stocksToLoad;
    unsigned int i = 0;
    Stock* s;

    while(stock = B_GetNextStockMovement(iterator))
    {
        lastPrice = &stock->GetLastTradePrice();
        if(lastPrice->isZero())//no trading today
        {
            lastPrice = &stock->GetClosePrice();
        }
        if(!lastPrice->isZero())
        {
            if(*lastPrice >= from && *lastPrice <= to)
            {
//                stocksToLoad.insert(std::string(stock->GetSymbol()));
                s = new Stock(stock);
//                s->SetIndex(i);
                if(m_ListBoxStocks.AddStock(s))
                {
                    ++i;
					_ultoa_s(i, num, sizeof(num), 10);
                    m_StaticStocksLoaded.SetWindowText(num);
					if(s->isLoaded())
					{
						OnSubscriptionCountIncrement(1, 0);
					}
                }
                else
                {
                    delete s;
                }
            }
        }
    }

    B_DestroyIterator(iterator);

    unsigned int count = m_ListBoxStocks.GetCount();

    for(i = 0; i < count; ++i)
    {
        ((Stock*)m_ListBoxStocks.GetItemDataPtr(i))->SetIndex(i);
    }

//    m_StaticStocksLoaded.SetWindowText(itoa(count, num, 10));
}

void LoadDlg::DoDataExchange(CDataExchange* pDX)
{
	BaseDlg::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(LoadDlg)
	DDX_Control(pDX, IDS_STOCKS_SUBSCRIBED, m_StaticStocksSubscribed);
	DDX_Control(pDX, IDS_STOCKS_LOADED, m_StaticStocksLoaded);
	DDX_Control(pDX, IDL_STOCKS, m_ListBoxStocks);
	DDX_Control(pDX, IDSP_PRICE_TO_DOLLARS, m_SpinPriceDollarsTo);
	DDX_Control(pDX, IDSP_PRICE_TO_CENTS, m_SpinPriceCentsTo);
	DDX_Control(pDX, IDSP_PRICE_FROM_DOLLARS, m_SpinPriceDollarsFrom);
	DDX_Control(pDX, IDSP_PRICE_FROM_CENTS, m_SpinPriceCentsFrom);
	DDX_Control(pDX, IDE_PRICE_TO_DOLLARS, m_EditPriceDollarsTo);
	DDX_Control(pDX, IDE_PRICE_TO_CENTS, m_EditPriceCentsTo);
	DDX_Control(pDX, IDE_PRICE_FROM_DOLLARS, m_EditPriceDollarsFrom);
	DDX_Control(pDX, IDE_PRICE_FROM_CENTS, m_EditPriceCentsFrom);
	DDX_Control(pDX, IDS_MARKET_LOADED, m_StaticMarketLoaded);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(LoadDlg, BaseDlg)
	//{{AFX_MSG_MAP(LoadDlg)
	ON_WM_VSCROLL()
	ON_EN_CHANGE(IDE_PRICE_FROM_CENTS, OnChangePriceFromCents)
	ON_EN_CHANGE(IDE_PRICE_FROM_DOLLARS, OnChangePriceFromDollars)
	ON_EN_CHANGE(IDE_PRICE_TO_CENTS, OnChangePriceToCents)
	ON_EN_CHANGE(IDE_PRICE_TO_DOLLARS, OnChangePriceToDollars)
	ON_EN_KILLFOCUS(IDE_PRICE_FROM_CENTS, OnKillfocusPriceFromCents)
	ON_EN_KILLFOCUS(IDE_PRICE_FROM_DOLLARS, OnKillfocusPriceFromDollars)
	ON_EN_KILLFOCUS(IDE_PRICE_TO_CENTS, OnKillfocusPriceToCents)
	ON_EN_KILLFOCUS(IDE_PRICE_TO_DOLLARS, OnKillfocusPriceToDollars)
	//}}AFX_MSG_MAP
	ON_MESSAGE(WM_USER + 200, OnSubscriptionCountIncrement)
	ON_MESSAGE(WM_USER + 201, OnLoadedCountIncrement)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// LoadDlg message handlers

LRESULT LoadDlg::OnLoadedCountIncrement(WPARAM count, LPARAM loaded)
{
	if(loaded)
	{
		OnSubscriptionCountIncrement(count, 0);
	}
	char num[33];
	_ultoa_s(m_ListBoxStocks.GetCount(), num, sizeof(num), 10);
    m_StaticStocksLoaded.SetWindowText(num);
	return 0;
}

void LoadDlg::UpdateSubscriptionCount()
{
	char num[33];
	_ultoa_s(m_subscribeCount, num, sizeof(num), 10);
	m_StaticStocksSubscribed.SetWindowText(num);
}

LRESULT LoadDlg::OnSubscriptionCountIncrement(WPARAM count, LPARAM)
{
	unsigned int oldValue = m_subscribeCount;
	int c = (int)count;
	if(c > 0)
	{
		m_subscribeCount += c;
	}
	else
	{
		unsigned int uc = -c;
		if(uc < m_subscribeCount)
		{
			m_subscribeCount -= uc;
		}
		else
		{
			m_subscribeCount = 0;
		}
	}
	if(oldValue != m_subscribeCount)
	{
		UpdateSubscriptionCount();
	}
	return 0;
}

BOOL LoadDlg::OnInitDialog()
{
	BaseDlg::OnInitDialog();
	
	// TODO: Add extra initialization here
    m_StaticMarketLoaded.SetWindowText(B_IsMarketSummaryPopulationDone() ? "Yes" : "No");

    ExtFrame::SetDollarsAndCents(m_SpinPriceDollarsFrom, m_EditPriceDollarsFrom,
        m_SpinPriceCentsFrom, m_EditPriceCentsFrom,
        5000);

    ExtFrame::SetDollarsAndCents(m_SpinPriceDollarsTo, m_EditPriceDollarsTo,
        m_SpinPriceCentsTo, m_EditPriceCentsTo,
        40000, 1000);

    ExtFrame::ScrollEnd(m_EditPriceDollarsFrom);

    m_StaticStocksLoaded.SetWindowText("0");
	m_StaticStocksSubscribed.SetWindowText("0");

    return TRUE;
}

void LoadDlg::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	// TODO: Add your message handler code here and/or call default
	
	switch(nSBCode)
    {
        case SB_THUMBPOSITION:
        switch(pScrollBar->GetDlgCtrlID())
        {
            case IDSP_PRICE_FROM_DOLLARS:
            ExtFrame::IncrementSpin(m_SpinPriceDollarsFrom, m_EditPriceDollarsFrom, nPos);
            break;
	
            case IDSP_PRICE_FROM_CENTS:
            ExtFrame::IncrementSpin(m_SpinPriceCentsFrom, m_EditPriceCentsFrom, nPos);
            break;

            case IDSP_PRICE_TO_DOLLARS:
            ExtFrame::IncrementSpin(m_SpinPriceDollarsTo, m_EditPriceDollarsTo, nPos);
            break;
	
            case IDSP_PRICE_TO_CENTS:
            ExtFrame::IncrementSpin(m_SpinPriceCentsTo, m_EditPriceCentsTo, nPos);
            break;
        }
        break;

        case SB_ENDSCROLL:
        switch(pScrollBar->GetDlgCtrlID())
        {
            case IDSP_PRICE_FROM_DOLLARS:
            ExtFrame::ScrollEnd(m_EditPriceDollarsFrom);
            break;

            case IDSP_PRICE_FROM_CENTS:
            ExtFrame::ScrollEnd(m_EditPriceCentsFrom);
            break;

            case IDSP_PRICE_TO_DOLLARS:
            ExtFrame::ScrollEnd(m_EditPriceDollarsTo);
            break;

            case IDSP_PRICE_TO_CENTS:
            ExtFrame::ScrollEnd(m_EditPriceCentsTo);
            break;
        }
    }
//	BaseDlg::OnVScroll(nSBCode, nPos, pScrollBar);
}

void LoadDlg::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
    switch(message->GetType())
    {

		case MSGID_CONNECTION_LOST:
		if(from == B_GetMarketReceiver())
		{
			m_ListBoxStocks.ResetContent();
			m_subscribeCount = 0;
			UpdateSubscriptionCount();
		}
		break;

		case MSGID_CONNECTION_MADE:
		if(from == B_GetMarketReceiver())
		{
			if(B_IsMarketSummaryReceiverConnected())
			{
				OnOK();
			}
		}
		break;
	}
}

void LoadDlg::MarketSummaryPopulationDone(bool done)
{
    m_StaticMarketLoaded.SetWindowText(done ? "Yes" : "No");
}

void LoadDlg::OnChangePriceFromCents() 
{
    ExtFrame::SpinChanged(m_SpinPriceCentsFrom, m_EditPriceCentsFrom);
}

void LoadDlg::OnChangePriceFromDollars() 
{
    ExtFrame::SpinChanged(m_SpinPriceDollarsFrom, m_EditPriceDollarsFrom);
}

void LoadDlg::OnChangePriceToCents() 
{
    ExtFrame::SpinChanged(m_SpinPriceCentsTo, m_EditPriceCentsTo);
}

void LoadDlg::OnChangePriceToDollars() 
{
    ExtFrame::SpinChanged(m_SpinPriceDollarsTo, m_EditPriceDollarsTo);
}

void LoadDlg::OnKillfocusPriceFromCents() 
{
    ExtFrame::AdjustEdit(m_SpinPriceCentsFrom, m_EditPriceCentsFrom, true);
}

void LoadDlg::OnKillfocusPriceFromDollars() 
{
    ExtFrame::AdjustEdit(m_SpinPriceDollarsFrom, m_EditPriceDollarsFrom, false);
}

void LoadDlg::OnKillfocusPriceToCents() 
{
    ExtFrame::AdjustEdit(m_SpinPriceCentsTo, m_EditPriceCentsTo, true);
}

void LoadDlg::OnKillfocusPriceToDollars() 
{
    ExtFrame::AdjustEdit(m_SpinPriceDollarsTo, m_EditPriceDollarsTo, false);
}
/*
void LoadDlg::OnDestroy() 
{
//    m_ListBoxStocks.ResetContent();
	BaseDlg::OnDestroy();
	
	// TODO: Add your message handler code here
	
}
*/

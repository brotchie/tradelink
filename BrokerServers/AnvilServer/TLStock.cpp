#include "stdafx.h"

#include "TLStock.h"
#include "Messages.h"
#include "SendMsg.h"
#include "TradeLink.h"
#include "TradeLinkServer.h"
#include "AnvilUtil.h"
using namespace TradeLinkServer;


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//const unsigned int TLStock::m_maxLevels = 4;
//const Money TLStock::m_maxThrough = Money(0, 40);

#pragma warning (disable:4355)


TLStock::TLStock(const char* symbol, bool load):
    m_symbol(symbol),
    m_stockHandle(NULL),
    m_level1(NULL),
    m_level2(NULL),
    m_prints(NULL),
    m_owningListBox(NULL),
//        m_loaded(false),
    m_index(0xFFFFFFFF),
    m_bidIterator(NULL),
    m_askIterator(NULL),
    m_stockCalcsLevelsBid(NULL),
    m_stockCalcsLevelsAsk(NULL),
    m_stockCalcsPricesBid(NULL),
    m_stockCalcsPricesAsk(NULL),
    m_levelBidIterator(NULL),
    m_levelAskIterator(NULL),
    m_priceBidIterator(NULL),
    m_priceAskIterator(NULL),
    m_position(NULL)
{
    if(load)
    {
        Load();
    }
}

void TLStock::SetPosition(Position* pos)
{
    if(m_position != pos)
    {
        if(m_position)
        {
            m_position->Remove(this);
        }
        m_position = pos;
        if(m_position)
        {
            m_position->Add(this);
        }
    }
}

void TLStock::Clear()
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

    if(m_levelBidIterator)
    {
        B_DestroyIterator(m_levelBidIterator);
        m_levelBidIterator = NULL;
    }
    if(m_levelAskIterator)
    {
        B_DestroyIterator(m_levelAskIterator);
        m_levelAskIterator = NULL;
    }
    if(m_priceBidIterator)
    {
        B_DestroyIterator(m_priceBidIterator);
        m_priceBidIterator = NULL;
    }
    if(m_priceAskIterator)
    {
        B_DestroyIterator(m_priceAskIterator);
        m_priceAskIterator = NULL;
    }

    if(m_stockCalcsLevelsBid)
    {
        B_DestroyStockCalc(m_stockCalcsLevelsBid);
        m_stockCalcsLevelsBid = NULL;
    }
    if(m_stockCalcsLevelsAsk)
    {
        B_DestroyStockCalc(m_stockCalcsLevelsAsk);
        m_stockCalcsLevelsAsk = NULL;
    }
    if(m_stockCalcsPricesBid)
    {
        B_DestroyStockCalc(m_stockCalcsPricesBid);
        m_stockCalcsPricesBid = NULL;
    }
    if(m_stockCalcsPricesAsk)
    {
        B_DestroyStockCalc(m_stockCalcsPricesAsk);
        m_stockCalcsPricesAsk = NULL;
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
    if(m_prints)
    {
        m_prints->Remove(this);
        m_prints = NULL;
    }
    m_stockHandle = NULL;
}

TLStock::~TLStock()
{
    Clear();
}


void TLStock::Load()
{
    m_stockHandle = B_GetStockHandle(m_symbol.c_str());//This will load the stock from the Hammer server. The stock will not be available immediately for monitoring and trading, only after Hammer server sends all the info about the stock.
	if(m_stockHandle)
	{
		m_level1 = B_GetLevel1(m_stockHandle);
		m_level2 = B_GetLevel2(m_stockHandle);
		m_prints = B_GetPrints(m_stockHandle);
		m_account = B_GetCurrentAccount();

	//Add 'this' as an Observer to a bunch of Observables.
		m_level1->Add(this);
		m_prints->Add(this);
	    m_level2->Add(this);
		m_account->Add(this);

		bool ecnsOnly = false;
		bool twoDecPlaces = false;


		unsigned int bookQuotesIntegrated[MAX_BOOKS];
		for(unsigned int i = 0; i < MAX_BOOKS; i++)
		{
			if(i == NYSE_BOOK)
			{
				bookQuotesIntegrated[i] = 0;
			}
			else
			{
				bookQuotesIntegrated[i] = 0xFFFFFFFF;
			}
		}

		twoDecPlaces = true;
	}
	else
	{
		//m_owningListBox->IncrementInvalidCount();
	}
}

void TLStock::SetUseMmBooks(bool use)
{
    if(m_stockHandle != NULL)
    {
        if(m_stockCalcsPricesBid)
        {
			B_SetStockCalcMmBookLines(m_stockCalcsPricesBid, use ? 0xFFFFFFFF : 0);
        }
        if(m_stockCalcsPricesAsk)
        {
            B_SetStockCalcMmBookLines(m_stockCalcsPricesAsk, use ? 0xFFFFFFFF : 0);
        }
        if(m_stockCalcsLevelsBid)
        {
            B_SetStockCalcMmBookLines(m_stockCalcsLevelsBid, use ? 0xFFFFFFFF : 0);
        }
        if(m_stockCalcsLevelsAsk)
        {
            B_SetStockCalcMmBookLines(m_stockCalcsLevelsAsk, use ? 0xFFFFFFFF : 0);
        }
    }
}

void TLStock::SetEcnsOnlyBeforeAfterMarket(bool ecnsOnly)
{
    if(m_stockHandle != NULL)
    {
        if(m_stockCalcsPricesBid)
        {
            B_SetStockCalcEcnsOnlyBeforeAfterMarket(m_stockCalcsPricesBid, ecnsOnly);
        }
        if(m_stockCalcsPricesAsk)
        {
            B_SetStockCalcEcnsOnlyBeforeAfterMarket(m_stockCalcsPricesAsk, ecnsOnly);
        }
        if(m_stockCalcsLevelsBid)
        {
            B_SetStockCalcEcnsOnlyBeforeAfterMarket(m_stockCalcsLevelsBid, ecnsOnly);
        }
        if(m_stockCalcsLevelsAsk)
        {
            B_SetStockCalcEcnsOnlyBeforeAfterMarket(m_stockCalcsLevelsAsk, ecnsOnly);
        }
    }
}

void TLStock::SetEcnsOnlyDuringMarket(bool ecnsOnly)
{
    if(m_stockHandle != NULL)
    {
        if(m_stockCalcsPricesBid)
        {
            B_SetStockCalcEcnsOnlyDuringMarket(m_stockCalcsPricesBid, ecnsOnly);
        }
        if(m_stockCalcsPricesAsk)
        {
            B_SetStockCalcEcnsOnlyDuringMarket(m_stockCalcsPricesAsk, ecnsOnly);
        }
        if(m_stockCalcsLevelsBid)
        {
            B_SetStockCalcEcnsOnlyDuringMarket(m_stockCalcsLevelsBid, ecnsOnly);
        }
        if(m_stockCalcsLevelsAsk)
        {
            B_SetStockCalcEcnsOnlyDuringMarket(m_stockCalcsLevelsAsk, ecnsOnly);
        }
    }
}

void TLStock::SetMonitorThroughLimit(const Money& throughLimit)
{
    if(m_stockHandle != NULL)
    {
        if(m_stockCalcsPricesBid)
        {
            B_SetStockCalcThroughLimit(m_stockCalcsPricesBid, throughLimit);
        }
        if(m_stockCalcsPricesAsk)
        {
            B_SetStockCalcThroughLimit(m_stockCalcsPricesAsk, throughLimit);
        }
    }
}

void TLStock::SetMonitorLevelLimit(unsigned int levelLimit)
{
    if(m_stockHandle != NULL)
    {
        if(m_stockCalcsLevelsBid)
        {
            B_SetStockCalcLevelLimit(m_stockCalcsLevelsBid, levelLimit);
        }
        if(m_stockCalcsLevelsAsk)
        {
            B_SetStockCalcLevelLimit(m_stockCalcsLevelsAsk, levelLimit);
        }
    }
}


void TLStock::InitNysQuote(bool side)
{
    MoneySize& quote = side ? m_nysBid : m_nysAsk;
    const BookEntry* be = B_FindLevel2QuoteByMmid(m_level2, side, "NYS");
    if(be)
    {
        quote = *be;
    }
    else
    {
        quote.SetZero();
    }
}

void TLStock::InitNysQuotes()
{
    InitNysQuote(true);
    InitNysQuote(false);
}

void TLStock::UpdateNysQuote(bool side, const MoneySize& q)
{
    (side ? m_nysBid : m_nysAsk) = q;
}

bool TLStock::UpdateBestQuote(bool side)
{
	return false; // TRADELINK (do we need this? think not)
    const StockCalc* stockCalc = side ? GetLevelBid() : GetLevelAsk();
    Money& bestQuote = side ? m_bestBid : m_bestAsk;
    const Money* best = stockCalc->GetLevelPrice(0);
    if(best)
    {
        if(bestQuote != *best)
        {
            bestQuote = *best;
            return true;
        }
    }
    else
    {
        if(!bestQuote.isZero())
        {
            bestQuote.SetZero();
            return true;
        }
    }
    return false;
}

void TLStock::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
    switch(message->GetType())
    {
        case M_RESP_REFRESH_SYMBOL:
			{

            Invalidate();
            UpdateBestQuote(true);
            UpdateBestQuote(false);

//NYSINFO
            InitNysQuotes();

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
//            RemoveFromListBox();
            Clear();
//            m_owningListBox->IncrementInvalidCount();
            Invalidate();
        }

        break;

        case M_NW2_INSIDE_QUOTE:
//This message comes from m_level1, when inside bid or ask change, or bid tick changes
//You can cast message to MsgInsideQuote*
//MsgInsideQuote* msg = (MsgInsideQuote*)message;
//See Messages.h
        if(from != m_position)
        {
            Invalidate();
        }
        break;

/*
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
*/
        case M_LEVEL2_QUOTE:
//This message comes from m_level2, when a level 2 quote is added, removed or modified.
//You can cast message to MsgLevel2Quote*
//MsgLevel2Quote* msg = (MsgLevel2Quote*)message;
//See Messages.h

        if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_LEVEL2_QUOTE)
        {

				AIMsgMMLevel2Quote* info = (AIMsgMMLevel2Quote*)additionalInfo;
				MsgLevel2Quote* msg = (MsgLevel2Quote*)message;
				time_t now;
				time(&now);
				CTime ct(now);
				int date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();
				int time = (ct.GetHour()*100)+ct.GetMinute();
				int sec = ct.GetSecond();
				TradeLinkServer::TLTick tick;
				tick.sym = msg->m_Symbol;
				tick.date = date;
				tick.time = time;
				tick.sec = sec;
				tick.bid = (double)msg->m_BidPrice/1024;
				tick.ask = (double)msg->m_AskPrice/1024;
				tick.ex = info->m_mmid;
				tick.bs = msg->m_BidSize;
				tick.os = msg->m_AskSize;
				tick.be = info->m_mmid;
				tick.oe = info->m_mmid;
				std::vector <CString> subname;
				// get TL subscribers to this stock
				Subscribers(msg->m_Symbol,subname);

				// send each subscriber a copy of the trade object
				for (size_t i = 0; i< subname.size(); i++) 
					SendMsg(TICKNOTIFY,tick.Serialize(),subname[i]);


            if(!info->isDirectEcn)
            {
                bool nys = !strcmp(info->m_mmid, "NYS");
                if(info->bidChanged)
                {
                    UpdateBestQuote(true);
//NYSINFO
                    if(nys)
                    {
                        UpdateNysQuote(true, info->m_becameBid);
                    }

                    Invalidate();
                }
                if(info->askChanged)
                {
                    UpdateBestQuote(false);
//NYSINFO
                    if(nys)
                    {
                        UpdateNysQuote(false, info->m_becameAsk);
                    }

                    Invalidate();
                }
            }

        }

        break;

		case M_ITCH_1_00_NewVisibleOrder:

        if(B_IsEcnBook(m_stockHandle, from, true))
//        if(from == m_aggregatedBook)// do it once, the message comes 2 times, because we attached the Observer to both books
        {
            Invalidate();

            UpdateBestQuote(((MsgItch100AddOrder*)message)->m_BuySellIndicator == 'B');
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
            UpdateBestQuote(((MsgItch100VisibleOrderExecution*)message)->m_BuySellIndicator == 'B');
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
            UpdateBestQuote(((MsgItch100CancelOrder*)message)->m_BuySellIndicator == 'B');
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
            UpdateBestQuote(((MsgBookNewOrder*)message)->m_Side == SIDE_BUY);
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
            UpdateBestQuote(((MsgBookModifyOrder*)message)->m_Side == SIDE_BUY);
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
            UpdateBestQuote(((MsgBookDeleteOrder*)message)->m_Side == SIDE_BUY);
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
						UpdateBestQuote(msg->m_BuySellIndicator == 'B');
					}
				}
			}
			else if(from != m_position)//MarketMaker book// if(m_mmLines != 0)
			{
	            Invalidate();
				UpdateBestQuote(msg->m_BuySellIndicator == 'B');
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
						UpdateBestQuote(msg->m_BuySellIndicator == 'B');
					}
				}
		        else if(from != m_position)
				{
//					AddPrint();
				}
			}
			else if(from != m_position)// if(m_mmLines != 0)
			{
	            Invalidate();
				UpdateBestQuote(msg->m_BuySellIndicator == 'B');
//					AddPrint();
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
						UpdateBestQuote(msg->m_BuySellIndicator == 'B');
					}
				}
			}
			else if(from != m_position)// if(m_mmLines != 0)
			{
				Invalidate();
				UpdateBestQuote(msg->m_BuySellIndicator == 'B');
			}
		}
        break;
/*
		case M_ITCH_1_00_HiddenAttributedOrderExecution:
        break;
*/
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
*/
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
		case M_FLUSH_ATTRIBUTED_BOOK:
		case M_FLUSH_ATTRIBUTED_BOOK_FOR_STOCK:
        Invalidate();
        {
            UpdateBestQuote(true);
            UpdateBestQuote(false);
//NYSINFO
            InitNysQuotes();

            unsigned int level;
            GetLevelBid()->GetMmidFirstQuote("ISLD", level, m_isldBidQuote);
            GetLevelAsk()->GetMmidFirstQuote("ISLD", level, m_isldAskQuote);
        }
        break;


        case M_LAST_TRADE_SHORT:
//This message comes from m_level1 and m_prints (in this order), when there is a new trade reported
//You can cast message to MsgLastTradeShort*
//MsgLastTradeShort* msg = (MsgLastTradeShort*)message;
//See Messages.h
        if(from == m_prints)
        {
			// send ticks with isTrade set
            MsgLastTradeShort* msg = (MsgLastTradeShort*)message;
			TLTick tick;
			tick.ex = CString(ExchangeName(msg->m_ExecutionExchange));
			tick.trade = (double)msg->m_price/1024;
			tick.sym = CString(msg->m_Symbol);
			tick.size = msg->m_LastTradeVolume;
			time_t now;
			time(&now);
			CTime ct(now);
			int date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();
			int time = (ct.GetHour()*100)+ct.GetMinute();
			tick.time = time;
			tick.date = date;
			tick.sec = ct.GetSecond();
			std::vector <CString> subname;
			Subscribers(msg->m_Symbol,subname);
			// send each subscriber a copy of the trade object
			for (size_t i = 0; i< subname.size(); i++) 
				SendMsg(TICKNOTIFY,tick.Serialize(),subname[i]);
            m_nysPrint.SetMoney(Money(msg->m_price));
            m_nysPrint.SetShares(msg->m_LastTradeVolume);
            Invalidate();
        }
        break;

/*
        case M_TAL_LAST_TRADE:
//This message comes from m_level1 and m_prints (in this order).
//You can cast message to MsgTalLastTrade*
		//MsgTalLastTrade* msg = (MsgTalLastTrade*)message;
//See Messages.h
        break;
*/

/*
        case M_POSITION_INVESTMENTCHANGE:
        case M_POSITION_PENDINGMONEYCHANGE:
        case M_POSITION_PENDINGORDERSCOUNTCHANGE:
//        case M_NW2_INSIDE_QUOTE:
        Invalidate();
        break;
*/
        case M_POOL_EXECUTION:
//Notification about a new execution.
        if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_EXECUTION)
        {
			// TRADELINK
            MsgPoolExecution* msg = (MsgPoolExecution*)message;//to get the structure, just cast Message* to  MsgPoolExecution* (not used here)
            AIMsgExecution* info = (AIMsgExecution*)additionalInfo;
            Order* order = info->m_order;

	         const Execution* exec = info->m_execution;
            const Position* position = info->m_position;
	
		}
		break;

        case M_POOL_UPDATE_ORDER:// Order status is modified
        if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_ORDER)
        {
            AIMsgOrder* info = (AIMsgOrder*)additionalInfo;
            Order* order = info->m_order;
            const Position* position = info->m_position;

			if(order->isDead())
			{
				unsigned int rejectedShares = order->GetRejectedSize();
				const MsgUpdateOrder* msg = (const MsgUpdateOrder*)message;
				bool rejected = false;
				switch(msg->m_order.x_Tracking)
				{
					case TR_REJECTED_BY_SERVER:
					case TR_REJECTED_BY_MARKET:
					rejected = true;
					break;
				}
			}
		}
		break;
    }
}


void TLStock::RemoveFromListBox()
{
/*    if(m_owningListBox && m_index != 0xFFFFFFFF)
    {
        unsigned int count = m_owningListBox->GetCount();
        for(unsigned int i = m_index + 1; i < count; ++i)
        {
            ((Stock*)m_owningListBox->GetItemDataPtr(i))->SetIndex(i - 1);
        }
//        Stock* stock = (Stock*)m_owningListBox->GetItemDataPtr(m_index);
        m_owningListBox->DecrementCount(this);
        CListBox* lb = m_owningListBox;
        m_owningListBox = NULL;
        lb->DeleteString(m_index);
	
    }*/
}

void TLStock::Invalidate()
{
/*    if(m_owningListBox && m_owningListBox->GetDisplayMode() != ListBoxTLStock::DM_NONE && m_index != 0xFFFFFFFF)
    {
        m_owningListBox->InvalidateItem(m_index);
    }*/
}

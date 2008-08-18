#include "stdafx.h"

#include "AVLStock.h"
#include "Messages.h"
#include "TradeLink.h"
#include "AnvilUtil.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//const unsigned int AVLStock::m_maxLevels = 4;
//const Money AVLStock::m_maxThrough = Money(0, 40);



#pragma warning (disable:4355)

AVLStock::AVLStock(const char* symbol, TradeLinkServer::TradeLink_WM* tlserver,bool load):
		tl(tlserver),
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

void AVLStock::SetPosition(Position* pos)
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

void AVLStock::Clear()
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

AVLStock::~AVLStock()
{
    Clear();
}


void AVLStock::Load()
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

void AVLStock::SetUseMmBooks(bool use)
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

void AVLStock::SetEcnsOnlyBeforeAfterMarket(bool ecnsOnly)
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

void AVLStock::SetEcnsOnlyDuringMarket(bool ecnsOnly)
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

void AVLStock::SetMonitorThroughLimit(const Money& throughLimit)
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

void AVLStock::SetMonitorLevelLimit(unsigned int levelLimit)
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


void AVLStock::InitNysQuote(bool side)
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

void AVLStock::InitNysQuotes()
{
    InitNysQuote(true);
    InitNysQuote(false);
}

void AVLStock::UpdateNysQuote(bool side, const MoneySize& q)
{
    (side ? m_nysBid : m_nysAsk) = q;
}

bool AVLStock::UpdateBestQuote(bool side)
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

std::vector<int> fillids;

bool hasFillID(int id)
{
	for (size_t i = 0; i<fillids.size(); i++)
		if (fillids[i]==id) return true;
	return false;
}

void AVLStock::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
    switch(message->GetType())
    {
        case M_RESP_REFRESH_SYMBOL:

            UpdateBestQuote(true);
            UpdateBestQuote(false);

//NYSINFO
            InitNysQuotes();

        break;

        case M_RESP_REFRESH_SYMBOL_FAILED:


        if(from == m_level1)
        {
            Clear();
        }

        break;

        case M_NW2_INSIDE_QUOTE:
        break;

        case M_LEVEL2_QUOTE:
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
				tl->SrvGotTick(tick);
			}

        break;

		case M_ITCH_1_00_NewVisibleOrder:
        break;

		case M_ITCH_1_00_CanceledOrder:
        if(B_IsEcnBook(m_stockHandle, from, true))
        {
            UpdateBestQuote(((MsgItch100CancelOrder*)message)->m_BuySellIndicator == 'B');
        }
        break;

		case M_BOOK_NEW_ORDER:
        if(B_IsEcnBook(m_stockHandle, from, true))
        {

            UpdateBestQuote(((MsgBookNewOrder*)message)->m_Side == SIDE_BUY);
        }
        break;

		case M_BOOK_MODIFY_ORDER:
        if(B_IsEcnBook(m_stockHandle, from, true))
        {

            UpdateBestQuote(((MsgBookModifyOrder*)message)->m_Side == SIDE_BUY);
        }
        break;

		case M_BOOK_DELETE_ORDER:
        if(B_IsEcnBook(m_stockHandle, from, true))
        {

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

						UpdateBestQuote(msg->m_BuySellIndicator == 'B');
					}
				}
			}
			else if(from != m_position)//MarketMaker book// if(m_mmLines != 0)
			{

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

						UpdateBestQuote(msg->m_BuySellIndicator == 'B');
					}
				}
			}
			else if(from != m_position)// if(m_mmLines != 0)
			{

				UpdateBestQuote(msg->m_BuySellIndicator == 'B');
			}
		}
        break;
		case M_FLUSH_ALL:
        case M_FLUSH_ALL_OPEN_BOOKS:
        case M_FLUSH_BOOK_FOR_STOCK:
		case M_FLUSH_ATTRIBUTED_BOOK:
		case M_FLUSH_ATTRIBUTED_BOOK_FOR_STOCK:

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
        if(from == m_prints)
        {
			// get time
			time_t now;
			time(&now);
			CTime ct(now);
			int date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();
			int time = (ct.GetHour()*100)+ct.GetMinute();
			int sec = ct.GetSecond();
            MsgLastTradeShort* msg = (MsgLastTradeShort*)message;

			TradeLinkServer::TLTrade fill;
			fill.symbol = msg->m_Symbol;
			fill.xprice = (double)msg->m_price/1024;
			CString sym = msg->m_Symbol;
			fill.xsize= msg->m_LastTradeVolume;
			fill.exchange = ExchangeName((int)msg->m_ExecutionExchange);
			fill.xdate = date;
			fill.xtime = time;
			fill.xsec = sec;
			tl->SrvGotFill(fill);
        }
        break;

        case M_POOL_EXECUTION:
//Notification about a new execution.
        if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_EXECUTION)
        {
			// TRADELINK
            MsgPoolExecution* msg = (MsgPoolExecution*)message;//to get the structure, just cast Message* to  MsgPoolExecution* (not used here)
            AIMsgExecution* info = (AIMsgExecution*)additionalInfo;
            Order* order = info->m_order;
			if (!hasFillID(order->GetId())) // don't send same notification twice
			{
				fillids.push_back(order->GetId()); // save the id
				CTime ct(msg->x_Time);
				int xd = (ct.GetYear()*10000)+(ct.GetMonth()*100)+ct.GetDay();
				int xt = (ct.GetHour()*100)+ct.GetMinute();

				TradeLinkServer::TLTrade fill;
				fill.id = order->GetId();
				fill.xsec = ct.GetSecond();
				fill.xtime = xt;
				fill.xdate = xd;
				fill.side = (order->GetSide()=='B');
				fill.comment = order->GetUserDescription();
				fill.symbol = msg->x_Symbol;
				fill.xprice = (double)msg->x_ExecutionPrice/1024;
				fill.xsize = msg->x_NumberOfShares;
				tl->SrvGotFill(fill);
			}
	
		}
		break;

        case M_POOL_UPDATE_ORDER:// Order status is modified
        if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_ORDER)
        {
            AIMsgOrder* info = (AIMsgOrder*)additionalInfo;
            Order* order = info->m_order;
            const Position* position = info->m_position;
			CTime ct(order->GetTimeCreated());
			TradeLinkServer::TLOrder o;
			o.id = order->GetId();
			o.price = order->GetOrderPrice().toDouble();
			o.stop = order->GetStopPrice()->toDouble();
			o.time = (ct.GetHour()*100)+ct.GetMinute();
			o.date = (ct.GetYear()*10000)+(ct.GetMonth()*100)+ct.GetDay();
			o.size = order->GetSize();
			o.comment = order->GetUserDescription();
			o.TIF = TIFName(order->GetTimeInForce());
			tl->SrvGotOrder(o);
		}
		break;
    }
}



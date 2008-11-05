#include "stdafx.h"

#include "TLStock.h"
#include "Messages.h"
#include "SendMsg.h"
#include "TLAnvil.h"
#include "TradeLink.h"
#include "TradeLinkServer.h"
#include "AnvilUtil.h"
using namespace TradeLinkServer;


#ifdef _DEBUG
//#include "Shiny.h"
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






void TLStock::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
    switch(message->GetType())
    {
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
        }
        break;


    }
}




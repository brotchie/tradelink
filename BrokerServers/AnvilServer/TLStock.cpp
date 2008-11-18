#include "stdafx.h"

#include "TLStock.h"
#include "Messages.h"
#include "AVL_TLWM.h"
#include "TradeLink.h"
#include "TradeLibFast.h"
#include "AnvilUtil.h"
using namespace TradeLibFast;


#ifdef _DEBUG

#endif

#pragma warning (disable:4355)


TLStock::TLStock(const char* symbol, TradeLibFast::TLServer_WM* tlinst, bool load):
    m_symbol(symbol),
    m_stockHandle(NULL),
    m_level1(NULL),
    m_level2(NULL),
    m_prints(NULL)
{
	tl = tlinst;
    if(load)
    {
        Load();
    }
}

void TLStock::Clear()
{
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
	tl = NULL;
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
				TradeLibFast::TLTick tick;
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
			tl->SrvGotTick(tick);
        }
        break;


    }
}




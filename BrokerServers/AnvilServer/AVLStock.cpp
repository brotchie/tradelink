#include "stdafx.h"

#include "AVLStock.h"
#include "Messages.h"
#include "AVL_TLWM.h"
#include "TradeLink.h"
#include "TradeLibFast.h"
#include "AnvilUtil.h"
using namespace TradeLibFast;


#ifdef _DEBUG

#endif

#pragma warning (disable:4355)


AVLStock::AVLStock(const char* symbol, TradeLibFast::TLServer_WM* tlinst, bool load, int dep):
    m_symbol(symbol),
    m_stockHandle(NULL),
    m_level1(NULL),
    m_level2(NULL),
    m_prints(NULL),
	bidi(NULL),
	aski(NULL),
	pnti(B_CreatePrintsAndBookExecutionsIterator(NULL, (1 << PS_LAST) - 1, 0, false, NULL, true)),
	depth(dep)
{
	tl = tlinst;
    if(load)
    {
        Load();
    }
	for(unsigned int i = 0; i < MAX_BOOKS; i++)
    {
        booki[i] = 1;//number of book lines integrated in Level2
    }
}

void AVLStock::Clear()
{
	if (isLoaded())
	{
		if (bidi)
		{
			B_DestroyIterator(bidi);
			bidi = NULL;
		}
		if (aski)
		{
			B_DestroyIterator(aski);
			aski = NULL;
		}
		if (pnti)
		{
			B_TransactionIteratorSetStock(pnti, NULL, this);
			B_DestroyIterator(pnti);
			pnti= NULL;
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
	tl = NULL;
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
		// setup book iterators which are used to get top of book and t&s when updates come in
		bidi = B_CreateLevel2AndBookIterator(m_stockHandle, true, false,false, booki, (depth + 1), this);
		aski = B_CreateLevel2AndBookIterator(m_stockHandle, false, false,false, booki, (depth + 1), this);
		B_TransactionIteratorSetStock(pnti, m_stockHandle, this);
		if (isLoaded())
		{
			TradeNotify();
			QuoteNotify();
		}
	}

}

void AVLStock::QuoteNotify()
{
	if (isLoaded())
	{
			time_t now;
			time(&now);
			CTime ct(now);
			TLTick k;
			k.date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();
			k.time = (ct.GetHour()*10000)+(ct.GetMinute()*100) + ct.GetSecond();
			k.sym = CString(m_symbol.c_str());
			// get bid
			B_StartIteration(bidi);
			//const BookEntry* be = B_GetNextBookEntry(bidi);
			const BookEntry* be;
			B_StartIteration(aski);
			//const BookEntry* ae = B_GetNextBookEntry(aski);
			const BookEntry* ae;
			for (int i = 0; i <= depth; i++) //report a tick, depth times
			{
				be = B_GetNextBookEntry(bidi);
				if (be==NULL) return;
				k.bid = be->toDouble();
				k.bs = be->GetSize()/m_stockHandle->GetRoundLot();
				k.be = be->GetMmid();
				// get ask
				//B_StartIteration(aski);
				//const BookEntry* ae = B_GetNextBookEntry(aski);
				ae = B_GetNextBookEntry(aski);
				if (ae==NULL) return;
				k.ask = ae->toDouble();
				k.os = ae->GetSize()/m_stockHandle->GetRoundLot();
				k.oe = ae->GetMmid();
				//set depth
				k.depth = i;
				// send tick
				tl->SrvGotTick(k);
			}
	}

}
void AVLStock::TradeNotify()
{
	if (isLoaded())
	{
			time_t now;
			time(&now);
			CTime ct(now);
			TLTick k;
			k.date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();
			k.time = (ct.GetHour()*10000)+(ct.GetMinute()*100) + ct.GetSecond();
			k.sym = CString(m_symbol.c_str());
			// get trade
			B_StartIteration(pnti);
			const Transaction* t = B_GetNextPrintsEntry(pnti);
			if (t==NULL) return;
			k.trade = t->toDouble();
			k.size = t->GetSize();
			k.ex = t->GetMmid();

			// send tick
			tl->SrvGotTick(k);
	}
}


void AVLStock::Process(const Message* message, Observable* from, const Message* additionalInfo)
{

    switch(message->GetType())
    {
		case M_RESP_REFRESH_SYMBOL:
		case M_ITCH_1_00_NewVisibleOrder:
		case M_ITCH_1_00_VisibleOrderExecution:
		case M_ITCH_1_00_CanceledOrder:
		case M_BOOK_NEW_ORDER:
		case M_BOOK_MODIFY_ORDER:
		case M_BOOK_DELETE_ORDER:
		case M_ITCH_1_00_NewVisibleAttributedOrder:
		case M_ITCH_1_00_VisibleAttributedOrderExecution:
		case M_ITCH_1_00_ATTRIBUTED_CanceledOrder:
		case M_BOOK_NYSE_OPEN_BOOK:
		case M_FLUSH_ALL:
        case M_FLUSH_ALL_OPEN_BOOKS:
        case M_FLUSH_BOOK_FOR_STOCK:
		case M_FLUSH_ATTRIBUTED_BOOK:
		case M_FLUSH_ATTRIBUTED_BOOK_FOR_STOCK:
		case M_NW2_INSIDE_QUOTE:
		case M_NW2_MM_QUOTE:
        case M_LEVEL2_QUOTE:
			if (from==m_prints)
				TradeNotify();
			if ((from==m_level2) || (from==m_level1))
				QuoteNotify();
			break;
        case M_LAST_TRADE_SHORT:
		case M_TAL_LAST_TRADE:
			TradeNotify();
			break;

    }
}




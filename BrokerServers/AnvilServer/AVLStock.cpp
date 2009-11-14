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


AVLStock::AVLStock(const char* symbol, int id, AVL_TLWM* tlinst, bool load, int dep):
    m_symbol(symbol),
    m_stockHandle(NULL),
    m_level1(NULL),
    m_level2(NULL),
    m_prints(NULL),
	bidi(NULL),
	aski(NULL),
	pnti(B_CreatePrintsAndBookExecutionsIterator(NULL, (1 << PS_LAST) - 1, 0, false, NULL, true)),
	depth(dep),
	_symid(id)
{
	_sym = CString(symbol);
	tl = tlinst;

	time_t now;
	time(&now);
	CTime ct(now);
	_date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();

    if(load)
    {
        Load();
    }
	for(unsigned int i = 0; i < MAX_BOOKS; i++)
    {
        booki[i] = dep;//number of book lines integrated in Level2
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
		if (m_account)
		{
			m_account->Remove(this);
			m_account = NULL;
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
	// ignore empty symbols
	if (m_symbol.empty()) return;
	// start loading stock
    m_stockHandle = B_GetStockHandle(m_symbol.c_str());//This will load the stock from the Hammer server. The stock will not be available immediately for monitoring and trading, only after Hammer server sends all the info about the stock.
	// make sure we got something back
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
		bidi = B_CreateLevel2AndBookIterator(m_stockHandle, true, false,false, booki, 0xFFFFFFFF, this);
		aski = B_CreateLevel2AndBookIterator(m_stockHandle, false, false,false, booki, 0xFFFFFFFF, this);
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
			kquote.date = _date;
			uint hour,minute,second;
			B_GetCurrentServerNYTimeTokens(hour, minute, second);
			kquote.time = (hour*10000)+(minute*100) + second;
			kquote.sym = _sym;
			kquote.symid = _symid;
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
				kquote.bid = be->toDouble();
				kquote.bs = be->GetSize()/m_stockHandle->GetRoundLot();
				kquote.be = be->GetMmid();
				// get ask
				//B_StartIteration(aski);
				//const BookEntry* ae = B_GetNextBookEntry(aski);
				ae = B_GetNextBookEntry(aski);
				if (ae==NULL) return;
				kquote.ask = ae->toDouble();
				kquote.os = ae->GetSize()/m_stockHandle->GetRoundLot();
				kquote.oe = ae->GetMmid();
				//set depth
				kquote.depth = i;
				// send tick
				tl->SrvGotTickAsync(kquote);
			}
	}

}
void AVLStock::TradeNotify()
{
	if (isLoaded())
	{
			uint hour,minute,second;
			B_GetCurrentServerNYTimeTokens(hour, minute, second);
			ktrade.date = _date;
			ktrade.time = (hour*10000)+(minute*100) + second;
			ktrade.sym = _sym;
			ktrade.symid = _symid;
			// get trade
			B_StartIteration(pnti);
			const Transaction* t = B_GetNextPrintsEntry(pnti);
			if (t==NULL) return;
			ktrade.trade = t->toDouble();
			ktrade.size = t->GetSize();
			ktrade.ex = t->GetMmid();

			// send tick
			tl->SrvGotTickAsync(ktrade);
	}
}

bool AVLStock::isLoaded() const
{
	const bool nonull = (m_stockHandle != NULL);
	if (!nonull) return false;
	__try
	{
		const bool load = m_stockHandle->isLoaded();
		return load && (m_stockHandle->GetSymbol()==m_symbol); 
	}
	__except (EXCEPTION_EXECUTE_HANDLER)
	{
		return false;
	}
}


void AVLStock::Process(const Message* message, Observable* from, const Message* additionalInfo)
{

    switch(message->GetType())
    {
		case M_RESP_REFRESH_SYMBOL:
		case M_NW2_MM_QUOTE:
        case M_LEVEL2_QUOTE:
		case M_ITCH_1_00_NewVisibleOrder:
		case M_ITCH_1_00_VisibleOrderExecution:
		case M_ITCH_1_00_CanceledOrder:
		case M_BOOK_NEW_ORDER:
		case M_BOOK_MODIFY_ORDER:
		case M_BOOK_DELETE_ORDER:
		case M_ITCH_1_00_HiddenOrderExecution:
		case M_ITCH_1_00_HiddenAttributedOrderExecution:
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

			if (from==m_prints)
				TradeNotify();
			if ((from==m_level2))
				QuoteNotify();
			break;
        case M_LAST_TRADE_SHORT:
		case M_TAL_LAST_TRADE:
			TradeNotify();
			break;
		case M_RESP_REFRESH_SYMBOL_FAILED:
			Clear();
			break;
		case MSGID_CONNECTION_MADE:
			if (from== B_GetMarketReceiver())
			{
				Load();
			}
			break;

    }
}




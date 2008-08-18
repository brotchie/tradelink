#include "stdAfx.h"
#include "ObserverApi.h"
#include "BusinessApi.h"
#include "AVL_TLWM.h"

#include "AVLStock.h"
#include "AnvilUtil.h"

namespace TradeLinkServer
{

	AVL_TLWM::AVL_TLWM(void)
	{
	}

	AVL_TLWM::~AVL_TLWM(void)
	{
	}

	int AVL_TLWM::BrokerName(void)
	{
		return Assent;
	}

	int AVL_TLWM::RegisterStocks(CString clientname)
	{
		int cid = FindClient(clientname);
		vector<CString> my = this->stocks[cid]; // get this clients stocks
		for (size_t i = 0; i<my.size();i++) // subscribe to stocks
		{
			if (hasHammerSub(my[i])) continue; // if we've already subscribed once, skip to next stock
			AVLStock* stk = new AVLStock(my[i],this); // create new stock instance
			stk->Load();
			this->stocksubs.push_back(stk);
		}
		return OK;
	}

	bool AVL_TLWM::hasHammerSub(CString symbol)
	{
		for (size_t i = 0; i<stocksubs.size(); i++) 
			if (symbol.CompareNoCase(stocksubs[i]->GetSymbol().c_str())==0) 
				return true;
		return false;
	}

	int AVL_TLWM::SendOrder(TLOrder order)
	{
		Observable* m_account;
		m_account = B_GetCurrentAccount();
		
		const StockBase* Stock = B_GetStockHandle(order.symbol);

		//convert the arguments
		Order* orderSent;
		char side = order.side ? 'B' : 'S';

		const Money pricem = Money((int)(order.price*1024));
		const Money stopm = Money((int)(order.stop*1024));
		unsigned int tif = TIF_DAY;

		// send the order (next line is from SendOrderDlg.cpp)
		unsigned int error = B_SendOrder(Stock,
				side,
				order.exchange,
				order.size,
				OVM_VISIBLE, //visability mode
				order.size, //visable size
				pricem,//const Money& price,0 for Market
				&stopm,//const Money* stopPrice,
				NULL,//const Money* discrtetionaryPrice,
				tif,
				false,//bool proactive,
				true,//bool principalOrAgency, //principal - true, agency - false
				SUMO_ALG_UNKNOWN,//char superMontageAlgorithm,
				OS_RESIZE,
				//false,//bool delayShortTillUptick,
				DE_DEFAULT, //destination exchange
				&orderSent,
				m_account,
				0,
				false,
				101, order.comment);	

		// cleanup all the stuff we created
		Stock = NULL;
		orderSent = NULL;
		m_account = NULL;
		return error;
	}
}

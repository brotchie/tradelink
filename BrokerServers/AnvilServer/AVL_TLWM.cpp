#include "stdAfx.h"
#include "AVL_TLWM.h"
#include "Util.h"
#include "Messages.h"


namespace TradeLibFast
{
	
	
	AVL_TLWM* AVL_TLWM::instance = NULL;	


	AVL_TLWM::AVL_TLWM(void)
	{
		instance = this;

		// imbalances are off by default
		imbalance = NULL;
		// only one client can subscribe to imbalances
		imbalance_client = -1;

		// add this object as observer to every account,
		// so we can get fill and order notifications

		void* iterator = B_CreateAccountIterator();
		B_StartIteration(iterator);
		Observable* acct;
		while (acct = B_GetNextAccount(iterator)) // loop through every available account
		{
			acct->Add(this); // add this object to account as an observer
			accounts.push_back(acct); // save the account
		}
		B_DestroyIterator(iterator);
	}

	AVL_TLWM::~AVL_TLWM(void)
	{
		// account monitoring stuff
		void* iterator = B_CreateAccountIterator();
		B_StartIteration(iterator);
		Observable* acct;
		while (acct = B_GetNextAccount(iterator)) // loop through every available account
		{
			acct->Remove(this); // add this object to account as an observer
		}
		B_DestroyIterator(iterator);
		accounts.clear();
		ordercache.clear();

		if (imbalance!=NULL)
		{
			imbalance->Remove(this);
			imbalance = NULL;
		}


		// stock stuff, close down hammer subscriptions
		for (size_t i = 0; i<subs.size(); i++)
		{
			if (subs[i]!=NULL)
			{
				delete subs[i];
				subs[i] = NULL;
			}
		}
		subs.clear();
		subsym.clear();

		// if we stored a pointer to ourself, remove it for safety
		instance = NULL;
	}

	int AVL_TLWM::BrokerName(void)
	{
		return Assent;
	}

	int AVL_TLWM::UnknownMessage(int MessageType,CString msg)
	{
		switch (MessageType)
		{
		case ISSHORTABLE :
			{
				const StockBase* s = preload(msg);
				if ((s==NULL)|| !s->isLoaded()) return SYMBOL_NOT_LOADED;
				bool shortable = (s->GetStockAttributes() & STOCKATTR_UPC11830) == 0;
				return shortable ? 1 : 0;
			}
			break;
		case IMBALANCEREQUEST :
			{
				// get an observable for imbalances, 
				imbalance = B_GetMarketImbalanceObservable();
				// make this object watch it
				imbalance->Add(this);
				// set this client to receive imbalances
				imbalance_client = FindClient(msg);
				return OK;
			}
			break;

		case VWAP:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetVwap() ;
				return MoneyToPacked(money) ;
			}
			break;	// GetVwap

		///
		/// GetLastTradeSize
		///
		case LASTTRADESIZE:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				return stock->GetLastTradeSize() ;
			}
			break;	// GetLastTradeSize

		///
		/// GetLastTradePrice	
		///
		case LASTTRADEPRICE:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetLastTradePrice() ;
				return MoneyToPacked(money) ;
			}
			break;	// GetLastTradePrice

		///
		/// GetBid
		///
		case LASTBID:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetBid() ;
				return MoneyToPacked(money) ;
			}
			break;			// GetBid

		///
		/// GetAsk
		///
		case LASTASK:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetAsk() ;
				return MoneyToPacked(money) ;
			}
			break;			// GetAsk

		///
		/// GetBidSize
		///
		case BIDSIZE:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				return stock->GetBidSize() ;
			}
			break;		// GetBidSize

		///
		/// GetAskSize
		///
		case ASKSIZE:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				return stock->GetAskSize() ;
			}
			break;		// GetAskSize

		///
		/// GetDayLow
		///
		case DAYLOW:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetDayLow() ;
				return MoneyToPacked(money) ;
			}
			break;			// GetDayLow

		///
		/// GetDayHigh
		///
		case DAYHIGH:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetDayHigh() ;
				return MoneyToPacked(money) ;
			}
			break;		// GetDayHigh

		///
		/// GetOpenPrice
		///
		case OPENPRICE:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetOpenPrice() ;
				return MoneyToPacked(money) ;
			}
			break;		// GetOpenPrice

		///
		/// GetClosePrice - yesterday
		///
		case CLOSEPRICE:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetClosePrice() ;
				return MoneyToPacked(money) ;
			}
			break;		// GetClosePrice - yesterday

		///
		/// GetLRP - both sides
	
		case LRPBUY:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetLRP(true);	// not sure if this is correct.
				return MoneyToPacked(money) ;
			}
			break;			// GetLRP - both sides

		case LRPSELL:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetLRP(false);	// not sure if this is correct.
				return MoneyToPacked(money) ;
			}
			break;			// GetLRP - both sides

		case AMEXLASTTRADE:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetAmexLastTrade() ;
				return MoneyToPacked(money) ;
			}
			break;			// GetAmexLastTrade

		case NASDAQLASTTRADE:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetNasdaqLastTrade() ;
				return MoneyToPacked(money) ;
			}
			break;			// GetNasdaqLastTrade
		case NYSEBID:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetNyseBid() ;
				return MoneyToPacked(money) ;

			}
			break;				// GetNyseBid
		case NYSEASK:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetNyseAsk() ;
				return MoneyToPacked(money) ;
			}
			break;				// GetNyseAsk
		case NYSEDAYHIGH:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetNyseDayHigh() ;
				return MoneyToPacked(money) ;
			}
			break;			// GetNyseDayHigh
		case NYSEDAYLOW:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetNyseDayLow() ;
				return MoneyToPacked(money) ;
			}
			break;			// GetNyseDayLow
		case NYSELASTTRADE:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetNyseLastTrade() ;
				return MoneyToPacked(money) ;
			}
			break;			// GetNyseLastTrade
		case NASDAQIMBALANCE:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				return stock->GetNasdaqImbalance() ;
			}
			break;			// GetNasdaqImbalance
		case NASDAQPREVIOUSIMBALANCE:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				return stock->GetNasdaqPreviousImbalance() ;
			}
			break;		// GetNasdaqPreviousImbalance
		case NYSEIMBALACE:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				return stock->GetNyseImbalance() ;
			}
			break;			// GetNyseImbalance
		case NYSEPREVIOUSIMBALANCE:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				return stock->GetNysePreviousImbalance() ;
			}
			break;		// GetNysePreviousImbalance
		}	// switch
		return UNKNOWNMSG;
	}


	bool AVL_TLWM::hasHammerSub(CString symbol)
	{
		for (uint i = 0; i<subsym.size(); i++) 
			if (symbol==subsym[i]) 
				return true;
		return false;
	}
	const StockBase* AVL_TLWM::preload(CString symbol)
	{
		for (uint i = 0; i<subs.size(); i++)
		{
			if (!isIndex(subsym[i]) && (subs[i]!=NULL) && (subsym[i]==symbol))
			{
				AVLStock* s = (AVLStock*)subs[i];
				CString ts = CString(s->GetSymbol().c_str());
				if ((s==NULL) || !s->isLoaded() || (symbol!=ts)) break;
				return s->GetStockHandle();
			}
		}
		return B_GetStockHandle(symbol);
	}

	int AVL_TLWM::SendOrder(TLOrder o) 
	{
		const StockBase* Stock = preload(o.symbol);

		Observable* m_account;
		// if order id is set and not-unique, reject order
		if ((o.id!=0) && (!IdIsUnique(o.id)))
			return DUPLICATE_ORDERID;

		// get account for this order
		if (o.account=="")
			m_account = B_GetCurrentAccount();
		else 
			m_account = B_GetAccount(o.account.GetBuffer());
		


		//convert the arguments
		Order* orderSent;
		char side = (o.side) ? 'B' : 'S';
		const Money pricem = Money((int)(o.price*1024));
		const Money stopm = Money((int)(o.stop*1024));
		const Money trailm = Money((int)(o.trail*1024));
		unsigned int mytif = TIFId(o.TIF);

		if ((Stock==NULL) || (!Stock->isLoaded()))
			return UNKNOWNSYM;

		uint error = 0;

		// anvil has seperate call for trailing stop orders
		if (!o.isTrail())
		{

		// send the order (next line is from SendOrderDlg.cpp)
		error = B_SendOrder(Stock,
				side,
				o.exchange,
				o.size,
				OVM_VISIBLE, //visability mode
				o.size, //visable size
				pricem,//const Money& price,0 for Market
				&stopm,//const Money* stopPrice,
				NULL,//const Money* discrtetionaryPrice,
				mytif,
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
				101, o.comment);	
		}
		else 
		{
			Observable* stopOrder = B_SendSmartStopOrder(Stock,
				side,
				o.size,
				&pricem,//const Money* priceOffset,//NULL for Stop Market
				trailm,//trail by this amount
				true, // price to decimal places
				true,//bool ecnsOnlyBeforeAfterMarket,
				false,//bool mmsBasedForNyse,
				TIF_DAY,//unsigned int stopTimeInForce,
				0,//unsigned int timeInForceAfterStopReached,
				"ISLD", //post quote dest
				NULL,//const char* redirection,
				false,//bool proactive,
				true,//bool principalOrAgency, //principal - true, agency - false
				SUMO_ALG_UNKNOWN,//char superMontageAlgorithm,
				OS_RESIZE,
				DE_DEFAULT,//unsigned int destinationExchange,
				TT_PRICE,//StopTriggerType triggerType,
				true,
				0,
				o.comment,
				NULL,//const char* regionalProactiveDestination,
				STPT_ALL,
				Money(0, 200),
				false,
				&orderSent,
				m_account);
			if(!stopOrder)
			{
				error = SO_INCORRECT_PRICE;
			}
		}
		if (error==0)
			saveOrder(orderSent,o.id); // save order if it was accepted
		return error;
	}


	bool AVL_TLWM::IdIsUnique(uint id)
	{
		for (uint i = 0; i<orderids.size(); i++)
			if (orderids[i]==id) 
				return false;
		return true;
	}
	uint AVL_TLWM::fetchOrderId(Order* order)
	{
		for (uint i = 0; i<ordercache.size(); i++)
			if (ordercache[i]==order) 
				return orderids[i];
		return 0;
	}

	bool AVL_TLWM::saveOrder(Order* o,uint id)
	{
		if (id==0) // if id is zero, we auto-assign the id
		{
			vector<int> now;
			id = GetTickCount();
			while (!IdIsUnique(id))
			{
				if (id<2) id = 4000000000;
				id--;
			}
		}
		for (unsigned int i = 0; i<ordercache.size(); i++)
			if (ordercache[i]==o) 
				return false; // already had this order, fail
		ordercache.push_back(o); // save the order
		orderids.push_back(id); // save the id
		return true; // didn't find order so we added it and returned index
	}

	void AVL_TLWM::Process(const Message* message, Observable* from, const Message* additionalInfo)
	{
		switch(message->GetType())
		{
			case M_POOL_EXECUTION:
			if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_EXECUTION)
			{
				 MsgPoolExecution* msg = (MsgPoolExecution*)message;//to get the structure, just cast Message* to  MsgPoolExecution* (not used here)

				//This is additional info structure prepared by Business.dll. 
				//It contains updated objects Position, Order Execution (look in BusinessApi.h).
				//You can access objects' fields, but it is not recommended to change them (The fields are protected and you should not play any tricks to modify the fields. It will cause unpredictable results)
				AIMsgExecution* info = (AIMsgExecution*)additionalInfo;
				Order* order = info->m_order;
				const Execution* exec = info->m_execution;
				if ((order==NULL) || (exec==NULL)) return; // don't process null orders

				unsigned int thisid = this->fetchOrderId(order);
				CString ac = CString(B_GetAccountName(order->GetAccount()));

				// build the serialized trade object
				CTime ct(msg->x_Time);
				int xd = (ct.GetYear()*10000)+(ct.GetMonth()*100)+ct.GetDay();
				int xt = (ct.GetHour()*100)+ct.GetMinute();
				TradeLibFast::TLTrade fill;
				fill.id = thisid;
				fill.xsec = ct.GetSecond();
				fill.xtime = xt;
				fill.xdate = xd;
				fill.side = (order->GetSide()=='B');
				fill.comment = CString(order->GetUserDescription());
				fill.symbol = CString(msg->x_Symbol);
				fill.xprice = (double)msg->x_ExecutionPrice/1024;
				fill.xsize= msg->x_NumberOfShares;
				fill.exchange = CString(ExchangeName((long)msg->x_executionId));
				fill.account = CString(B_GetAccountName(order->GetAccount()));
				SrvGotFill(fill);

			} // has additional info end
			break;
			case M_POOL_ASSIGN_ORDER_ID://Original order sent has a unigue generated id. The server sends this message to notify you that the order was assigned a new id different from the original. Both ids are part of this notification structure. This message can come 1 or 2 times.
			case M_POOL_UPDATE_ORDER:// Order status is modified
			if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_ORDER)
			{

				AIMsgOrder* info = (AIMsgOrder*)additionalInfo;
				Order* order = info->m_order;

				if ((order==NULL) || (info==NULL)) return; // don't process null orders

				if (order->isDead()) return; // don't notify on dead orders

				// try to save this order
				bool isnew = saveOrder(order,0);
				// if it fails, we already have it so get the id
				// if it succeeds, we should be able to get the id anyways
				uint id = fetchOrderId(order);

				CTime ct = CTime::GetCurrentTime();
				TLOrder o;
				o.id = id;
				o.price = order->isMarketOrder() ? 0: order->GetOrderPrice().toDouble();
				o.sec = ct.GetSecond();
				o.stop = order->GetStopPrice()->toDouble();
				o.time = (ct.GetHour()*100)+ct.GetMinute();
				o.date = (ct.GetYear()*10000)+(ct.GetMonth()*100)+ct.GetDay();
				o.size = order->GetSize();
				o.side = order->GetSide()=='B';
				o.comment = order->GetUserDescription();
				o.TIF = TIFName(order->GetTimeInForce());
				o.account = CString(B_GetAccountName(order->GetAccount()));
				o.symbol = CString(order->GetSymbol());
				SrvGotOrder(o);
				break;
			} // has addt info / caseend
			case M_REQ_CANCEL_ORDER:
			{
				AIMsgOrder* info = (AIMsgOrder*)additionalInfo;
				Order* order = info->m_order;
				unsigned int anvilid = order->GetId();
				unsigned int id = fetchOrderId(order);
				SrvGotCancel(id);
				break;

			}
			break;
			case M_MARKET_IMBALANCE:
			case M_NEW_MARKET_IMBALANCE:
				{
					if ((imbalance_client == -1)|| (client[imbalance_client]=="")) return;
					if (additionalInfo && (additionalInfo->GetType()==M_AI_STOCK_MOVEMENT))
					{
						const StockMovement* sm = ((MsgStockMovement*)additionalInfo)->m_stock;
						TLImbalance nyi;
						nyi.Symbol = CString(sm->GetSymbol());
						nyi.ThisImbalance = sm->GetNyseImbalance();
						nyi.PrevImbalance = sm->GetNysePreviousImbalance();
						nyi.Ex = CString("NYSE");
						nyi.ThisTime = sm->GetNyseImbalanceTime();
						nyi.PrevTime = sm->GetNysePreviousImbalanceTime();
						if (nyi.hasImbalance()||nyi.hadImbalance())
							TLSend(IMBALANCERESPONSE,TLImbalance::Serialize(nyi),client[imbalance_client]);
						TLImbalance qyi;
						qyi.Symbol = CString(sm->GetSymbol());
						qyi.ThisImbalance = sm->GetNasdaqImbalance();
						qyi.PrevImbalance = sm->GetNasdaqPreviousImbalance();
						qyi.Ex = CString("NASDAQ");
						qyi.ThisTime = sm->GetNasdaqImbalanceTime();
						qyi.PrevTime = sm->GetNasdaqPreviousImbalanceTime();
						if (qyi.hasImbalance()||qyi.hadImbalance())
							TLSend(IMBALANCERESPONSE,TLImbalance::Serialize(nyi),client[imbalance_client]);
					}

					break;
				}
		} // switchend
	}

	int AVL_TLWM::AnvilId(unsigned int TLOrderId)
	{
		for (uint i = 0; i<orderids.size(); i++)
			if (orderids[i]==TLOrderId)
				return ordercache[i]->GetId();
		return -1;
	}

	std::vector<int> AVL_TLWM::GetFeatures()
	{
		std::vector<int> f;
		f.push_back(ISSHORTABLE);
		f.push_back(BROKERNAME);
		f.push_back(ACCOUNTREQUEST);
		f.push_back(ACCOUNTRESPONSE);
		f.push_back(HEARTBEAT);
		f.push_back(SENDORDER);
		f.push_back(REGISTERCLIENT);
		f.push_back(REGISTERSTOCK);
		f.push_back(CLEARCLIENT);
		f.push_back(CLEARSTOCKS);
		f.push_back(ORDERCANCELREQUEST);
		f.push_back(ORDERCANCELRESPONSE);
		f.push_back(FEATUREREQUEST);
		f.push_back(FEATURERESPONSE);
		f.push_back(TICKNOTIFY);
		f.push_back(EXECUTENOTIFY);
		f.push_back(ORDERNOTIFY);
		f.push_back(IMBALANCEREQUEST);
		f.push_back(IMBALANCERESPONSE);

		// added 2009.01.17
		f.push_back(VWAP);	// GetVwap
		f.push_back(LASTTRADESIZE);	// GetLastTradeSize
		f.push_back(LASTTRADEPRICE);	// GetLastTradePrice
		f.push_back(LASTBID);			// GetBid
		f.push_back(LASTASK);			// GetAsk
		f.push_back(BIDSIZE);		// GetBidSize
		f.push_back(ASKSIZE);		// GetAskSize
		f.push_back(DAYLOW);			// GetDayLow
		f.push_back(DAYHIGH);		// GetDayHigh
		f.push_back(OPENPRICE);		// GetOpenPrice
		f.push_back(CLOSEPRICE);		// GetClosePrice - yesterday
		f.push_back(LRPBUY);			// GetLRP - both sides
		f.push_back(LRPSELL);
		return f;
	}

	void AVL_TLWM::CancelRequest(long order)
	{
		// get current anvil id from tradelink id
        unsigned int orderId = AnvilId(order); 
        void* iterator = B_CreateAccountIterator();
        B_StartIteration(iterator);
        Observable* acct;

		bool continue_search_flag = true ;
        // loop through every available account, cancel any matching orders
		// stop when order is found
        while ((acct = B_GetNextAccount(iterator)) && (continue_search_flag == true ))
        {
                Order* order = B_FindOrder(orderId,acct);
                if(order)
                {
                        order->Cancel();
						continue_search_flag = false ;
                }
        }
        B_DestroyIterator(iterator);
	}

	int AVL_TLWM::AccountResponse(CString client)
	{
		void* iterator = B_CreateAccountIterator();
		B_StartIteration(iterator);
		Observable* a;
		std::vector<CString> accts;
		while (a = B_GetNextAccount(iterator)) // loop through every available account
		{
			PTCHAR username = (PTCHAR)B_GetAccountName(a);
			CString un(username);
			accts.push_back(un);
		}
		B_DestroyIterator(iterator);
		CString msg = ::gjoin(accts,CString(","));
		TLSend(ACCOUNTRESPONSE,msg,client);
		return OK;
	}

	int AVL_TLWM::PositionResponse(CString account, CString client)
	{
		if (account=="") return BAD_PARAMETERS;
		Observable* m_account = B_GetAccount(account);
		void* iterator = B_CreatePositionIterator(POSITION_FLAT|POSITION_LONG|POSITION_SHORT, (1 << ST_LAST) - 1,m_account);
		B_StartIteration(iterator);
		const Position* pos;
		int count = 0;
		while(pos = B_GetNextPosition(iterator))
		{
			TradeLibFast::TLPosition p;
			p.AvgPrice = pos->GetAverageExecPrice().GetMoneyValueForServer()/(double)1024;
			p.ClosedPL = pos->GetClosedPnl().GetMoneyValueForServer()/(double)1024;
			p.Size = pos->GetSize();
			p.Symbol = CString(pos->GetSymbol());
			CString msg = p.Serialize();
			TLSend(POSITIONRESPONSE,msg,client);
			count++;
		}
		B_DestroyIterator(iterator);
		m_account = NULL;
		pos = NULL;
		return count;
	}

	int AVL_TLWM::SubIdx(CString symbol)
	{
		for (size_t i = 0; i<subsym.size(); i++) 
			if (subsym[i]==symbol)
				return (int)i;
		return -1;
	}



	bool AVL_TLWM::isIndex(CString sym)
	{
		int slashi = sym.FindOneOf("/");
		int doli = sym.FindOneOf("$");
		int poundi = sym.FindOneOf("#");
		return (slashi!=-1)||(doli!=-1)||(poundi!=-1);
	}

	void AVL_TLWM::RemoveSub(CString stock)
	{
		if (hasHammerSub(stock))
		{
			size_t i = SubIdx(stock);
			if (subs[i]!=NULL)
			{
				subs[i]->Clear();
				delete subs[i];
				subsym[i] = "";
				subs[i]= NULL;
			}
		}
	}

	void AVL_TLWM::RemoveUnused()
	{
		for (uint i = 0; i<stocks.size(); i++)
			for (uint j = 0; j<stocks[i].size(); j++)
				if (!needStock(stocks[i][j]))
					RemoveSub(stocks[i][j]);
		B_UnsubscribeUnusedStocks();

	}

	int AVL_TLWM::RegisterStocks(CString clientname)
	{ 
		TLServer_WM::RegisterStocks(clientname);
		unsigned int cid = FindClient(clientname);
		if (cid==-1) return CLIENTNOTREGISTERED; //client not registered
		clientstocklist my = stocks[cid]; // get stocks
		for (size_t i = 0; i<my.size();i++) // subscribe to stocks
		{
			if (hasHammerSub(my[i])) continue; // if we've already subscribed once, skip to next stock
			Observer* sec;
			if (isIndex(my[i]))
				sec = new AVLIndex(my[i],this);
			else
			{
				AVLStock *stk = new AVLStock(my[i],this); // create new stock instance
				sec = stk;
			}
			subs.push_back(sec);
			subsym.push_back(my[i]);
		}
		stocks[cid] = my; // index the array by the client's id
		HeartBeat(clientname); // update the heartbeat
		return 0;
	}

	int AVL_TLWM::ClearClient(CString client) 
	{
		// call base clear
		TLServer_WM::ClearClient(client);
		// remove any subscriptions this stock has that aren't used by others
		RemoveUnused(); 
		return OK;
	}

	int AVL_TLWM::ClearStocks(CString client)
	{
		// remove record of stocks
		TLServer_WM::ClearStocks(client);
		// remove anvil subs
		RemoveUnused();
		return OK;
	}

}


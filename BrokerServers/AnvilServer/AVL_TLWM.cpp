#include "stdAfx.h"
#include "AVL_TLWM.h"
#include "Util.h"
#include "Messages.h"
#include "MessageIds.h"
#include "windows.h"
#include "mmsystem.h"



	
	
	AVL_TLWM* AVL_TLWM::instance = NULL;	

	const char* CONFIGFILE = "AnvilServer.Config.txt";
	void AVL_TLWM::ReadConfig()
	{
		std::ifstream file;
		file.open(CONFIGFILE);
		int sessionid = 0;
		const int ss = 200;
		char skip[ss];
		const int ds = 20;
		char data[ds];
		file.getline(skip,ss);
		file.getline(data,ds);
		_proactive = atoi(data) == 1;
		file.close();
	}

	AVL_TLWM::AVL_TLWM(void)
	{
		instance = this;

		PROGRAM = "AnvilServer";

		// remove canceled orders
		B_KeepCancelledOrders(false);

		// imbalances are off by default
		imbalance = NULL;

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

		depth = 0;

		_startimb = false;
		_readimb = 0;
		_writeimb = 0;
		_imbflip = false;

		for (uint i = 0; i<MAXTICKS; i++)
		{
			TLImbalance imb;
			_imbcache.push_back(imb);
		}
		ReadConfig();


	}



	AVL_TLWM::~AVL_TLWM(void)
	{
		// remove account observables
		void* iterator = B_CreateAccountIterator();
		B_StartIteration(iterator);
		Observable* acct;
		while (acct = B_GetNextAccount(iterator)) 
		{
			acct->Remove(this); 
		}
		B_DestroyIterator(iterator);
		// remove all account observables
		for (uint i = 0; i<accounts.size(); i++)
			accounts[i] = NULL;
		accounts.clear();
		// remove all pointers to orders
		for (uint i = 0; i<ordercache.size(); i++)
			ordercache[i] = NULL;
		// clear cache
		ordercache.clear();

		if (imbalance!=NULL)
		{
			imbalance->Remove(this);
			imbalance = NULL;
		}
		_imbcache.clear();

		// stock stuff, close down hammer subscriptions
		for (size_t i = 0; i<subs.size(); i++)
		{
			if (subs[i]!=NULL)
			{
				subs[i]->Clear();
				subs[i] = NULL;
			}
		}
		subs.clear();
		subsym.clear();


	}








	int AVL_TLWM::BrokerName(void)
	{
		return Assent;
	}

	int AVL_TLWM::UnknownMessage(int MessageType,CString msg)
	{
		switch (MessageType)
		{
		case ORDERNOTIFYREQUEST :
			{
				std::vector<CString> r;
				gsplit(msg,CString("+"),r);
				if (r.size()<2)
				{
					D(CString("Must send ORDERNOTIFY WITH: 'account+client' msg."));
					break;
				}
				Observable* m_account = B_GetAccount(r[0]);
				if (m_account)
				{
					// get all the orders available
					void* iterator = B_CreateOrderIterator(OS_PENDINGLONG|OS_PENDINGSHORT, (1 << ST_LAST) - 1, m_account);
					B_StartIteration(iterator);
					Order* order;
					while(order = B_GetNextOrder(iterator))
					{
						TLOrder o = ProcessOrder(order);
						if (o.isValid())
						{
							TLSend(ORDERNOTIFY,o.Serialize(),r[1]);
						}
					}
					B_DestroyIterator(iterator);
				}
				else
				{
					CString m;
					m.Format("Invalid account: %s",r[0]);
					D(m);
				}
			}
			break;
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
				// get client id
				int id = FindClient(msg);
				// ignore invalid clients
				if (id<0) return OK;
				// only request imbalances once on broker side
				if (imbalance==NULL)
				{
					// get an observable for imbalances, 
					imbalance = B_GetMarketImbalanceObservable();
					// make this object watch it
					imbalance->Add(this);
				}
				// set this client to receive imbalances
				imbalance_clients.push_back(id);
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
		/// GetIntraDayHigh
		///
		case INTRADAYHIGH:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetIntradayHigh() ;
				return MoneyToPacked(money) ;
			}
			break;		
		///
		/// GetIntraDayLow
		///
		case INTRADAYLOW:
			{
				const StockBase* stock = preload(msg) ;	// Returns from B_GetStockHandle
				if ((stock == NULL)|| !stock->isLoaded()) return -1 ;
				Money money ;
				money = stock->GetIntradayLow() ;
				return MoneyToPacked(money) ;
			}
			break;			

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
		return UNKNOWN_MESSAGE;
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

	inline int rndup(int val)
	{
		double t = val/(double)10;
		int tw = (int)t;
		double tf = t-tw;
		if (tf>0.5)
			tw++;
		tw *=10;
		return tw;
	} 

	Money AVL_TLWM::Double2Money(double val)
	{
		int vw = (int)val;
		int vfp = (int)((val-vw)*1000);
		int vf = rndup(vfp);
		return Money(vw,vf);
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
		// ensure we have an exchange
		if (o.exchange=="")
			o.exchange = o.symbol.GetLength()>3 ? "NSDQ" : "NYSE";
		


		//convert the arguments
		Order* orderSent;
		char side = (o.side) ? 'B' : 'S';
		const Money pricem = Double2Money(o.price);
		const Money stopm = Double2Money(o.stop);
		const Money trailm = Double2Money(o.trail);
		unsigned int mytif = TIFId(o.TIF);

		if (Stock==NULL)
			return UNKNOWN_SYMBOL;
		if (!Stock->isLoaded())
			return SYMBOL_NOT_LOADED;

		uint error = 0;
		uint size = abs(o.size);

		// anvil has seperate call for trailing stop orders
		if (!o.isTrail() && (o.isLimit() || o.isMarket()))
		{

		// send the order (next line is from SendOrderDlg.cpp)
		error = B_SendOrder(Stock,
				side,
				o.exchange,
				size,
				OVM_VISIBLE, //visability mode
				size, //visable size
				pricem,//const Money& price,0 for Market
				&stopm,//const Money* stopPrice,
				NULL,//const Money* discrtetionaryPrice,
				mytif,
				_proactive,//bool proactive,
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
		else if (o.isStop())
		{
			Money insideMarketQuote;
			if (B_GetSafeInsidePrice(Stock,o.side,!o.side,false,insideMarketQuote))
			{
				Money stopPriceOffset;
				if (o.side) stopPriceOffset = stopm - insideMarketQuote;
				else stopPriceOffset = insideMarketQuote - stopm;
				Observable* stopOrder = B_SendSmartStopOrder(Stock,
					side,
					size,
					NULL,//const Money* priceOffset,//NULL for Stop Market
					stopPriceOffset,
					true, // price 2 decimal places
					true,//bool ecnsOnlyBeforeAfterMarket,
					false,//bool mmsBasedForNyse,
					TIF_DAY,//unsigned int stopTimeInForce,
					TIFId(o.TIF),//unsigned int timeInForceAfterStopReached,
					"ISLD",
					NULL,//const char* redirection,
					false,//bool proactive,
					true,//bool principalOrAgency, //principal - true, agency - false
					SUMO_ALG_UNKNOWN,//char superMontageAlgorithm,
					OS_RESIZE,
		//            false,//bool delayShortTillUptick,
					DE_DEFAULT,//unsigned int destinationExchange,
					TT_PRICE,//StopTriggerType triggerType,
					false, // is trailing
					0,
					o.comment,
					NULL,//const char* regionalProactiveDestination,
					STPT_ALL,
					Money(0, 200),
					false,
					&orderSent,
					m_account);
				if(!stopOrder)
					error = SO_INCORRECT_PRICE;
			}
			else
				error = SO_INCORRECT_PRICE;

		}
		else if (o.isTrail())
		{
			Observable* stopOrder = B_SendSmartStopOrder(Stock,
				side,
				size,
				NULL,//const Money* priceOffset,//NULL for Stop Market
				trailm,//trail by this amount
				true, // price 2 decimal places
				true,//bool ecnsOnlyBeforeAfterMarket,
				false,//bool mmsBasedForNyse,
				TIF_DAY,//unsigned int stopTimeInForce,
				TIFId(o.TIF),//unsigned int timeInForceAfterStopReached,
				"ISLD", //post quote dest
				NULL,//const char* redirection,
				false,//bool proactive,
				true,//bool principalOrAgency, //principal - true, agency - false
				SUMO_ALG_UNKNOWN,//char superMontageAlgorithm,
				OS_RESIZE,
				DE_DEFAULT,//unsigned int destinationExchange,
				TT_PRICE,//StopTriggerType triggerType,
				true, // is trailing
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
		// make sure order sent is valid order
		if (orderSent==NULL)
		{
			if (error==0) // if no error, return empty order
				error = EMPTY_ORDER;
		}
		else // if order is good, save it
		{
			// save order if it was accepted
			if (error==0)
			{
				if (saveOrder(orderSent,o.id,true))
					error = OK;
				else 
					error = UNKNOWN_ERROR;
			}
		}
		// return result
		return error;
	}


	bool AVL_TLWM::IdIsUnique(int64 id)
	{
		for (uint i = 0; i<orderids.size(); i++)
			if (orderids[i]==id) 
				return false;
		return true;
	}
	int64 AVL_TLWM::fetchOrderId(Order* order)
	{
		if (order==NULL) return ORDER_NOT_FOUND;
		for (uint i = 0; i<ordercache.size(); i++)
			if (ordercache[i]==order) 
				return orderids[i];
		return 0;
	}

	int64 AVL_TLWM::fetchOrderIdAndRemove(Order* order)
	{
		if (order==NULL) return ORDER_NOT_FOUND;
		for (uint i = 0; i<ordercache.size(); i++)
			if (ordercache[i]==order) 
			{
				ordercache[i] = NULL;
				int64 id = orderids[i];
				orderids[i] = 0;
				return id;
			}
		return 0;
	}

	bool AVL_TLWM::saveOrder(Order* o, int64 id) { return saveOrder(o,id,false); }
	bool AVL_TLWM::saveOrder(Order* o,int64 id, bool allowduplicates)
	{
		// fail if invalid order
		if (o==NULL) return false;
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
			if (!allowduplicates && (ordercache[i]==o))
			{
				// duplicate order
				return false;
			}
		// save the order
		ordercache.push_back(o);
		// save the id
		orderids.push_back(id); 
		// we added order and it's id
		return true; 
	}

	vector<uint> sentids;
	bool sent(uint id)
	{
		for (int i = 0; i<(int)sentids.size();i++)
			if (sentids[i]==id) return true;
		return false;
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
				uint xid = info->m_execution->GetUniqueExecutionId();
				if (sent(xid)) return; // don't notify twice on same execution
				else sentids.push_back(xid);

				int64 thisid = this->fetchOrderId(order);
				CString ac = CString(B_GetAccountName(order->GetAccount()));

				// build the serialized trade object
				CTime ct(msg->x_Time);
				int xd = (ct.GetYear()*10000)+(ct.GetMonth()*100)+ct.GetDay();
				int xt = (ct.GetHour()*10000)+(ct.GetMinute()*100)+ct.GetSecond();
				TradeLibFast::TLTrade fill;
				fill.id = thisid;
				fill.xtime = xt;
				fill.xdate = xd;
				fill.side = (order->GetSide()=='B');
				fill.comment = CString(order->GetUserDescription());
				fill.symbol = CString(msg->x_Symbol);
				fill.xprice = (double)msg->x_ExecutionPrice/1024;
				fill.xsize= msg->x_NumberOfShares;
				fill.exchange = CString(DestExchangeName((long)msg->x_executionId));
				fill.account = CString(B_GetAccountName(order->GetAccount()));
				SrvGotFill(fill);

			} // has additional info end
			break;
			case M_SMARTORDER_ADD: 
			case M_POOL_ASSIGN_ORDER_ID://Original order sent has a unigue generated id. The server sends this message to notify you that the order was assigned a new id different from the original. Both ids are part of this notification structure. This message can come 1 or 2 times.
			case M_POOL_UPDATE_ORDER:// Order status is modified
			{
				Order* order = NULL;
				// see if it's a smart order
				if (message->GetType()==M_SMARTORDER_ADD)
				{
					MsgOrderChange* info = (MsgOrderChange*)message;
					if (info!=NULL)
						order = info->m_order;
				}
				// otherwise it's a normal order
				else 
				{
					// make sure normal order has data we need
					if ((additionalInfo==NULL) || (additionalInfo->GetType()!= M_AI_ORDER))
						return;
					// get the data
					AIMsgOrder* info = (AIMsgOrder*)additionalInfo;
					if (info!=NULL)
						order = info->m_order;
				}

				TLOrder o = ProcessOrder(order);
				SrvGotOrder(o);
				
			}
				break;
			case M_ORDER_DELETED: // for regular cancels
			case M_SMARTORDER_REMOVE: // for smart cancels
				{
					MsgOrderChange* info = (MsgOrderChange*)message;
					if (info->m_order!=NULL)
					{
						Order* order = info->m_order;
						int64 id = fetchOrderIdAndRemove(order);
						if (id>0)
							SrvGotCancel(id);

					}

				}
				break;
			case M_MS_NYSE_IMBALANCE_OPENING:
				{
					if (imbalance_clients.size()==0) return;
					const StockMovement* sm = ((MsgStockMovement*)additionalInfo)->m_stock;
					const StockBase* stk = (StockBase*)sm;
					if (from==B_GetLevel1(stk))
					{
						TLImbalance imb;
						if ((stk!=NULL) && stk->isLoaded()) 
						{
							imb.InfoImbalance = stk->GetNyseInformationalImbalance();
							uint exid = (uint)stk->GetStockExchange();
							imb.Ex = SymExchangeName(exid);
						}
						imb.Symbol = CString(sm->GetSymbol());
						imb.ThisImbalance = sm->GetNyseImbalance();
						imb.PrevImbalance = sm->GetNysePreviousImbalance();
						// don't send empty imbalances
						if ((imb.InfoImbalance==0) && (imb.ThisImbalance==0))
							return;
						imb.ThisTime = sm->GetNyseImbalanceTime();
						imb.PrevTime = sm->GetNysePreviousImbalanceTime();
						SrvGotImbAsync(imb);

					}
				}
				break;
			case M_MS_NYSE_IMBALANCE_CLOSING: 
			//case M_MS_NYSE_IMBALANCE_NONE:
				{
					if (imbalance_clients.size()==0) return;
					if (additionalInfo && (additionalInfo->GetType()==M_AI_STOCK_MOVEMENT))
					{
						const StockMovement* sm = ((MsgStockMovement*)additionalInfo)->m_stock;
						const StockBase* stk = (StockBase*)sm;
						TLImbalance imb;
						if ((stk!=NULL) && stk->isLoaded()) 
						{
							imb.InfoImbalance = stk->GetNyseInformationalImbalance();
							uint exid = (uint)stk->GetStockExchange();
							imb.Ex = SymExchangeName(exid);
						}
						imb.Symbol = CString(sm->GetSymbol());
						imb.ThisImbalance = sm->GetNyseImbalance();
						imb.PrevImbalance = sm->GetNysePreviousImbalance();
						// don't send empty imbalances
						if ((imb.InfoImbalance==0) && (imb.ThisImbalance==0))
							return;
						imb.ThisTime = sm->GetNyseImbalanceTime();
						imb.PrevTime = sm->GetNysePreviousImbalanceTime();
						SrvGotImbAsync(imb);

					}

					break;
				}
			case M_NEW_MARKET_IMBALANCE:
				{
					if (imbalance_clients.size() == 0) return;
					if (additionalInfo && (additionalInfo->GetType()==M_AI_STOCK_MOVEMENT))
					{
						const StockMovement* sm = ((MsgStockMovement*)additionalInfo)->m_stock;
						const StockBase* stk = (StockBase*)sm;
						TLImbalance imb;
						imb.Symbol = CString(sm->GetSymbol());
						if ((stk!=NULL) && stk->isLoaded()) 
						{
							imb.InfoImbalance = stk->GetNyseInformationalImbalance();
							uint exid = (uint)stk->GetStockExchange();
							imb.Ex = SymExchangeName(exid);
						}

						imb.ThisImbalance = sm->GetNasdaqImbalance();
						imb.PrevImbalance = sm->GetNasdaqPreviousImbalance();
						if (imb.hasImbalance() || imb.hadImbalance()) 
						{
							imb.Ex = CString("NASDAQ");
							imb.ThisTime = sm->GetNasdaqImbalanceTime();
							imb.PrevTime = sm->GetNasdaqPreviousImbalanceTime();
							SrvGotImbAsync(imb);
						}
					}

				}
				break;
			case MSGID_CONNECTION_LOST:
				{
					B_IsMarketSummaryPopulationDone();
				}
				break;
			case MS_RESP_SYMBOL_SORTABLE_POPULATION_DONE:
				{
				}
				break;
		} // switchend
	}

		UINT __cdecl DoReadImbThread(LPVOID param)
	{
		// we need a queue object
		AVL_TLWM* tl = (AVL_TLWM*)param;
		// ensure it's present
		if (tl==NULL)
		{
			return OK;
		}

		// process until quick req
		while (tl->_go)
		{
			// process ticks in queue
			while (tl->_go && (tl->_readimb < tl->_imbcache.size()))
			{
				// if we're done reading, quit trying
				if ((tl->_readimb>=tl->_writeimb) && !tl->_imbflip)
					break;
				// read next tick from cache
				TLImbalance imb;
				imb = tl->_imbcache[tl->_readimb++];
				// send it
				tl->SrvGotImbalance(imb);
				// if we hit end of cache buffer, ring back around to start
				if (tl->_readimb>=tl->_imbcache.size())
				{
					tl->_readimb = 0;
					tl->_imbflip = false;
				}
				
				// this is from asyncresponse, but may not be same
				// functions because it doesn't appear to behave as nicely
				//ResetEvent(tl->_tickswaiting);
				//WaitForSingleObject(tl->_tickswaiting,INFINITE);
			}
			Sleep(100);
		}
		// mark thread as terminating
		tl->_startimb = false;
		// end thread
		return OK;
	}

	TLOrder AVL_TLWM::ProcessOrder(Order* order)
	{
		TLOrder null;
		// don't process null orders
		if (order==NULL) 
			return null; 

		// send cancel for a dead order update
		if (order->isDead()) 
		{
			//const MsgUpdateOrder* msg2 = (const MsgUpdateOrder*)message;
			int64 id = fetchOrderIdAndRemove(order);
			if (id>0)
				SrvGotCancel(id);
			return null;
		}

		// try to save this order
		bool isnew = saveOrder(order,0);
		// if it fails, we already have it so get the id
		// if it succeeds, we should be able to get the id anyways
		int64 id = fetchOrderId(order);

		CTime ct = CTime::GetCurrentTime();
		TLOrder o;
		o.id = id;
		o.price = order->isMarketOrder() ? 0: GetDouble(order->GetOrderPrice());
		o.stop = GetDouble(order->GetStopPrice());
		o.time = (ct.GetHour()*10000)+(ct.GetMinute()*100)+ct.GetSecond();
		o.date = (ct.GetYear()*10000)+(ct.GetMonth()*100)+ct.GetDay();
		o.size = order->GetSize();
		o.side = order->GetSide()=='B';
		o.comment = order->GetUserDescription();
		o.TIF = TIFName(order->GetTimeInForce());
		o.account = CString(B_GetAccountName(order->GetAccount()));
		o.symbol = CString(order->GetSymbol());
		return o;
	}


	void AVL_TLWM::SrvGotImbAsync(TLImbalance imb)
	{
		// if thread is stopped don't restart it
		if (!_go) return;
		// add tick to queue and increment
		_imbcache[_writeimb++] = imb;
		// implement ringbuffer on queue
		if (_writeimb>=_imbcache.size())
		{
			_writeimb = 0;
			_imbflip  = true;
		}
		// ensure that we're reading from thread
		if (!_startimb)
		{
			AfxBeginThread(DoReadImbThread,this);
			_startimb = true;
		}
		else
		{
			// signal read thread that ticks are ready (adapted from asyncresponse)
			//SetEvent(_tickswaiting);
		}
	}

	void AVL_TLWM::SrvGotImbalance(TLImbalance imb)
	{
		for (uint i = 0; i<imbalance_clients.size(); i++)
			TLSend(IMBALANCERESPONSE,TLImbalance::Serialize(imb),imbalance_clients[i]);

	}

	unsigned int AVL_TLWM::AnvilId(int64 TLOrderId)
	{
		for (uint i = 0; i<orderids.size(); i++)
		{
			if ((orderids[i]==TLOrderId) && ordercache[i])
				return ordercache[i]->GetId();
		}
		return 0;
	}

	std::vector<int> AVL_TLWM::GetFeatures()
	{
		std::vector<int> f;
		f.push_back(ISSHORTABLE);
		f.push_back(BROKERNAME);
		f.push_back(ACCOUNTREQUEST);
		f.push_back(ACCOUNTRESPONSE);
		f.push_back(HEARTBEATREQUEST);
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
		f.push_back(VWAP);	
		f.push_back(LASTTRADESIZE);	
		f.push_back(LASTTRADEPRICE);	
		f.push_back(LASTBID);			
		f.push_back(LASTASK);			
		f.push_back(BIDSIZE);
		f.push_back(ASKSIZE);
		f.push_back(DAYLOW);
		f.push_back(DAYHIGH);		
		f.push_back(OPENPRICE);		
		f.push_back(CLOSEPRICE);	
		f.push_back(LRPBUY);		
		f.push_back(LRPSELL);
		f.push_back(SENDORDERMARKET);
		f.push_back(SENDORDERLIMIT);
		f.push_back(SENDORDERSTOP);
		f.push_back(SENDORDERTRAIL);
		f.push_back(DOMREQUEST);
		f.push_back(LIVEDATA);
		f.push_back(ISSHORTABLE);
		f.push_back(IMBALANCEREQUEST);
		f.push_back(VWAP);
		f.push_back(LASTTRADESIZE);
		f.push_back(LASTTRADEPRICE);
		f.push_back(LASTBID);
		f.push_back(LASTASK);
		f.push_back(BIDSIZE);
		f.push_back(ASKSIZE);
		f.push_back(DAYLOW);
		f.push_back(DAYHIGH);
		f.push_back(OPENPRICE);
		f.push_back(CLOSEPRICE);
		f.push_back(LRPBUY);
		f.push_back(LRPSELL);
		f.push_back(AMEXLASTTRADE);
		f.push_back(NASDAQLASTTRADE);
		f.push_back(NYSEBID);
		f.push_back(NYSEASK);
		f.push_back(NYSEDAYHIGH);
		f.push_back(NYSEDAYLOW);
		f.push_back(NYSELASTTRADE);
		f.push_back(NASDAQIMBALANCE);
		f.push_back(NASDAQPREVIOUSIMBALANCE);
		f.push_back(NYSEIMBALACE);
		f.push_back(NYSEPREVIOUSIMBALANCE);
		f.push_back(INTRADAYHIGH);
		f.push_back(INTRADAYLOW);
		f.push_back(POSITIONREQUEST);
		f.push_back(POSITIONRESPONSE);
		f.push_back(ORDERNOTIFYREQUEST);
		bool sim = B_IsAccountSimulation();
		if (sim)
			f.push_back(SIMTRADING);
		else 
			f.push_back(LIVETRADING);
		return f;
	}

	int AVL_TLWM::CancelRequest(int64 tlsid)
	{
		bool found = false;
		// get current anvil id from tradelink id
		for (uint i = 0; i<orderids.size(); i++)
		{
			// make sure it's our order and order isn't NULL
			if ((orderids[i]==tlsid) && (ordercache[i]!=NULL))
			{
				__try 
				{
					// try to cancel it
					ordercache[i]->Cancel();
					// mark it as found
					bool found = true;
				} 
				__except (EXCEPTION_EXECUTE_HANDLER) // catch errors
				{
					// mark this order as null
					ordercache[i] = NULL;
				}
			}
		}
		// return
		if (!found) return ORDER_NOT_FOUND;
		return OK;
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

	double AVL_TLWM::GetDouble(const Money* m)
	{
		double v = m->GetWhole();
		int tf = m->GetThousandsFraction();
		double f = ((double)tf/1000);
		v += f;
		return v;

	}
	double AVL_TLWM::GetDouble(Money  m)
	{
		double v = m.GetWhole();
		int tf = m.GetThousandsFraction();
		double f = ((double)tf/1000);
		v += f;
		return v;
	}

	int AVL_TLWM::PositionResponse(CString account, CString client)
	{
		if (account=="") return INVALID_ACCOUNT;
		Observable* m_account = B_GetAccount(account);
		void* iterator = B_CreatePositionIterator(POSITION_FLAT|POSITION_LONG|POSITION_SHORT, (1 << ST_LAST) - 1,m_account);
		B_StartIteration(iterator);
		const Position* pos;
		int count = 0;
		while(pos = B_GetNextPosition(iterator))
		{
			TradeLibFast::TLPosition p;
			bool overnight = pos->isOvernight();
			p.AvgPrice = GetDouble((Money)pos->GetAveragePrice());
			p.ClosedPL = GetDouble(pos->GetClosedPnl());
			p.Size = pos->GetSize();
			p.Symbol = CString(pos->GetSymbol());
			p.Account = account;
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
			int symidx = FindSym(my[i]);
			if (isIndex(my[i]))
				sec = new AVLIndex(my[i],symidx,this);
			else
			{
				//AVLStock *stk = new AVLStock(my[i],this); // create new stock instance
				AVLStock *stk = new AVLStock(my[i],symidx,this,true,depth); // create new stock instance with added depth param
				sec = stk;
				
			}
			subs.push_back(sec);
			subsym.push_back(my[i]);
		}
		stocks[cid] = my; // index the array by the client's id
		HeartBeat(clientname); // update the heartbeat
		return 0;
	}

	int AVL_TLWM::DOMRequest(int depth)
	{ 
		this->depth = depth;
		//ClearStocks();
		return 0;
	}

	int AVL_TLWM::ClearClient(CString client) 
	{
		// get id for this client
		int id = FindClient(client);
		// make sure client exists
		if (id<0) return OK;
		// call base clear
		TLServer_WM::ClearClient(client);
		// remove any subscriptions this stock has that aren't used by others
		RemoveUnused(); 
		// remove imbalance subscriptions if they exist
		vector<int> newics;
		for (uint i = 0; i<imbalance_clients.size(); i++)
			if (imbalance_clients[i]!=id)
				newics.push_back(imbalance_clients[i]);
		imbalance_clients = newics;
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

	int AVL_TLWM::ClearStocks()
	{
		//clear stocks for all clients
		size_t len = client.size();
		for (size_t i = 0; i<len; i++)
		{
			// remove record of stocks
			TLServer_WM::ClearStocks(client[i]);
		}
		// remove anvil subs
		//RemoveUnused();
		return OK;
	}




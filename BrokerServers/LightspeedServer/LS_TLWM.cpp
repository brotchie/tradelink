#include "stdAfx.h"
#include "LS_TLWM.h"
//#include "Util.h"




	
	
	LS_TLWM* LS_TLWM::instance = NULL;	

	const char* CONFIGFILE = "LightSpeedServer.Config.txt";
	void LS_TLWM::ReadConfig()
	{
		// read config file
		std::ifstream file;
		file.open(CONFIGFILE);
		// prepare buffers
		int sessionid = 0;
		const int ss = 200;
		char skip[ss];
		const int ds = 20;
		char data[ds];
		// skip comment
		file.getline(skip,ss);
		// read data
		file.getline(data,ds);
		// convert it (imbalance exchanges)
		_imbexch = atoi(data) == 1;
		// skip comment
		file.getline(skip,ss);
		// read data
		file.getline(data,ds);
		// convert it (verbose)
		_noverb = atoi(data) == 0;

		// close config file
		file.close();
	}

	void LS_TLWM::v(const CString &message)
	{
		if (_noverb)
			return;
		D(message);
	}

	void LS_TLWM::D(const CString &message)
	{
		L_AddMessageToExtensionWnd(message);
		TLServer_WM::D(message);
		
	}

	IMPLEMENT_DYNAMIC(LS_TLWM, CWnd)

	LS_TLWM::LS_TLWM(void)
	{
		instance = this;

		PROGRAM = "LightspeedConnector";

		// remove canceled orders
		//B_KeepCancelledOrders(false);

		// imbalances are off by default
		imbreq = false;

		// add this object as observer to every account,
		// so we can get fill and order notifications
		
		L_Account* account = L_GetAccount();
		account->L_Attach(this);
		accounts.push_back(account);

		std::vector<int> now;
		TLTimeNow(now);
		_date = now[TLdate];
		_time = now[TLtime];

		depth = 0;

		nextcorr = GetTickCount();
		
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



	LS_TLWM::~LS_TLWM(void)
	{
		// remove all account observables
		for (uint i = 0; i<accounts.size(); i++)
		{
			accounts[i]->L_Detach(this);
			accounts[i] = NULL;
		}
		accounts.clear();
		// remove all pointers to orders
		for (uint i = 0; i<ordercache.size(); i++)
			ordercache[i] = NULL;
		// clear cache
		ordercache.clear();

		if (imbreq)
		{
			L_UnsubscribeFromOrderImbalances(this);
			imbreq = false;
		}
		_imbcache.clear();

		// stock stuff, close down hammer subscriptions
		for (size_t i = 0; i<subs.size(); i++)
		{
			if (subs[i]!=NULL)
			{
				L_UnsubscribeFromTrades(subs[i]->L_Symbol(),this);
				subs[i]->L_Detach(this);
				L_DestroySummary(subs[i]);
				subs[i] = NULL;
			}
		}
		subs.clear();
		subsym.clear();


	}








	int LS_TLWM::BrokerName(void)
	{
		return LightspeedDesktop;
	}

	TLOrder LS_TLWM::ProcessOrder(L_Order* order)
	{
		TLOrder o;
		long lsid = order->L_ReferenceId();
		// ensure it exists
		if (order)
		{
			
			// udpate order information
			CTime ct = CTime(order->L_CreateTime());
			o.symbol = CString(order->L_Symbol());
			o.time = (ct.GetHour()*10000)+(ct.GetMinute()*100)+ct.GetSecond();
			o.date = (ct.GetYear()*10000)+(ct.GetMonth()*100)+ct.GetDay();
			o.exchange = CString(order->L_OriginalMarket());
			o.price = order->L_AveragePrice();
			o.side = order->L_TradeSide()=='B';
			o.stop = order->L_SecondaryPrice();
			o.size = (o.side ? 1 : -1) * abs(order->L_ActiveShares());
			o.account = CString(accounts[0]->L_TraderId());
			// try to save this order
			bool isnew = saveOrder(order,0);
			// if it fails, we already have it so get the id
			// if it succeeds, we should be able to get the id anyways
			int64 tlid = fetchOrderId(order);
			o.id = tlid; 
			// save the relationship
			saveOrderId(tlid,lsid);
			// debug
			if (!_noverb)
			{
				CString tmp;
				tmp.Format("lsid: %i tlid: %lld ordack: %s",lsid,tlid,o.Serialize());
				v(tmp);
			}
		}
		else
		{
			CString tmp;
			tmp.Format("%i order id not found.",lsid);
			D(tmp);
		}
		return o;
	}

	int LS_TLWM::UnknownMessage(int MessageType,CString msg)
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
				if (accounts.size()==0)
					return INVALID_ACCOUNT;
				
				int count = 0;
				
				L_Account* acct = accounts[0];
				

				for (order_iterator pi = acct->active_orders_begin();pi != acct->active_orders_end(); ++pi)
				{
					L_Order * ord = (*pi);
					TradeLibFast::TLOrder o = ProcessOrder(ord);
					// send valid orders back to client who requested them
					if (o.isValid())
						TLSend(ORDERNOTIFY,o.Serialize(),r[1]);
				}
				acct = NULL;
			}
			break;
		case ISSHORTABLE :
			{
				
				L_Summary* s = preload(msg);
				if ((s==NULL)) 
					return SYMBOL_NOT_LOADED;
				int bcode = s->L_Borrowable();
				if (bcode==-1)
					return UNKNOWN_MESSAGE;
				else if (bcode==0)
					return 1;
				else
					return 0;
			}
			break;
		case IMBALANCEREQUEST :
			{
				// get client id
				int id = FindClient(msg);
				// ignore invalid clients
				if (id<0) return OK;
				CString m;
				m.Format("got imbalance request from: %s",client[id]);
				D(m);
				// only request imbalances once on broker side
				if (!imbreq)
				{
					// get an observable for imbalances
					// make this object watch it
					L_SubscribeToOrderImbalances(this);
					// mark it
					imbreq = true;
				}
				// set this client to receive imbalances
				imbalance_clients.push_back(id);
				return OK;
			}
			break;

		
		
		}
		return UNKNOWN_MESSAGE;
	}


	bool LS_TLWM::hasHammerSub(CString symbol)
	{
		for (uint i = 0; i<subsym.size(); i++) 
			if (symbol==subsym[i]) 
				return true;
		return false;
	}
	
	L_Summary* LS_TLWM::preload(CString symbol)
	{
		for (uint i = 0; i<subs.size(); i++)
		{
			if (!isIndex(subsym[i]) && (subs[i]!=NULL) && (subsym[i]==symbol))
			{
				return subs[i];
			}
		}
		L_Summary* sec = L_CreateSummary(symbol);
		sec->L_Attach(this);
		subs.push_back(sec);
		subsym.push_back(symbol);
		return sec;
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

	long LS_TLWM::gettype(TLOrder o)
	{
		long type = o.isStop() ? L_OrderType::STOP : 
			( o.isLimit() ? L_OrderType::LIMIT : L_OrderType::MARKET);

		if ((o.TIF=="DAY")
			|| (o.TIF=="IOC")
			|| (o.TIF=="GTC")
			|| (o.TIF==""))
			return type;

		if (o.TIF=="MOC")
			return L_OrderType::MOC;
		else if ((o.TIF=="LOO") || (o.TIF=="OPG"))
			return L_OrderType::LOO;
		else if (o.TIF=="LOC")
			return L_OrderType::LOC;
		else if (o.TIF=="MOO")
			return L_OrderType::MOO;

		return type;
	}

	long LS_TLWM::gettif(TLOrder o)
	{
		CString tifs = o.TIF;
		if (tifs=="DAY")
			return L_TIF::DAY;
		else if (tifs=="IOC")
			return L_TIF::IOC;

		return L_TIF::DAY;
	}

	int LS_TLWM::SendOrder(TLOrder o) 
	{
		if (accounts.size()==0)
			return INVALID_ACCOUNT;
		// if order id is set and not-unique, reject order
		if ((o.id!=0) && (!IdIsUnique(o.id)))
			return DUPLICATE_ORDERID;

		L_Summary* summary = preload(o.symbol);
		L_Account* account = NULL;
		for (uint i = 0; i<accounts.size(); i++)
		{
			if (CString(accounts[i]->L_TraderId())==o.account)
				account = accounts[i];
		}
		if (account==NULL)
			account = accounts[0];

		CString ex = o.exchange;
		CString ex2 = NULL;
		if (o.exchange.FindOneOf("+")!=-1)
		{
			std::vector<CString> exr;
			gsplit(o.exchange,CString("+"),exr);
			ex = exr[0];
			ex2 = exr[1];
		}
		

		//convert the arguments
		char side = o.side ? L_Side::BUY : L_Side::SELL;
		
		double price = o.isStop() ? o.stop : o.price;
		// get correlation id
		nextcorr++;
		long* corid = &nextcorr;
		// associate order id and correlation id
		lscorrelationid.push_back(corid);
		tlcorrelationid.push_back(o.id);

		// get appropriate tif
		long tif = gettif(o);
		long type = gettype(o);

		// prepare to receive the result
		L_Order** orderSent = NULL;
		L_Order** orderSent2 = NULL;
		// send the order
		account->L_SendOrder(
				summary,
				type,
				side,
				abs(o.size),
				price,
				ex,
				tif,
				false,
				abs(o.size),
				0,ex2,corid);
		
		
		
		// return result
		return 0;
		
	}


	bool LS_TLWM::IdIsUnique(int64 id)
	{
		for (uint i = 0; i<orderids.size(); i++)
			if (orderids[i]==id) 
				return false;
		return true;
	}
	
	int64 LS_TLWM::fetchOrderId(L_Order* order)
	{
		if (order==NULL) return ORDER_NOT_FOUND;
		for (uint i = 0; i<ordercache.size(); i++)
			if (ordercache[i]==order) 
				return orderids[i];
		return 0;
	}

	int64 LS_TLWM::fetchOrderIdAndRemove(L_Order* order)
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

	bool LS_TLWM::saveOrderId(int64 tlid, long lsid)
	{
		for (uint i = 0; i<orderids.size(); i++)
		{
			if (orderids[i]==tlid)
			{
				if (lsids[i]==0)
				{
					lsids[i] = lsid;
					return true;
				}
				else
					return false;
			}
		}
		return false;
	}

	bool LS_TLWM::saveOrder(L_Order* o, int64 id) { return saveOrder(o,id,false); }
	bool LS_TLWM::saveOrder(L_Order* o,int64 id, bool allowduplicates)
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
		// save spot for lsid
		lsids.push_back(0);
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

	CString LS_TLWM::or2str(long res)
	{
		switch (res)
		{
		case L_OrderResult::ORDER_SENT_OK : return CString("ORDER_SENT_OK");
		case L_OrderResult::SOES_ORDER_DISABLED: return CString("SOES_ORDER_DISABLED");
		case L_OrderResult::MM_IN_EXCLUSION_LIST : return CString("MM_IN_EXCLUSION_LIST");
		case L_OrderResult::ZERO_SHARES_ORDERED : return CString("ZERO_SHARES_ORDERED");
		case L_OrderResult::EXECUTIONS_DISABLED : return CString("EXECUTIONS_DISABLED");
		case L_OrderResult::BUYING_POWER_EXCEEDED : return CString("BUYING_POWER_EXCEEDED");
		case L_OrderResult::SHORT_SELL_VIOLATION : return CString("SHORT_SELL_VIOLATION");
		case L_OrderResult::STOCK_NOT_SHORTABLE : return CString("STOCK_NOT_SHORTABLE");
		case L_OrderResult::EXECUTOR_NOT_CONNECTED : return CString("EXECUTOR_NOT_CONNECTED");
		case L_OrderResult::MAXORDERSHARES_EXCEEDED : return CString("MAXORDERSHARES_EXCEEDED");
		case L_OrderResult::WAIT_CONSTRAINT_VIOLATION : return CString("WAIT_CONSTRAINT_VIOLATION");
		case L_OrderResult::STOCK_HALTED : return CString("STOCK_HALTED");
		case L_OrderResult::MKXT_BOOK_OR_KILL : return CString("MKXT_BOOK_OR_KILL");
		case L_OrderResult::SMALL_CAPS_NOT_SOESABLE : return CString("SMALL_CAPS_NOT_SOESABLE");
		case L_OrderResult::OWN_CROSSING : return CString("OWN_CROSSING");
		case L_OrderResult::CANNOT_TRADE_SYMBOL : return CString("CANNOT_TRADE_SYMBOL");
		case L_OrderResult::MARKET_HALTED : return CString("MARKET_HALTED");
		case L_OrderResult::FUTURES_MARGINABILITY_UNKNOWN : return CString("FUTURES_MARGINABILITY_UNKNOWN");
		case L_OrderResult::TRADINGMONITOR_BLOCKED_ORDER : return CString("TRADINGMONITOR_BLOCKED_ORDER");
		case L_OrderResult::DECLINED_AT_CONFIRM_BY_USER : return CString("DECLINED_AT_CONFIRM_BY_USER");
		case L_OrderResult::ROUTING_BLOCKED_ORDER : return CString("ROUTING_BLOCKED_ORDER");
		case L_OrderResult::OTHER_REJECTION : return CString("OTHER_REJECTION");
		case L_OrderResult::EXECUTOR_NOT_LOGGED_IN : return CString("EXECUTOR_NOT_LOGGED_IN");
		case L_OrderResult::UNINITIALIZED_SUMMARY : return CString("UNINITIALIZED_SUMMARY");
		case L_OrderResult::INVALID_SUMMARY : return CString("INVALID_SUMMARY");
		case L_OrderResult::INVALID_ORDER_TYPE : return CString("INVALID_ORDER_TYPE");
		default :
												 {
													 CString m;
													 m.Format("Unknown sendorder result: %l"+res);
													 return m;
													 break;
												 }
		}
	}

	void LS_TLWM::HandleMessage(L_Message const *msg)
	{
		switch (msg->L_Type())
		{
		case L_MsgOrderRequested::id:
			{
				L_MsgOrderRequested* m = (L_MsgOrderRequested*)msg;
				long res = m->L_Result();
				// check for error
					long lscorid = m->L_CorrelationId();
					CString err = or2str(res);
					int64 tlid = 0;
					for (uint i = 0; i<lscorrelationid.size(); i++)
						if (*lscorrelationid[i]==lscorid)
							tlid = tlcorrelationid[i];
					CString ms;
					ms.Format("orderreq id: %i tlid: %lld result: %s code: %i Symbol: %s",m->L_Order1ReferenceId(),tlid,err,res,m->L_Symbol());
					D(ms);
				break;
			}
		case L_MsgOrderChange::id: // orders?
			{
				L_MsgOrderChange* m = (L_MsgOrderChange*)msg;
				const L_Execution* x = m->L_Exec();
				long lsid = m->L_ReferenceId();
				// test if this is an execution or an order
				// no execution, must be an order
				if (x==NULL)
				{
					// check type of order
					switch (m->L_Category())
					{
					case L_OrderChange::Create:
						{
							TLOrder o;
							o.symbol = CString(msg->L_Symbol());
							// ensure we have an account
							if (accounts.size()>0)
							{
								// get order
								L_Order* order = accounts[0]->L_FindOrder(lsid);
								// process it
								o = ProcessOrder(order);
								// notify
								if (o.isValid())
									SrvGotOrder(o);
							}

						}
						break;
					case L_OrderChange::Rejection:
						{
							CString m;
							m.Format("lsid: %i rejected for %s",lsid,msg->L_Symbol());
							D(m);
						break;
						}
					case L_OrderChange::Cancel:
						{
							// ensure we have accounts
							if (accounts.size()>0)
							{
								bool found = false;
								// get the order from id
								for (uint i = 0; i<lsids.size(); i++)
									if (lsids[i]==lsid)
									{
										int64 tlid = orderids[i];
										SrvGotCancel(tlid);
										found = true;
										// debug
										if (!_noverb)
										{
											CString tmp;
											tmp.Format("lsid: %i tlid: %lld cancelack.",lsid,tlid);
											v(tmp);
										}
										break;
									}
								// notify
							    if (!found)
								{
									CString tmp;
									tmp.Format("%i order id was not found to send cancel ack.",lsid);
									D(tmp);
								}

							}

						
						}
						break;


					}

				}
				else// it's an execution
				{
					
					
					
					TLTrade f;
					f.symbol = CString(x->L_Symbol());
					f.id = x->L_OrderId();
					f.xprice = x->L_AveragePrice();
					f.side = x->L_Side()=='B';
					f.xsize = abs(x->L_Shares()) * (f.side ? 1 : -1);
					CTime ct = CTime(x->L_ExecTime());
					f.xtime = (ct.GetHour()*10000)+(ct.GetMinute()*100)+ct.GetSecond();
					f.xdate = (ct.GetYear()*10000)+(ct.GetMonth()*100)+ct.GetDay();

					SrvGotFill(f);
					return;
			}
			}
			

			//SetDlgItemInt(IDC_PENDINGORDERS, account->L_PendingOrdersCount());
			break;
		case L_MsgL1::id:
		case L_MsgL1Update::id: // bid+ask
			{
				TLTick k;

				L_MsgL1Update* m = (L_MsgL1Update*)msg;
				
				k.bid = m->L_Bid();
				k.bs = m->L_BidSize();
				k.ask = m->L_Ask();
				k.os = m->L_AskSize();
				k.sym = CString(m->L_Symbol());

				k.time = _time;
				k.date = _date;
				this->SrvGotTickAsync(k);



			}
			break;
		case L_MsgL2Update::id:
		case  L_MsgL2Refresh::id:
		case L_MsgL2::id:
			{
			}
			break;

		case L_MsgTradeUpdate::id: // trade
			{
				L_MsgTradeUpdate* m = (L_MsgTradeUpdate*)msg;
				if (!m->L_Printable())
					break;
				TLTick k;
				CTime ct = CTime(m->L_Time());
				k.sym = CString(m->L_Symbol());
				k.trade = m->L_Price();
				k.date = _date;
				k.time = (ct.GetHour()*10000)+(ct.GetMinute()*100)+ct.GetSecond();
				_time = k.time;
				k.size = (int)m->L_Volume();
				k.ex = CString(m->L_Market());
				SrvGotTickAsync(k);

			}
			break;
		case L_MsgOrderImbalance::id:
			{
				L_MsgOrderImbalance* m = (L_MsgOrderImbalance*)msg;
				TLImbalance imb;
				imb.Symbol = CString(m->L_Symbol());
				CTime ct = CTime(m->L_Time());
				if (_imbexch)
				{
					// get summary
					L_Summary* sum = L_CreateSummary(m->L_Symbol());
					// update exchange
					imb.Ex = CString(sum->L_Exchange());
					// release summary 
					L_DestroySummary(sum);
				}

				imb.ThisTime = (ct.GetHour()*10000)+(ct.GetMinute()*100)+ct.GetSecond();

				if (m->L_RegImbalance()=='1')
				{
					imb.ThisImbalance = (int)(m->L_BuyVolumeReg() - m->L_SellVolumeReg());
				}
				if (m->L_RegImbalance()=='0')
				{
					imb.ThisImbalance = (int)(m->L_BuyVolumeReg() - m->L_SellVolumeReg());
					imb.InfoImbalance = (int)(m->L_BuyVolume() - m->L_SellVolume());
				}
				if ((imb.ThisImbalance==0) && (imb.InfoImbalance==0))
					break;
				SrvGotImbAsync(imb);

			}
			break;
		}
	}



		UINT __cdecl DoReadImbThread(LPVOID param)
	{
		// we need a queue object
		LS_TLWM* tl = (LS_TLWM*)param;
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

		


	void LS_TLWM::SrvGotImbAsync(TLImbalance imb)
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

	void LS_TLWM::SrvGotImbalance(TLImbalance imb)
	{
		for (uint i = 0; i<imbalance_clients.size(); i++)
			TLSend(IMBALANCERESPONSE,TLImbalance::Serialize(imb),imbalance_clients[i]);

	}

	unsigned int LS_TLWM::AnvilId(int64 TLOrderId)
	{
		for (uint i = 0; i<orderids.size(); i++)
		{
			if ((orderids[i]==TLOrderId) && ordercache[i])
				return ordercache[i]->L_OrderId();
		}
		return 0;
	}

	std::vector<int> LS_TLWM::GetFeatures()
	{
		std::vector<int> f;
		f.push_back(BROKERNAME);
		f.push_back(HEARTBEATREQUEST);
		f.push_back(REGISTERCLIENT);
		f.push_back(SENDORDER);
		f.push_back(EXECUTENOTIFY);
		f.push_back(ORDERNOTIFY);
		f.push_back(ORDERCANCELRESPONSE);
		f.push_back(ORDERCANCELREQUEST);
		f.push_back(REGISTERSTOCK);
		f.push_back(CLEARCLIENT);
		f.push_back(CLEARSTOCKS);
		f.push_back(ISSHORTABLE);
		f.push_back(ORDERNOTIFYREQUEST);
		f.push_back(FEATUREREQUEST);
		f.push_back(FEATURERESPONSE);
		f.push_back(TICKNOTIFY);
		f.push_back(IMBALANCEREQUEST);
		f.push_back(IMBALANCERESPONSE);

		return f;
	}

	int LS_TLWM::CancelRequest(int64 tlid)
	{
		if (accounts.size()==0)
		{
			D("No accounts available for cancel.");
			return OK;
		}
		L_Account* account = accounts[0];
		bool found = false;
		// get current anvil id from tradelink id
		for (uint i = 0; i<orderids.size(); i++)
		{
			
			// make sure it's our order and order isn't NULL
			if ((orderids[i]==tlid) && (ordercache[i]!=NULL))
			{
				long lsid = lsids[i];
					// try to cancel it
					account->L_CancelOrder(ordercache[i]);
					// mark it as found
					found = true;
					// debug
					if (!_noverb)
					{
						CString tmp;
						tmp.Format("lsid: %i tlid: %lld cancelreq.",lsid,tlid);
						v(tmp);
					}
				
			}
			
		}
		account = NULL;
		// return
		if (!found) 
		{
				// debug
				if (!_noverb)
				{
					CString tmp;
					tmp.Format("tlid: %lld could not find ls order to cancel.",tlid);
					v(tmp);
				}
			return ORDER_NOT_FOUND;
		}
		return OK;
	}

	int LS_TLWM::AccountResponse(CString client)
	{
		
		std::vector<CString> accts;
		// loop through every available account
		for (uint i = 0; i<accounts.size(); i++)
		{
			accts.push_back(CString(accounts[i]->L_TraderId()));
		}
		CString msg = ::gjoin(accts,CString(","));
		TLSend(ACCOUNTRESPONSE,msg,client);
		
		return OK;
	}

	

	int LS_TLWM::PositionResponse(CString account, CString client)
	{
		if (account=="") 
			return INVALID_ACCOUNT;
		if (accounts.size()==0)
			return INVALID_ACCOUNT;
		
		int count = 0;
		
		L_Account* acct = accounts[0];
		

		for (position_iterator pi = acct->positions_end();pi != acct->positions_end(); ++pi)
		{
			L_Position* pos = (*pi);
			TradeLibFast::TLPosition p;
			p.AvgPrice = pos->L_AveragePrice();;
			p.ClosedPL = pos->L_ClosedPL();;
			p.Size = pos->L_Shares();
			p.Symbol = CString(pos->L_Symbol());
			p.Account = CString(acct->L_TraderId());
			CString msg = p.Serialize();
			TLSend(POSITIONRESPONSE,msg,client);
			count++;
		}
		acct = NULL;
		
		return count;
	}

	int LS_TLWM::SubIdx(CString symbol)
	{
		for (size_t i = 0; i<subsym.size(); i++) 
			if (subsym[i]==symbol)
				return (int)i;
		return -1;
	}



	bool LS_TLWM::isIndex(CString sym)
	{
		int slashi = sym.FindOneOf("/");
		int doli = sym.FindOneOf("$");
		int poundi = sym.FindOneOf("#");
		return (slashi!=-1)||(doli!=-1)||(poundi!=-1);
	}

	void LS_TLWM::RemoveSub(CString stock)
	{
		if (hasHammerSub(stock))
		{
			size_t i = SubIdx(stock);
			if (subs[i]!=NULL)
			{
				subsym[i] = "";
				subs[i]= NULL;
			}
		}
	}

	void LS_TLWM::RemoveUnused()
	{
		for (uint i = 0; i<stocks.size(); i++)
			for (uint j = 0; j<stocks[i].size(); j++)
				if (!needStock(stocks[i][j]))
					RemoveSub(stocks[i][j]);
		//B_UnsubscribeUnusedStocks();

	}

	int LS_TLWM::RegisterStocks(CString clientname)
	{ 
		TLServer_WM::RegisterStocks(clientname);
		unsigned int cid = FindClient(clientname);
		if (cid==-1) return CLIENTNOTREGISTERED; //client not registered
		clientstocklist my = stocks[cid]; // get stocks
		for (size_t i = 0; i<my.size();i++) // subscribe to stocks
		{
			if (hasHammerSub(my[i])) continue; // if we've already subscribed once, skip to next stock
			L_Summary * sec;
			int symidx = FindSym(my[i]);
			if (isIndex(my[i]))
				sec = NULL; 
			else
			{
				
				sec = L_CreateSummary(my[i]);
				sec->L_Attach(this);
				// get trade data
				L_SubscribeToTrades(my[i],this);
				CString m;
				m.Format("added subscription for: %s",my[i]);
				D(m);
			}
			subs.push_back(sec);
			subsym.push_back(my[i]);
		}
		stocks[cid] = my; // index the array by the client's id
		HeartBeat(clientname); // update the heartbeat
		return 0;
	}

	int LS_TLWM::DOMRequest(int depth)
	{ 
		this->depth = depth;
		//ClearStocks();
		return 0;
	}

	int LS_TLWM::ClearClient(CString client) 
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

	int LS_TLWM::ClearStocks(CString client)
	{
		// remove record of stocks
		TLServer_WM::ClearStocks(client);
		// remove anvil subs
		RemoveUnused();
		return OK;
	}

	int LS_TLWM::ClearStocks()
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




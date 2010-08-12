#include "StdAfx.h"
#include "TWS_TLServer.h"
#include "Util.h"
#include "Execution.h"
#include <fstream>
#include "IBUtil.h"

namespace TradeLibFast
{
	const char* CONFIGFILE = "TwsServer.Config.txt";
	const char* LINE = "-------------------------------";

	TWS_TLServer::TWS_TLServer(void)
	{
		IGNOREERRORS = false;
		_currency = CString("USD");
		noverb = true;

	}

	void TWS_TLServer::v(CString msg)
	{
		if (noverb) return;
		D(msg);
	}

	TWS_TLServer::~TWS_TLServer(void)
	{
		// cancel subscriptions
		for (size_t i = 0; i<stockticks.size(); i++)
			this->m_link[this->validlinkids[0]]->cancelMktData((TickerId)i);
		// disconnect from IB and unallocate memory
		for (size_t i = 0; i<m_link.size(); i++)
		{
			m_link[i]->eDisconnect();
			delete m_link[i];
		}

	}
	
	void TWS_TLServer::Start(void)
	{
		TLServer_WM::Start();

		std::ifstream file;
		file.open(CONFIGFILE);
		int sessionid = 0;
		char skip[100];
		char data[8];
		file.getline(skip,100);
		file.getline(data,8);
		this->FIRSTSOCKET = atoi(data);
		file.getline(skip,100);
		file.getline(data,8);
		int maxsockets = atoi(data); // get the # of sockets first
		file.getline(skip,100);
		file.getline(data,8);
		sessionid = atoi(data); // get the session id next 
		file.getline(skip,100);
		file.getline(data,8); 
		_currency = CString(data); // get currency
		// get verbosity
		file.getline(skip,100);
		file.getline(data,8);
		noverb = 0==atoi(data);
		file.close();
		if (!noverb)
			D("verbosity is on");
		CString msg;
		if (sessionid!=0)
		{
			D("Orders can only be received from this machine.");
			D(CString("To change this, see ")+CONFIGFILE);
		}
		msg.Format("Looking for %i TWS instances...",maxsockets);
		D(msg);
		D(CString(LINE));
		InitSockets(maxsockets,sessionid);
		D(CString(LINE));
		msg.Format("Found %i of %i.",this->validlinkids.size(),maxsockets);
		D(msg);
		D(CString("For more instances, change value in: ")+CONFIGFILE);
		msg.Format("Found accounts: %s",gjoin(accts,","));
		D(msg);
		msg.Format("Using currency: %s",_currency);
		D(msg);
	}

	void TWS_TLServer::InitSockets(int maxsockets, int clientid)
	{

		// start socket connections that attempt to connect
		// to a specified number of [already running] TWS instances, starting at FIRSTSOCKET
		m_nextsocket = FIRSTSOCKET;
		IGNOREERRORS = true;


		for (int idx = 0; idx<maxsockets; idx++)
		{
			m_link.push_back(new EClientSocket(this)); // create socket instance
			const char* host = _T("");
			CString msg;
			bool good = m_link[idx]->eConnect(host,m_nextsocket,clientid); // connect
			m_link[idx]->setServerLogLevel(5);
			if (clientid==0)
			{
				m_link[idx]->reqAutoOpenOrders(true); // enable order notifications
			}
			m_link[idx]->reqAccountUpdates(true,_T("")); // get account names for this link
			CString time = m_link[idx]->TwsConnectionTime();
			msg.Format(" port: %i",m_nextsocket);
			if (time.GetLength()>0)
			{
				D(CString("Found instance at")+msg);
				this->validlinkids.push_back(idx);
			}
			else D(CString("Nothing found at")+msg);
			m_nextsocket++;
		}
		IGNOREERRORS = false;
	}

	std::vector<int> TWS_TLServer::GetFeatures()
	{
		std::vector<int> f;
		f.push_back(SENDORDERMODIFY);
		f.push_back(SENDORDER);
		f.push_back(BROKERNAME);
		f.push_back(REGISTERCLIENT);
		f.push_back(REGISTERSTOCK);
		f.push_back(ACCOUNTREQUEST);
		f.push_back(ACCOUNTRESPONSE);
		f.push_back(ORDERCANCELREQUEST);
		f.push_back(ORDERCANCELRESPONSE);
		f.push_back(ORDERNOTIFY);
		f.push_back(EXECUTENOTIFY);
		f.push_back(TICKNOTIFY);
		f.push_back(SENDORDERMARKET);
		f.push_back(SENDORDERLIMIT);
		f.push_back(SENDORDERSTOP);
		f.push_back(SENDORDERTRAIL);
		f.push_back(LIVEDATA);
		f.push_back(LIVETRADING);
		f.push_back(SIMTRADING);
		f.push_back(POSITIONRESPONSE);
		f.push_back(POSITIONREQUEST);
		return f;
	}

	int TWS_TLServer::UnknownMessage(int MessageType, CString msg)
	{
		return UNKNOWN_MESSAGE;
	}

	OrderId TWS_TLServer::TL2IBID(int64 tlid)
	{
		for (uint i = 0; i<tlorders.size(); i++)
			if (tlorders[i]==tlid)
				return iborders[i];
		return 0;
	}
	int64 TWS_TLServer::IB2TLID(OrderId ibid)
	{
		for (uint i = 0 ; i<iborders.size(); i++)
			if (iborders[i]==ibid)
				return tlorders[i];
		return 0;
	}

	OrderId TWS_TLServer::newOrder(int64 tlid,CString acct)
	{
		if (tlid==0) tlid = GetTickCount(); // if no id, auto-assign one
		OrderId ibid = TL2IBID(tlid);
		if (ibid!=0) return ibid; // id already exists
		ibid = getNextOrderId(acct);
		tlorders.push_back(tlid);
		iborders.push_back(ibid);
		return ibid;
	}

	int TypeFromExchange(CString ex)
	{
		if ((ex=="GLOBEX")|| (ex=="NYMEX")||(ex=="CFE"))
			return FUT;
		else if ((ex=="NYSE")||(ex=="NASDAQ")||(ex=="ARCA"))
			return STK;
		// default to STK if not sure
		return 0;

	}

	vector<int> specsymid;
	vector<CString> specsym;
	const int NOTSPECIAL = -1;
	int specid(int id)
	{
		for (int i = 0; i<(int)specsymid.size(); i++)
			if (specsymid[i]==id) return i;
		return NOTSPECIAL;
	}

	void TWS_TLServer::pcont(Contract* c)
	{
		CString m;
		m.Format("sym: %s local: %s currency: %s ex: %s primaryex: %s security: %s strike: %f expiry: %s",c->symbol,c->localSymbol,c->currency,c->exchange,c->primaryExchange,c->secType,c->strike,c->expiry);
		v(m);
	}

	void TWS_TLServer::pord(Order* o)
	{
		CString m;
		m.Format("size: %i price: %f stop: %f",o->totalQuantity,o->lmtPrice,o->auxPrice);
		v(m);
	}

	int TWS_TLServer::SendOrder(TLOrder o)
	{
		// check our order
		if (o.symbol=="") 
			return UNKNOWN_SYMBOL;
		if (!o.isValid()) 
			return INVALID_ORDERSIZE;
		if (!linktest())
			return BROKERSERVER_NOT_FOUND;


		// create broker-specific objects here
		Order* order(new Order);

		order->auxPrice = o.isTrail() ? o.trail : o.stop;
		order->lmtPrice = o.price;
		order->orderType = (o.isStop()) ? "STP" : (o.isLimit() ? "LMT" : (o.isTrail() ? "TRAIL" : "MKT"));
		order->totalQuantity = (long)abs(o.size);
		order->action = (o.side) ? "BUY" : "SELL";
		order->account = o.account;
		order->tif = o.TIF;
		order->outsideRth = true;
		order->orderId = newOrder(o.id,o.account);
		order->transmit = true;

		Contract* contract(new Contract);
		TLSecurity tmpsec = TLSecurity::Deserialize(o.symbol);
		contract->symbol = o.symbol;
		if (o.symbol.FindOneOf(" ")!=-1)
		{
			contract->symbol = tmpsec.sym;
			// options stuff
			if (tmpsec.isCall()|| tmpsec.isPut())
			{
				CString expire;
				expire.Format("%i",tmpsec.date);
				contract->expiry = expire;
				contract->strike = tmpsec.strike;
				contract->right = tmpsec.details;
				//contract->currency = _currency;			
			}
			else 
				contract->localSymbol = o.localsymbol!="" ? o.localsymbol : tmpsec.sym;
			if (tmpsec.hasDest())
				contract->exchange = tmpsec.dest;
			if (tmpsec.hasType())
				contract->secType = tmpsec.SecurityTypeName(tmpsec.type);
			else if (tmpsec.hasDest())
				contract->secType = TLSecurity::SecurityTypeName(TypeFromExchange(tmpsec.dest));
		}
		else 
		{
			contract->localSymbol = o.localsymbol!="" ? o.localsymbol : o.symbol;
			contract->exchange = o.exchange;
			contract->secType = o.security;

		}
		if (tmpsec.type==CASH)
		{
			// remove base currency from symbol
			CString cpy = CString(o.localsymbol);
			int pidx = cpy.Find(CString("."));
			if (pidx!=-1)
			{

				cpy.Delete(pidx,cpy.GetLength()-pidx);
			}
			contract->symbol = cpy;
			contract->currency = truncateat(o.localsymbol,CString("."));
		}
		else
			contract->currency = o.currency;
		if (contract->exchange=="")
			contract->exchange= "SMART";
		contract->symbol = tl2ibspace(contract->symbol);
		contract->localSymbol = tl2ibspace(contract->localSymbol);
		
		

		// get the TWS session associated with our account
		EClient* client;
		if (o.account=="") // if no account specified, get default
			client = m_link[this->validlinkids[0]];
		else // otherwise get the session our account is logged into
			client	= GetOrderSink(o.account);

		pcont(contract);
		pord(order);

		// place our order
		if (client!=NULL)
			client->placeOrder(order->orderId,*contract,*order);
		

		delete order;
		delete contract;

		return OK;
	}

	int TWS_TLServer::BrokerName(void)
	{
		return InteractiveBrokers;
	}



	bool TWS_TLServer::hasAccount(CString account)
	{
		if (account=="") return false; // don't match empty strings
		for (size_t i = 0; i<accts.size(); i++)
			if (accts[i].Find(account)!=-1) return true;
		return false;
	}


	void TWS_TLServer::updateAccountValue( const CString &key, const CString &val,
										  const CString &currency, const CString &accountName) 
	{
		// make sure we don't have this account already
		if (!hasAccount(accountName))
		{
			accts.push_back(accountName); // save the account name
			// get socket for this account
			EClient* socket = m_link[validlinkids[accts.size()-1]];
			// request updates so we get portfolio data
			socket->reqAccountUpdates(true,accountName);
		}
	}

	void TWS_TLServer::managedAccounts(const CString& accountsList)
	{
		CString msg(accountsList);
		accts.push_back(msg); //save the list of FA accounts
	}

	int TWS_TLServer::AccountResponse(CString clientname)
	{
		std::vector<CString> all;
		for (size_t i = 0; i<accts.size(); i++)
		{
			if (accts[i].Find(",")!=-1) // look for multiple accounts
				all.push_back(accts[i]); // if only one, save it
			else
				gsplit(accts[i],",",all); // otherwise save em all
		}
		CString s = gjoin(all,","); // join em back together
		TLSend(ACCOUNTRESPONSE,s,clientname); // send the whole list
		return OK;
	}

	EClient* TWS_TLServer::GetOrderSink(CString account)
	{
		for (size_t i = 0; i<accts.size(); i++)
			if (accts[i].Find(account)!=-1)
				return m_link[this->validlinkids[i]];
		return NULL;
	}



	void TWS_TLServer::nextValidId( OrderId orderId) 
	{ 
		m_nextorderids.push_back(orderId);
	}

	OrderId TWS_TLServer::getNextOrderId(CString account)
	{
		for (size_t i = 0; i<accts.size(); i++)
			if (accts[i].Find(account)!=-1)
				return m_nextorderids[i]++;
		return m_nextorderids[0]++; // default is first one
	}

	int TWS_TLServer::getMlinkId(OrderId id)
	{
		// find the closest order id to an order and return 
		// the mlink associated with this id

		long mindist = 2000000000; // set large initial minimum
		int mlinkid = 0; // set default mlink
		for (size_t i = 0; i<m_nextorderids.size(); i++)
		{
			long dist = abs(id-m_nextorderids[i]); // get distance
			if (dist<mindist)
			{
				mindist = dist;
				mlinkid = (int)i;
			}
		}
		return mlinkid;
	}




	std::vector<int> ordercache;

	bool newOrder(OrderId id)
	{
		size_t count = ordercache.size();
		bool neworder = false;
		for (size_t i = 0; i< count; i++)
			neworder |= ordercache[i]==id;
		if (!neworder)
		{
			ordercache.push_back(id);
			return true;
		}
		return false;
	}

	void TWS_TLServer::openOrder( OrderId orderId, const Contract& contract,
								const Order& order, const OrderState& orderState)
	{
			// count order
			IncOrderId(order.account);

			// prepare client order and notify client
			TradeLibFast::TLOrder o;
			o.id = IB2TLID(orderId);
			o.side = (order.action=="BUY");
			o.size = abs(order.totalQuantity) * ((o.side) ? 1 : -1);
			o.symbol = contract.localSymbol;
			o.price = (order.orderType=="LMT") ? order.lmtPrice : 0;
			o.stop = (order.orderType=="STP") ? order.auxPrice : 0;
			o.exchange = contract.exchange;
			o.account = order.account;
			o.security = contract.secType;
			o.currency = contract.currency;
			o.localsymbol = contract.localSymbol;
			o.TIF = order.tif;
			std::vector<int> nowtime;
			TLTimeNow(nowtime);
			o.date = nowtime[TLdate];
			o.time = nowtime[TLtime];
			this->SrvGotOrder(o);

	}

	CString  TWS_TLServer::ib2tlspace(CString ibsym)
	{
		if (ibsym.FindOneOf(" ")==-1)
			return ibsym;
		ibsym.Replace(" ","_");
		return ibsym;
	}

	CString TWS_TLServer::tl2ibspace(CString tlsym)
	{
		if (tlsym.FindOneOf("_")==-1)
			return tlsym;
		tlsym.Replace("_"," ");
		return tlsym;
	}

	void TWS_TLServer::IncOrderId(CString account)
	{
		for (size_t i = 0; i<accts.size(); i++)
			if (accts[i].Find(account)!=-1)
			{
				m_nextorderids[i]++;
				return;
			}
		m_nextorderids[0]++;
	}

	int TWS_TLServer::CancelRequest(int64 orderid)
	{
		// gets the IB id
		OrderId ibid = TL2IBID(orderid);
		// gets mlink associated with order
		int mlink = getMlinkId(ibid);
		if (ibid==0) return ORDER_NOT_FOUND;
		CString m;
		m.Format("trying to cancel order: %lld",orderid);
		v(m);
		m_link[this->validlinkids[mlink]]->cancelOrder(ibid);
		return OK;
	}

	void TWS_TLServer::winError( const CString &str, int lastError)
	{
		D(str);
	}
	void TWS_TLServer::error(const int id, const int errorCode, const CString errorString)
	{
		// for some reason IB sends order cancels as an error rather than
		// as an order update message
		int64 tlid = IB2TLID(id);
		CString msg;
		msg.Format("%s [err:%i] [ibid:%i] [tlid:%lld]",errorString,errorCode,id,tlid);
		if (errorCode==202) 
			this->SrvGotCancel(tlid); // cancels
		else if (IGNOREERRORS) return; // ignore errors during init
		else if (errorCode==2109) return; // ignore 'place outside of market hours' error as we set this on every order
		else D(msg); // print other errors
	}



	void TWS_TLServer::execDetails( OrderId orderId, const Contract& contract, const Execution& execution) 
	{ 
		// convert to a tradelink trade
		TLTrade trade;
		trade.currency = contract.currency;
		trade.account = execution.acctNumber;
		trade.exchange = contract.exchange;
		trade.id = IB2TLID(orderId);
		trade.xprice = execution.price;
		trade.xsize = execution.shares;
		trade.side = execution.side=="BOT";
		trade.security = contract.secType;
		if (trade.security==CString("OPT"))
		{
			CString m;
			m.Format("%s %s %s %f OPT",contract.symbol,contract.expiry,(contract.right==CString("C")) ? "CALL" : "PUT",contract.strike);
			trade.symbol = m;
		}
		else
		{
			trade.localsymbol = contract.localSymbol;
			trade.symbol = contract.localSymbol;
		}


		// convert date and time
		std::vector<CString> r;
		std::vector<CString> r2;
		gsplit(execution.time," ",r);
		gsplit(r[2],":",r2);
		int sec = atoi(r2[2]);
		trade.xdate = atoi(r[0]);
		trade.xtime = (atoi(r2[0])*10000)+(atoi(r2[1])*100)+sec;
		this->SrvGotFill(trade);

	}


	bool TWS_TLServer::hasTicker(CString symbol)
	{
		for (size_t i = 0; i<stockticks.size(); i++)
			if (stockticks[i].sym==symbol) return true;
		return false;
	}

	bool TWS_TLServer::linktest()
	{
		if (validlinkids.size()>0)
			return true;
		D("Operation failed, no TWS instances running.");
		return false;
	}

	int TWS_TLServer::RegisterStocks(CString clientname)
	{
		// make sure base function is called
		TLServer_WM::RegisterStocks(clientname);
		// get client id so we can find his symbols
		int cid = this->FindClient(clientname);  
		// make sure at least one TWS is running
		if (!linktest())
			return BROKERSERVER_NOT_FOUND;

		// loop through every stock for this client
		for (unsigned int i = 0; i<stocks[cid].size(); i++)
		{
			// if we already have a subscription to this stock, proceed to next one
			if (hasTicker(stocks[cid][i])) continue;
			// get symbol
			TLSecurity sec = TLSecurity::Deserialize(stocks[cid][i]);
			// keep copy of original symbol
			CString lsym = CString(sec.sym);
			sec.sym = tl2ibspace(sec.sym);

			// otherwise, subscribe to this stock and save it to subscribed list of tickers
			Contract contract;
			
			// if option, pass options parameters
			if (sec.isCall() || sec.isPut())
			{
				contract.symbol = sec.sym;
				contract.right = sec.details;
				CString expire;
				expire.Format("%i",sec.date);
				contract.expiry = expire;
				contract.strike = sec.strike;
				if (!sec.hasDest())
					contract.exchange = "SMART";
			}
			else // set local symbol to symbol
			{
				contract.localSymbol = sec.sym;

			}

			// if destination specified use it
			if (sec.hasDest())
				contract.exchange = sec.dest;
			// if we have a destination but no type, try to guess type
			if (!sec.hasType() && sec.hasDest())
				sec.type = TypeFromExchange(contract.exchange);
			// if we still don't have a type, use stock
			if (!sec.hasType())
				sec.type = STK;
			// if we have a stock and it has no destination, use default
			if ((sec.type==STK) && !sec.hasDest())
				contract.exchange = "SMART";
			if (sec.type==CASH)
				contract.currency = truncateat(sec.sym,CString("."));
			else
				contract.currency = _currency;
			contract.secType = TLSecurity::SecurityTypeName(sec.type);
			v(CString("attempting to add symbol"));
			pcont(&contract);
			this->m_link[this->validlinkids[0]]->reqMktData((TickerId)stockticks.size(),contract,"",false);
			TLTick k; // create blank tick
			k.sym = stocks[cid][i]; // store long symbol
			stockticks.push_back(k);
			v(CString("Added IB subscription for ")+CString(sec.sym));

		}
		return OK;

	}

	CString TWS_TLServer::truncateat(CString original,CString after)
	{
			CString cpy = CString(original);
			int pidx = cpy.Find(after);
			if (pidx!=-1)
			{
				cpy.Delete(0,cpy.GetLength()-pidx);				
			}
			return cpy;
	}

	CString TWS_TLServer::truncatebefore(CString original,CString before)
	{
			CString cpy = CString(original);
			int pidx = cpy.Find(before);
			if (pidx!=-1)
			{
				cpy.Delete(pidx,cpy.GetLength()-pidx);				
			}
			return cpy;
	}



	void TWS_TLServer::tickPrice( TickerId tickerId, TickType tickType, double price, int canAutoExecute) 
	{ 
		if ((tickerId>=0)&&(tickerId<(TickerId)stockticks.size()) && needStock(stockticks[tickerId].sym))
		{
			time_t now;
			time(&now);
			CTime ct(now);
			TLTick k;
			k.date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();
			k.time = (ct.GetHour()*10000)+(ct.GetMinute()*100)+ct.GetSecond();
			k.sym = stockticks[tickerId].sym;
			if (tickType==LAST)
			{
				stockticks[tickerId].trade = price;
				k.trade = price;
				k.size = stockticks[tickerId].size;
			}
			else if (tickType==BID)
			{
				stockticks[tickerId].bid = price;
				k.bid = stockticks[tickerId].bid;
				k.bs = stockticks[tickerId].bs;
			}
			else if (tickType==ASK)
			{
				stockticks[tickerId].ask = price;
				k.ask = stockticks[tickerId].ask;
				k.os = stockticks[tickerId].os;
			}
			else return; // not relevant tick info
			if (k.isValid() && needStock(k.sym))
				this->SrvGotTick(k);
		}
	
	}
	void TWS_TLServer::tickSize( TickerId tickerId, TickType tickType, int size) 
	{ 
		if ((tickerId>=0)&&(tickerId<(TickerId)stockticks.size()) && needStock(stockticks[tickerId].sym))
		{
			time_t now;
			time(&now);
			CTime ct(now);
			TLTick k;
			k.date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();
			k.time = (ct.GetHour()*10000)+(ct.GetMinute()*100) + ct.GetSecond();
			k.sym = stockticks[tickerId].sym;
			TLSecurity sec = TLSecurity::Deserialize(k.sym);
			bool hundrednorm = (sec.type== STK) || (sec.type== NIL );
			if (tickType==LAST_SIZE)
			{
				stockticks[tickerId].size = hundrednorm ? size*100 : size;
				k.trade = stockticks[tickerId].trade;
				k.size = stockticks[tickerId].size;
			}
			else if (tickType==BID_SIZE)
			{
				stockticks[tickerId].bs = size;
				k.bid = stockticks[tickerId].bid;
				k.bs = stockticks[tickerId].bs;
			}
			else if (tickType==ASK_SIZE)
			{
				stockticks[tickerId].os = size;
				k.ask= stockticks[tickerId].ask;
				k.os = stockticks[tickerId].os;
			}
			else return; // not relevant tick info
			if (k.isValid() && needStock(k.sym))
				this->SrvGotTick(k);
		}


	}

	

	bool TWS_TLServer::havepos(TLPosition pos)
	{
		for (int i = 0; i<(int)poslist.size(); i++)
			if ((poslist[i].Symbol==pos.Symbol))
				return true;
		return false;
	}

	void TWS_TLServer::updatePortfolio( const Contract& contract, int position,
		double marketPrice, double marketValue, double averageCost,
		double unrealizedPNL, double realizedPNL, const CString &accountName) 
	{ 
		TLPosition pos;
		if (contract.secType!=CString("OPT"))
			pos.Symbol = contract.localSymbol;
		else
		{
			CString m;
			m.Format("%s %s %s %f OPT",contract.symbol,contract.expiry,(contract.right==CString("C")) ? "CALL" : "PUT",contract.strike);
			pos.Symbol = m;
		}
		pos.Size = position;
		pos.AvgPrice = marketPrice;
		pos.ClosedPL = realizedPNL;
		if (!havepos(pos))
			poslist.push_back(pos);

	}

	int TWS_TLServer::PositionResponse(CString account, CString clientname)
	{
		for (int i = 0; i<(int)poslist.size(); i++)
				TLSend(POSITIONRESPONSE,poslist[i].Serialize(),clientname);
		return OK;
	}



	void TWS_TLServer::updateMktDepthL2( TickerId id, int position, CString marketMaker, int operation, 
			int side, double price, int size) 
	{ 
		// add DOM support here
	}




	void TWS_TLServer::tickOptionComputation( TickerId ddeId, TickType field, double impliedVol,
		double delta, double modelPrice, double pvDividend) { }
	void TWS_TLServer::tickGeneric(TickerId tickerId, TickType tickType, double value) { }
	void TWS_TLServer::tickString(TickerId tickerId, TickType tickType, const CString& value) { }
	void TWS_TLServer::tickEFP(TickerId tickerId, TickType tickType, double basisPoints,
		const CString& formattedBasisPoints, double totalDividends, int holdDays,
		const CString& futureExpiry, double dividendImpact, double dividendsToExpiry) { }
	void TWS_TLServer::orderStatus( OrderId orderId, const CString &status, int filled, int remaining, 
		double avgFillPrice, int permId, int parentId, double lastFillPrice,
		int clientId, const CString& whyHeld) { }
	void TWS_TLServer::connectionClosed() { D("TWS connection closed.");}

	void TWS_TLServer::updateAccountTime(const CString &timeStamp) { }
	void TWS_TLServer::contractDetails( int reqId, const ContractDetails& contractDetails) {}
	void TWS_TLServer::bondContractDetails( int reqId, const ContractDetails& contractDetails) {}
	void TWS_TLServer::contractDetailsEnd( int reqId) {}
	void TWS_TLServer::updateMktDepth( TickerId id, int position, int operation, int side, 
			double price, int size) { }
	void TWS_TLServer::updateNewsBulletin(int msgId, int msgType, const CString& newsMessage, const CString& originExch) { }
	void TWS_TLServer::receiveFA(faDataType pFaDataType, const CString& cxml) { }
	void TWS_TLServer::historicalData(TickerId reqId, const CString& date, double open, double high, double low,
								   double close, int volume, int barCount, double WAP, int hasGaps) {}
	void TWS_TLServer::scannerParameters(const CString &xml) { }
	void TWS_TLServer::scannerData(int reqId, int rank, const ContractDetails &contractDetails, const CString &distance,
		const CString &benchmark, const CString &projection, const CString &legsStr) { }
	void TWS_TLServer::scannerDataEnd(int reqId) { }
	void TWS_TLServer::realtimeBar(TickerId reqId, long time, double open, double high, double low, double close,
	   long volume, double wap, int count) { }
	void TWS_TLServer::currentTime(long time) {}
	void TWS_TLServer::fundamentalData(TickerId reqId, const CString& data) {}
}







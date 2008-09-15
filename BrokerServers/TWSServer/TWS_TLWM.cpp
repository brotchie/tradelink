#include "StdAfx.h"
#include "TWS_TLWM.h"
#include "Util.h"
#include "Execution.h"
#include <fstream>
#include "IBUtil.h"

namespace TradeLinkServer
{
	const char* CONFIGFILE = "TwsServer.Config.txt";
	const char* LINE = "-----------------------------------------------------";

	TWS_TLWM::TWS_TLWM(void)
	{
		IGNOREERRORS = false;

	}



	TWS_TLWM::~TWS_TLWM(void)
	{
		// cancel subscriptions
		for (size_t i = 0; i<stocktickers.size(); i++)
			this->m_link[this->validlinkids[0]]->cancelMktData((TickerId)i);
		// disconnect from IB and unallocate memory
		for (size_t i = 0; i<m_link.size(); i++)
		{
			m_link[i]->eDisconnect();
			delete m_link[i];
		}

	}

	void TWS_TLWM::Start(void)
	{
		TradeLink_WM::Start();

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
		file.close();
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
		msg.Format("Found %i of %i TWS instances.",this->validlinkids.size(),maxsockets);
		D(msg);
		D(CString("For more instances, change value in: ")+CONFIGFILE);
		msg.Format("Found accounts: %s",gjoin(accts,","));
		D(msg);
	}

	void TWS_TLWM::InitSockets(int maxsockets, int clientid)
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
			else D(CString("Noting found at")+msg);
			m_nextsocket++;
		}
		IGNOREERRORS = false;
	}

	std::vector<int> TWS_TLWM::GetFeatures()
	{
		std::vector<int> f;
		f.push_back(SENDORDER);
		f.push_back(BROKERNAME);
		f.push_back(REGISTERSTOCK);
		f.push_back(ACCOUNTREQUEST);
		f.push_back(ACCOUNTRESPONSE);
		f.push_back(ORDERCANCELREQUEST);
		f.push_back(ORDERCANCELRESPONSE);
		f.push_back(ORDERNOTIFY);
		f.push_back(TRADENOTIFY);
		f.push_back(TICKNOTIFY);
		return f;
	}



	int TWS_TLWM::SendOrder(TLOrder o)
	{
		// check our order
		if (!o.isValid()) return GOTNULLORDER;
		if (o.symbol=="") return UNKNOWNSYM;

		// create broker-specific objects here
		Order* order(new Order);
		order->auxPrice = o.stop;
		order->lmtPrice = o.price;
		order->orderType = (o.stop!=0) ? "STP" : (o.price!=0 ? "LMT" : "MKT");
		order->totalQuantity = (long)o.size;
		order->action = (o.side) ? "BUY" : "SELL";
		order->account = o.account;
		order->tif = o.TIF;
		order->outsideRth = true;
		if (o.id!=0) // if ID is provided, keep it
			order->orderId = o.id;
		else // otherwise just get the next id
			order->orderId = getNextOrderId(o.account);
		order->transmit = true;

		Contract* contract(new Contract);
		contract->symbol = o.symbol;
		contract->localSymbol = o.localsymbol;
		if (o.exchange=="")
			o.exchange = "SMART";
		contract->exchange = o.exchange;
		contract->secType = o.security;
		contract->currency = o.currency;

		// get the TWS session associated with our account
		EClient* client;
		if (o.account=="") // if no account specified, get default
			client = m_link[this->validlinkids[0]];
		else // otherwise get the session our account is logged into
			client	= GetOrderSink(o.account);

		// place our order
		if (client!=NULL)
			client->placeOrder(order->orderId,*contract,*order);

		delete order;
		delete contract;

		return OK;
	}

	int TWS_TLWM::BrokerName(void)
	{
		return InteractiveBrokers;
	}



	bool TWS_TLWM::hasAccount(CString account)
	{
		if (account=="") return false; // don't match empty strings
		for (size_t i = 0; i<accts.size(); i++)
			if (accts[i].Find(account)!=-1) return true;
		return false;
	}


	void TWS_TLWM::updateAccountValue( const CString &key, const CString &val,
										  const CString &currency, const CString &accountName) 
	{
		// make sure we don't have this account already
		if (!hasAccount(accountName))
		{
			accts.push_back(accountName); // save the account name
		}
	}

	void TWS_TLWM::managedAccounts(const CString& accountsList)
	{
		CString msg(accountsList);
		accts.push_back(msg); //save the list of FA accounts
	}

	int TWS_TLWM::AccountResponse(CString clientname)
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
		SendMsg(ACCOUNTRESPONSE,s,clientname); // send the whole list
		return OK;
	}

	EClient* TWS_TLWM::GetOrderSink(CString account)
	{
		for (size_t i = 0; i<accts.size(); i++)
			if (accts[i].Find(account)!=-1)
				return m_link[this->validlinkids[i]];
		return NULL;
	}



	void TWS_TLWM::nextValidId( OrderId orderId) 
	{ 
		m_nextorderids.push_back(orderId);
	}

	OrderId TWS_TLWM::getNextOrderId(CString account)
	{
		for (size_t i = 0; i<accts.size(); i++)
			if (accts[i].Find(account)!=-1)
				return m_nextorderids[i]++;
		return m_nextorderids[0]++; // default is first one
	}

	int TWS_TLWM::getMlinkId(OrderId id)
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

	void TWS_TLWM::openOrder( OrderId orderId, const Contract& contract,
								const Order& order, const OrderState& orderState)
	{

			TradeLinkServer::TLOrder o;
			o.id = orderId;
			o.side = (order.action=="BUY");
			o.size = abs(order.totalQuantity) * ((o.side) ? 1 : -1);
			o.symbol = contract.symbol;
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
			o.sec = nowtime[TLsec];
			this->SrvGotOrder(o);
	}

	void TWS_TLWM::SrvCancelRequest(OrderId orderid)
	{
		// gets mlink associated with order
		int mlink = getMlinkId(orderid); 
		m_link[this->validlinkids[mlink]]->cancelOrder(orderid);
	}

	void TWS_TLWM::winError( const CString &str, int lastError)
	{
		D(str);
	}
	void TWS_TLWM::error(const int id, const int errorCode, const CString errorString)
	{
		// for some reason IB sends order cancels as an error rather than
		// as an order update message
		if (errorCode==202) 
			this->SrvCancelNotify(id); // cancels
		else if (IGNOREERRORS) return; // ignore errors during init
		else D(errorString); // print other errors
	}

	void TWS_TLWM::execDetails( OrderId orderId, const Contract& contract, const Execution& execution) 
	{ 
		// convert to a tradelink trade
		TLTrade trade;
		trade.currency = contract.currency;
		trade.account = execution.acctNumber;
		trade.exchange = contract.exchange;
		trade.id = orderId;
		trade.localsymbol = contract.localSymbol;
		trade.xprice = execution.price;
		trade.xsize = execution.shares;
		trade.symbol = contract.localSymbol;
		trade.side = execution.side=="BOT";

		// convert date and time
		std::vector<CString> r;
		std::vector<CString> r2;
		gsplit(execution.time," ",r);
		gsplit(r[2],":",r2);
		trade.xdate = atoi(r[0]);
		trade.xtime = (atoi(r2[0])*100)+atoi(r2[1]);
		trade.xsec = atoi(r2[2]);
		this->SrvGotFill(trade);

	}


	bool TWS_TLWM::hasTicker(CString symbol)
	{
		for (size_t i = 0; i<stocktickers.size(); i++)
			if (stocktickers[i]==symbol) return true;
		return false;
	}

	int TWS_TLWM::RegisterStocks(CString clientname)
	{
		TradeLink_WM::RegisterStocks(clientname);
		int cid = this->FindClient(clientname);  // get client id so we can find his stocks
		// loop through every stock for this client
		for (size_t i = 0; i<stocks[cid].size(); i++)
		{
			// if we already have a subscription to this stock, proceed to next one
			TLSecurity sec = TLSecurity::Deserialize(stocks[cid][i]);
			if (hasTicker(sec.sym)) continue;
			// otherwise, subscribe to this stock and save it to subscribed list of tickers
			Contract contract;
			contract.localSymbol = sec.sym;
			if (sec.hasDest())
				contract.exchange = sec.dest;
			else if ((sec.type==STK) && (sec.sym.GetLength()>3))
				contract.exchange = "SMART";
			else if (sec.type==STK)
				contract.exchange = "NYSE";
			contract.secType = TLSecurity::SecurityTypeName(sec.type);
			this->m_link[this->validlinkids[0]]->reqMktData(stocktickers.size(),contract,"",false);
			stocktickers.push_back(sec.sym);
			TLTick k; // create blank tick
			stockticks.push_back(k);
			D(CString("Added IB subscription for ")+CString(sec.sym));
		}
		return OK;

	}

	void TWS_TLWM::tickPrice( TickerId tickerId, TickType tickType, double price, int canAutoExecute) 
	{ 
		if ((tickerId>-1)&&(tickerId<stocktickers.size()) && needStock(stocktickers[tickerId]))
		{
			time_t now;
			time(&now);
			CTime ct(now);
			TLTick k;
			k.date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();
			k.time = (ct.GetHour()*100)+ct.GetMinute();
			k.sec = ct.GetSecond();
			k.sym = stocktickers[tickerId];
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
	void TWS_TLWM::tickSize( TickerId tickerId, TickType tickType, int size) 
	{ 
		if ((tickerId>-1)&&(tickerId<stocktickers.size()) && needStock(stocktickers[tickerId]))
		{
			time_t now;
			time(&now);
			CTime ct(now);
			TLTick k;
			k.date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();
			k.time = (ct.GetHour()*100)+ct.GetMinute();
			k.sec = ct.GetSecond();
			k.sym = stocktickers[tickerId];
			if (tickType==LAST_SIZE)
			{
				stockticks[tickerId].size = size*100;
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
	void TWS_TLWM::tickOptionComputation( TickerId ddeId, TickType field, double impliedVol,
		double delta, double modelPrice, double pvDividend) { }
	void TWS_TLWM::tickGeneric(TickerId tickerId, TickType tickType, double value) { }
	void TWS_TLWM::tickString(TickerId tickerId, TickType tickType, const CString& value) { }
	void TWS_TLWM::tickEFP(TickerId tickerId, TickType tickType, double basisPoints,
		const CString& formattedBasisPoints, double totalDividends, int holdDays,
		const CString& futureExpiry, double dividendImpact, double dividendsToExpiry) { }
	void TWS_TLWM::orderStatus( OrderId orderId, const CString &status, int filled, int remaining, 
		double avgFillPrice, int permId, int parentId, double lastFillPrice,
		int clientId, const CString& whyHeld) { }
	void TWS_TLWM::connectionClosed() { D("TWS connection closed.");}

	void TWS_TLWM::updatePortfolio( const Contract& contract, int position,
		double marketPrice, double marketValue, double averageCost,
		double unrealizedPNL, double realizedPNL, const CString &accountName) { }
	void TWS_TLWM::updateAccountTime(const CString &timeStamp) { }
	void TWS_TLWM::contractDetails( int reqId, const ContractDetails& contractDetails) {}
	void TWS_TLWM::bondContractDetails( int reqId, const ContractDetails& contractDetails) {}
	void TWS_TLWM::contractDetailsEnd( int reqId) {}
	void TWS_TLWM::updateMktDepth( TickerId id, int position, int operation, int side, 
			double price, int size) { }
	void TWS_TLWM::updateMktDepthL2( TickerId id, int position, CString marketMaker, int operation, 
			int side, double price, int size) { }
	void TWS_TLWM::updateNewsBulletin(int msgId, int msgType, const CString& newsMessage, const CString& originExch) { }
	void TWS_TLWM::receiveFA(faDataType pFaDataType, const CString& cxml) { }
	void TWS_TLWM::historicalData(TickerId reqId, const CString& date, double open, double high, double low,
								   double close, int volume, int barCount, double WAP, int hasGaps) {}
	void TWS_TLWM::scannerParameters(const CString &xml) { }
	void TWS_TLWM::scannerData(int reqId, int rank, const ContractDetails &contractDetails, const CString &distance,
		const CString &benchmark, const CString &projection, const CString &legsStr) { }
	void TWS_TLWM::scannerDataEnd(int reqId) { }
	void TWS_TLWM::realtimeBar(TickerId reqId, long time, double open, double high, double low, double close,
	   long volume, double wap, int count) { }
	void TWS_TLWM::currentTime(long time) {}
	void TWS_TLWM::fundamentalData(TickerId reqId, const CString& data) {}
}







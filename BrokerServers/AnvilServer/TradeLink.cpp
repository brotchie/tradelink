
#include "stdafx.h"
#include "Resource.h"
#include "TradeLink.h"
#include "SendOrderDlg.h"
#include "ExtFrame.h"
#include "time.h"
#include "TLStock.h"
#include "SendMsg.h"
#include "TLOrder.h"
#include "AnvilUtil.h"
#include "Monitor.h"


// TLIdx message handlers

void TLIdx::Load(CString symbol)
{
    if(symbol.GetLength() != 0)
    {
        MarketIndex* index = B_FindIndex(symbol);
        if(m_index != index)
        {
            if(m_index)
            {
                m_index->Remove(this);
            }
            m_index = index;
            if(m_index)
            {
				m_symbol = m_index->GetSymbol();
				m_index->Add(this);
                FillInfo();
            }
		}
	}
	else m_symbol = "";
}


TLIdx::TLIdx(CString symbol)
{
	m_index = NULL;
	m_symbol = "";
	Load(symbol);

}

void TLIdx::OnChangeIndexSymbol() 
{
}



void gsplit(CString msg, CString del, std::vector<CString>& rec)
{
	while (msg.FindOneOf(del)!=-1)
	{
		int pos = msg.FindOneOf(del);
		CString r = msg.Left(pos);
		rec.push_back(r);
		msg = msg.Right(msg.GetLength()-(pos+1));
	}
	if (msg.GetLength()>0)
		rec.push_back(msg);
}

CString gjoin(std::vector<CString>& vec, CString del)
{
	CString s(_T(""));
	for (size_t i = 0; i<vec.size(); i++)
			s += vec[i] + del;
	s.TrimRight(del);
	return s;
}

std::vector <CString>client;
std::vector <time_t>heart;
typedef std::vector <CString> mystocklist;
std::vector < mystocklist > stocks;
std::vector <TLStock*> subs; // this saves our subscriptions with the hammer server
std::vector <TLIdx*> idxs; // saves our indicies with hammer



void TLUnload()
{
	client.clear();
	heart.clear();
	stocks.clear();
	for (size_t i = 0; i<subs.size(); i++) subs[i]->Clear();
	for (size_t i = 0; i<idxs.size(); i++) idxs[i]->Clear();
	subs.clear();
	idxs.clear();
}

void TLIdx::FillInfo()
{
	
    CString tick = m_index->GetSymbol();
	tick.AppendChar(',');
	time_t now;
	CTime ct(time(&now));
	int xd = (ct.GetYear()*10000)+(ct.GetMonth()*100)+ct.GetDay();
	int xt = (ct.GetHour()*100)+ct.GetMinute();
	CString xdate;
	xdate.Format(_T("%i,"),xd);
	CString xtime;
	xtime.Format(_T ("%i,"),xt);
	tick.Append(xdate);
	tick.Append(xtime);
	CString str;
	ExtFrame::FormatMoney(str, m_index->GetValue());
	tick.Append(str);
    str = "";
    ExtFrame::FormatMoney(str, m_index->GetOpenValue());
	tick.AppendChar(',');
	tick.Append(str);
    str = "";
    ExtFrame::FormatMoney(str, m_index->GetHigh());
	tick.AppendChar(',');
	tick.Append(str);
    str = "";
    ExtFrame::FormatMoney(str, m_index->GetLow());
	tick.AppendChar(',');
	tick.Append(str);
    str = "";
    ExtFrame::FormatMoney(str, m_index->GetCloseValue());
	tick.AppendChar(',');
	tick.Append(str);
	tick.AppendChar(',');

	for (size_t i = 0; i<client.size(); i++)
		SendMsg(TICKNOTIFY,tick,client[i]); // send update to every client
}


void TLIdx::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
    switch(message->GetType())
    {
        case M_NW2_INDEX_DETAILS:
        FillInfo();
        break;
    }
}

void TLIdx::OnDynamicUpdate() 
{
    if(m_index)
    {
            FillInfo();
            m_index->Add(this);
    }
}



void AllClients(std::vector <CString> &clients)
{
	size_t count = client.size();
	for (size_t i = 0; i<count; i++)
		clients.push_back(client[i]);
}

void Subscribers(CString stock, std::vector <CString> &subscriberids)
{
	size_t maxclient = stocks.size();
	for (size_t i = 0; i<maxclient; i++)
	{
		size_t maxstocks = stocks[i].size(); //numstocks for this client
		for (size_t j=0; j<maxstocks; j++) 
			if (stock.CompareNoCase(stocks[i][j])==0) // our stock matches the current stock of this client
			{
				subscriberids.push_back(client[i]); // so we save this client
				break; // no need to process more stocks for this client
			}
	}
}



int FindClient(CString cwind)
{
	size_t len = client.size();
	for (size_t i = 0; i<len; i++) if (client[i]==cwind) return i;
	return -1;
}

LRESULT RegClient(CString m) 
{
	if (FindClient(m)!=-1) return 1;
	client.push_back(m);
	time_t now;
	time(&now);
	heart.push_back(now); // save heartbeat at client index
	mystocklist my = mystocklist(0);
	stocks.push_back(my);
	return 0;
}

LRESULT HeartBeat(CString cwind)
{
	int cid = FindClient(cwind);
	if (cid==-1) return -1;
	time_t now;
	time(&now);
	time_t then = heart[cid];
	double dif = difftime(now,then);
	heart[cid] = now;
	return (LRESULT)dif;
}

int LastBeat(CString cwind)
{
	int cid = FindClient(cwind);
	if (cid==-1) return 6400000;
	time_t now,then;
	time(&now);
	then = heart[cid];
	double dif = difftime(now,then);
	if (then==0) dif = 0;
	return (int)dif;
}

bool hasHammerSub(std::string symbol)
{
	for (size_t i = 0; i<subs.size(); i++) if (symbol == subs[i]->GetSymbol()) return true;
	return false;
}
bool hasHammerIdx(CString symbol)
{
	for (size_t i = 0; i<idxs.size(); i++) if (symbol == idxs[i]->m_symbol) return true;
	return false;
}

LRESULT RegIndex(CString m)
{
	std::vector <CString> rec;
	gsplit(m,"+",rec); // split message body into client id and stock list
	unsigned int cid = FindClient(rec[0]);
	if (cid==-1) return 1; //client not registered
	mystocklist my; // initialize stocklist array to proper length
	gsplit(rec[1],",",my); // populate array from the message
	for (size_t i = 0; i<my.size();i++) // subscribe to stocks
	{
		CString idx = my[i];
		if (hasHammerIdx(idx)) continue; // if already subscribed, go to next index
		TLIdx * newidx = new TLIdx(idx);
		idxs.push_back(newidx);
	}
	return 0;
}


LRESULT RegStocks(CString m)
{ 
	std::vector <CString> rec;
	gsplit(m,"+",rec); // split message body into client id and stock list
	unsigned int cid = FindClient(rec[0]);
	if (cid==-1) return 1; //client not registered
	mystocklist my; // initialize stocklist array to proper length
	gsplit(rec[1],",",my); // populate array from the message
	for (size_t i = 0; i<my.size();i++) // subscribe to stocks
	{
		if (hasHammerSub((std::string)my[i])) continue; // if we've already subscribed once, skip to next stock
		TLStock* stk = new TLStock(my[i]); // create new stock instance
		stk->Load();
		subs.push_back(stk);
	}
	stocks[cid] = my; // index the array by the client's id
	HeartBeat(rec[0]); // update the heartbeat
	return 0;
}

void RemoveSub(CString stock)
{
	for (size_t i = 0; i<subs.size(); i++)
		if (subs[i]->GetSymbol().compare(stock)==0) subs[i]->Clear();
}

void RemoveSubs(CString cwind)
{
	int cid = FindClient(cwind);
	if (cid==-1) return;
	size_t maxstocks = stocks[cid].size();
	for (size_t i =0; i<maxstocks; i++)
	{
		std::vector <CString> ALLSUBS;
		Subscribers(stocks[cid][i],ALLSUBS);
		if ((ALLSUBS.size()==1) // if we only have one subscriber
			&& (ALLSUBS[0].Compare(cwind)==0)) // and it's the one we requested
			RemoveSub(stocks[cid][i]);  // remove it
		// otherwise leave it
	}

}


LRESULT ClearClient(CString m) 
{
	RemoveSubs(m); // remove any subscriptions this stock has that aren't used by others
	int cid = FindClient(m);
	if (cid==-1) return 1;
	client[cid] = "";
	stocks[cid] = mystocklist(0);
	heart[cid] = NULL;
	return 0;
}

LRESULT ClearStocks(CString m)
{
	int cid = FindClient(m);
	if (cid==-1) return 1;
	stocks[cid] = mystocklist(0);
	HeartBeat(m);
	return 0;
}



void TLKillDead(int deathInseconds)
{
	for (size_t i = 0; i<client.size(); i++) 
	{
		int lastbeat = LastBeat(client[i]);
		if ((client[i]!="") && (lastbeat>deathInseconds))
		{
			RemoveSubs(client[i]);
			ClearClient(client[i]);
		}
	}
}

int Sendorder(CString msg) 
{
	if (msg=="") return GOTNULLORDER;

	TradeLinkServer::TLOrder o = TradeLinkServer::TLOrder::Deserialize(msg);
	const StockBase* Stock = B_GetStockHandle(o.symbol);

	Observable* m_account;
	if (o.account=="")
		m_account = B_GetCurrentAccount();
	else 
		m_account = B_GetAccount(o.account.GetBuffer());
	


	//convert the arguments
	Order* orderSent;
	char side = (o.side) ? 'B' : 'S';
	const Money pricem = Money((int)(o.price*1024));
	const Money stopm = Money((int)(o.stop*1024));
	unsigned int mytif = TIFId(o.TIF);

	if (!Stock->isLoaded()) return UNKNOWNSYM;
	
	// send the order (next line is from SendOrderDlg.cpp)
	unsigned int error = B_SendOrder(Stock,
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
	return error;
}

long GetPosF(CString msg, int funct) 
{
	CString m(msg);
	Position* mypos;
	if (m.Find(",",0)==-1)
	{

		const char* symbol = (LPCTSTR)msg;
		Observable* m_account = B_GetCurrentAccount();
		mypos = B_FindPosition(symbol,m_account);
	} 
	else
	{
		std::vector<CString> parse;
		gsplit(m,",",parse);
		Observable* m_account = B_GetAccount(parse[1].GetBuffer());
		mypos = B_FindPosition(parse[0].GetBuffer(),m_account);
	}
	Money& p = Money(0,0);
	long price = 0;
	if (mypos) {
		if (funct==AVGPRICE) p = mypos->GetAveragePrice();
		else if (funct==POSOPENPL) p = mypos->GetOpenPnl();
		else if (funct==POSCLOSEDPL) p = mypos->GetClosedPnl();
		price = MoneyToPacked(p);
	}
	return price;
}

int GetPosI(CString msg, int funct) {
	CString m(msg);
	Position* mypos;
	if (m.Find(",",0)==-1)
	{

		const char* symbol = (LPCTSTR)msg;
		Observable* m_account = B_GetCurrentAccount();
		mypos = B_FindPosition(symbol,m_account);
	} 
	else
	{
		std::vector<CString> parse;
		gsplit(m,",",parse);
		Observable* m_account = B_GetAccount(parse[1].GetBuffer());
		mypos = B_FindPosition(parse[0].GetBuffer(),m_account);
	}
	int s = 0;
	if (mypos) {
		if (funct==POSLONGPENDSHARES) s = mypos->GetSharesPendingLong();
		else if (funct==POSSHORTPENDSHARES) s = mypos->GetSharesPendingShort();
		else if (funct==POSTOTSHARES) s = (int)mypos->GetSharesClosed();
		else if (funct==GETSIZE) s = (int)mypos->GetSize();
	}
	return s;
}

int GetStockI(CString msg, int funct) {
	const char* symbol = (LPCTSTR)msg;
	const StockBase* Stock = B_GetStockHandle(symbol);
	int s = 0;
	if (Stock) {
		if (funct==LASTSIZE) s = Stock->GetLastTradeSize();
	}
	return s;
}

long GetStockF(CString msg, int funct) {
	const char* symbol = (LPCTSTR)msg;
	const StockBase* Stock = B_GetStockHandle(symbol);
	Money& p = Money(0,0);
	long price = 0;
	if (Stock) {
		if (funct==LRPBID) p = Stock->GetLRP(true); // true = bid
		else if (funct==LRPASK) p = Stock->GetLRP(false); // false = ask
		else if (funct==LASTTRADE) p = Stock->GetLastTradePrice();
		else if (funct==NDAYHIGH) p = Stock->GetNyseDayHigh();
		else if (funct==NDAYLOW) p = Stock->GetNyseDayLow();
		else if (funct==OPENPRICE) p = Stock->GetOpenPrice();
		else if (funct==CLOSEPRICE) p = Stock->GetTodaysClosePrice();
		else if (funct==YESTCLOSE) p = Stock->GetClosePrice();
		price = MoneyToPacked(p);
	}
	return price;
}

LRESULT gotAccountRequest(CString client)
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
	CString msg = gjoin(accts,",");
	SendMsg(ACCOUNTRESPONSE,msg,client);
	return OK;
}

CString SerializeIntVec(std::vector<int> input)
{
	std::vector<CString> tmp;
	for (size_t i = 0; i<input.size(); i++)
	{
		CString t; // setup tmp string
		t.Format("%i",input[i]); // convert integer into tmp string
		tmp.push_back(t); // push converted string onto vector
	}
	// join vector and return serialized structure
	return gjoin(tmp,",");
}

CString GetFeatures()
{
	std::vector<int> f;
	f.push_back(BROKERNAME);
	f.push_back(ACCOUNTREQUEST);
	f.push_back(ACCOUNTRESPONSE);
	f.push_back(HEARTBEAT);
	f.push_back(NDAYHIGH);
	f.push_back(NDAYLOW);
	f.push_back(OPENPRICE);
	f.push_back(CLOSEPRICE);
	f.push_back(YESTCLOSE);
	f.push_back(SENDORDER);
	f.push_back(AVGPRICE);
	f.push_back(POSOPENPL);
	f.push_back(POSCLOSEDPL);
	f.push_back(POSLONGPENDSHARES);
	f.push_back(POSSHORTPENDSHARES);
	f.push_back(LRPBID);
	f.push_back(LRPASK);
	f.push_back(POSTOTSHARES);
	f.push_back(LASTTRADE);
	f.push_back(LASTSIZE);
	f.push_back(REGISTERCLIENT);
	f.push_back(REGISTERSTOCK);
	f.push_back(CLEARCLIENT);
	f.push_back(CLEARSTOCKS);
	f.push_back(REGISTERINDEX);
	f.push_back(ACCOUNTOPENPL);
	f.push_back(ACCOUNTCLOSEDPL);
	f.push_back(ORDERCANCELREQUEST);
	f.push_back(ORDERCANCELRESPONSE);
	f.push_back(FEATUREREQUEST);
	f.push_back(FEATURERESPONSE);
	f.push_back(ISSIMULATION);
	f.push_back(TICKNOTIFY);
	f.push_back(TRADENOTIFY);
	f.push_back(ORDERNOTIFY);
	return SerializeIntVec(f);
}

LPARAM ServiceMsg(const int t,CString m) {
	
	// we need an Observer (SendOrderDlg) for stocks that aren't loaded
	if (t==BROKERNAME) return (LRESULT)Assent;
	if (t==ACCOUNTREQUEST) return gotAccountRequest(m);
	if (t==HEARTBEAT) return (LRESULT)HeartBeat(m);
	if (t==NDAYHIGH) return (LRESULT)GetStockF(m,NDAYHIGH);
	if (t==NDAYLOW) return (LRESULT)GetStockF(m,NDAYLOW);
	if (t==OPENPRICE) return (LRESULT)GetStockF(m,OPENPRICE);
	if (t==CLOSEPRICE) return (LRESULT)GetStockF(m,CLOSEPRICE);
	if (t==YESTCLOSE) return (LRESULT)GetStockF(m,YESTCLOSE);
	if (t==SENDORDER) return Sendorder(m);
	if (t==AVGPRICE) return (LRESULT)GetPosF(m,AVGPRICE);
	if (t==POSOPENPL) return (LRESULT)GetPosF(m,POSOPENPL);
	if (t==POSCLOSEDPL) return (LRESULT)GetPosF(m,POSCLOSEDPL);
	if (t==POSLONGPENDSHARES) return GetPosI(m,POSLONGPENDSHARES);
	if (t==POSSHORTPENDSHARES) return GetPosI(m,POSSHORTPENDSHARES);
	if (t==LRPBID) return (LRESULT)GetStockF(m,LRPBID);
	if (t==LRPASK)	return (LRESULT)GetStockF(m,LRPASK);
	if (t==POSTOTSHARES) return (LRESULT)GetPosI(m,POSTOTSHARES);
	if (t==LASTTRADE) return (LRESULT)GetStockF(m,LASTTRADE);
	if (t==LASTSIZE) return (LRESULT)GetStockI(m,LASTSIZE);
	if (t==GETSIZE) return (LRESULT)GetPosI(m,GETSIZE);
	if (t==REGISTERCLIENT) return(LRESULT)RegClient(m);
	if (t==REGISTERSTOCK) return (LRESULT)RegStocks(m);
	if (t==CLEARCLIENT) return (LRESULT)ClearClient(m);
	if (t==CLEARSTOCKS) return (LRESULT)ClearStocks(m);
	if (t==REGISTERINDEX) return (LRESULT)RegIndex(m);
	if (t==ACCOUNTOPENPL)
	{
		if (m.GetLength()>0)
			return MoneyToPacked(B_GetAccountOpenPnl(B_GetAccount((LPCTSTR)m)));
		else 
			return MoneyToPacked(B_GetAccountOpenPnl(B_GetCurrentAccount()));
	}
	if (t==ACCOUNTCLOSEDPL)
	{
		if (m.GetLength()>0)
			return MoneyToPacked(B_GetAccountClosedPnl(B_GetAccount((LPCTSTR)m)));
		else 
			return MoneyToPacked(B_GetAccountClosedPnl(B_GetCurrentAccount()));

	}
	if (t==ORDERCANCELREQUEST) 
	{
		unsigned int TLorderId = strtoul(m.GetBuffer(),NULL,10); //get tradelink's order id
		unsigned int orderId = AnvilId(TLorderId); // get current anvil id from tradelink id
		void* iterator = B_CreateAccountIterator();
		B_StartIteration(iterator);
		Observable* acct;
		// loop through every available account, cancel any matching orders
		while (acct = B_GetNextAccount(iterator)) 
		{
			Order* order = B_FindOrder(orderId,acct);
			if(order)
			{
				order->Cancel();

			}
		}
		B_DestroyIterator(iterator);
		return OK;
	}
	if (t==FEATUREREQUEST)
	{
		SendMsg(FEATURERESPONSE,GetFeatures(),m);
		return OK;
	}

	
	if (t==ISSIMULATION) {
		Observable* m_account = B_GetCurrentAccount();
		BOOL isSim = B_IsAccountSimulation(m_account);
		return (LRESULT)isSim;
	}

	return UNKNOWNMSG;
}
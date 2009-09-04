#include "stdafx.h"
#include "ServerGenesis.h"

using namespace TradeLibFast;

ServerGenesis::ServerGenesis()
{
	//_gt = new GTSess(this);

}

ServerGenesis::~ServerGenesis()
{
	//delete gt;
}

void ServerGenesis::Start()
{
	TLServer_WM::Start();
}

int ServerGenesis::BrokerName()
{
	return Genesis;
}

MMID getexchange(CString exch)
{
	if (exch=="ISLD")
		return METHOD_ISLD;
	if (exch=="ARCA")
		return METHOD_ARCA;
	if (exch=="BRUT")
		return METHOD_BRUT;
	if (exch=="BATS")
		return METHOD_BATS;
	if (exch=="AUTO")
		return METHOD_AUTO;
	if (exch=="HELF")
		return METHOD_HELF;
	return METHOD_HELF;

}

std::vector<int> ServerGenesis::GetFeatures()
{
	std::vector<int> f;
	f.push_back(SENDORDER);
	f.push_back(BROKERNAME);
	//f.push_back(REGISTERCLIENT);
	//f.push_back(REGISTERSTOCK);
	//f.push_back(ACCOUNTREQUEST);
	//f.push_back(ACCOUNTRESPONSE);
	f.push_back(ORDERCANCELREQUEST);
	//f.push_back(ORDERCANCELRESPONSE);
	//f.push_back(ORDERNOTIFY);
	//f.push_back(EXECUTENOTIFY);
	//f.push_back(TICKNOTIFY);
	f.push_back(SENDORDERMARKET);
	f.push_back(SENDORDERLIMIT);
	f.push_back(SENDORDERSTOP);
	//f.push_back(SENDORDERTRAIL);
	//f.push_back(LIVEDATA);
	//f.push_back(LIVETRADING);
	//f.push_back(SIMTRADING);
	//f.push_back(POSITIONRESPONSE);
	//f.push_back(POSITIONREQUEST);
	return f;
}


int ServerGenesis::SendOrder(TradeLibFast::TLOrder o)
{
	/*
	BOOL valid = _sg->IsLoggedIn();
	if(valid == FALSE)
		return BROKERSERVER_NOT_FOUND;
	
	GTStock *pStock = GetStock(o.symbol);
	if(pStock == NULL)
		return BROKERSERVER_NOT_FOUND;

	MMID mmid = getexchange(o.exchange);

	double price = o.isMarket() ? (o.side ? pStock->m_level2.GetBestAskPrice() : pStock->m_level2.GetBestBidPrice() ) : o.price;

	int err = OK;

	GTOrder go;
	go.chSide = o.side ? 'B' : 'S';
	go.dblPrice = o.price;
	go.dblStopLimitPrice = o.stop;
	go.method = mmid;
	go.dwShare = o.size;
	go.nIOIid = o.id;

	err = pStock->PlaceOrder(go);
	
	// save order so it can be canceled later
	if (err==OK)
		m_order.push_back(go);

	return err;
	*/
	return OK;
}



int ServerGenesis::CancelRequest(long id)
{
	//CancelOrder(id);
	return OK;
}



void ServerGenesis::Start(LPCSTR user, LPCSTR pw)
{
	Start();
	//int err = _sg->Login(user,pw);
	//D((err==0 ? CString("Login succeeded.") : CString("Login failed.  Check information.")));
}

void ServerGenesis::Stop()
{
	//_sg->Logout();
	//_sg->TryClose();
}




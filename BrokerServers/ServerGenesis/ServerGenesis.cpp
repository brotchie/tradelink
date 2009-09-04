#include "stdafx.h"
#include "ServerGenesis.h"

using namespace TradeLibFast;

ServerGenesis::ServerGenesis()
{
	gtw = new GTWrap();
	gtw->_sg = this;
}

ServerGenesis::~ServerGenesis()
{
	gtw->_sg = NULL;
	delete gtw;
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
	BOOL valid = gtw->IsLoggedIn();
	if(valid == FALSE)
	{
		D("Session not logged in.");
		return BROKERSERVER_NOT_FOUND;
	}
	
	GTStock *pStock;
	pStock = gtw->GetStock(o.symbol);
	if(pStock == NULL)
	{
		D("Symbol could not be obtained.");
		return BROKERSERVER_NOT_FOUND;
	}

	MMID mmid = getexchange(o.exchange);

	double price;
	price = o.isMarket() ? (o.side ? pStock->m_level2.GetBestAskPrice() : pStock->m_level2.GetBestBidPrice() ) : o.price;

	int err;
	err = OK;

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
}



int ServerGenesis::CancelRequest(long id)
{
	gtw->CancelOrder(id);
	return OK;
}

void ServerGenesis::accounttest()
{
	CString m;
	std::list<std::string> lstAccounts;
	gtw->GetAllAccountName(lstAccounts);
	
	std::list<std::string>::iterator s;
	for (s = lstAccounts.begin(); s != lstAccounts.end(); s++)
		m_accts.push_back((*s).c_str());

	if (m_accts.size()>0)
	{
		m.Format("User %s [ID: %s] has %i accounts",gtw->m_user.szUserName,gtw->m_user.szUserID,lstAccounts.size());
		gtw->SetCurrentAccount(m_accts[0]);
	}
	else
		m.Format("No accounts found.");
	D(m);
}

void ServerGenesis::Start(LPCSTR user, LPCSTR pw)
{
	Start();
	int err;
	err = gtw->Login(user,pw);
	CString m = (err==0 ? CString("Login succeeded.") : CString("Login failed.  Check information."));
	D(m);
	

}

void ServerGenesis::Stop()
{
	gtw->Logout();
	D("Logged out.");
	while(!gtw->CanClose()){
		gtw->TryClose();
		Sleep(0);
	}
	D("Session closed.");
}




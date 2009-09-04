#include "stdafx.h"
#include "ServerGenesis.h"
#include <fstream>

using namespace TradeLibFast;

const char* CONFIGFILE = "GenesisServer.Config.txt";
const char* AUTOFILE = "GenesisServer.Login.txt";
const int MAXSERVERS = 3;
enum GENESISSERVERTYPE
{
	GTEXEC,
	GTQUOTE,
	GTLEVEL2,
};
bool ServerGenesis::LoadConfig()
{
	std::ifstream file;
	file.open(CONFIGFILE);
	std::vector<CString> servers;
	std::vector<int> ports;
	if (file.is_open())
	{
		char skip[100];
		char data[100];
		int i = 0;
		while (i++<MAXSERVERS)
		{
			file.getline(skip,100);
			file.getline(data,100);
			CString tmp = CString(data);
			std::vector<CString> r;
			gsplit(tmp,CString(","),r);
			servers.push_back(r[0]);
			int port = 15805;
			port = atoi(r[1].GetBuffer());
			ports.push_back(port);
		}
		file.close();
	}

	if (servers.size()<MAXSERVERS)
	{
		servers.clear();
		ports.clear();
		for (int i = 0; i<MAXSERVERS; i++)
		{
			servers.push_back("69.64.202.155");
			ports.push_back(15805);
		}
	}

	gtw->m_setting.SetExecAddress(servers[GTEXEC], ports[GTEXEC]);
	gtw->m_setting.SetQuoteAddress(servers[GTQUOTE], ports[GTQUOTE]);
	gtw->m_setting.SetLevel2Address(servers[GTLEVEL2], ports[GTLEVEL2]);
	return true;
}

ServerGenesis::ServerGenesis()
{
	autoattempt = false;
	gtw = new GTWrap();
	LoadConfig();
	gtw->_sg = this;
}

ServerGenesis::~ServerGenesis()
{
	gtw->_sg = NULL;
	delete gtw;
}

bool ServerGenesis::Autologin()
{
	if (autoattempt) return false;
	autoattempt = true;
	std::ifstream file;
	file.open(AUTOFILE);
	if (file.is_open())
	{
		char skip[100];
		char data[100];
		file.getline(skip,100);
		file.getline(data,100);
		un = CString(data);
		file.getline(skip,100);
		file.getline(data,100);
		pw = CString(data);
		file.close();
		if (pw.GetLength()*un.GetLength()>0)
		{
			Start(un,pw);
			return true;
		}
	}
	return false;
}

void ServerGenesis::Start()
{
	TLServer_WM::Start();
	Autologin();
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
		D("Must login before sending orders.");
		return BROKERSERVER_NOT_FOUND;
	}
	
	GTStock *pStock;
	pStock = gtw->GetStock(o.symbol);
	if(pStock == NULL)
	{
		CString m;
		m.Format("Symbol %s could not be loaded.",o.symbol);
		D(m);
		return SYMBOL_NOT_LOADED;
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
		CString js = gjoin(m_accts,CString(","));
		m.Format("User %s [ID: %s] has %i accounts: %s",gtw->m_user.szUserName,gtw->m_user.szUserID,lstAccounts.size(),js);
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
	CString m = (err==0 ? CString("Login succeeded, wait for connection turn-up...") : CString("Login failed.  Check information."));
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
	D("Connection closed.");
}




#include "stdafx.h"
#include "ServerGenesis.h"
#include <fstream>

using namespace TradeLibFast;

const char* CONFIGFILE = "GenesisServer.Config.txt";
const char* AUTOFILE = "GenesisServer.Login.txt";
const int MINSERVERS = 3;
#define METHOD_STOP	MAKE_MMID('S', 'T', 'O', 'P')

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
		while (i++<MINSERVERS)
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

	if (servers.size()<MINSERVERS)
	{
		servers.clear();
		ports.clear();
		servers.push_back("69.64.202.155");
		ports.push_back(15805);
		servers.push_back("69.64.202.156");
		ports.push_back(17811);
		servers.push_back("69.64.202.156");
		ports.push_back(17810);
	}

	gtw->m_setting.SetExecAddress(servers[GTEXEC], ports[GTEXEC]);
	gtw->m_setting.SetQuoteAddress(servers[GTQUOTE], ports[GTQUOTE]);
	gtw->m_setting.SetLevel2Address(servers[GTLEVEL2], ports[GTLEVEL2]);
	return true;
}

	int ServerGenesis::AccountResponse(CString clientname)
	{
		CString s = gjoin(m_accts,","); // join em back together
		TLSend(ACCOUNTRESPONSE,s,clientname); // send the whole list
		return OK;
	}

ServerGenesis::ServerGenesis()
{
	_depth = 0;
	PROGRAM = "GenesisServer";
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

bool ServerGenesis::subscribed(CString sym)
{
	for (uint i = 0; i<m_stk.size(); i++)
		if (m_stk[i]->GetSymbolName()==sym) 
			return true;
	return false;
}

int ServerGenesis::RegisterStocks(CString client)
{
	TLServer_WM::RegisterStocks(client);

	// get client id
	int cid = FindClient(client);

	// loop through every stock for this client
	for (unsigned int i = 0; i<stocks[cid].size(); i++)
	{
		CString sym = stocks[cid][i];
		// make sure we have subscription
		if (!subscribed(sym))
		{
			StkWrap* stk = (StkWrap*)gtw->CreateStock(sym);
			stk->SetupDepth(_depth);
			//stk->tl = this;
			m_stk.push_back(stk);
			CString m;
			m.Format("Added subscription for: %s",sym);
			D(m);
		}
	}
	return OK;
}

void ServerGenesis::SendPosition(int cid,GTOpenPosition& pos)
{
	CString sym = CString(pos.szStock);
	int len = sym.GetLength();
	if ((len>10) || (len==0)) return;
	TLPosition p;
	p.Symbol = CString(pos.szStock);
	p.Size = abs(pos.nOpenShares)*(pos.chOpenSide=='B' ? 1 : -1);
	p.AvgPrice = pos.dblOpenPrice;
	TLSend(POSITIONRESPONSE,p.Serialize(),cid);
}

int ServerGenesis::PositionResponse(CString account, CString client)
{
	GTOpenPosition pos;
	int cid = FindClient(client);
	if (cid==-1) return CLIENTNOTREGISTERED;
	const char * acct = (account=="") ? NULL : account.GetBuffer();
	void *it = gtw->GetFirstOpenPosition(acct,pos);
	SendPosition(cid,pos);
	while (it!=NULL)
	{
		it = gtw->GetNextOpenPosition(acct,it,pos);
		SendPosition(cid,pos);
	}
	return OK;
}

int ServerGenesis::BrokerName()
{
	return Genesis;
}

MMID getmethod(CString exch)
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
	return METHOD_ISLD;

}

MMID getplace(CString exch)
{
	if (exch=="ISLD")
		return MMID_ISLD;
	if (exch=="ARCA")
		return MMID_ARCA;
	if (exch=="MSE")
		return MMID_MSE;
	if (exch=="BATS")
		return MMID_BATS;
	if (exch=="NASD")
		return MMID_NASD;
	if (exch=="NYSE")
		return MMID_DOTI;
	return MMID_DOTI;

}

long getTIF(CString tif)
{
	if (tif=="DAY")
		return TIF_DAY;
	if (tif=="IOC")
		return TIF_IOC;
	if (tif=="GTC")
		return TIF_MGTC;
	return TIF_DAY;
}

char getPI(TLOrder o)
{
	if (o.isLimit() && o.isStop())
		return PRICE_INDICATOR_STOPLIMIT;
	else if (o.isLimit())
		return PRICE_INDICATOR_LIMIT;
	else if (o.isStop())
		return PRICE_INDICATOR_STOP;
	return PRICE_INDICATOR_MARKET;

}

int ServerGenesis::DOMRequest(int depth)
{
	_depth = depth;
	if (_depth>SENDDEPTH)
		_depth = SENDDEPTH;
	return OK;
}

std::vector<int> ServerGenesis::GetFeatures()
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
	f.push_back(EXECUTENOTIFY);
	f.push_back(TICKNOTIFY);
	f.push_back(SENDORDERMARKET);
	f.push_back(SENDORDERLIMIT);
	f.push_back(SENDORDERSTOP);
	f.push_back(DOMREQUEST);
	//f.push_back(SENDORDERTRAIL);
	f.push_back(LIVEDATA);
	f.push_back(LIVETRADING);
	f.push_back(SIMTRADING);
	f.push_back(POSITIONRESPONSE);
	f.push_back(POSITIONREQUEST);
	return f;
}


int ServerGenesis::SendOrder(TradeLibFast::TLOrder o)
{
	// ensure logged in
	BOOL valid = gtw->IsLoggedIn();
	if(valid == FALSE)
	{
		D("Must login before sending orders.");
		return BROKERSERVER_NOT_FOUND;
	}
	// make sure account is default or valid
	bool accountok = (o.account=="") && (m_accts.size()!=0);
	for (uint i = 0; i<m_accts.size(); i++)
		accountok |= (m_accts[i]==o.account) ;
	if (!accountok)
	{
		CString m;
		m.Format("Account %s unavailable.   Are you logged in?",o.account);
		D(m);
		return INVALID_ACCOUNT;
	}
	// set requested account, otherwise use whatever was discovered
	if (o.account!="")
		gtw->SetCurrentAccount(o.account);
	// make sure symbol is loaded
	GTStock *pStock;
	pStock = gtw->GetStock(o.symbol);
	if(pStock == NULL)
	{
		gtw->CreateStock(o.symbol);
		pStock = gtw->GetStock(o.symbol);
		if (pStock==NULL)
		{
			CString m;
			m.Format("Symbol %s could not be loaded.",o.symbol);
			D(m);
			return SYMBOL_NOT_LOADED;
		}
	}

	// ensure id is unique
	if (o.id==0) // if id is zero, we auto-assign the id
	{
		vector<int> now;
		int id = GetTickCount();
		while (!IdIsUnique(id))
		{
			if (id<2) id = 4000000000;
			id--;
		}
		o.id = id;
	}
	// get genesis order id
	DWORD goid = 0;
	// ensure that our id is unique in the DWORD
	if (o.id>4294967295)
	{
		goid = GetTickCount();
		while (!IdIsUnique(goid))
		{
			if (goid<2) goid = 4000000000;
			goid--;
		}
	}
	else
		goid = o.id;
	// prepare to track order
	// first... save our id
	orderids.push_back(o.id);
	// save genesis relationship to our id
	gorderids.push_back(goid);
	// with same index, store unknown sequence id we get on order ack
	orderseq.push_back(0);
	// with same index, store unknown trade id we get on fill
	orderticket.push_back(0);

	// place order
	int err = OK;
	GTOrder go;
	go = pStock->m_defOrder;
	go.dwUserData = goid;
	if (o.isStop())
	{
		if(pStock->PlaceOrder(go, (o.side ? 'B' : 'S'), abs(o.size), o.price, METHOD_STOP, getTIF(o.TIF))==0)
		{
			go.chPriceIndicator = getPI(o);
			go.dblStopLimitPrice = o.stop;
			//go.place = getplace(o.exchange);
			err = pStock->PlaceOrder(go);
		}
	}
	else
	{
		if(pStock->PlaceOrder(go, (o.side ? 'B' : 'S'), abs(o.size), o.price, getmethod(o.exchange), getTIF(o.TIF))==0)
		{
			go.chPriceIndicator = getPI(o);
			go.dblStopLimitPrice = o.stop;
			//go.place = getplace(o.exchange);
			err = pStock->PlaceOrder(go);
		}
	}
	
	// save order so it can be canceled later
	if (err==OK)
	{
		return OK;
	}
	CString msg;
	msg.Format("sendorder error: %i",err);
	D(msg);
	return UNKNOWN_ERROR;
}

int ServerGenesis::GetIDIndex(int64 id, int type)
{
	switch (type)
	{
	case ID :
		for (uint i = 0; i<orderids.size(); i++)
			if (orderids[i]==id) return i;
		break;
	case SEQ:
		for (uint i = 0; i<orderseq.size(); i++)
			if (orderseq[i]==id) return i;
		break;
	case TICKET:
		for (uint i = 0; i<orderseq.size(); i++)
			if (orderticket[i]==id) return i;
		break;
	case GENESIS:
		for (uint i = 0; i<gorderids.size(); i++)
			if (gorderids[i]==id) return i;
		break;
	}
	// not found
	return NO_ID;
}

bool ServerGenesis::IdIsUnique(int64 id)
{
	for (uint i = 0; i<orderids.size(); i++)
		if (orderids[i]==id) 
			return false;
	return true;
}

int ServerGenesis::CancelRequest(int64 id)
{
	int idx = GetIDIndex(id,ID);
	if (idx==NO_ID) 
		return OK;
	long ticket = orderticket[idx];
	gtw->CancelOrder(ticket);
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
		m.Format("Current account set to: %s",m_accts[0]);
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
	for (uint i = 0; i<m_stk.size(); i++)
		m_stk[i] = NULL;
	m_stk.clear();
	gtw->DeleteAllStocks();
	gtw->Logout();
	D("Logged out.");
	while(!gtw->CanClose()){
		gtw->TryClose();
		Sleep(0);
	}
	D("Connection closed.");
}




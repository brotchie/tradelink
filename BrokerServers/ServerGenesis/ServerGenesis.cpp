#include "stdafx.h"
#include "ServerGenesis.h"

using namespace TradeLibFast;

ServerGenesis::ServerGenesis()
{

}

ServerGenesis::~ServerGenesis()
{

}

void ServerGenesis::Start()
{
	TLServer_WM::Start();
	DeleteAllStocks();
	m_setting.SetExecAddress("69.64.202.155", 15805);
	m_setting.SetQuoteAddress("69.64.202.155", 15805);
	m_setting.SetLevel2Address("69.64.202.155", 15805);
	
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
	if(IsLoggedIn() == FALSE)
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
}



int ServerGenesis::CancelRequest(long id)
{
	CancelOrder(id);
	return OK;
}

GTStock *ServerGenesis::OnCreateStock(LPCSTR pszStock)
{
	GTStock *pStock = new GTStock(*this, pszStock);
	if(pStock == NULL)
		return NULL;

	return pStock;
}

void ServerGenesis::OnDeleteStock(GTStock *pStock)
{
	GTSession::OnDeleteStock(pStock);
}

int ServerGenesis::OnExecConnected()
{
	D("Exec Connected");
	return GTSession::OnExecConnected();
}

int ServerGenesis::OnExecDisconnected()
{

	D("Exec Disconnected");

	return GTSession::OnExecDisconnected();
}

int ServerGenesis::OnExecMsgLoggedin()
{
	D("Logged in");

	return GTSession::OnExecMsgLoggedin();
}

int ServerGenesis::OnExecMsgErrMsg(const GTErrMsg &errmsg)
{
	char msg[1024];
	LPCSTR pMsg = GetErrorMessage(errmsg.nErrCode, msg);

	D(pMsg);

	return GTSession::OnExecMsgErrMsg(errmsg);
}

int ServerGenesis::OnExecMsgChat(const GTChat &chat)
{
	char msg[1024];
	sprintf(msg, "%s -> %s: %s", chat.szUserFm, chat.szUserTo, chat.szText);
	D(msg);

	return GTSession::OnExecMsgChat(chat);
}

int ServerGenesis::OnGotLevel2Connected()
{
	D("Level 2 Connected");
	
	return GTSession::OnGotLevel2Connected();
}

int ServerGenesis::OnGotLevel2Disconnected()
{
	D("Level 2 Disconnected");

	return GTSession::OnGotLevel2Disconnected();
}

int ServerGenesis::OnGotQuoteConnected()
{
	D("Quote Connected");

	return GTSession::OnGotQuoteConnected();
}

int ServerGenesis::OnGotQuoteDisconnected()
{
	D("Quote Disconnected");

	return GTSession::OnGotQuoteDisconnected();
}

////
int ServerGenesis::OnGotChartConnected()
{
	D("Chart Connected");

	return GTSession::OnGotChartConnected();
}

int ServerGenesis::OnGotChartDisconnected()
{
	D("Chart Disconnected");

	return GTSession::OnGotQuoteDisconnected();
}

int ServerGenesis::OnTick()
{
	GTSession::OnTick();

	static int count = 0;
	
	if(count++ % 60 == 0)
	{
		// To Do here
	}

	//m_pDlg->SetDlgItemInt(IDC_LEVEL2S, this->m_stocks.GetCount());

	return 0;
}

void ServerGenesis::OnUDPQuoteMessage(int nCode, unsigned short nPort, const char *pszMsg)
{
	char ss[1024];
	_snprintf(ss, sizeof(ss), "%s, code=%d, port=%u", pszMsg, nCode, nPort);
	D(ss);

	GTSession::OnUDPQuoteMessage(nCode, nPort, pszMsg);
}



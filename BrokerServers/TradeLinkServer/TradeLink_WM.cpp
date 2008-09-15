// TradeLink_WM.cpp : implementation file
//

#include "stdafx.h"
#include "TradeLink_WM.h"
#include "TradeLink.h"
#include "Util.h"
using namespace std;


namespace TradeLinkServer
{


	// TradeLink_WM

	IMPLEMENT_DYNAMIC(TradeLink_WM, CWnd)

	TradeLink_WM::TradeLink_WM(void)
	{
		TLDEBUG = true;
		ENABLED = false;
		debugbuffer = CString("");
	}

	TradeLink_WM::~TradeLink_WM()
	{
		delete debugbuffer;
	}


	BEGIN_MESSAGE_MAP(TradeLink_WM, CWnd)
		ON_WM_COPYDATA()
	END_MESSAGE_MAP()

	int TradeLink_WM::FindClientFromStock(CString stock)
	{
		for (size_t i = 0; i<client.size(); i++)
			for (size_t j = 0; j<stocks[i].size(); j++)
			{
				TLSecurity sec = TLSecurity::Deserialize(stocks[i][j]);
				if (sec.sym.CompareNoCase(stock)==0)
					return i;
			}
		return -1;
	}

	bool TradeLink_WM::needStock(CString stock)
	{
		for (size_t i = 0; i<stocks.size(); i++)
			for (size_t j = 0; j<stocks[i].size(); j++)
			{
				TLSecurity sec = TLSecurity::Deserialize(stocks[i][j]);
				if (sec.sym==stock) return true;
			}
		return false;
	}

	int TradeLink_WM::FindClient(CString cwind)
	{
		size_t len = client.size();
		for (size_t i = 0; i<len; i++) if (client[i]==cwind) return (int)i;
		return -1;
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

	// TradeLink_WM message handlers


	BOOL TradeLink_WM::OnCopyData(CWnd* pWnd, COPYDATASTRUCT* pCopyDataStruct)
	{
		CString msg = (LPCTSTR)(pCopyDataStruct->lpData);
		int type = (int)pCopyDataStruct->dwData;
		return ServiceMessage(type,msg);
	}


	int TradeLink_WM::RegisterClient(CString  clientname)
	{
		if (FindClient(clientname)!=-1) return OK;
		client.push_back(clientname);
		time_t now;
		time(&now);
		heart.push_back(now); // save heartbeat at client index
		clientstocklist my = clientstocklist(0);
		stocks.push_back(my);
		D(CString(_T("Client ")+clientname+_T(" connected.")));
		return OK;
	}

	int TradeLink_WM::ServiceMessage(int MessageType, CString msg)
	{
			switch (MessageType)
			{
			case ORDERCANCELREQUEST :
				SrvCancelRequest((long)atoi(msg.GetBuffer()));
				return OK;
			case ACCOUNTREQUEST :
				return AccountResponse(msg);
			case CLEARCLIENT :
				return ClearClient(msg);
			case CLEARSTOCKS :
				return ClearStocks(msg);
			case REGISTERSTOCK :
				{
				vector<CString> rec;
				gsplit(msg,CString("+"),rec);
				CString client = rec[0];
				vector<CString> hisstocks;
				gsplit(rec[1],CString(","),hisstocks);
				unsigned int cid = FindClient(client); // parse first part as client name
				if (cid==-1) return CLIENTNOTREGISTERED; //client not registered
				stocks[cid] = hisstocks; // save the stocklist
				D(CString(_T("Client ")+client+_T(" registered: ")+gjoin(hisstocks,",")));
				HeartBeat(client);
				return RegisterStocks(client);
				}
			case REGISTERFUTURE :
				{
					vector<CString> rec;
					gsplit(msg,CString("+"),rec);
					CString client = rec[0];
					vector<CString> hisstocks;
					gsplit(rec[1],CString(","),hisstocks);
					unsigned int cid = FindClient(client); // parse first part as client name
					if (cid==-1) return CLIENTNOTREGISTERED; //client not registered
					stocks[cid] = hisstocks; // save the future list
					D(CString(_T("Client ")+client+_T(" registered: ")+gjoin(hisstocks,",")));
					HeartBeat(client);
					return RegisterFutures(client);
				}

			case REGISTERCLIENT :
				return RegisterClient(msg);
			case HEARTBEAT :
				return HeartBeat(msg);
			case BROKERNAME :
				return BrokerName();
			case SENDORDER :
				return SendOrder(TLOrder::Deserialize(msg));
			case OPENPRICE :
				return OK;
			case GETSIZE :
				return OK;
			case CLOSEPRICE :
				return OK;
			case YESTCLOSE :
				return OK;
			case NDAYHIGH :
				return OK;
			case NDAYLOW :
				return OK;
			case FEATUREREQUEST:
				{
					// get features supported by child class
					std::vector<int> stub = GetFeatures();
					// append basic feature we provide as parent
					stub.push_back(REGISTERCLIENT);
					stub.push_back(HEARTBEAT);
					stub.push_back(CLEARSTOCKS);
					stub.push_back(CLEARCLIENT);
					// send entire feature set back to client
					SendMsg(FEATURERESPONSE,SerializeIntVec(stub),msg);
					return OK;
				}

			}
			return UNKNOWNMSG;
	}

	int TradeLink_WM::HeartBeat(CString clientname)
	{
			int cid = FindClient(clientname);
			if (cid==-1) return -1;
			time_t now;
			time(&now);
			time_t then = heart[cid];
			double dif = difftime(now,then);
			heart[cid] = now;
			return (int)dif;
	}

	int TradeLink_WM::RegisterStocks(CString clientname) { return OK; }
	int TradeLink_WM::RegisterFutures(CString clientname) { return OK; }
	std::vector<int> TradeLink_WM::GetFeatures() { std::vector<int> blank; return blank; } 

	int TradeLink_WM::AccountResponse(CString clientname)
	{
		return FEATURE_NOT_IMPLEMENTED;
	}

	int TradeLink_WM::BrokerName(void)
	{
		return UnknownBroker;
	}

	int TradeLink_WM::SendOrder(TLOrder order)
	{
		return FEATURE_NOT_IMPLEMENTED;
	}

	int TradeLink_WM::ClearClient(CString clientname)
	{
		int cid = FindClient(clientname);
		if (cid==-1) return CLIENTNOTREGISTERED;
		client[cid] = "";
		stocks[cid] = clientstocklist(0);
		heart[cid] = NULL;
		D(CString(_T("Client ")+clientname+_T(" disconnected.")));
		return OK;
	}
	int TradeLink_WM::ClearStocks(CString clientname)
	{
		int cid = FindClient(clientname);
		if (cid==-1) return CLIENTNOTREGISTERED;
		stocks[cid] = clientstocklist(0);
		HeartBeat(clientname);
		D(CString(_T("Cleared stocks for ")+clientname));
		return OK;
	}
	void TradeLink_WM::SrvGotOrder(TLOrder order)
	{
		if (order.symbol=="") return;
		for (size_t i = 0; i<client.size(); i++)
			if (client[i]!="")
				SendMsg(ORDERNOTIFY,order.Serialize(),client[i]);
	}

	void TradeLink_WM::D(const CString & message)
	{

		if (this->TLDEBUG)
		{
			const CString NEWLINE = "\r\n";
			CString line(message);
			line.Append(NEWLINE);
			debugbuffer.Append(line);
			__raise this->GotDebug(message);
		}
	}

	void TradeLink_WM::SrvGotFill(TLTrade trade)
	{
		if (trade.symbol=="") return;
		for (size_t i = 0; i<stocks.size(); i++)
			for (size_t j = 0; j<stocks[i].size(); j++)
			{
				TLSecurity sec = TLSecurity::Deserialize(stocks[i][j]);
				if (sec.sym==trade.symbol)
					SendMsg(EXECUTENOTIFY,trade.Serialize(),client[i]);
			}
	}

	void TradeLink_WM::SrvGotTick(TLTick tick)
	{
		if (tick.sym=="") return;
		for (size_t i = 0; i<stocks.size(); i++)
			for (size_t j = 0; j<stocks[i].size(); j++)
			{
				TLSecurity sec = TLSecurity::Deserialize(stocks[i][j]);
				if (sec.sym==tick.sym)
					SendMsg(TICKNOTIFY,tick.Serialize(),client[i]);
			}
	}

	void TradeLink_WM::SrvCancelNotify(int orderid)
	{
		CString id;
		id.Format(_T("%i"),orderid);
		for (size_t i = 0; i<client.size(); i++)
			if (client[i]!="")
				SendMsg(ORDERCANCELRESPONSE,id,client[i]);
	}

	void TradeLink_WM::SrvCancelRequest(long order)
	{
		return;
	}

	bool TradeLink_WM::HaveSubscriber(CString stock)
	{
		for (size_t i = 0; i<stocks.size(); i++) // go through each client
			for (size_t j = 0; j<stocks[i].size(); j++) // and each stock
				if (stocks[i][j].CompareNoCase(stock)==0) 
					return true;
		return false;
	}
	void TradeLink_WM::Start(bool live)
	{
		if (!ENABLED)
		{
			CString wind(live ? LIVEWINDOW : SIMWINDOW);
			this->Create(NULL, wind, 0,CRect(0,0,20,20), CWnd::GetDesktopWindow(),NULL);
			this->ShowWindow(SW_HIDE); // hide our window
			this->D(CString("Started TL BrokerServer ")+wind);
			ENABLED = true;
		}


	}


}







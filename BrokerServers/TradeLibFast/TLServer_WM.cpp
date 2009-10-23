// TLServer_WM.cpp : implementation file
//

#include "stdafx.h"
#include "TLServer_WM.h"
#include "TradeLink.h"
#include "Util.h"
#include <fstream>
using namespace std;


namespace TradeLibFast
{

	const char* VERFILE = "\\tradelink\\brokerserver\\VERSION.txt";
	TLServer_WM::TLServer_WM(void)
	{
		MajorVer = 0.1;
		MinorVer = 0;
		TLDEBUG = true;
		ENABLED = false;
		debugbuffer = CString("");
		std::ifstream file;
		TCHAR path[MAX_PATH];
		SHGetFolderPath(NULL,CSIDL_PROGRAM_FILES,NULL,0,path);
		CString ver(path);
		ver.Append(VERFILE);
		file.open(ver.GetBuffer());
		if (file.is_open())
		{
			char data[8];
			file.getline(data,8);
			MinorVer = atoi(data);
			file.close();
		}

	}

	TLServer_WM::~TLServer_WM()
	{
		debugbuffer = "";

	}

	CString TLServer_WM::Version()
	{
		CString ver;
		ver.Format("%.1f.%i",MajorVer,MinorVer);
		return ver;
	}


	BEGIN_MESSAGE_MAP(TLServer_WM, CWnd)
		ON_WM_COPYDATA()
	END_MESSAGE_MAP()


	bool TLServer_WM::needStock(CString stock)
	{
		int idx = FindSym(stock);
		if (idx==-1) return false;
		bool needed = symclientidx[idx].size()!=0;
		return needed;
	}

	int TLServer_WM::FindClient(CString cwind)
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

	int TLServer_WM::FindSym(CString sym)
	{
		for (uint i = 0; i<symindex.size(); i++)
		{
			if (symindex[i]==sym) return i;
		}
		return -1;
	}

	void TLServer_WM::IndexBaskets()
	{
		// this function builds an index of all client subscribed to each symbol

		// go through every client's symbols
		for (uint c = 0; c<stocks.size(); c++)
		{
			// get current client
			clientstocklist hisstocks = stocks[c];
			// go through every symbol
			for (uint i = 0; i<hisstocks.size(); i++)
			{
				// get client's next symbol
				CString sym = hisstocks[i];
				// see if a client has subscribed to it already
				int idx = FindSym(sym);
				// if this client is first subscriber, add the symbol
				if (idx==-1) 
				{
					// never seen this symbol so add it
					symindex.push_back(sym);
					// keep track of the index
					idx = (int)symindex.size()-1;
					// create an index to hold all clients watching this symbol
					clientindex tmp;
					// add our client to the index
					tmp.push_back(c);
					// save the index at the same offset as the symbol index
					symclientidx.push_back(tmp);
				}
				else // somebody has already subscribed to this symbol before
				{
					// now that we have the symbol indexed,
					// lets go through every client and make sure
					// 1) client is subscribing to this symbol
					// 2) if he's subscribed, only subscribed once

					// we'll build a new index to replace current one
					clientindex newidx;
					// go through every client
					for (uint cid = 0; cid<stocks.size(); cid++)
					{
						// if client still wants symbol, add him to index
						if (ClientHasSymbol(cid,sym))
							newidx.push_back(cid);
					}
					// save index for this symbol and continue to next one
					symclientidx[idx] = newidx;
				}
			}
		}
	}

	bool TLServer_WM::ClientHasSymbol(int clientid, CString sym)
	{
		for (uint i = 0; i<stocks[clientid].size(); i++)
			if (stocks[clientid][i]==sym)
				return true;
		return false;
	}

	// TLServer_WM message handlers


	BOOL TLServer_WM::OnCopyData(CWnd* pWnd, COPYDATASTRUCT* pCopyDataStruct)
	{
		CString msg = (LPCTSTR)(pCopyDataStruct->lpData);
		int type = (int)pCopyDataStruct->dwData;
		switch (type)
		{
			case ORDERCANCELREQUEST :
				{
					const char * ch = msg.GetBuffer();
					long id = (long)atoi(ch);
					return CancelRequest(id);
				}
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
				// make sure client sent a basket, otherwise clear the basket
				if (rec.size()!=2) return ClearStocks(client);
				// get the basket
				gsplit(rec[1],CString(","),hisstocks);
				// make sure we have the client
				unsigned int cid = FindClient(client); 
				if (cid==-1) return CLIENTNOTREGISTERED; //client not registered
				// save the basket
				stocks[cid] = hisstocks; 
				// index his basket
				IndexBaskets();
				D(CString(_T("Client ")+client+_T(" registered: ")+gjoin(hisstocks,",")));
				HeartBeat(client);
				return RegisterStocks(client);
				}
			case POSITIONREQUEST :
				{
				vector<CString> r;
				gsplit(msg,CString("+"),r);
				if (r.size()!=2) return UNKNOWN_MESSAGE;
				return PositionResponse(r[1],r[0]);
				}
			case REGISTERCLIENT :
				return RegisterClient(msg);
			case HEARTBEAT :
				return HeartBeat(msg);
			case BROKERNAME :
				return BrokerName();
			case SENDORDER :
				return SendOrder(TLOrder::Deserialize(msg));
			case FEATUREREQUEST:
				{
					// get features supported by child class
					std::vector<int> stub = GetFeatures();
					// append basic feature we provide as parent
					stub.push_back(REGISTERCLIENT);
					stub.push_back(HEARTBEAT);
					stub.push_back(CLEARSTOCKS);
					stub.push_back(CLEARCLIENT);
					stub.push_back(VERSION);
					// send entire feature set back to client
					TLSend(FEATURERESPONSE,SerializeIntVec(stub),msg);
					return OK;
				}
			case VERSION :
					return MinorVer;
			case DOMREQUEST :
				{
				vector<CString> rec;
				gsplit(msg,CString("+"),rec);
				CString client = rec[0];
				// make sure we have the client
				unsigned int cid = FindClient(client); 
				if (cid==-1) return CLIENTNOTREGISTERED; //client not registered
				D(CString(_T("Client ")+client+_T(" registered: ")));
				HeartBeat(client);
				return DOMRequest(atoi(rec[1]));
				}
			default: // unknown messages
				{
					int um = UnknownMessage(type,msg);
					// issue #141
					CString data;
					data.Format("%i",um);
					for (uint i = 0; i<client.size(); i++)
						TLSend(type,data,i);
					// this will go away soon
					return um;

				}
		}

		return FEATURE_NOT_IMPLEMENTED;
	}


	int TLServer_WM::RegisterClient(CString  clientname)
	{
		// make sure client is unique
		if (FindClient(clientname)!=-1) return OK;
		// save client
		client.push_back(clientname);
		// get handle to client
		HWND dest = FindWindowA(NULL,(LPCSTR)(LPCTSTR)clientname)->GetSafeHwnd();
		// save client handle
		hims.push_back(dest);
		// get time
		time_t now;
		time(&now);
		// save time as last heartbeat
		heart.push_back(now); // save heartbeat at client index
		// save empty list of symbols
		clientstocklist my = clientstocklist(0);
		stocks.push_back(my);
		// notify users
		D(CString(_T("Client ")+clientname+_T(" connected.")));
		return OK;
	}

	int TLServer_WM::UnknownMessage(int MessageType, CString msg)
	{
		return UNKNOWN_MESSAGE;
	}

	int TLServer_WM::HeartBeat(CString clientname)
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

	int TLServer_WM::RegisterStocks(CString clientname) 
	{ 
		return OK; 
	}
	int TLServer_WM::DOMRequest(int depth) { return OK; }
	std::vector<int> TLServer_WM::GetFeatures() { std::vector<int> blank; return blank; } 

	int TLServer_WM::AccountResponse(CString clientname)
	{
		return FEATURE_NOT_IMPLEMENTED;
	}

	int TLServer_WM::PositionResponse(CString account, CString clientname)
	{
		return FEATURE_NOT_IMPLEMENTED;
	}

	int TLServer_WM::BrokerName(void)
	{
		return TradeLink;
	}

	int TLServer_WM::SendOrder(TLOrder order)
	{
		return FEATURE_NOT_IMPLEMENTED;
	}

	int TLServer_WM::ClearClient(CString clientname)
	{
		int cid = FindClient(clientname);
		if (cid==-1) return CLIENTNOTREGISTERED;
		client[cid] = "";
		stocks[cid] = clientstocklist(0);
		heart[cid] = NULL;
		hims[cid] = NULL;
		D(CString(_T("Client ")+clientname+_T(" disconnected.")));
		return OK;
	}
	int TLServer_WM::ClearStocks(CString clientname)
	{
		int cid = FindClient(clientname);
		if (cid==-1) return CLIENTNOTREGISTERED;
		stocks[cid] = clientstocklist(0);
		IndexBaskets();
		HeartBeat(clientname);
		D(CString(_T("Cleared stocks for ")+clientname));
		return OK;
	}


	void TLServer_WM::D(const CString & message)
	{

		if (this->TLDEBUG)
		{
			const CString NEWLINE = "\n";
			CString line;
			vector<int> now;
			TLTimeNow(now);
			line.Format("%i %s%s",now[TLtime],message,NEWLINE);
			debugbuffer.Append(line);
			__raise this->GotDebug(line);
		}
	}

	void TLServer_WM::SrvGotOrder(TLOrder order)
	{
		if (order.symbol=="") return;
		for (size_t i = 0; i<client.size(); i++)
			if (client[i]!="")
				TLSend(ORDERNOTIFY,order.Serialize(),client[i]);
	}

	void TLServer_WM::SrvGotFill(TLTrade trade)
	{
		if (!trade.isValid()) return;
		for (size_t i = 0; i<client.size(); i++)
			if (client[i]!="")
				TLSend(EXECUTENOTIFY,trade.Serialize(),client[i]);
	}

	void TLServer_WM::SrvGotTick(TLTick tick)
	{
		// if tick has no symbol index, send it old way
		if (tick.symid<0)
		{

			for (uint i = 0; i<stocks.size(); i++)
				for (uint j = 0; j<stocks[i].size(); j++)
				{
					if (stocks[i][j]==tick.sym)
						TLSend(TICKNOTIFY,tick.Serialize(),i);
				}
			return;
		}
		// otherwise get only clients by their index
		clientindex symclients = symclientidx[tick.symid];
		for (uint i = 0; i<symclients.size(); i++)
		{
			TLSend(TICKNOTIFY,tick.Serialize(),symclients[i]);
		}
	}

	void TLServer_WM::SrvGotCancel(int orderid)
	{
		CString id;
		id.Format(_T("%i"),orderid);
		for (size_t i = 0; i<client.size(); i++)
			if (client[i]!="")
				TLSend(ORDERCANCELRESPONSE,id,client[i]);
	}

	int TLServer_WM::CancelRequest(long order)
	{
		return FEATURE_NOT_IMPLEMENTED;
	}

	bool TLServer_WM::HaveSubscriber(CString stock)
	{
		for (size_t i = 0; i<stocks.size(); i++) // go through each client
			for (size_t j = 0; j<stocks[i].size(); j++) // and each stock
				if (stocks[i][j].CompareNoCase(stock)==0) 
					return true;
		return false;
	}
	void TLServer_WM::Start() 
	{
		
		if (!ENABLED)
		{
			ENABLED = true;
			CString servername = UniqueWindowName("TradeLinkServer");
			CWnd* parent = CWnd::GetDesktopWindow();
			this->Create(NULL, servername, 0,CRect(0,0,20,20),parent ,NULL);
			this->ShowWindow(SW_HIDE); // hide our window
			CString msg;
			msg.Format("Started TL BrokerServer %s [ %.1f.%i]",servername,MajorVer,MinorVer);
			this->D(msg);

		}
		
					
	}


	
	long TLServer_WM::TLSend(int type,LPCTSTR msg,int clientid) 
	{
		// make sure client exists
		if ((clientid>=(int)hims.size()) || (clientid<0)) 
			return (long)TLCLIENT_NOT_FOUND;
		HWND dest = hims[clientid];
		return TLSend(type,msg,dest);
	}
	long TLServer_WM::TLSend(int type,LPCTSTR msg,CString windname) 
	{
		HWND dest = FindWindowA(NULL,(LPCSTR)(LPCTSTR)windname)->GetSafeHwnd();
		return TLSend(type,msg,dest);
	}
	long TLServer_WM::TLSend(int type,LPCTSTR msg, HWND dest)
	{
		// set default result
		LRESULT result = TLCLIENT_NOT_FOUND;
		
		if (dest) 
		{
			COPYDATASTRUCT CD;  // windows-provided structure for this purpose
			CD.dwData=type;		// stores type of message
			int len = 0;
			len = (int)strlen((char*)msg);

			CD.cbData = len+1;
			CD.lpData = (void*)msg;	//here's the data we're sending
			result = ::SendMessageA(dest,WM_COPYDATA,0,(LPARAM)&CD);
		} 
		return (long)result;
	}
}










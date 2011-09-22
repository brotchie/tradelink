#pragma once
#include "TradeLink.h"
#include "EWrapper.h"
#include "eclientsocket.h"
#include "TLOrder.h"
#include <vector>
#include "orderstate.h"
#include "Contract.h"
#include "TLServer_IP.h"
#include "BarRequest.h"


namespace TradeLibFast
{

	class TWS_TLServer :
		public TLServer_WM ,
		public EWrapper
	{
	public:
		TWS_TLServer(void);
		~TWS_TLServer(void);

		// these are the TradeLink methods we're overriding
		int SendOrder(TLOrder o);
		int BrokerName(void);
		int CancelRequest(int64 id);
		int AccountResponse(CString clientname);
		int RegisterStocks(CString clientname);
		std::vector<int> GetFeatures();
		int UnknownMessage(int MessageType, CString msg);
		void Start(void);
	    //ILDEBEGIN
        int DOMRequest(int depth);
		int TWS_TLServer::findStockticksTid(CString symbol);
		//ILDEEND

	private:

		CString getmultiplier(TLSecurity sec);
		void getcontract(CString symbol, CString currency, CString exchange,Contract* contract);
		CString truncateat(CString original,CString after);
		CString truncatebefore(CString original,CString before);
		void pcont(Contract* c);
		void pord(Order* o);
		CString tl2ibspace(CString tlsym);
		CString ib2tlspace(CString ibsym);
		void v(CString msg);
		bool noverb;
		bool fillnotifyfullsymbol;
		bool linktest();
		vector<TLPosition> poslist;
		bool havepos(TLPosition p);
		int PositionResponse(CString account, CString client);
		// here's a vector of pointers to our socket connections to IB
		// (if we have more than one)
		std::vector<EClient*> m_link;
		int FIRSTSOCKET; // first port to look for TWS instances
		std::vector<OrderId> m_nextorderids; // next valid id for each mlink
		std::vector<CString> accts; // accounts for each mlink
		void InitSockets(int maxsockets, int clientid); // discover mlinks
		int m_nextsocket; // next socket to search for mlink
		OrderId getNextOrderId(CString account);
		vector<OrderId> iborders;
		vector<int64> tlorders;
		OrderId TL2IBID(int64 tlid);
		int64 IB2TLID(OrderId ibid);
		int64 saveOrder(OrderId ibid, CString acct);
		OrderId newOrder(int64 tlid,CString acct);
		void IncOrderId(CString account);
		bool hasAccount(CString account);
		int getMlinkId(OrderId id);
		std::vector<int> validlinkids; // gets the m_links ids that are logged in
		bool IGNOREERRORS;
		int getsymbolindex(CString sym);
		// for storing subscribed symbols and their tickerid
		bool hasTicker(CString symbol);
		// following used because tick updates only include size or price that changed (not both)
		std::vector<TLTick> stockticks;
		// default currency
		CString _currency;
		// historical bar prices requested
		CString histBarWhatToShow;
		// historical bars use regular trading hours
		int histBarRTH;
		// historical bar symbols
		std::vector<BarRequest> histBarSymbols;

		// these are the IB-api methods we'll override (from EWrapper above)
		void tickPrice( TickerId ddeId, TickType field, double price, int canAutoExecute);
		void tickSize( TickerId ddeId, TickType field, int size);
		void tickOptionComputation( TickerId ddeId, TickType field, double impliedVol,
			double delta, double modelPrice, double pvDividend);
		void tickGeneric(TickerId tickerId, TickType tickType, double value);
		void tickString(TickerId tickerId, TickType tickType, const CString& value);
		void tickEFP(TickerId tickerId, TickType tickType, double basisPoints,
			const CString& formattedBasisPoints, double totalDividends, int holdDays,
			const CString& futureExpiry, double dividendImpact, double dividendsToExpiry);
		void orderStatus( OrderId orderId, const CString &status, int filled, int remaining, 
			double avgFillPrice, int permId, int parentId, double lastFillPrice,
			int clientId, const CString& whyHeld);
		void openOrder( OrderId orderId, const Contract&, const Order&, const OrderState&);
		void winError( const CString &str, int lastError);
		void connectionClosed();
		void updateAccountValue(const CString &key, const CString &val,
			const CString &currency, const CString &accountName);
		virtual void updatePortfolio( const Contract& contract, int position,
			double marketPrice, double marketValue, double averageCost,
			double unrealizedPNL, double realizedPNL, const CString &accountName);
		void updateAccountTime(const CString &timeStamp);
		void nextValidId( OrderId orderId);
		void contractDetails( int reqId, const ContractDetails& contractDetails);
		void bondContractDetails( int reqId, const ContractDetails& contractDetails);
		void contractDetailsEnd( int reqId);
		void execDetails( OrderId orderId, const Contract& contract, const Execution& execution);
		void error(const int id, const int errorCode, const CString errorString);

		void updateMktDepth( TickerId id, int position, int operation, int side, 
				double price, int size);
		void updateMktDepthL2( TickerId id, int position, CString marketMaker, int operation, 
				int side, double price, int size);
		void updateNewsBulletin(int msgId, int msgType, const CString& newsMessage, const CString& originExch);
		void managedAccounts(const CString& accountsList);
		void receiveFA(faDataType pFaDataType, const CString& cxml);
		void historicalData(TickerId reqId, const CString& date, double open, double high, double low,
						  double close, int volume, int barCount, double WAP, int hasGaps) ;
		void scannerParameters(const CString &xml);
		void scannerData(int reqId, int rank, const ContractDetails &contractDetails, const CString &distance,
			const CString &benchmark, const CString &projection, const CString &legsStr);
		void scannerDataEnd(int reqId);
		void realtimeBar(TickerId reqId, long time, double open, double high, double low, double close,
		   long volume, double wap, int count);
		void currentTime(long time);
		void fundamentalData(TickerId reqId, const CString& data);

		//BEGINILDE
		CString clientname_;
		//ENDILDE

	public:
		EClient* GetOrderSink(CString account);
	};
}
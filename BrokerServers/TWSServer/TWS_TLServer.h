#pragma once
#include "TradeLink.h"
#include "EWrapper.h"
#include "eclientsocket.h"
#include "TLOrder.h"
#include <vector>
#include "orderstate.h"
#include "Contract.h"
#include "TLServer_IP.h"



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
		int CancelRequest(OrderId id);
		int AccountResponse(CString clientname);
		int RegisterStocks(CString clientname);
		std::vector<int> GetFeatures();
		int UnknownMessage(int MessageType, CString msg);
		void Start(void);


	private:
		// here's a vector of pointers to our socket connections to IB
		// (if we have more than one)
		std::vector<EClient*> m_link;
		int FIRSTSOCKET; // first port to look for TWS instances
		std::vector<int> m_nextorderids; // next valid id for each mlink
		std::vector<CString> accts; // accounts for each mlink
		void InitSockets(int maxsockets, int clientid); // discover mlinks
		int m_nextsocket; // next socket to search for mlink
		OrderId getNextOrderId(CString account);
		vector<uint> iborders;
		vector<uint> tlorders;
		uint TL2IBID(uint tlid);
		uint IB2TLID(uint ibid);
		uint newOrder(uint tlid,CString acct);
		void IncOrderId(CString account);
		bool hasAccount(CString account);
		int getMlinkId(OrderId id);
		std::vector<int> validlinkids; // gets the m_links ids that are logged in
		bool IGNOREERRORS;
		// for storing subscribed symbols and their tickerid
		bool hasTicker(CString symbol);
		// following used because tick updates only include size or price that changed (not both)
		std::vector<TLTick> stockticks;

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

	public:
		EClient* GetOrderSink(CString account);
	};
}
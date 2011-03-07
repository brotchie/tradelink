#pragma once
//#include "LightspeedTrader.h"
#include "TradeLibFast.h"
#include <vector>
using namespace std;
using namespace TradeLibFast;



	class LS_TLWM :
		public TLServer_WM,
		public L_Observer
	{
	public:
		LS_TLWM(void);
		~LS_TLWM(void);
		vector<int> GetFeatures();
		int AccountResponse(CString clientname);
		int PositionResponse(CString account,CString clientname);
		int RegisterStocks(CString clientname);
		int DOMRequest(int depth);
		static LS_TLWM* GetInstance() { return instance; };
		volatile uint _writeimb;
		volatile uint _readimb;
		volatile bool _imbflip;
		vector<TLImbalance> _imbcache;
		volatile bool _startimb;
		void SrvGotImbalance(TLImbalance imb);

	protected:
		void ReadConfig();
		bool _proactive;
		//TLOrder ProcessOrder(Order* order);
		void SrvGotImbAsync(TLImbalance imb);
		//double GetDouble(const Money* m);
		//double GetDouble(Money m);
		//Money  Double2Money(double val);
		static LS_TLWM* instance;
		vector <L_Observer*> subs;
		vector<CString> subsym;
		//vector<Order*> ordercache;
		//const StockBase* preload(CString symbol);

		int BrokerName(void);
		int SendOrder(TLOrder order);
		int UnknownMessage(int MessageType,CString msg);
		int CancelRequest(int64 order);

		// account monitoring stuff
		L_Observable* imbalance;
		vector<int> imbalance_clients;
		vector<L_Observable*> accounts;
		vector<int64> orderids;
		//int64 fetchOrderIdAndRemove(Order* order);
		//int64 fetchOrderId(Order * order);
		bool IdIsUnique(int64 id);
		//bool saveOrder(Order* o,int64 id, bool overwriteexistingid);
		//bool saveOrder(Order* o,int64 id);
		unsigned int AnvilId(int64 TLOrderId);

		//virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);

		void RemoveUnused();
		void RemoveSub(CString stock);
		bool isIndex(CString sym);
		bool hasHammerSub(CString symbol);
		int SubIdx(CString symbol);
		int ClearClient(CString client);
		int ClearStocks(CString client);
		int ClearStocks(void);

		int depth;



	};





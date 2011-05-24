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
			DECLARE_DYNAMIC(LS_TLWM)
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
		void D(const CString &message);
		// L_Observer
	virtual void HandleMessage(L_Message const *msg);

	protected:
		CString or2str(long res);
		void v(const CString &msg);
		bool _noverb;
		bool saveOrderId(int64 tlid, long lsid);
		void ReadConfig();
		bool _imbexch;
		long _date;
		long _time;
		TLOrder ProcessOrder(L_Order* order);
		void SrvGotImbAsync(TLImbalance imb);
		static LS_TLWM* instance;
		vector <L_Summary*> subs;
		vector<CString> subsym;
		vector<L_Order*> ordercache;
		vector<long> lsids;
		L_Summary* preload(CString symbol);
		long gettif(TLOrder o);
		long gettype(TLOrder o);

		int BrokerName(void);
		int SendOrder(TLOrder order);
		int UnknownMessage(int MessageType,CString msg);
		int CancelRequest(int64 order);

		// account monitoring stuff
		bool imbreq;
		vector<int> imbalance_clients;
		vector<L_Account*> accounts;
		vector<int64> orderids;
		vector<long*> lscorrelationid;
		vector<int64> tlcorrelationid;
		long nextcorr;
		int64 fetchOrderIdAndRemove(L_Order* order);
		int64 fetchOrderId(L_Order * order);
		bool IdIsUnique(int64 id);
		bool saveOrder(L_Order* o,int64 id, bool overwriteexistingid);
		bool saveOrder(L_Order* o,int64 id);
		unsigned int AnvilId(int64 TLOrderId);


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





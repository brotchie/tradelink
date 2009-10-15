#pragma once
#include "TradeLibFast.h"
#include "AVLStock.h"
#include "AVLIndex.h"
#include "AnvilUtil.h"
#include "BusinessApi.h"
#include <vector>
using namespace std;

namespace TradeLibFast
{
	class AVL_TLWM :
		public TLServer_WM,
		public Observer
	{
	public:
		AVL_TLWM(void);
		~AVL_TLWM(void);
		vector<int> GetFeatures();
		int AccountResponse(CString clientname);
		int PositionResponse(CString account,CString clientname);
		int RegisterStocks(CString clientname);
		int DOMRequest(int depth);
		static AVL_TLWM* GetInstance() { return instance; };
		

	protected:
		double GetDouble(const Money* m);
		double GetDouble(Money m);
		Money  Double2Money(double val);
		static AVL_TLWM* instance;
		vector <Observer*> subs;
		vector<CString> subsym;
		vector<Order*> ordercache;
		const StockBase* preload(CString symbol);

		int BrokerName(void);
		int SendOrder(TLOrder order);
		int UnknownMessage(int MessageType,CString msg);
		int CancelRequest(long order);

		// account monitoring stuff
		Observable* imbalance;
		vector<int> imbalance_clients;
		vector<Observable*> accounts;
		vector<uint> orderids;
		uint fetchOrderIdAndRemove(Order* order);
		uint fetchOrderId(Order * order);
		bool IdIsUnique(uint id);
		bool saveOrder(Order* o,uint id, bool overwriteexistingid);
		bool saveOrder(Order* o,uint id);
		unsigned int AnvilId(unsigned int TLOrderId);

		virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);

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
}




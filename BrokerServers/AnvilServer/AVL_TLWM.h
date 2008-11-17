#pragma once
#include "TradeLibFast.h"
#include "TLStock.h"
#include "TLIndex.h"
#include "AnvilUtil.h"
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
	protected:
		vector <TLStock*> stocksubs;
		
		bool hasHammerSub(CString stock);
		int RegisterStocks(CString clientname);
		int BrokerName(void);
		int SendOrder(TLOrder order);

		// account monitoring stuff
		vector<Observable*> accounts;
		int cacheOrder(Order* o);
		bool hasOrder(unsigned int id);
		virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);


	};
}

void TLUnload();
void AllClients(std::vector <CString> &subscriberids);
void Subscribers(CString stock,std::vector <CString> &subscriberids);
void TLKillDead(int deathInseconds);


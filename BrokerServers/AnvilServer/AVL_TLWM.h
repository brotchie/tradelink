#pragma once
#include "TradeLibFast.h"
#include "TLStock.h"
#include "TLIndex.h"
using namespace std;

namespace TradeLibFast
{
	class AVL_TLWM :
		public TLServer_WM
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

	};
}

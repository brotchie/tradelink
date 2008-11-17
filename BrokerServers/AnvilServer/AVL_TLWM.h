#pragma once
#include "TradeLink.h"
#include "TradeLink_WM.h"
#include "AVLStock.h"
#include "AVLIdx.h"
using namespace std;

namespace TradeLinkServer
{
	class AVL_TLWM :
		public TradeLink_WM
	{
	public:
		AVL_TLWM(void);
		~AVL_TLWM(void);
	protected:
		vector <AVLStock*> stocksubs;
		bool hasHammerSub(CString stock);
		int RegisterStocks(CString clientname);
		int BrokerName(void);
		int SendOrder(TLOrder order);

	};
}

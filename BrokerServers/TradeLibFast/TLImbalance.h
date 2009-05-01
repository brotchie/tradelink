#pragma once
#include "TradeLibFast.h"

namespace TradeLibFast
{

	class AFX_EXT_CLASS TLImbalance
	{
	public:
		TLImbalance();
		~TLImbalance();
		CString Symbol;
		CString Ex;
		int ThisImbalance;
		int ThisTime;
		int PrevImbalance;
		int PrevTime;
		int InfoImbalance;
		static CString Serialize(TLImbalance imbal);
		static TLImbalance Deserialize(CString msg);
		bool hasImbalance();
		bool hadImbalance();
	};

    enum ImbalanceField
    {
        IF_SYM,
        IF_EX,
        IF_SIZE,
        IF_TIME,
        IF_PSIZE,
        IF_PTIME,
		IF_INFO,
    };
}
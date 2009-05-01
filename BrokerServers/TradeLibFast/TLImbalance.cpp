#include "stdafx.h"
#include "TLImbalance.h"
#include "Util.h"

namespace TradeLibFast
{
	TLImbalance::TLImbalance()
	{
		Symbol = CString("");
		Ex = CString("");
		ThisImbalance = 0;
		PrevImbalance = 0;
		ThisTime = 0;
		PrevTime = 0;
		InfoImbalance = 0;
	}
	TLImbalance::~TLImbalance()
	{
	}

	bool TLImbalance::hasImbalance()
	{
		return (ThisImbalance!=0) || (InfoImbalance!=0);
	}

	bool TLImbalance::hadImbalance()
	{
		return PrevImbalance!=0;
	}

	CString TLImbalance::Serialize(TLImbalance i)
	{
		CString msg;
		msg.Format("%s,%s,%i,%i,%i,%i,%i",i.Symbol,i.Ex,i.ThisImbalance,i.ThisTime,i.PrevImbalance,i.PrevTime,i.InfoImbalance);
		return msg;
	}

	TLImbalance TLImbalance::Deserialize(CString msg)
	{
		std::vector<CString> r;
		gsplit(msg,CString(","),r);
		TLImbalance i;
		i.Symbol = r[IF_SYM];
		i.Ex = r[IF_EX];
		i.ThisImbalance = atoi(r[IF_SIZE].GetBuffer());
		i.PrevImbalance= atoi(r[IF_PSIZE].GetBuffer());
		i.PrevTime = atoi(r[IF_PTIME].GetBuffer());
		i.ThisTime = atoi(r[IF_TIME].GetBuffer());
		i.InfoImbalance = atoi(r[IF_INFO].GetBuffer());
		return i;
	}
}

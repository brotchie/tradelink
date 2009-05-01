#include "stdafx.h"
#include <cfix.h>
#include "TradeLibFast.h"

using namespace TradeLibFast;

static void __stdcall SerializeDeserialize()
{
	// some stocks
	TLImbalance ib;
	ib.Symbol = CString("IBM");
	ib.Ex = CString("NYSE");
	ib.ThisImbalance = 500000;
	ib.ThisTime = 1550;
	ib.PrevImbalance = 400000;
	ib.PrevTime = 1540;
	ib.InfoImbalance = 11500;
	
	// serialize it
	CString msg = TLImbalance::Serialize(ib);
	// undo it
	TLImbalance ni = TLImbalance::Deserialize(msg);
	// verify are same
	CFIX_ASSERT(ni.Symbol==ib.Symbol);
	CFIX_ASSERT(ni.Ex==ib.Ex);
	CFIX_ASSERT(ni.ThisImbalance==ib.ThisImbalance);
	CFIX_ASSERT(ni.PrevImbalance==ib.PrevImbalance);
	CFIX_ASSERT(ni.ThisTime==ib.ThisTime);
	CFIX_ASSERT(ni.PrevTime == ib.PrevTime);
	CFIX_ASSERT(ni.InfoImbalance == ib.InfoImbalance);
}



CFIX_BEGIN_FIXTURE( TLImbalance )
	CFIX_FIXTURE_ENTRY( SerializeDeserialize )
CFIX_END_FIXTURE()
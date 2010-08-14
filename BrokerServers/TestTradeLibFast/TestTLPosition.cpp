#include "stdafx.h"
#include <cfix.h>
#include "TradeLibFast.h"

using namespace TradeLibFast;

static void __stdcall Basics()
{
	const CString sym = "LVS";
	const double x = 10;
	const int s = 200;
	TLPosition p(sym,x,s,CString("test"));
	CFIX_ASSERT(p.ClosedPL== 0);
	CFIX_ASSERT(p.Symbol==sym);
	CFIX_ASSERT(p.Size==s);
	CFIX_ASSERT(p.AvgPrice==x);
	CFIX_ASSERT(p.Account=="test");

	
}

static void __stdcall SerializeDeserialize()
{
	const CString sym = "LVS";
	const double x = 10;
	const int s = 200;
	TLPosition p(sym,x,s,CString("test"));
	CFIX_ASSERT(p.ClosedPL== 0);
	CFIX_ASSERT(p.Symbol==sym);
	CFIX_ASSERT(p.Size==s);
	CFIX_ASSERT(p.AvgPrice==x);
	CFIX_ASSERT(p.Account=="test");
	// flatten it
	CString msg = p.Serialize();
	// unflatten it
	TLPosition p2 = TLPosition::Deserialize(msg);
	// check the results
	CFIX_ASSERT(p2.ClosedPL== 0);
	CFIX_ASSERT(p2.Symbol==sym);
	CFIX_ASSERT(p2.Size==s);
	CFIX_ASSERT(p2.AvgPrice==x);
	CFIX_ASSERT(p2.Account=="test");
}

CFIX_BEGIN_FIXTURE( TestTLPosition )
	CFIX_FIXTURE_ENTRY( Basics )
	CFIX_FIXTURE_ENTRY( SerializeDeserialize )
CFIX_END_FIXTURE()

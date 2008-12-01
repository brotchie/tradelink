#include "stdafx.h"
#include <cfix.h>
#include "TradeLibFast.h"

using namespace TradeLibFast;

static void __stdcall Create()
{
	// some stocks
	char* list[] = { "LVS", "WAG","GM","MHS" };
	const uint count = 4;
	vector<CString> secs(list,list+count);
	// initial basket with our stocks
	TLMarketBasket mb;
	mb.Add(secs);
	// test to make sure our basket got them all
	bool match = true;
	for (uint i = 0; i<count; i++)
	{
		match = mb[i].sym.CompareNoCase(list[i])==0;
		if (!match) break;
	}
	CFIX_ASSERT(match);
}

static void __stdcall SerializeDeserialize()
{
	// some stocks
	char* list[] = { "LVS", "WAG","GM","MHS" };
	vector<CString> secs(list,list+4);
	// initial basket with our stocks
	TLMarketBasket mb;
	mb.Add(secs);
	// serialize it
	CString msg = mb.Serialize();
	// undo it
	TLMarketBasket mb2 = TLMarketBasket::Deserialize(msg);
	// test to make sure our basket got them all
	bool match = true;
	for (uint i = 0; i<mb2.Count(); i++)
	{
		match &= mb2[i].sym == secs[i];
	}
	CFIX_ASSERT(match);

}



CFIX_BEGIN_FIXTURE( TestTLMarketBasket )
	CFIX_FIXTURE_ENTRY( Create )
	CFIX_FIXTURE_ENTRY( SerializeDeserialize )
CFIX_END_FIXTURE()
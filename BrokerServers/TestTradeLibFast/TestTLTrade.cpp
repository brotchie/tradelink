#include "stdafx.h"
#include <cfix.h>
#include "TradeLibFast.h"

using namespace TradeLibFast;

static void __stdcall Basics()
{
	const CString sym = "LVS";
	const double x = 10;
	const int s = 200;
	TLTrade f;
	CFIX_ASSERT(!f.isValid());
	CFIX_ASSERT(!f.isFilled());
	f.xdate = 20081201;
	f.xprice = x;
	f.xsize = s;
	f.symbol = sym;
	CFIX_ASSERT(f.isValid());
	CFIX_ASSERT(f.isFilled());

}

static void __stdcall SerializeDeserialize()
{
	// serialize
	const CString sym = "CLZ8";
	const CString ex = "NYMEX";

	TLTrade o;
	o.symbol = sym;
	o.xdate = 20081201;
	o.xtime = 153100;
	o.xprice = 0;
	o.xsize = -100;
	o.side = false;
	o.exchange = "NYMEX";
	// flatten it
	CString m = o.Serialize();

	// convert it back to object
	TLTrade d = TLTrade::Deserialize(m);
	CFIX_ASSERT(o.symbol==d.symbol);
	CFIX_ASSERT(o.xprice==d.xprice);
	CFIX_ASSERT(o.xsize==d.xsize);
	CFIX_ASSERT(o.side==d.side);
	CFIX_ASSERT(o.xdate==d.xdate);
	CFIX_ASSERT(o.xtime==d.xtime);
	CFIX_ASSERT(o.exchange==d.exchange);

}

CFIX_BEGIN_FIXTURE( TestTLTrade )
	CFIX_FIXTURE_ENTRY( Basics )
	CFIX_FIXTURE_ENTRY( SerializeDeserialize )
CFIX_END_FIXTURE()
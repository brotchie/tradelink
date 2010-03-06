#include "stdafx.h"
#include <cfix.h>
#include "TradeLibFast.h"

using namespace TradeLibFast;

static void __stdcall Basics()
{
	const CString sym = "LVS";
	const double x = 10;
	const int s = 200;
	TLOrder o;
	CFIX_ASSERT(!o.isFilled());
	CFIX_ASSERT(!o.isValid());
	o.symbol = sym;
	o.size = s;
	CFIX_ASSERT(o.isMarket());
	o.price = x;
	CFIX_ASSERT(o.isLimit());
	o.stop = x;
	CFIX_ASSERT(o.isStop());
}

static void __stdcall SerializeDeserialize()
{
	// serialize
	const CString sym = "CLZ8";
	const CString ex = "NYMEX";
	TLOrder o;
	o.id = 2;
	o.symbol = sym;
	o.exchange = ex;
	o.date = 20081201;
	o.time = 153100;
	o.price = 0;
	o.size = -100;
	o.side = false;
	// flatten it
	CString m = o.Serialize();

	// convert it back to object
	TLOrder d = TLOrder::Deserialize(m);
	CFIX_ASSERT(o.symbol==d.symbol);
	CFIX_ASSERT(o.price==d.price);
	CFIX_ASSERT(o.size==d.size);
	CFIX_ASSERT(o.side==d.side);
	CFIX_ASSERT(o.exchange==d.exchange);
	CFIX_ASSERT(o.date==d.date);
	CFIX_ASSERT(o.time==d.time);
	CFIX_ASSERT(o.id==d.id);

}

CFIX_BEGIN_FIXTURE( TestTLOrder)
	CFIX_FIXTURE_ENTRY( Basics )
	CFIX_FIXTURE_ENTRY( SerializeDeserialize )
CFIX_END_FIXTURE()
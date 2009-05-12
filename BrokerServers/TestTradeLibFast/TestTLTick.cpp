#include "stdafx.h"
#include <cfix.h>
#include "TradeLibFast.h"

using namespace TradeLibFast;

static void __stdcall Basics()
{
	const CString sym = "LVS";
	const double x = 10;
	const int s = 200;
	TLTick k;
	k.sym = sym;
	k.bid = 10;
	CFIX_ASSERT(!k.hasBid());
	k.bs = 1;
	CFIX_ASSERT(k.hasBid());
	CFIX_ASSERT(!k.isTrade());
	CFIX_ASSERT(k.isValid());

	TLTick k2;
	k2.sym = sym;
	k2.trade = 10;
	CFIX_ASSERT(!k2.isTrade());
	CFIX_ASSERT(!k2.hasAsk());
	CFIX_ASSERT(!k2.hasBid());
	k2.size = s;
	CFIX_ASSERT(k2.isTrade());
	CFIX_ASSERT(k2.isValid());
}

static void __stdcall SerializeDeserialize()
{
	// serialize
	const CString sym = "CLZ8";
	const CString ex = "NYMEX";
	TLTick t;
	t.sym = sym;
	t.trade = 10;
	t.size =100;
	t.ex = ex;
	t.depth = 4;
	CString m = t.Serialize();

	// go back to object
	TLTick k = TLTick::Deserialize(m);
	CFIX_ASSERT(k.sym==t.sym);
	CFIX_ASSERT(k.trade==t.trade);
	CFIX_ASSERT(k.isValid());
	CFIX_ASSERT(k.size==t.size);
	CFIX_ASSERT(k.ex==t.ex);
	CFIX_ASSERT(k.depth==t.depth);
}

CFIX_BEGIN_FIXTURE( TestTLTick )
	CFIX_FIXTURE_ENTRY( Basics )
	CFIX_FIXTURE_ENTRY( SerializeDeserialize )
CFIX_END_FIXTURE()
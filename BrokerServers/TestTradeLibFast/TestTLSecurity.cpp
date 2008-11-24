#include "stdafx.h"
#include <cfix.h>
#include "TradeLibFast.h"

using namespace TradeLibFast;

static void __stdcall Basics()
{
	const CString sym = "LVS";
	const double x = 10;
	const int s = 200;
	TLSecurity sec(sym);
	CFIX_ASSERT(sec.isValid());
	CFIX_ASSERT(sec.sym== sym);
	CFIX_ASSERT(sec.dest=="");

	
}

static void __stdcall SerializeDeserialize()
{
	// serialize
	const CString sym = "CLZ8";
	const CString ex = "NYMEX";
	TLSecurity s(sym);
	s.dest = ex;
	s.type = FUT;
	CString m = s.Serialize();

	// make sure it matches
	const CString fut = "CLZ8 FUT NYMEX";
	CFIX_ASSERT(m==fut);
	// go back to object
	TLSecurity f = TLSecurity::Deserialize(fut);
	CFIX_ASSERT(f.dest==ex);
	CFIX_ASSERT(f.sym==sym);
	CFIX_ASSERT(f.type==FUT);
}

CFIX_BEGIN_FIXTURE( TestTLSecurity )
	CFIX_FIXTURE_ENTRY( Basics )
	CFIX_FIXTURE_ENTRY( SerializeDeserialize )
CFIX_END_FIXTURE()
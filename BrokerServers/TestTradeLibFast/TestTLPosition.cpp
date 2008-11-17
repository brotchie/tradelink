#include "stdafx.h"
#include <cfix.h>
#include "TradeLibFast.h"

using namespace TradeLibFast;

static void __stdcall Basics()
{
	int a = 1;
	int b = 1;
	CFIX_ASSERT( a + b == 2 );
	TLPosition p;
	p.ClosedPL = 5;
	CFIX_ASSERT(p.ClosedPL+a == 6);
	
}

CFIX_BEGIN_FIXTURE( MyMinimalisticFixture )
	CFIX_FIXTURE_ENTRY( Basics )
CFIX_END_FIXTURE()

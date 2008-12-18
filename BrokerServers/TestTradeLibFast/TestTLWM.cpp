#include "stdafx.h"
#include <cfix.h>
#include "TradeLibFast.h"

using namespace TradeLibFast;

static void __stdcall Setup()
{

}

static void __stdcall Basics()
{


	
}


CFIX_BEGIN_FIXTURE( TestTLWM )
	CFIX_FIXTURE_SETUP( Setup )
	CFIX_FIXTURE_ENTRY( Basics )
CFIX_END_FIXTURE()
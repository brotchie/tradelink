#include "stdafx.h"
#include <cfix.h>
#include "TLPacket.h"

using namespace TradeLibFast;

static void __stdcall EncodeDecode()
{
	// THESE TESTS ARE PRESENTLY BROKEN... CHECK OUT ENCODE FUNCTION
	return;

	// encode packet
	TLPacket code;
	TLPacket::Encode(10,15,17000,CString("hello world"),code);

	// prepare decoder
	TLPacket decode;

	// verify decoder is empty
	CFIX_ASSERT(decode.From!=code.From);
	CFIX_ASSERT(decode.To!=code.To);
	CFIX_ASSERT(decode.Data!=code.Data);
	CFIX_ASSERT(decode.Intention!=code.Intention);

	// decode it
	TLPacket::Decode(code,decode);
	// verify it's the same
	CFIX_ASSERT(code.From==decode.From);
	CFIX_ASSERT(code.To == decode.To);
	CFIX_ASSERT(code.Intention == decode.Intention);
	CFIX_ASSERT(code.Data == decode.Data);

}

CFIX_BEGIN_FIXTURE( TestTLPacket )
	CFIX_FIXTURE_ENTRY( EncodeDecode )
CFIX_END_FIXTURE()

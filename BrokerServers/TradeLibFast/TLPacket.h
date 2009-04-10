#pragma once
#include "TradeLibFast.h"

namespace TradeLibFast
{

	class AFX_EXT_CLASS TLPacket
	{
	public :
		int Intention ;
		CString Data;
		static const int PACKETSIZE = 1024;
		int From;
		int To;
		CByteArray EncodedData;
		TLPacket();
		~TLPacket();
		TLPacket(TLPacket & copy);
		static void Decode(TLPacket& packet, TLPacket & decoded);
		static void Encode(int From, int To, int Intention, CString msg, TLPacket & encoded);

	};

	enum TLPacketMessage
	{
		PKT_FROM = 0,
		PKT_TO = 4,
		PKT_TYPE = 8,
		PKT_DATA = 12,
	};

}
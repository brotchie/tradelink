#include "stdafx.h"
#include "TLPacket.h"
#include "Convert.h"

namespace TradeLibFast
{

	TLPacket::TLPacket()
	{
		EncodedData.SetSize(PACKETSIZE);
		Data = CString("");
		From = 0;
		To = 0;
		Intention = OK;
	}

	TLPacket::~TLPacket()
	{
	}

	TLPacket::TLPacket(TLPacket & copy)
	{
		Data = copy.Data;
		From = copy.From;
		To = copy.To;
		Intention = copy.Intention;
		EncodedData.SetSize(copy.EncodedData.GetSize());
		copy.EncodedData.Copy(EncodedData);
	}

	int bitsToInt(const unsigned char* bits, bool little_endian = true )
	{
		int result = 0;
		if (little_endian)
		for (int n = sizeof( result ); n >= 0; n--)
		  result = (result << 8) +bits[ n ];
		else
		for (unsigned n = 0; n < sizeof( result ); n++)
		  result = (result << 8) +bits[ n ];
		return result;
	}

	void TLPacket::Decode(TLPacket& packet, TLPacket & decoded)
	{
		// get header
		decoded.From = bitsToInt(&packet.EncodedData.ElementAt(PKT_FROM));
		decoded.To = bitsToInt(&packet.EncodedData.ElementAt(PKT_TO));
		decoded.Intention = bitsToInt(&packet.EncodedData.ElementAt(PKT_TO));
		/*
		// remove header from encoded packet to leave payload
		packet.EncodedData.RemoveAt(PKT_FROM);
		packet.EncodedData.RemoveAt(PKT_TO);
		packet.EncodedData.RemoveAt(PKT_TYPE);
		*/
		// get payload and decode it
		decoded.Data = packet.EncodedData.GetData()+PKT_DATA;
	}

	void IntToBits(CByteArray & a, int offset, int number, bool little_endian = true)
	{
		if (little_endian)
			for (int n = sizeof(number); n>=0; n--)
				a[n+offset] = number >> 8;
		else 
			for (unsigned n = 0; n<sizeof(number); n++)
				a[n+offset] = number >> 8;

	}



	void TLPacket::Encode(int From, int To, int Intention, CString msg, TLPacket & tp)
	{
		
		// build object
		tp.From = From;
		tp.To = To;
		tp.Intention = Intention;
		tp.Data = msg;

		// build header
		tp.EncodedData.SetSize(PKT_DATA);
		IntToBits(tp.EncodedData,PKT_FROM,From);
		IntToBits(tp.EncodedData,PKT_TO,To);
		IntToBits(tp.EncodedData,PKT_TYPE,Intention);

		// convert msg to payload
		CByteArray msgtmp;
		msgtmp.SetSize(PACKETSIZE);
		MoveMemory(msgtmp.GetData(),(LPCSTR)msg,msg.GetLength());

		// append payload
		tp.EncodedData.Append(msgtmp);
	}

	
}
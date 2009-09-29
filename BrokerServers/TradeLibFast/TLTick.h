#pragma once

namespace TradeLibFast
{
	class AFX_EXT_CLASS TLTick
	{
	public:
		TLTick(void);
		~TLTick(void);
		CString sym;
		int symid;
		int date;
		int time;
		int size;
		int depth;
		double trade;
		double bid;
		double ask;
		int bs;
		int os;
		CString be;
		CString oe;
		CString ex;
		bool isValid();
		bool isTrade();
		bool hasBid();
		bool hasAsk();
		CString Serialize(void);
		static TLTick Deserialize(CString message);
	};

	enum TickField
    { // tick message fields from TL server
        ksymbol = 0,
        kdate,
        ktime,
        KUNUSED,
        ktrade,
        ktsize,
        ktex,
        kbid,
        kask,
        kbidsize,
        kasksize,
        kbidex,
        kaskex,
		ktdepth,
    };

}
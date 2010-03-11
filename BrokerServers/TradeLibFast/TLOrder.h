#pragma once
typedef long long int64;

namespace TradeLibFast 
{
	class AFX_EXT_CLASS TLOrder 
	{
	public:
		TLOrder(void);
		~TLOrder(void);
		int64 id;
		int size;
		double price;
		double stop;
		double trail;
		bool side;
		CString symbol;
		int date;
		int time;
		CString comment;
		CString account;
		CString exchange;
		CString security;
		CString currency;
		CString localsymbol;
		CString TIF;

		bool isValid(void);
		bool isFilled(void) { return false; }
		bool isLimit(void) { return (price!=0); }
		bool isMarket(void) { return (price==0) && (stop==0); }
		bool isStop(void) { return (stop!=0); }
		bool isTrail() { return trail!=0; }


		CString Serialize(void);
		
		static TLOrder Deserialize(CString message);
	};

	enum OrderField
	{
		oSYM = 0,
		oSIDE,
		oSIZE,
		oPRCE,
		oSTOP,
		oUSER,
		oEXCH,
		oACCT,
		oUNUSEDT,
		oCURR,
		oLSYM,
		oID,
		oTIF,
		oDate,
		oTime,
		oUNUSED,
		oTRAIL,
	};


}

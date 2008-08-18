#pragma once

namespace TradeLinkServer 
{
	class AFX_EXT_CLASS TLOrder 
	{
	public:
		TLOrder(void);
		~TLOrder(void);
		unsigned int id;
		int size;
		double price;
		double stop;
		bool side;
		CString symbol;
		int date;
		int time;
		int sec;
		CString comment;
		CString account;
		CString exchange;
		CString security;
		CString currency;
		CString localsymbol;
		CString TIF;

		bool isValid(void);
		bool isFilled(void) { return false; }


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
		oSECT,
		oCURR,
		oLSYM,
		oID,
		oTIF,
		oDate,
		oTime,
		oSec,
	};


}

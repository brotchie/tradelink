#pragma once

namespace TradeLibFast
{

	enum TLSecurityID
    {
		NIL = -1,
        STK = 0,
        OPT=1,
        FUT=2,
        CFD=3,
        FOR=4,
        FOP=5,
        WAR=6,
        FOX=7,
        IDX=8,
        BND=9,
		CASH=10,
		BAG=11,  //IB Supported combo type
    };

	enum TLSecurityField
	{
		SecSym,
		SecType,
		SecDest,
	};

	class AFX_EXT_CLASS TLSecurity
	{
	public:
		TLSecurity(CString symbol);
		TLSecurity(void);
		~TLSecurity(void);
		CString sym;
		int date;
		CString dest;
		int type;
		double strike;
		CString details;
		bool isValid();
		CString Serialize(void);
		bool hasDest();
		bool hasType();
		static TLSecurity Deserialize(CString msg);
		static LPCTSTR SecurityTypeName(int SecurityTypeID);
		static int SecurityID(CString SecurityTypeName);
		bool isPut() { return (type==OPT) && (details==CString("PUT")); }
		bool isCall() { return (type==OPT) && (details==CString("CALL")); }
	};

}




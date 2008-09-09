#pragma once

namespace TradeLinkServer
{

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
		bool isValid();
		CString Serialize(void);
		bool hasDest();
		static TLSecurity Deserialize(CString msg);
		static LPCTSTR SecurityTypeName(int SecurityTypeID);
		static int SecurityID(CString SecurityTypeName);
	};

}

	enum TLSecurityID
    {
        STK = 0,
        OPT,
        FUT,
        CFD,
        FOR,
        FOP,
        WAR,
        FOX,
        IDX,
        BND,
    };

	enum TLSecurityField
	{
		SecSym,
		SecType,
		SecDest,
	};


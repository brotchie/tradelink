#pragma once

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
		CString DefaultDest();
		bool isValid();
		CString Serialize(void);
		static TLSecurity Deserialize(CString msg);
	};

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

	CString SecurityTypeName(int SecurityTypeID);
	int SecurityID(CString SecurityTypeName);

	enum TLSecurityField
	{
		SecSym,
		SecType,
		SecDest,
	};

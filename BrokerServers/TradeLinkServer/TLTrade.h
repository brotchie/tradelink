#pragma once
#include "TLOrder.h"

namespace TradeLinkServer 
{
	class AFX_EXT_CLASS TLTrade : public TLOrder
	{
	public:
		TLTrade(void);
		~TLTrade(void);
		int xsize;
		double xprice;
		int xtime;
		int xdate;
		int xsec;

		bool isValid();
		bool isFilled();

		CString Serialize(void);
		static TLTrade Deserialize(CString message);
	};

	enum TradeField
    {
        xDate=0,
        xTime,
        xSeconds,
        tSymbol,
        tSide,
        tSize,
        tPrice,
        tComment,
        tAccount,
        tSecurity,
        tCurrency,
        tLocalSymbol,
		tID,
    };
}

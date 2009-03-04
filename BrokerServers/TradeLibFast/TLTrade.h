#pragma once
#include "TLOrder.h"

namespace TradeLibFast 
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

		bool isValid();
		bool isFilled();

		CString Serialize(void);
		static TLTrade Deserialize(CString message);
	};

	enum TradeField
    {
        xDate=0,
        xTime,
        xUNUSED,
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
		tExch,
    };
}

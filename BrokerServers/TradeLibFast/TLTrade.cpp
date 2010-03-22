#include "StdAfx.h"
#include "TLTrade.h"
#include "Util.h"

namespace TradeLibFast
{
	TLTrade::TLTrade(void)
	{
		xsize = 0;
		xtime = 0;
		xdate = 0;
		xprice = 0;
		id = 0;
	}

	TLTrade::~TLTrade(void)
	{
	}

	bool TLTrade::isValid()
	{
		return this->isFilled();
	}

	bool TLTrade::isFilled()
	{
		return (xsize*xprice!=0);
	}
	CString TLTrade::Serialize(void)
	{
		CString m;
		CString sde = this->side ? _T("True") : _T("False");
		// date,time,sec,symbol,side,size,price,comment,acc,sect,currency,localsymbol,exchange
		m.Format(_T("%i,%i,,%s,%s,%i,%f,%s,%s,%s,%s,%s,%I64d,%s"),xdate,xtime,symbol,sde,xsize,xprice,comment,account,security,currency,localsymbol,id,exchange);
		return m;
	}

	TLTrade TLTrade::Deserialize(CString message)
	{
		TLTrade t;
		std::vector<CString> r;
		gsplit(message,_T(","),r);
		t.xdate = _tstoi(r[xDate]);
		t.xtime = _tstoi(r[xTime]);
		t.symbol = r[tSymbol];
		t.side = (r[tSide].CompareNoCase(_T("True"))==0);
		t.xsize = _tstoi(r[tSize]);
		t.xprice = _tstof(r[tPrice]);
		t.comment = r[tComment];
		t.account = r[tAccount];
		t.localsymbol = r[tLocalSymbol];
		t.security = r[tSecurity];
		t.currency = r[tCurrency];
		t.id = _atoi64(r[tID]);
		t.exchange = r[tExch];
		return t;
	}
}





#include "StdAfx.h"
#include "TLOrder.h"
#include "Util.h"

namespace TradeLibFast
{

	TLOrder::TLOrder(void)
	{
		symbol = "";
		TIF = "DAY";
		size = 0;
		price = 0;
		trail = 0;
		stop = 0;
		comment = "";
		date = 0;
		time = 0;
		security = "STK";
		currency = "USD";
		account = "";
		exchange = "NYSE";
		localsymbol = "";
		id = 0;
	}

	bool TLOrder::isValid()
	{
		return (symbol!="") && (size!=0);
	}

	CString TLOrder::Serialize()
	{
		CString sde = (this->side) ? CString("True") : CString("False");
		CString m;
		// sym,side,size,price,stop,user,exch,acct,sect,curr,lsym,id,TIF,date,time,sec,trail
		m.Format(_T("%s,%s,%i,%f,%f,%s,%s,%s,%s,%s,%s,%I64d,%s,%i,%i,,%f"),symbol,sde,size,price,stop,comment,exchange,account,security,currency,localsymbol,id,TIF,date,time,trail);
		return m;
	}

	TLOrder::~TLOrder(void)
	{
	}

	TLOrder TLOrder::Deserialize(CString message)
	{
		TLOrder o;
		std::vector<CString> r;
		gsplit(message,_T(","),r);
		o.account = r[oACCT];
		o.comment = r[oUSER];
		o.currency = r[oCURR];
		o.exchange = r[oEXCH];
		o.price = _tstof(r[oPRCE].GetBuffer());
		o.security = r[oUNUSEDT];
		CString sde = r[oSIDE];
		o.side = (sde.CompareNoCase(_T("True"))==0);
		o.size = _tstoi(r[oSIZE].GetBuffer());
		o.stop = _tstof(r[oSTOP].GetBuffer());
		o.trail = _tstof(r[oTRAIL].GetBuffer());
		o.symbol = r[oSYM];
		o.localsymbol = r[oLSYM];
		o.id = _atoi64(r[oID].GetBuffer());
		o.TIF = r[oTIF];
		o.date = _tstoi(r[oDate].GetBuffer());
		o.time = _tstoi(r[oTime].GetBuffer());
		return o;
	}

}


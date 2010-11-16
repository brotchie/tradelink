
#include "stdafx.h"
#include "TLBar.h"


namespace TradeLibFast
{
	TLBar::TLBar()
	{
		symbol = CString("");
	}

	bool TLBar::isValid()
	{
		return symbol!="";
	}

	TLBar::~TLBar()
	{
	}

	CString TLBar::Serialize(TLBar b)
	{
		CString m;
		m.Format("%f,%f,%f,%f,%I64d,%i,%i,%s,%i",b.open,b.high,b.low,b.close,b.Vol,b.date,b.time,b.symbol,b.interval);
		return m;
	}
}
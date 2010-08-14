#include "stdafx.h"
#include "TLPosition.h"
#include "Util.h"

namespace TradeLibFast
{

	TLPosition::TLPosition() 
	{ 
		Symbol = "";
		AvgPrice = 0;
		Size = 0;
		ClosedPL = 0;
		Account = "";
	}

	TLPosition::TLPosition(CString symbol, double avgprice, int possize, CString acct)
	{
		Symbol = symbol;
		AvgPrice = avgprice;
		Size = possize;
		ClosedPL = 0;
		Account = acct;
	}

	TLPosition::TLPosition(CString symbol)
	{
		Symbol = symbol;
		AvgPrice = 0;
		Size = 0;
		ClosedPL = 0;
	}

	CString TLPosition::Serialize()
	{
		CString m;
		m.Format("%s,%f,%i,%f,%s",Symbol,AvgPrice,Size,ClosedPL,Account);
		return m;
	}

	TLPosition TLPosition::Deserialize(CString msg)
	{
		TLPosition pos;
		std::vector<CString> r;
		gsplit(msg,CString(","),r);
		pos.Symbol = r[psym];
		pos.AvgPrice = _tstof(r[pavg]);
		pos.Size = _tstoi(r[psiz]);
		pos.ClosedPL = _tstof(r[pcpl]);
		pos.Account = r[pact];
		return pos;
	}

	TLPosition::~TLPosition()
	{
	}
}
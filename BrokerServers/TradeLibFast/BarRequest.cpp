#include "StdAfx.h"
#include "barrequest.h"
#include "Util.h"

namespace TradeLibFast 
{
	BarRequest::BarRequest()
	{
		Symbol = CString("");
	}
	BarRequest::~BarRequest()
	{
		
	}
	BarRequest BarRequest::Deserialize(CString msg)
	{
		std::vector<CString> r;
		gsplit(msg,CString(","),r);
		BarRequest br;
		br.Symbol = r[brsym];
		br.CustomInterval = _tstoi(r[brci]);
		br.EndDate = _tstoi(r[bred]);
		br.EndTime = _tstoi(r[bret]);
		br.ID = _tstoi(r[brid]);
		br.Interval = _tstoi(r[brint]);
		br.StartDate = _tstoi(r[brsd]);
		br.StartTime = _tstoi(r[brst]);
		br.Client = r[brclient];
		return br;
	}

	bool BarRequest::isValid()
	{
		return Symbol!=CString("");

	}
}
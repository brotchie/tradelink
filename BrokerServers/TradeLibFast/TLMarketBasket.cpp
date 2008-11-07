#include "stdafx.h"
#include "TLMarketBasket.h"
#include "Util.h"

namespace TradeLibFast
{

	TLMarketBasket::TLMarketBasket()
	{
	}

	TLMarketBasket::~TLMarketBasket()
	{
	}

	CString TLMarketBasket::Serialize()
	{
		std::vector<CString> r;
		for (size_t i = 0; i<_secs.size(); i++)
			r.push_back(_secs[i].Serialize());
		CString m = gjoin(r,",");
		return m;
	}

	int TLMarketBasket::Count()
	{
		return _secs.size();
	}

	TLMarketBasket TLMarketBasket::Deserialize(CString msg)
	{
		std::vector<CString> r;
		gsplit(msg,",",r);
		TLMarketBasket mb;
		for (size_t i = 0; i<r.size(); i++)
		{
			TLSecurity sec = TLSecurity::Deserialize(r[i]);
			mb._secs.push_back(sec);
		}
		return mb;
	}

}
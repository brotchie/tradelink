#include "stdafx.h"
#include "TLMarketBasket.h"
#include "Util.h"
#include <vector>

namespace TradeLibFast
{

	TLMarketBasket::TLMarketBasket()
	{
	}

	TLMarketBasket::~TLMarketBasket()
	{
	}

	void TLMarketBasket::Add(std::vector<CString> seclist)
	{
		for (unsigned int i = 0; i< seclist.size(); i++)
			Add(seclist[i]);
	}
	
	void TLMarketBasket::Add(CString symbol) 
	{ 
		TLSecurity sec = TLSecurity::Deserialize(symbol);
		_secs.push_back(sec); 
	}

	void TLMarketBasket::Add(TLMarketBasket basket)
	{
		for (unsigned int i = 0; i<basket.Count(); i++)
			Add(basket[i]);
	}

	CString TLMarketBasket::Serialize()
	{
		std::vector<CString> r;
		for (size_t i = 0; i<_secs.size(); i++)
			r.push_back(_secs[i].Serialize());
		CString m = gjoin(r,",");
		return m;
	}

	unsigned int TLMarketBasket::Count()
	{
		return (unsigned int)_secs.size();
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
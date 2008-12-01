#pragma once
#include "TLSecurity.h"
#include <vector>
using namespace std;

namespace TradeLibFast
{
	class AFX_EXT_CLASS TLMarketBasket
	{
	public:
		TLMarketBasket(void);
		~TLMarketBasket(void);
		CString Serialize();
		static TLMarketBasket Deserialize(CString msg);
		unsigned int Count();
		TLSecurity operator[](unsigned int index) { return _secs[index]; }
		void Add(TLSecurity sec) { _secs.push_back(sec); }
		void Add(CString symbol);
		void Add(TLMarketBasket basket);
		void Add(vector<CString> seclist);

	protected:
		std::vector<TLSecurity> _secs;
	};
}


#pragma once
typedef long long int64;

namespace TradeLibFast 
{
	class AFX_EXT_CLASS TLBar
	{
	public:
		TLBar();
		~TLBar();
		CString symbol;
		double open;
		double high;
		double low;
		double close;
		int64 Vol;
		int date;
		int time;
		int interval;
		bool isValid();
		static CString Serialize(TLBar b);
	};

	enum BarFields
	{
		bfo,
		bfh,
		bfl,
		bfc,
		bfv,
		bfd,
		bft,
		bfs,
		bfi,
	};
}
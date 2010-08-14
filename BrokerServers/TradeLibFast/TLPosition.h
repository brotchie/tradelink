#pragma once

namespace TradeLibFast 
{
	class AFX_EXT_CLASS TLPosition 
	{
	public:
		CString Account;
		TLPosition(void);
		~TLPosition(void);
		TLPosition(CString sym);
		TLPosition(CString sym, double price, int size,CString acct);
		CString Symbol;
		double ClosedPL;
		double AvgPrice;
		int Size;
		CString Serialize();
		static TLPosition Deserialize(CString msg);
	};

	enum PosField
	{
		psym,
		pavg,
		psiz,
		pcpl,
		pact,
	};
}
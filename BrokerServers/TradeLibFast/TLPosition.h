#pragma once

namespace TradeLibFast 
{
	class AFX_EXT_CLASS TLPosition 
	{
	public:
		TLPosition(void);
		~TLPosition(void);
		TLPosition(CString sym);
		TLPosition(CString sym, double price, int size);
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
	};
}
#pragma once
#include "ObserverApi.h"
#include "BusinessApi.h"
#include "TradeLibFast.h"


class TLIdx : public Observer
{
// Construction
public:
	TLIdx(CString symbol,TradeLibFast::TLServer_WM* tl);   // standard constructor
	~TLIdx();
	CString m_symbol;
	CString m_StaticSymbol;
	protected:

// Implementation
protected:
	TradeLibFast::TLServer_WM* tl;

	afx_msg void OnChangeIndexSymbol();
	afx_msg void OnDynamicUpdate();
	void Load(CString symbol);
    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
    void FillInfo();

    MarketIndex* m_index;
};
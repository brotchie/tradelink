#pragma once
#include "ObserverApi.h"
#include "BusinessApi.h"
#include "TradeLibFast.h"


class AVLIndex : public Observer
{
// Construction
public:
	AVLIndex(CString symbol, int id, TradeLibFast::TLServer_WM* tl);   // standard constructor
	~AVLIndex();
	CString m_symbol;
	CString m_StaticSymbol;
	protected:

// Implementation
protected:
	TradeLibFast::TLServer_WM* tl;
	int _symid;
	int _date;

	afx_msg void OnChangeIndexSymbol();
	afx_msg void OnDynamicUpdate();
	void Load(CString symbol);
    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
    void FillInfo();

    MarketIndex* m_index;
};
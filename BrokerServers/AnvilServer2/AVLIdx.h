#pragma once
#include "ObserverApi.h"
#include "BusinessApi.h"
#include "MessageIds.h"


class TLIdx : public Observer
{
// Construction
public:
	TLIdx(CString symbol);   // standard constructor
	CString m_symbol;
	CString m_StaticSymbol;
	protected:

// Implementation
protected:

	afx_msg void OnChangeIndexSymbol();
	afx_msg void OnDynamicUpdate();
	void Load(CString symbol);
    virtual void Process(const Message* message, Observable* from, const Message* additionalInfo);
    void FillInfo();

    MarketIndex* m_index;
};
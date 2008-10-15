#include "stdafx.h"
#include "TLIndex.h"

// TLIdx message handlers

void TLIdx::Load(CString symbol)
{
    if(symbol.GetLength() != 0)
    {
        MarketIndex* index = B_FindIndex(symbol);
        if(m_index != index)
        {
            if(m_index)
            {
                m_index->Remove(this);
            }
            m_index = index;
            if(m_index)
            {
				m_symbol = m_index->GetSymbol();
				m_index->Add(this);
                FillInfo();
            }
		}
	}
	else m_symbol = "";
}


TLIdx::TLIdx(CString symbol)
{
	m_index = NULL;
	m_symbol = "";
	Load(symbol);

}

void TLIdx::OnChangeIndexSymbol() 
{
}


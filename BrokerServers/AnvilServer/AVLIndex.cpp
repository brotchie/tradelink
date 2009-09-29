#include "stdafx.h"
#include "TradeLink.h"
#include "AVLIndex.h"
#include "AVL_TLWM.h"
#include "MessageIds.h"
#include "TLTick.h"



// AVLIndex message handlers

void AVLIndex::Load(CString symbol)
{
	time_t now;
	time(&now);
	CTime ct(now);
	_date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();

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
            }
		}
	}
	else m_symbol = "";
}


AVLIndex::AVLIndex(CString symbol,int id,TradeLibFast::TLServer_WM* tlinst)
{
	_symid = id;
	m_index = NULL;
	m_symbol = "";
	Load(symbol);
	tl = tlinst;

}
AVLIndex::~AVLIndex()
{
	m_index = NULL;
	m_symbol = "";
	tl = NULL;
}

void AVLIndex::OnDynamicUpdate() 
{
    if(m_index)
    {
            FillInfo();
            m_index->Add(this);
    }
}

void AVLIndex::OnChangeIndexSymbol() 
{
}

void AVLIndex::FillInfo()
{

	double val = m_index->GetValue().toDouble();

	TradeLibFast::TLTick k;
	k.sym = m_symbol;
	k.symid = _symid;
	k.trade = val;
	k.size = -1;
	k.date = _date;
	uint hour,minute,second;
	B_GetCurrentServerNYTimeTokens(hour, minute, second);
	k.time = (hour*10000)+(minute*100) + second;
	tl->SrvGotTick(k);
}


void AVLIndex::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
    switch(message->GetType())
    {
        case M_NW2_INDEX_DETAILS:
        FillInfo();
        break;
    }
}


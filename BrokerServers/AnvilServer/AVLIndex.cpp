#include "stdafx.h"
#include "TradeLink.h"
#include "AVLIndex.h"
#include "AVL_TLWM.h"
#include "MessageIds.h"
#include "TLTick.h"



// AVLIndex message handlers

void AVLIndex::Load(CString symbol)
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
            }
		}
	}
	else m_symbol = "";
}


AVLIndex::AVLIndex(CString symbol,TradeLibFast::TLServer_WM* tlinst)
{
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

	time_t now;
	CTime ct(time(&now));
	int xd = (ct.GetYear()*10000)+(ct.GetMonth()*100)+ct.GetDay();
	int xt = (ct.GetHour()*10000)+(ct.GetMinute()*100)+ct.GetSecond();
	double val = m_index->GetValue().toDouble();
	double open = m_index->GetOpenValue().toDouble();
	double high = m_index->GetHigh().toDouble();
	double low = m_index->GetLow().toDouble();
	double close = m_index->GetCloseValue().toDouble();

	TradeLibFast::TLTick k;
	k.sym = m_index->GetSymbol();
	k.trade = val;
	k.size = -1;
	k.date = xd;
	k.time = xt;
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


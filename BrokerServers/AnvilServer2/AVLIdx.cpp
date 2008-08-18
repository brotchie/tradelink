#include "stdafx.h"
#include "AVLIdx.h"

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

void FormatDollarsAndCents(CString& dest, int dollars, int cents)
{
    char num[33]; 
	_itoa_s(dollars, num, sizeof(num), 10);
    dest += num;
    dest += ".";
    if(cents < 0)
    {
        cents = -cents;
    }
    _itoa_s(cents, num, sizeof(num), 10);
    for(unsigned int i = (unsigned int)strlen(num); i < 3; i++)
    {
        dest += "0";
    }
    dest += num;
}

void FormatMoney(CString& dest, const Money& money)
{
    FormatDollarsAndCents(dest, money.GetWhole(), money.GetThousandsFraction());
}



void TLIdx::OnChangeIndexSymbol() 
{
}

void TLIdx::FillInfo()
{
    CString tick = m_index->GetSymbol();
	tick.AppendChar(',');
	time_t now;
	CTime ct(time(&now));
	int xd = (ct.GetYear()*10000)+(ct.GetMonth()*100)+ct.GetDay();
	int xt = (ct.GetHour()*100)+ct.GetMinute();
	CString xdate;
	xdate.Format(_T("%i,"),xd);
	CString xtime;
	xtime.Format(_T ("%i,"),xt);
	tick.Append(xdate);
	tick.Append(xtime);
	CString str;
	FormatMoney(str, m_index->GetValue());
	tick.Append(str);
    str = "";
    FormatMoney(str, m_index->GetOpenValue());
	tick.AppendChar(',');
	tick.Append(str);
    str = "";
    FormatMoney(str, m_index->GetHigh());
	tick.AppendChar(',');
	tick.Append(str);
    str = "";
    FormatMoney(str, m_index->GetLow());
	tick.AppendChar(',');
	tick.Append(str);
    str = "";
    FormatMoney(str, m_index->GetCloseValue());
	tick.AppendChar(',');
	tick.Append(str);
	tick.AppendChar(',');

	// need to insert index update function here

}


void TLIdx::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
    switch(message->GetType())
    {
        case M_NW2_INDEX_DETAILS:
        FillInfo();
        break;
    }
}

void TLIdx::OnDynamicUpdate() 
{
    if(m_index)
    {
            FillInfo();
            m_index->Add(this);
    }
}
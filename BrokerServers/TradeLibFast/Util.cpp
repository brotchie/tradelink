#include "stdafx.h"
#include "Util.h"
#include <cmath>


double unpack(long i) {
	if (i==0) return 0;
	// unpacks price in form whole/fractional from a single
	// long integer, converts to double
	int w = (int)(i >> 16);
	int f = (int)(i-(w<<16));
	double dec = (double)w;
	double frac = (double)f;
	frac /= 1000;
	dec += frac;
	return dec;
}



void TLTimeNow(std::vector<int> & nowtime)
{
	time_t now;
	time(&now);
	CTime ct(now);
	int date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();
	int time = (ct.GetHour()*10000)+(ct.GetMinute()*100) + ct.GetSecond();
	nowtime.push_back(date);
	nowtime.push_back(time);
}

CString UniqueWindowName(CString rootname)
{
	HWND dest = FindWindow(NULL,(LPCSTR)(LPCTSTR)rootname);
	int i = -1;
	CString final(rootname);
	while (dest!=NULL)
	{
		i++;
		final = CString("");
		final.Format("%s.%i",rootname,i);
		dest = FindWindowA(NULL,(LPCSTR)(LPCTSTR)final);
	}
	return final;
}






CString gjoin(std::vector<CString>& vec, CString del)
{
	CString s(_T(""));
	for (size_t i = 0; i<vec.size(); i++)
			s += vec[i] + del;
	s.TrimRight(del);
	return s;
}

char* cleansvnrev(const char * dirtyrev)
{
	CString clean(dirtyrev);
	clean.Replace("$Rev: ","");
	clean.TrimRight(" $");
	return clean.GetBuffer();
}

CString SerializeIntVec(std::vector<int> input)
{
	std::vector<CString> tmp;
	for (size_t i = 0; i<input.size(); i++)
	{
		CString t; // setup tmp string
		t.Format("%i",input[i]); // convert integer into tmp string
		tmp.push_back(t); // push converted string onto vector
	}
	// join vector and return serialized structure
	return gjoin(tmp,",");
}

// adapted from Eddie Velasquez article
// http://www.codeproject.com/KB/string/tokenizer.aspx?display=PrintAll&fid=210&df=90&mpp=25&noise=3&sort=Position&view=Quick&fr=26

CTokenizer::CTokenizer(const CString& cs, const CString& csDelim):
	m_cs(cs),
	m_nCurPos(0)
{
	SetDelimiters(csDelim);
}

void CTokenizer::SetDelimiters(const CString& csDelim)
{
	for(int i = 0; i < csDelim.GetLength(); ++i)
		m_delim.set(static_cast<BYTE>(csDelim[i]));
}

bool CTokenizer::Next(CString& cs)
{
	cs.Empty();

	int nStartPos = m_nCurPos;
	while(m_nCurPos < m_cs.GetLength() && !m_delim[static_cast<BYTE>(m_cs[m_nCurPos])])
		++m_nCurPos;

	if(m_nCurPos >= m_cs.GetLength())
	{
		if (nStartPos<m_cs.GetLength())
			cs = m_cs.Mid(nStartPos,m_cs.GetLength()-nStartPos);
		return false;
	}
/*
	int nStartPos = m_nCurPos;
	while(m_nCurPos < m_cs.GetLength() && !m_delim[static_cast<BYTE>(m_cs[m_nCurPos])])
		++m_nCurPos;
*/	
	cs = m_cs.Mid(nStartPos, m_nCurPos - nStartPos);
	m_nCurPos++;

	return true;
}

CString	CTokenizer::Tail() const
{
	int nCurPos = m_nCurPos;

	while(nCurPos < m_cs.GetLength() && m_delim[static_cast<BYTE>(m_cs[nCurPos])])
		++nCurPos;

	CString csResult;

	if(nCurPos < m_cs.GetLength())
		csResult = m_cs.Mid(nCurPos);

	return csResult;
}

void gsplit(CString msg, CString del, std::vector<CString>& rec)
{
	CTokenizer tok(msg,del);
	CString m;
	while (tok.Next(m))
		rec.push_back(m);
	if (m.GetLength()>0)
		rec.push_back(m);

}


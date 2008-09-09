#include "stdafx.h"
#include "Util.h"


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
	int time = (ct.GetHour()*100)+ct.GetMinute();
	int sec = ct.GetSecond();
	nowtime.push_back(date);
	nowtime.push_back(time);
	nowtime.push_back(sec);
}

int SendMsg(int type,LPCTSTR msg,CString windname) {
	LRESULT result = 999;
	HWND hdlAnVig = FindWindowA(NULL,(LPCSTR)(LPCTSTR)windname);
	
	if (hdlAnVig) {
		COPYDATASTRUCT CD;  // windows-provided structure for this purpose
		CD.dwData=type;		// stores type of message
		int len = 0;
		len = (int)strlen((char*)msg);

		CD.cbData = len+1;
		CD.lpData = (void*)msg;	//here's the data we're sending
		result = SendMessageA(hdlAnVig,WM_COPYDATA,0,(LPARAM)&CD);
	} 
	return (int)result;
}

void gsplit(CString msg, CString del, std::vector<CString>& rec)
{
	while (msg.FindOneOf(del)!=-1)
	{
		int pos = msg.FindOneOf(del);
		CString r = msg.Left(pos);
		rec.push_back(r);
		msg = msg.Right(msg.GetLength()-(pos+1));
	}
	if (msg.GetLength()>0)
		rec.push_back(msg);
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

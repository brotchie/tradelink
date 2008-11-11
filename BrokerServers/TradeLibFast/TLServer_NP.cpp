#include "stdafx.h"
#include "TLServer_NP.h"
#include "Util.h"

namespace TradeLibFast
{

	//IMPLEMENT_DYNAMIC(TLServer_NP)

	TLServer_NP::TLServer_NP(void)
	{
	}

	TLServer_NP::~TLServer_NP()
	{
	}

	int TLServer_NP::symidx(CString symbol)
	{
		for (size_t i = 0; i<_secs.size(); i++)
			if (_secs[i].sym==symbol) return i;
		return -1;
	}

	LPCTSTR pipename(TLSecurity sec)
	{
		CString m;
		if (sec.date==0)
		{
			vector<int> dt;
			TLTimeNow(dt);
			sec.date = dt[TLdate];
		}
		m.Format("\\.\pipe\%s%i",sec.sym,sec.date);
		return (LPCTSTR)m;
	}

	int TLServer_NP::newSecurity(TLSecurity sec)
	{
		int idx = symidx(sec.sym);
		if (idx!=-1) return idx;
		HANDLE pipe = CreateNamedPipe(pipename(sec),
			PIPE_ACCESS_OUTBOUND|FILE_FLAG_OVERLAPPED,
			PIPE_TYPE_MESSAGE|PIPE_READMODE_MESSAGE|PIPE_NOWAIT,
			1,512,512,1000,NULL);

		if (pipe!=INVALID_HANDLE_VALUE)
		{
			_pipes.push_back(pipe);
			_secs.push_back(sec);
			return _secs.size()-1;
		}
	}

	void TLServer_NP::newTick(TLTick k)
	{
		

	}
}
#include "stdafx.h"
#include <cfix.h>
#include "Util.h"


static void __stdcall gsplittest()
{
	CString m;
	m.Format(_T("%s,%i,%i,,%f,%i,,%f,%f,%i,%i,,,%i"),"TST",10,100,0,0,0,0,0,0,0);

	vector<CString> r;
	gsplit(m,CString(","),r);
	CFIX_ASSERT(r.size()==14);

	// performance test
	const int MAXGSPLITS = 1000;
	vector<CString> strs;
	// start timer
	unsigned long start = GetTickCount();
	// split strings
	for (int i = 0; i<MAXGSPLITS; i++)
	{
		m.Format(_T("%s,%i,%i,,%f,%i,,%f,%f,%i,%i,,,%i"),"TST",rand(),100,0,0,0,0,0,0,0);
		vector<CString> r2;
		gsplit(m,CString(","),r2);
	}
	// stop timer
	unsigned long stop = GetTickCount();
	// elapsed
	int elapms = stop - start;
	int rate = (MAXGSPLITS/elapms)*1000;
	CFIX_LOG(L"Gsplit elapsed time(ms): %i",elapms);
	CFIX_LOG(L"Gsplits perf (splits/sec: %i",rate);
	CFIX_ASSERT(elapms<50);
}

CFIX_BEGIN_FIXTURE( TestUtil )
	CFIX_FIXTURE_ENTRY( gsplittest )
CFIX_END_FIXTURE()
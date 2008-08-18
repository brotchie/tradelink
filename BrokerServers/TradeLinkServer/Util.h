


#pragma once

#include <vector>


int SendMsg(int type,LPCTSTR msg,CString windname);
double unpack(long i);
void gsplit(CString msg, CString del, std::vector<CString>& rec);
CString gjoin(std::vector<CString>& vec, CString del);
void TLTimeNow(std::vector<int> & nowtime);
char* cleansvnrev(const char * dirtyrev);

enum TLTimeField
{
	TLdate,
	TLtime,
	TLsec,
};
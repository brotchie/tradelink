#pragma once
#include <vector>
using namespace std;

int SendMsg(int type,LPCTSTR msg,CString windname);
double unpack(long i);
void gsplit(CString msg, CString del, vector<CString>& rec);
CString gjoin(vector<CString>& vec, CString del);
void TLTimeNow(vector<int> & nowtime);
char* cleansvnrev(const char * dirtyrev);
CString SerializeIntVec(std::vector<int> input);

enum TLTimeField
{
	TLdate,
	TLtime,
};
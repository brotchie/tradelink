#pragma once
#include <vector>
#include <bitset>
using namespace std;

double unpack(long i);
void gsplit(CString msg, CString del, vector<CString>& rec);
CString gjoin(vector<CString>& vec, CString del);
void TLTimeNow(vector<int> & nowtime);
char* cleansvnrev(const char * dirtyrev);
CString SerializeIntVec(std::vector<int> input);
CString UniqueWindowName(CString rootname);

enum TLTimeField
{
	TLdate,
	TLtime,
};

class CTokenizer
{
public:
	CTokenizer(const CString& cs, const CString& csDelim);
	void SetDelimiters(const CString& csDelim);

	bool Next(CString& cs);
	CString	Tail() const;

private:
	CString m_cs;
	std::bitset<256> m_delim;
	int m_nCurPos;
};
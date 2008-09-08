#include "StdAfx.h"
#include "TLSecurity.h"
#include "Util.h"

TLSecurity::TLSecurity()
{
	this->sym = CString("");
	this->type = 0;
	this->date = 0;
}

TLSecurity::TLSecurity(CString symbol)
{
	this->sym = symbol;
	this->type = 0;
	this->date = 0;
}

TLSecurity::~TLSecurity()
{
}

bool TLSecurity::isValid()
{
	return (this->sym.CompareNoCase("")!=0);
}

TLSecurity TLSecurity::Deserialize(CString msg)
{
	std::vector<CString> rec;
	gsplit(msg,"&",rec);
	TLSecurity sec;
	sec.sym = rec[SecSym];
	bool hastype = rec[SecType].CompareNoCase("")!=0;
	bool hasdest = rec[SecDest].CompareNoCase("")!=0;
	sec.type = hastype ? SecurityID(rec[SecType]) : STK;
	sec.dest = hasdest ? rec[SecDest] : CString("");
	return sec;
}

CString TLSecurity::Serialize()
{
	std::vector<CString> rec;
	rec.push_back(this->sym);
	rec.push_back(SecurityTypeName(this->type));
	rec.push_back(this->dest);
	return gjoin(rec,"&");
}

CString TLSecurity::DefaultDest()
{
	switch (type)
	{
		case STK : return CString("NYSE");
		case FUT : return CString("GLOBEX");
	}
	return CString("");
}

	CString SecurityTypeName(int TLSecurity)
	{
		switch (TLSecurity)
		{
		case STK : return CString("STK"); 
		case OPT : return CString("OPT");
		case FUT : return CString("FUT");
		case CFD : return CString("CFD");
		case FOR : return CString("FOR");
		case FOP : return CString("FOP");
		case WAR : return CString("WAR");
		case FOX : return CString("FOX");
		case IDX : return CString("IDX");
		case BND : return CString("BND");
		}
		return "";
	}
	int SecurityID(CString TLSecurity)
	{
		if (TLSecurity=="STK") return STK;
		else if (TLSecurity=="OPT") return OPT;
		else if (TLSecurity=="FUT") return FUT;
		else if (TLSecurity=="CFD") return CFD;
		else if (TLSecurity=="FOR") return FOR;
		else if (TLSecurity=="FOP") return FOP;
		else if (TLSecurity=="WAR") return WAR;
		else if (TLSecurity=="FOX") return FOX;
		else if (TLSecurity=="IDX") return IDX;
		else if (TLSecurity=="BND") return BND;
		return -1;
	}
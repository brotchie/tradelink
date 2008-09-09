#include "StdAfx.h"
#include "TLSecurity.h"
#include "Util.h"

namespace TradeLinkServer
{

	TLSecurity::TLSecurity()
	{
		this->sym = CString("");
		this->type = 0;
		this->date = 0;
		this->dest = CString("");
	}

	TLSecurity::TLSecurity(CString symbol)
	{
		this->sym = symbol;
		this->type = 0;
		this->date = 0;
		this->dest = CString("");
	}

	TLSecurity::~TLSecurity()
	{
	}

	bool TLSecurity::hasDest()
	{
		return this->dest.CompareNoCase("")!=0;
	}

	bool TLSecurity::isValid()
	{
		return (this->sym.CompareNoCase("")!=0);
	}

	TLSecurity TLSecurity::Deserialize(CString msg)
	{
		std::vector<CString> rec;
		CString del(" ");
		bool hasdel = msg.FindOneOf(del) != -1;
		if (hasdel)
			gsplit(msg,del,rec);
		else 
			rec.push_back(msg);
		TLSecurity sec;
		sec.sym = rec[SecSym];
		if (rec.size()>2)
		{
			int f2id = SecurityID(rec[2]);
			int f1id = SecurityID(rec[1]);
			if (f1id!=-1)
			{
				sec.type = f1id;
				sec.dest = rec[2];
			}
			else if (f2id!=-1)
			{
				sec.type = f2id;
				sec.dest = rec[1];
			}
		}
		else if (rec.size()>1)
		{
			int id = SecurityID(rec[1]);
			if (id!=-1)
				sec.type = id;
		}
		return sec;
	}

	CString TLSecurity::Serialize()
	{
		std::vector<CString> rec;
		rec.push_back(this->sym);
		rec.push_back(SecurityTypeName(this->type));
		rec.push_back(this->dest);
		return gjoin(rec," ");
	}

// we're using the same security names as IB since they're so complete
	LPCTSTR TLSecurity::SecurityTypeName(int SecurityType)
	{
		LPCTSTR t = "";
		switch (SecurityType)
		{
		case STK : t = "STK"; break;
		case OPT : t = "OPT"; break;
		case FUT : t = "FUT"; break;
		case CFD : t = "CFD"; break;
		case FOR : t = "FOR"; break;
		case FOP : t= "FOP"; break;
		case WAR : t = "WAR"; break;
		case FOX : t = "FOX"; break;
		case IDX : t = "IDX"; break;
		case BND : t = "BND"; break;
		}
		return t;
	}
	int TLSecurity::SecurityID(CString TLSecurityName)
	{
		if (TLSecurityName=="STK") return STK;
		else if (TLSecurityName=="OPT") return OPT;
		else if (TLSecurityName=="FUT") return FUT;
		else if (TLSecurityName=="CFD") return CFD;
		else if (TLSecurityName=="FOR") return FOR;
		else if (TLSecurityName=="FOP") return FOP;
		else if (TLSecurityName=="WAR") return WAR;
		else if (TLSecurityName=="FOX") return FOX;
		else if (TLSecurityName=="IDX") return IDX;
		else if (TLSecurityName=="BND") return BND;
		return -1;
	}
}
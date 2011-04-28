#include "StdAfx.h"
#include "TLSecurity.h"
#include "Util.h"

namespace TradeLibFast
{

	TLSecurity::TLSecurity()
	{
		this->sym = CString("");
		this->type = -1;
		this->date = 0;
		this->dest = CString("");
		this->details = CString("");
		this->strike = 0;
	}

	TLSecurity::TLSecurity(CString symbol)
	{
		this->sym = symbol;
		this->type = -1;
		this->date = 0;
		this->dest = CString("");
		this->details = CString("");
		this->strike = 0;
	}

	TLSecurity::~TLSecurity()
	{
	}

	bool TLSecurity::hasType() { return type!=-1; }

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
		// symbol is a requirement for every type
		sec.sym = rec[SecSym];
		// check for option
		//bool isopt = (msg.Find(CString("PUT"))!=-1) || (msg.Find(CString("CALL"))!=-1);
		if (rec.size()>1 && _tstoi(rec[1])!=0)
		{
			// sym, date, details, price, ex
			sec.date = _tstoi(rec[1]);
			bool isput = msg.Find(CString("PUT"))!=-1;
			sec.details = isput ? CString("PUT") : CString("CALL");
			sec.strike = _tstof(rec[3]);
			if (rec.size()>5)
			{
				sec.dest = rec[4];
				sec.type = SecurityID(rec[5]);
			}
			else
				sec.type = SecurityID(rec[4]);

		}// see if both type and destination were specified
		else if (rec.size()>2)
		{
			// try to get type from both parameters
			int f2id = SecurityID(rec[2]);
			int f1id = SecurityID(rec[1]);
			// whichever one works, use type for that one and assume destination 
			// is other parameter
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
		// otherwise if we only ahve one extra parameters
		else if (rec.size()>1)
		{
			// try to get it's type
			int id = SecurityID(rec[1]);
			// if it works assume it's type
			if (id!=-1)
				sec.type = id;
			else // otherwise it's exchange
				sec.dest = rec[1];

		}
		return sec;
	}

	CString TLSecurity::Serialize()
	{
		std::vector<CString> rec;
		rec.push_back(this->sym);
		rec.push_back(SecurityTypeName(this->type));
		rec.push_back(this->dest);
		if (type!=STK) return gjoin(rec," ");
		return sym;
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
		case CASH: t = "CASH"; break;
		case BAG: t = "BAG"; break;
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
		else if (TLSecurityName=="CASH") return CASH;
		else if (TLSecurityName=="BAG") return BAG; //IB Supported combo type
		return -1;
	}
	
}
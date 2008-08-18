#include "StdAfx.h"
#include "TLTick.h"
#include "Util.h"

namespace TradeLinkServer
{
	TLTick::TLTick(void)
	{
		sym = "";
		size = 0;
		bs = 0;
		os = 0;
		trade = 0;
		bid = 0;
		ask = 0;
		ex = "";
		be = "";
		oe = "";
		date = 0;
		time = 0;
		sec = 0;
	}
	bool TLTick::isTrade()
	{
		return (sym!="") && (size*trade!=0);
	}
	bool TLTick::hasAsk() { return (sym!="") && (ask*os!=0); }
	bool TLTick::hasBid() { return (sym!="") && (bid*bs!=0); }
	bool TLTick::isValid()
	{
		return (sym!="") && (isTrade() || hasAsk() || hasBid());
	}
	CString TLTick::Serialize(void)
	{
		CString m;
		m.Format(_T("%s,%i,%i,%i,%f,%i,%s,%f,%f,%i,%i,%s,%f"),sym,date,time,sec,trade,size,ex,bid,ask,bs,os,be,oe);
		return m;
	}
	TLTick TLTick::Deserialize(CString message)
	{
		TLTick k;
		std::vector<CString> r;
		gsplit(message,_T(","),r);
		k.sym = r[ksymbol];
		k.date = _tstoi(r[kdate]);
		k.time = _tstoi(r[ktime]);
		k.sec = _tstoi(r[ksec]);
		k.trade = _tstof(r[ktrade]);
		k.size = _tstoi(r[ktsize]);
		k.ex = r[ktex];
		k.bid = _tstof(r[kbid]);
		k.ask = _tstof(r[kask]);
		k.bs = _tstoi(r[kbidsize]);
		k.os = _tstoi(r[kasksize]);
		k.be = r[kbidex];
		k.oe = r[kaskex];
		return k;
	}

	TLTick::~TLTick(void)
	{

	}


}



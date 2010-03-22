
#include "Stdafx.h"
#include "AnvilUtil.h"



long MoneyToPacked(Money m)
{
	long price = 0;
	int w = m.GetWhole();
	short f = m.GetThousandsFraction();
	price = (w<<16) + f;//store whole part in upper word and frac in lower
	return price;
}


CString TIFName(unsigned int tifid)
{
	CString name = "DAY";
	switch (tifid)
	{
		case TIF_GTC: name="GTC"; break;
		case TIF_DAY: name="DAY";break;
		case TIF_OPENING:name="OPG";break;
	}
	return name;

}

unsigned int TIFId(CString name)
{
	unsigned int tif = TIF_DAY;

	if (name=="GTC")
		tif = TIF_GTC;
	else if (name=="OPG")
		tif = TIF_OPENING;
	else if (name=="DAY")
		tif = TIF_DAY;
	return tif;
}

CString DestExchangeName(unsigned int exchangeid)
{
	CString ex = "";
	switch (exchangeid)
	{
	case ExecExch_ANY:
		ex="";
		break;
	case ExecExch_ASE:
		ex="ASE";
		break;
	case ExecExch_NYS:
		ex="NYS";
		break;
	case ExecExch_NAS:
		ex="NAS";
		break;
	case ExecExch_BSE:
		ex="BSE";
		break;
	case ExecExch_CIN:
		ex="CIN";
		break;
	case ExecExch_CSE:
		ex="CSE";
		break;
	case ExecExch_PSE:
		ex="PSE";
		break;
	case ExecExch_CBO:
		ex = "CBO";
		break;
	case ExecExch_PHS:
		ex = "PHS";
		break;
	case ExecExch_ISE:
		ex = "ISE";
		break;
	case ExecExch_ADF:
		ex = "ADF";
		break;
	case ExecExch_NSD:
		ex = "NSD";
		break;
	}
	return ex;
}


CString SymExchangeName(unsigned int exid)
{
	CString ex = CString("");
	switch (exid)
	{
			case NASDAQ:
		ex = "NSDQ";
		break;
	case NYSE:
		ex = "NYSE";
		break;

    case AMEX:
		ex = "AMEX";
		break;

    case ARCA:
		ex = "ARCA";
		break;

    case CBOE:
		ex = "CBOE";
		break;
	}
	return ex;
}

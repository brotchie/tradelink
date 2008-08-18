
#include "stdafx.h"
#include "AnvilUtil.h"

char* TIFName(int tifid)
{
	CString name = "DAY";
	switch (tifid)
	{
		case TIF_GTC: name="GTC"; break;
		case TIF_DAY: name="DAY";break;
		case TIF_OPENING:name="OPG";break;
	}
	return name.GetBuffer();

}

int TIFId(CString name)
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

char* ExchangeName(int exchangeid)
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
	return ex.GetBuffer();
}
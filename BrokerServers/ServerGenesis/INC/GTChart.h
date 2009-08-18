/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTChart.h: interface for the GTChart class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTChart.h
	\brief interface for the GTChart class. also defines CArrayCharts, GTChart2, CListStudies
 */

#if !defined(__GTCHART_H__)
#define __GTCHART_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTChart32.h"
#include "GTAPI_API.h"
#include "GTPtrList.h"

inline int CalcChartBarIndex(int nSeconds, int nChartType)
{
	if((nSeconds % nChartType) == 0)
		return nSeconds / nChartType;
	else
		return nSeconds / nChartType + 1;
}

/*! \ingroup cpp
*/
/*! \struct GTChart
	\brief One chart information. The same to GTChart32 or tagGTChart32.

	\copydoc tagGTChart32
*/
#define GTChart GTChart32

/*
struct GTChart : public GTChart32 
{
};
*/
/*! \struct GTChart2
	\brief [Internal used] One chart information. The same to GTChart232 or tagGTChart232.

	\copydoc tagGTChart232
*/
struct GTChart2 : public GTChart232
{
public:
	GTChart2()
	{
	}

	GTChart2(const GTChart32 &rcd)
	{
		*this = rcd;
	}

	GTChart2(const GTPrint32 &rcd, int nChartType)
	{
		gdate.dwDate = 0;
		minutes = CalcChartBarIndex((rcd.gtime.chHour * 60 + rcd.gtime.chMin) * 60 + rcd.gtime.chSec, nChartType) * nChartType;

		for(int i = valUser0; i <= valMax_Value; ++i)
			dblValue[i] = 0;

		dblValue[valOpen]	= rcd.dblPrice;
		dblValue[valClose]	= rcd.dblPrice;
		dblValue[valHigh]	= rcd.dblPrice;
		dblValue[valLow]	= rcd.dblPrice;
		dblValue[valVolume]	= rcd.dwShares;
	}

	GTChart2 &operator=(const GTChart32 &rcd)
	{
		gdate.dwDate = rcd.gdate.dwDate;
		minutes = rcd.gtime.chHour * 60 + rcd.gtime.chMin;

		for(int i = valUser0; i <= valMax_Value; ++i)
			dblValue[i] = 0;

		dblValue[valOpen]	= rcd.dblOpen;
		dblValue[valClose]	= rcd.dblClose;
		dblValue[valHigh]	= rcd.dblHigh;
		dblValue[valLow]	= rcd.dblLow;
		dblValue[valVolume]	= rcd.dwVolume;

		return *this;
	}

	GTChart2 &Merge(const GTChart232 &rcd)
	{
		minutes = rcd.minutes;
		dblValue[valClose] = rcd.dblValue[valClose];
		dblValue[valVolume] += rcd.dblValue[valVolume];

		if(dblValue[valHigh]	< rcd.dblValue[valHigh])
			dblValue[valHigh]	= rcd.dblValue[valHigh];
		if(dblValue[valLow]	> rcd.dblValue[valLow])
			dblValue[valLow]	= rcd.dblValue[valLow];

		return *this;
	}
};

class GTStudy;

typedef CList<GTStudy *, GTStudy *>	CListStudies;
typedef CArray<GTChart2, const GTChart2 &> CArrayCharts;

/*! \struct GTCharts
	\brief The list for chart information. 

	The API can generate the charts from the prints automatically. To use the API with the charts, you can:
	- 1. Create the GTStock as before;
	- 2. Set GTStock::m_bChartFromPrint=TRUE;
	- 3. Read from the GTStock::m_charts, which is a GTCharts object. The method GTCharts::GetValue() can be very helpful.
	
	GTCharts is a CArray object. So all the method of CArray can be applied.
*/
class GTAPI_API GTCharts : public CArrayCharts
{
protected:
	BOOL			m_bTransferring;
	BOOL			m_nChartType;		// Seconds

public:
	CListStudies	m_studies;

public:
	GTCharts();
	virtual ~GTCharts();

	virtual void ResetContent();
	virtual int Dump(FILE *fp, int nLevel) const;

public:
	int Refresh();
	int Display();

	int Add(const GTPrint32 &rcd);
	int Add(const GTChart32 &rcd);
	int Add(const GTChart2 &rcd);

public:
	int CalcStudiesAll();
	int CalcStudies(int index);

public:
	double GetValue(int nValIdx, int nIndex) const
		{ return GetAt(nIndex).dblValue[nValIdx]; }

protected:
	int AddPrint(const GTPrint32 &rcd);
};

#endif // !defined(__GTCHART_H__)

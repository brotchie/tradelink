/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

#pragma once

#include "GTChart.h"

//! \cond INTERNAL

class GTAPI_API GTStudy
{
public:
	GTCharts &m_chart;

public:
	int m_nPeriod;
	int m_nSrcIdx;
	int m_nValIdx;

public:
	GTStudy(GTCharts &chart);
	virtual ~GTStudy();

	double GetSrc(int index) const
		{ assert(0 <= index && index < m_chart.GetSize()); return m_chart[index].dblValue[m_nSrcIdx]; }

	double GetVal(int index) const
		{ assert(0 <= index && index < m_chart.GetSize()); return m_chart[index].dblValue[m_nValIdx]; }
	double &GetVal(int index)
		{ assert(0 <= index && index < m_chart.GetSize()); return m_chart[index].dblValue[m_nValIdx]; }

public:
	virtual int Dump(FILE *fp, int nLevel) const;
	virtual int CalcAll();
	virtual int CalcAt(int index);
};
//! \endcond


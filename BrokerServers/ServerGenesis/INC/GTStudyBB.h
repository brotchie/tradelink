/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

#pragma once
#include "GTStudy.h"

//! \cond INTERNAL

class GTAPI_API GTStudyBB :
	public GTStudy
{
public:
	GTStudyBB(GTCharts &chart);
	virtual ~GTStudyBB(void);

public:
	virtual int CalcAll();
	virtual int CalcAt(int nIndex);

public:
	static double CalcStudy(const GTCharts &chart, int nPeriod, int nSrcIdx, int nIndex, double *pdblAvg);
	static double CalcStdev(const GTCharts &chart, int nSrcIdx, int nBegin, int nEnd, double dblAvg);
};

//! \endcond

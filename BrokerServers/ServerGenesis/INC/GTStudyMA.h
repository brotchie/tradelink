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
class GTAPI_API GTStudyMA :
	public GTStudy
{
public:
	GTStudyMA(GTCharts &chart);
	virtual ~GTStudyMA(void);

public:
	virtual int Dump(FILE *fp, int nLevel) const;
	virtual int CalcAll();
	virtual int CalcAt(int index);

public:
	static double CalcStudy(const GTCharts &chart, int nPeriod, int nSrcIdx, int index);
};
//! \endcond

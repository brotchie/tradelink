#pragma once
#if !defined(LS_L_CHART_H)
#define LS_L_CHART_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

namespace LightspeedTrader
{

class __declspec(novtable) L_ChartPoint
{
public:
	virtual double L_Open() const = 0;
	virtual double L_High() const = 0;
	virtual double L_Low() const = 0;
	virtual double L_Close() const = 0;
	virtual long long L_Volume() const = 0;
	virtual time_t L_Time() const = 0;
};

class __declspec(novtable) L_Chart
{
public:
	virtual size_t L_NumPoints() const = 0;
	virtual L_ChartPoint const *L_GetPoint(size_t n) const = 0;
};

} // namespace LightspeedTrader
#endif // !defined(LS_L_CHART_H)


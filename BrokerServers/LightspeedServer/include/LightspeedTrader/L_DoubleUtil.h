#pragma once
#if !defined(LS_L_DOUBLEUTIL_H)
#define LS_L_DOUBLEUTIL_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.


namespace LightspeedTrader
{

double const ls_epsilon = 0.00000001;

inline bool ls_neq(double lhs, double rhs)
{
	return (lhs < rhs - ls_epsilon) || (lhs > rhs + ls_epsilon);
}

inline bool ls_eq(double lhs, double rhs)
{
	return (lhs > rhs - ls_epsilon) && (lhs < rhs + ls_epsilon);
}

inline bool ls_gt(double lhs, double rhs)
{
	return lhs > rhs + ls_epsilon;
}

inline bool ls_geq(double lhs, double rhs)
{
	return lhs > rhs - ls_epsilon;
}

inline bool ls_lt(double lhs, double rhs)
{
    return lhs < rhs - ls_epsilon;
}

inline bool ls_leq(double lhs, double rhs)
{
	return lhs < rhs + ls_epsilon;
}

inline double ls_round(double p, int prec)
{
	double factor;
	double factorR;
	__int64 round = p >= 0.0 ? 5 : -5;

	switch (prec)
	{
	case 0:
		return double((__int64(p * 10.0) + round) / __int64(10));
	case 1:
		factor = 10.0;
		factorR = 100.0;
		break;
	case 2:
		factor = 100.0;
		factorR = 1000.0;
		break;
	case 3:
		factor = 1000.0;
		factorR = 10000.0;
		break;
	case 4:
		factor = 10000.0;
		factorR = 100000.0;
		break;
	case 5:
		factor = 100000.0;
		factorR = 1000000.0;
		break;
	case 6:
		factor = 1000000.0;
		factorR = 10000000.0;
		break;
	case 7:
		factor = 10000000.0;
		factorR = 100000000.0;
		break;
	case 8:
		factor = 100000000.0;
		factorR = 1000000000.0;
		break;
	default:
		return p;
	}
	return double((__int64(p * factorR) + round) / __int64(10)) / factor;
}

}

#endif // !defined(LS_L_DOUBLEUTIL_H)


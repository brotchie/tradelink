/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GDate0.h
    \brief interface for the GDate0 class.
 */

#if !defined(__GDATE0_H__)
#define __GDATE0_H__

#include "../Inc32/GDate32.h"

/*! \struct GDate0
	\brief Date formated in GTAPI.

	\copydoc tagGDate32

	Some useful operator also defined here.
*/
struct GDate0 : public GDate32
{
public:

public:
	BOOL operator>(const GDate32 &g) const
		{ return dwDate > g.dwDate; }
	BOOL operator<(const GDate32 &g) const
		{ return dwDate < g.dwDate; }

	BOOL operator!=(const GDate32 &g) const
		{ return dwDate - g.dwDate; }
	BOOL operator==(const GDate32 &g) const
		{ return dwDate == g.dwDate; }

	BOOL operator>=(const GDate32 &g) const
		{ return dwDate >= g.dwDate; }
	BOOL operator<=(const GDate32 &g) const
		{ return dwDate <= g.dwDate; }
};

#endif//__GDATE0_H__

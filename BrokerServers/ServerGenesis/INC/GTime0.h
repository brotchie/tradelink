/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTime0.h
	\brief interface for the GTime0 class.
 */

#if !defined(__GTIME0_H__)
#define __GTIME0_H__

#include "../Inc32/GTime32.h"

/**	\class GTime0
 *	\brief Time formated in GTAPI.
 *
 *	\copydoc tagGTime32
 *	Some useful methods are defined.
 */
struct GTime0 : public GTime32
{
public:

public:
	/**
	 * \brief Get the minutes betwwen 0:0:0 and this time.
	 * \return The minutes between.
	 */
	int GetMinutes() const
	{
		return chHour * 60 + chMin;
	}
	/**
	 * \brief Get the seconds betwwen 0:0:0 and this time.
	 * \return The seconds between.
	 */
	int GetSeconds() const
	{
		return ((chHour * 60 + chMin) * 60 + chSec);
	}
	/**
	 * \brief Get how many 1/100 seconds betwwen 0:0:0 and this time.
	 * \return The 1/100 seconds between.
	 */
	int GetSeconds100() const
	{
		return ((chHour * 60 + chMin) * 60 + chSec) * 100 + chSec100;
	}

	/**
	 * \brief Get the difference in seconds.
	 * \param [in] g The time to compare.
	 * \return The difference in seconds.
	 * \remark This  minuses the \c g.
	 */
	int DiffSeconds(const GTime0 &g) const
	{
		return GetSeconds() - g.GetSeconds();
	}
};

//! @name GTime32 comparison
//@{
//! \brief GTime32 comparison operator
inline int operator>(const GTime32 &g1, const GTime32 &g2)
	{ return g1.dwTime > g2.dwTime; }
inline int operator<(const GTime32 &g1, const GTime32 &g2)
	{ return g1.dwTime < g2.dwTime; }

inline int operator!=(const GTime32 &g1, const GTime32 &g2)
	{ return g1.dwTime - g2.dwTime; }
inline int operator==(const GTime32 &g1, const GTime32 &g2)
	{ return g1.dwTime == g2.dwTime; }

inline int operator>=(const GTime32 &g1, const GTime32 &g2)
	{ return g1.dwTime >= g2.dwTime; }
inline int operator<=(const GTime32 &g1, const GTime32 &g2)
	{ return g1.dwTime <= g2.dwTime; }
//@}
#endif//__GTIME0_H__

/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTPending.h: interface for the GTPending class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTPending.h
	\brief interface for the GTPending class.
 */

#if !defined(__GTPENDING_H__)
#define __GTPENDING_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/*! \ingroup cpp
*/
/*! \class GTPending
    \brief Pending order information. The same as GTPending32 or tagGTPending32.

	\copydoc tagGTPending32
*/
typedef GTPending32 GTPending;

/*
struct GTPending : public GTPending32
{
	GTPending &operator=(const GTPending32 &user32)
	{
		*(GTPending32 *)this = user32;
		return *this;
	}
};
*/
typedef CMap<long, long, GTPending, const GTPending &>	CMapGTPending;
typedef CList<GTPending, const GTPending &>				GTPendingList;

int GetDisplayShares(const GTPending &pending);

int Dump(FILE *fp, const CMapGTPending &pendings, int nLevel);
int Dump(FILE *fp, const GTPending &pending, int nLevel);

#endif // !defined(__GTPENDING_H__)

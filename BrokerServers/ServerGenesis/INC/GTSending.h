/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTSending.h: interface for the GTSending class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTSending.h
	\brief interface for the GTSending class.
 */

#if !defined(__GTENTRY_H__)
#define __GTENTRY_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTSending32.h"

/*! \ingroup cpp
*/
/*! \struct GTSending
	\brief GTSending is the alias of GTSending32 and tagGTSending32. 

	@copydoc tagGTSending32
*/
typedef GTSending32 GTSending;

/*
struct GTSending : public GTSending32
{
	GTSending &operator=(const GTSending32 &user32)
	{
		*(GTSending32 *)this = user32;
		return *this;
	}
};
*/

// Map SeqNo to GTSending
typedef CMap<long, long, GTSending, const GTSending &>	CMapGTSending;

int Dump(FILE *fp, const CMapGTSending &pendings, int nLevel);
int Dump(FILE *fp, const GTSending &pending, int nLevel);

#endif // !defined(__GTENTRY_H__)

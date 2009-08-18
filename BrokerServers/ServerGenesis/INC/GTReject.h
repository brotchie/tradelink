/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTReject.h: interface for the GTReject class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTReject.h
	\brief interface for the GTReject class.
 */

#if !defined(__GTREJECT_H__)
#define __GTREJECT_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTReject32.h"

/*! \ingroup cpp
*/
/*! \class GTReject
    \brief GTReject is the alias of GTReject32 and tagGTReject32. 

	@copydoc tagGTReject32
*/
typedef GTReject32 GTReject;

/*
struct GTReject : public GTReject32
{
	GTReject &operator=(const GTReject32 &user32)
	{
		*(GTReject32 *)this = user32;
		return *this;
	}
};
*/

typedef CMap<long, long, GTReject, const GTReject &>	CMapGTReject;

#endif // !defined(__GTREJECT_H__)

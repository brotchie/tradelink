/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTCancel.h: interface for the GTCancel class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTCancel.h
	\brief interface for the GTCancel class. also defines CMapGTCancel, GTCancelList
 */

#if !defined(__GTCANCEL_H__)
#define __GTCANCEL_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTCancel32.h"

/*! \ingroup cpp
*/
/*! \struct GTCancel
	\brief The cancelling or cancelled order information. The same to GTCancel32 or tagGTCancel32.

	\copydoc tagGTCancel32
*/
typedef GTCancel32 GTCancel;
/*
struct GTCancel : public GTCancel32
{
	GTCancel &operator=(const GTCancel32 &user32)
	{
		*(GTCancel32 *)this = user32;
		return *this;
	}
};
*/
typedef CMap<long, long, GTCancel, const GTCancel &>	CMapGTCancel;
typedef CList<long, long>	GTCancelList;

#endif // !defined(__GTCANCEL_H__)

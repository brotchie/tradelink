/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTRejectCancel.h: interface for the GTRejectCancel class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTRejectCancel.h
	\brief interface for the GTRejectCancel class.
 */

#if !defined(__GTREJECTCANCEL_H__)
#define __GTREJECTCANCEL_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTRejectCancel32.h"
/*! \ingroup cpp
*/
/*! \struct GTRejectCancel
	\brief GTRejectCancel is the alias of GTRejectCancel32 and tagGTRejectCancel32. 

	@copydoc tagGTRejectCancel32
*/
typedef GTRejectCancel32 GTRejectCancel;
/*
struct GTRejectCancel : public GTRejectCancel32
{
	GTRejectCancel &operator=(const GTRejectCancel32 &user32)
	{
		*(GTRejectCancel32 *)this = user32;
		return *this;
	}
};
*/

typedef CMap<long, long, GTRejectCancel, const GTRejectCancel &>	CMapGTRejectCancel;
typedef CList<long, long>	GTRejectCancelList;

#endif // !defined(__GTREJECTCANCEL_H__)

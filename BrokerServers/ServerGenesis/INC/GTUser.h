/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTUser.h: interface for the GTUser class.
//
//////////////////////////////////////////////////////////////////////

/*! \file GTUser.h
	\brief interface for the GTUser class.
 */

#if !defined(__GTUSER_H__)
#define __GTUSER_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTUser32.h"
/*! \ingroup cpp
*/
/*! \class GTUser
    \brief User Information Class. The same as GTUser32 or tagGTUser32.

	
	@copydoc tagGTUser32
*/
typedef GTUser32 GTUser;

/*
struct GTUser : public GTUser32
{
	GTUser &operator=(const GTUser32 &user32)
	{
		*(GTUser32 *)this = user32;
		return *this;
	}
};
*/
#endif // !defined(__GTUSER_H__)

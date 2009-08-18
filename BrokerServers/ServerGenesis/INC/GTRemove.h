/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTRemove.h: interface for the GTRemove class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTRemove.h
	\brief interface for the GTRemove class.
 */

#if !defined(__GTREMOVE_H__)
#define __GTREMOVE_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTRemove32.h"

/*! \ingroup cpp
*/
/*! \struct  GTRemove
	\brief GTRemove is the alias of GTRemove32 and tagGTRemove32. 

	@copydoc tagGTRemove32
*/
typedef GTRemove32 GTRemove;
/*
struct GTRemove : public GTRemove32
{
	GTRemove &operator=(const GTRemove32 &user32)
	{
		*(GTRemove32 *)this = user32;
		return *this;
	}
};
*/
typedef CMap<long, long, GTRemove, const GTRemove &>	CMapGTRemove;

#endif // !defined(__GTREMOVE_H__)

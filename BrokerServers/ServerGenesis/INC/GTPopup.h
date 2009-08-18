/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTPopup.h: interface for the GTPopup class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTPopup.h
	\brief interface for the GTPopup class.
 */

#if !defined(__GTPOPUP_H__)
#define __GTPOPUP_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTPopup32.h"

/*! \class GTPopup
	\brief GTPopup is the alias of GTPopup32 and tagGTPopup32.

	@copydoc tagGTPopup32
*/
typedef GTPopup32 GTPopup;

/*
struct GTPopup : public GTPopup32
{
	GTPopup &operator=(const GTPopup32 &user32)
	{
		*(GTPopup32 *)this = user32;
		return *this;
	}
};
*/

#endif // !defined(__GTPOPUP_H__)

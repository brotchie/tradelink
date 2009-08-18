/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTChat.h: interface for the GTChat class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTChat.h
	\brief interface for the GTChat class.
 */

#if !defined(__GTCHAT_H__)
#define __GTCHAT_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTChat32.h"

/*! \struct GTChat
	\brief The order information. The same to GTChat32 or tagGTChat32.

	\copydoc tagGTChat32
*/
typedef GTChat32 GTChat;

/*
struct GTChat : public GTChat32
{
	GTChat &operator=(const GTChat32 &user32)
	{
		*(GTChat32 *)this = user32;
		return *this;
	}
};
*/

#endif // !defined(__GTCHAT_H__)

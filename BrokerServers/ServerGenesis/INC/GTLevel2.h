/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTLevel2.h: interface for the GTLevel2 class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTLevel2.h
	\brief interface for the GTLevel2 class.
 */

#if !defined(__GTLEVEL2_H__)
#define __GTLEVEL2_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTLevel232.h"

#define LEVEL2_CONNECTED		1
#define LEVEL2_DISCONNECTED		2

#define LEVEL2_RECORD			11
#define LEVEL2_REFRESH			12
#define LEVEL2_DISPLAY			13
#define LEVEL2_CLEAR			14

/*! \ingroup cpp
*/
/*! \struct GTLevel2
	\brief The level 2 information. Has the same structure as GTLevel232 or tagGTLevel232

	\copydoc tagGTLevel232

	Some operations are added.  Within those, IsMM, IsECN, IsTotalView and IsOpenView might be useful.
*/
struct GTLevel2 : public GTLevel232
{
	GTLevel2 &operator=(const GTLevel232 &user32)
	{
		*(GTLevel232 *)this = user32;
		return *this;
	}

	BOOL IsMM() const										//!< Marketmaker?
		{ return (bECN == FALSE && bBook == FALSE); }
	BOOL IsECN() const										//!< ECN or Book?
		{ return (bECN || bBook); }
	BOOL IsTotalView() const								//!< Total View?
		{ return bTotalView; }
	BOOL IsOpenView() const									//!< Open View?
		{ return bOpenView; }	
};

#endif // !defined(__GTLEVEL2_H__)

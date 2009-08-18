/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTAccount.h: interface for the GTAccount class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTAccount.h
    \brief interface for the GTAccount class.
 */

#if !defined(__GTACCOUNT_H__)
#define __GTACCOUNT_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTAccount32.h"

/*! \ingroup cpp
*/
/**	\class GTAccount
 *	\brief The account structure. Alias of GTAccount32 and tagGTAccount32.
 *	
	\copydoc tagGTAccount32
 */
typedef GTAccount32 GTAccount;

/*
struct GTAccount : public GTAccount32
{
	GTAccount &operator=(const GTAccount32 &user32)
	{
		*(GTAccount32 *)this = user32;
		return *this;
	}
};
*/
typedef CMap<CString, LPCSTR, GTAccount, const GTAccount &>	CMapGTAccount;

#endif // !defined(__GTACCOUNT_H__)

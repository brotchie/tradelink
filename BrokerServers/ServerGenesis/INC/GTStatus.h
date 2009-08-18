/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTStatus.h: interface for the GTStatus class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTStatus.h
	\brief interface for the GTStatus class.
 */

#if !defined(__GTStatus_H__)
#define __GTStatus_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTStatus32.h"
/*! \ingroup cpp
*/
/*! \struct GTStatus
	\brief GTStatus is the alias of GTStatus32 and tagGTStatus32. 

	@copydoc tagGTStatus32
*/
typedef GTStatus32 GTStatus;
/*
struct GTStatus : public GTStatus32
{
	GTStatus &operator=(const GTStatus32 &status32)
	{
		*(GTStatus32 *)this = status32;
		return *this;
	}
};
*/

typedef GTIOIStatus32 GTIOIStatus;

#ifdef __cplusplus
extern "C"
{
#endif//__cplusplus
int WINAPI ScanIOIStatus(GTIOIStatus32 &ioi, const GTStatus32 &status);
#ifdef __cplusplus
}
#endif//__cplusplus

#endif // !defined(__GTStatus_H__)

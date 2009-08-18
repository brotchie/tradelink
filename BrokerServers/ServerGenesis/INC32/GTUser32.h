/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTUser32.h: interface for the GTUser32 class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTUser32.h
	\brief interface for the GTUser32 class.
 */

#if !defined(__GTUSER32_H__)
#define __GTUSER32_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "GTConst.h"

#pragma pack(8)
/*! \ingroup c
*/
/*!	\class GTUser32
	\brief User Information Structure

	\copydoc tagGTUser32 
 */
/*! \typedef typedef tagGTUser32 GTUser32 */
/*! \struct tagGTUser32
    \brief User Information Structure

	Detailed user information after login.
*/
typedef struct tagGTUser32
{
	char		szUserID[LEN_USER_ID + 1];		//!< User ID 
	char		szUserName[LEN_USERNAME + 1];	//!< User Name 
	char		szPassword[LEN_PASSWORD + 1];	//!< Password 
	char		szEntID[LEN_ENTID + 1];			//!< Entitlement ID
	BOOL		bAdmin;							//!< Administrator or not
	BOOL		bNoSOES;						//!< SOES is enabled or not
	int			nUserType;						//!< User Type: TEST, REAL or TRAIN

	short		nLogExecID;						//!< Executor ID
	int			nLastLoginDate;					//!< Last login date 
	int			nLastLoginTime;					//!< Last login time 
	int			nLastLocalDate;					//!< Last login local date 
	int			nLastLocalTime;					//!< Last login local time 
}GTUser32;
#pragma pack()

#endif // !defined(__GTUSER32_H__)

/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTChat32.h: interface for the GTChat class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTChat32.h
	\brief interface for the GTChat32 class.
 */

#if !defined(__GTCHAT32_H__)
#define __GTCHAT32_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "GTConst.h"

/*! \ingroup c
*/
/*! \struct GTChat32
	\brief The chat message, Alias of tagGTChat32

	\copydoc tagGTChat32
*/
/*! \typedef typedef tagGTChat32 GTChat32
*/
/*! \struct tagGTChat32
	\brief The chat message
	
	When someone want to talk to you, he can send some chat to the executor. The executor then sends event to trigger OnExecMsgChat callback, and
	deliver the chat to you.

	\sa GTSession::OnExecMsgChat GTAPI_MSG_SESSION_OnExecMsgChat
*/
typedef struct tagGTChat32
{
	char	szUserFm[LEN_USER_ID + 1];
	char	szUserTo[LEN_USER_ID + 1];
	short	nLevel;
	char	szText[381];	// text
}GTChat32;

#endif // !defined(__GTCHAT32_H__)

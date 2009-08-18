/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTExecMsg.h: interface for the GTExecMsg class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTExecMsg.h
	\brief interface for the GTExecMsg class which has GTEXECMSG.
 */

#if !defined(__GTEXECMSG_H__)
#define __GTEXECMSG_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTExecMsg32.h"

#include "GTAPI_API.h"
/*!	\struct GTEXECMSG
	\brief The raw message format from executor. 

	\copydoc tagGTExecMsg32

	One assign operator is defined.
	\sa GTExecMsg32
*/
struct GTEXECMSG : public GTExecMsg32
{
	GTEXECMSG &operator=(const GTExecMsg32 &msg)
	{
		*(GTExecMsg32 *)this = msg;
		return *this;
	}
};

/*!	\class GTExecMsg
	\brief The raw message from executor. 

	This encloses a message from executor. Used in OnExecMsg callback. API will then generate some new event according to the type of the message.

	\sa GTSession::OnExecMsg GTAPI_MSG_SESSION_OnExecMsg GTEXECMSG GTExecMsg32
 */
class GTAPI_API GTExecMsg  
{
public:
	GTEXECMSG m_msg;

public:
	GTExecMsg();
	virtual ~GTExecMsg();
};

#endif // !defined(__GTEXECMSG_H__)

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

#if !defined(__GTEXECMSG23_H__)
#define __GTEXECMSG23_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/*! \file GTExecMsg32.h
	\brief interface for the GTExecMsg32 class.
 */

#include "GTErrMsg32.h"
#include "GTChat32.h"
#include "GTPopup32.h"
#include "GTServerState32.h"

#include "GTUser32.h"
#include "GTAccount32.h"

#include "GTOpenPosition32.h"
#include "GTTrade32.h"
#include "GTPending32.h"
#include "GTSending32.h"
#include "GTCancel32.h"
#include "GTReject32.h"
#include "GTRemove32.h"
#include "GTRejectCancel32.h"
#include "GTStatus32.h"

//! @name Message type
//@{
#define EXECMSG_TYPE_RECORD			0
#define EXECMSG_TYPE_CONNECTED		1
#define EXECMSG_TYPE_DISCONNECTED	2
#define EXECMSG_TYPE_SERVERSTATE	3
#define EXECMSG_TYPE_ERRMSG			4
#define EXECMSG_TYPE_CHAT			5
#define EXECMSG_TYPE_POPUP			6

#define EXECMSG_TYPE_LOGIN			11
#define EXECMSG_TYPE_LOGGEDIN		12
#define EXECMSG_TYPE_LOGOUT			13

#define EXECMSG_TYPE_USER			21
#define EXECMSG_TYPE_ACCOUNT		22

#define EXECMSG_TYPE_OPENPOSITION	31
#define EXECMSG_TYPE_TRADE			32
#define EXECMSG_TYPE_PENDING		33
#define EXECMSG_TYPE_SENDING		34
#define EXECMSG_TYPE_CANCELING		35
#define EXECMSG_TYPE_CANCEL			36
#define EXECMSG_TYPE_REJECT			37
#define EXECMSG_TYPE_REMOVE			38
#define EXECMSG_TYPE_REJECTCANCEL	39
#define EXECMSG_TYPE_STATUS			40
//@}
/*! \struct GTExecMsg32
	\brief The message from executor. Alias of tagGTExecMsg32.

	\copydoc tagGTExecMsg32
*/
/*! \typedef typedef tagGTExecMsg32 GTExecMsg32
*/
/*! \struct tagGTExecMsg32
	\brief The message from executor

	This describes a message from executor. 
*/

typedef struct tagGTExecMsg32
{
	int type;		//!< Type of message

	union			//!< Content of the message. For different message type, the content will be different data struct.
	{
		GTServerState32		state;		//!< Union: Server state
		GTErrMsg32			errmsg;		//!< Union: Error message
		GTChat32			chat;		//!< Union: Chat
		GTPopup32			popup;		//!< Union: Popup message
		
		GTUser32			user;		//!< Union: User information
		GTAccount32			account;	//!< Union: Account information

		GTOpenPosition32	open;		//!< Union: Open position
		GTTrade32			trade;		//!< Union: Trade
		GTPending32			pending;	//!< Union: Pending
		GTSending32			sending;	//!< Union: Sending
		GTCancel32			cancel;		//!< Union: Cancel
		GTReject32			reject;		//!< Union: Reject
		GTRemove32			remove;		//!< Union: Remove
		GTRejectCancel32	rejectcancel;	//!< Union: Reject cancel

		GTStatus32			status;		//!< Union: Status
	};
}GTExecMsg32;

#endif // !defined(__GTEXECMSG23_H__)

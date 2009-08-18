/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTErrMsg32.h
	\brief interface for the GTErrMsg32 class.
 */

#ifndef __GTERRMSG32_H__
#define __GTERRMSG32_H__

#include "GTConst.h"

#pragma pack(8)
/*! \ingroup c
*/
/*! \struct GTErrMsg32
	\brief The Error message, Alias of tagGTErrMsg32

	\copydoc tagGTErrMsg32
*/
/*! \typedef typedef tagGTErrMsg32 GTErrMsg32
*/
/*! \struct tagGTErrMsg32
	\brief The Error message
	
	When some error happen, the executor sends event to trigger OnExecMsgErrMsg callback.

	\sa GTSession::OnExecMsgErrMsg GTStock::OnExecMsgErrMsg GTAPI_MSG_SESSION_OnExecMsgErrMsg GTAPI_MSG_STOCK_OnExecMsgErrMsg
*/

typedef struct tagGTErrMsg32
{
	int		nErrCode;			//!< Reason 
	long	dwOrderSeqNo;		//!< User Order Seq No
	char	szStock[LEN_STOCK + 1];	//!< Stock name
	char	szText[80];			//!< Message Text
}GTErrMsg32;
#pragma pack()

#endif//__GTERRMSG32_H__

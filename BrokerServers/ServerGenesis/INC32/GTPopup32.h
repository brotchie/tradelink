/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTPopup32.h: interface for the GTPopup32 class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTPopup32.h
	\brief interface for the GTPopup32 class.
 */

#if !defined(__GTPOPUP32_H__)
#define __GTPOPUP32_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "GTConst.h"

/*! \ingroup c
*/
/*! \struct GTPopup32
	\brief The same as tagGTPopup32.

	\copydoc tagGTPopup32
*/
/*! \typedef typedef tagGTPopup32 GTPopup32
*/
/*! \struct tagGTPopup32
	\brief The popup information structure

	Executor issues a popup event and this will be transferred.

	\sa GTSession::OnExecMsgPopup GTAPI_MSG_SESSION_OnExecMsgPopup
*/
typedef struct tagGTPopup32
{
	int nLength;
	const char *pMsg;
}GTPopup32;

#endif // !defined(__GTPOPUP32_H__)

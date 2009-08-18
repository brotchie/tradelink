/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTRemove32.h: interface for the GTRemove32 class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTRemove32.h
	\brief interface for the GTRemove32 class.
 */

#if !defined(__GTREMOVE32_H__)
#define __GTREMOVE32_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "GTConst.h"
#include "MMID.h"

/*! \ingroup c
*/
/*! \struct GTRemove32
	\brief The removed order information. Alias of tagGTRemove32.

	\copydoc tagGTRemove32
*/
/*! \typedef typedef tagGTRemove32 GTRemove32
*/
/*! \struct tagGTRemove32
	\brief The removed order information

	This describes a removed order. Used in OnExecMsgRemove callback.

	\sa GTSession::OnExecMsgRemove GTStock::OnExecMsgRemove GTAPI_MSG_SESSION_OnExecMsgRemove GTAPI_MSG_STOCK_OnExecMsgRemove
*/
typedef struct tagGTRemove32
{
//@{
//! \brief [Trade info] Ticket Information
	long		dwTicketNo;
	long		dwTraderSeqNo;
	char		szRefNo[LEN_REFNO + 1];
//@}
//@{
//! \brief [User/Account Information] 
	char		szUserID[LEN_USER_ID + 1];
	char		szAccountID[LEN_ACCOUNT_ID + 1];
	char		szAccountCode[LEN_ACCOUNT_CODE + 1];
	char		szReconcileID[LEN_RECONCILE_ID + 1];
//@}
/*! \brief Stock name
*/
	char		szStock[LEN_STOCK + 1];
/*! \brief Method of sending order
*/
	MMID		method;
/*! \brief Place sending order to
*/
	MMID		place;

//! \brief Date of entry
	int			nEntryDate;

//! \brief Date of pending
	int			nPendDate;
//! \brief Time of pending
	int			nPendTime;
//! \brief Side of pending
	char		chPendSide;
//! \brief Share of pending
	int			nPendShares;
//! \brief Minimum of pending
	int			nPendMinimum;
/*! \brief Order type
	Refer to \ref chPriceIndicator of GTOrder
*/
	char		chPriceIndicator;
/*! \brief Price to bid/ask
	Refer to \ref dblPrice of GTOrder
*/
	double		dblPendPrice;
/*! \brief Stop Price for Stop-limit order
	Refer to \ref dblStopLimitPrice of GTOrder.
*/
	double		dblPendStopLimitPrice;
/*! \brief Hidden order option
	Refer to \ref chDisplay of GTOrder.
*/
	char		chPendDisplay;
/*! \brief Time in force
	Refer to \ref dwTimeInForce of GTOrder.
*/
	int			nPendTIF;

//! \brief Remove date
	int			nRemoveDate;
//! \brief Remove time
	int			nRemoveTime;
//! \brief Remove reason
	char		szRemoveReason[4 + 1];
}GTRemove32;

#endif // !defined(__GTREMOVE32_H__)

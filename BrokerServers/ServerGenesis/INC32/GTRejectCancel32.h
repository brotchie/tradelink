/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTRejectCancel32.h
	\brief interface for the GTRejectCancel32 class.
 */

#ifndef __GTRejectCancel32_H__
#define __GTRejectCancel32_H__

#include "..\Inc32\GTConst.h"
#include "..\Inc32\MMID.h"
#include "..\Inc32\CMapClass.h"

//! \cond INTERNAL
typedef struct { unsigned _unused; }GTREJECTCANCELS, *LPGTREJECTCANCELS;
//! \endcond

/*! \ingroup c
*/
/*! \struct GTRejectCancel32
	\brief The rejected cancel information. Alias of tagGTRejectCancel32.

	\copydoc tagGTRejectCancel32
*/
/*! \typedef typedef tagGTRejectCancel32 GTRejectCancel32
*/
/*! \struct tagGTRejectCancel32
	\brief The rejected cancel information

	When one sends a cancel, it can be rejected for some reason. This will trigger a OnExecMsgRejectCancel event, and the information
	will be enclosed in this structure.

	\sa GTSession::OnExecMsgRejectCancel GTStock::OnExecMsgRejectCancel GTAPI_MSG_SESSION_OnExecMsgRejectCancel GTAPI_MSG_STOCK_OnExecMsgRejectCancel
*/
typedef struct tagGTRejectCancel32
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

//! \brief Date of pending
	int			nPendDate;
//! \brief Time of pending
	int			nPendTime;
//! \brief Side of pending
	char		chPendSide;
//! \brief Share of pending
	int			nPendShares;
//! \brief Minimum of pending, Reserved
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
//! \brief Reject date
	int			nRejectDate;
//! \brief Reject time
	int			nRejectTime;
//! \brief Reject reason
	char		szRejectReason[64 + 1];
}GTRejectCancel32;

DEFINE_MAP_CLASS(gtRejectCancel, LPGTREJECTCANCELS, GTRejectCancel32, long)

#endif//__GTRejectCancel32_H__

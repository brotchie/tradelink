/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTReject32.h
	\brief interface for the GTReject32 class.
 */

#ifndef __GTReject32_H__
#define __GTReject32_H__

#include "..\Inc32\GTConst.h"
#include "..\Inc32\MMID.h"
#include "..\Inc32\CMapClass.h"

//! \cond INTERNAL
typedef struct { unsigned _unused; }GTREJECTS, *LPGTREJECTS;
//! \endcond

/*! \ingroup c
*/
/*! \struct GTReject32
	\brief Alias of tagGTReject32.

	\copydoc tagGTReject32
*/
/*! \typedef typedef tagGTReject32 GTReject32
*/
/*! \struct tagGTReject32
	\brief The rejected order information

	An order might be rejected by the market (ECN/Exchange). This encloses the rejected order information.
	Used in OnExecMsgReject() callback
	\sa GTSession::OnExecMsgReject GTStock::OnExecMsgReject GTAPI_MSG_SESSION_OnExecMsgReject GTAPI_MSG_STOCK_OnExecMsgReject

*/
typedef struct tagGTReject32
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

//! \brief Reject date
	int			nRejectDate;
//! \brief Reject time
	int			nRejectTime;
//! \brief Reject shares
	int			nRejectShares;
//! \brief Reject number
	int			nRejectNo;
//! \brief Reject reason
	char		szRejectReason[64 + 1];
}GTReject32;

DEFINE_MAP_CLASS(gtReject, LPGTREJECTS, GTReject32, long)

#endif//__GTReject32_H__

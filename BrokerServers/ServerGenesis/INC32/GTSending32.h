/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTSending32.h
	\brief interface for the GTSending32 class.
 */

#ifndef __GTSENDING32_H__
#define __GTSENDING32_H__

#include "..\Inc32\GTConst.h"
#include "..\Inc32\MMID.h"
#include "..\Inc32\CMapClass.h"
//! \cond INTERNAL
typedef struct { unsigned _unused; }GTSENDINGS, *LPGTSENDINGS;
//! \endcond

/*! \ingroup c
*/
/*! \struct GTSending32
	\brief Alias of tagGTSending32.

	\copydoc tagGTSending32
*/

/*! \typedef tagGTSending32 GTSending32 
*/
/*!	\struct tagGTSending32
	\brief The sending order structure
	
	This struct descripbes the sending phase order, which is used in OnSendingOrder() callback and OnExecMsgSending() callback.
	\sa GTSession::OnSendingOrder GTSession::OnExecMsgSending GTStock::OnSendingOrder GTStock::OnExecMsgSending 
	\sa GTAPI_MSG_SESSION_OnSendingOrder GTAPI_MSG_SESSION_OnExecMsgSending GTAPI_MSG_STOCK_OnSendingOrder GTAPI_MSG_STOCK_OnExecMsgSending 
*/
typedef struct tagGTSending32
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

/*! \brief Date of order
*/
	int			nEntryDate;
/*! \brief Time of order
*/
	int			nEntryTime;
/*! \brief Side of order
*/
	char		chEntrySide;
/*! \brief Share of order
*/
	int			nEntryShares;
/*! \brief Reserved
*/
	int			nEntryMinimum;
/*! \brief Order type
	Refer to \ref chPriceIndicator of GTOrder
*/
	char		chPriceIndicator;
/*! \brief Price to bid/ask
	Refer to \ref dblPrice of GTOrder
*/
	double		dblEntryPrice;
/*! \brief Stop Price for Stop-limit order
	Refer to \ref dblStopLimitPrice of GTOrder.
*/
	double		dblEntryStopLimitPrice;
/*! \brief Hidden order option
	Refer to \ref chDisplay of GTOrder.
*/
	char		chEntryDisplay;
/*! \brief Time in force
	Refer to \ref dwTimeInForce of GTOrder.
*/
	int			nEntryTIF;
//@{
//! \brief user data
	DWORD		dwUserData;
	LPVOID		lpUserData;
//@}
}GTSending32;

DEFINE_MAP_CLASS(gtSending, LPGTSENDINGS, GTSending32, long)

#endif//__GTSENDING32_H__

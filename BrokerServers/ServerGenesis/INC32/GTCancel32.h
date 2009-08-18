/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTCancel32.h
	\brief interface for the GTCancel32 class.
 */

#ifndef __GTCANCEL32_H__
#define __GTCANCEL32_H__

#include "..\Inc32\GTConst.h"
#include "..\Inc32\MMID.h"
#include "..\Inc32\CMapClass.h"

//! \cond INTERNAL
typedef struct { unsigned _unused; }GTCANCELS, *LPGTCANCELS;
//! \endcond

#pragma pack(8)
/*! \ingroup c
*/
/*! \struct GTCancel32
	\brief The Cancelled order information. Alias of tagGTCancel32.

	\copydoc tagGTCancel32
*/
/*! \typedef typedef tagGTCancel32 GTCancel32
*/
/*! \struct tagGTCancel32
	\brief The cancelling or cancelled order information

	Used in GTStock::OnExecMsgCanceling() and GTStock::OnExecMsgCancel().
	This structure gives the description of the cancel order information,
	including the order information, cancel reason and so on.

*/
typedef struct tagGTCancel32
{
//@{
	//! \brief [Trade info] Internal used. Ticket information
	long		dwTicketNo;
	long		dwTraderSeqNo;
	char		szRefNo[LEN_REFNO + 1];
//@}
//@{
	//! \brief [Account info] Internal used. User/account information
	char		szUserID[LEN_USER_ID + 1];
	char		szAccountID[LEN_ACCOUNT_ID + 1];
	char		szAccountCode[LEN_ACCOUNT_CODE + 1];
	char		szReconcileID[LEN_RECONCILE_ID + 1];
//@}
	char		szStock[LEN_STOCK + 1];	//!< Stock name

	MMID		method;			//!< Method
	MMID		place;			//!< Place

	int			nEntryDate;		//!< Entry date

	int			nPendDate;		//!< Pending date
	int			nPendTime;		//!< Pending time
	char		chPendSide;		//!< Pending side
	int			nPendShares;	//!< Pending share
	int			nPendMinimum;	//!< Reserved
	char		chPriceIndicator;	//!< Order type
	double		dblPendPrice;		//!< Pending price
	double		dblPendStopLimitPrice;	//!< Stop price if Stop-limit order
	char		chPendDisplay;		//!< Display it?
	int			nPendTIF;			//!< Pending order time in force

	int			nCancelDate;		//!< Date when order cancelled
	int			nCancelTime;		//!< Time when order cancelled
	double		dblCancelPrice;		//!< Price when order cancelled
	int			nCancelShares;		//!< Share cancelled
	int			nCancelRemainShares;	//!< Share remains
	char		szCancelReason[4 + 1];	//!< Cancel reason
	short		nCanceler;			//!< Canceller
}GTCancel32;
#pragma pack()

DEFINE_MAP_CLASS(gtCancel, LPGTCANCELS, GTCancel32, long)

#endif//__GTCANCEL32_H__

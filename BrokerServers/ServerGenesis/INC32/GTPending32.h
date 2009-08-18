/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTPending32.h
	\brief interface for the GTPending32 class.
 */

#ifndef __GTPENDING32_H__
#define __GTPENDING32_H__

#include "..\Inc32\GTConst.h"
#include "..\Inc32\MMID.h"
#include "..\Inc32\CMapClass.h"

//! \cond INTERNAL
typedef struct { unsigned _unused; }GTPENDINGS, *LPGTPENDINGS;
//! \endcond

/*! \ingroup c
*/
/*! \struct GTPending32
	\brief The pending order information. Alias of tagGTPending32.

	\copydoc tagGTPending32
*/
/*! \typedef typedef tagGTPending32 GTPending32
*/
/*! \struct tagGTPending32
	\brief The pending order information

	An order becomes in pending after the executor send the order to the exchange/ECN.
	This struct describes the information about the pending order.

	This struct is used in OnExecMsgPending() callback.

	\sa GTSession::OnExecMsgPending GTStock::OnExecMsgPending GTAPI_MSG_SESSION_OnExecMsgPending GTAPI_MSG_STOCK_OnExecMsgPending
*/
typedef struct tagGTPending32
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

	//! \brief	Stock name
	/*!
	*/
	char		szStock[LEN_STOCK + 1];

	//! \brief	Method
	/*!
		\sa MMID.h for available MMIDs
	*/
	MMID		method;
	//! \brief	Place
	/*!
	*/
	MMID		place;

	//! \brief	Entry date of the order
	/*!
	*/
	int			nEntryDate;

	//! \brief	Maximum share to show on the book
	/*!
		Some ECN/exchange support to display only part of the order size. 
		This defines the maximum size to be shown.
	*/
	int			nMaxFloor;

	//! \brief	Pend date of the order
	/*!
	*/
	int			nPendDate;
	//! \brief	Pend time of the order
	/*!
	*/
	int			nPendTime;
	//! \brief	Pend side of the order
	/*!
	*/
	char		chPendSide;
	//! \brief	Pend share of the order
	/*!
		Might differ from the original order since partial fill might have happened.
	*/
	int			nPendShares;

	//! \brief	Reserved
	/*!
	*/
	int			nPendMinimum;
	//! \brief	Order type. 
	/*!
		See \ref chPriceIndicator of GTOrder for more information.
	*/
	char		chPriceIndicator;
	//! \brief	Price of the original order
	/*!
	*/
	double		dblEntryPrice;
	//! \brief	Price of the submitted order that becomes pending
	/*!
	*/
	double		dblPendPrice;
	//! \brief	Stop limit price for Stop-limit order.
	/*!	It is the price for one share.

		Only valid for stop-Limit order, it is the stop price.
	*/
	double		dblPendStopLimitPrice;
	/*! \brief Display this order?

		'Y' for yes, 'N' for no.	Now only ISLD supports this. If no, this order will not
		show on any book or level 2. (Refer as hidden order)
	*/
	char		chPendDisplay;
	
	//! \brief	Share number of the original order
	/*!
	*/
	int			nEntryShares;
	//! \brief	Time_in_force of the original order
	/*!
	*/
	int			nEntryTIF;
	//! \brief	Time_in_force of the pending order
	/*!
	*/
	int			nPendTIF;
}GTPending32;

DEFINE_MAP_CLASS(gtPending, LPGTPENDINGS, GTPending32, long)

#endif//__GTPENDING32_H__

/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTTrade32.h
	\brief interface for the GTTradeKey32, GTTrade32 class.
 */

#ifndef __GTTRADE32_H__
#define __GTTRADE32_H__

#include "..\Inc32\GTConst.h"
#include "..\Inc32\MMID.h"
#include "..\Inc32\CMapClass.h"

//! \cond INTERNAL
typedef struct { unsigned _unused; }GTTRADES, *LPGTTRADES;
//! \endcond

#pragma pack(8)
/*! \ingroup c
*/
/*! \struct GTTradeKey32
	\brief The same as tagGTTradeKey32.

	\copydoc tagGTTradeKey32
*/
/*! \typedef tagGTTradeKey32 GTTradeKey32 
*/
/*!	\struct tagGTTradeKey32
	\brief The key used to store/retrieve a trade.
*/
typedef struct tagGTTradeKey32
{
	long		dwTicketNo;
	long		dwMatchNo;
}GTTradeKey32;

/*! \struct GTTrade32
	\brief The same as tagGTTrade32.

	\copydoc tagGTTrade32
*/
/*! \typedef tagGTTrade32 GTTrade32 
*/
/*!	\struct tagGTTrade32
	\brief The trade structure

	Once an order gets filled, this message will be got by API. It tells an order is just filled.
	The information about the trade is enclosed in this struct.
	
	This struct is used in OnExecMsgTrade() callback.

	\sa GTSession::OnExecMsgTrade GTStock::OnExecMsgTrade GTAPI_MSG_SESSION_OnExecMsgTrade GTAPI_MSG_STOCK_OnExecMsgTrade
*/
typedef struct tagGTTrade32
{
//@{
	//! \brief [Trade info] Internal used. Do not modify
	long		dwTicketNo;
	long		dwTraderSeqNo;
	char		szRefNo[LEN_REFNO + 1];
	long		dwMatchNo;
//@}

//@{
	//! \brief [Account info] Internal used. Do not modify
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

	//! \brief	Order type. 
	/*!
	\ref chPriceIndicator of GTOrder
	*/
	char		chPriceIndicator;

	//! \brief	Bid price when sending out order.
	/*!
	Internal used only.
	*/
	double		dblBidPrice;
	//! \brief	Ask price when sending out order.
	/*!
	Internal used only.
	*/
	double		dblAskPrice;
	//! \brief	Cost of the trade.
	/*!
	Internal used only.
	*/
	double		dblCost;

	//! \brief	Entry date of the order
	/*!
	*/
	int			nEntryDate;
	//! \brief	Price of the original order
	/*!
	*/
	double		dblEntryPrice;
	//! \brief	Share number of the original order
	/*!
	*/
	int			nEntryShares;
	//! \brief	Price of the submitted order that becomes pending
	/*!
	*/
	double		dblPendPrice;
	//! \brief	Side of the pending order.
	/*!
	*/
	char		chPendSide;

	//! \brief	Exec date of the order
	/*!
	*/
	int			nExecDate;

	//! \brief	Exec time of the order
	/*!
	*/
	int			nExecTime;
	//! \brief	The final exec price of the order
	/*!
	*/
	double		dblExecPrice;
	//! \brief	The final exec side of the order
	/*!
	*/
	char		chExecSide;
	//! \brief	The final exec share of the order
	/*!
	*/
	int			nExecShares;
	//! \brief	Remaining shares of the order
	/*!
	*/
	int			nExecRemainShares;
	//! \brief	Where the order is executed.
	/*!
	\sa MMID.h for available MMIDs
	*/
	MMID		execfirm;

	/*! \brief Liquidity of this trade
	
	Many exchanges/ECNs have their individually defined character. You'd better 
	call them to ask for the the details. Or call trading support for some 
	information.

	Some usually seen values:
		- 'A' = ADDED
		- 'R' = REMOVED
		- 'X' = ROUTED
		- 'D' = DOT ROUTE
		- 'B' = ADDED (NON BILLABLE)
		- 'S' = REMOVED (NON BILLABLE)
	*/
	char		chExecLiquidity;

	/*! \brief MOE? Do not modify
	
	Internal use only. Indicate if the trade information has ever be updated.
	*/
	BOOL		bMOE;
}GTTrade32;
#pragma pack()

DEFINE_MAP_CLASS(gtTrade, LPGTTRADES, GTTrade32, GTTradeKey32)

#endif//__GTTRADE32_H__

/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTAccount32.h
	\brief interface for the GTAccount32 class.
 */

#ifndef __GTACCOUNT32_H__
#define __GTACCOUNT32_H__

#include "..\Inc32\GTConst.h"

#pragma pack(8)

/*! \ingroup c*/
/*! \struct GTAccount32
	\brief The account structure. Alias of tagGTAccount32.

	\copydoc tagGTAccount32
*/
/*! \typedef typedef tagGTAccount32 GTAccount32
*/
/*! \struct tagGTAccount32
	\brief The account structure.

	One can read account information from the structure. Those information is set by the Genesis according to 
	the contract, and it will not be revised without permission from both customer and Genesis Securities. If one
	wants to change some of this values, call Genesis.

	Used in OnExecMsgAccount callback. When the login processes, the account information
	will be sent to API from the executor. API issues OnExecMsgAccount, and this class is the parameter.

	\sa GTSession::OnExecMsgAccount GTAPI_MSG_SESSION_OnExecMsgAccount  
*/
typedef struct tagGTAccount32
{
	char		szAccountID[LEN_ACCOUNT_ID + 1];		//!< Account ID
	char		szAccountCode[LEN_ACCOUNT_CODE + 1];	//!< Account Code
	char		szReconcileID[LEN_RECONCILE_ID + 1];	//!< Reconcile ID
	char		szGroupID[LEN_GROUP_ID + 1];			//!< Group ID
	char		szAccountName[LEN_ACCOUNT_NAME + 1];	//!< Account name

	int			nType;									//!< Account Type
	int			nSortNo;								//!< Sort No
	int			nTraderType;							//!< Trader Type
	int			nDiscretion;							//!< Discretion

	double		dblBPScale;								//!< Buying power scale			
	double		dblMaintExcess;							//!< Maintain excess
	double		dblInitialBP;							//!< Initial buying power
	double		dblInitialEquity;						//!< Initial equity value
	double		dblCurrentBP;							//!< Current buying power
	double		dblCurrentEquity;						//!< Current equity value	
	double		dblCurrentAmount;						//!< Current equity amount
	int			nCurrentTickets;						//!< Current ticket number
	int			nCurrentPartialFills;					//!< Current number of partial fills
	int			nCurrentCancel;							//!< Current cancel tickets
	int			nCurrentShares;							//!< Current Shares

	double		dblRiskRate;							//!< rick rate
	int			nMaxOpenPosPerDay;						//!< Maximum open position per day
	double		dblMaxAmountPerDay;						//!< Maximum amount per day
	int			nMaxTicketsPerDay;						//!< Maximum tickets per day
	int			nMaxSharesPerDay;						//!< Maximum shares per day
	double		dblMaxAmountPerTicket;					//!< Maximum amount per ticket
	int			nMaxSharesPerTicket;					//!< Maximum shares per ticket
	int			nMaxSharesPerPos;						//!< Maximum shares per position
	double		dblMaxLossPerDay;						//!< Maximum loss per day

	double		dblPLRealized;							//!< Profit/loss realized
	double		dblCurrentLong;							//!< Current long
	double		dblCurrentShort;						//!< Current short
}GTAccount32;
#pragma pack()

#endif//__GTACCOUNT32_H__

/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTOpenPosition32.h
	\brief interface for the GTOpenPosition32 class.
 */

#ifndef __GTOPENPOSITION32_H__
#define __GTOPENPOSITION32_H__

#include "..\Inc32\GTConst.h"
#include "..\Inc32\CMapClass.h"
//! \cond INTERNAL
typedef struct { unsigned _unused; }GTOPENPOSITIONS, *LPGTOPENPOSITIONS;
//! \endcond
#pragma pack(8)
/*! \struct GTOpenPositionKey32
	\brief The same as tagGTOpenPositionKey32.

	\copydoc tagGTOpenPositionKey32
*/
/*! \typedef typedef tagGTOpenPositionKey32 GTOpenPositionKey32
*/
/*! \struct tagGTOpenPositionKey32
	\brief The open position key structure
*/
typedef struct tagGTOpenPositionKey32
{
	char		szAccountID[LEN_ACCOUNT_ID + 1];
	char		szStock[LEN_STOCK + 1];
}GTOpenPositionKey32;

/*! \ingroup c
*/
/*! \struct GTOpenPosition32
	\brief The same as tagGTOpenPosition32.

	\copydoc tagGTOpenPosition32
*/
/*! \typedef typedef tagGTOpenPosition32 GTOpenPosition32
*/
/*! \struct tagGTOpenPosition32
	\brief The open position structure

	Then open position changes each time, the OnExecMsgOpenPosition() callbacks will be triggered. And this 
	structure is delivered.
	\sa GTSession::OnExecMsgOpenPosition GTStock::OnExecMsgOpenPosition GTAPI_MSG_SESSION_OnExecMsgOpenPosition GTAPI_MSG_STOCK_OnExecMsgOpenPosition  
*/
typedef struct tagGTOpenPosition32
{
	//! \brief	Account ID
	char		szAccountID[LEN_ACCOUNT_ID + 1];
	//! \brief	Account Code
	char		szAccountCode[LEN_ACCOUNT_CODE + 1];
	//! \brief	Reconcile ID
	char		szReconcileID[LEN_RECONCILE_ID + 1];
	//! \brief	Stock name
	char		szStock[LEN_STOCK + 1];

	//! \brief	Open date
	int			nOpenDate;
	//! \brief	Open time
	int			nOpenTime;
	//! \brief	Price per share
	double		dblOpenPrice;
	//! \brief	Open side ('B' or 'S'/'T')
	char		chOpenSide;
	//! \brief	Shares
	int			nOpenShares;

	//! \brief	Yesterday side ('B' or 'S'/'T')
	char	chYestSide;
	//! \brief	Yesterday Shares
	int		nYestShares;

	//! \brief Currency Rate
	double		dblCurrencyRate;
}GTOpenPosition32;
#pragma pack()

DEFINE_MAP_CLASS(gtOpenPosition, LPGTOPENPOSITIONS, GTOpenPosition32, GTOpenPositionKey32)

#endif//__GTOPENPOSITION32_H__

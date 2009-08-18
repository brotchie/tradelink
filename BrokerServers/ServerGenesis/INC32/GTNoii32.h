/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTNoii32.h
	\brief interface for the GTNoii32 class.
 */

#ifndef __GTNOII32_H__
#define __GTNOII32_H__

#include "GTConst.h"
#include "GTime32.h"

#pragma pack(8)
/*! \ingroup c
*/
/*! \struct GTNoii32
	\brief Alias of tagGTNoii32.

	\copydoc tagGTNoii32
*/
/*! \typedef typedef tagGTNoii32 GTNoii32
*/
/*! \struct tagGTNoii32
	\brief The NOII information

	The NOII information is enclosed in this structure. 

	Always this is updated when a NOII sent in. 

	When a new NOII record comes in, OnGotNoiiRecord callback is called.
	\sa GTSession::OnGotQuoteNoii GTStock::OnGotQuoteNoii  GTAPI_MSG_SESSION_OnGotQuoteNoii  GTAPI_MSG_STOCK_OnGotQuoteNoii  
*/
typedef struct tagGTNoii32
{
	//! \brief	Stock name
	char	szStock[LEN_STOCK + 1];

	//! \brief	When this NOII information was generated (By ECN/Exchange)
	GTime32	gtime;
	
	//! \brief Paired shares
	long	dwPairedShares;

	//! \brief Imbalance shares
	long	dwImbalanceShares;

	//! \brief Imbalance direction
	char	chImbalanceDirection;

	//! \brief Far price
	double	dblFarPrice;

	//! \brief Near price
	double	dblNearPrice;
	
	//! \brief Current reference price
	double	dblCurrentReferencePrice;

	//! \brief Cross type
	char	chCrossType;

	//! \brief Price variation indicator
	char	chPriceVariationIndicator;

}GTNoii32;
#pragma pack()

#endif//__GTNOII32_H__

/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GT_NYXI_32.h
	\brief interface for the GT_NYXI_32 class.
 */

#ifndef __GT_NYXI_32_H__
#	define __GT_NYXI_32_H__

#	include "GTConst.h"
#	include "GTime32.h"

	#pragma pack(8)
	/*! \ingroup c
	*/
	/*! \struct GT_NYXI_32
		\brief Alias of tag_GT_NYXI_32.

		\copydoc tag_GT_NYXI_32
	*/
	/*! \typedef typedef tag_GT_NYXI_32 GT_NYXI_32
	*/
	/*! \struct tag_GT_NYXI_32
		\brief The NYSE Imbalance information

		The NYSE Imbalance information is enclosed in this structure. 

		Always updated when a NYXI message arrives.

		When a new NYXI record comes in, OnGotNoiiRecord callback is called.
		\sa GTSession::OnGotQuoteNyxi GTStock::OnGotQuoteNyxi  GTAPI_MSG_SESSION_OnGotQuoteNyxi  GTAPI_MSG_STOCK_OnGotQuoteNyxi
	*/
	typedef struct tag_GT_NYXI_32
	{
		//! \brief	Stock name
		char	szStock[LEN_STOCK + 1];

		//! \brief Reference price
		double	dblReferencePrice;

		//! \brief Imbalance shares
		long	dwImbalanceShares;

		//! \brief Paired shares
		long	dwPairedShares;

		//! \brief	When this NOII information was generated (By ECN/Exchange)
		GTime32	gtime;
		
		//! \brief Imbalance Direction
		char	chDirection;

		//! \brief Opening or Closing Session
		char	chSession;

		//! \brief NYSE reason for imbalance.
		//! ' ' = not applicable; 'R'=regulatory, 'i'=informational
		char	chReason;

	} GT_NYXI_32;

#	pragma pack()

#endif //__GT_NYXI_32_H__

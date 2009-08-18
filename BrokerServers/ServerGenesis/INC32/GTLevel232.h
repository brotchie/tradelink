/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTLevel232.h
	\brief interface for the GTLevel232 class.
 */

#ifndef __GTLEVEL232_H__
#define __GTLEVEL232_H__

#include "..\Inc32\GTConst.h"
#include "..\Inc32\GTime32.h"
#include "..\Inc32\MMID.h"

#pragma pack(8)
/*! \ingroup c
*/
/*! \struct GTLevel232
	\brief  Alias of tagGTLevel232.

	\copydoc tagGTLevel232
*/
/*! \typedef typedef tagGTLevel232 GTLevel232
*/
/*! \struct tagGTLevel232
	\brief The level 2 information

	The level 2 information is enclosed in this structure. 

	Each level2 record takes one of this struct. GTLevel2Box keeps all the records in the GTLevel2List.

	When a new level2 record comes in, OnGotLevel2Record callback is called.
	\sa GTSession::OnGotLevel2Record GTStock::OnGotLevel2Record  GTAPI_MSG_SESSION_OnGotLevel2Record  GTAPI_MSG_STOCK_OnGotLevel2Record  
*/
typedef struct tagGTLevel232
{
	//! \brief	Stock name.
	char	szStock[LEN_STOCK + 1];

	//! \brief	When this level 2 information was generated (By ECN/Exchange).
	GTime32	gtime;
	//! \brief	Which place this level 2 come from.
	MMID	mmid;
	//! \brief Bid or Ask.
	char	chSide;
	//! \brief Price.
	double	dblPrice;
	//! \brief	How many shares bid/ask outstanding.
	long	dwShares;

//@{
	BOOL	bECN;			//!< is this ECN?
	BOOL	bBook;			//!< is this book?
	BOOL	bTotalView;		//!< is this total view?
	BOOL	bOpenView;		//!< is this open view?
	BOOL	bAxe;			//!< is this axe?
//@}

	//! \brief [API GENERATED, INTERNAL USED] Level Price
	/*!
		This indicate which price level this level 2 belongs to. You can set
		the price level precision on the GTSetting32::m_nLevelRate. When use 
		100, the level division will be based on $0.01; if 1000, each price
		level will have differency that is on step $0.001. 

		This is calculated by the API.
	*/
	double	dblLevelPrice;
	//! \brief [API GENERATED, INTERNAL USED] The price used to compare
	/*!	
		Due to the situation binary numbers used in the computer but decimal 
		numbers used on the price of the stocks, we need this field to compare
		the price. It is simply the price*10000. 

		This is calculated by the API.
	*/
	__int64	ddwComparePrice;

	//! \brief How many shares you have that pending there.
	int		nOwnShares;
	//! \brief [API GENERATED] When API got it.
	DWORD	dwRecvTick;

	//! \brief Quote Condition from SIAC.
	char	chQuoteCond;
}GTLevel232;
#pragma pack()

#endif//__GTLEVEL232_H__

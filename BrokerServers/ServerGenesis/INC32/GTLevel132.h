/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTLevel132.h
	\brief interface for the GTLevel132 class.
 */

#ifndef __GTLEVEL132_H__
#define __GTLEVEL132_H__

#include "GTConst.h"
#include "GTime32.h"

// tick direction
#define TICK_LAST_UNKNOWN	0x00 //     1
#define TICK_LAST_UP		0x01 //     1
#define TICK_LAST_DOWN		0x02 //    10

#define TICK_BID_UNKNOWN	0x00 // 10000
#define TICK_BID_UP			0x10 // 10000
#define TICK_BID_DOWN		0x20 //100000

#pragma pack(8)
/*! \ingroup c
*/
/*! \struct GTLevel132
	\brief Alias of tagGTLevel132.

	\copydoc tagGTLevel132
*/
/*! \typedef typedef tagGTLevel132 GTLevel132
*/
/*! \struct tagGTLevel132
	\brief The level 1 information

	The level 1 information is enclosed in this structure. 

	Always this is updated when a Level 1 sent in. Any exchange might send level 1
	to the system, while the system will handle like following:
	
	1. overwrite those fields refering to the local information, such as
		- locBidExchangeCode
		- locAskExchangeCode
		- locBidPrice
		- locAskPrice
		- locBidSize
		- locAskSize

	2. if NBBO is affected by this level 1 information, those NBBO fields are also
	revised:
		- dblBidPrice
		- dblAskPrice
		- bboBidExchangeCode
		- bboAskExchangeCode
		- dblBidSize
		- dblAskSize

	When a new level1 record comes in, OnGotLevel1Record callback is called.
	\sa GTSession::OnGotQuoteLevel1 GTStock::OnGotQuoteLevel1  GTAPI_MSG_SESSION_OnGotQuoteLevel1  GTAPI_MSG_STOCK_OnGotQuoteLevel1  
*/
typedef struct tagGTLevel132
{
	//! \brief	Stock name
	char	szStock[LEN_STOCK + 1];

	//! \brief	When this level 1 information was generated (By ECN/Exchange)
	GTime32	gtime;
	
	//! \brief Last print price
	double	dblLast;
	//! \brief today open
	double	dblOpen;		// today open
	//! \brief last day close
	double	dblClose;		// last day close
	//! \brief today highest
	double	dblHigh;		// today highest
	//! \brief today lowest
	double	dblLow;			// today lowest
	//! \brief volume until now
	long	dwVolume;		// volume until now

	//! \brief National Bid price when this level 1 was generated
	double	dblBidPrice;	// bid price
	//! \brief National Ask price when this level 1 was generated
	double	dblAskPrice;	// ask price

	//! \brief Indicate whether it is uptick or downtick when this trade filled.
	/*!
		Refer to \ref tick.
		Can be any one of below:
		<TABLE class="dtTABLE" cellspacing="0">
			<TR>
				<TH>HEX</TH><TH>BIN</TH><TH>NAME</TH>
			</TR>
			<TR>
				<TD>0x00</TD><TD>0000 0000</TD><TD>TICK_LAST_UNKNOWN, TICK_BID_UNKNOWN</TD>
			</TR>
			<TR>
				<TD>0x01</TD><TD>0000 0001</TD><TD>TICK_LAST_UP</TD>
			</TR>
			<TR>
				<TD>0x02</TD><TD>0000 0010</TD><TD>TICK_LAST_DOWN</TD>
			</TR>
			<TR>
				<TD>0x10</TD><TD>0001 0000</TD><TD>TICK_BID_UP</TD>
			</TR>
			<TR>
				<TD>0x20</TD><TD>0010 0000</TD><TD>TICK_BID_DOWN</TD>
			</TR>
		</TABLE>
	*/
	char	nTickDirect;
	/*!	\brief Reserved
	*/
	char	flags;
	//! \brief Sale condition
	/*! 
		A general classification of this trade. Internal used.
	*/
	char	chSaleCondition;
	/*!	\brief Reserved
	*/
	char	chReserved;

	/*!	\brief regional exchange code

		Because the data source's individual definition of those charactors, we don't have any standards to
		discribe the letters. The possible values is listed in the \ref exchangecode. 
	*/
	char	locBidExchangeCode[2];		// regional exchange code
	/*!	\brief regional exchange code

		Because the data source's individual definition of those charactors, we don't have any standards to
		discribe the letters. The possible values is listed in the \ref exchangecode. 
	*/
	char	locAskExchangeCode[2];		// regional exchange code

	/*!	\brief nbbo exchange code

		Because the data source's individual definition of those charactors, we don't have any standards to
		discribe the letters. The possible values is listed in the \ref exchangecode. 
	*/
	char	bboBidExchangeCode[2];		// nbbo exchange code
	/*!	\brief nbbo exchange code

		Because the data source's individual definition of those charactors, we don't have any standards to
		discribe the letters. The possible values is listed in the \ref exchangecode. 
	*/
	char	bboAskExchangeCode[2];		// nbbo exchange code

	/*!	\brief last size
	*/
	int		nLastSize;			// last size
	/*!	\brief NBBO bid size

	in hundreds.
	*/
	int		nBidSize;			// bid size
	/*!	\brief NBBO ask size

	in hundreds.
	*/
	int		nAskSize;			// ask size

	/*!	\brief Local Bid Price
	*/
	double	locBidPrice;		// Local Bid Price
	/*!	\brief Local Ask Price
	*/
	double	locAskPrice;		// Local Ask Price
	/*!	\brief local bid size

	in hundreds.
	*/
	int		locBidSize;			// bid size
	/*!	\brief local ask size

	in hundreds.
	*/
	int		locAskSize;			// ask size
}GTLevel132;
#pragma pack()

#endif//__GTLEVEL132_H__

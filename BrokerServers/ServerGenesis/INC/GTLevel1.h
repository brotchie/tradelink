/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTLevel1.h: interface for the GTLevel1 class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTLevel1.h
	\brief interface for the GTLevel1 class.
 */

#if !defined(__GTLEVEL1_H__)
#define __GTLEVEL1_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTConst.h"
#include "..\Inc32\MMID.h"
#include "..\Inc32\GTime32.h"
#include "..\Inc32\GTLevel132.h"

#define QUOTE_CONNECTED		1
#define QUOTE_DISCONNECTED	2
#define QUOTE_HEARTBEAT		3

#define QUOTE_LEVEL1		21
#define QUOTE_OPTION_LEVEL1		22

/*! \ingroup cpp
*/
/*! \struct GTLevel1
	\brief The level 1 information. Has the same structure as GTLevel132 or tagGTLevel132

	\copydoc tagGTLevel132

	\anchor tick <b>Up Tick vs Down Tick</b>

	There are two kinds of uptick/downtick, which take measurement on different objects. 
	
	For those NASDAQ symbol, the bid prices are compared. If the new bid price is greater
	than the one before, an uptick is showed; if less than, a downtick is showed. 
	
	For listed symbols, the last two print prices are compared. If the last print price
	is greater than the 2nd last print price, an uptick is showed; if less than, a downtick
	is showed.

	In our system, they are called BID_TICK (for NASDAQ) and LAST_TICK(listed).

*/
struct GTLevel1 : public tagGTLevel132
{
public:
	//! \brief Get BID_TICK
	//! \return 1 for UP_BID_TICK; -1 for DOWN_BID_TICK.
	int GetBidTick() const
	{
		if(TICK_BID_UP & nTickDirect)
			return 1;
		if(TICK_BID_DOWN & nTickDirect)
			return -1;
		
		return 0;
	}

	//! \brief Get LAST_TICK
	//! \return 1 for UP_LAST_TICK; -1 for DOWN_LAST_TICK.
	int GetLastTick() const
	{
		if(TICK_LAST_UP & nTickDirect)
			return 1;
		if(TICK_LAST_DOWN & nTickDirect)
			return -1;
		
		return 0;
	}

	//! \brief Reset the Level 1 information. All filled with 0's.
	void ResetContent()
	{
		gtime.dwTime			= 0;
		dblLast					= 0;
		dblOpen					= 0;	// today open
		dblClose				= 0;	// last day close
		dblHigh					= 0;	// today highest
		dblLow					= 0;	// today lowest
		dwVolume				= 0;	// volume until now
							
		dblBidPrice				= 0;	// bid price
		dblAskPrice				= 0;	// ask price
						
		nTickDirect				= 0;
		flags					= 0;
		chSaleCondition			= 0;
		chReserved				= 0;

		locBidExchangeCode[0]	= 0;	// exchange code
		locBidExchangeCode[1]	= 0;	// Second Byte of Exchange Code
		locAskExchangeCode[0]	= 0;	// exchange code
		locAskExchangeCode[1]	= 0;	// Second Byte of Exchange Code
		bboBidExchangeCode[0]	= 0;	// exchange code
		bboBidExchangeCode[1]	= 0;	// Second Byte of Exchange Code
		bboAskExchangeCode[0]	= 0;	// exchange code
		bboAskExchangeCode[1]	= 0;	// Second Byte of Exchange Code

		nLastSize				= 0;	// last size
		nBidSize				= 0;	// bid size
		nAskSize				= 0;	// ask size
							
		locBidPrice				= 0;	// Local Bid Price
		locAskPrice				= 0;	// Local Ask Price
		locBidSize				= 0;	// bid size
		locAskSize				= 0;	// ask size
	}
};

#endif // !defined(__GTLEVEL1_H__)

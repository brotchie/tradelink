/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTChart32.h: interface for the GTChart32 class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTChart32.h
	\brief interface for the GTChart32 class.
 */

#if !defined(__GTCHART32_H__)
#define __GTCHART32_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "GTConst.h"
#include "GTPrint32.h"
#include "GDate32.h"

#define CHART_CONNECTED		1
#define CHART_DISCONNECTED	2

#define CHART_RECORD		21
#define CHART_HISTORY		22

#define CHART_REFRESH		31
#define CHART_DISPLAY		32
#define CHART_HEALTH		33

/*! \ingroup c
*/
/*! \struct GTChart32
	\brief The chart message from chart server. Alias of tagGTChart32.

	\copydoc tagGTExecMsg32
*/
/*! \typedef typedef tagGTChart32 GTChart32
*/
/*! \struct tagGTChart32
	\brief The chart message from chart server.

	This describes chart information from chart server. Used in Chart server callbacks. 
	A chart record has following information enclosed: the name of the symbol, time, prices.

	\sa GTSession::OnExecExecMsg GTAPI_MSG_SESSION_OnExecExecMsg
*/
typedef struct tagGTChart32
{
//	int		nType;
	char	szStock[LEN_STOCK + 1];		//!< Symbol name

	GDate32	gdate;						//!< Date \sa GDate32
	GTime32	gtime;						//!< Time \sa GTime32

	double	dblOpen;					//!< Open price
	double	dblHigh;					//!< Highest price
	double	dblLow;						//!< Lowest price
	double	dblClose;					//!< Close price
	long	dwVolume;					//!< Volume.
}GTChart32;

enum 
{
	valOpen = 0,
	valHigh,
	valLow,
	valClose,
	valVolume,

	valUser0,
	valMax_Value = 20
};

/*! \struct GTChart232
	\brief [Internal Used] The chart message from chart server. Alias of tagGTChart232.
*/
/*! \typedef typedef tagGTChart232 GTChart232
*/
/*! \struct tagGTChart232
	\brief [Internal Used] The chart message from chart server.

	This describes chart information stored inside the API.
*/
typedef struct tagGTChart232
{
	GDate32	gdate;					//!< Date
	int		minutes;				//!< Minute

	double dblValue[valMax_Value + 1];	//!< Values
}GTChart232;

#endif // !defined(__GTCHART32_H__)

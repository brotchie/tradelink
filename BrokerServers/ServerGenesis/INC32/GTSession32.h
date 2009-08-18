/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTSession32.h
	\brief Define GTSession32
 */
#ifndef __GTSESSION32_H__
#define __GTSESSION32_H__

#include "GTConst.h"
#include "GTime32.h"
#include "GTExecMsg32.h"

#include "GTLevel132.h"
#include "GTLevel232.h"
#include "GTPrint32.h"
#include "GTChart32.h"
#include "GTSetting32.h"
#include "GTQuoteText32.h"
#include "GTNoii32.h"
#include "GT_NYXI_32.h"
#include "GTNyseAlert32.h"

//! \cond INTERNAL
struct tagGTSTOCK;
typedef struct tagGTSESSION { unsigned _unused; }GTSESSION, *LPGTSESSION;
//! \endcond
/*! @name Definition for Session Callback Type
	\anchor Session_Callback_Event
*/
//@{
//! \brief Base.
#define GTAPI_MSG_SESSION_0						1000

//! \brief Callback for OnCreateStock. Function prototype refer to following:
/*! @copydoc GTSession::OnCreateStock() */
#define GTAPI_MSG_SESSION_OnCreateStock			(GTAPI_MSG_SESSION_0 + 1)
//! \brief Callback for OnDeleteStock. Function prototype refer to following:
/*! @copydoc GTSession::OnDeleteStock() */
#define GTAPI_MSG_SESSION_OnDeleteStock			(GTAPI_MSG_SESSION_0 + 2)
//! \brief Callback for OnInitStock. Function prototype refer to following:
/*! @copydoc GTSession::OnInitStock() */
#define GTAPI_MSG_SESSION_OnInitStock			(GTAPI_MSG_SESSION_0 + 3)
//! \brief Callback for OnExitStock. Function prototype refer to following:
/*! @copydoc GTSession::OnExitStock() */
#define GTAPI_MSG_SESSION_OnExitStock			(GTAPI_MSG_SESSION_0 + 4)
	
//! \brief Callback for OnTick. Function prototype refer to following:
/*! @copydoc GTSession::OnTick() */
#define GTAPI_MSG_SESSION_OnTick				(GTAPI_MSG_SESSION_0 + 11)
//! \brief Callback for OnExecMsg. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsg() */
#define GTAPI_MSG_SESSION_OnExecMsg				(GTAPI_MSG_SESSION_0 + 12)

//! \brief Callback for OnExecHeartBeat. Function prototype refer to following:
/*! @copydoc GTSession::OnExecHeartBeat() */
#define GTAPI_MSG_SESSION_OnExecHeartBeat		(GTAPI_MSG_SESSION_0 + 21)
//! \brief Callback for OnExecConnected. Function prototype refer to following:
/*! @copydoc GTSession::OnExecConnected() */
#define GTAPI_MSG_SESSION_OnExecConnected		(GTAPI_MSG_SESSION_0 + 22)
//! \brief Callback for OnExecDisconnected. Function prototype refer to following:
/*! @copydoc GTSession::OnExecDisconnected() */
#define GTAPI_MSG_SESSION_OnExecDisconnected	(GTAPI_MSG_SESSION_0 + 23)

//! \brief Callback for OnExecMsgState. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgState() */
#define GTAPI_MSG_SESSION_OnExecMsgState		(GTAPI_MSG_SESSION_0 + 31)
//! \brief Callback for OnExecMsgErrMsg. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgErrMsg() */
#define GTAPI_MSG_SESSION_OnExecMsgErrMsg		(GTAPI_MSG_SESSION_0 + 32)
//! \brief Callback for OnExecMsgChat. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgChat() */
#define GTAPI_MSG_SESSION_OnExecMsgChat			(GTAPI_MSG_SESSION_0 + 33)
//! \brief Callback for OnExecMsgPopup. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgPopup() */
#define GTAPI_MSG_SESSION_OnExecMsgPopup		(GTAPI_MSG_SESSION_0 + 34)

//! \brief Callback for OnSendingOrder. Function prototype refer to following:
/*! @copydoc GTSession::OnSendingOrder() */
#define GTAPI_MSG_SESSION_OnSendingOrder		(GTAPI_MSG_SESSION_0 + 40)
//! \brief Callback for OnExecMsgLogin. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgLogin() */
#define GTAPI_MSG_SESSION_OnExecMsgLogin		(GTAPI_MSG_SESSION_0 + 41)
//! \brief Callback for OnExecMsgLoggedin. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgLoggedin() */
#define GTAPI_MSG_SESSION_OnExecMsgLoggedin		(GTAPI_MSG_SESSION_0 + 42)
//! \brief Callback for OnExecMsgLogout. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgLogout() */
#define GTAPI_MSG_SESSION_OnExecMsgLogout		(GTAPI_MSG_SESSION_0 + 43)
//! \brief Callback for OnExecMsgUser. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgUser() */
#define GTAPI_MSG_SESSION_OnExecMsgUser			(GTAPI_MSG_SESSION_0 + 44)
//! \brief Callback for OnExecMsgAccount. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgAccount() */
#define GTAPI_MSG_SESSION_OnExecMsgAccount		(GTAPI_MSG_SESSION_0 + 45)
//! \brief Callback for OnExecMsgOpenPosition. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgOpenPosition() */
#define GTAPI_MSG_SESSION_OnExecMsgOpenPosition	(GTAPI_MSG_SESSION_0 + 46)
//! \brief Callback for OnExecMsgTrade. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgTrade() */
#define GTAPI_MSG_SESSION_OnExecMsgTrade		(GTAPI_MSG_SESSION_0 + 47)
//! \brief Callback for OnExecMsgPending. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgPending() */
#define GTAPI_MSG_SESSION_OnExecMsgPending		(GTAPI_MSG_SESSION_0 + 48)
//! \brief Callback for OnExecMsgSending. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgSending() */
#define GTAPI_MSG_SESSION_OnExecMsgSending		(GTAPI_MSG_SESSION_0 + 49)
//! \brief Callback for OnExecMsgCanceling. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgCanceling() */
#define GTAPI_MSG_SESSION_OnExecMsgCanceling	(GTAPI_MSG_SESSION_0 + 50)
//! \brief Callback for OnExecMsgCancel. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgCancel() */
#define GTAPI_MSG_SESSION_OnExecMsgCancel		(GTAPI_MSG_SESSION_0 + 51)
//! \brief Callback for OnExecMsgReject. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgReject() */
#define GTAPI_MSG_SESSION_OnExecMsgReject		(GTAPI_MSG_SESSION_0 + 52)
//! \brief Callback for OnExecMsgRemove. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgRemove() */
#define GTAPI_MSG_SESSION_OnExecMsgRemove		(GTAPI_MSG_SESSION_0 + 53)
//! \brief Callback for OnExecMsgRejectCancel. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgRejectCancel() */
#define GTAPI_MSG_SESSION_OnExecMsgRejectCancel	(GTAPI_MSG_SESSION_0 + 54)
//! \brief Callback for OnExecMsgStatus. Function prototype refer to following:
/*! @copydoc GTSession::OnExecMsgStatus() */
#define GTAPI_MSG_SESSION_OnExecMsgStatus		(GTAPI_MSG_SESSION_0 + 55)

//! \brief Callback for OnGotLevel2Connected. Function prototype refer to following:
/*! @copydoc GTSession::OnGotLevel2Connected() */
#define GTAPI_MSG_SESSION_OnGotLevel2Connected		(GTAPI_MSG_SESSION_0 + 61)
//! \brief Callback for OnGotLevel2Disconnected. Function prototype refer to following:
/*! @copydoc GTSession::OnGotLevel2Disconnected() */
#define GTAPI_MSG_SESSION_OnGotLevel2Disconnected	(GTAPI_MSG_SESSION_0 + 62)
//! \brief Callback for OnGotLevel2Record. Function prototype refer to following:
/*! @copydoc GTSession::OnGotLevel2Record() */
#define GTAPI_MSG_SESSION_OnGotLevel2Record			(GTAPI_MSG_SESSION_0 + 63)
//! \brief Callback for OnGotLevel2Refresh. Function prototype refer to following:
/*! @copydoc GTSession::OnGotLevel2Refresh() */
#define GTAPI_MSG_SESSION_OnGotLevel2Refresh		(GTAPI_MSG_SESSION_0 + 64)
//! \brief Callback for OnGotLevel2Display. Function prototype refer to following:
/*! @copydoc GTSession::OnGotLevel2Display() */
#define GTAPI_MSG_SESSION_OnGotLevel2Display		(GTAPI_MSG_SESSION_0 + 65)
//! \brief Callback for OnGotLevel2Clear. Function prototype refer to following:
/*! @copydoc GTSession::OnGotLevel2Clear() */
#define GTAPI_MSG_SESSION_OnGotLevel2Clear			(GTAPI_MSG_SESSION_0 + 66)
//! \brief Callback for OnGotLevel2Text. Function prototype refer to following:
/*! @copydoc GTSession::OnGotLevel2Text() */
#define GTAPI_MSG_SESSION_OnGotLevel2Text			(GTAPI_MSG_SESSION_0 + 67)

//! \brief Callback for OnGotQuoteConnected. Function prototype refer to following:
/*! @copydoc GTSession::OnGotQuoteConnected() */
#define GTAPI_MSG_SESSION_OnGotQuoteConnected		(GTAPI_MSG_SESSION_0 + 71)
//! \brief Callback for OnGotQuoteDisconnected. Function prototype refer to following:
/*! @copydoc GTSession::OnGotQuoteDisconnected() */
#define GTAPI_MSG_SESSION_OnGotQuoteDisconnected	(GTAPI_MSG_SESSION_0 + 72)
//! \brief Callback for OnGotQuoteLevel1. Function prototype refer to following:
/*! @copydoc GTSession::OnGotQuoteLevel1() */
#define GTAPI_MSG_SESSION_OnGotQuoteLevel1			(GTAPI_MSG_SESSION_0 + 73)
//! \brief Callback for OnGotQuotePrint. Function prototype refer to following:
/*! @copydoc GTSession::OnGotQuotePrint() */
#define GTAPI_MSG_SESSION_OnGotQuotePrint			(GTAPI_MSG_SESSION_0 + 74)
//! \brief Callback for OnGotQuotePrintHistory. Function prototype refer to following:
/*! @copydoc GTSession::OnGotQuotePrintHistory() */
#define GTAPI_MSG_SESSION_OnGotQuotePrintHistory	(GTAPI_MSG_SESSION_0 + 75)
//! \brief Callback for OnGotQuotePrintRefresh. Function prototype refer to following:
/*! @copydoc GTSession::OnGotQuotePrintRefresh() */
#define GTAPI_MSG_SESSION_OnGotQuotePrintRefresh	(GTAPI_MSG_SESSION_0 + 76)
//! \brief Callback for OnGotQuotePrintDisplay. Function prototype refer to following:
/*! @copydoc GTSession::OnGotQuotePrintDisplay() */
#define GTAPI_MSG_SESSION_OnGotQuotePrintDisplay	(GTAPI_MSG_SESSION_0 + 77)
//! \brief Callback for OnGotQuoteText. Function prototype refer to following:
/*! @copydoc GTSession::OnGotQuoteText() */
#define GTAPI_MSG_SESSION_OnGotQuoteText			(GTAPI_MSG_SESSION_0 + 78)
//! \brief Callback for OnGotQuoteNoii. Function prototype refer to following:
/*! @copydoc GTSession::OnGotQuoteNoii() */
#define GTAPI_MSG_SESSION_OnGotQuoteNoii			(GTAPI_MSG_SESSION_0 + 79)
//! \brief Callback for OnGotQuoteNyseAlert. Function prototype refer to following:
/*! @copydoc GTSession::OnGotQuoteNyseAlert() */
#define GTAPI_MSG_SESSION_OnGotQuoteNyseAlert			(GTAPI_MSG_SESSION_0 + 80)

//! \brief Callback for OnGotChartConnected. Function prototype refer to following:
/*! @copydoc GTSession::OnGotChartConnected() */
#define GTAPI_MSG_SESSION_OnGotChartConnected		(GTAPI_MSG_SESSION_0 + 81)
//! \brief Callback for OnGotChartDisconnected. Function prototype refer to following:
/*! @copydoc GTSession::OnGotChartDisconnected() */
#define GTAPI_MSG_SESSION_OnGotChartDisconnected	(GTAPI_MSG_SESSION_0 + 82)
//! \brief Callback for OnGotChartRecord. Function prototype refer to following:
/*! @copydoc GTSession::OnGotChartRecord() */
#define GTAPI_MSG_SESSION_OnGotChartRecord			(GTAPI_MSG_SESSION_0 + 83)
//! \brief Callback for OnGotChartRecordHistory. Function prototype refer to following:
/*! @copydoc GTSession::OnGotChartRecordHistory() */
#define GTAPI_MSG_SESSION_OnGotChartRecordHistory	(GTAPI_MSG_SESSION_0 + 84)
//! \brief Callback for OnGotChartRecordRefresh. Function prototype refer to following:
/*! @copydoc GTSession::OnGotChartRecordRefresh() */
#define GTAPI_MSG_SESSION_OnGotChartRecordRefresh	(GTAPI_MSG_SESSION_0 + 85)
//! \brief Callback for OnGotChartRecordDisplay. Function prototype refer to following:
/*! @copydoc GTSession::OnGotChartRecordDisplay() */
#define GTAPI_MSG_SESSION_OnGotChartRecordDisplay	(GTAPI_MSG_SESSION_0 + 86)

//! \brief Callback for OnGotQuoteNYXI. Function prototype refer to following:
/*! @copydoc GTSession::OnGotQuoteNYXI() */
#define GTAPI_MSG_SESSION_OnGotQuoteNYXI			(GTAPI_MSG_SESSION_0 + 87)

//
////! \brief Callback for OnGotChainRecord. Function prototype refer to following:
///*! @copydoc GTSession::OnGotChainRecord() */
//#define GTAPI_MSG_SESSION_OnGotChainRecord			(GTAPI_MSG_SESSION_0 + 91)
////! \brief Callback for OnGotChainUnderly. Function prototype refer to following:
///*! @copydoc GTSession::OnGotChainUnderly() */
//#define GTAPI_MSG_SESSION_OnGotChainUnderly			(GTAPI_MSG_SESSION_0 + 92)
////! \brief Callback for OnGotChainOpenInt. Function prototype refer to following:
///*! @copydoc GTSession::OnGotChainOpenInt() */
//#define GTAPI_MSG_SESSION_OnGotChainOpenInt			(GTAPI_MSG_SESSION_0 + 93)

//@}
// Call Back
//! \cond INTERNAL
#define DEF_SESSION_CALLBACK0(name)				\
	typedef int (CALLBACK * PFNSession##name)(LPGTSESSION hSession);

#define DEF_SESSION_CALLBACK1(name, type)		\
	typedef int (CALLBACK * PFNSession##name)(LPGTSESSION hSession, type param);

#define DECL_SESSION_CALLBACK(name)				\
	PFNSession##name	m_pfn##name;			\
	HWND				m_wnd##name;			\
	UINT				m_msg##name;			\
//! \endcond
DEF_SESSION_CALLBACK1(OnCreateStock, struct tagGTSTOCK *);
DEF_SESSION_CALLBACK1(OnDeleteStock, struct tagGTSTOCK *);
DEF_SESSION_CALLBACK1(OnInitStock, struct tagGTSTOCK *);
DEF_SESSION_CALLBACK1(OnExitStock, struct tagGTSTOCK *);

DEF_SESSION_CALLBACK0(OnTick);
DEF_SESSION_CALLBACK1(OnExecMsg, const GTExecMsg32 *);

DEF_SESSION_CALLBACK0(OnExecHeartBeat);
DEF_SESSION_CALLBACK0(OnExecConnected);
DEF_SESSION_CALLBACK0(OnExecDisconnected);

DEF_SESSION_CALLBACK1(OnExecMsgState, const GTServerState32 *);
DEF_SESSION_CALLBACK1(OnExecMsgErrMsg, const GTErrMsg32 *);
DEF_SESSION_CALLBACK1(OnExecMsgChat, const GTChat32 *);
DEF_SESSION_CALLBACK1(OnExecMsgPopup, const GTPopup32 *);

DEF_SESSION_CALLBACK1(OnExecMsgLogin, BOOL);
DEF_SESSION_CALLBACK0(OnExecMsgLoggedin);
DEF_SESSION_CALLBACK1(OnExecMsgLogout, const GTErrMsg32 *);
DEF_SESSION_CALLBACK1(OnExecMsgUser, const GTUser32 *);
DEF_SESSION_CALLBACK1(OnExecMsgAccount, const GTAccount32 *);

DEF_SESSION_CALLBACK1(OnExecMsgOpenPosition, const GTOpenPosition32 *);
DEF_SESSION_CALLBACK1(OnExecMsgTrade, const GTTrade32 *);
DEF_SESSION_CALLBACK1(OnExecMsgPending, const GTPending32 *);
DEF_SESSION_CALLBACK1(OnExecMsgSending, const GTSending32 *);
DEF_SESSION_CALLBACK1(OnSendingOrder, const GTSending32 *);
DEF_SESSION_CALLBACK1(OnExecMsgCanceling, const GTCancel32 *);
DEF_SESSION_CALLBACK1(OnExecMsgCancel, const GTCancel32 *);
DEF_SESSION_CALLBACK1(OnExecMsgReject, const GTReject32 *);
DEF_SESSION_CALLBACK1(OnExecMsgRemove, const GTRemove32 *);
DEF_SESSION_CALLBACK1(OnExecMsgRejectCancel, const GTRejectCancel32 *);
DEF_SESSION_CALLBACK1(OnExecMsgStatus, const GTStatus32 *);

DEF_SESSION_CALLBACK0(OnGotLevel2Connected);
DEF_SESSION_CALLBACK0(OnGotLevel2Disconnected);
DEF_SESSION_CALLBACK1(OnGotLevel2Record, const GTLevel232 *);
DEF_SESSION_CALLBACK1(OnGotLevel2Refresh, LPCSTR);
DEF_SESSION_CALLBACK1(OnGotLevel2Display, LPCSTR);
DEF_SESSION_CALLBACK1(OnGotLevel2Clear, MMID);
DEF_SESSION_CALLBACK1(OnGotLevel2Text, const GTQuoteText32 *);

DEF_SESSION_CALLBACK0(OnGotQuoteConnected);
DEF_SESSION_CALLBACK0(OnGotQuoteDisconnected);
DEF_SESSION_CALLBACK1(OnGotQuoteLevel1, const GTLevel132 *);
DEF_SESSION_CALLBACK1(OnGotQuoteNoii, const GTNoii32 *);
DEF_SESSION_CALLBACK1(OnGotQuoteNYXI, const GT_NYXI_32 *);
DEF_SESSION_CALLBACK1(OnGotQuoteNyseAlert, const GTNyseAlert32 *);
DEF_SESSION_CALLBACK1(OnGotQuotePrint, const GTPrint32 *);
DEF_SESSION_CALLBACK1(OnGotQuotePrintHistory, const GTPrint32 *);
DEF_SESSION_CALLBACK1(OnGotQuotePrintRefresh, const GTPrint32 *);
DEF_SESSION_CALLBACK1(OnGotQuotePrintDisplay, const GTPrint32 *);
DEF_SESSION_CALLBACK1(OnGotQuoteText, const GTQuoteText32 *);

DEF_SESSION_CALLBACK0(OnGotChartConnected);
DEF_SESSION_CALLBACK0(OnGotChartDisconnected);
DEF_SESSION_CALLBACK1(OnGotChartRecord, const GTChart32 *);
DEF_SESSION_CALLBACK1(OnGotChartRecordHistory, const GTChart32 *);
DEF_SESSION_CALLBACK1(OnGotChartRecordRefresh, const GTChart32 *);
DEF_SESSION_CALLBACK1(OnGotChartRecordDisplay, const GTChart32 *);

struct tagGTSTOCK;

#pragma pack(8)
/*! \ingroup c
*/
/*! \struct GTSession32
	\brief The same to tagGTSession32.

	\copydoc tagGTSession32
*/

/*! \typedef tagGTSession32 GTSession32 
*/
/*!	\struct tagGTSession32
	\brief The session structure
	
	This struct describes the session information, which is the base of GTSession class in C++ version. In C version, it is 
	the only data struct for session description, which can be visit by gtGetSession32.

	It encloses the important session information, like the user information (m_pUser32), system settings (m_pSetting32),
	information about stocks (m_pStocks), and session statistics.

	\sa GTSession
*/

typedef struct tagGTSession32
{
	//! \brief System Time
	/*!
		\remark A pointer to internel system time. This time is updated once a second. 
		If the API is running with real quotes, the time is the same as local computer time. 
		Or it is the same as history quotes time.
	*/
	const GTime32	*m_pSysTime32;
	
	//! \brief System Date
	/*!
		\remark A pointer to internel system date. 
		If the API is running with real quotes, the date is the same as local computer date. 
		Or it is the same as history quotes date.
	*/
	const GDate32	*m_pSysDate32;


	//! \brief Is the session logged in?
	BOOL			m_bLoggedIn;
	//! \brief [Internal Used] Transfer login information? Don't change it.
	BOOL			m_bTransfering;

	//! \brief The pointer to the stock map.
	struct tagGTSTOCK *	m_pStocks;
	//! \brief The pointer to the GTSetting32.
	/*!
		Follow it one can visit the GTSetting32 for the session. 
		In C++ version, GTSession::m_setting is the alternate for this. In C version,
		use gtGetSetting32 and gtSetSetting32 to visit the settings.
	*/
	GTSetting32		*m_pSetting32;
	//! \brief The pointer to the user information GTUser32.
	GTUser32		*m_pUser32;
	GTAccount32		*m_pAccount32;


	//! \brief Total tickets number for the session.
	int			m_nTotalTickets;
	//! \brief Total filled tickets number for the session.
	int			m_nTotalFills;
	//! \brief Total filled share number for the session.
	int			m_nTotalShares;
	//! \brief Profit/loss on open position for the session.
	double		m_dblOpenPL;
	//! \brief Profit/loss on closed position for the session.
	double		m_dblClosePL;
	//! \brief Passthrough for the session.
	double		m_dblPassThr;
	//! \brief Gross net profit/loss for the session.
	double		m_dblGrossNet;

	LPARAM		m_lParam;

	// Call Back Function
	DECL_SESSION_CALLBACK(OnCreateStock)
	DECL_SESSION_CALLBACK(OnDeleteStock)
	DECL_SESSION_CALLBACK(OnInitStock)
	DECL_SESSION_CALLBACK(OnExitStock)

	DECL_SESSION_CALLBACK(OnTick)
	DECL_SESSION_CALLBACK(OnExecMsg)

	DECL_SESSION_CALLBACK(OnExecHeartBeat)
	DECL_SESSION_CALLBACK(OnExecConnected)
	DECL_SESSION_CALLBACK(OnExecDisconnected)

	DECL_SESSION_CALLBACK(OnExecMsgState)
	DECL_SESSION_CALLBACK(OnExecMsgErrMsg)
	DECL_SESSION_CALLBACK(OnExecMsgChat)
	DECL_SESSION_CALLBACK(OnExecMsgPopup)

	DECL_SESSION_CALLBACK(OnExecMsgLogin)
	DECL_SESSION_CALLBACK(OnExecMsgLoggedin)
	DECL_SESSION_CALLBACK(OnExecMsgLogout)
	DECL_SESSION_CALLBACK(OnExecMsgUser)
	DECL_SESSION_CALLBACK(OnExecMsgAccount)

	DECL_SESSION_CALLBACK(OnExecMsgOpenPosition)
	DECL_SESSION_CALLBACK(OnExecMsgTrade)
	DECL_SESSION_CALLBACK(OnExecMsgPending)
	DECL_SESSION_CALLBACK(OnExecMsgSending)
	DECL_SESSION_CALLBACK(OnSendingOrder)
	DECL_SESSION_CALLBACK(OnExecMsgCanceling)
	DECL_SESSION_CALLBACK(OnExecMsgCancel)
	DECL_SESSION_CALLBACK(OnExecMsgReject)
	DECL_SESSION_CALLBACK(OnExecMsgRemove)
	DECL_SESSION_CALLBACK(OnExecMsgRejectCancel)
	DECL_SESSION_CALLBACK(OnExecMsgStatus)

	DECL_SESSION_CALLBACK(OnGotLevel2Connected)
	DECL_SESSION_CALLBACK(OnGotLevel2Disconnected)
	DECL_SESSION_CALLBACK(OnGotLevel2Record)
	DECL_SESSION_CALLBACK(OnGotLevel2Refresh)
	DECL_SESSION_CALLBACK(OnGotLevel2Display)
	DECL_SESSION_CALLBACK(OnGotLevel2Clear)
	DECL_SESSION_CALLBACK(OnGotLevel2Text)

	DECL_SESSION_CALLBACK(OnGotQuoteConnected)
	DECL_SESSION_CALLBACK(OnGotQuoteDisconnected)
	DECL_SESSION_CALLBACK(OnGotQuoteLevel1)
	DECL_SESSION_CALLBACK(OnGotQuoteNoii)
	DECL_SESSION_CALLBACK(OnGotQuoteNYXI)
	DECL_SESSION_CALLBACK(OnGotQuoteNyseAlert)
	DECL_SESSION_CALLBACK(OnGotQuotePrint)
	DECL_SESSION_CALLBACK(OnGotQuotePrintHistory)
	DECL_SESSION_CALLBACK(OnGotQuotePrintRefresh)
	DECL_SESSION_CALLBACK(OnGotQuotePrintDisplay)
	DECL_SESSION_CALLBACK(OnGotQuoteText)

	DECL_SESSION_CALLBACK(OnGotChartConnected)
	DECL_SESSION_CALLBACK(OnGotChartDisconnected)
	DECL_SESSION_CALLBACK(OnGotChartRecord)
	DECL_SESSION_CALLBACK(OnGotChartRecordHistory)
	DECL_SESSION_CALLBACK(OnGotChartRecordRefresh)
	DECL_SESSION_CALLBACK(OnGotChartRecordDisplay)

#ifdef OPTION_ORDER
	DECL_SESSION_CALLBACK(OnGotChainRecord)
	DECL_SESSION_CALLBACK(OnGotChainUnderly)
	DECL_SESSION_CALLBACK(OnGotChainOpenInt)
#endif 

	/*!
	  	\brief Request NYSE Alert data.
	*/
	BOOL			m_bNYSEALERT;

	/*!
	  	\brief Request NYSE Imbalance data.
	*/
	BOOL			m_bNYSEImbalance;
}GTSession32;
#pragma pack()

#endif//__GTSESSION32_H__

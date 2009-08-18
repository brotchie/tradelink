/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTStock32.h
	\brief interface for the GTStock32 class.
 */

#ifndef __GTSTOCK32_H__
#define __GTSTOCK32_H__

#include "GTime32.h"
#include "GTOpenPosition32.h"
#include "GTTrade32.h"
#include "GTPending32.h"
#include "GTSending32.h"
#include "GTCancel32.h"
#include "GTReject32.h"
#include "GTRejectCancel32.h"
#include "GTStatus32.h"
#include "GTNOII32.h"
#include "GT_NYXI_32.h"
#include "GTNyseAlert32.h"
#include "GTRemove32.h"
#include "GTOrder32.h"

#include "GTOrder32.h"
#include "GTLevel132.h"
#include "GTLevel232.h"
#include "GTLevel2Box32.h"
#include "GTPrint32.h"
#include "GTChart32.h"

#include "GTErrMsg32.h"

//! \cond INTERNAL
typedef struct tagGTSTOCK { unsigned _unused; } GTSTOCK, *LPGTSTOCK;
//! \endcond
/*! \ingroup c
*/
/*! @name Definition for Stock Callback Type
	\anchor Stock_Callback_Event
*/
//@{
//! \brief Base.
#define GTAPI_MSG_STOCK_0						2000

//! \brief Callback for stock init. Function prototype refer to following:
/*! @copydoc GTStock::Init() */
#define GTAPI_MSG_STOCK_Init					(GTAPI_MSG_STOCK_0 + 1)
//! \brief Callback for stock exit. Function prototype refer to following:
/*! @copydoc GTStock::Exit() */
#define GTAPI_MSG_STOCK_Exit					(GTAPI_MSG_STOCK_0 + 2)

//! \brief Callback for stock ontick. Function prototype refer to following:
/*! @copydoc GTStock::OnTick() */
#define GTAPI_MSG_STOCK_OnTick					(GTAPI_MSG_STOCK_0 + 11)
//! \brief Callback for stock OnExecCmd. Function prototype refer to following:
/*! @copydoc GTStock::OnExecCmd() */
#define GTAPI_MSG_STOCK_OnExecCmd				(GTAPI_MSG_STOCK_0 + 12)

//! \brief Callback for stock OnSendingOrder. Function prototype refer to following:
/*! @copydoc GTStock::OnSendingOrder() */
#define GTAPI_MSG_STOCK_OnSendingOrder			(GTAPI_MSG_STOCK_0 + 20)
//! \brief Callback for stock OnExecMsgErrMsg. Function prototype refer to following:
/*! @copydoc GTStock::OnExecMsgErrMsg() */
#define GTAPI_MSG_STOCK_OnExecMsgErrMsg			(GTAPI_MSG_STOCK_0 + 21)
//! \brief Callback for stock OnExecMsgOpenPosition. Function prototype refer to following:
/*! @copydoc GTStock::OnExecMsgOpenPosition() */
#define GTAPI_MSG_STOCK_OnExecMsgOpenPosition	(GTAPI_MSG_STOCK_0 + 22)
//! \brief Callback for stock OnExecMsgTrade. Function prototype refer to following:
/*! @copydoc GTStock::OnExecMsgTrade() */
#define GTAPI_MSG_STOCK_OnExecMsgTrade			(GTAPI_MSG_STOCK_0 + 23)
//! \brief Callback for stock OnExecMsgPending. Function prototype refer to following:
/*! @copydoc GTStock::OnExecMsgPending() */
#define GTAPI_MSG_STOCK_OnExecMsgPending		(GTAPI_MSG_STOCK_0 + 24)
//! \brief Callback for stock OnExecMsgSending. Function prototype refer to following:
/*! @copydoc GTStock::OnExecMsgSending() */
#define GTAPI_MSG_STOCK_OnExecMsgSending		(GTAPI_MSG_STOCK_0 + 25)
//! \brief Callback for stock OnExecMsgCanceling. Function prototype refer to following:
/*! @copydoc GTStock::OnExecMsgCanceling() */
#define GTAPI_MSG_STOCK_OnExecMsgCanceling		(GTAPI_MSG_STOCK_0 + 26)
//! \brief Callback for stock OnExecMsgCancel. Function prototype refer to following:
/*! @copydoc GTStock::OnExecMsgCancel() */
#define GTAPI_MSG_STOCK_OnExecMsgCancel			(GTAPI_MSG_STOCK_0 + 27)
//! \brief Callback for stock OnExecMsgReject. Function prototype refer to following:
/*! @copydoc GTStock::OnExecMsgReject() */
#define GTAPI_MSG_STOCK_OnExecMsgReject			(GTAPI_MSG_STOCK_0 + 28)
//! \brief Callback for stock OnExecMsgRemove. Function prototype refer to following:
/*! @copydoc GTStock::OnExecMsgRemove() */
#define GTAPI_MSG_STOCK_OnExecMsgRemove			(GTAPI_MSG_STOCK_0 + 29)
//! \brief Callback for stock OnExecMsgRejectCancel. Function prototype refer to following:
/*! @copydoc GTStock::OnExecMsgRejectCancel() */
#define GTAPI_MSG_STOCK_OnExecMsgRejectCancel	(GTAPI_MSG_STOCK_0 + 30)
//! \brief Callback for stock OnExecMsgIOIStatus. Function prototype refer to following:
/*! @copydoc GTStock::OnExecMsgIOIStatus() */
#define GTAPI_MSG_STOCK_OnExecMsgIOIStatus		(GTAPI_MSG_STOCK_0 + 31)

//! \brief Callback for stock OnGotLevel2Record. Function prototype refer to following:
/*! @copydoc GTStock::OnGotLevel2Record() */
#define GTAPI_MSG_STOCK_OnGotLevel2Record		(GTAPI_MSG_STOCK_0 + 41)
//! \brief Callback for stock OnGotLevel2Refresh. Function prototype refer to following:
/*! @copydoc GTStock::OnGotLevel2Refresh() */
#define GTAPI_MSG_STOCK_OnGotLevel2Refresh		(GTAPI_MSG_STOCK_0 + 42)
//! \brief Callback for stock OnGotLevel2Display. Function prototype refer to following:
/*! @copydoc GTStock::OnGotLevel2Display() */
#define GTAPI_MSG_STOCK_OnGotLevel2Display		(GTAPI_MSG_STOCK_0 + 43)
//! \brief Callback for stock OnGotLevel2Clear. Function prototype refer to following:
/*! @copydoc GTStock::OnGotLevel2Clear() */
#define GTAPI_MSG_STOCK_OnGotLevel2Clear		(GTAPI_MSG_STOCK_0 + 44)

//! \brief Callback for stock OnGotQuoteLevel1. Function prototype refer to following:
/*! @copydoc GTStock::OnGotQuoteLevel1() */
#define GTAPI_MSG_STOCK_OnGotQuoteLevel1		(GTAPI_MSG_STOCK_0 + 51)
//! \brief Callback for stock OnGotQuotePrint. Function prototype refer to following:
/*! @copydoc GTStock::OnGotQuotePrint() */
#define GTAPI_MSG_STOCK_OnGotQuotePrint			(GTAPI_MSG_STOCK_0 + 52)
//! \brief Callback for stock OnGotQuotePrintHistory. Function prototype refer to following:
/*! @copydoc GTStock::OnGotQuotePrintHistory() */
#define GTAPI_MSG_STOCK_OnGotQuotePrintHistory	(GTAPI_MSG_STOCK_0 + 53)
//! \brief Callback for stock OnGotQuotePrintRefresh. Function prototype refer to following:
/*! @copydoc GTStock::OnGotQuotePrintRefresh() */
#define GTAPI_MSG_STOCK_OnGotQuotePrintRefresh	(GTAPI_MSG_STOCK_0 + 54)
//! \brief Callback for stock OnGotQuotePrintDisplay. Function prototype refer to following:
/*! @copydoc GTStock::OnGotQuotePrintDisplay() */
#define GTAPI_MSG_STOCK_OnGotQuotePrintDisplay	(GTAPI_MSG_STOCK_0 + 55)
//! \brief Callback for stock OnGetPrintColor. Function prototype refer to following:
/*! @copydoc GTStock::OnGetPrintColor() */
#define GTAPI_MSG_STOCK_OnGetPrintColor			(GTAPI_MSG_STOCK_0 + 56)
//! \brief Callback for stock OnGotQuoteNoii. Function prototype refer to following:
/*! @copydoc GTStock::OnGotQuoteNoii() */
#define GTAPI_MSG_STOCK_OnGotQuoteNoii			(GTAPI_MSG_STOCK_0 + 57)
//! \brief Callback for stock OnGotQuoteNyseAlert. Function prototype refer to following:
/*! @copydoc GTStock::OnGotQuoteNyseAlert() */
#define GTAPI_MSG_STOCK_OnGotQuoteNyseAlert			(GTAPI_MSG_STOCK_0 + 58)
//! \brief Callback for stock OnGotQuoteNYXI. Function prototype refer to following:
/*! @copydoc GTStock::OnGotQuoteNYXI() */
#define GTAPI_MSG_STOCK_OnGotQuoteNYXI			(GTAPI_MSG_STOCK_0 + 59)

//! \brief Callback for stock OnGotChartRecord. Function prototype refer to following:
/*! @copydoc GTStock::OnGotChartRecord() */
#define GTAPI_MSG_STOCK_OnGotChartRecord		(GTAPI_MSG_STOCK_0 + 61)
//! \brief Callback for stock OnGotChartRecordHistory. Function prototype refer to following:
/*! @copydoc GTStock::OnGotChartRecordHistory() */
#define GTAPI_MSG_STOCK_OnGotChartRecordHistory	(GTAPI_MSG_STOCK_0 + 62)
//! \brief Callback for stock OnGotChartRecordRefresh. Function prototype refer to following:
/*! @copydoc GTStock::OnGotChartRecordRefresh() */
#define GTAPI_MSG_STOCK_OnGotChartRecordRefresh	(GTAPI_MSG_STOCK_0 + 63)
//! \brief Callback for stock OnGotChartRecordDisplay. Function prototype refer to following:
/*! @copydoc GTStock::OnGotChartRecordDisplay() */
#define GTAPI_MSG_STOCK_OnGotChartRecordDisplay	(GTAPI_MSG_STOCK_0 + 64)

//! \brief Callback for stock PrePlaceOrder. Function prototype refer to following:
/*! @copydoc GTStock::PrePlaceOrder() */
#define GTAPI_MSG_STOCK_PrePlaceOrder			(GTAPI_MSG_STOCK_0 + 71)
//! \brief Callback for stock PostPlaceOrder. Function prototype refer to following:
/*! @copydoc GTStock::PostPlaceOrder() */
#define GTAPI_MSG_STOCK_PostPlaceOrder			(GTAPI_MSG_STOCK_0 + 72)

//! \brief Callback for stock OnBestBidPriceChanged. Function prototype refer to following:
/*! @copydoc GTStock::OnBestBidPriceChanged() */
#define GTAPI_MSG_STOCK_OnBestBidPriceChanged	(GTAPI_MSG_STOCK_0 + 81)
//! \brief Callback for stock OnBestAskPriceChanged. Function prototype refer to following:
/*! @copydoc GTStock::OnBestAskPriceChanged() */
#define GTAPI_MSG_STOCK_OnBestAskPriceChanged	(GTAPI_MSG_STOCK_0 + 82)


////! \brief Callback for stock OnGotChainRecord. Function prototype refer to following:
///*! @copydoc GTStock::OnGotChainRecord() */
//#define GTAPI_MSG_STOCK_OnGotChainRecord		(GTAPI_MSG_STOCK_0 + 91)
////! \brief Callback for stock OnGotChainUnderly. Function prototype refer to following:
///*! @copydoc GTStock::OnGotChainUnderly() */
//#define GTAPI_MSG_STOCK_OnGotChainUnderly		(GTAPI_MSG_STOCK_0 + 92)
////! \brief Callback for stock OnGotChainOpenInt. Function prototype refer to following:
///*! @copydoc GTStock::OnGotChainOpenInt() */
//#define GTAPI_MSG_STOCK_OnGotChainOpenInt		(GTAPI_MSG_STOCK_0 + 93)

//@}
// Call Back

//@{
//! \brief [internal used] Call Back defines. 
#define DEF_STOCK_CALLBACK0(name)	\
	typedef int (CALLBACK * PFNStock##name)(LPGTSTOCK hStock);

#define DEF_STOCK_CALLBACK1(name, type)		\
	typedef int (CALLBACK * PFNStock##name)(LPGTSTOCK hStock, type param);

#define DECL_STOCK_CALLBACK(name)				\
	PFNStock##name		m_pfn##name;			\
	HWND				m_wnd##name;			\
	UINT				m_msg##name;			\

DEF_STOCK_CALLBACK0(Init);
DEF_STOCK_CALLBACK0(Exit);

DEF_STOCK_CALLBACK0(OnTick);
DEF_STOCK_CALLBACK1(OnExecCmd, LPCSTR);

DEF_STOCK_CALLBACK1(OnExecMsgErrMsg, const GTErrMsg32 *);
DEF_STOCK_CALLBACK1(OnExecMsgOpenPosition, const GTOpenPosition32 *);
DEF_STOCK_CALLBACK1(OnExecMsgTrade, const GTTrade32 *);
DEF_STOCK_CALLBACK1(OnExecMsgPending, const GTPending32 *);
DEF_STOCK_CALLBACK1(OnExecMsgSending, const GTSending32 *);
DEF_STOCK_CALLBACK1(OnSendingOrder, const GTSending32 *);
DEF_STOCK_CALLBACK1(OnExecMsgCanceling, const GTCancel32 *);
DEF_STOCK_CALLBACK1(OnExecMsgCancel, const GTCancel32 *);
DEF_STOCK_CALLBACK1(OnExecMsgReject, const GTReject32 *);
DEF_STOCK_CALLBACK1(OnExecMsgRemove, const GTRemove32 *);
DEF_STOCK_CALLBACK1(OnExecMsgRejectCancel, const GTRejectCancel32 *);
DEF_STOCK_CALLBACK1(OnExecMsgIOIStatus, const GTIOIStatus32 *);

DEF_STOCK_CALLBACK1(OnGotLevel2Record, const GTLevel232 *);
DEF_STOCK_CALLBACK0(OnGotLevel2Refresh);
DEF_STOCK_CALLBACK0(OnGotLevel2Display);
DEF_STOCK_CALLBACK1(OnGotLevel2Clear, MMID);

DEF_STOCK_CALLBACK1(OnGotQuoteLevel1, const GTLevel132 *);
DEF_STOCK_CALLBACK1(OnGotQuoteNoii, const GTNoii32 *);
DEF_STOCK_CALLBACK1(OnGotQuoteNYXI, const GT_NYXI_32 *);
DEF_STOCK_CALLBACK1(OnGotQuoteNyseAlert, const GTNyseAlert32 *);
DEF_STOCK_CALLBACK1(OnGotQuotePrint, const GTPrint32 *);
DEF_STOCK_CALLBACK1(OnGotQuotePrintHistory, const GTPrint32 *);
DEF_STOCK_CALLBACK0(OnGotQuotePrintRefresh);
DEF_STOCK_CALLBACK0(OnGotQuotePrintDisplay);
DEF_STOCK_CALLBACK1(OnGetPrintColor, const GTPrint32 *);

DEF_STOCK_CALLBACK1(OnGotChartRecord, const GTChart32 *);
DEF_STOCK_CALLBACK1(OnGotChartRecordHistory, const GTChart32 *);
DEF_STOCK_CALLBACK1(OnGotChartRecordRefresh, const GTChart32 *);
DEF_STOCK_CALLBACK1(OnGotChartRecordDisplay, const GTChart32 *);

DEF_STOCK_CALLBACK1(PrePlaceOrder, const GTOrder32 *);
DEF_STOCK_CALLBACK1(PostPlaceOrder, const GTOrder32 *);

DEF_STOCK_CALLBACK0(OnBestBidPriceChanged);
DEF_STOCK_CALLBACK0(OnBestAskPriceChanged);
//@}
#pragma pack(8)
/*! \ingroup c
*/
/*! \struct GTStock32
	\brief The stock structure. The same as tagGTStock32.

	\copydoc tagGTStock32
*/
/*! \typedef tagGTStock32 GTStock32 
*/
/*!	\struct tagGTStock32
	\brief The stock structure.

	This is the base struct for GTStock. In C version, only this is accessible. It provides the 
	accessibility for C code.

	It encloses the information for a symbol. 

	\sa GTStock 
*/
typedef struct tagGTStock32
{
	//! \brief Symbol name
	char				m_szStock[LEN_STOCK + 1];

	//! \brief System Time
	/*!
		A pointer to internel system time. This time is updated once a second. 
		If the API is running with real quotes, the time is the same as local computer time. 
		Or it is the same as history quotes time.
	*/
	const GTime32		*m_pSysTime32;

	//! \brief System Date
	/*!
		\remark A pointer to internel system date. 
		If the API is running with real quotes, the date is the same as local computer date. 
		Or it is the same as history quotes date.
	*/
	const GDate32		*m_pSysDate32;

	//! \brief A pointer to openposition information
	/*! 
		It is updated by the API system in real time. 

		In C version, call gtGetStockOpenPosition32() to get the open position information.

		In C++ version API, this is just pointing to the GTStock::m_open.
	*/
	GTOpenPosition32	*m_pOpen;

	//! \brief A pointer to colleciton of the Prints
	/*! 
		This keeps the all the print records. It is updated by the API system in real time. 

		In C version, use gtGetStockFirstTrade32() and gtGetStockNextTrade32() to get the trades
		for a symbol.

		In C++ version API, this is just pointing to the GTStock::m_prints.
	*/
	GTTRADES			*m_pTrades;
	//! \brief A pointer to colleciton of the pending orders
	/*! 
		This keeps the all the pending order records. It is updated by the API system in real time. 
		
		In C version, use gtGetStockFirstPending32() and gtGetStockNextPending32() to get the pending
		orders for a symbol.

		In C++ version API, this is just pointing to the GTStock::m_pending.
	*/
	GTPENDINGS			*m_pPendings;
	//! \brief A pointer to colleciton of the sending orders
	/*! 
		This keeps the all the sending order records. It is updated by the API system in real time. 

		In C++ version API, this is just pointing to the GTStock::m_sending.
	*/
	GTSENDINGS			*m_pSendings;
	//! \brief A pointer to colleciton of the cancelled orders
	/*! 
		This keeps the all the cancelled order records. It is updated by the API system in real time. 

		In C++ version API, this is just pointing to the GTStock::m_cancel.
	*/
	GTCANCELS			*m_pCancels;
	//! \brief A pointer to colleciton of the rejected orders
	/*! 
		This keeps the all the rejected order records. It is updated by the API system in real time. 

		In C++ version API, this is just pointing to the GTStock::m_reject.
	*/
	GTREJECTS			*m_pRejects;
	
	//! \brief A pointer to default order
	/*!
		This is the default GTOrder struct that has been filled with the information of this
		GTStock object. Each time when sending an order, you should copy this struct to the
		GTOrder. After that, you can fill in the addition options like place, method and so on
		specified for the order.

		In the C version API, you can also call gtInitOrder() function to initialize the order
		struct. 

		In C++ version API, this is just pointing to the GTStock::m_defOrder.
	*/
	GTOrder32			*m_pDefOrder;

	//! \brief A pointer to Level 1
	/*!	\anchor m_plevel1
		This GTLevel132 keeps the newest level 1 information. It is realtimely updated by the API system. 
		In this struct, the NBBO(National Best Bid/Ask), exchange BBO(Best Bid/Ask) and 
		daily bar information are kept. You can also use this to retrieve the NBBO and daily bar
		information.

		But it is more convenient to use \ref m_pLevel2 (the Level 2 list) data as the source of 
		Bid/Ask price. There are several reasons. First of all, in \ref m_pLevel2, lists of bid/ask prices
		are kept, by looking into the lists, you can get more information about the prices, not only 
		the NBBO, but also other book prices; also there are methods that are easily called for 
		NBBO (C++: GTLevel2Box::GetBestAskPrice() and GTLevel2Box::GetBestBidPrice(), 
		C:gtGetBestBidLevel2Item(), gtGetBestAskLevel2Item(), gtGetBestBidLevel2Price(), gtGetBestAskLevel2Price()).

		Also, the level 2 data includes more information about the quote. If the client program
		needs to get information about the quote for one specified exchange/ECN, you have to 
		look into level 2 instead of level 1, since level 1 can give you only an overall 
		description of the inside quote. 
		
		Even so, in some circumstance, using level 1 is suggested. If you only want to read NBBO, 
		level 1 is the enough source for this job. Since Level 1 has a much more simple structure
		than level 2, it will be more fast to retrieve information from level 1.

		In C++ version API, this is just pointing to GTStock::m_level1. 
	*/
	GTLevel132			*m_pLevel1;
	//! \brief A pointer to collection of Level 2 data
	/*! \anchor m_level2
		This GTLEVEL2BOX32 keeps the all the level 2 record. It is updated by the API system in real time. 
	
		In C version API, you can travel through the level 2 records by:
		\code
			int count = gtGetBidLevel2Count(hStock);

			while(count){
				GTLevel232* pLevel232 = gtGetBidLevel2Item(hStock, count);
				// Handle the level 2 record.
			}
		\endcode

		In C++ version API, this is just pointing to GTStock::m_level2. 

		Refer to tagGTLevel2Box32 for the struct details.
	*/
	GTLEVEL2BOX32		*m_pLevel2;

//@{
	//! \brief Flags (Internal used)
	BOOL				m_bLevel1;
	BOOL				m_bLevel2;
	BOOL				m_bPrint;
	BOOL				m_bChart;
	BOOL				m_bChartFromPrint;
	BOOL				m_bNOII;
//@}

//@{
	//! \brief Statistics (Internal used)
	int			m_nTotalTickets;
	int			m_nTotalFills;
	int			m_nTotalShares;
	double		m_dblOpenPL;
	double		m_dblClosePL;
	double		m_dblPassThr;
	double		m_dblGrossNet;
//@}
	LPARAM		m_lParam;

	// Call Back Function
	DECL_STOCK_CALLBACK(Init)
	DECL_STOCK_CALLBACK(Exit)

	DECL_STOCK_CALLBACK(OnTick)
	DECL_STOCK_CALLBACK(OnExecCmd)

	DECL_STOCK_CALLBACK(OnExecMsgErrMsg)
	DECL_STOCK_CALLBACK(OnExecMsgOpenPosition)
	DECL_STOCK_CALLBACK(OnExecMsgTrade)
	DECL_STOCK_CALLBACK(OnExecMsgPending)
	DECL_STOCK_CALLBACK(OnExecMsgSending)
	DECL_STOCK_CALLBACK(OnSendingOrder)
	DECL_STOCK_CALLBACK(OnExecMsgCanceling)
	DECL_STOCK_CALLBACK(OnExecMsgCancel)
	DECL_STOCK_CALLBACK(OnExecMsgReject)
	DECL_STOCK_CALLBACK(OnExecMsgRemove)
	DECL_STOCK_CALLBACK(OnExecMsgRejectCancel)
	DECL_STOCK_CALLBACK(OnExecMsgIOIStatus)
	
	DECL_STOCK_CALLBACK(OnGotLevel2Record)
	DECL_STOCK_CALLBACK(OnGotLevel2Refresh)
	DECL_STOCK_CALLBACK(OnGotLevel2Display)
	DECL_STOCK_CALLBACK(OnGotLevel2Clear)

	DECL_STOCK_CALLBACK(OnGotQuoteLevel1)
	DECL_STOCK_CALLBACK(OnGotQuoteNoii)
	DECL_STOCK_CALLBACK(OnGotQuoteNYXI)
	DECL_STOCK_CALLBACK(OnGotQuoteNyseAlert)
	DECL_STOCK_CALLBACK(OnGotQuotePrint)
	DECL_STOCK_CALLBACK(OnGotQuotePrintHistory)
	DECL_STOCK_CALLBACK(OnGotQuotePrintRefresh)
	DECL_STOCK_CALLBACK(OnGotQuotePrintDisplay)
	DECL_STOCK_CALLBACK(OnGetPrintColor)

	DECL_STOCK_CALLBACK(OnGotChartRecord)
	DECL_STOCK_CALLBACK(OnGotChartRecordHistory)
	DECL_STOCK_CALLBACK(OnGotChartRecordRefresh)
	DECL_STOCK_CALLBACK(OnGotChartRecordDisplay)

	DECL_STOCK_CALLBACK(PrePlaceOrder)
	DECL_STOCK_CALLBACK(PostPlaceOrder)

	DECL_STOCK_CALLBACK(OnBestBidPriceChanged)
	DECL_STOCK_CALLBACK(OnBestAskPriceChanged)

#ifdef OPTION_ORDER
	DECL_STOCK_CALLBACK(OnGotChainRecord)
	DECL_STOCK_CALLBACK(OnGotChainUnderly)
	DECL_STOCK_CALLBACK(OnGotChainOpenInt)
#endif
	// Call Back Function

	//! \brief Is this an option symbol?
	BOOL				m_bOptionSymbol;
	//! \brief The underlying stock symbol.
	char				m_szUnderlyStock[LEN_STOCK + 1];
	//! \brief Strike price for this option.
	double				m_dblStrikePrice;
	//! \brief Expire date of the option symbol.
	GDate32				m_gdExpDate;
	//! \brief Exchange that the option can be traded.
	/*
		Usually the value can be the combination of following:
		<TABLE class="dtTABLE" cellspacing="0">
		<TR><TH width="50%">CODE</TH><TH width="50%"> DESCRIPTION</TH></TR>
		<TR><TD>0x01 </TD><TD> EXCHANGE_AMEX	</TD></TR>
		<TR><TD>0x02 </TD><TD> EXCHANGE_BOX		</TD></TR>
		<TR><TD>0x04 </TD><TD> EXCHANGE_CBOE	</TD></TR>
		<TR><TD>0x08 </TD><TD> EXCHANGE_ISE		</TD></TR>
		<TR><TD>0x10 </TD><TD> EXCHANGE_PCX		</TD></TR>
		<TR><TD>0x20 </TD><TD> EXCHANGE_PHLX	</TD></TR>
		</TABLE>
	*/
	char				m_chExchange;
	//! \brief Put or call.
	char				m_chPutOrCall;
	//! \brief Open Interest for the option symbol.
	int					m_nOpenInt;
	//! \brief Multiplier for the option symbol.
	/*
		This is the share number that covered per option contract. 
		In most of the cases this will be 100. Some times after split, this can change to value other than 100.
	*/
	short				m_nMultiplier;
}GTStock32;
#pragma pack()

DEFINE_MAP_CLASS(gtStock, GTSTOCK, GTStock32 *, LPCSTR)

#endif//__GTSTOCK32_H__

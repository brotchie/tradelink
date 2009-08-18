/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTStock.h: interface for the GTStock class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTStock.h
	\brief interface for the GTStock and GTMapStock class.
 */

#if !defined(__GTSTOCK_H__)
#define __GTSTOCK_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTStock32.h"

#include "GTAPI_API.h"
#include "GTErrMsg.h"
#include "GTOpenPosition.h"
#include "GTTrade.h"
#include "GTPending.h"
#include "GTSending.h"
#include "GTCancel.h"
#include "GTReject.h"
#include "GTRemove.h"
#include "GTRejectCancel.h"
#include "GTOrder.h"
#include "GTStatus.h"

#include "GTLevel1.h"
#include "GTLevel2.h"
#include "GTPrint.h"
#include "GTChart.h"
#include "GTNoii.h"
#include "GT_NYXI.h"
#include "GTNyseAlert.h"

#include "GTLevel2Box.h"
#include "GTime0.h"
#include "GDate0.h"

#define INIT_STOCK_CALLBACK(name)	\
	m_pfn##name = NULL;				\
	m_wnd##name = NULL;				\
	m_msg##name = NULL;				\

#define CASE_STOCK_CALLBACK(name, pfn)				\
	case GTAPI_MSG_STOCK_##name: m_pfn##name = (PFNStock##name)pfn; break;		\

typedef CMap<long, long, int, int> CMapSeqNoToStaying;

class GTStock;
/*! \ingroup cpp
*/
/*!	\class GTMapStock
 *	\brief The map used in GTSession to store the GTStock.
	
	Any GTStock is stored in it. It is a CMap, So one can use any CMap methods to visit the GTStock.
	Though it is not encouraged.
 */
class GTAPI_API GTMapStock : public CMap<CString, LPCSTR, GTStock *, GTStock *>
{
public:
	GTMapStock();
	virtual ~GTMapStock();

public:
};

class GTSession;
class GTSessionSystem;
/*! \ingroup cpp
*/
/*! \class GTStock
    \brief Stock class

	This class derives from struct GTStock32. In our API system any object of this class 
	is the representative of a stock. All the operations and information retrieving actions
	about the symbol can be done through this class. 

	You should call GTSession.CreateStock() to generate a new GTStock object. To destory the
	object, call GTSession.DeleteStock() method or GTSession.DeleteAllStocks().

	\par Examples:
	To send an order as limit order, you can set the GTOrder32.chPriceIndicator='2', and the 
	GTOrder32.dblPrice=(the price you place). As below:
	\code
		//stock is an object of GTStock
		GTOrder32 order=stock.m_defOrder;
		if(stock.Bid(order, 
			100,		//Share
			10.19,		//price,
			METHOD_ISLD	//method
			))
			{
				order.chPriceIndicate = '2';	//limit order
				sotck.PlaceOrder(order);
			}
	\endcode
	To send an order as market order, you can set the GTOrder32.chPriceIndicator='1', and the 
	GTOrder32.dblPrice=0. As below:
	\code
		//stock is an object of GTStock
		GTOrder32 order=stock.m_defOrder;
		if(stock.Bid(order, 
			100,		//Share
			0,		//price,
			METHOD_ISLD	//method
			))
			{
				order.chPriceIndicate = '1';	//market order
				sotck.PlaceOrder(order);
			}
	\endcode
	Something to be noticed: 
	1) Not all ECN support market orders, for an example, ISLD does not support market order so far; 
	2) the ECNs deal with market order mostly like the way they treat limit order. So, it is possible that after you send a market order, the order will pend in the market as a limit order.
	3) the safest way to send a order that executes as the market order way, you can send a limit order with a big enough price. 
*/

class GTAPI_API GTStock : public GTStock32
{
	friend		GTSession;
	friend		GTSessionSystem;

protected:
	//! \brief The reference to the GTSession
	/*!
		\remark Each GTStock object will keep a reference to the GTSession object so that you can easily
		visit the GTSession from the GTStock object.
	*/
	GTSession	&m_session;

	using		GTStock32::m_szStock;

public:
	//! \brief System Time
	/*!
		\remark A pointer to internel system time. This time is updated once a second. 
		If the API is running with real quotes, the time is the same as local computer time. 
		Or it is the same as history quotes time.
	*/
	const GTime0		*m_pSysTime;

	//! \brief System Date
	/*!
		\remark A pointer to internel system date. 
		If the API is running with real quotes, the date is the same as local computer date. 
		Or it is the same as history quotes date.
	*/
	const GDate0		*m_pSysDate;

public:
	//! \brief The default order struct
	/*!
		\remark This is the default GTOrder struct that has been filled with the information of this
		GTStock object. Each time when sending an order, you should copy this struct to the
		GTOrder. After that, you can fill in the addition options like place, method and so on
		specified for the order.

		\par
		In the C version API, you can also call gtInitOrder() function to initialize the order
		struct. 
	*/
	GTOrder				m_defOrder;
	
	//! \brief Level 1
	/*!	\anchor m_level1
		\remark This keeps the newest level 1 information. It is realtimely updated by the API system. 
		In this struct, the NBBO(National Best Bid/Ask), exchange BBO(Best Bid/Ask) and 
		daily bar information are kept. You can also use this to retrieve the NBBO and daily bar
		information.

		\par
		But it is more convienent to use \ref m_level2 (the Level 2 list) data as the source of 
		Bid/Ask price. There are several reasons. First of all, in \ref m_level2, lists of bid/ask prices
		are kept, by looking into the lists, you can get more information about the prices, not only 
		the NBBO, but also other book prices; also there are methods that are easily called for 
		NBBO (GTLevel2Box::GetBestAskPrice() and GTLevel2Box::GetBestBidPrice()).

		\par
		Also, the level 2 data includes more information about the quote. If the client program
		needs to get information about the quote for one specified exchange/ECN, you have to 
		look into level 2 instead of level 1, since level 1 can give you only an overall 
		description of the inside quote. 
		
		\par
		Even so, in some circumstance, using level 1 is suggested. If you only want to read NBBO, 
		level 1 is the enough source for this job. Since Level 1 has a much more simple structure
		than level 2, it will be more fast to retrieve information from level 1.

		\par
		Refer to GTLevel1 for the struct details.
	*/
	GTLevel1			m_level1;

	//! \brief NOII
	/*!	\anchor m_noii
		\remark This keeps the newest NOII information. It is realtimely updated by the API system. 
		\par
		Refer to GTNoii for the struct details.
	*/

	GTNoii			m_noii;

	//! \brief NYXI
	/*!	\anchor m_nyxi
		\remark This keeps the newest NYSE Imbalance information. It is realtimely updated by the API system. 
		\par
		Refer to GTN_NYXI for the struct details.
	*/

	GT_NYXI			m_nyxi;

	//! \brief Collection of Level 2 data
	/*! \anchor m_level2
		\remark This keeps the all the level 2 record. It is updated by the API system in real time. 

		\par
		This keeps current bid list and ask list. Get level 2 record of the GTStock object by this
		field. You can find many straightforward method in the definition of the GTLevel2Box, which is 
		the type of this field.	Following code example shows one way to travel through the level 2 
		records:
		
		\code
			const int count = Stock.m_level2.GetBidCount();
			for(int i = 0; i < count; ++i)
			{
				const GTLevel2 *rcd = Stock.m_level2.GetBidItem(i);
				// put your staff to handle rcd here
			}
		\endcode
	
		
		Refer to GTLevel2Box for the struct details.
	*/
	GTLevel2Box			m_level2;
	
	//! \brief Colleciton of the Prints
	/*! \anchor m_prints
		\remark This keeps the all the print records. It is updated by the API system in real time. 
		Refer to GTPrints for the struct details.
	*/
	GTPrints			m_prints;

	//! \brief Colleciton of the Charts
	/*! \anchor m_charts
		\remark This keeps the all the chart records. It is updated by the API system in real time. 
		Refer to GTCharts for the struct details.
	*/
	GTCharts			m_charts;

	BOOL				m_bModified;

	double				m_dblCurrencyRate;

	//! \brief Map of Trade
	/*! \anchor m_trade
		\remark This keeps the all the prints record. Once a order gets filled, a trade record will be added 
		into this map. The Key of this map is a GTTradeKey32 struct which include ticketNo and matchNo. 

		\par
		When GTStock object generated, it will retrieve all the trades in this trading day and put
		into this map. So, even you start the program second time, this field will still keep all
		the trade happened in this trading day. 
		
		Refer to GTTrade32 for the struct details.
	*/
	CMapGTTrade			m_trade;

	//! \brief Map of Trade
	/*! \anchor m_pending
		\remark This keeps the all the pending order record. The key used here is the ticket number of the order.

		\par
		When GTStock object generated, it will retrieve all the pending orders and put into this map. So, 
		even you start the program second time, this field will still keep all the orders pending. 
		
		\par
		The orders in sending phase are those orders have sent out by the API but not got acknowledgement 
		from the executor. After executor gets the order information and checked for validation, it send
		an acknowledgement to the API, API will then move the order from sending phase into pending phase;
		in the meanwhile, the callback OnExecMsgSending() will be called. (Look into the lifetime of order
		graph for a picture.)

		\par
		It is useful to retrieve all the pending orders from time to time. Following codes do this thing:
		\code
			const CMapGTPending::CPair *pos = Stock.m_pending.PGetFirstAssoc();
			while(pos)
			{
				//handle pos->value as the GTPending32

				pos = Stock.m_pending.PGetNextAssoc(pos);
			}
		\endcode
		
		\par
		Refer to GTPending32 for the struct details.
	*/
	CMapGTPending		m_pending;

	//! \brief Map of Sending orders
	/*! \anchor m_sending
		\remark This keeps the all the sending order record. The key used here is the 
		TraderSeqNo of the order.

		\par
		When GTStock object generated, it will retrieve all the sending orders and put into this map. So, 
		even you start the program second time, this field will still keep all the orders in sending phase.
		
		\par
		In this map we don't use ticket number as the key, instead, we use traderSeqNo. This is reasonable
		since after API send out the order and before the executor get the order, API will never know
		what the ticket number is. Refer to \ref m_pending. 

		\par
		Refer to GTPending32 for the struct details.
	*/
	CMapGTSending		m_sending;

	//! \brief Map of cancelled orders
	/*! \anchor m_cancel
		\remark This keeps the all the cancelled order record in this execution period. The key used here is the ticket number of the order.

		\par
		To simplify the process, our system will NOT retrieve all the cancelled orders when the GTStock 
		generating. Instead, it simply keeps the cancelled order AFTER the GTStock generated.

		\par
		Refer to GTPending32 for the struct details.
	*/
	CMapGTCancel		m_cancel;

	//! \brief Map of Rejected orders
	/*! \anchor m_reject
		\remark This keeps the all the rejected order record in this execution period. The key used here is the 
		ticket number of the order.

		\par
		To simplify the process, our system will \b NOT retrieve all the rejected orders when the GTStock 
		generating. Instead, it simply keeps the rejected order \b AFTER the GTStock generated.

		\par
		Refer to GTReject32 for the struct details.
	*/
	CMapGTReject		m_reject;

//	//! \brief Openposition Information
//	/*! \anchor m_open
//		\remark This keeps the open position information of the account. 
//
//		\par
//		Most of time you don't need to visit this field by yourself. These's a method called GetOpenPosition()
//		in GTStock which is provided to give you a convenient way to retrieve the open position information.
//
//		\par
//		Refer to GTOpenPosition32 for the struct details.
//	*/
	CMapGTOpen			m_opens;


	//! \brief Map of Rejected orders
	/*! \anchor m_staying
		\remark This keeps the all the active order record in this execution period. The key used here is the 
		TraderSeqNo of the order.

		\par
		The active orders are those in sending phase and pending phase. So, this is helpful when you want to
		manage the orders by some parameters such as its lifetime. A example usage of this map is, if you want
		to cancel all the orders have lived for 2 or more munites. To do so, traval through this and check for
		the time passed for each order, cancel those have life greater or equal 2 munites.

		\par
		Refer to GTReject32 for the struct details.
	*/
	CMapSeqNoToStaying	m_staying;

public:
	//! \brief Constructor.
	/*!
		\param [in] session an reference to the GTSession that generate this GTStock object.
		\param [in] pszStock a pointer to the the symbol name of the stock.
		
		\remark Most of the time, DON'T new a GTStock object by yourself. Call GTSession::CreateStock()
		to generate a stock.
	*/
	GTStock(GTSession &session, LPCSTR pszStock);

	//! \brief Destructor.
	/*!
		\remark Don't ever call this by yourself. It is a destructor.
	*/
	virtual ~GTStock();

	//! \brief Reset the stock.
	/*!
		\remark DON'T call this by yourself. 

		\par
		This will remove all the information in the GTStock object. The contents in \ref m_trade, 
		\ref m_pending, \ref m_sending, \ref m_cancel, \ref m_reject, \ref m_staying, 
		\ref m_level1, \ref m_level2, \ref m_prints, \ref m_charts are all cleared.

	*/
	virtual void ResetContent();

	//! \brief Dump the information of this GTStock into a file.
	/*!
		\param fp [in] The FILE pointer that the information will dump into.
		\param nLevel [in] How many information will be dumped. Not used so far.
		
		\remark [Internal used] Call this to dump the information of the GTStock. The contents in \ref m_trade, 
		\ref m_pending, \ref m_sending, \ref m_cancel, \ref m_reject, \ref m_staying, 
		\ref m_level1, \ref m_level2, \ref m_prints, \ref m_charts are all dumped.
	*/
	virtual int Dump(FILE *fp, int nLevel) const;

public:
	//! \brief Get the GTSession object
	/*!
		\return the reference to the GTSession object.

		\remark Simply return m_session, the GTSession object who generated this GTStock object.
	*/
	GTSession &GetSession() const
		{ return m_session; }

	//! \brief Get the stock symbol name
	/*!
		\return the string contains the name of the symbol.

		\remark Simply return m_szStock.
	*/
	LPCSTR GetSymbolName() const
		{ return m_szStock; }


/** @name API Status Callbacks
	Callback functions for the status of the GTStock object. The derived classes can implement one or more 
	these callbacks by defining your way of handling the information that just comes. All this callbacks
	are related to the status changes of the GTStock object, such as changes of level1, level2, chart, print
	and so on.

	Be awared that, in our system, the information will always trigger some event in the API and then the
	corresponding callback will be called. For the realtime trading system, it is necessary to implement
	in your strategy how to handle the specified event.
*/
//@{
protected:
	//! \brief Callback for new level 1 data.
	/*!
		\param [in] pRcd an pointer to the new coming GTLevel1 data.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.
		
        \remark This callback is called when a new Level 1 data comes in. Implement this callback function with the
		action that will be taken when new level 1 data comes. Make sure you call the GTStock::OnGotQuoteLevel1()
		explicitly in client program, so that the internal status of m_level1 can be changed accordingly.
	*/
	virtual int OnGotQuoteLevel1(GTLevel1 *pRcd);

	//! \brief Callback for new NOII data.
	/*!
		\param [in] pRcd an pointer to the new coming GTNoii data.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.
		
        \remark This callback is called when a new NOII data comes in. Implement this callback function with the
		action that will be taken when new NOII data comes. Make sure you call the GTStock::OnGotQuoteNoii()
		explicitly in client program, so that the internal status of m_noii can be changed accordingly.
	*/
	virtual int OnGotQuoteNoii(GTNoii *pRcd);

	//! \brief Callback for new NYSE Imbalance data.
	/*!
		\param [in] pRcd an pointer to the new coming GT_NYXI data.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.
		
        \remark This callback is called when a new NYSE Imbalance data comes in. Implement this callback function with the
		action that will be taken when new data arriuves. Make sure you call the GTStock::OnGotQuoteNYXI()
		explicitly in client program, so that the internal status of m_nyxi can be changed accordingly.
	*/
	virtual int OnGotQuoteNYXI(GT_NYXI * pRcd);

	//! \brief Callback for new NYSEALERT data.
	/*!
		\param [in] pRcd an pointer to the new coming GTNyseAlert data.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.
		
        \remark This callback is called when a new NYSEALERT data comes in. Implement this callback function with the
		action that will be taken when new NYSEALERT data comes. Make sure you call the GTStock::OnGotQuoteNyseAlert()
		explicitly in client program, so that the internal status of m_noii can be changed accordingly.
	*/
	virtual int OnGotQuoteNyseAlert(GTNyseAlert *pRcd);

protected:
	//! \brief Callback for new coming print.
	/*!
		\param [in] pRcd an pointer to the new coming GTPrint data.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

        \remark This callback is called when a new GTPrint data comes in. Implement this callback function with the
		action that will be taken when new print data comes. Make sure you call the GTStock::OnGotQuotePrint()
		explicitly, so that the print information can be added into \ref m_prints.
	*/
	virtual int OnGotQuotePrint(GTPrint *pRcd);

	//! \brief Callback for retrieving the old print data (250 records will be transferred).
	/*!
		\param [in] pRcd an pointer to the new coming GTPrint data.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.
		
		\remark Reserved for future version.
	*/
	virtual int OnGotQuotePrintHistory(GTPrint *pRcd);

	//! \brief Callback when Server ready to transfer historical print data.
	/*!
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark Reserved for future version.
	*/
	virtual int OnGotQuotePrintRefresh();

	//! \brief Callback when server has finished transfering historical print data.
	/*!
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark Reserved for future version.
	*/
	virtual int OnGotQuotePrintDisplay();

	//! \brief Callback for new print color comes.
	/*!
		\param [in] print an reference to GTPrint object.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark In the GTPrint there's a field called rgbColor which indicates the color of
		this print. If it was executed on the bid price, a default green color is
		filled, otherwise in the ask price, the default red color is filled. Use this
		callback to provide your loved color.

		\par
		This callback will be called when a new print comes. If client program need not
		render the print price, ignore this callback for avoiding more tedious coding.
	*/
	virtual int OnGetPrintColor(GTPrint &print);

protected:
	//! \brief Callback for new level2 data comes.
	/*!
		\param [in] pRcd the new level2 data that coming in.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark When new level2 data comes in, this is called. Extremely helpful to make a realtime
		trading system.	
		
		\par
		Make sure you call the GTStock::OnGotLevel2Record() explicitly, so that the internal
		level2 information can be updated.

	*/
	virtual int OnGotLevel2Record(GTLevel2 *pRcd);

	//! \brief Callback when getting notification of clearing all level2 data.
	/*!
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark In some situation, there's need for retransfering the level2 data. In this situation,
		the server will always send a Level2Refresh message first to notify the API to remove 
		all existing level2 data. After this, the server will send Level2 records to the API,
		the OnGotLevel2Record() callback thus will be called. After it finishes the transfering,
		the Level2Display message is send and API will call OnGotLevel2Display().

		\par
		Make sure you call the GTStock::OnGotLevel2Refresh() explicitly, so that the internal 
		level2 information can be cleared.
	*/
	virtual int OnGotLevel2Refresh();

	//! \brief Callback when the level2 data are fully updated.
	/*!
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark This will be called after the updating level2 is finished. Refer to OnGotLevel2Refresh()
		for more details. 

		\par
		Make sure you call the GTStock::OnGotLevel2Display() explicitly, so that the internal 
		level2 information can be updated.
	*/
	virtual int OnGotLevel2Display();

	//! \brief Callback for new print color comes.
	/*!
		\param [in] mmid The MMID of the target exchange/ECN.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark This callback will be called when server send a Level2Clear message. This can be happened
		when the level2 data of the specified MMID have become invalid. For example, for some reason
		the METHOD_ISLD disconnected from the server, server will immediately send Level2Clear message
		to all the client to tell them the existed level2 data from METHOD_ISLD are invalid now. 

		\par 
		Make sure you call the GTStock::OnGotLevel2Clear() explicitly, so that the internal level2 
		information can be updated.
	*/
	virtual int OnGotLevel2Clear(MMID mmid);

protected:
	//! \brief Callback for new chart data comes.
	/*!
		\param [in] pRcd the new chart data that coming in.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark When new chart data comes in, this is called. 

		\par
		Make sure you call the GTStock::OnGotLevel2Record() explicitly, so that the internal
		level2 information can be updated.

	*/
	virtual int OnGotChartRecord(GTChart *pRcd);

	//! \brief Callback when getting old chart data from server.
	/*!
		\param [in] pRcd The record
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark In some situation, there's need for retransfering the chart data. In this situation,
		the server will always send a ChartRefresh message first to notify the API to remove 
		all existing chart data. After this, the server will send history chart records to the API,
		the OnGotChartRecordHistory() callback thus will be called. After it finishes the transfering,
		the ChartDisplay message is send and API will call OnGotChartRecordDisplay().

		\par
		Make sure you call the GTStock::OnGotChartRecordHistory() explicitly, so that the internal 
		chart information can be cleared.
	*/
	virtual int OnGotChartRecordHistory(GTChart *pRcd);

	//! \brief Callback when getting old chart data from server.
	/*!
		\param [in] pRcd The record
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark Refer to OnGotChartRecordHistory(). The parameter used here so far is not in use since only
		minute chart is implemented in this version of API.

		\par
		Make sure you call the GTStock::OnGotChartRecordHistory() explicitly, so that the internal 
		chart information can be cleared.
	*/
	virtual int OnGotChartRecordRefresh(GTChart *pRcd);

	//! \brief Callback when getting old chart data from server.
	/*!
		\param [in] pRcd The record
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark Refer to OnGotChartRecordHistory(). The parameter used here so far is not in use since only
		minute chart is implemented in this version of API.

		\par
		Make sure you call the GTStock::OnGotChartRecordHistory() explicitly, so that the internal 
		chart information can be cleared.
	*/
	virtual int OnGotChartRecordDisplay(GTChart *pRcd);

public:
	//! \brief Callback for Best Bid price changed
	/*!
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark This will be called when the best bid price is changed.
	*/
	virtual int OnBestBidPriceChanged();

	//! \brief Callback for Best Ask price changed
	/*!
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark This will be called when the best ask price is changed.
	*/
	virtual int OnBestAskPriceChanged();

//@}

public:
	//! \brief Initialize the GTStock object.
	/*!
		\remark This is called to initialize the GTStock object. Client program need not call it by yourself. 
		The API system will call it for you instead.
	*/
	virtual int Init();

	//! \brief Clean the GTStock object.
	/*!
		\remark This is called to do housekeeping for the GTStock object. Client program need not call it by yourself. 
		The API system will call it for you instead.
	*/
	virtual int Exit();

	//! \brief Try to close the GTStock object.
	/*!
		\remark This is called when you call GTSession::TryClose(). Client program need not call it by yourself. 
		The API system will call it for you instead.

		\see GTSession::TryClose()
	*/
	virtual int TryClose();

	//! \brief See if you can safely close the GTStock object.
	/*!
		\remark This is called when you call GTSession::CanClose(). Client program need not call it by yourself. 
		The API system will call it for you instead.

		\see GTSession::CanClose()
	*/
	virtual BOOL CanClose() const;

public:
	//! \brief Send request command to the server to send quote
	/*!
		\remark This is called to request server starting sending quote. Client program need not call it by yourself. 
		The API system will call it for you instead. Default setting is sending.
	*/
	int RequestQuote();

	//! \brief Send request command to the server not to send quote
	/*!
		\remark This is called to request server starting sending quote. Client program need not call it by yourself. 
		The API system will call it for you instead. Default setting is sending.
	*/
	int CancelQuote();

	//! \brief Send request command to the server to send Level2
	/*!
		\remark This is called to request server starting sending quote. Client program need not call it by yourself. 
		The API system will call it for you instead. Default setting is sending.
	*/
	int RequestLevel2();

	//! \brief Send request command to the server not to send Level2
	/*!
		\remark This is called to request server starting sending quote. Client program need not call it by yourself. 
		The API system will call it for you instead. Default setting is sending.
	*/
	int CancelLevel2();

	//! \brief Send request command to the server to send Chart
	/*!
		\remark This is called to request server starting sending quote. Client program need not call it by yourself. 
		The API system will call it for you instead. Default setting is sending.
	*/
	int RequestChart();

	//! \brief Send request command to the server not to send Chart
	/*!
		\remark This is called to request server starting sending quote. Client program need not call it by yourself. 
		The API system will call it for you instead. Default setting is sending.
	*/
	int CancelChart();

	//! \brief Get status of level1 data sending
	/*!
		\remark Usually need not to call it
	*/
	virtual BOOL WantLevel1() const;
	//! \brief Get status of level2 data sending
	/*!
		\remark Usually need not to call it
	*/
	virtual BOOL WantNOII() const;
	//! \brief Get status of NOII data sending
	/*!
		\remark Usually need not to call it
	*/
	virtual BOOL WantLevel2() const;
	//! \brief Get status of print data sending
	/*!
		\remark Usually need not to call it
	*/
	virtual BOOL WantPrint() const;
	//! \brief Get status of chart data sending
	/*!
		\remark Usually need not to call it
	*/
	virtual BOOL WantChart() const;
	//! \brief Get status of chart data construction from print
	/*!
		\remark Usually need not to call it
	*/
	virtual BOOL WantChartFromPrint() const;

public:
	//! \brief Print header
	/*!
		\remark Used only internally. for debug only.
	*/
	static int PrintTradesHeader(FILE *fp);

	//! \brief Print header
	/*!
		\remark Used only internally. for debug only.
	*/
	int PrintTrades(FILE *fp) const;


/** @name Buy/Sell/Cancel/OpenPosition Actions

	These methods are related to the trade actions. 

	There're two kinds of placing order methods, while they take the same name but different parameters. The ones
	with an GTOrder in their parameter list, and defined as const member functions, will just fill out the GTOrder 
	struct; others without GTOrder in their parameter list will do the fillout and actual sending. 

	The typical usage is:
	\code
		// typical most useful method to place order. You have full control on the order.
		GTStock &Stock;
		GTOrderList	m_orders;
		GTOrder order = Stock.m_defOrder;
		MMID method = METHOD_BELZ;
		MMID place = MMID_NYSE;

		if(Stock.PlaceOrder(order, 'B', 100, CUT1000(dblPrice), method, TIF_DAY) == 0){
			order.lpUserData = lpUserData;

			// if you want to specify the place
			order.place = place;			

			// if you want market order instead of default limit order
			order.chPriceIndicator = '1';	
		}

		Stock.PlaceOrder(m_orders);
	\endcode
	
	<b>Some special order</b>
	- To send reserve order, you can fill out the GTOrder and set order.dwMaxFloor to be the max size to display.
	- To send a hidden order to ISLD, fill out the GTOrder and then set order.chDisplay to be 'N'.

	\anchor autoroute
	\b Autoroute

	Some people want to use autoroute provided by some ECN. The smart route is provided by the ECN not by Genesis. 
	Several ECN provides smart route, like ISLD, BRUT and so on. So you should send order to those ECN to have them smart 
	route for your order. They will choose the right place to route your order to. The point of smart route is to make the 
	best execution of you order. If not, your order reached the ECN will stay there. But if you set smart route option on, 
	your order will continue travelling to the place with best price for you. Say, now the BRUT bid is 10.01 and ISLD bid 
	is 10.00. You send order to ask 10.00 to ISLD. If smart route is on, your order will go to BRUT to get filled with 
	price 10.01. To utilize smart route functionality, what you should do is to set the AutoRoute option (either bIsldAutoRoute, 
	bBrutAutoRoute and so on) to be true, and properly set the order.method. Some ECN also request the order.place to be filled. 
	As following code:
	\code
		//I am using ISLD autoroute. So method should be METHOD_ISLD
		if(Stock.PlaceOrder(order, 'B', 100, dblPrice, METHOD_ISLD, TIF_DAY) == 0) {	
			order.lpUserData = lpUserData;
			order.bIsldAutoRoute = TRUE;	//Set autoroute
			Stock.PlaceOrder(order);
		}
	\endcode

	\anchor discretionary
	<b>Discretionary order</b>

	For the discretionary orders, following code might be used:
	\code
		//I am using discretionary order. So method should be METHOD_SOES
		if(Stock.PlaceOrder(order, 'B', 100, dblPrice, METHOD_SOES, TIF_DAY) == 0) {	
			order.lpUserData = lpUserData;
			order.chDiscretionary='Y';
			order.nDiscrOffset = 5;
			Stock.PlaceOrder(order);
		}
		//this will send an order with a $0.05 discretion attached to the limit price
			
	\endcode
	Make sure only SOES now support discretionary orders. The nDiscrOffset can only take value
	in the range [ 1 - 99 ], so the maximum offset can be is $0.99. It can not take a negative value.
	So, if the nDiscrOffset is 10, for a bid on a stock bid at $19.00, the possible price to execute 
	is between 19.00 and 19.10; for a ask on a stock ask at 19.01, the possible execution price is in 
	range 18.91 ~ 19.01.

	\anchor oddorder
	<b>One note on the odd orders</b>
	
	Due to the possible cancellation of some exchange for odd share orders, we have settings to redirect the 
	order with odd shares. In the GTSetting32 structure, you can find two fields:
	\code
			BOOL m_bAutoRedirect100;
			MMID m_mmidAutoRedirect100;
	\endcode
	The settings read:
		- If m_bAutoRedirect100 is true, the odd share order will be redirect to the specified place. The place is 
		defined in m_mmidAutoRedirect100.
		- If m_bAutoRedirect100 is false, this feature will be disabled and your order will 
		go to the place you specified in the order.

	So you can write in your code to enable/disable this feature and specify which place to execute the odd orders.
 
	\anchor abcd
	<b>(NEWLY ADDED)Market on open, Market on close, Limit on open, Limit on close orders</b>

	Those orders are now available for BELZ and MLYN. To send a Market_on_close order, you can:
	\code
		if(Stock.PlaceOrder(order, 'B', 100, dblPrice, METHOD_BELZ, 0) == 0) {	
			order.lpUserData = lpUserData;
			order.place = MMID_NYSE;
			order.chPriceIndicator = 'B';
			Stock.PlaceOrder(order);
		}
	\endcode

	\anchor distinction
	<b>Distinction of Bid()/Ask(), Buy()/Sell() and BuyDirect()/SellDirect():</b>
	
	All the Buy/Sell, Bid/Ask, BuyDirect/SellDirect method will send the order as limit order by default.
	If you want to send other order type, manually set the GTOrder.chPriceIndicator.

	The only different thing between buy and bid or sell and ask, namely, is the TIF. For buy/sell, the tif is TIF_IOC (0); while for
	bid/ask, the tif is TIF_DAY(99999). So, for the buy/sell, the order will be a IOC limit order. For bid/ask, the limit order
	will be a day order. IOC means immediately or cancel.

	The BuyDirect/SellDirect are also buy/sell (IOC limit order). But API will choose the method to send the order, even when you have 
	set the method of the order. Also, the price you given will be a reference price. In a word, this two will try to make the order be 
	executed as a market order, by providing the right method and right price. API chooses the method by this way: If BuyDirect is used, 
	it checks the Ask level2 data; vice versa. The first level2 records with connected method will be choosed. So, when buydirect, the 
	lowest ask record is first checked: if the record is from isld, and now method ISLD is usable, then the buydirect will try to hit 
	ISLD with the price from the record. Otherwise, the next level2 record is checked, and so on.
*/

//@{
public:
	//! \brief Get open position information
	/*!
		\param [in] chOpenSide the open side will be set after calling this method
		\param [in] nOpenPosition how many long/short position
		\param [in] dblOpenCost the average cost over the position at hand
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark Call this to retrieve your positions of this symbol. The paramters inputed will be filled out with the
		information. This is helpful for you to judge your position and make decision to take further trading
		actions.
	*/
	int GetOpenPosition(char &chOpenSide, int &nOpenPosition, double &dblOpenCost) const;

protected:
	//! \brief Post Process After Sending Out Order
	/*!
		\param [in] order The order will be sent out.
		\return 0 for success, otherwise not zero. This will affect the return value of PlaceOrder(GTOrder), but
		the order will have been sent. If this returns value other than 0, the PlaceOrder(GTOrder) will return -1.
	
		\remark This is the place to put your stuff that handles post sending event. After each time an order is sent out, 
		this function is called.
	*/
	virtual int PostPlaceOrder(GTOrder &order);

	//! \brief Pre Process Before Sending Out Order.
	/*!
		\param [in] order The order will be sent out.
		\return 0 for success, otherwise not zero. If the return vaule is any value other than 0, the PlaceOrder(GTOrder) 
		will stop to send this order and immediately return -1.

		\remark This is the place to put your stuff that handles pre sending event. Before each time an order is sent out, 
		this function is called. 

		\par
		Sometimes client may want to put some order checking codes in this function.
	*/
	virtual int PrePlaceOrder(GTOrder &order);

public:
	//! \brief Send order to executor
	/*!
		\param [in] order The order will be sent out.
		\return 0 for success, otherwise not zero.

		\remark Do the actual sending out.
	*/
	int PlaceOrder(GTOrder &order);

	//! \brief Send order to executor
	/*!
		\param [in] chSide 'B' for bid/buy, 'S' for Ask/Sell;
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order
		\param [in] method The method used to send out this order;
		\param [in] dwTIF Time in force. Some exchange/ECN support this, which will automatically cancel the order when the order have a life greater than TIF;
		\param [in] bDisplay Will this order displayed in the book?
		\return 0 for success, otherwise not zero.

		\remark This will fill the order with the parameters as provided and others with default value, then send order out through 'PlaceOrder(GTOrder &order);
	*/
	int PlaceOrder(char chSide, long dwShares, double dblPrice, MMID method, long dwTIF, int bDisplay = -1);

	//! \brief Fill out an order
	/*!
		\param [in] order The GTOrder struct needed to be filled;
		\param [in] chSide 'B' for bid/buy, 'S' for Ask/Sell;
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order
		\param [in] method The method used to send out this order;
		\param [in] dwTIF Time in force. Some exchange/ECN support this, which will automatically cancel the order when the order have a life greater than TIF;
		\param [in] bDisplay Will this order displayed in the book?
		\return 0 for success, otherwise not zero.

		\remark Similiar to 'MakeOrder', just fill up an 'order'.
	*/
	int PlaceOrder(GTOrder &order, char chSide, long dwShares, double dblPrice, MMID method, long dwTIF, int bDisplay = -1) const;

	//! \brief Send a buy order to executor
	/*!
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order
		\param [in] method The method used to send out this order;
		\return 0 for success, otherwise not zero.

		\remark Simply call PlaceOrder(char chSide, long dwShares, ...)

		\par
		\ref distinction "See also here for what is buy."
	*/
	int Buy(long dwShares, double dblPrice, MMID method);

	//! \brief Fill out an buy order
	/*!
		\param [in] order The GTOrder struct needed to be filled;
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order;
		\param [in] method The method used to send out this order;
		\return 0 for success, otherwise not zero.

		\remark Fill up a 'Buy' order by calling 'PlaceOrder(GTOrder &order, char chSide, ...)

		\par
		\ref distinction "See also here for what is buy."
	*/
	int Buy(GTOrder &order, long dwShares, double dblPrice, MMID method) const;

	//! \brief Send a sell order to executor
	/*!
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order
		\param [in] method The method used to send out this order;
		\return 0 for success, otherwise not zero.

		\remark Simply call 'PlaceOrder(char chSide, long dwShares, ...)

		\par
		\ref distinction "See also here for what is sell."
	*/
	int Sell(long dwShares, double dblPrice, MMID method);

	//! \brief Fill out an sell order
	/*!
		\param [in]  order The GTOrder struct needed to be filled;
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order
		\param [in] method The method used to send out this order;
		\return 0 for success, otherwise not zero. 

		\remark Fill up a 'sell' order by calling 'PlaceOrder(GTOrder &order, char chSide, ...)

		\par
		\ref distinction "See also here for what is sell."
	*/
	int Sell(GTOrder &order, long dwShares, double dblPrice, MMID method) const;

	//! \brief Send a bid order to executor
	/*!
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order
		\param [in] method The method used to send out this order;
		\param [in] bDisplay Will this order displayed in the book?
		\return 0 for success, otherwise not zero. 

		\remark Simply call PlaceOrder(char chSide, long dwShares, ...)

		\par
		\ref distinction "See also here for what is bid."
	*/
	int Bid(long dwShares, double dblPrice, MMID method, int bDisplay = -1);

	//! \brief Fill out an bid order
	/*!
		\param [in] order The GTOrder struct needed to be filled;
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order
		\param [in] method The method used to send out this order;
		\param [in] bDisplay Will this order displayed in the book?
		\return 0 for success, otherwise not zero. 

		\remark Fill up a 'bid' order by calling 'PlaceOrder(GTOrder &order, char chSide, ...)

		\par
		\ref distinction "See also here for what is bid."
	*/
	int Bid(GTOrder &order, long dwShares, double dblPrice, MMID method, int bDisplay = -1) const;

	//! \brief Send a ask order to executor
	/*!
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order
		\param [in] method The method used to send out this order;
		\param [in] bDisplay Will this order displayed in the book?
		\return 0 for success, otherwise not zero. 

		\remark Call 'PlaceOrder(char chSide, long dwShares, ...)

		\par
		\ref distinction "See also here for what is ask."
	*/
	int Ask(long dwShares, double dblPrice, MMID method, int bDisplay = -1);

	//! \brief Fill out an ask order
	/*!
		\param [in] order The GTOrder struct needed to be filled;
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order
		\param [in] method The method used to send out this order;
		\param [in] bDisplay Will this order displayed in the book?
		\return 0 for success, otherwise not zero.

		\remark Fill up a 'bid' order by calling 'PlaceOrder(GTOrder &order, char chSide, ...)

		\par
		\ref distinction "See also here for what is ask."
	*/
	int Ask(GTOrder &order, long dwShares, double dblPrice, MMID method, int bDisplay = -1) const;

	//! \brief Send a direct buy order to executor
	/*!
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order
		\return 0 for success, otherwise not zero. 

		\remark Call 'PlaceOrder(char chSide, long dwShares, ...)

		\par
		\ref distinction "See also here for what is buydirect."
	*/
	int BuyDirect(long dwShares, double dblPrice);

	//! \brief Fill out an direct buy order
	/*!
		\param [in] order The GTOrder struct needed to be filled;
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order
		\return 0 for success, otherwise not zero.

		\remark Fill up a 'bid' order by calling 'PlaceOrder(GTOrder &order, char chSide, ...)

		\par
		\ref distinction "See also here for what is buydirect."
	*/
	int BuyDirect(GTOrder &order, long dwShares, double dblPrice) const;

	//! \brief Send a direct sell order to executor
	/*!
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order
		\return 0 for success, otherwise not zero. 

		\remark Call 'PlaceOrder(char chSide, long dwShares, ...)

		\par
		\ref distinction "See also here for what is selldirect."
	*/
	int SellDirect(long dwShares, double dblPrice);

	//! \brief Fill out an direct sell order
	/*!
		\param [in] order The GTOrder struct needed to be filled;
		\param [in] dwShares Number of shares in this order;
		\param [in] dblPrice The price per share of this order
		\return 0 for success, otherwise not zero. 

		\remark Fill up a 'bid' order by calling 'PlaceOrder(GTOrder &order, char chSide, ...)

		\par
		\ref distinction "See also here for what is selldirect."
	*/
	int SellDirect(GTOrder &order, long dwShares, double dblPrice) const;

	//! \brief Send out orders in the GTOrderList, companion used with above order filling methods.
	/*!
		\param [in] orders The list of the orders
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark This is used to send many order out. All the orders should be put into GTOrderList container. 
	*/
	int SendOutOrders(const GTOrderList &orders);

protected:
	//! \brief Caculate direct buy level.
	/*!
		\param [in] order The order struct need to be filled;
		\param [in] nBegin starting level
		\param [in] nEnd ending level
		\param [in] dwShares Shares to be bought
		\param [in] dblPrice price in the buy
		\return 0 for success, otherwise not zero. 
	
		\remark Internal used method, which is called in BuyDirect to find the right place to buy.
	*/
	int BuyDirectLevel(GTOrder &order, int nBegin, int nEnd, long dwShares, double dblPrice) const;

	//! \brief Caculate direct Sell level.
	/*!
		\param [in] order	The order struct need to be filled;
		\param [in] nBegin  starting level
		\param [in] nEnd	ending level
		\param [in] dwShares Shares to be bought
		\param [in] dblPrice price in the buy
		\return 0 for success, otherwise not zero. 

		\remark Internal used method, which is called in BuyDirect to find the right place to sell.
	*/
	int SellDirectLevel(GTOrder &order, int nBegin, int nEnd, long dwShares, double dblPrice) const;

public:
	//! \brief Cancel all existed orders.
	/*!
		\return 0 for success, otherwise not zero. 

		\remark This will cancel all orders that in Pending phase.
	*/
	int CancelAllOrders();

	//! \brief Cancel all existed orders with specified side ('B' or 'S').
	/*!
		\param [in] chSide  The side to be cancelled.
		\return 0 for success, otherwise not zero. 

		\remark This will cancel all orders that in Pending phase with specified side ('B' or 'S').
	*/
	int CancelOrder(char chSide);

	//! \brief Cancel all existed orders with specified method.
	/*!
		\param [in] mmid  The method of the orders that will be cancelled
		\return 0 for success, otherwise not zero. 

		\remark This will cancel all orders that in Pending phase with specified method.
	*/
	int CancelOrder(MMID mmid);

	//! \brief Cancel specified order
	/*!
		\param [in] pending  The GTPending that describes the pending order.
		\return 0 for success, otherwise not zero. 

		\remark This will cancel the order that specified.
	*/
	int CancelOrder(const GTPending &pending);
//@}

protected:
	//! \brief Timer callback
	/*!
		\return 0 for success, Otherwise nonzero. The return value will not effect the status of the API system.

		\remark This is called by the GTSession timer callback. 

		\par
		GTSession has a internal timer that will be alive each second. It will call all the GTStock's OnTick(). Some 
		strategies can implement this to play the trades as each second basis. 
	*/
	virtual int OnTick();

	//! \brief Callback for command recieved
	/*!
		\return 0 for success, Otherwise nonzero. The return value will not effect the status of the API system.

		\remark This is not related to the trade or executor. It is useless in almost all the time.
	*/
	virtual int OnExecCmd(LPCSTR pszCmd);

/** @name Order Status and Executor Command Callbacks
	Callback functions for the order status changing or executor messages. The derived classes can implement one 
	or more these callbacks by defining your way of handling the information that just comes.

	Be awared that, in our system, the status change of an order will always trigger some event in the API 
	and then the corresponding callback will be called. For the realtime trading system, it is necessary 
	to implement in your strategy how to handle the specified event.

	Following picture might be helpful:
	\htmlonly
	<IMG SRC="image001.gif">
	\endhtmlonly
	\latexonly
		\includegraphics[viewport=220 -100 660 700, scale=0.80]{image001.eps}
	\endlatexonly

	Some notes for the picture above:
	- *: For filled situation, our executor will generate two messages: the filled shares in the Trade message and 
	the left pending shares in a Pending message. The newer Pending message have the same information with the old 
	one except that the nPendShares field is different. If it is fully filled, the left share number is 0, this 
	tells the API to delete the pending record; if it partially filled, the left share number is not 0, which tells 
	the API to update the Pending records.

	- **: There is another message Reject that I have not shown in the diagram. 

	For the implement of your derived stock class, make sure you call the base class callback explicitly, as an example,

	\code
		class MyStock : public GTStock{...}

		int MyStock::OnSendingOrder(const GTSending &gtsending)
		{
			//Do something you want

			return GTStock::OnSendingOrder(gtsending);
		}
	\endcode
*/

//@{
public:
	//! \brief Callback when Order is ready to be sent.
	/*!
		\param [in] gtsending  The GTSending struct that will be sent out.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark When you call PlaceOrder() to send an order, this is first called before the order is actually sent out. 
	
		\note Make sure you call the GTStock::OnSendingOrder() explicitly, so that the API default action will be taken.
	*/
	virtual int OnSendingOrder(const GTSending &gtsending);

protected:
	//! \brief Callback when some error happened in the executor.
	/*!
		\param [in] errmsg  The error message struct.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark Once executor issues an error message, API will call this callback. The errorcode and reason text can be 
		found in the errmsg.

		\note Make sure you call the GTStock::OnExecMsgErrMsg() explicitly, so that the API default action will be taken.
	*/
	virtual int OnExecMsgErrMsg(const GTErrMsg &errmsg);

	//! \brief Callback when API gets open position information from executor.
	/*!
		\param [in] open  The GTOpenPosition struct that describes your open position.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark When executor sends open position to the client, API call this.

		\par
		This will be called each time the openposition information changes. When a trade is done the openposition will
		change.

		\note Make sure you call the GTStock::OnExecMsgOpenPosition() explicitly, so that the API default action will be taken.
	*/
	virtual int OnExecMsgOpenPosition(const GTOpenPosition &open);

	//! \brief Callback when an order has just been filled.
	/*!
		\param [in] trade  The GTTrade struct that describes the trade.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark When an order gets filled, executor will tell client API to call this.

		\note Make sure you call the GTStock::OnExecMsgTrade() explicitly, so that the API default action will be taken.
	*/
	virtual int OnExecMsgTrade(const GTTrade &trade);

	//! \brief Callback when an order has become pending.
	/*!
		\param [in] pending  The GTPending struct that contains the pending order information.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark When the ECN/exchange accepts an order and notifies executor about it, executor will tell client API to
		call this callback.

		\note Make sure you call the GTStock::OnExecMsgPending() explicitly, so that the API default action will be taken.
	*/
	virtual int OnExecMsgPending(const GTPending &pending);

	//! \brief Callback when an order has become sent out to ECN/exchange.
	/*!
		\param [in] sending  The GTSending struct that contains the information about the order in sending.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark When an order is recieved by executor, it checks for the validation and then sends the order to the ECN/exchange,
		in the meanwhile, it tells API to call this callback.

		\note Make sure you call the GTStock::OnExecMsgSending() explicitly, so that the API default action will be taken.
	*/
	virtual int OnExecMsgSending(const GTSending &sending);

	//! \brief Callback when an cancel command has been sent out to ECN/exchange.
	/*!
		\param [in] cancel  The GTCancel struct that describes the information of the order that is requested to cancel.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark When an order is sending to be cancelled, this is called. The executor sends the cancel to the ECN/exchange,
		in the meanwhile, it notifies the client API and API will call this.

		\note Make sure you call the GTStock::OnExecMsgCanceling() explicitly, so that the API default action will be taken.
	*/
	virtual int OnExecMsgCanceling(const GTCancel &cancel);

	//! \brief Callback when an order has been cancelled
	/*!
		\param [in] cancel  The GTCancel struct that describes the order that has been cancelled.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark When an order is successfully cancelled, this is called. 

		\note Make sure you call the GTStock::OnExecMsgCancel() explicitly, so that the API default action will be taken.
	*/
	virtual int OnExecMsgCancel(const GTCancel &cancel);

	//! \brief Callback when order has been rejected by executor or ECN/exchange
	/*!
		\param [in] reject  The GTReject struct that describes the information of the rejection.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark When an order is rejected, this is triggered. You can find information about the reason.

		\note Make sure you call the GTStock::OnExecMsgReject() explicitly, so that the API default action will be taken.
	*/
	virtual int OnExecMsgReject(const GTReject &reject);

	//! \brief Callback when executor asks API to remove some invalid order.
	/*!
		\param [in] remove  The GTRemove struct that describes the information of the removal.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark In rare situation some pending orders are invalid in the system. Once executor finds them, this is triggered.

		\note Make sure you call the GTStock::OnExecMsgRemove() explicitly, so that the API default action will be taken.
	*/
	virtual int OnExecMsgRemove(const GTRemove &remove);

	//! \brief Callback when cancel command is rejected by ECN/exchange.
	/*!
		\param [in] rejectcancel  The GTRejectCancel struct that describes the information of the rejection.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\remark When the ECN/exchange rejects your cancellation of an order, this will be fired. The reason and order information
		is in the GTRejectCancel struct.

		\note Make sure you call the GTStock::OnExecMsgRejectCancel() explicitly, so that the API default action will be taken.
	*/
	virtual int OnExecMsgRejectCancel(const GTRejectCancel &rejectcancel);

	//! \brief Callback when an IOI status is sent from ECN/exchange.
	/*!
		\param [in] ioi The GTIOIStatus struct that describes the information of the rejection.
		\return 0 for success, otherwise not zero. The return value will not effect the status of the API system.

		\note Make sure you call the GTStock::OnExecMsgIOIStatus() explicitly, so that the API default action will be taken.
	*/
	virtual int OnExecMsgIOIStatus(const GTIOIStatus &ioi);
//@}
public:
//@{
/*! \brief	Used to calculate the profit/loss (Internal used by GTSession)
*/
	void CalcGrossNet();
	void CalcOpenPL();
	void CalcGrossNet(const GTTrade &trade);
	void CalcGrossNet(const GTCancel &cancel);
//@}
public:
//@{
/*! \brief	Internal used by GTSession
*/
	BOOL EnablePendingBook(BOOL bEnable);
	void RemovePending(DWORD dwTicketNo);
	void SetPending(const GTPending32 &pending);
//@}

public:
//! \brief Set user-defined callback functions.
/*!	
	\param [in] nID	function ID
	\param [in] pfn	function pointer
	\return 0 for success. If function ID is not found, -1 is returned.

	Internally used. This is used to set user homemade function as the callback function. 
*/
	int SetCallBack(int nID, FARPROC pfn);

public:
	static LPCSTR GetErrorMessage(int nReason, LPSTR pszMsg = NULL);
};

GTAPI_API double CalcOpenPL(const GTOpenPosition &open, const GTLevel1 &l1);

#endif // !defined(__GTSTOCK_H__)

/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

#ifndef __GTAPI32_H__
#define __GTAPI32_H__

/*! \file GTAPI32.h
    \brief GTAPI32 header file.
    
    This is GTAPI32 header file, used by external C version applications. All external C version applications 
	should include this header file.
*/

#include "GTSession32.h"
#include "GTStock32.h"

/*! \def GTAPI_VERSIONDEF
  \brief Computes GTAPI version from tiny integer, \a v, \a m, \a o and \a u.\n

  \remark
  \a v: Major version\n
  \a m: Minor version\n
  \a o: Update\n
  \a r: Release\n
*/
#define GTAPI_VERSIONDEF(v, m, u, r)	MAKELONG(MAKEWORD(r, u), MAKEWORD(m, v))
/*! \def GTAPI_VERSION
	\brief The version of GTAPI. The current version is 0.8.0.0.
*/
#define GTAPI_VERSION					GTAPI_VERSIONDEF(0, 8, 0, 0)		// 0.8.0.0

/*! \def GTAPI_CLASSNAME
	\brief GTSession class name in Windows. When GTSession object is created in a 
	Dialog box with custom control, the class name must be GTAPI_CLASSNAME.
*/
#define GTAPI_CLASSNAME					"GTSessionWindow"

#ifdef __cplusplus
extern "C"
{
#endif//__cplusplus

/*! \ingroup c*/
/**
 * \brief Initialize the API library.
 * \param [in] dwReqVer The version number. One can use the macro GTAPI_VERSIONDEF to make a version number.
 * \param [in] hInstance The program instance handler. Can be NULL.
 * \return 0 for success, otherwise -1.
 * \remark Call this to initialize the API library. This should be called before any other function in the API is called.
	
	\par
    The return value indicates the status of the API system. If it is not 0, the initialization is failed and the program should exit.

	\par
	After using the API, call gtUninitialize to uninitialize the API system.

	\sa gtUninitialize
 */
int WINAPI gtInitialize(DWORD dwReqVer, HINSTANCE hInstance);
/*! \ingroup c*/
/**
 * \brief Uninitialize the system.
 * \return 0 for success, otherwise -1.
 * \remark. Call this to uninitialize the API after using. It will clean up the system.
 */
int WINAPI gtUninitialize();
/*! \ingroup c*/
/**
 * \brief Get the error message for the error code.
 * \param [in] nCode The error code.
 * \param [in] pszMsg The optional buffer. Can be NULL.
 * \return The string that contains the error message.
 */
LPCSTR WINAPI gtGetErrorMessage(int nCode, LPSTR pszMsg);

/*! \ingroup c*/
/**
 * \brief Create the session window
 * \param [in] hParent Parent window handler. 
 * \param [in] nID The resource ID for the session window. Can be 0.
 * \return The pointer to the internal GTSession. 
 * \remark Use this to create the session window. The parameter \a nID is optional, which can be 0 if you don't want to specify. If you want to
	assign an ID for the created session window, make sure the ID you input will NOT duplicate with all existed controls. This distincts from 
	gtSubClassSessionWindow by that, this function created a NEW session window control, while the calling of gtSubClassSessionWindow will just
	attach the session window to the exist custom control. 

	\par
	After creation, the window will be located to the left-top corner by default. If one want to place it to some other place, one can using 
	the window position functions such as SetWindowPlacement to move it to the right place.

	\par
	The returned pointer can be treated as the handler of the session. Later when calling some functions, such as gtCreateStock(), this handler
	can be used as the first parameter. So, keep the return value safely.

	\par
	Call gtDestroySessionWindow to destory the session window when one want to close the session.

	\sa gtDestroySessionWindow
 */
LPGTSESSION WINAPI gtCreateSessionWindow(HWND hParent, UINT nID);
/*! \ingroup c*/
/**
 * \brief Destory the session created by gtCreateSessionWindow.
 * \param [in] hSession The handler of the session one want to destory.
 * \remark One should call this to destory the session if it is created by calling gtCreateSessionWindow. 
 * \sa gtCreateSessionWindow
 */
void WINAPI gtDestroySessionWindow(LPGTSESSION hSession);

/*! \ingroup c*/
/**
 * \brief Subclass the session window
 * \param [in] hParent Parent window handler. 
 * \param [in] nID The resource ID for the session window. Can be 0.
 * \return The pointer to the internal GTSession. 
 * \remark Use this to subclass the session window. The parameter \a nID is the resource ID of the target CustomControl ID. This is called when one 
	wants to attach	the session window to the specified custom control. This parameter should be filled with the existed CustomControl ID. To do
	this, one has to add the custom control to the resource, and the class parameter of the control should be GTSessionWindow.

	\par
	The returned pointer can be treated as the handler of the session. Later when calling some functions, such as gtCreateStock(), this handler
	can be used as the first parameter. So, keep the return value safely.

	\par
	Call gtUnsubClassSessionWindow to unsubclass the session window when one want to close the session.

	\sa gtUnsubClassSessionWindow
 */
LPGTSESSION WINAPI gtSubClassSessionWindow(HWND hParent, UINT nID);
/*! \ingroup c*/
/**
	\brief Unsubclass the session window.
	\param [in] hSession the session handler returned by gtSubClassSessionWindow.
	\remark This should be called when you want to close the session and the session is subclassed by gtSubClassSessionWindow.
	\sa gtSubClassSessionWindow
 */
void WINAPI gtUnsubClassSessionWindow(LPGTSESSION hSession);

/*! \ingroup c*/
/**
 * \brief Get the stock handler from the API system.
 * \param [in] hSession The session handler.
 * \param [in] pszStock The stock name.
 * \return The stock handler. If the symbol has not been created, a NULL will be returned.
 * \remark Call this to get the handler for some symbol. The stock should be created by calling gtCreateStock. 
 * \sa gtCreateStock
 */
LPGTSTOCK WINAPI gtGetStock(LPGTSESSION hSession, LPCSTR pszStock);
/*! \ingroup c*/
/**
 * \brief Create a stock inside the session.
 * \param [in] hSession The session handler.
 * \param [in] pszStock The stock name.
 * \return The handler for the stock that just created.
 * \remark Call this to create the stock for the symbol wanted. Only after calling this, one can get the quote, level2 and chart data for the symbol.
	Also after that one can bid/ask/buy/sell the stock. This will register the symbol in the system for data recieving, and initialize the internal 
	data needed for trading the symbol. 
 * \par
	If the stock exists already when calling this, this equals to the calling of gtGetStock.
 */
LPGTSTOCK WINAPI gtCreateStock(LPGTSESSION hSession, LPCSTR pszStock);
/*! \ingroup c*/
/**
 * \brief Delete a stock from the session.
 * \param [in] hSession The session handler.
 * \param [in] pszStock The stock name.
 * \remark Delete a stock from the session.
 */
void WINAPI gtDeleteStock(LPGTSESSION hSession, LPCSTR pszStock);

/*! \ingroup c*/
/**
 * \brief Get the GTSession32 struct that describes the session.
 * \param [in] hSession The session handler.
 * \return The pointer to the GTSession32 structure.
 */
GTSession32 *WINAPI gtGetSession32(LPGTSESSION hSession);
/*! \ingroup c*/
/**
 * \brief Get the GTStock32 struct that describes the stock.
 * \param [in] hStock The stock handler.
 * \return A pointer to the GTStock32 structure.
 */
GTStock32 *WINAPI gtGetStock32(LPGTSTOCK hStock);
/*! \ingroup c*/
/**
 * \brief [Depreciated] Get the GTLevel2Box32 for a stock. 
 * \param [in] hLevel2Box The handler for the level2Box, which can be got from the GTStock32.
 * \return The pointer to GTLevel2Box32 structure.
 * \remark For who wants to get the level2 information, one can alternatively use gtGetBidLevel2Count and gtGetBidLevel2Item (or gtGetAskLevel2Count and gtGetAskLevel2Item)
 * to get the level 2 items. Using this and gtGetLevel2List32, gtGetLevel2Item gives one another way to retrieve all the level2 records. See following example.
 * \par
 * This is used to get the level 2 information. To use this, following code might be applied:
	\code
		GTStock32 *pStock;
		GTLevel2Box32 *pLevel2Box;
		const GTLevel232 *item;
		int n, i;

		pStock = gtGetStock32(hStock);
		pLevel2Box = gtGetLevel2Box32(pStock->m_pLevel2);
		i = 0;
		item = gtGetLevel2Item(pLevel2Box->m_bid32, i++);
		while(item){    //If item==NULL the while loop stops.
						//do something to item
			item = gtGetLevel2Item(pLevel2Box->m_bid32, i++);
		}
	\endcode
 */
GTLevel2Box32 *WINAPI gtGetLevel2Box32(LPGTLEVEL2BOX32 hLevel2Box);
/*! \ingroup c*/
/**
 * \brief [Depreciated] Get the GTLevel2List32 for a level 2. 
 * \param [in] hLevel2List32 The Level2List that can be got from GTLevel2Box32.
 * \return The pointer GTLevel2List32
 * \remark For who wants to get the level2 information, please use gtGetBidLevel2Count and gtGetBidLevel2Item (or gtGetAskLevel2Count and gtGetAskLevel2Item)
 * to get the level 2 items.
 * \sa gtGetLevel2Box32
 */
GTLevel2List32 *WINAPI gtGetLevel2List32(LPGTLEVEL2LIST32 hLevel2List32);

/*! \ingroup c*/
/**
 * \brief Start the login procedure for the session.
 * \param [in] hSession The session handler.
 * \param [in] pszUserID Username.
 * \param [in] pszPassword Password.
 * \return 0 for success, otherwise -1.
 * \remark Call this to start the login procedure. 

	\par 
	The login of the session (also the GTAPI) is just login to the executor. The login procedure consists several steps:
	First it connects to the executor server. If the executor address is correct, the system will then login the executor, 
	and GTAPI_MSG_SESSION_OnExecConnected callback will be triggered (if you have set one).

	\par
	When connection is established, the username and password will be sent. The
	executor checks for the username and password, sends GTAPI_MSG_SESSION_OnExecMsgLogin event to the API, and in the meanwhile, the login
	data will be sent, an GTAPI_MSG_SESSION_OnExecMsgLoggedin event will be sent when the data sending is finished. To this point, the login 
	procedure is finished. 

	\par
	After logged in, the quote server, level 2 server and chart server will be automatically connected to by the API.
	Thus you need not code explicitly to connect to those servers.

	\par
	If any error happens in this procedure, such as the username/password is not right, the connection will be cut and 
	API will raise GTAPI_MSG_SESSION_OnExecDisconnected event.
 */
int WINAPI gtLogin(LPGTSESSION hSession, LPCSTR pszUserID, LPCSTR pszPassword);
/*! \ingroup c*/
/**
 * \brief Logout a session.
 * \param [in] hSession The session handler.
 */
void WINAPI gtLogout(LPGTSESSION hSession);

/*! \ingroup c*/
/**
 * \brief Connect all the quote server, level2 server and chart server.
 * \param [in] hSession The session handler.
 * \return 0 for success, otherwise -1.
 */
int	WINAPI gtConnectQuotes(LPGTSESSION hSession);

//! @name Buy & Sell Functions
//@{
/*! \ingroup c*/
/**
 * \brief Send a buy order
 * \param [in] hStock The Stock handler.
 * \param [in] nShares Shares to buy.
 * \param [in] dblPrice Price per share.
 * \param [in] method Method to buy.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtBuy(LPGTSTOCK hStock, int nShares, double dblPrice, MMID method);
/*! \ingroup c*/
/**
 * \brief Send a bid order
 * \param [in] hStock The Stock handler.
 * \param [in] nShares Shares to bid.
 * \param [in] dblPrice Price per share.
 * \param [in] method Method to bid.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtBid(LPGTSTOCK hStock, int nShares, double dblPrice, MMID method);

/*! \ingroup c*/
/**
 * \brief Send a sell order
 * \param [in] hStock The Stock handler.
 * \param [in] nShares Shares to sell.
 * \param [in] dblPrice Price per share.
 * \param [in] method Method to sell.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtSell(LPGTSTOCK hStock, int nShares, double dblPrice, MMID method);
/*! \ingroup c*/
/**
 * \brief Send an ask order
 * \param [in] hStock The Stock handler.
 * \param [in] nShares Shares to ask.
 * \param [in] dblPrice Price per share.
 * \param [in] method Method to ask.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtAsk(LPGTSTOCK hStock, int nShares, double dblPrice, MMID method);

/*! \ingroup c*/
/**
 * \brief Send a direct buy order
 * \param [in] hStock The Stock handler.
 * \param [in] nShares Shares to buy.
 * \param [in] dblPrice Price per share.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtBuyDirect(LPGTSTOCK hStock, int nShares, double dblPrice);
/*! \ingroup c*/
/**
 * \brief Send a direct sell order
 * \param [in] hStock The Stock handler.
 * \param [in] nShares Shares to sell.
 * \param [in] dblPrice Price per share.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtSellDirect(LPGTSTOCK hStock, int nShares, double dblPrice);

/*! \ingroup c*/
/**
 * \brief Fill the buy order.
 * \param [in] hStock The Stock handler.
 * \param [in, out] pOrder The GTOrder32 to be filled.
 * \param [in] nShares Shares to buy.
 * \param [in] dblPrice Price per share.
 * \param [in] method Method to buy.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtOrderBuy(LPGTSTOCK hStock, GTOrder32 *pOrder, int nShares, double dblPrice, MMID method);
/*! \ingroup c*/
/**
 * \brief Fill the bid order.
 * \param [in] hStock The Stock handler.
 * \param [in, out] pOrder The GTOrder32 to be filled.
 * \param [in] nShares Shares to bid.
 * \param [in] dblPrice Price per share.
 * \param [in] method Method to bid.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtOrderBid(LPGTSTOCK hStock, GTOrder32 *pOrder, int nShares, double dblPrice, MMID method);

/*! \ingroup c*/
/**
 * \brief Fill the Sell order.
 * \param [in] hStock The Stock handler.
 * \param [in, out] pOrder The GTOrder32 to be filled.
 * \param [in] nShares Shares to Sell.
 * \param [in] dblPrice Price per share.
 * \param [in] method Method to sell.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtOrderSell(LPGTSTOCK hStock, GTOrder32 *pOrder, int nShares, double dblPrice, MMID method);
/*! \ingroup c*/
/**
 * \brief Fill the ask order.
 * \param [in] hStock The Stock handler.
 * \param [in, out] pOrder The GTOrder32 to be filled.
 * \param [in] nShares Shares to ask.
 * \param [in] dblPrice Price per share.
 * \param [in] method Method to ask.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtOrderAsk(LPGTSTOCK hStock, GTOrder32 *pOrder, int nShares, double dblPrice, MMID method);

/*! \ingroup c*/
/**
 * \brief Fill the direct buy order.
 * \param [in] hStock The Stock handler.
 * \param [in, out] pOrder The GTOrder32 to be filled.
 * \param [in] nShares Shares to buy.
 * \param [in] dblPrice Price per share.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtOrderBuyDirect(LPGTSTOCK hStock, GTOrder32 *pOrder, int nShares, double dblPrice);
/*! \ingroup c*/
/**
 * \brief Fill the direct sell order.
 * \param [in] hStock The Stock handler.
 * \param [in, out] pOrder The GTOrder32 to be filled.
 * \param [in] nShares Shares to sell.
 * \param [in] dblPrice Price per share.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtOrderSellDirect(LPGTSTOCK hStock, GTOrder32 *pOrder, int nShares, double dblPrice);
//@}
//! @name Cancel Functions
//@{
/*! \ingroup c*/
/**
 * \brief Cancel all pending order for the session.
 * \param [in] hSession The session handler.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtCancelAll(LPGTSESSION hSession);
/*! \ingroup c*/
/**
 * \brief Cancel one ticket
 * \param [in] hSession The session handler.
 * \param [in] dwTicketNo The ticket number.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtCancelTicket(LPGTSESSION hSession, long dwTicketNo);
/*! \ingroup c*/
/**
 * \brief Cancel all pending order on the side.
 * \param [in] hSession The session handler.
 * \param [in] chSide The side to be cancelled. 'B' for bid/buy, 'S' for ask/sell/shortsell.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtCancelSide(LPGTSESSION hSession, char chSide);

/*! \ingroup c*/
/**
 * \brief Cancel all pending orders for the stock.
 * \param [in] hStock The stock handler.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtCancelStockAll(LPGTSTOCK hStock);
/*! \ingroup c*/
/**
 * \brief Cancel all pending orders using the method for the stock.
 * \param [in] hStock The stock handler.
 * \param [in] method The order method.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtCancelStockMethod(LPGTSTOCK hStock, MMID method);
/*! \ingroup c*/
/**
 * \brief Cancel all pending orders on the side for the stock.
 * \param [in] hStock The stock handler.
 * \param [in] chSide The side.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtCancelStockSide(LPGTSTOCK hStock, char chSide);
//@}
/*! \ingroup c*/
/**
 * \brief Initial the order for the stock
 * \param [in] hStock The stock handler.
 * \param [in, out] pOrder The GTOrder32 object to be initialized.
 * \remark One should pass an order struct into the function for initialize. 
 */
void WINAPI gtInitOrder(LPGTSTOCK hStock, GTOrder32 *pOrder);
/*! \ingroup c*/
/**
 * \brief Send the order for the stock
 * \param [in] hStock The stock handler.
 * \param [in] pOrder The order to be sent.
 * \return 0 for success, otherwise nonzero.
 */
int WINAPI gtPlaceOrder(LPGTSTOCK hStock, GTOrder32 *pOrder);
	
////////////////////
//! @name Stock Information Retrieving Functions
//@{
/*! \ingroup c*/
/**
 * \brief Get level2 item from the level2list.
 * \param [in] hList The list handler.
 * \param [in] nItem The index of the item.
 * \return The level2 item pointer, or NULL if the \c nItem invalid.
 * \remark To get the list, using gtGetLevel2List32. To get the range for the \c nItem, call gtGetBidLevel2Count or gtGetAskLevel2Count. 
	Here, item 0 is the best bid/ask record.

	\par 
	Or one can judge by the return value whether the end is reached. See the example.
	\par Example
	\code
		//hList gets from gtGetLevel2List32. See gtGetLevel2Box32.
		int i = 0;
		GTLevel232 *item;
		item = gtGetLevel2Item(hList, i++);
		i = 0;
		while(item){	//If item==NULL the while loop stops.
			//do something to item

			item = gtGetLevel2Item(hList, i++);
		}
	\endcode
 * \sa gtGetLevel2List32
 */
const GTLevel232 *WINAPI gtGetLevel2Item(LPGTLEVEL2LIST32 hList, int nItem);
/*! \ingroup c*/
/**
 * \brief Get the BestBid level2 item.
 * \param [in] hStock The stock handler.
 * \return The level2 item.
 */
const GTLevel232 *WINAPI gtGetBestBidLevel2Item(LPGTSTOCK hStock);
/*! \ingroup c*/
/**
 * \brief Get the BestAsk level2 item.
 * \param [in] hStock The stock handler.
 * \return The level2 item.
 */
const GTLevel232 *WINAPI gtGetBestAskLevel2Item(LPGTSTOCK hStock);
/*! \ingroup c*/
/**
 * \brief Get bestbid level2 price.
 * \param [in] hStock The stock handler.
 * \return The best bid price.
 */
double WINAPI gtGetBestBidLevel2Price(LPGTSTOCK hStock);
/*! \ingroup c*/
/**
 * \brief Get bestask level2 price.
 * \param [in] hStock The stock handler.
 * \return The best ask price.
 */
double WINAPI gtGetBestAskLevel2Price(LPGTSTOCK hStock);
/*! \ingroup c*/
/**
 * \brief Get bid level2 item number.
 * \param [in] hStock The stock handler.
 * \return The level2 bid item number.
 * \remark This is used with gtGetBidLevel2Item to retrieve all level2 bid records. Call this to get the number of records,
 * and call gtGetBidLevel2Item to get the individual record.
 * \par
 * This is more easy way to read through all the level2 records. See gtGetBidLevel2Item for an example.
 */
int WINAPI gtGetBidLevel2Count(LPGTSTOCK hStock);
/*! \ingroup c*/
/**
 * \brief Get ask level2 item number.
 * \param [in] hStock The stock handler.
 * \return The level2 ask item number.
 * \remark This is used with gtGetAskLevel2Item to retrieve all level2 bid records. Call this to get the number of records,
 * and call gtGetAskLevel2Item to get the individual record.
 * \par
 * This is more easy way to read through all the level2 records. See gtGetAskLevel2Item for an example.
 */
int WINAPI gtGetAskLevel2Count(LPGTSTOCK hStock);

/*! \ingroup c*/
/** 
 * \brief Get the <tt>nItem</tt>th bid level2 item 
 * \param [in] hStock The stock handler.
 * \param [in] nItem The index of the item.
 * \return The pointer to the level2 item, or NULL if reaching the end.
 * \remark This is used with gtGetBidLevel2Count to get all the bid records.
 * \par Example
 * This shows how to get the records. (Also you can judge by the return value for whether reaching the end. See gtGetLevel2Item.)
 * \code
	int n, i;
	GTLevel232 *bid;
	n = gtGetBidLevel2Count(hStock);
	for(i=0; i<n; ++i){
		bid = gtGetBidLevel2Item(hStock, i);
		if(bid==NULL) break;	//if reach the end, return will be NULL;

		//Do something with bid.
	}
 * \endcode
 */
const GTLevel232 *WINAPI gtGetBidLevel2Item(LPGTSTOCK hStock, int nItem);
/*! \ingroup c*/
/** 
 * \brief Get the <tt>nItem</tt>th ask level2 item 
 * \param [in] hStock The stock handler.
 * \param [in] nItem The index of the item.
 * \return The pointer to the level2 item, or NULL if reaching the end.
 * \remark This is used with gtGetAskLevel2Count to get all the bid records.
 * \par Example
 * This shows how to get the records. (Also you can judge by the return value for whether reaching the end. See gtGetLevel2Item.)
 * \code
	int i, n;
	GTLevel232 *ask;
	n = gtGetAskLevel2Count(hStock);
	for(i=0; i<n; ++i){
		ask = gtGetAskLevel2Item(hStock, i);
		if(ask==NULL) break;	//if reach the end, return will be NULL;

		//Do something with ask.
	}
 * \endcode
 */
const GTLevel232 *WINAPI gtGetAskLevel2Item(LPGTSTOCK hStock, int nItem);
/*! \ingroup c*/
/**
 * \brief Get the print items.
 * \param [in] hStock The stock handler.
 * \param [in] nItem The index of the item.
 * \return The pointer to the GTPrint32 item, or NULL if reaching the end.
 * \remark To get the prints, one can call this. See following example.
 * \par Example
	\code
		int i;
		GTPrint32 *item;
		i = 0;
		item = gtGetPrintItem(hStock, i++);
		while(item){	//If item==NULL the while loop stops.
			//do something to item

			item = gtGetPrintItem(hStock, i++);
		}
	\endcode
 */
const GTPrint32 *WINAPI gtGetPrintItem(LPGTSTOCK hStock, int nItem);
/*! \ingroup c*/
/**
 * \brief Get the item number of the prints.
 * \param [in] hStock The stock handler.
 * \return The number of prints.
 * \remark This can be used with gtGetPrintItem to get the prints. The code looks like the one gtGetBidLevel2Item used.
 */
int WINAPI gtGetPrintCount(LPGTSTOCK hStock);
//@}
//! @name Session Information Retrieving Functions
//@{
/*! \ingroup c*/
/**
 * \brief Get the system time.
 * \param [in] hSession Session handler.
 * \return The GTime32.
 */
const GTime32 * WINAPI gtGetSystemTime32(LPGTSESSION hSession);
/*! \ingroup c*/
/**
 * \brief Get the system setting.
 * \param [in] hSession Session handler.
 * \return The GTSetting32.
 * \remark Coupling with gtSetSetting32 one can change the system settings. 
 * \par Example
	This shows how to change a setting:
	\code
		GTSetting32 setting = *gtGetSetting32(hsession);
		setting.m_bAutoRedirect100 = 0;
		gtSetSetting32(hsession, &setting);
	\endcode
 * \sa GTSetting32
 */
GTSetting32 * WINAPI gtGetSetting32(LPGTSESSION hSession);
/*! \ingroup c*/
/**
 * \brief Set the system setting.
 * \param [in] hSession Session handler.
 * \param pSetting32 The setting that wants to be set.
 * \remark Coupling with gtGetSetting32 one can change the system settings. 
 * \sa gtGetSetting32
 */
void WINAPI gtSetSetting32(LPGTSESSION hSession, GTSetting32 *pSetting32);
/*! \ingroup c*/
/**
 * \brief Get the GTUser32 information for the session.
 * \param [in] hSession Session handler.
 * \return The GTUser32.
 */
GTUser32 * WINAPI gtGetUser32(LPGTSESSION hSession);
/*! \ingroup c*/
/**
 * \brief Get the GTAccount32 information for the session.
 * \param [in] hSession Session handler.
 * \return The GTAccount32.
 */
GTAccount32 * WINAPI gtGetAccount32(LPGTSESSION hSession);
//@}
//! @name Stock Information Retrieving Functions
//@{
/*! \ingroup c*/
/**
 * \brief Get the level1 information.
 * \param [in] hStock Stock handler.
 * \return The pointer to GTLevel132 structure.
 */
GTLevel132 * WINAPI gtGetLevel132(LPGTSTOCK hStock);

/*! \ingroup c*/
/**
 * \brief Get the GTOpenPosition32 information.
 * \param [in] hStock Stock handler.
 * \return Pointer to GTOpenPosition32 information.
 */
GTOpenPosition32 * WINAPI gtGetStockOpenPosition32(LPGTSTOCK hStock);

/*! \ingroup c*/
/**
 * \brief Get first pending order information.
 * \param [in] hStock Stock handler.
 * \param [in, out] pTrade32 The GTTrade32 structure. User will have to allocate the memory for the \c pTrade32.
 * \return The next position.
 * \remark This is used to retrieve the pending information for the stock. It is coupled with gtGetStockNextTrade32.
 * \par
 * The \c pTrade32 is the pointer to the GTTrade32 structure that has been allocated by the caller. The return value 
 * will be the next position for the travel. Following code will be an example:
  \code
	GTTrade32 trade;
	UINT pos;
	pos = gtGetStockFirstTrade32(hStock, &trade);
	while(pos!=0){
		//do something to trade ...

		pos = gtGetStockNextTrade32(hStock, pos, &trade);
	}
  \endcode
 * \sa gtGetStockNextPending32
 */
UINT WINAPI gtGetStockFirstTrade32(LPGTSTOCK hStock, GTTrade32 *pTrade32);
/*! \ingroup c*/
/**
 * \brief Get next pending order information.
 * \param [in] hStock Stock handler.
 * \param [in] dwPos The position returned by previous calling of gtGetStockFirstTrade32 or gtGetStockNextTrade32.
 * \param [in, out] pTrade32 The trade structure. User will have to allocate the memory for the \c pTrade32.
 * \return The next position.
 * \sa gtGetStockFirstTrade32
 */
UINT WINAPI gtGetStockNextTrade32(LPGTSTOCK hStock, UINT dwPos, GTTrade32 *pTrade32);

/*! \ingroup c*/
/**
 * \brief Get first pending order information.
 * \param [in] hStock Stock handler.
 * \param [in, out] pPending32 The pending structure. User will have to allocate the memory for the \c pPending32.
 * \return The next position.
 * \remark This is used to retrieve the pending information for the stock. It is coupled with gtGetStockNextPending32.
 * \par
 * The \c pPending32 is the pointer to the GTPending32 structure that has been allocated by the caller. The return value 
 * will be the next position for the travel. Following code will be an example:
  \code
	GTPending32 pending;
	UINT pos;
	pos = gtGetStockFirstPending32(hStock, &pending);
	while(pos!=0){
		//do something to pending ...

		pos = gtGetStockNextPending32(hStock, pos, &pending);
	}
  \endcode
 * \sa gtGetStockNextPending32
 */
UINT WINAPI gtGetStockFirstPending32(LPGTSTOCK hStock, GTPending32 *pPending32);
/*! \ingroup c*/
/**
 * \brief Get next pending order information.
 * \param [in] hStock Stock handler.
 * \param [in] dwPos The position returned by previous calling of gtGetStockFirstPending32 or gtGetStockNextPending32.
 * \param [in, out] pPending32 The pending structure. User will have to allocate the memory for the \c pPending32.
 * \return The next position.
 * \sa gtGetStockFirstPending32
 */
UINT WINAPI gtGetStockNextPending32(LPGTSTOCK hStock, UINT dwPos, GTPending32 *pPending32);
//@}
//! @name Session Information Setting Functions
//@{
/*! \ingroup c*/
/**
 * \brief Set the executor address.
 * \param [in] hSession Session handler.
 * \param [in] pszAddress Address. Can be IP or DNS name.
 * \param [in] nPort Service port number.
 */
void WINAPI gtSetExecAddress(LPGTSESSION hSession, LPCSTR pszAddress, unsigned short nPort);
/*! \ingroup c*/
/**
 * \brief Set the quote server address.
 * \param [in] hSession Session handler.
 * \param [in] pszAddress Address. Can be IP or DNS name.
 * \param [in] nPort Service port number.
 */
void WINAPI gtSetQuoteAddress(LPGTSESSION hSession, LPCSTR pszAddress, unsigned short nPort);
/*! \ingroup c*/
/**
 * \brief Set the level2 server address.
 * \param [in] hSession Session handler.
 * \param [in] pszAddress Address. Can be IP or DNS name.
 * \param [in] nPort Service port number.
 */
void WINAPI gtSetLevel2Address(LPGTSESSION hSession, LPCSTR pszAddress, unsigned short nPort);
/*! \ingroup c*/
/**
 * \brief Set the chart server address.
 * \param [in] hSession Session handler.
 * \param [in] pszAddress Address. Can be IP or DNS name.
 * \param [in] nPort Service port number.
 */
void WINAPI gtSetChartAddress(LPGTSESSION hSession, LPCSTR pszAddress, unsigned short nPort);

/*! \ingroup c*/
/**
 * \brief Set the option quote server address.
 * \param [in] hSession Session handler.
 * \param [in] pszAddress Address. Can be IP or DNS name.
 * \param [in] nPort Service port number.
 */
void WINAPI gtSetOptionQuoteAddress(LPGTSESSION hSession, LPCSTR pszAddress, unsigned short nPort);
/*! \ingroup c*/
/**
 * \brief Set the option level2 server address.
 * \param [in] hSession Session handler.
 * \param [in] pszAddress Address. Can be IP or DNS name.
 * \param [in] nPort Service port number.
 */
void WINAPI gtSetOptionLevel2Address(LPGTSESSION hSession, LPCSTR pszAddress, unsigned short nPort);
/*! \ingroup c*/
/**
 * \brief Set the option chart server address.
 * \param [in] hSession Session handler.
 * \param [in] pszAddress Address. Can be IP or DNS name.
 * \param [in] nPort Service port number.
 */
void WINAPI gtSetOptionChartAddress(LPGTSESSION hSession, LPCSTR pszAddress, unsigned short nPort);

/*! \ingroup c*/
/**
 * \brief Set the session callbacks.
 * \param [in] hSession Session handler.
 * \param [in] nID  The callback ID for the session
 * \param [in] pfn  The function as the callback.
 * \return 0 for success, otherwise -1.
 * \remark This set the function as the callback for the session. The event related to the callback is defined in nID, which can be any one of
	the list of \ref Session_Callback_Event "events for session callback".
 */
int WINAPI gtSetSessionCallBack(LPGTSESSION hSession, int nID, FARPROC pfn);
/*! \ingroup c*/
/**
 * \brief Set the stock callbacks.
 * \param [in] hStock The stock handler
 * \param [in] nID The callback ID for the stock
 * \param [in] pfn The function as the callback.
 * \return 0 for success, otherwise -1.
 * \remark This set the function as the callback for the stock. The event related to the callback is defined in nID, which can be any one of
	the list of \ref Stock_Callback_Event "events for stock callback".
 */
int WINAPI gtSetStockCallBack(LPGTSTOCK hStock, int nID, FARPROC pfn);

/*! \ingroup c*/
/**
 * \brief Set the next trader sequence number.
 * \param [in] hSession The session handler.
 * \param [in] nTraderSeqNo The next traderseqno.
 * \param [in] nStep Increasing afterward.
 * \remark This is used to set the trader's sequence number. After calling this , the next order will be filled with the traderseqno as \c nTraderSeqNo,
   and after that, the traderseqno will increase in the step \c nStep.
 */
void WINAPI gtSetNextTraderSeqNo(LPGTSESSION hSession, long nTraderSeqNo, long nStep);
//@}
//void WINAPI gtGetTickStatistic(int *tick, int* recv, int *send);
/*! \ingroup c*/
/**
 * \brief Set the next trader sequence number.
 * \param [in] hSession The session handler.
 * \param [in, out] pOpen A pointer to the preallocated GTOpenPosition32 struct.
 * \return 0 if has reached the end of the records; otherwise nonzero value is returned, which can be used as an iterator.
 * \remark This is used to retrieve the open position information from a session. Call this to get the first record. After calling this,
 call gtGetNextOpenPosition to get the next one.
 */
UINT WINAPI gtGetFirstOpenPosition(LPGTSESSION hSession, GTOpenPosition32* pOpen);

/*! \ingroup c*/
/**
 * \brief Set the next trader sequence number.
 * \param [in] hSession The session handler.
 * \param [in] pos The position information used to get the next record.
 * \param [in, out] pOpen A pointer to the preallocated GTOpenPosition32 struct.
 * \return 0 if has reached the end of the records; otherwise nonzero value is returned, which can be used as an iterator.
 * \remark This is used to retrieve the open position information from a session. Call gtGetFirstOpenPosition to get the first record. After that,
 call this to get the next one until it return 0..
 */
UINT WINAPI gtGetNextOpenPosition(LPGTSESSION hSession, UINT pos, GTOpenPosition32* pOpen);

/*! \ingroup c*/
/**
 * \brief Set the next trader sequence number.
 * \param [in] hSession The session handler.
 * \param [in, out] pTrade A pointer to the preallocated GTTrade32 struct.
 * \return 0 if has reached the end of the records; otherwise nonzero value is returned, which can be used as an iterator.
 * \remark This is used to retrieve the trade information from a session. Call this to get the first record. After calling this,
 call gtGetNextTrade to get the next one.
 */
UINT WINAPI gtGetFirstTrade(LPGTSESSION hSession, GTTrade32* pTrade);
/*! \ingroup c*/
/**
 * \brief Set the next trader sequence number.
 * \param [in] hSession The session handler.
 * \param [in] pos The position information used to get the next record.
 * \param [in, out] pTrade A pointer to the preallocated GTTrade32 struct.
 * \return 0 if has reached the end of the records; otherwise nonzero value is returned, which can be used as an iterator.
 * \remark This is used to retrieve the trade information from a session. Call gtGetFirstTrade to get the first record. After that,
 call this to get the next one until it return 0..
 */
UINT WINAPI gtGetNextTrade(LPGTSESSION hSession, UINT pos, GTTrade32* pTrade);
/*! \ingroup c*/
/**
 * \brief Set the next trader sequence number.
 * \param [in] hSession The session handler.
 * \param [in, out] pPend A pointer to the preallocated GTPending32 struct.
 * \return 0 if has reached the end of the records; otherwise nonzero value is returned, which can be used as an iterator.
 * \remark This is used to retrieve the pending order information from a session. Call this to get the first record. After calling this,
 call gtGetNextPending to get the next one.
 */
UINT WINAPI gtGetFirstPending(LPGTSESSION hSession, GTPending32* pPend);
/*! \ingroup c*/
/**
 * \brief Set the next trader sequence number.
 * \param [in] hSession The session handler.
 * \param [in] pos The position information used to get the next record.
 * \param [in, out] pPend A pointer to the preallocated GTPending32 struct.
 * \return 0 if has reached the end of the records; otherwise nonzero value is returned, which can be used as an iterator.
 * \remark This is used to retrieve the pending order information from a session. Call gtGetFirstPending to get the first record. After that,
 call this to get the next one until it return 0..
 */
UINT WINAPI gtGetNextPending(LPGTSESSION hSession, UINT pos, GTPending32* pPend);

/*! \ingroup c*/
/**
 * \brief Disconnect Quote.
 * \param [in] hSession The session handler.
 * \param [in] bWait for the disconnection finishing?
 * \return 0 if the request is finished; otherwise nonzero value is returned.
 */
UINT WINAPI gtDisconnectQuote(LPGTSESSION hSession, UINT bWait);

/*! \ingroup c*/
/**
 * \brief Disconnect Quote.
 * \param [in] hSession The session handler.
 * \param [in] bWait for the disconnection finishing?
 * \return 0 if the request is finished; otherwise nonzero value is returned.
 */
UINT WINAPI gtDisconnectLevel2(LPGTSESSION hSession, UINT bWait);

/*! \ingroup c*/
/**
 * \brief Disconnect Quote.
 * \param [in] hSession The session handler.
 * \param [in] bWait for the disconnection finishing?
 * \return 0 if the request is finished; otherwise nonzero value is returned.
 */
UINT WINAPI gtDisconnectChart(LPGTSESSION hSession, UINT bWait);

#ifdef __cplusplus
}
#endif//__cplusplus

#endif//__GTAPI32_H__

/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTSession.h: interface for the GTSession class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTSession.h
	\brief interface for the GTSession class.
 */

#if !defined(__GTSESSION_H__)
#define __GTSESSION_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <string>
#include <list>
#include "Inc32\GTAPI32.h"
#include "Inc32\GTSession32.h"

#include "GTAPI_API.h"
#include "GTime0.h"
#include "GTSetting.h"
#include "GTExecMsg.h"
#include "GTStock.h"
#include "GTOrder.h"
#include "GTQuoteText.h"

#include "GTErrMsg.h"
#include "GTChat.h"
#include "GTPopup.h"
#include "GTServerState.h"

#include "GTUser.h"
#include "GTAccount.h"

#include "GTOpenPosition.h"
#include "GTTrade.h"
#include "GTPending.h"
#include "GTSending.h"
#include "GTCancel.h"
#include "GTReject.h"
#include "GTRemove.h"
#include "GTRejectCancel.h"
#include "GTStatus.h"

#define INIT_SESSION_CALLBACK(name)	\
	m_pfn##name = NULL;				\
	m_wnd##name = NULL;				\
	m_msg##name = NULL;				\

#define CASE_SESSION_CALLBACK(name, pfn)				\
	case GTAPI_MSG_SESSION_##name: m_pfn##name = (PFNSession##name)pfn; break;		\

class GTSessionSystem;
class CGTChainManager;
/////////////////////////////////////////////////////////////////////////////
// GTSession window
/*! \ingroup cpp
*/
/*! \class GTSession
    \brief Session class

	GTSession class is derived from CWnd. It must be created in user's application. Session object
	controls the connection between servers and includes all stocks object. In GTSession class, 
	There are a lots of functions, like Login(), Logout(), CancelAllOrders(), and so on. 

	The connection of the GTAPI to the servers is maintained in this class. The API will check for
	the health of the connection and if some problems are discovered, the reconnection will be done
	automatically. Thus, one need not code for the reconnection.

	Many virtual callbacks are defined in this class which handle the GTAPI system events. Notice 
	that the events for stocks will be further delivered to the targeted GTStock object. Here most 
	of the events are related to the connections, or accounts/users. One should implement the 
	interested callbacks. The typical usage is to program one's own GTSession class that derives from 
	this class:
	\code
		class MySession: public GTSession{
			//Add you implementations here.

		};
	\endcode
	
	Most callbacks are triggered by some event from the servers. The event will first go to GTSession, which
	will cause the corresponding callback to be called. And if the event is also related to some GTStock, 
	GTSession will distribute the event to the destination GTStock object, and eventually the callback of GTStock
	will get called. So it is important to call the base class function in your implement of
	those callback functions (which will distribute the event to GTStock if applicable.). Also,
	most events are related to the individual symbols. It wiill be more reasonable to implement
	the callbacks for the symbol GTStock instead for the session. So, the suggestion is that, only those 
	events about connectivity should be implemented in the GTSession. 

	When implement some callbacks, one should call the base method. one should have the code like:
	\code
		//This is just an example. You will never ever implement OnCreateStock.
		//The idea in this example is, make sure to call the base method.
		class MySession: public GTSession{
			virtual GTStock *OnCreateStock(LPCSTR pszStock);
		};

		GTStock *MySession::OnCreateStock(LPCSTR pszStock){
			//Do anything before create the GTStock;
			
			GTStock stock = GTSession::OnCreateStock(pszStock);

			//Do things after creating the GTStock object;

			return stock;
		}
	\endcode

	One other callback that should be implemented is OnCreateStock(). It is the place that new GTStock object is 
	actually created. To let session create user-defined stock class, you should write this as:
	\code
		GTStock* MySession::OnCreateStock(LPCSTR pszStock)
		{
			MyStock *pStock = new MyStock(*this, pszStock);
			if(pStock == NULL)
				return NULL;
			
			//TO DO: Other stuff to initialize the stock object;
			
			//END OF TO DO

			return pStock;
		}
	\endcode
	It returns the pointer to the newly created stock object.

	Many methods in this class are extremely useful. Login() and Logout() enable
	the user to login and logout to the server. CreateStock(), DeleteStock(), DeleteAllStocks()
	and GetStock() provide the GTStock object management. CancelAllOrders() and CancelOrder()
	can be used to cancel orders from the session directly.

	\note 
	GTSession is derived from class CWnd. This means that, the methods one can do to the CWnd object can also be done to a 
	GTSession object. As one example, you can subclass the GTSession. To do that, one has to add a custom control to the 
	resource (can be located in any place one wants), and the class parameter of the control should be GTSessionWindow. Then
	call \c SubclassDlgItem() of CWnd method to add the custom control into message loop.
*/

class GTAPI_API GTSession : public CWnd, public GTSession32
{
	friend GTStock;
	friend GTSessionSystem;

private:
	LPVOID		m_sys;
	UINT		m_nTimer;
	BOOL		m_bAutoLogin;
	int			m_nTryingCount;

public:
	//! \brief System Time
	/*!
		\remark A pointer to internel system time. This time is updated once a second. 
		If the API is running with real quotes, the time is the same as local computer time. 
		Or it is the same as history quotes time.
	*/
	const GTime0	*m_pSysTime;

	//! \brief System Date
	/*!
		\remark A pointer to internel system date. 
		If the API is running with real quotes, the date is the same as local computer date. 
		Or it is the same as history quotes date.
	*/
	const GDate0	*m_pSysDate;

	/*!
	  	\brief Auto create stocks
		\remark An indicator to create stocks automatically when receiving Level 1 quote.
	*/
	BOOL			m_bAutoCreateStocks;

	/*!
	  	\brief Auto resize stock hash map
		\remark An indicator to resize stocks hash map size when too small.
	*/
	BOOL			m_bAutoResizeStocks;

public:
	//! \brief User Information
	/*!
		\remark It has all detailed informations of this user after login.
	*/
	GTUser			m_user;

	CMapGTAccount	m_accounts;

	//! \brief The GTSetting for the session.
	/*!
		\remark The global settings for the session. Any options listed will effect all the actions in the session.
	*/
	GTSetting		m_setting;

	//! \brief Stocks map
	/*!
		\remark The map to store the GTStock objects.
	*/
	GTMapStock		m_stocks;

#ifdef GTSESSION_ORDERS
	GTCancelList	m_cancels;
	GTOrderList		m_orders;
#endif

public:
	//! \brief Constructor
	GTSession();
	//! \brief Destructor
	virtual ~GTSession();

	//! \brief Reset the content of the session. 
	/*!
		\remark This will erase all the information. So be very careful when calling this.
		\note The GTStock object will not be deleted. Instead, the GTStock::ResetContent() will be called for all the existed symbols.
	*/
	virtual void ResetContent();
	//! \brief Dump the information into a file.
	virtual int Dump(FILE *fp, int nLevel) const;

public:
	//! \brief Set login with specified username and password.
	/*!
		\param [in] pszUserName Username.
		\param [in] pszPassword Password (for secure login users, please input a '*' before password. 
		e.g. Your secure password is "123", then pszPassword should be "*123".
		\return 0 for success, otherwise -1.

		\remark This just sets the system to login and raises up the thread to try to login
		to the executor. 

		\par 
		The login of the session (also the GTAPI) is just login to the executor. The login procedure consists several steps:
		First it connects to the executor server. If the executor address is correct, the system will then login the executor, 
		and OnExecConnected() callback will be triggered.
		
		\par
		When connection is established, the username and password will be sent. The
		executor checks for the username and password, sends OnExecMsgLogin event to the API, and in the meanwhile, the login
		data will be sent, an OnExecMsgLoggedin event will be sent when the data sending is finished. To this point, the login 
		procedure is finished. 

		\par
		After logged in, the quote server, level 2 server and chart server will be automatically connected to by the API.
		Thus you need not code explicitly to connect to those servers.

		\par
		If any error happens in this procedure, such as the username/password is not right, the connection will be cut and 
		API will raise OnExecDisconnected event.

		\par 
		Following is the code snippet showing how to login:
		\code
			//m_session is a GTSession object.

			//set the server addresses:
			m_session.m_setting.SetExecAddress("10.1.3.101", 16405);
			m_session.m_setting.SetQuoteAddress("10.1.3.29", 16811);
			m_session.m_setting.SetLevel2Address("10.1.3.29", 16810);

			//login
			m_session.Login(m_strUserName, m_strPassword);	
		\endcode

		\sa Logout	GTSetting::SetExecAddress GTSetting::SetQuoteAddress GTSetting::SetLevel2Address
	*/
	int Login(LPCSTR pszUserName, LPCSTR pszPassword);
	//! \brief Logout.
	/*!
		\remark This will cause the program logout from the executor and disconnect from the servers.
	*/
	void Logout();
	//! \brief Is logged in?
	BOOL IsLoggedIn() const;
	
public:
	//! \brief Set current account ID.
	/*!
		\param pszAccountID The account ID to set.
		\return 0 for success, otherwise -1.
	*/
	int SetCurrentAccount(LPCSTR pszAccountID);
	//! \brief Get current account ID.
	/*! \return The current account ID.
	*/
	LPCSTR GetCurrentAccount() const;

	// GetAllAccountName will return all accounts name for current user.
	// return: 0 for ok,else for failure.
	int GetAllAccountName(std::list<std::string>& lstAccount);
protected:
	
	/**
	 *	\brief Start the trying to login.	
		\return 0 for success, otherwise -1.
	 */
	int TryLogin();

/*! @name Stock control
	These methods are used to control stocks.
*/
//@{
public:
	/**	
	 * \brief Create a symbol in the system.
	 * \param [in] pszStock The symbol name.
	 * \return GTStock that generated.
	 *	
		\remark This will register a symbol in the system. Thereafter, the events of the symbol will
		be enabled to trigger the callbacks, also, through the generated GTStock object, one
		can do actions like buy/sell/bid/ask.

		\sa OnCreateStock
	 */
	virtual GTStock *CreateStock(LPCSTR pszStock);

	/**	
	 * \brief Create a symbol in the system.
	 * \param [in] pszStock The symbol name.
	 * \param [in] pParam The parameter that you want to send to OnCreateStock2.
	 * \return GTStock that generated.
	 *	
		\remark This will register a symbol in the system. Thereafter, the events of the symbol will
		be enabled to trigger the callbacks, also, through the generated GTStock object, one
		can do actions like buy/sell/bid/ask.

		\sa CreateStock OnCreateStock2
	 */
	virtual GTStock *CreateStock2(LPCSTR pszStock, LPVOID pParam);

	/**
	 * \brief Delete all the symbol in the system.
	 * \return 0 for success, otherwise -1.

		\remark Delete all the registered symbol in the system.
	 */
	int DeleteAllStocks();

	/**
	 * \brief Delete one symbol in the system.
	 * \param [in] pszStock The symbol to be deleted.
	 * \return 0 for success, otherwise -1.

		\remark The symbol to be deleted is specified by the parameter parameter pszStock.
	 */
	int DeleteStock(LPCSTR pszStock);
	/**
	 * \brief Delete stock specified by the pointer to the object.
	 * \param [in] pStock The pointer to the GTStock object.
	 * \return 0 for success, otherwise -1.
	 */
	int DeleteStock(GTStock *pStock);

	/**
	 * \brief Get the GTStock object.
	 * \param [in] pszStock The symbol name.
	 * \return The pointer to the existed GTStock object or NULL.
	 */
	GTStock *AutoCreateStock(LPCSTR pszStock);
	/**
	 * \brief Get the GTStock object.
	 * \param [in] pszStock The symbol name.
	 * \return The pointer to the existed GTStock object or NULL.
	 */
	GTStock *GetStock(LPCSTR pszStock);
	/**
	 * \brief Change one symbol to another.
	 * \param [in] pszOld Old symbol name.
	 * \param [in] pszNew New symbol name.
	 * \return 0 for success, 1 for the new symbol name has existed, -1 for the old symbol did not exist.
	 */
	int	ChangeStock(LPCSTR pszOld, LPCSTR pszNew);

	/**
	 * \brief Resize stock hasp map.
	 * \return 0 for success, 1 for unnecessariness.
	 */
	int ResizeStocks();
//@}
protected:
	/**
	 * \brief Callback OnCreateStock(LPCSTR pszStock).
	 * \param [in] pszStock The symbol name.
	 * \return The created GTStock object.
		
		\remark This is triggered when CreateStock is called. Internal GTSession::OnCreateStock() simply new the GTStock object. 
		If you rewrite the subclass of GTStock, you should implement this. 
		
		\par
		To let session create user-defined stock class, you should write this as:
		\code
			GTStock* MySession::OnCreateStock(LPCSTR pszStock)
			{
				MyStock *pStock = new MyStock(*this, pszStock);
				if(pStock == NULL)
					return NULL;
				
				//TO DO: Other stuff to initialize the stock object;
				
				//END OF TO DO

				return pStock;
			}
		\endcode
		It returns the pointer to the newly created stock object.
		
		\sa CreateStock
	 */
	virtual GTStock *OnCreateStock(LPCSTR pszStock);

	/**
	 * \brief Callback OnCreateStock2(LPCSTR pszStock, LPVOID pParam).
	 * \param [in] pszStock The symbol name.
	 * \param [in] pParam	Parameter used
	 * \return The created GTStock object.
		
		\remark This is triggered when CreateStock is called. Internal GTSession::OnCreateStock() simply new the GTStock object. 
		If you rewrite the subclass of GTStock, you should implement this. 
		
		\par
		To let session create user-defined stock class, you should write this as:
		\code
			GTStock* MySession::OnCreateStock2(LPCSTR pszStock, LPVOID pParam)
			{
				YourData * pyd = (YourData *)pParam;	
				MyStock *pStock = new MyStock(*this, pszStock, pyd);
				if(pStock == NULL)
					return NULL;
				
				//TO DO: Other stuff to initialize the stock object;

				//END OF TO DO

				return pStock;
			}
		\endcode
		It returns the pointer to the newly created stock object.

		\sa OnCreateStock CreateStock2
	 */
	virtual GTStock *OnCreateStock2(LPCSTR pszStock, LPVOID pParam);
	/**
	 * \brief Callback <tt>void OnDeleteStock(GTStock *pStock)</tt>.
	 * \param [in] pStock The pointer to GTStock object.

		\remark This is triggered when DeleteStock is called. Internal one simply deletes the object.
		Don't implement this in the derived class unless one has to. 
	 */
	virtual void OnDeleteStock(GTStock *pStock);
	/**
	 * \brief Callback <tt>int OnInitStock(GTStock &stock)</tt>.
	 * \param [in] stock The GTStock to be initialized.
	 * \return 0 for success, otherwise -1.

	 \remark It will call the GTStock::Init(). Don't implement this in the derived class unless one has to. If one has to
	 do so, one must call this base method.

	 */
	virtual int OnInitStock(GTStock &stock);
	/**
	 * \brief Callback <tt>int OnExitStock(GTStock &stock)</tt>.
	 * \param [in] stock The GTStock to be uninitialized.
	 * \return 0 for success, otherwise -1.

	   \remark It will call the GTStock::Exit(). Don't implement this in the derived class unless one has to. If one has to
		do so, one must call this base method.

	 */
	virtual int OnExitStock(GTStock &stock);

/*! @name Connection control
	These methods are used to connect/disconnect to the server. Don't call them since system will do so for you internally.
	If you want to connect the the servers, just set the proper server addresses and use the right username and password
	to Login, the API will do the connection for you internally. If you want to disconnect from the servers, call TryClose()
	which will close the connections for you internally.
*/
//@{
public:
	/**
	 * \brief This will start to connect all quote, level2, chart servers.
	 * \param [in] bForce Startup everything by force
	 * \return 0 for success, otherwise -1.
	 */
	int DoConnectQuotes(BOOL bForce = FALSE);
	/**
	 * \brief Connect to quote server. 
	 * \return 0 for success, otherwise -1.

	 \remark Called in DoConnectQuotes(). Users won't want to call this.
	 */
	int ConnectQuote();
	/**
	 * \brief Request to close connection to the quote server.
	 * \param [in] bWait Enable/disable the waiting for the closing.
	 * \return 0 for success. If bWait==TRUE, the return 1 indicate after waiting 1 second the close of connection
		is still not completed.
	 */
	int DisconnectQuote(BOOL bWait);
	/**
	 * \brief Connect to option quote server. 
	 * \return 0 for success, otherwise -1.

	 \remark Called in DoConnectQuotes(). Users won't want to call this.
	 */
	int ConnectOptionQuote();
	/**
	 * \brief Request to close connection to the quote server.
	 * \param [in] bWait Enable/disable the waiting for the closing.
	 * \return 0 for success. If bWait==TRUE, the return 1 indicate after waiting 1 second the close of connection
		is still not completed.
	 */
	int DisconnectOptionQuote(BOOL bWait);
	/**
	 * \brief Connect to level2 server. 
	 * \return 0 for success, otherwise -1.

	 \remark Called in DoConnectQuotes(). Users won't want to call this.
	 */
	int ConnectLevel2();
	/**
	 * \brief Request to close connection to the level2 server.
	 * \param [in] bWait Enable/disable the waiting for the closing.
	 * \return 0 for success. If bWait==TRUE, the return 1 indicate after waiting 1 second the close of connection
		is still not completed.
	 */
	int DisconnectLevel2(BOOL bWait);
	/**
	 * \brief Connect to chart server. 
	 * \return 0 for success, otherwise -1.

	 \remark Called in DoConnectQuotes(). Users won't want to call this.
	 */
	int ConnectChart();
	/**
	 * \brief Request to close connection to the chart server.
	 * \param [in] bWait Enable/disable the waiting for the closing.
	 * \return 0 for success. If bWait==TRUE, the return 1 indicate after waiting 1 second the close of connection
		is still not completed.
	 */
	int DisconnectChart(BOOL bWait);

	/**
	 * \brief Add an UDP quote source.
	 * \param [in] pszIFAddr Interface IP address.
	 * \param [in] uIFPort Interface port number.
	 * \param [in] pszMCAddr Multicast IP address.
	 * \param [in] uMCPort Multicast port number.
	 * \return 0 for success. 0 for success, otherwize -1.
	 */
	int AddUDPQuote(LPCSTR pszIFAddr, WORD uIFPort, LPCSTR pszMCAddr, WORD uMCPort);

	/**
	 * \brief Remove an UDP quote source.
	 * \param [in] uIFPort Interface port number.
	 * \return 0 for success. 0 for success, otherwize -1.
	 */
	int RemoveUDPQuote(WORD uIFPort);

	/**
	 * \brief Stop receiving UDP quotes
	 * \param [in] bWait Wait until all threads are closed or not.
	 * \return 0 for success. 0 for success, otherwize -1.
	 */
	int StopUDPQuotes(BOOL bWait);

	/**
	 * \brief Retrieve some information from a UDP connection.
	 * \param [in] nCode the code of UDP information.
	 * \param [in] nPort the UDP port to be retrieve.
	 * \return UDP information of the port.
	 */
	int GetUDPInfo(int nCode, unsigned short nPort);
//@}
public:
	/**
	 * \brief Do correct level2 for all GTStocks.
	 * \return 0 for success, otherwise -1.
	 */
	int AutoCorrectLevel2();

public:
	/**
	 * \brief Get the order sequence number.
	 * \param [out] pdwNext The next order sequence number will be used.
	 * \param [out] pdwStep Afterwards the increasing step will be used. Default is 1.
	 * \return 0 for success, otherwise -1.
	 */
	int GetOrderSeqNo(long *pdwNext, long *pdwStep) const;
	/**
	 * \brief Set the order sequence number.
	 * \param [in] dwNext The next order sequence number will be used.
	 * \param [in] dwStep Afterwards the increasing step will be used. Default is 1.
	 * \return 0 for success, otherwise -1.

	 \remark This will effect the next and thereafter orders. 
	 */
	int SetOrderSeqNo(long dwNext, long dwStep = 1);
	/**
	 * \brief Used by C version. Internal used. 
	 * \param [in] nID The function ID.
	 * \param [in] pfn The function pointer.
	 * \return 0 for success, otherwise -1.
	 */
	int SetCallBack(int nID, FARPROC pfn);

public:
	/**
	 * \brief Is the executor connected?
	 * \return TRUE if connected; FALSE if not.
	 */
	BOOL IsExecutorConnected() const;
	/**
	 * \brief Is the method connection connected?
	 * \param [in] method The MMID of the method.
	 * \return TRUE if connected; FALSE if not.
	 */
	BOOL IsMethodConnected(MMID method) const;

/*! @name Order handling
	These methods are used to cancel or send orders.
*/
//@{
public:
	/**
	 * \brief Cancel all the pending orders.
	 * \return 0 for success, otherwise -1.

	 \remark If method is failed, check if executor is still connected. 
	 
	 \par 
	 If one has many accounts, this will cancel all the orders in all the account.
	 */
	int CancelAllOrders();
	/**
	 * \brief Cancel all the pending orders of one account.
	 * \param [in] pszAccountID The account ID.
	 * \return 
	 */
	int CancelAllOrders(LPCSTR pszAccountID);
	/**
	 * \brief Cancel one order.
	 * \param [in] dwTicketNo Ticket number
	 * \return 0 for success, otherwise -1.

	 Most time one might want to call GTStock::CancelOrder().
	 */
	int CancelOrder(long dwTicketNo);

	/**
	 * \brief Cancel one order.
	 * \param [in] pend Pending structure.
	 * \return 0 for success, otherwise -1.

	 Most time one might want to call GTStock::CancelOrder().
	 */
	int CancelOrder(const GTPending32 &pend);
	/**
	 * \brief Cancel one side order in current account.
	 * \param [in] chSide The side.
	 * \return 0 for success, otherwise -1.

	 \remark Most time one might want to call GTStock::CancelOrder().
	 */
	int	CancelOrder(char chSide);

	/**
	 * \brief Send out all the cancels in the GTCancelList
	 * \param [in] cancels The GTCancelList object that holds all cancels.
	 * \return 0 for success, otherwise -1.
	 */
	int SendOutCancels(const GTCancelList &cancels);
	/**
	 * \brief Send out all the cancels in the GTOrderList
	 * \param [in] orders The GTOrderList object that holds all orders.
	 * \return 0 for success, otherwise -1.

	 \remark One might want to call GTStock::SendOutOrders() instead.
	 */
	int SendOutOrders(const GTOrderList &orders);

	/**
	 * \brief Set execution throttle
	 * \param [in] nLimit the number of orders to be sent in period nInterval
	 * \param [in] nInterval Throttle interval, default 1000 (ms)
	 * \param [in] nSleep 
	 * \return 0 for success, otherwise -1.

	 \remark One might want to call GTStock::SendOutOrders() instead.
	 */
	int SetExecutionThrottle(int nLimit, int nInterval = -1, int nSleep = -1);

public:
	/**
	 * Remove a pending order.
	 * \param [in] dwTicketNo The ticket number.
	 * \return 0 for success. Otherwise -1.
	 */
	int RemoveOrder(long dwTicketNo);
//@}
public:
	/**
	 * \brief Get the current tick count.
	 * \return The system tick count.

	 \remark This will call windows API ::GetTickCount().
	 */
	DWORD GetTickCount() const;
	/**
	 * \brief Get the network speed.
	 * \param [in] pnSend The returned send speed will be filled in this parameter.
	 * \return The recieve speed.
	 */
	int GetGlobalNetworkSpeed(int *pnSend);

	/**
	 * \brief Get the quote traffic speed.
	 * \param [in,out] pnSend The returned send speed will be filled in this parameter.
	 * \param [in,out] pnRecv The returned receive speed will be filled in this parameter.
	 * \param [in] bReset The internel counter is reset or not.
	 * \return TRUE if calculated, or FALSE.
	 */
	BOOL GetQuoteSpeed(int *pnSend, int *pnRecv, BOOL bReset);

	/**
	 * \brief Get the level 2 traffic speed.
	 * \param [in,out] pnSend The returned send speed will be filled in this parameter.
	 * \param [in,out] pnRecv The returned receive speed will be filled in this parameter.
	 * \param [in] bReset The internel counter is reset or not.
	 * \return TRUE if calculated, or FALSE.
	 */
	BOOL GetLevel2Speed(int *pnSend, int *pnRecv, BOOL bReset);

public:
	/**
	 * \brief Try to close the session.
	 * \return 0 or 1 for success.

	 \remark Call this to try to close the session. The proper way to close the session will be first call this, and later call CanClose to get the status.
	 
	 \par 
	 An possible code read like:
	 \code
		MySession::Close(){
			while(!GTSession::CanClose()){
				GTSession::TryClose();
				Sleep(0);
			}
		}
	 \endcode
	 */
	virtual int TryClose();
	/**
	 * \brief Is it safe now to close the session?
	 * \return If the system is clean for close, TRUE will be returned, and one can then safely delete the GTSession. Otherwise one should call
		TryClose() to close the session.

		\sa TryClose
	 */
	virtual BOOL CanClose() const;

	/**
	 * [Internal Used] Make it rock.
	 */
	virtual void ClearMessage();

public:
/*! @name Debug used
	These methods are used to debug.
*/
//@{
	//! \brief [Internal Used] Debugging used
	BOOL IsPlayingHistory() const;
	int PlayHistory(LPCSTR pszQuoteFile);
	void PlayHistoryClearMemory(LPCSTR pszStock);
	void CancelPlayHistory();
	
	int PrintPerformance(LPCSTR pszAccountID, LPCSTR pszDate, LPCSTR pszMethod, LPCSTR pszStrategy, LPCSTR pszVersion, LPCSTR pszFileName, const GTSetting &setting);
	int PrintTrades(LPCSTR pszFile) const;
	int PrintTrades(FILE *fp) const;

	double CalcEcnTradeFee(const GTTrade &trade, BOOL *pbTicket);
	double CalcEcnCancelFee(const GTCancel &cancel);

	void CalcGrossNet();
	void AddCmdUser(LPCSTR pszUser);
	void DeleteCmdUser(LPCSTR pszUser);

	long GetTrainModeTicketNo();
	void SetTrainModeTicketNo(long dwTicketNo);
//@}
public:
/*! @name Data retrieval
	These methods are used to retrieve data.
*/
//@{
	/**
	 * \brief Get the first open position record.
	 * \param [in]	pszAccountID	Account ID. If single account, this can be NULL.
	 * \param [out] open			The GTOpenPosition struct will be filled with the information.
	 * \return The next position iterator.
	 * 
	 \remark Used to retrieve all the open position records. An example as:
	 \code
			GTOpenPosition open;
			void *pos = m_session.GetFirstOpenPosition(NULL, open);
			while(pos != NULL){
				//do something to open;

				pos = m_session.GetNextOpenPosition(NULL, open);
			}
	 \endcode
	 */
	void *GetFirstOpenPosition(const char *pszAccountID, GTOpenPosition &open);

	/**
	 * \brief Get the next open position information.
	 * \param [in]	pszAccountID	Account ID. If single account, this can be NULL.
	 * \param [in]	pos				The position iterator returned by the previous calling of GetFirstOpenPosition() or GetNextOpenPosition().
	 * \param [out]	open			The GTOpenPosition struct will be filled with the information.
		
	  \sa GetFirstOpenPosition
	 */
	void *GetNextOpenPosition(const char *pszAccountID, void *pos, GTOpenPosition &open);
	
	/**
	 * \brief Query an open position information.
	 * \param [in]	pszStock		Stock to be queried.
	 * \param [in]	pszAccountID	Account ID. If single account, this can be NULL.
	 * \param [out]	open			The GTOpenPosition struct will be filled with the information.

	 * \return non-zero for success, 0 for failure.
	 */
	BOOL QueryOpenPosition(const char *pszStock, const char *pszAccountID, GTOpenPosition &open) const;

	/**
	 * \brief Get the first trade record.
	 * \param [in]	pszAccountID	Account ID. If single account, this can be NULL.
	 * \param [out]	trade			The GTTrade struct will be filled with the information.
	 * \return The next position iterator.
	 * 
	 \remark Used to retrieve all the trade records. An example as:
	 \code
			GTTrade trade;
			void *pos = m_session.GetFirstFill(NULL, trade);
			while(pos != NULL){
				//do something to trade;

				pos = m_session.GetNextFill(NULL, trade);
			}
	 \endcode
	 */
	void *GetFirstFill(const char *pszAccountID, GTTrade &trade);
	/**
	 * \brief Get the next trade information.
	 * \param [in]	pszAccountID	Account ID. If single account, this can be NULL.
	 * \param [in]	pos				The position iterator returned by the previous calling of GetFirstFill() or GetNextFill().
	 * \param [out]	trade			The GTTrade struct will be filled with the information.

	 \sa GetFirstFill
	 */
	void *GetNextFill(void *pos, const char *pszAccountID, GTTrade &trade);
	
	/**
	 * \brief Query a fill information.
	 * \param [in]	dwTicketNo		TicketNo to be queried.
	 * \param [in]	dwMatchNo		MatchNo to be queried.
	 * \param [in]	pszAccountID	Account ID. If single account, this can be NULL.
	 * \param [out]	fill			The GTTrade struct will be filled with the information.

	 * \return non-zero for success, 0 for failure.
	 */
	BOOL QueryFill(long dwTicketNo, long dwMatchNo, const char *pszAccountID, GTTrade &fill) const;

	/**
	 * \brief Get the first pending record.
	 * \param [in]	pszAccountID	Account ID. If single account, this can be NULL.
	 * \param [out]	pending	 		The GTPending struct will be filled with the information.
	 * \return The next position iterator.
	 * 
	 \remark Used to retrieve all the pending records. An example as:
	 \code
			GTPending pending;
			void *pos = m_session.GetFirstPending(NULL, pending);
			while(pos != NULL){
				//do something to pending;

				pos = m_session.GetNextPending(NULL, pending);
			}
	 \endcode
	 */
	void *GetFirstPending(const char *pszAccountID, GTPending &pending);
	/**
	 * \brief Get the next pending information.
	 * \param [in]	pos				The position iterator returned by the previous calling of GetFirstFill() or GetNextFill().
	 * \param [in]	pszAccountID	Account ID. If single account, this can be NULL.
	 * \param [out]	pending			The GTPending struct will be filled with the information.

	 \sa GetFirstPending
	 */
	void *GetNextPending(void *pos, const char *pszAccountID, GTPending &pending);
	
	/**
	 * \brief Query a pending information.
	 * \param [in]	dwTicketNo		TicketNo to be queried.
	 * \param [in]	pszAccountID	Account ID. If single account, this can be NULL.
	 * \param [out]	pending			The GTPending struct will be filled with the information.

	 * \return non-zero for success, 0 for failure.
	 */
	BOOL QueryPending(long dwTicketNo, const char *pszAccountID, GTPending &pending) const;
//@}
protected:
	/**
	 * \brief Initialize the session.
	 * \return 0 for success, owtherwise -1.

	\remark  One need not call this. It will be called automatically by the API when initializing the system. One can implement
	 this in the derived class. But make sure to call GTSession::InitSession() explicitly in the implementation.
	 */
	virtual int InitSession();
	/**
	 * \brief Uninitialize the session.
	 * \return 0 for success, owtherwise -1.

	\remark  One need not call this. It will be called automatically by the API when uninitializing the system. One can implement
	 this in the derived class. But make sure to call GTSession::ExitSession() explicitly in the implementation.
	 */
	virtual int ExitSession();

	/**
	 * \brief The callback <tt>int OnTick()</tt> for the internal timer.
	 * \return 0 for success, otherwise -1.

	\remark  In the GTAPI system, one timer is internally placed, which will trigger per second. It will check for the system health
	 and do corresponding actions like reconnection and so on. Also every GTStock object's GTStock::OnTick() will be called
	 in this callback.

	\par 
	 It is possible to implement this if one wants to do some systemwide check routine. One important thing is to call GTSession::OnTick()
	 in the implementation.
	 \code
		class MySession: public GTSession{...};

		int MySession::OnTick(){
			//Do your stuff. This will be called each second.

			return GTSession::OnTick();
		}
	 \endcode

	 \sa GTStock::OnTick()
	 */
	virtual int OnTick();
	/**
	 * \brief Callback <tt>int OnExecMsg(const GTExecMsg &msg)</tt> to handle the user defined message.
	 * \param [in]	msg		The message got.
	 * \return 0 for success, otherwise -1.

	\remark  This is not related to the trade or executor. It is useless in almost all the time.
	 */
	virtual int OnExecMsg(const GTExecMsg &msg);

/*!
	@name Executor callbacks
	These callbacks are related to the executor. API connects to the executor via a TCP/IP connection. 
	All the events generated by the executor will trigger the API to call one of below callbacks. Thus
	one should put his stuff in the implementation of interested callbacks.

	The login procedure is described in the Login(). There several callbacks are mentioned. 
*/
//@{
	/**
	 * \brief Callback <tt>int OnExecHeartBeat()</tt> for getting heartbeat signal from executor.
	 * \return 0 for success, otherwise -1.

	   \remark Heartbeat signal is generated per second to detect the health of connection. If the API does not
	 recieve the heartbeat for several second, the API will disconnect the executor and try to reconnect.
	 User needs not to implement this.
	 */
	virtual int OnExecHeartBeat();
	/**
	 * \brief Callback <tt>int OnExecConnected()</tt> for getting connected with executor.
	 * \return 0 for success, otherwise -1.

	   \remark This is called when the network connection (physical connection) is established. 
	 */
	virtual int OnExecConnected();
	/**
	 * \brief Callback <tt>int OnExecDisconnected()</tt> for getting disconnected from executor.
	 * \return 0 for success, otherwise -1.

	   \remark This is called when the connection to executor is lost. 

	   \par
		The connection of the GTAPI to the servers is maintained in this class. The API will check for
		the health of the connection and if some problems are discovered, the reconnection will be done
		automatically. Thus, one need not code for the reconnection.
	   
	 */
	virtual int OnExecDisconnected();
	/**
	 * \brief Callback <tt>int OnExecMsgState(const GTServerState &state)</tt> for executor state changing.
	 * \param [in] state		The GTServerState that describe the changes
	 * \return 0 for success, otherwise -1.
	   \remark This is called when the state of any server changes. For example, when ISLD method is disconnected,
	   the executor will find the state change of the ISLD server, and it will tell API to have this callback called.
	   
	   \sa GTServerState32
	 */
	virtual int OnExecMsgState(const GTServerState &state);
	/**
	 * \brief Callback <tt>int OnExecMsgErrMsg(const GTErrMsg &errmsg)</tt> for getting some error message from executor.
	 * \param [in]	errmsg	The GTErrMsg that describe the error.
	 * \return 0 for success, otherwise -1.
	   \remark This is called when the executor sends some error message. For example, if you send an order
	   to the executor but executor finds it erratic, it will send a error message to the API as notification.
	 */
	virtual int OnExecMsgErrMsg(const GTErrMsg &errmsg);
	/**
	 * \brief Callback <tt>int OnExecMsgChat(const GTChat &chat)</tt> when getting chat message from executor.
	 * \param [in]	chat		The GTChat contains the chat content.
	 * \return 0 for success, otherwise -1.
	   \remark This is called when the executor sends chat message to the API. User needs not to implement this.
	 */
	virtual int OnExecMsgChat(const GTChat &chat);
	/**
	 * \brief Callback <tt>int OnExecMsgPopup(const GTPopup &popup)</tt> when getting popup message from executor.
	 * \param [in]	popup		The GTPopup that contains the popup message.
	 * \return 0 for success, otherwise -1.
	   \remark This is called when the executor sends popup message to the API. User needs not to implement this.
	 */
	virtual int OnExecMsgPopup(const GTPopup &popup);

	/**
	 * \brief Callback <tt>int OnExecMsgLogin(BOOL bLogin)</tt> when logging into executor.
	 * \param [in]	bLogin		This indicates whether the user successfully logged into the executor.
	 * \return 0 for success, otherwise -1.
	   \remark This is called when the user login to the server with the right username and password. After this event,
	   the executor will send the login data (such as account information, user information etc. ) to the API. When this
	   is done, a new event is raise and the OnExecMsgLoggedin() will be called.
	 */
	virtual int OnExecMsgLogin(BOOL bLogin);
	/**
	 * \brief Callback <tt>int OnExecMsgLoggedin()</tt> when user is fully logged into the system.
	 * \return 0 for success, otherwise -1.
	   \remark This is called when the login process is fully finished. 
	   \sa OnExecMsgLogin
	 */
	virtual int OnExecMsgLoggedin();
	/**
	 * \brief Callback <tt>int OnExecMsgLogout(const GTErrMsg &errmsg)</tt> when logged out.
	 * \return 0 for success, otherwise -1.
	   \remark This is called when logged out.
	 */
	virtual int OnExecMsgLogout(const GTErrMsg &errmsg);

	/*! \brief Callback <tt>int OnExecMsgUser(const GTUser &user)</tt>  when getting GTUser information
		\param [in] user The GTUser information
	 * \return 0 for success, otherwise -1.
		\sa OnExecMsgLogin
	*/
	virtual int OnExecMsgUser(const GTUser &user);
	/*! \brief Callback <tt>int OnExecMsgAccount(const GTAccount &account)</tt> when getting GTAccount information
		\param account The GTAccount information
	 * \return 0 for success, otherwise -1.
		\sa OnExecMsgLogin
	*/
	virtual int OnExecMsgAccount(const GTAccount &account);
	/*! \brief Callback <tt>int OnExecMsgOpenPosition(const GTOpenPosition &open)</tt> when getting GTOpenPosition information.
		\param [in] open The GTOpenPosition information.
	 * \return 0 for success, otherwise -1.

		\remark Use GTStock::OnExecMsgOpenPosition for an individual symbol. In most cases, this need not be overloaded.
	 */
	virtual int OnExecMsgOpenPosition(const GTOpenPosition &open);
	/*! \brief Callback <tt>int OnExecMsgTrade(const GTTrade &trade)</tt> when getting trade information.
		\param [in] trade The GTTrade information.
	 * \return 0 for success, otherwise -1.

		\remark Use GTStock::OnExecMsgTrade for an individual symbol. In most cases, this need not be overloaded.
	*/
	virtual int OnExecMsgTrade(const GTTrade &trade);
	/*! \brief Callback <tt>int OnExecMsgPending(const GTPending &pending)</tt> when getting pending information.
		\param [in] pending The GTPending information.
	 * \return 0 for success, otherwise -1.

		\remark Use GTStock::OnExecMsgPending for an individual symbol. In most cases, this need not be overloaded.
	*/
	virtual int OnExecMsgPending(const GTPending &pending);
	/*! \brief Callback <tt>int OnExecMsgSending(const GTSending &sending)</tt> when getting GTSending information.
		\param [in] sending The GTSending information.
	 * \return 0 for success, otherwise -1.

		\remark Use GTStock::OnExecMsgSending for an individual symbol. In most cases, this need not be overloaded.
	 */
	virtual int OnExecMsgSending(const GTSending &sending);
	/*! \brief Callback <tt>int OnExecMsgCanceling(const GTCancel &cancel)</tt> when getting GTCancel information.
		\param [in] cancel The GTCancel information.
	 * \return 0 for success, otherwise -1.
		\remark Use GTStock::OnExecMsgCanceling for an individual symbol. In most cases, this need not be overloaded.
	*/
	virtual int OnExecMsgCanceling(const GTCancel &cancel);
	/*! \brief Callback <tt>int OnExecMsgCancel(const GTCancel &cancel)</tt> when getting GTCancel information.
		\param [in] cancel The GTCancel information.
	 * \return 0 for success, otherwise -1.

		\remark Use GTStock::OnExecMsgCancel for an individual symbol. In most cases, this need not be overloaded.
	*/
	virtual int OnExecMsgCancel(const GTCancel &cancel);
	/*! \brief Callback <tt>int OnExecMsgReject(const GTReject &reject)</tt> when getting GTReject information.
		\param [in] reject The GTReject information.
	 * \return 0 for success, otherwise -1.
		\remark Use GTStock::OnExecMsgReject for an individual symbol. In most cases, this need not be overloaded.
	*/
	virtual int OnExecMsgReject(const GTReject &reject);
	/*! \brief Callback <tt>int OnExecMsgRemove(const GTRemove &remove)</tt> when getting GTRemove information.
		\param [in] remove The GTRemove information.
	 * \return 0 for success, otherwise -1.
		\remark Use GTStock::OnExecMsgRemove for an individual symbol. In most cases, this need not be overloaded.
	*/
	virtual int OnExecMsgRemove(const GTRemove &remove);
	/*! \brief Callback <tt>int OnExecMsgRejectCancel(const GTRejectCancel &rejectcancel)</tt> when getting GTRejectCancel information.
		\param [in] rejectcancel The GTRejectCancel information.
	 * \return 0 for success, otherwise -1.
		\remark Use GTStock::OnExecMsgRejectCancel for an individual symbol. In most cases, this need not be overloaded.
	*/
	virtual int OnExecMsgRejectCancel(const GTRejectCancel &rejectcancel);
	
	/**
	 * \brief Callback <tt>int OnExecMsgStatus(const GTStatus &status)</tt> when getting status message from executor.
	 * \param [in]	status		The GTStatus that contains the status message.
	 * \return 0 for success, otherwise -1.
	   \remark This is called when the executor sends status message to the API. User needs not to implement this.
	 */
	virtual int OnExecMsgStatus(const GTStatus &status);
//@}
protected:
/*! @name Level2 server callbacks

	These callbacks are related to the Level2 server. API connects to the Level2 server via a TCP/IP connection. 
	All the events generated by the Level2 server will trigger the API to call one of below callbacks. Thus
	one should put his stuff in the implementation of interested callbacks.
*/
//@{
	/*! \brief Callback <tt>int OnGotLevel2Connected()</tt> when level 2 server is connected.
	 * \return 0 for success, otherwise -1.
		\remark This is called when the level 2 server is connected. 
		
		\par
		Level 2 server will be connected automatically by the API. One need not code explicitly to connect to it. 
		\sa OnGotLevel2Disconnected
	*/
	virtual int OnGotLevel2Connected();
	/*! \brief Callback <tt>int OnGotLevel2Disconnected()</tt> when level 2 server is disconnected.
	 * \return 0 for success, otherwise -1.
		\remark Level 2 server will be connected automatically by the API. One need not code explicitly to connect to
		it. In the case the level 2 server cannot be connected, what one should do is just in this callback to
		set another level 2 server addresss and service port by m_setting.SetLevel2Address(), and wait for the API
		connect to the new server automatically. 
		\sa OnGotLevel2Connected
	*/
	virtual int OnGotLevel2Disconnected();
	/*! \brief Callback <tt>int OnGotLevel2Record(GTLevel2 *pRcd)</tt> when getting a data record from the level 2.
		\param [in] pRcd The GTLevel2 data information.
	 * \return 0 for success, otherwise -1.
		\remark Use GTStock::OnGotLevel2Record for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotLevel2Record
	*/
	virtual int OnGotLevel2Record(GTLevel2 *pRcd);
	/*! \brief Callback <tt>int OnGotLevel2Refresh(LPCSTR pszStock)</tt> when getting level 2 refresh command.
		\param [in] pszStock The symbol name for this command.
	 * \return 0 for success, otherwise -1.
		\remark Use GTStock::OnGotLevel2Refresh for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotLevel2Refresh
	*/
	virtual int OnGotLevel2Refresh(LPCSTR pszStock);
	/*! \brief Callback <tt>int OnGotLevel2Display(LPCSTR pszStock)</tt> when getting level 2 display command.
		\param [in] pszStock The symbol name for this command.
	 * \return 0 for success, otherwise -1.
		\remark Use GTStock::OnGotLevel2Display for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotLevel2Display
	*/
	virtual int OnGotLevel2Display(LPCSTR pszStock);
	/*! \brief Callback <tt>int OnGotLevel2Clear(MMID mmid)</tt> when getting level 2 clear command.
		\param [in] mmid The mmid from which the level2 data need clear.
	 * \return 0 for success, otherwise -1.
		\remark Use GTStock::OnGotLevel2Clear for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotLevel2Clear
	*/
	virtual int OnGotLevel2Clear(MMID mmid);
	/*! \brief Callback <tt>int OnGotLevel2Text(GTQuoteText *pRcd)</tt> when getting level 2 text information.
		\param [in] pRcd The GTQuoteText information.
	 * \return 0 for success, otherwise -1.
		\remark Rarely used. Need not implement.
	*/
	virtual int OnGotLevel2Text(GTQuoteText *pRcd);

	/*! \brief Callback <tt>void OnUDPQuoteMessage(int nCode, unsigned short nPort, const char *pszMsg)</tt> when getting UDP quote connection state.
		\param [in] nCode Message code. 1: Startup; 2: Restartup; 3: Stopped; 4: Message.
		\param [in] nPort Interface port number.
		\param [in] pszMsg Error message.
	 * \return no return.
	*/
	virtual void OnUDPQuoteMessage(int nCode, unsigned short nPort, const char *pszMsg);
//@}

protected:
/*! @name Quote server callbacks

	These callbacks are related to the Quote server. API connects to the Quote server via a TCP/IP connection. 
	All the events generated by the Quote server will trigger the API to call one of below callbacks. Thus
	one should put his stuff in the implementation of interested callbacks.
*/
//@{
	/*! \brief Callback <tt>int OnGotQuoteConnected()</tt> when quote server is connected.
	 * \return 0 for success, otherwise -1.
		\remark This is called when the level 2 server is connected. 
		
		\par
		Quote server will be connected automatically by the API. One need not code explicitly to connect to it. 
		\sa OnGotQuoteDisconnected
	*/
	virtual int OnGotQuoteConnected();
	/*! \brief Callback <tt>int OnGotQuoteDisconnected()</tt> when quote server is disconnected.
	 * \return 0 for success, otherwise -1.
		\remark Quote server will be connected automatically by the API. One need not code explicitly to connect to
		it. In the case the quote server cannot be connected, what one should do is just in this callback to
		set another quote server addresss and service port by m_setting.SetQuoteAddress(), and wait for the API
		connect to the new server automatically. 
		\sa OnGotQuoteConnected
	*/
	virtual int OnGotQuoteDisconnected();
	/*! \brief Callback <tt>int OnGotQuoteLevel1(GTLevel1 *pRcd)</tt> when getting a data record from the level 1 server.
		\param [in] pRcd The GTLevel1 data information.
	 * \return 0 for success, otherwise -1.
		\remark Use GTStock::OnGotQuoteLevel1 for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotQuoteLevel1
	*/
	virtual int OnGotQuoteLevel1(GTLevel1 *pRcd);

	/*! \brief Callback <tt>int OnGotQuoteHeartBeat(int reserved)</tt> when getting heartbeat from the level 1 server.
	 * \return 0 for success, otherwise -1.
	*/
	virtual int OnGotQuoteHeartBeat(int reserved);

	/*! \brief Callback <tt>int OnGotQuoteNoii(GTNoii *pRcd)</tt> when getting a data record from the quote server.
		\param [in] pRcd The GTNoii data information.
	 * \return 0 for success, otherwise -1.
		\remark Use GTStock::OnGotQuoteNoii for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotQuoteNoii
	*/
	virtual int OnGotQuoteNoii(GTNoii *pRcd);

	/*! \brief Callback <tt>int OnGotQuoteNYXI(GT_NYXI *pRcd)</tt> when getting a data record from the quote server.
		\param [in] pRcd The GT_NYXI data information.
	 * \return 0 for success, otherwise -1.
		\remark Use GTStock::OnGotQuoteNYXI for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotQuoteNYXI
	*/
	virtual int OnGotQuoteNYXI(GT_NYXI * pRcd);

	/*! \brief Callback <tt>int OnGotQuoteNyseAlert(GTNyseAlert *pRcd)</tt> when getting a data record from the quote server.
		\param [in] pRcd The GTNyseAlert data information.
	 * \return 0 for success, otherwise -1.
		\remark Use GTStock::OnGotQuoteNyseAlert for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotQuoteNyseAlert
	*/
	virtual int OnGotQuoteNyseAlert(GTNyseAlert *pRcd);
	/**
	 * \brief Callback <tt>int OnGotQuotePrint(GTPrint *pRcd)</tt> when getting a print record from the level 1 server.
	 * \param [in] pRcd The print data information (GTPrint).
	 * \return 0 for success, otherwise -1.
	 * \remark This is called when quote server sends a print to the API. 
		\remark Use GTStock::OnGotQuotePrint for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotQuotePrint
	 */
	virtual int OnGotQuotePrint(GTPrint *pRcd);
	/**
	 * \brief Callback <tt>int OnGotQuotePrintHistory(GTPrint *pRcd)</tt> when getting a history print record from the level 1 server.
	 * \param [in] pRcd The print data information (GTPrint).
	 * \return 0 for success, otherwise -1.
	 * \remark This is called when quote server sends a history print to the API. 
		\remark Use GTStock::OnGotQuotePrintHistory for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotQuotePrintHistory
	 */
	virtual int OnGotQuotePrintHistory(GTPrint *pRcd);
	/**
	 * \brief Callback <tt>int OnGotQuotePrintRefresh(GTPrint *pRcd)</tt> when getting a print refresh command from the level 1 server.
	 * \param [in] pRcd The print data information (GTPrint).
	 * \return 0 for success, otherwise -1.
	 * \remark This is called when quote server sends a print refresh command to the API. 
		\remark Use GTStock::OnGotQuotePrintRefresh for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotQuotePrintRefresh
	 */
	virtual int OnGotQuotePrintRefresh(GTPrint *pRcd);
	/**
	 * \brief Callback <tt>int OnGotQuotePrintDisplay(GTPrint *pRcd)</tt> when getting a print display command from the level 1 server.
	 * \param [in] pRcd The print data information (GTPrint).
	 * \return 0 for success, otherwise -1.
	 * \remark This is called when quote server sends a print display command to the API. 
		\remark Use GTStock::OnGotQuotePrintDisplay for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotQuotePrintDisplay
	 */
	virtual int OnGotQuotePrintDisplay(GTPrint *pRcd);
	/*! \brief Callback <tt>int OnGotQuoteText(GTQuoteText *pRcd)</tt> when getting quote text information.
		\param [in] pRcd The GTQuoteText information.
	 * \return 0 for success, otherwise -1.
		\remark Rarely used. Need not implement.
	*/
	virtual int OnGotQuoteText(GTQuoteText *pRcd);
//@}

protected:
/*! @name Chart server callbacks

	These callbacks are related to the Chart server. API connects to the Chart server via a TCP/IP connection. 
	All the events generated by the Chart server will trigger the API to call one of below callbacks. Thus
	one should put his stuff in the implementation of interested callbacks.
*/
//@{
	/*! \brief Callback <tt>int OnGotChartConnected()</tt> when chart server is connected.
	 * \return 0 for success, otherwise -1.
		\remark This is called when the level 2 server is connected. 
		
		\par
		Chart server will be connected automatically by the API. One need not code explicitly to connect to it. 
		\sa OnGotChartDisconnected
	*/
	virtual int OnGotChartConnected();
	/*! \brief Callback <tt>int OnGotChartDisconnected()</tt> when chart server is disconnected.
	 * \return 0 for success, otherwise -1.
		\remark Chart server will be connected automatically by the API. One need not code explicitly to connect to
		it. In the case the chart server cannot be connected, what one should do is just in this callback to
		set another quote server addresss and service port by m_setting.SetChartAddress(), and wait for the API
		connect to the new server automatically. 
		\sa OnGotChartConnected
	*/
	virtual int OnGotChartDisconnected();
	/**
	 * \brief Callback <tt>int OnGotChartRecord(GTChart *pRcd)</tt> when getting a chart record from the chart server.
	 * \param [in] pRcd The chart data information (GTChart).
	 * \return 0 for success, otherwise -1.
	 * \remark This is called when chart server sends a chart to the API. 
		\remark Use GTStock::OnGotChartRecord for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotChartRecord
	 */
	virtual int OnGotChartRecord(GTChart *pRcd);
	/**
	 * \brief Callback <tt>int OnGotChartRecordHistory(GTChart *pRcd)</tt> when getting a chart history record from the chart server.
	 * \param [in] pRcd The chart data information (GTChart).
	 * \return 0 for success, otherwise -1.
	 * \remark This is called when chart server sends a historical chart to the API. 
		\remark Use GTStock::OnGotChartRecordHistory for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotChartRecordHistory
	 */
	virtual int OnGotChartRecordHistory(GTChart *pRcd);
	/**
	 * \brief Callback <tt>int OnGotChartRecordRefresh(GTChart *pRcd)</tt> when getting a chart refresh command from the chart server.
	 * \param [in] pRcd The chart data information (GTChart).
	 * \return 0 for success, otherwise -1.
	 * \remark This is called when chart server sends a chart refresh command to the API. 
		\remark Use GTStock::OnGotChartRecordRefresh for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotChartRecordRefresh
	 */
	virtual int OnGotChartRecordRefresh(GTChart *pRcd);
	/**
	 * \brief Callback <tt>int OnGotChartRecordDisplay(GTChart *pRcd)</tt> when getting a chart display command from the chart server.
	 * \param [in] pRcd The chart data information (GTChart).
	 * \return 0 for success, otherwise -1.
	 * \remark This is called when chart server sends a chart display command to the API. 
		\remark Use GTStock::OnGotChartRecordDisplay for an individual symbol. In most cases, this need not be overloaded.
		\sa GTStock::OnGotChartRecordDisplay
	 */
	virtual int OnGotChartRecordDisplay(GTChart *pRcd);
//@}

public:
	/*! \brief Callback <tt>int OnSendingOrder(const GTSending &gtsending)</tt> when ready to send an order.
		\param [in] gtsending The GTSending information.
	 *  \return 0 for success, otherwise -1.

		\remark Use GTStock::OnSendingOrder for an individual symbol. In most cases, this need not be overloaded.
	 */
	virtual int OnSendingOrder(const GTSending &gtsending);

protected:
	//!\ brief The size of BMP used by heartbeat
	CSize	m_nBmpSize;
	//!\ brief The BMPs used by heartbeat
	CBitmap	m_bmpSessionState[6];

	int	 m_nSessionStateCounter;
	int	 m_nSessionStateCurrent;

	//! \brief Draw the heartbeat window. Don't call it.
	void DrawSessionState(CDC &dc);
	//! \brief Initialize the heartbeat window. Don't call it.
	int  InitSessionState();

protected:
	/**
	 * \brief Callback <tt>int OnExecCmd(LPCSTR pszCmd)</tt> when a command is sent in.
	 * \param [in] pszCmd The command
	 * \return 0 for success, otherwise -1.
	 * \remark Internal used. Debugging
	 */
	virtual int OnExecCmd(LPCSTR pszCmd);
	/**
	 * \brief Callback <tt>int OnExecSessionCmd(LPCSTR pszCmd)</tt> when a command is sent in.
	 * \param [in] pszCmd The command
	 * \return 0 for success, otherwise -1.
	 * \remark Internal used. Debugging
	 */
	virtual int OnExecSessionCmd(LPCSTR pszCmd);

/*! @name Windows event handler
*/
//@{
//! \brief Operations
public:
	//{{AFX_VIRTUAL(GTSession)
	virtual void PreSubclassWindow();
	//}}AFX_VIRTUAL
//@}

// Implementation
public:
	/**
	 * \brief Create the session window
	 * \param [in] pParentWnd The parent window handler, can be NULL.
	 * \param [in] nID	The resource ID for the session window. Can be 0.
	 * \return Nonzero if successful; otherwise 0.
	 * \remark This creates the session window. The parameter \a nID is optional, which can be 0 if you don't want to specify. If you want to
		assign an ID for the created session window, make sure the ID you input will NOT duplicate with all existed controls. This distincts from 
		gtSubClassSessionWindow by that, this function created a NEW session window control, while the calling of gtSubClassSessionWindow will just
		attach the session window to the exist custom control. 

	   \par
		After creation, the window will be located to the left-top corner by default. If one want to place it to some other place, one can using 
		the window position functions such as SetWindowPlacement to move it to the right place.

	   \par
		Call DestroyWindow if you want to destory the created session window.
	 */
	virtual BOOL Create(CWnd* pParentWnd, UINT nID);
	// Generated message map functions
protected:
/*! @name Windows event handler
	Don't overload these. 
*/
//@{
	//! \brief Windows generated code. Internal used.
	//{{AFX_MSG(GTSession)
	afx_msg void OnPaint();
	afx_msg LRESULT OnMsgQueue(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGotExec(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGotHealth(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGotHeartBeat(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGotQuote(WPARAM wParam, LPARAM lParam);
//	afx_msg LRESULT OnGotChain(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGotChart(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGotLevel2(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnUDPMsg(WPARAM wParam, LPARAM lParam);
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnDestroy();
	afx_msg void OnTimer(UINT nIDEvent);
	afx_msg void OnGetMinMaxInfo(MINMAXINFO FAR* lpMMI);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

	LRESULT SendMessageQuote(UINT message, WPARAM wParam, LPARAM lParam);
	LRESULT SendMessageChart(UINT message, WPARAM wParam, LPARAM lParam);
	LRESULT SendMessageLevel2(UINT message, WPARAM wParam, LPARAM lParam);
	LRESULT SendMessageExec(UINT message, WPARAM wParam, LPARAM lParam);
//@}
public:
	static DWORD m_dwReqVer;

	/**
	 * \brief Initialize the API.
	 * \param [in] dwReqVer Required API version number.
	 * \param [in] hInstance The application instant handler. Can be NULL.
	 * \param [in] dwFlags The reserved flag. Should be 0.
	 * \return 0 for success, otherwise -1. If -1, one can not use the API to go further, instead, the program should quit.

	 \remark This should be called in the starting of the program to initialize the API.
	 Notice this is static method, calling this has no priority on the constructor of GTSession. One can call this 
	 either before GTSession object is created or after that. 

	 \par
	 But in principal, one should call this before GTSession creation. This makes sure the version
	 API you used is the right one. 

	 \sa Uninitialize
	 */
	static int Initialize(DWORD dwReqVer, HINSTANCE hInstance = NULL, DWORD dwFlags = 0);

	/*! \brief Uninitialize the API.
		\return 0 for success, otherwise -1.

		\remark If Initialize() is called successfully, call this before quit the program. It cleans up the 
		program running environment.
		\sa Initialize.
	*/
	static int Uninitialize();

	/*! \brief Get the error message for the nCode.
	 * \param [in] nCode The error code.
	 * \param [in] pszMsg The message string buffer pointer.
	 * \return The message string pointer.
	 */
	static LPCSTR GetErrorMessage(int nCode, LPSTR pszMsg = NULL);

protected:
	/*! \brief Register the window class for the program.
		\param [in] hInstance The instance handler.
	*/
	static int RegisterWindowClass(HINSTANCE hInstance);
};

//{{AFX_INSERT_LOCATION}}

#endif // !defined(__GTSESSION_H__)

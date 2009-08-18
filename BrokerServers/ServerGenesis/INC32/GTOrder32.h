/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTOrder32.h
	\brief interface for the GTOrder32 class.
 */

#ifndef __GTORDER32_H__
#define __GTORDER32_H__

#include "..\Inc32\GTConst.h"
#include "..\Inc32\MMID.h"
#include "..\Inc\SoesTif.h"

#pragma pack(8)
/*! \struct GTOrder32_CreditSuisse_Type
	\brief The special order instruction for orders sent to Credit Suisse. The same as tagGTOrder32_CreditSuisse_Type.

	\copydoc tagGTOrder32_CreditSuisse_Type
*/
/*! \typedef typedef tagGTOrder32_CreditSuisse_Type GTOrder32_CreditSuisse_Type
*/
/*! \struct GTOrder32_CreditSuisse_Type
	\brief The special order instruction for orders sent to Credit Suisse.
*/
typedef struct tagGTOrder32_CreditSuisse_Type
{
		/*! \brief	Product Code

		Production code for Credit Suisse Dark Pools. 
		Valid value: "4A" - CROSSFINDER+
		*/
		char	szProductCode[3];

		/*! \brief Execution Style

		Editable. Can be:
		- 0: Normal Cross
		- 1: Aggressive Cross
		- 2: Passive Cross		
		*/
		int		nExecStyle;
	
		//! \brief	Reserved
		int		nDisplaySize;

		//! \brief	Reserved
		int		nMinPctVol;

		//! \brief	Reserved
		char	szTrigger[10];

		//! \brief	Reserved
		int		nStarttime;

		//! \brief	Reserved
		int		nEndtime;
}GTOrder32_CreditSuisse_Type;

/*! \ingroup c
*/
/*! \struct GTOrder32
	\brief The order structure. The same as tagGTOrder32.

	\copydoc tagGTOrder32
*/
/*! \typedef typedef tagGTOrder32 GTOrder32
*/
/*! \struct tagGTOrder32
	\brief The order structure

	Fill out the struct and use GTStock.PlaceOrder() to place an order.

	Some basic information of orders is included, such as share number, order side (bid/buy or ask/sell) and so on.
	Type of order is defined by <see cref="chPriceIndicator"/>. For different types there are two fields: <see cref="dblPrice"/>
	and <see cref="dblStopLimitPrice"/> can be used to indicate the order price (per share). Some simple pricing schemes just use
	dblPrice, such as limit order and market order. Some pricing schemes have to use both, such as  stop-limit order,
	which uses <see cref="dblPrice"/> to specify the limit price, while uses <see cref="dblStopLimitPrice"/> to define the stop
	price.
	
	For listed symbols, some methods ask for the place to be filled (they need to know where to execute the order.) In such situations, 
	the place should be filled by the exchange's MMID. An example is to trade listed symbol by METHOD_MLYN or METHOD_BELZ, the place should 
	be filled by MMID_NYSE or MMID_AMEX. If you did not fill it, the order will be rejected and an error message "could not get exchange name' 
	will be fired.

	dwTimeInForce is used to define the life of the order. But for most exchange/ECN, there's a problem to
	synchronize the time. In our system, now only ISLD can support this seamless. For other places, the limit orders with dwTimeInForce
	will be treated as day order.
	
	Many options are used only in Supermontage. Please call them or our trading supports for the details.
	
	There are two user data fields, which might be useful for strategy designing. The most asked question that used with
	those user data is: Is it possible to know either the Ticket number or the TraderSeqNo of an order when I get one order status changed? 
	Following steps might be used: 
		- 1. fill the lpUserData or dwUserData by some identification information, then PlaceOrder;
		- 2. In the callback GTStock.OnSendingOrder(), record the dwTraderSeqNo according to the dwUserData or lpUserData. This callback will be called before the order sends to the executor, the API will generate a session unique dwTraderSeqNo to each order;
		- 3. In the callback GTStock.OnExecMsgPending(), record the dwTraderSeqNo and corresponding dwTicketNo. This callback is called when server acknowledge that an order is sending out, by that time the server has filled in the ticket no;
		- 4. backtracing to find out which dwTicketNo is used in which order.

	\par Example:
	[C version] Following code shows how to fill the GTOrder32 to make a bid.
	\code
		GTOrder32 order;
		LPGTSTOCK hStock = gtGetStock(hSession, "LU");
		gtInitOrder(hStock, &order);
		gtBid(hStock, order, 100, 10, METHOD_BELZ);
		// Change order options here
		order.dwTimeInForce = 3;	// expire in 10 seconds
		order.dwUserData = m_seqno; m_seqno++;
		order.place = MMID_NYSE;
		order.chPriceIndicator = '1';
		gtPlaceOrder(hStock, order);
	\endcode

	\par
	[C++ Version] Following code shows how to fill the GTOrder32 to make a bid.
	\code
		GTStock *pStock = m_session.GetStock(m_strStock);
		if(pStock == NULL)
			return;

		GTOrder order = pStock->m_defOrder;

		pStock->Bid(order, 100, pStock->m_level2.GetBestBidPrice(), METHOD_ISLD);

		// Change order options here, for example
		order.place = MMID_NYSE;

		pStock->PlaceOrder(order);
	\endcode

	\par
	[C++ Version] Another example, sending a stop order:
	\code
		//trade LU. Following codes are in a callback of the subclass of GTStock
		GTOrder order = m_defOrder;        //initialize the order struct;
		PlaceOrder(order, 'S', 100, dblprice, METHOD_BELZ, TIF_DAY);
		order.place=MMID_NYSE;
		order.chPriceIndicator='3';        //Set the Stop order;
		PlaceOrder(order);
	\endcode
*/
typedef struct tagGTOrder32
{
	//! \brief	AccountID
	/*!
		This will be filled automatically by API. Never chenge it in client code.
	*/
	char	szAccountID[LEN_ACCOUNT_ID + 1];

	//! \brief	Stock name
	/*!
		This will be filled automatically by API. Never change it in client code.
	*/
	char	szStock[LEN_STOCK + 1];

	//! \brief	Order side
	/*!
		Editable. Can be 'B' for bid/buy or 'S' for ask/sell.
	*/
	char	chSide;

	//! \brief	Number of share
	/*!
		Editable. The number of shares that you want to buy/sell
	*/
	long	dwShare;

	//! \brief	Reserved
	/*!
	*/
	long	dwMinimum;
	//! \brief	Maximum share to show on the book
	/*!
		Editable. Some ECN/exchange support to display only part of the order size. 
		This defines the maximum size to be shown.
	*/
	long	dwMaxFloor;

	//! \brief	Time in force
	/*!	\anchor dwTimeInForce
		Editable. How long the order will stay alive. in second. 

		Two predefined time:
		- TIF_IOC:	Time in force is 0. For IOC order;
		- TIF_DAY:	Alive for this trading day.

		INET accepted the following additional TIF:
		- TIF_MGTC: 99960 Market Hours Good-Til-Canceled.
		- TIF_SGTC: 99964 System Hours Good-Til-Canceled (GTC).
		- TIF_99994: 99994 For some routing strategies, this TIF designates that the order be rerouted every four minutes or so.
		- TIF_MDAY: 99998 Market day.
		- TIF_SDAY: 99999 NASDAQ Day.
	*/
	long	dwTimeInForce;

	//! \brief	Price indicator
	/*!	\anchor chPriceIndicator
		Editable. Which kind of order is this. 

		Thirteen kinds are available:
		<TABLE class="dtTABLE" cellspacing="0">
		<TR><TH>VALUE</TH><TH>MACRO</TH><TH>DESCRIPTION</TH></TR>
		<TR><TD>'1'</TD><TD>PRICE_INDICATOR_MARKET</TD><TD>Market</TD></TR>
		<TR><TD>'2'</TD><TD>PRICE_INDICATOR_LIMIT</TD><TD>Limit</TD></TR>
		<TR><TD>'3'</TD><TD>PRICE_INDICATOR_STOP</TD><TD>Stop</TD></TR>
		<TR><TD>'4'</TD><TD>PRICE_INDICATOR_STOPLIMIT</TD><TD>Stop-limit</TD></TR>
		<TR><TD>'A'</TD><TD>PRICE_INDICATOR_MARKET_ON_OPEN</TD><TD>MARKET_ON_OPEN order</TD></TR>
		<TR><TD>'B'</TD><TD>PRICE_INDICATOR_MARKET_ON_CLOSE</TD><TD>MARKET_ON_CLOSE order</TD></TR>
		<TR><TD>'C'</TD><TD>PRICE_INDICATOR_LIMIT_ON_OPEN</TD><TD>LIMIT_ON_OPEN order</TD></TR>
		<TR><TD>'D'</TD><TD>PRICE_INDICATOR_LIMIT_ON_CLOSE</TD><TD>LIMIT_ON_CLOSE order</TD></TR>
		<TR><TD>'E'</TD><TD>PRICE_INDICATOR_IO_OPEN</TD><TD>Imbalance-Only Open order</TD></TR>
		<TR><TD>'F'</TD><TD>PRICE_INDICATOR_IO_CLOSE</TD><TD>Imbalance-Only Close order</TD></TR>
		<TR><TD>'G'</TD><TD>PRICE_INDICATOR_IPC_NXT</TD><TD>Intraday crossing order - IOC</TD></TR>
		<TR><TD>'H'</TD><TD>PRICE_INDICATOR_IPC_REG</TD><TD>Intraday crossing order - REG</TD></TR>
		<TR><TD>'I'</TD><TD>PRICE_INDICATOR_IPC_ALX</TD><TD>Intraday crossing order - ALX</TD></TR>
		</TABLE>

		Notice:
		 - it is ASCII '1' instead of integer 1, and so on.
		 - for the MARKET_ON_OPEN, MARKET_ON_CLOSE, LIMIT_ON_OPEN, LIMIT_ON_CLOSE orders, now only BELZ, INET 
		 and MLYN support them. For the last five kinds of orders, now only ISLD support them. For other methods,
		 you should consult them before trying. There are also no further information about those orders that 
		 we can provide on. For questions like when to send or why some got no execution,
		 please send to BELZ or MLYN. 
		 - Intraday crossing order - IOC, next cross only.
		 - Intraday crossing order - REG, next cross and all subsequent Intraday Crosses until 4 p.m.
		 - Intraday crossing order - ALX, next cross and all subsequent Intraday Crosses, including Post-Close at 4:30 p.m.
	*/
	char	chPriceIndicator;		// '1':Market, '2':Limit, '3':Stop, '4':Stop-limit

	//! \brief	Price of the order.
	/*!	\anchor dblPrice
		Editable. It is the price for one share:
		
		<TABLE class="dtTABLE" cellspacing="0">
		<TR><TH width="50%">TYPE</TH><TH width="50%">DESCRIPTION</TH></TR>
		<TR><TD>Market order</TD><TD>this will be 0. Any value you fill will be erased;</TD></TR>
		<TR><TD>Limit order</TD><TD>it is the limit price;</TD></TR>
		<TR><TD>Stop order</TD><TD>it is the stop price;</TD></TR>
		<TR><TD>Stop-Limit order</TD><TD>it is the limit price, and the dblStopLimitPrice will be the stop price.</TD></TR>
		<TR><TD>MARKET_ON_OPEN order</TD><TD>The market price to be executed at market open;</TD></TR>
		<TR><TD>MARKET_ON_CLOSE order</TD><TD>The market price to be executed at market close;</TD></TR>
		<TR><TD>LIMIT_ON_OPEN order</TD><TD>The limit price to be executed at market open;</TD></TR>
		<TR><TD>LIMIT_ON_CLOSE order</TD><TD>The limit price to be executed at market close;</TD></TR>
		<TR><TD>IO_OPEN order</TD><TD>The limit price to be executed when crossing at market open;</TD></TR>
		<TR><TD>IO_CLOSE order</TD><TD>The limit price to be executed when crossing at market close;</TD></TR>
		<TR><TD>IPC_NXT order</TD><TD>The limit price to be executed when intraday crossing, IOC;</TD></TR>
		<TR><TD>IPC_REG order</TD><TD>The limit price to be executed when intraday crossing, REG;</TD></TR>
		<TR><TD>IPC_ALX order</TD><TD>The limit price to be executed when intraday crossing, ALX;</TD></TR>
		</TABLE>

	*/
	double	dblPrice;				// per Share

	//! \brief	Stop limit price for Stop-limit order.
	/*!	\anchor dblStopLimitPrice
		Editable. It is the price for one share. Only valid for stop-Limit order
	*/
	double	dblStopLimitPrice;

	/*! \brief Display this order?
		\anchor chDisplay
		
		Editable. Can be:
		- 'Y': Anonymous-Price to Display.
		- 'N': Non-Displayed.
		- 'A': Attributable- Price to Display.
		- 'C': Anonymous ?Price to Comply.
		- 'I': Imbalance Only.	
		Now only ISLD supports this. If 'N', this order will not
		show on any book or level 2. (Refer as hidden order)
	*/
	char	chDisplay;

	/*! \brief (Supermontage) Capacity

		Editable. Can be:
		- 'A': agent capacity
		- 'P': principal capacity
		- 'R': riskless capacity
	*/
	char	chCapacity;				//'A' 

	//! \brief	method to send order
	/*!
		Editable. method used to send this order. \sa MMID.h for available METHODIDs
	*/
	MMID	method;

	//! \brief	place to execute
	/*!
		Editable. Some order has to specify the place to execute, especially for
		listed symbols. For example, when using Belzberg to send listed symbol,
		we need to set
		\code
			order.method = METHOD_BELZ;
			order.place = MMID_NYSE;
		\endcode
		\sa MMID.h for available MMIDs
	*/
	MMID	place;


	/*! \brief (Supermontage) Auto Ex
		
		Editable. TRUE or FALSE.
	*/
	BOOL	bSoesHitECN;			// SOES Hit ECN

//@{
	//! \brief ISLD autoroute
	/*!
		Editable. Enable/disable ISLD autorouting. Refer to \ref autoroute
	*/
	BOOL	bIsldAutoRoute;
	//! \brief ARCA autoroute
	/*!
		Editable. Enable/disable ARCA autorouting. Refer to \ref autoroute
	*/
	BOOL	bArcaAutoRoute;
//@}

/*
	Refer to our laser program 'Trade'->'Supermontage Trade Options'.
*/
//@{
	/*! \brief (SuperMortage options) Dealer ID

		Editable. Using MMID_UNKNOWN.
	*/
	MMID	mpid;					// GNDS GNSE -> usually MMID_UNKNOWN

	/*! \brief (SuperMortage options) Can order be filled with those from the same dealer?

		Editable. Can be one of:
		- 'N': Never internalize
		- 'I': Always internalize
		- 'Y': Match internalize
	*/
	char	chInternalize;			// N I Y

	/*! \brief (SuperMortage options) Is this a discrenionary order?

		Editable. Can be one of:
		- 'N': No
		- 'Y': Yes

		If yes, the nDiscrOffset will defined the offset.
	*/
	char	chDiscretionary;		// Y N

	/*! \brief (SuperMortage options) The offset of the discrenionary order. 

		Editable. In cents. Can be an integer in range [1-99]. Valid when chDiscretionary = 'Y'.
	*/
	int		nDiscrOffset;

	/*! \brief This field is abandoned in Laser Trading System

		This field is abandoned in Laser Trading System.
	*/
	char	chSoesTif;				// SOES_TIF_???

	/*! \brief This field is abandoned in Laser Trading System
		
		This field is abandoned in Laser Trading System.
	*/
	char	chImbalance;

	/*! \brief (SuperMortage options) Pegged order?

		Editable. Can be one of:
		<TABLE class="dtTABLE" cellspacing="0">
		<TR><TH width="50%">CODE</TH><TH width="50%"> DESCRIPTION		</TH></TR>
		<TR><TD>'G' </TD><TD> Primary pegged order</TD></TR>
		<TR><TD>'R' </TD><TD> Market pegged order</TD></TR>
		<TR><TD>'M' </TD><TD> Mid-point pegged order</TD></TR>
		<TR><TD>'N' </TD><TD> Not pegged order</TD></TR>
		</TABLE>

	*/
	char	chPegged;				// G R M N

	/*! \brief This field is abandoned in Laser Trading System
		
		This field is abandoned in Laser Trading System.
	*/
	BOOL	bPegCap;

	/*! \brief (SuperMortage options) Pegged order option: Cap price
		
		Editable. If the cap price is zero, no cap for the pegged order, 
		otherwise the price is the cap for the pegged order.
	*/
	double	dblPegCap;

	/*! \brief (SuperMortage options) Pegged order option: Peg offset
		
		Editable. In cents.
	*/
	int		nPegOffset;

	/*! \brief (IOI options) IOI order option: IOI ID
		
		Editable.
	*/
	int		nIOIid;

	/*! \brief INET Routing strategy
		
		Editable.

		Valid values:
			INET_RS_INET, INET_RS_DOTN, INET_RS_DOTA, INET_RS_DOTM, INET_RS_DOTO, INET_RS_DOTP,
			INET_RS_DOTI, INET_RS_DOTD, INET_RS_SPDY, INET_RS_STGY, INET_RS_SCAN, INET_RS_SWIM			
	*/
	MMID		mmidRoutingStrategy;
//@}

//@{
	/*! \union freetext. 		
		\brief Freetext for special order instruction. 
		
		Freetext for special order instruction. 
	*/
	union
	{
		GTOrder32_CreditSuisse_Type crss;
	}freetext;

	/*! \brief Reserved
		
		Reserved.
	*/
	char	chReserved[20];
//@}
	
	/*! \brief Staying time before being canceled
		
		In second. Used with GTStock::m_staying. This value will be kept in
		GTStock::m_staying. 
	*/
	int		nStayingTime;			// Staying time before being canceled

	/*! \brief User data
		
		Editable. User data. 
	*/
	DWORD	dwUserData;				// User Data

	/*! \brief Another user data as pointer
		
		Editable. User data. 
	*/
	LPVOID	lpUserData;				// User Data 2
}GTOrder32;
#pragma pack()

#endif//__GTORDER32_H__

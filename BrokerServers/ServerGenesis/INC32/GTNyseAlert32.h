/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTNyseAlert32.h
	\brief interface for the GTNyseAlert32 class.
 */

#ifndef __GTNYSEALERT32_H__
#define __GTNYSEALERT32_H__

#include "GTConst.h"
#include "GTime32.h"


#define NYSEALERT_UNKNOWN							0	

#define NYSEALERT_TYPESEQUENCENUMBERRESET			1	
#define NYSEALERT_TYPEHEARTBEAT						2	
#define NYSEALERT_TYPEADMIN							3	
#define NYSEALERT_TYPENODATAAVAILABLE				4	
#define NYSEALERT_TYPEPACKETUNAVAILABLE				5	
#define NYSEALERT_TYPERETRANSMISSIONREQUEST			20	
#define NYSEALERT_TYPEREQUESTACKNOWLEDGEMENT		30	
#define NYSEALERT_TYPEINVALIDREQUEST				31	

#define NYSEALERT_TYPEMARKETIMBALANCE				130	
#define NYSEALERT_TYPEOPENINGDELAYS_TRADINGHALTS	131	
#define NYSEALERT_TYPENYSEINDICATIONS				132	
#define NYSEALERT_TYPETRADEDISSEMINATIONTIME		133	
#define NYSEALERT_TYPETRADINGCOLLAR 				134	
#define NYSEALERT_TYPECIRCUITBREAKER				135	

#pragma pack(8)
/*! \ingroup c
*/

typedef struct NyseAlert_Header_Type 
{	
	//! \brief	Stock name
	char			szStock[LEN_STOCK + 1];

	/*! \brief	Financial Status\n
	0: "None"\n
	1: Bankrupt\n
	2: Pending Delisting\n
	3: Bankrupt and Pending Delisting\n
	*/
	unsigned char	chFinancialStatus;

	/*! \brief	Corporate Action\n
	0: "None"\n
	1: Ex-Dividend\n
	2: Ex-Distribution\n
	4: Ex-Rights\n
	8: New\n
	16: Ex-Interest\n
	*/
	unsigned char	chCorporateAction;

	/*! \brief	Security Status\n
	'1': Opening Delay\n
	'2': Trading Halt\n
	'3': Resume\n
	'4': No open/no resume\n
	'5': Price Indication\n
	'6': Trading Range Indication\n
	'7': Market Imbalance Buy\n
	'8': Market Imbalance Sell\n
	'9': Market On Close Imbalance Buy\n
	'A': Market On Close Imbalance Sell\n
	'C': No Market Imbalance\n
	'D': No Market On Close Imbalance\n
	'T': Trade Dissemination Time\n
	*/
	unsigned char	chSecurityStatus;
}NyseAlert_Header_Type;

typedef struct NyseAlert_Imbalance_Type
{
	//! \brief	Message Header
	NyseAlert_Header_Type hdr;

	//! \brief	Buy Volume
	unsigned int	nBuyVolume;

	//! \brief	Sell Volume
	unsigned int	nSellVolume;

	//! \brief	Adjustment
	unsigned char	chAdjustment;
}NyseAlert_Imbalance_Type;

typedef struct NyseAlert_Halts_Type 
{
	//! \brief	Message Header
	NyseAlert_Header_Type hdr;

	/*! \brief	Halt Condition\n
	'D': News dissemination\n
	'E': Order influx\n
	'I': Order imbalance\n
	'J': Due to related security news dissemination\n
	'K': Due to related security news pending\n
	'M': Additional information\n
	'P': News pending\n
	'Q': Due to related security\n
	'T': Resume\n
	'V': In view of common\n
	'X': Equipment changeover\n
	'Z': No open/No resume\n
	*/
	unsigned char	chHaltCondition;
}NyseAlert_Halts_Type;

typedef struct NyseAlert_Indication_Type 
{
	//! \brief	Message Header
	NyseAlert_Header_Type hdr;

	//! \brief	Ask Price
	double			dblAskPriceNumer;

	//! \brief	Bid Price
	double			dblBidPriceNumer;

	/*! \brief	Adjustment\n
	0: "None"\n
	1: Cancel\n
	2: Correction\n
	*/
	char			chAdjustment;
}NyseAlert_Indication_Type;

typedef struct NyseAlert_TradeTime_Type
{
	//! \brief	Message Header
	NyseAlert_Header_Type hdr;
}NyseAlert_TradeTime_Type;

typedef struct NyseAlert_AdminMsg_Type
{
	//! \brief	Message text
	char  szText[0x50];				
}NyseAlert_AdminMsg_Type;

/*! \struct GTNyseAlert32
	\brief Alias of tagGTNyseAlert32.

	\copydoc tagGTNyseAlert32
*/
/*! \typedef typedef tagGTNyseAlert32 GTNyseAlert32
*/
/*! \struct tagGTNyseAlert32
	\brief The NYSEALERT information

	The NYSEALERT information is enclosed in this structure. 

	Always this is updated when a NYSEALERT sent in. 

	When a new NYSEALERT record comes in, OnGotNyseAlertRecord callback is called.
	\sa GTSession::OnGotQuoteNyseAlert GTStock::OnGotQuoteNyseAlert  GTAPI_MSG_SESSION_OnGotQuoteNyseAlert  GTAPI_MSG_STOCK_OnGotQuoteNyseAlert  
*/	
typedef struct tagGTNyseAlert32
{
#if defined(__cplusplus)
	typedef NyseAlert_Header_Type		Header;
	typedef NyseAlert_Imbalance_Type	Imbalance;
	typedef NyseAlert_Halts_Type		Halts;	
	typedef NyseAlert_Indication_Type	Indication;
	typedef NyseAlert_TradeTime_Type	TradeTime;	
	typedef NyseAlert_AdminMsg_Type		AdminMsg;
#endif

	//! \brief	Product Type
	unsigned char chProductID;
	
	//! \brief	Message Type
	unsigned char chMsgType;
	
	//! \brief	When this NYSEALERT information was generated (By ECN/Exchange)
	GTime32		  gtime;

	union 
	{
		NyseAlert_Imbalance_Type	imbalance;
		NyseAlert_Halts_Type		halts;
		NyseAlert_Indication_Type	indication;
		NyseAlert_TradeTime_Type	trade;
		NyseAlert_AdminMsg_Type		admin;
	};
}GTNyseAlert32;
#pragma pack()

#endif//__GTNYSEALERT32_H__

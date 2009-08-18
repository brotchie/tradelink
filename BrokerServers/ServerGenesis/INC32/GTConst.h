/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

#ifndef __GTCONST_H__
#define __GTCONST_H__

/////////////////////////////////////////////////////////////////////////

#define LEN_ACCOUNT_ID		16
#define LEN_ACCOUNT_CODE	16
#define LEN_RECONCILE_ID	16
#define LEN_GROUP_ID		16
#define LEN_ACCOUNT_NAME	32
#define LEN_USER_ID			16
#define LEN_USERNAME		16
#define LEN_ENTID			8
#define LEN_PASSWORD		16
#define LEN_STOCK			11
#define LEN_COMPUTER_NAME	32
#define LEN_IP_ADDRESS		16
#define LEN_REFNO			35
#define LEN_COMPANY			50
#define LEN_UNDERLY			6

#define LEN_HASH_SALT		8
#define LEN_HASH_METHOD		8
#define LEN_HASH_VALUE		64

/////////////////////////////////////////////////////////////////////////

#define ORDER_RCD_ERROR_0	1000

#define ADMIN_CMD_0			2000
#define ADMIN_RCD_0			2200
#define ADMIN_RCD_ERROR_0	2500

#define TRADER_CMD_0		3000
#define TRADER_RCD_0		3200
#define TRADER_RCD_ERROR_0	3500

// Price Indicator
#define PRICE_INDICATOR_MARKET			'1'
#define PRICE_INDICATOR_LIMIT			'2'
#define PRICE_INDICATOR_STOP			'3'
#define PRICE_INDICATOR_STOPLIMIT		'4'
#define PRICE_INDICATOR_LIMIT_OR_BETTER '7'
#define PRICE_INDICATOR_PEGGED			'P'

#define PRICE_INDICATOR_MARKET_ON_OPEN	'A'
#define PRICE_INDICATOR_MARKET_ON_CLOSE	'B'
#define PRICE_INDICATOR_LIMIT_ON_OPEN	'C'
#define PRICE_INDICATOR_LIMIT_ON_CLOSE	'D'

#define PRICE_INDICATOR_IO_OPEN			'E'
#define PRICE_INDICATOR_IO_CLOSE		'F'

// NASDAQ Crossing Network - Intraday & Post-Close (NXT, REG, ALX)
// Intraday crossing order - IOC, next cross only
#define	PRICE_INDICATOR_IPC_NXT			'G'
// Intraday crossing order - REG, Next cross and all subsequent Intraday Crosses until 4 p.m.
#define	PRICE_INDICATOR_IPC_REG			'H'
// Intraday crossing order - ALX, Next cross and all subsequent Intraday Crosses, including Post-Close at 4:30 p.m.
#define	PRICE_INDICATOR_IPC_ALX			'I'

//Pegged order type
#define TRADER_PEGGED_REGULAR		'G'
#define TRADER_PEGGED_PRIMARY		TRADER_PEGGED_REGULAR

#define TRADER_PEGGED_REVERSE		'R'
#define TRADER_PEGGED_MARKET		TRADER_PEGGED_REVERSE

#define TRADER_PEGGED_MIDPOINT		'M'
#define TRADER_PEGGED_NONE			'N'

#define ORDER_TYPE_MARKET				0
#define ORDER_TYPE_LIMIT				1
#define ORDER_TYPE_STOP					2
#define ORDER_TYPE_STOP_LIMIT			3
#define ORDER_TYPE_MARKET_ON_OPEN		4
#define ORDER_TYPE_MARKET_ON_CLOSE		5
#define ORDER_TYPE_LIMIT_ON_OPEN		6
#define ORDER_TYPE_LIMIT_ON_CLOSE		7
#define ORDER_TYPE_IO_OPEN				8
#define ORDER_TYPE_IO_CLOSE				9
#define ORDER_TYPE_IPC_NXT				10
#define ORDER_TYPE_IPC_REG				11
#define ORDER_TYPE_IPC_ALX				12

#define ORDER_TYPE_NAME_MARKET			"MKT"
//#define ORDER_TYPE_NAME_LIMIT			"LIMIT"
#define ORDER_TYPE_NAME_LIMIT			"Limit"
#define ORDER_TYPE_NAME_STOP			"Stop"
#define ORDER_TYPE_NAME_STOP_LIMIT		"StopL"
#define ORDER_TYPE_NAME_MARKET_ON_OPEN	"MOO"
#define ORDER_TYPE_NAME_MARKET_ON_CLOSE	"MOC"
#define ORDER_TYPE_NAME_LIMIT_ON_OPEN	"LOO"
#define ORDER_TYPE_NAME_LIMIT_ON_CLOSE	"LOC"
#define ORDER_TYPE_NAME_IO_OPEN			"IOO"
#define ORDER_TYPE_NAME_IO_CLOSE		"IOC"
#define ORDER_TYPE_NAME_IPC_NXT			"IPC-NXT"
#define ORDER_TYPE_NAME_IPC_REG			"IPC-REG"
#define ORDER_TYPE_NAME_IPC_ALX			"IPC-ALX"

//jason
#define TIF_TYPE_IOC					0
#define TIF_TYPE_DAY					1


#define TIF_NAME_DAY					"DAY"
#define TIF_NAME_IOC					"IOC"
//end
// Canceler
#define CANCELER_UNKNOWN	0
#define CANCELER_TRADER		1
#define CANCELER_ADMIN		2
#define CANCELER_SERVER		3

// Time In Force
#define TIF_IOC		0
// #define TIF_IOC		99997
#define TIF_EOM		99998
#define TIF_DAY		99999
#define TIF_99994	99994

#define TIF_MDAY	99998
#define TIF_MIOC	0
#define TIF_MGTC	99960
#define TIF_SDAY	99999
#define TIF_SIOC	0
#define TIF_SGTC	99964


#define LEVEL1_RESTRICTED	1

// tick direction
#define TICK_LAST_UNKNOWN	0x00 //     1
#define TICK_LAST_UP		0x01 //     1
#define TICK_LAST_DOWN		0x02 //    10
#define TICK_BID_UNKNOWN	0x00 // 10000
#define TICK_BID_UP			0x10 // 10000
#define TICK_BID_DOWN		0x20 //100000

// T&S Source
#define TRADE_SOURCE_ISLAND	'I'
#define TRADE_SOURCE_BRUT	'B'
#define TRADE_SOURCE_REDI	'R'
#define TRADE_SOURCE_ARCA	'A'
#define TRADE_SOURCE_INCA	'N'
#define TRADE_SOURCE_QUOTE	'-'
#define TRADE_SOURCE_OPRA	'-'
#define TRADE_SOURCE_BATS	'P'				//BATS PITCH'S book data
#define TRADE_SOURCE_NSX	'n'				// NSX order execution

#define TRADE_SOURCE_TSX	'T'						//Toronto TSXL1. TSXL2.


#define TSX_SUFFIX			"@CA" 					//symbol's extention for TSX (L1 and L2) 

#endif//__GTCONST_H__

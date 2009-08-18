/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTPrint32.h: interface for the GTPrint32 class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTPrint32.h
	\brief interface for the GTPrint32 class.
 */

#if !defined(__GTPRINT32_H__)
#define __GTPRINT32_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "GTConst.h"
#include "MMID.h"
#include "GTime32.h"

#define QUOTE_PRINT			31
#define QUOTE_PRINT_REFRESH	32
#define QUOTE_PRINT_DISPLAY	33
#define QUOTE_PRINT_HISTORY	34

#define PRINT_INSIDE_RANGE	0.003
#define PRINT_BID_COLOR		RGB(0xff, 0x00, 0x00)
#define PRINT_ASK_COLOR		RGB(0x00, 0xff, 0x00)
#define PRINT_INSIDE_COLOR	RGB(0xff, 0xff, 0xff)

#define PRINT_SOURCE_ISLAND		'I'
#define PRINT_SOURCE_BRUT		'B'
#define PRINT_SOURCE_REDI		'R'
#define PRINT_SOURCE_ARCA		'A'
#define PRINT_SOURCE_INCA		'N'
#define PRINT_SOURCE_QUOTE		'-'

#define QUOTE_OPTION_PRINT	39
#pragma pack(8)

/*! \ingroup c
*/
/*! \struct GTPrint32
	\brief The print information. Alias of tagGTPrint32.

	\copydoc tagGTPrint32
*/
/*! \typedef typedef tagGTPrint32 GTPrint32
*/
/*! \struct tagGTPrint32
	\brief The print information

	This is the description of one trade. When a trade happens in the market, a print or more
	are sent to notify this trade. 
	\sa GTSession::OnExecMsgPrint GTStock::OnExecMsgPrint GTAPI_MSG_SESSION_OnExecMsgPrint GTAPI_MSG_STOCK_OnExecMsgPrint
*/
typedef struct tagGTPrint32
{
	//! \brief	Stock name
	char	szStock[LEN_STOCK + 1];

	//! \brief	When this print was generated (By ECN/Exchange)
	GTime32	gtime;

	//! \brief Indicate from which connection this record comes
	/*!
		Can be any one of:
		\anchor sources
		<TABLE class="dtTABLE" cellspacing="0">
		<TR>
			<TH>CHAR</TH><TH>MACRO</TH><TH>MEANING</TH>
		</TR>
		<TR>
		<TD>'I'</TD><TD>PRINT_SOURCE_ISLAND</TD><TD>This print comes from ISLD</TD>
		</TR>
		<TR>
		<TD>'B'</TD><TD>PRINT_SOURCE_BRUT</TD><TD>This print comes from BRUT</TD>
		</TR>
		<TR>
		<TD>'R'</TD><TD>PRINT_SOURCE_REDI</TD><TD>This print comes from REDI</TD>
		</TR>
		<TR>
		<TD>'A'</TD><TD>PRINT_SOURCE_ARCA</TD><TD>This print comes from ARCA</TD>
		</TR>
		<TR>
		<TD>'N'</TD><TD>PRINT_SOURCE_INCA</TD><TD>This print comes from INCA</TD>
		</TR>
		<TR>
		<TD>'-'</TD><TD>PRINT_SOURCE_QUOTE</TD><TD>This print comes from market</TD>
		</TR>
		</TABLE>

		If this field is not PRINT_SOURCE_QUOTE, the print comes from some book.
	*/
	char	chSource;		// ISLAND, ARCA, HF, etc
	//! \brief Indicate whether it is uptick or downtick when this trade filled.
	/*!
		Here the tick is different from the tagGTLevel132.nTickDirect since
		only LAST_TICK is considered. The TIC_NYSE_XX says the situation that if only take NYSE prints into 
		considerations.

		Can be any one of below:
		<TABLE class="dtTABLE" cellspacing="0">
			<TR>
				<TH>HEX</TH><TH>BIN</TH><TH>NAME</TH>
			</TR>
			<TR>
				<TD>0x01</TD><TD>0000 0001</TD><TD>TICK_LAST_UP</TD>
			</TR>
			<TR>
				<TD>0x02</TD><TD>0000 0010</TD><TD>TICK_LAST_DOWN</TD>
			</TR>
			<TR>
				<TD>0x40</TD><TD>0000 0100</TD><TD>TICK_NYSE_UP</TD>
			</TR>
			<TR>
				<TD>0x80</TD><TD>0000 1000</TD><TD>TICK_NYSE_DOWN</TD>
			</TR>
		</TABLE>
	*/
	char	nLastDirect;	//

	long	dwShares;		//!< Shares of this trade
	double	dblPrice;		//!< brief Price of this trade
	MMID	mmidContraBrokerCode; //!< (Deprecated) Who hits?
	char	chContraBrokerSide;	//!< (Deprecated) Which side hits?
	
	union
	{
		/*! \cond INTERNAL*/
		//! \brief Used to adjust the structure compatible with C#
		unsigned short exchangecode;
		/*! \endcond */

		//! \brief Indicate where this trade executed.
		/*!
			Can be any one of \ref exchangecode listed or integer 0. Some prints reported by book will have this 
			field 0.

			Because the data source's individual definition of those charactors, we don't have any standards to
			discribe the letters. The possible values is listed in the \ref exchangecode. 
		*/
		char	chExchangeCode[2];
	};
	//! \brief Sale condition
	/*! 
		A general classification of this trade. Some prints reported by book will have this field 0.

		refer to \ref salecondition for a list.
	*/
	char	chSaleCondition;		// Sale Condition

	//! \brief Color of this print.
	/*
		Used when need this print to be rendered. 

		/sa GTStock::OnGetPrintColor
	*/
	COLORREF	rgbColor;
}GTPrint32;
#pragma pack()

/*! \page salecondition Sale Condition List

\section saic Sale Condition from SAIC
Code Value can be:
<TABLE class="dtTABLE" cellspacing="0">
<TR><TH width="20%">CODE</TH><TH width="80%">DESCRIPTION</TH></TR>
<TR><TD>@ </TD><TD> Regular Sale (no condition)</TD></TR>
<TR><TD>A </TD><TD> Cash (only) Market</TD></TR>
<TR><TD>B </TD><TD> Average Price Trade</TD></TR>
<TR><TD>C </TD><TD> Cash Trade (same day clearing)</TD></TR>
<TR><TD>D </TD><TD> Next Day (only) Market</TD></TR>
<TR><TD>E </TD><TD> Automatic Execution</TD></TR>
<TR><TD>F </TD><TD> Burst Basket Execution</TD></TR>
<TR><TD>G </TD><TD> Opening/Reopening Trade Detail</TD></TR>
<TR><TD>H </TD><TD> Intraday Trade Detail</TD></TR>
<TR><TD>I </TD><TD> Basket Index on Close Transaction</TD></TR>
<TR><TD>J </TD><TD> Rule 127 Trade (NYSE)</TD></TR>
<TR><TD>K </TD><TD> Rule 155 Trade (Amex)</TD></TR>
<TR><TD>L </TD><TD> Sold Last (late reporting)</TD></TR>
<TR><TD>N </TD><TD> Next Day Trade (next day clearing)</TD></TR>
<TR><TD>O </TD><TD> Opened (late report of opened trade)</TD></TR>
<TR><TD>R </TD><TD> Seller</TD></TR>
<TR><TD>S </TD><TD> Reserved</TD></TR>
<TR><TD>T </TD><TD> Pre/Post Market Trade</TD></TR>
<TR><TD>Z </TD><TD> Sold (out of sequence)</TD></TR>
</TABLE>

\section nqds Sale Condition from NQDS
Code Value can be:
<TABLE class="dtTABLE" cellspacing="0">
<TR><TH width="20%">CODE</TH><TH width="80%">DESCRIPTION</TH></TR>
<TR><TD>@ </TD><TD> Regular Trade</TD></TR>
<TR><TD>A </TD><TD> Acquisition</TD></TR>
<TR><TD>B </TD><TD> Bunched Trade</TD></TR>
<TR><TD>C </TD><TD> Cash Trade</TD></TR>
<TR><TD>D </TD><TD> Distribution</TD></TR>
<TR><TD>G </TD><TD> Bunched Sold Trade</TD></TR>
<TR><TD>K </TD><TD> Rule 155 Trade (AMEX only)</TD></TR>
<TR><TD>L </TD><TD> Sold Last</TD></TR>
<TR><TD>N </TD><TD> Next Day</TD></TR>
<TR><TD>O </TD><TD> Opened</TD></TR>
<TR><TD>P </TD><TD> Prior Reference Price</TD></TR>
<TR><TD>M </TD><TD> Market Center Close Price</TD></TR>
<TR><TD>R </TD><TD> Seller (Long-Form Message Formats Only)</TD></TR>
<TR><TD>S </TD><TD> Split Trade</TD></TR>
<TR><TD>T </TD><TD> Form-T Trade</TD></TR>
<TR><TD>W </TD><TD> Average Price Trade</TD></TR>
<TR><TD>Z </TD><TD> Sold (Out of Sequence)</TD></TR>
<TR><TD>1 </TD><TD> Stopped Stock ?Regular Trade</TD></TR>
<TR><TD>2 </TD><TD> Stopped Stock ?Sold Last</TD></TR>
<TR><TD>3 </TD><TD> Stopped Stock ?Sold (Out of Sequence)</TD></TR>
</TABLE>
*/

/*! \page exchangecode Exchange Code

-- Exchange code from NQDS/UQDF
<TABLE class="dtTABLE" cellspacing="0">
<TR><TH width="20%">CODE</TH><TH width="80%">DESCRIPTION</TH></TR>
<TR><TD>A </TD><TD> American Stock Exchange</TD></TR>
<TR><TD>B </TD><TD> Boston Stock Exchange</TD></TR>
<TR><TD>C </TD><TD> Cincinnati Stock Exchange</TD></TR>
<TR><TD>D </TD><TD> NASD ADF</TD></TR>
<TR><TD>M </TD><TD> Chicago Stock Exchange</TD></TR>
<TR><TD>P </TD><TD> Archipelago/Pacific Exchange</TD></TR>
<TR><TD>Q </TD><TD> NASDAQ</TD></TR>
<TR><TD>X </TD><TD> Philadelphia Stock Exchange</TD></TR>
</TABLE>


-- Exchange From HyperFeed
<TABLE class="dtTABLE" cellspacing="0">
<TR><TH width="20%">CODE</TH><TH width="80%">DESCRIPTION</TH></TR>
<TR><TD>A  </TD><TD>  American Stock Exchange</TD></TR>
<TR><TD>AE </TD><TD> Alberta Stock Exchange Equities</TD></TR>
<TR><TD>AO </TD><TD> American Stock Exchange Options</TD></TR>
<TR><TD>B  </TD><TD> Boston Stock Exchange</TD></TR>
<TR><TD>C  </TD><TD> Cincinnati Stock Exchange</TD></TR>
<TR><TD>CO </TD><TD> Chicago Board Options Exchange</TD></TR>
<TR><TD>M  </TD><TD> Chicago (Midwest) Stock Exchange</TD></TR>
<TR><TD>ME </TD><TD> Montreal Stock Exchange Equities</TD></TR>
<TR><TD>MO </TD><TD> Montreal Stock Exchange Options</TD></TR>
<TR><TD>N  </TD><TD> New York Stock Exchange</TD></TR>
<TR><TD>P  </TD><TD> Pacific Stock Exchange</TD></TR>
<TR><TD>PO </TD><TD> Pacific Stock Exchange Options</TD></TR>
<TR><TD>QE </TD><TD> NASDAQ NMS Equities</TD></TR>
<TR><TD>SE </TD><TD> NASDAQ Small Cap Equities</TD></TR>
<TR><TD>T  </TD><TD> NASDAQ Listed Stocks</TD></TR>
<TR><TD>TE </TD><TD> Toronto Stock Exchange Equities</TD></TR>
<TR><TD>TO </TD><TD> Toronto Stock Exchange Options</TD></TR>
<TR><TD>UE </TD><TD> NASDAQ Bulletin Board (U.S.)</TD></TR>
<TR><TD>VE </TD><TD> Vancouver Stock Exchange Equities</TD></TR>
<TR><TD>X  </TD><TD> Philadelphia Stock Exchange</TD></TR>
<TR><TD>XO </TD><TD> Philadelphia Stock Exchange Options</TD></TR>
</TABLE>

-- Exchange code from SIAC
<TABLE class="dtTABLE" cellspacing="0">
<TR><TH width="20%">CODE</TH><TH width="80%">DESCRIPTION</TH></TR>
<TR><TD>b  </TD><TD> Bulletin Board -- Genesis Code</TD></TR>
<TR><TD>A  </TD><TD> American Stock Exchange (including options)</TD></TR>
<TR><TD>B  </TD><TD> Boston Stock Exchange</TD></TR>
<TR><TD>C*  </TD><TD> Chicago Board Options Exchange (when an option)</TD></TR>
<TR><TD>C*  </TD><TD> Cincinnati Stock Exchange</TD></TR>
<TR><TD>F  </TD><TD> NASDAQ Money Market and Mutual Funds</TD></TR>
<TR><TD>I  </TD><TD> International Securities Exchange (ISE)	</TD></TR>	<!-- //added by Jason 2005/05/13 --!>
<TR><TD>M  </TD><TD> Chicago Stock Exchange (formerly Midwest)</TD></TR>
<TR><TD>N  </TD><TD> New York Stock Exchange</TD></TR>
<TR><TD>O  </TD><TD> Options Price Reporting Authority (OPRA)	</TD></TR>	<!-- //added by Jason 2005/05/13 --!>
<TR><TD>P  </TD><TD> Pacific Stock Exchange (including options)</TD></TR>
<TR><TD>Q  </TD><TD> NASDAQ NMS</TD></TR>
<TR><TD>S  </TD><TD> NASDAQ Small Cap</TD></TR>
<TR><TD>T  </TD><TD> NASDAQ Listed Stocks</TD></TR>
<TR><TD>U  </TD><TD> NASDAQ U.S. Over-the-Counter Bulletin Board (OTCBB)</TD></TR>
<TR><TD>V  </TD><TD> NASDAQ OTC Other</TD></TR>
<TR><TD>X  </TD><TD> Philadelphia Stock Exchange (including options)</TD></TR>
<TR><TD>W  </TD><TD> Chicago Board Options Exchange Non-options</TD></TR>
<TR><TD>**  </TD><TD> Chicago Board of Trade</TD></TR>
<TR><TD>**  </TD><TD> Chicago Mercantile Exchange</TD></TR>
<TR><TD>**  </TD><TD> New York Futures Exchange</TD></TR>
<TR><TD>**  </TD><TD> New York Mercantile Exchange</TD></TR>
<TR><TD>**  </TD><TD> New York Cotton Exchange</TD></TR>
<TR><TD>**  </TD><TD> Commodity Exchange Center</TD></TR>
<TR><TD>**  </TD><TD> Minneapolis Grain Exchange</TD></TR>
<TR><TD>**  </TD><TD> MidAmerica Commodity Exchange</TD></TR>
<TR><TD>**  </TD><TD> Kansas Board of Trade</TD></TR>
<TR><TD>**  </TD><TD> Philadelphia Board of Trade</TD></TR>
<TR><TD>**  </TD><TD> Coffee, Sugar and Cocoa Exchange</TD></TR>
</TABLE>

Note: '*' stands for the star.
*/

#endif // !defined(__GTPRINT32_H__)

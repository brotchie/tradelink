/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file MMID.h
	\brief Defines MMID's
 */

#ifndef __MMID_H__
#define __MMID_H__

// MMID
typedef unsigned long MMID;

#define CAST_TEXT_MMID(psz)		(*(MMID *)(LPCSTR)(psz))
#define CAST_MMID_TEXT(mmid)	((LPSTR)&(mmid))

#define MAKE_MMID(a, b, c, d) ((MMID)(\
			((unsigned char)(a)) <<  0 |\
			((unsigned char)(b)) <<  8 |\
			((unsigned char)(c)) << 16 |\
			((unsigned char)(d)) << 24))

// exchange method
#define METHOD_UNKNOWN	MAKE_MMID(' ', ' ', ' ', ' ')
#define METHOD_ISLD		MAKE_MMID('I', 'S', 'L', 'D')
// Kevin:
// INET == ISLD, at least at this point
#define METHOD_INET		MAKE_MMID('I', 'S', 'L', 'D') 

#define METHOD_SOES		MAKE_MMID('S', 'O', 'E', 'S')
#define METHOD_SIZE		MAKE_MMID('S', 'I', 'Z', 'E')
#define METHOD_SWST		MAKE_MMID('S', 'W', 'S', 'T')
#define METHOD_HELF		MAKE_MMID('H', 'E', 'L', 'F')
#define METHOD_SNET		MAKE_MMID('S', 'N', 'E', 'T')
#define METHOD_ARCA		MAKE_MMID('A', 'R', 'C', 'A')
#define METHOD_BULT		MAKE_MMID('B', 'U', 'L', 'T')
#define METHOD_INCA		MAKE_MMID('I', 'N', 'C', 'A')
#define METHOD_INST		MAKE_MMID('I', 'N', 'S', 'T')
#define METHOD_BRUT		MAKE_MMID('B', 'R', 'U', 'T')
#define METHOD_BRTX		MAKE_MMID('B', 'R', 'T', 'X')
#define METHOD_REDI		MAKE_MMID('R', 'E', 'D', 'I')
#define METHOD_GNET		MAKE_MMID('G', 'N', 'E', 'T')
#define METHOD_LSPD		MAKE_MMID('L', 'S', 'P', 'D')
#define METHOD_ATTN		MAKE_MMID('A', 'T', 'T', 'N')
#define METHOD_ERCO		MAKE_MMID('E', 'R', 'C', 'O')
#define METHOD_DATA		MAKE_MMID('D', 'A', 'T', 'A')
#define METHOD_TRAC		MAKE_MMID('T', 'R', 'A', 'C')
#define MMID_MPN        MAKE_MMID('M', 'P', 'N', ' ')      // MarcoPolo Network  (Cathy)

#define METHOD_SMRT		MAKE_MMID('S', 'M', 'R', 'T')

//changed by Jason Ruan
#define METHOD_BELZ		MAKE_MMID('B', 'E', 'L', 'Z')
#define METHOD_BLZX		MAKE_MMID('B', 'L', 'Z', 'X')
#define METHOD_BLZV		MAKE_MMID('V', 'W', 'A', 'P')	//for VWAP order

#define METHOD_MLYN		MAKE_MMID('M', 'L', 'C', 'O')
#define METHOD_MLNX		MAKE_MMID('M', 'L', 'N', 'X')

#define METHOD_PPLN		MAKE_MMID('P', 'P', 'L', 'N')
#define METHOD_SENG		MAKE_MMID('S', 'E', 'N', 'G')
#define METHOD_SEN2		MAKE_MMID('S', 'E', 'N', '2')

#define METHOD_ISEO		MAKE_MMID('I', 'S', 'E', 'O')
#define METHOD_ITS		MAKE_MMID('I', 'T', 'S', 'D')
#define METHOD_ITSX		MAKE_MMID('I', 'T', 'S', 'X')

//add by M.Y. 1/15/2005
#define METHOD_OES	    MAKE_MMID('O', 'E', 'S', 'D')
#define METHOD_OESX		MAKE_MMID('O', 'E', 'S', 'X')
#define METHOD_NYF		MAKE_MMID('N', 'Y', 'F', 'D')
#define METHOD_NYFX		MAKE_MMID('N', 'Y', 'F', 'X')
#define METHOD_NITE		MAKE_MMID('N', 'I', 'T', 'E')

#define METHOD_LSTK		MAKE_MMID('L', 'S', 'T', 'K')
#define METHOD_SLIP		MAKE_MMID('S', 'L', 'I', 'P')
#define METHOD_PFTD		MAKE_MMID('P', 'F', 'T', 'D')
#define METHOD_ACT		MAKE_MMID('A', 'C', 'T', 'S')

#define METHOD_UBSS	    MAKE_MMID('U', 'B', 'S', 'S')
#define METHOD_BATS	    MAKE_MMID('B', 'A', 'T', 'S')
#define METHOD_AUTO	    MAKE_MMID('A', 'U', 'T', 'O')
#define METHOD_AUTP	    MAKE_MMID('A', 'U', 'T', 'P')
#define METHOD_EDGA	    MAKE_MMID('E', 'D', 'G', 'A')
#define METHOD_EDGX	    MAKE_MMID('E', 'D', 'G', 'X')
#define METHOD_ISEQ	    MAKE_MMID('I', 'S', 'E', 'Q')
#define METHOD_CDRG	    MAKE_MMID('C', 'D', 'R', 'G')
#define METHOD_CRSS	    MAKE_MMID('C', 'R', 'S', 'S')
#define METHOD_NSXX	    MAKE_MMID('N', 'S', 'X', 'X')
#define METHOD_GETC	    MAKE_MMID('G', 'F', 'L', 'O')

//Sun
#define METHOD_MLOP		MAKE_MMID('M', 'L', 'O', 'P')

#define METHOD_SM01		MAKE_MMID('S', 'M', '0', '1')  //FOR CITI

//Jason: 
#define METHOD_EXTERNAL_NAME_SMART			"Smart"

#define METHOD_EXTERNAL_NAME_SMART1			"Smart1"

#define	METHOD_EXTERNAL_NAME_MARKET			"MKT"

// Kevin:
#define METHOD_EXTERNAL_NAME_INET			"INET" 
#define METHOD_EXTERNAL_NAME_ISLAND			"Island" 

#define METHOD_EXTERNAL_NAME_BRUT			"BRUT"
#define	METHOD_EXTERNAL_NAME_BRTX			"BT-X"
#define METHOD_EXTERNAL_NAME_ARCA			"ARCA"
#define METHOD_EXTERNAL_NAME_INCA			"INCA"
#define METHOD_EXTERNAL_NAME_REDI			"REDI"
#define METHOD_EXTERNAL_NAME_GNET			"GNET"
#define METHOD_EXTERNAL_NAME_LSPD			"LSPD"
#define METHOD_EXTERNAL_NAME_ATTN			"ATTN"
#define METHOD_EXTERNAL_NAME_ERCO			"ERCO"
#define METHOD_EXTERNAL_NAME_DATA			"DATA"
#define METHOD_EXTERNAL_NAME_TRAC			"TRAC"
#define METHOD_EXTERNAL_NAME_SOES			"SOES"
#define METHOD_EXTERNAL_NAME_SNET			"SNET"
#define METHOD_EXTERNAL_NAME_SWST			"SWST"
#define METHOD_EXTERNAL_NAME_SIZE			"SIZE"
#define METHOD_EXTERNAL_NAME_BULT			"BULT"
#define METHOD_EXTERNAL_NAME_INST			"INST"
#define	METHOD_EXTERNAL_NAME_HELF			"NYSE-H"
#define METHOD_EXTERNAL_NAME_BELZ			"BZ-Dot"
#define	METHOD_EXTERNAL_NAME_BLZX			"BZ-X"
#define METHOD_EXTERNAL_NAME_BLZV			"VWAP"
#define METHOD_EXTERNAL_NAME_MLYN			"ML-Dot"
#define	METHOD_EXTERNAL_NAME_MLNX			"ML-X"
#define METHOD_EXTERNAL_NAME_PPLN			"PPLN"
#define METHOD_EXTERNAL_NAME_SENG			"SENG"
#define METHOD_EXTERNAL_NAME_SEN2			"SEN2"
#define METHOD_EXTERNAL_NAME_ISE			"ISE Fix"
#define METHOD_EXTERNAL_NAME_OES			"OES-D" // add 1/5/2005
#define METHOD_EXTERNAL_NAME_OESX			"OES-X"
#define METHOD_EXTERNAL_NAME_NYF			"NYF-Dot"
#define METHOD_EXTERNAL_NAME_NYFX			"NYF-Res"
#define METHOD_EXTERNAL_NAME_ITS			"ITS"
#define METHOD_EXTERNAL_NAME_ITSX			"ITS-X"
#define METHOD_EXTERNAL_NAME_LSTK			"LSTK"
#define METHOD_EXTERNAL_NAME_SLIP			"SLIP"
#define METHOD_EXTERNAL_NAME_PFTD			"PFTD"
#define METHOD_EXTERNAL_NAME_NITE			"NITE"
#define METHOD_EXTERNAL_NAME_BATS			"BATS"
#define METHOD_EXTERNAL_NAME_AUTO			"Stop"
#define METHOD_EXTERNAL_NAME_AUTP			"ATDLP"
#define METHOD_EXTERNAL_NAME_EDGA			"EDGA"
#define METHOD_EXTERNAL_NAME_EDGX			"EDGX"
#define METHOD_EXTERNAL_NAME_MLOP			"MLYN-Option"
#define METHOD_EXTERNAL_NAME_ISEQ			"ISEQ"
#define METHOD_EXTERNAL_NAME_CDRG			"Citadel"
#define METHOD_EXTERNAL_NAME_CRSS			"CreditSuisse"
#define METHOD_EXTERNAL_NAME_NSXX			"NSX"
#define METHOD_EXTERNAL_NAME_GETCO			"GETCO"
//end

//end change

#define MMID_UNKNOWN	MAKE_MMID(' ', ' ', ' ', ' ')
#define MMID_DEFAULT	MAKE_MMID('1', '0', '8', '8')
#define MMID_ISLD		MAKE_MMID('I', 'S', 'L', 'D')	//Island ECN
#define MMID_INET       MAKE_MMID('I', 'N', 'E', 'T')	//INET
#define MMID_BRUT		MAKE_MMID('B', 'R', 'U', 'T')	//Brass Utility ECN
#define MMID_REDI		MAKE_MMID('R', 'E', 'D', 'I')	//Spear Leeds & Kellogg ECN
#define MMID_ARCA		MAKE_MMID('A', 'R', 'C', 'A')	//Archipelago Exchange
#define MMID_BULT		MAKE_MMID('B', 'U', 'L', 'T')
#define MMID_INCA		MAKE_MMID('I', 'N', 'C', 'A')	//Instinet ECN
#define MMID_AMEX		MAKE_MMID('A', 'M', 'E', 'X')	//AMERICA EXCHANGE
#define MMID_MMID		MAKE_MMID('M', 'M', 'I', 'D')	//InterMarket Market Maker
#define MMID_NASD		MAKE_MMID('N', 'A', 'S', 'D')	//NASDAQ
#define MMID_BOSX		MAKE_MMID('B', 'O', 'S', 'X')	//BOSTON EXCHANGE
#define MMID_LINX		MAKE_MMID('L', 'I', 'N', 'X')	
#define MMID_CMSE		MAKE_MMID('C', 'M', 'S', 'E')	//Chicago Mechindase
#define MMID_NYSE		MAKE_MMID('N', 'Y', 'S', 'E')	//NYSE
#define MMID_PACF		MAKE_MMID('P', 'A', 'C', 'F')	//PACIFIC
#define MMID_PHLX		MAKE_MMID('P', 'H', 'L', 'X')	//Philadophia
#define MMID_GNET		MAKE_MMID('G', 'N', 'E', 'T')
#define MMID_LSPD		MAKE_MMID('L', 'S', 'P', 'D') 
#define MMID_ATTN		MAKE_MMID('A', 'T', 'T', 'N')	//Attain ECN
#define MMID_ERCO		MAKE_MMID('E', 'R', 'C', 'O')	//Erco
#define MMID_DATA		MAKE_MMID('D', 'A', 'T', 'A')	//Data
#define MMID_TRAC		MAKE_MMID('T', 'R', 'A', 'C')	//Track ECN
#define MMID_MPN        MAKE_MMID('M', 'P', 'N', ' ')		// MarcoPolo Network  (Cathy)
#define MMID_MSE		MAKE_MMID('M', 'W', 'S', 'E')	//Chichargo Stock Exchange
#define MMID_OES		MAKE_MMID('O', 'E', 'S', 'X')   //Order Excution Service LLC add by M.Y. 1/12/2005
#define MMID_NYSD		MAKE_MMID('N', 'Y', 'S', 'D')	//Place NYSE direct +
#define MMID_VWAP		MAKE_MMID('V', 'W', 'A', 'P')	
#define MMID_NYFX		MAKE_MMID('N', 'Y', 'F', 'X')	//NyFix

#define MMID_DOTA		MAKE_MMID('D', 'O', 'T', 'A')
#define MMID_DOTI		MAKE_MMID('D', 'O', 'T', 'I')
#define MMID_STGY		MAKE_MMID('S', 'T', 'G', 'Y')

//Options
#define MMID_ISE		MAKE_MMID('I', 'S', 'E', 'O')	//ISE
#define MMID_AMOP		MAKE_MMID('A', 'M', 'O', 'P')
#define MMID_CBOE		MAKE_MMID('C', 'B', 'O', 'E')
#define MMID_PCOP		MAKE_MMID('P', 'C', 'O', 'P')
#define MMID_PHOP		MAKE_MMID('P', 'H', 'O', 'P')
#define MMID_BOX		MAKE_MMID('B', 'O', 'X', 'O')

#define MMID_SLIP		MAKE_MMID('S', 'L', 'I', 'P')
#define MMID_OPRA		MAKE_MMID('O', 'P', 'R', 'A')

#define MMID_NITE		MAKE_MMID('N', 'I', 'T', 'E')	//Knight Securities
#define MMID_BATS		MAKE_MMID('B', 'A', 'T', 'S')
#define MMID_AUTO		MAKE_MMID('A', 'U', 'T', 'O')
#define MMID_AUTP		MAKE_MMID('A', 'U', 'T', 'P')
#define MMID_EDGA		MAKE_MMID('E', 'D', 'G', 'A')
#define MMID_EDGX		MAKE_MMID('E', 'D', 'G', 'X')
#define MMID_ISEQ		MAKE_MMID('I', 'S', 'E', 'Q')
#define MMID_CDRG		MAKE_MMID('C', 'D', 'R', 'G')

//TSX Toronto
#define MMID_TSXL1		MAKE_MMID('T', 'S', 'X', '1')
#define MMID_TX2B		MAKE_MMID('T', 'X', 'B', ' ')	//(MMID_TSXL2) TSXL2 (BOOK)	

//changed by Jason Ruan
//#define MMID_BELZ		MAKE_MMID('B', 'L', 'Z', '*')
//end change

#define MMID_ADFN		MAKE_MMID('A', 'D', 'F', 'N')
#define MMID_ARCX		MAKE_MMID('A', 'R', 'C', 'X')

#define MMID_ECN		MAKE_MMID('E', 'C', 'N', ' ') //stat server
#define MMID_ALL		MAKE_MMID('A', 'L', 'L', ' ') //stat server 

// ecn book
#define MMID_ISB		MAKE_MMID('I', 'S', 'B', ' ')	//ISLAND BOOK
#define MMID_BRB		MAKE_MMID('B', 'R', 'B', ' ')	//BRUT BOOK
#define MMID_RDB		MAKE_MMID('R', 'D', 'B', ' ')	//REDI BOOK
#define MMID_ARB		MAKE_MMID('A', 'R', 'B', ' ')
#define MMID_INB		MAKE_MMID('I', 'N', 'B', ' ')	//Instinet ECN BOOK
#define MMID_GNB		MAKE_MMID('G', 'N', 'B', ' ')	
#define MMID_NYB		MAKE_MMID('N', 'Y', 'B', ' ')	//NYSE BOOK
#define MMID_NXB		MAKE_MMID('N', 'X', 'B', ' ')	// NSX BOOK

#define MMID_NTRD		MAKE_MMID('N', 'T', 'R', 'D')	//Nextrade ECN
#define MMID_BTRD		MAKE_MMID('B', 'T', 'R', 'D')	//Bloomberg TradeBook ECN

						//					|-->  ECN_BOOK_BIT
#define MMID_BPHB		MAKE_MMID('B', 'P', 'B', ' ')	//BATS' PITCH BOOK
#define MMID_DGAB		MAKE_MMID('G', 'A', 'B', ' ')	//direct edge's EDGA BOOK
#define MMID_DGXB		MAKE_MMID('G', 'X', 'B', ' ')	//direct edge's EDGX BOOK

//#define MMID_GNDS		MAKE_MMID('G', 'N', 'D', 'S')
//#define MMID_GNDT		MAKE_MMID('G', 'N', 'D', 'T')

#ifdef __cplusplus
extern "C"
{
#endif//__cplusplus
	LPSTR WINAPI copymmid(LPSTR pszMMID, MMID mmid);
	MMID WINAPI makemmid(LPCSTR pszMMID);
#ifdef __cplusplus
}
#endif//__cplusplus

#if 0
#define IS_ECN_BOOK(mmid)	(\
								(mmid) == MMID_ISB \
							 || (mmid) == MMID_BRB \
							 || (mmid) == MMID_RDB \
							 || (mmid) == MMID_ARB \
							 || (mmid) == MMID_INB \
							 || (mmid) == MMID_GNB \
							 || (mmid) == MMID_BPHB \
							 || (mmid) == MMID_DGAB \
							 || (mmid) == MMID_DGXB \
							)
#else

//#define IS_ECN_BOOK(mmid)	(((mmid) & MAKE_MMID(0x00, 0x00, 0xff, 0xff)) == MAKE_MMID(0x00, 0x00, 'B', ' '))
#define ECN_BOOK_BIT				MAKE_MMID(0x00, 0x00, 'B', ' ')
#define ECN_BOOK_MASK				MAKE_MMID(0x00, 0x00, 0xff, 0xff)
#define TOTAL_VIEW_BIT				MAKE_MMID(0x80, 0x00, 0x00, 0x00)
#define OPEN_VIEW_BIT				MAKE_MMID(0x00, 0x80, 0x00, 0x00)
#define OPTION_VIEW_BIT				MAKE_MMID(0x00, 0x00, 0x80, 0x00)
#define _ALL_VIEW_BIT				MAKE_MMID(0x80, 0x80, 0x80, 0x80)

#define CLEAR_ALL_VIEW_BIT(mmid) 	((mmid) & (~_ALL_VIEW_BIT))

#define CLEAR_TOTAL_VIEW_BIT(mmid) 	((mmid) & (~TOTAL_VIEW_BIT))
#define SET_TOTAL_VIEW_BIT(mmid) 	((mmid) | (TOTAL_VIEW_BIT))

#define CLEAR_OPEN_VIEW_BIT(mmid) 	((mmid) & (~OPEN_VIEW_BIT))
#define SET_OPEN_VIEW_BIT(mmid) 	((mmid) | (OPEN_VIEW_BIT))

#define CLEAR_OPTION_VIEW_BIT(mmid) 	((mmid) & (~OPTION_VIEW_BIT))
#define SET_OPTION_VIEW_BIT(mmid) 	((mmid) | (OPTION_VIEW_BIT))

#define IS_ECN_BOOK_(mmid)			(((mmid) & ECN_BOOK_MASK) == ECN_BOOK_BIT)
#define IS_TOTAL_VIEW(mmid)			(((mmid) & TOTAL_VIEW_BIT) != 0)
#define IS_OPEN_VIEW(mmid)			(((mmid) & OPEN_VIEW_BIT) != 0)
#define IS_OPTION_VIEW(mmid)		(((mmid) & OPTION_VIEW_BIT) != 0)

#define IS_OPTION_MMID(mmid)		(mmid == MMID_ISE			\
										|| mmid == MMID_AMEX	\
										|| mmid == MMID_CBOE	\
										|| mmid == MMID_PHLX	\
										|| mmid == MMID_PACF	\
										|| mmid == MMID_BOSX)

#define IS_BOOK(mmid)				(IS_ECN_BOOK_(mmid) || IS_TOTAL_VIEW(mmid) || IS_OPEN_VIEW(mmid))

#endif

#endif//__MMID_H__

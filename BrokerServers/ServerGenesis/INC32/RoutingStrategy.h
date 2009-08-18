/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

/*! \file RoutingStrategy.h
	\brief Defines INET Routing Strategy ID's
	The routing strategy is defined by NASDAQ. Please check www.nasdaqtrader.com for more details.
 */

#pragma once

//! \def INET_RS_INET
//  \brief No routing.
#define INET_RS_INET	MAKE_MMID('I', 'N', 'E', 'T')

//! \def INET_RS_DOTA
//  \brief Sweep the INET book and the Street. If the order is not completely filled, the remainder is sent to the NYSE floor. The order IOCs the INET book at the price it would send to another destination before sending it to that destination. Finally, it is sent to the NYSE floor at the limit price specified on the order. Routing destinations are INET, NASDAQ, Brut and ARCA. In order to send your order via NYSE Direct+, please follow the instructions in the Implementation Table below.
#define INET_RS_DOTA	MAKE_MMID('D', 'O', 'T', 'A')

//! \def INET_RS_DOTM
//  \brief Sweep the INET book and the Street. After routing to accessible market centers, the order will be posted to the INET book. If the order becomes marketable on another accessible market center while on the INET book, the order will route to the other market center. If the order routes to the NYSE or Amex it will remain on the NYSE or Amex until being executed or canceled. In order to send your order via NYSE Direct+, please follow the instructions in the Implementation Table below.
#define INET_RS_DOTM	MAKE_MMID('D', 'O', 'T', 'M')

//! \def INET_RS_DOTP
//  \brief Sweep the INET book and the Street. After routing to accessible market centers, the order will reside on the INET book for a pre-determined period of time, currently set at three seconds, after which the order will be routed via DOT. In order to send your order via NYSE Direct+, please follow the instructions in the Implementation Table below.
#define INET_RS_DOTP	MAKE_MMID('D', 'O', 'T', 'P')

//! \def INET_RS_DOTI
//  \brief Similar to DOTN except the order will only route to the INET book before sending to DOT. In order to send your order via NYSE Direct+, please follow the instructions in the Implementation Table below.
#define INET_RS_DOTI	MAKE_MMID('D', 'O', 'T', 'I')

//! \def INET_RS_DOTD
//  \brief The order will be routed directly to the NYSE or Amex without sweeping the INET book. In order to send your order via NYSE Direct+, please follow the instructions in the Implementation Table below.
#define INET_RS_DOTD	MAKE_MMID('D', 'O', 'T', 'D')

//! \def INET_RS_STGY
//  \brief The order will sweep the INET book at the price it would send to another destination before sending it to that destination. Finally, it will be posted on INET. This strategy will not send the order to DOT. All orders, except IOC orders, will post to the INET book if not filled in the market.
#define INET_RS_STGY	MAKE_MMID('S', 'T', 'G', 'Y')

//! \def INET_RS_SCAN
//  \brief The strategy is similar to STGY except once it posts on the book it will not route out again if the market moves.
#define INET_RS_SCAN	MAKE_MMID('S', 'C', 'A', 'N')

//! \def INET_RS_MOPP
//  \brief The strategy will sweep the market upon entry and post the balance on the NASDAQ book. Unlike the SCAN order, the MOPP order will only route to destinations for their displayed size.  
#define INET_RS_MOPP	MAKE_MMID('M', 'O', 'P', 'P')

//! \def INET_RS_ISNY
//  \brief The ISNY is directed to NYSE other than NASDAQ.
#define INET_RS_ISNY	MAKE_MMID('I', 'S', 'N', 'Y')

//! \def INET_RS_ISAM
//  \brief The ISAM is directed to AMEX other than NASDAQ.
#define INET_RS_ISAM	MAKE_MMID('I', 'S', 'A', 'M')

////ROUT = If order is marketable, it is eligible to be routed externally.
//#define EDGX_RS_ROUT	MAKE_MMID('R', 'O', 'U', 'T')
//
////ROUX = If order is marketable, it is eligible to be routed externally but will not be
////routed to any IOI destinations that DirectEdge routes to.
//#define EDGX_RS_ROUX	MAKE_MMID('R', 'O', 'U', 'X')
//
////ROUZ = After taking available liquidity, the order will be routed to IOI destinations
////but will not be routed externally to the street.
//#define EDGX_RS_ROUZ	MAKE_MMID('R', 'O', 'U', 'Z')
//
////RDOT = If listed order is marketable, it is eligible to be routed to DOT.
//#define EDGX_RS_RDOT	MAKE_MMID('R', 'D', 'O', 'T')

//! \def EDGX_RS_ALIQ
//  \brief Edgx Add liquidity Only
#define EDGX_RS_ALIQ	MAKE_MMID('A', 'L', 'I', 'Q')

//! \EDGX_RS_EDGX
//	\brief For EDGX Only
#define EDGX_RS_EDGX	MAKE_MMID('E', 'D', 'G', 'X')

//! \EDGX_RS_RDOX
//	\brief EDGX Not route out once on the NYSE book
#define EDGX_RS_RDOX	MAKE_MMID('R', 'D', 'O', 'X')

//! \EDGX_RS_RDOT
//	\brief Sweep EDGX and send order to the primary exchange
#define EDGX_RS_RDOT	MAKE_MMID('R', 'D', 'O', 'T')

//! \EDGA_RS_EDGA
//	\brief For EDGA Only
#define EDGA_RS_EDGA	MAKE_MMID('E', 'D', 'G', 'A')

//! \EDGA_RS_RDOX
//	\brief Sweep EDGA and send order to the primary exchange as non routable order
#define EDGA_RS_RDOX	MAKE_MMID('R', 'D', 'O', 'X')

//! \EDGA_RS_RDOT
//	\brief Sweep EDGA and send order to the primary exchange
#define EDGA_RS_RDOT	MAKE_MMID('R', 'D', 'O', 'T')

//! \def ARCA_RS_ISO
//  \brief The ISO is Inter-market Sweep Order in ARCA
#define ARCA_RS_ISO		MAKE_MMID('I', 'S', 'O', ' ')

//! \def ARCA_RS_POO
//  \brief The POO is Primary Open Orders in ARCA
#define ARCA_RS_POO		MAKE_MMID('P', 'O', 'O', ' ')

//! \def ARCA_RS_PNPB
//  \brief In ARCA, Marketable Contra Orders will first execute against the PNP B Orders, then the rest of the book. PNP B Orders will be ranked in time priority regardless of the price of the order when they are blind. When the PBBO moves away from the price of the PNP B but the prices continue to overlap; the PNP B will remain blind but adjust its price to the PBBO. When the PBBO moves away from the price of the PNP B and there is no longer price overlap, the PNP lights up and becomes a regular limit order including standing its ground.
#define ARCA_RS_PNPB	MAKE_MMID('P', 'N', 'P', 'B')

//! \def ARCA_RS_TO
//  \brief In ARCA, Tracking order that executes against outbound orders with a leaves quantity less than or equal to the size of the tracking limit order.
#define ARCA_RS_TO		MAKE_MMID('T', 'O', ' ', ' ')

//! \def ARCA_RS_PL
//  \brief In ARCA, A Passive Liquidity order (PL) is an order that is never displayed externally. It’s ranked behind display and reserve orders and aheadof all other orders. For NYSE Arca primary listings, LMMs musthave a displayed Q order meeting certain requirements for a PL Order to be eligible to execute. These display requirements areoutlined in NYSE Arca Rule 7.31(h)(4) and section 3.15 (FIX Market Making) of this document. Please note that Q Orders onlywill be considered when evaluating whether an LMM is meeting the display requirement for execution of a PL Order; regular displayed limit orders will not be considered for this purpose.
#define ARCA_RS_PL		MAKE_MMID('P', 'L', ' ', ' ')

//! \def ARCA_RS_MPL
//  \brief In ARCA, MPLs will have a minimum entry size of 100 shares and will not be displayed. MPL Orders will be valid for any session but do notparticipate in any auctions. MPL Orders may also be entered with a Minimum Execution Size condition, though this is not required.MPL orders always execute at the midpoint and do not receive price improvement.
#define ARCA_RS_MPL		MAKE_MMID('M', 'P', 'L', ' ')

//! \def ARCA_RS_MPLO
//  \brief In ARCA, MidPoint Passive Liquidity orders with Tag 9416=0. Do not want to interact with MidPoint Passive Liquidity orders.
#define ARCA_RS_MPLO	MAKE_MMID('M', 'P', 'L', 'O')

//! \def ARCA_RS_NOW
//  \brief In ARCA, A limited price order that is executed in whole or in part that will be routed to one or more NOW recipients (those venues that respondimmediately with a fill or a cancel) for immediate execution if the order cannot be executed on ArcaEx. Orders are immediately canceled if not executed at the quoted price or better.
#define ARCA_RS_NOW		MAKE_MMID('N', 'O', 'W', ' ')

//! \def BATS_RS_NYSE
//  \brief The order will remain on the NYSE after sweep BATS if not filled
#define BATS_RS_NYSE	MAKE_MMID('N', 'Y', 'S', 'E')

//! \def ARCA_RS_POP
//  \brief PO plus Order Type
#define ARCA_RS_POP		MAKE_MMID('P', 'O', 'P', ' ')

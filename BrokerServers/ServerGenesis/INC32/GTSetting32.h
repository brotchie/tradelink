/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTSetting32.h
	\brief interface for the GTSetting32 class.
 */

#ifndef __GTSETTING32_H__
#define __GTSETTING32_H__

#include "MMID.h"
#include "..\Inc32\CMapClass.h"
#include "..\Inc32\RoutingStrategy.h"

//! \cond INTERNAL
typedef struct { unsigned _unused; }GTECNS, *LPGTECNS;
typedef struct { unsigned _unused; }GTAXES, *LPGTAXES;
typedef struct { unsigned _unused; }GTHIDDENS, *LPGTHIDDENS;
//! \endcond
#pragma pack(8)

/*! \ingroup c
*/
/*! \struct GTSetting32
	\brief The same as tagGTSetting32.

	\copydoc tagGTSetting32
*/
/*! \typedef tagGTSetting32 GTSetting32
*/
/*! \struct  tagGTSetting32
	\brief Global settings for the trading system.

	This is the default settings for the trading system. In this struct,
	the important information is kept. When an trading action is taken while some
	parameter is not specified, API will looking into this for the default
	value. 

	Revise the value in this struct will affect the global trading actions.
	So, it is suggested to do the revision on you GTStock class (or GTStock32),
	which will just act on the select symbol.

	\sa GTSetting
*/
typedef struct tagGTSetting32
{
	/*! \brief Executor Server Address
	*/
	char szExecAddress[256];
	/*! \brief Executor Port Number
	*/
	unsigned short nExecPort;

	/*! \brief Quote Server Address
	*/
	char szQuoteAddress[256];
	/*! \brief Quote Server Port Number
	*/
	unsigned short nQuotePort;

	/*! \brief Level 2 Server Address
	*/
	char szLevel2Address[256];
	/*! \brief Level 2 Server Port Number
	*/
	unsigned short nLevel2Port;

	/*! \brief Chart Server Address
	*/
	char szChartAddress[256];
	/*! \brief Chart Server Port Number
	*/
	unsigned short nChartPort;

//@{	->related to level 2
	/*!	\brief The precision used to divide level 2
		
		This is used when API calculate the level2 level price. Usually 100 or 1000 is used. 

		\sa tagGTLevel232::dblLevelPrice GTLevel2List::GetLevel
	*/
	int		m_nLevelRate;			// 100 or 1000
	/*!	\brief Sort sequence
		
		If this is LEVEL2_SORTNO_IME, in level 2 list it will sort the records in one level as MM then ECN.
		If this is LEVEL2_SORTNO_IEM, in level 2 list it will sort the records in one level as ECN then MM.
	*/
	int		m_nLevel2SortNo;

	/*!	\brief Do auto correction?
		
		API can do auto correction when obvious error occurs in level 2 data. Set this TRUE when you want to
		enable this feature.
	*/
	BOOL	m_bAutoCorrection;
	/*!	\brief Is m_dblACPriceThreshold valid?

		Auto correction price threshold is used to make judgement if an error should be AUTOCORRECTed. Setting
		this TRUE will set m_bACPriceThreshold to be valid.
	*/
	BOOL	m_bACPriceThreshold;
	/*!	\brief The threshold for a price error that should be autocorrected.
	
		When m_bAutoCorrection and m_bAutoCorrection both are true, any level 2 record that comes in will be checked
		for price validation. 
		- For bid side, the valid price should be less than MAX(BestBid, BestAsk)+m_dblACPriceThreshold;
		- For ask side, the valid price should be greater than MIN(BestBid, BestAsk)-m_dblACPriceThreshold.
	*/
	double	m_dblACPriceThreshold;
	/*!	\brief Minimum Bid/Ask difference that will be autocorrected.
		
		Used when correct Level 2. This is the maximum difference that BestBid-BestAsk can have. If BestBid-BestAsk
		greater than this, it will be corrected. 

		When a correction is conducting, the invalid record will be deleted if:
		- if m_bACRemoveOddShares = TRUE:
			- BestBid is odd share, and BestAsk is not: bid is deleted;
			- BestBid is not odd share, and BestAsk is: ask is deleted.
			- Otherwise, none is deleted.
		- if m_bACRemoveBook = TRUE:
			- BestBid is from book, and BestAsk is not: bid is deleted;
			- BestBid is not from book, and BestAsk is: ask is deleted.
			- Otherwise, none is deleted.
		- if m_bACTimeThreshold = TRUE, and the difference of life of BestBid
			and life of BestAsk is greater than m_nACTimeThreshold (in second), the
			older one will be deleted.
	*/
	double	m_dblACDiff;
	/*!	\brief Need autocorrect odd shares? 
		
		\see m_dblACDiff
	*/
	BOOL	m_bACRemoveOddShares;
	/*!	\brief Need autocorrect book? 
		
		\see m_dblACDiff
	*/
	BOOL	m_bACRemoveBook;
	/*!	\brief Need autocorrect with time invalid? 
		
		\see m_dblACDiff
	*/
	BOOL	m_bACTimeThreshold;
	/*!	\brief Time threshold for the autocorrection? 
		
		In second, default is 3 second. 
		\see m_dblACDiff
	*/
	int		m_nACTimeThreshold;
//@}
//@{
	//! \brief If ARCA disconnected, will order to ARCA route to INET?
	BOOL	m_bRouteARCAtoINET;
	//! \brief If TRAC disconnected, will order to TRAC route to INET?
	BOOL	m_bRouteTRACtoINET;
	//! \brief If DATA disconnected, will order to DATA route to INET?
	BOOL	m_bRouteDATAtoINET;
//@}
	//! \brief [Dangerous] Allow multiple sell?
	/*!
		Will send shortsell as sell if the long position is less than the sell size?
	*/
	BOOL	m_bAllowMultipleSell;

	//! \brief Odd order Option: \ref oddorder "Enable/disable auto redirect?"
	BOOL	m_bAutoRedirect100;
	//! \brief Odd order Option: \ref oddorder "place to redirect"
	MMID	m_mmidAutoRedirect100;

	double		m_dblDirectHitRange;
//@{
	//! \brief [INTERNAL USED] Test only
	int			m_nTrainExecDiff0;		// < 0.01
	int			m_nTrainExecDiff1;		// >= 0.01
	int			m_nTrainExecDiffECN0;
	int			m_nTrainExecDiffECN1;
//@}

	//! \brief Will ISLD display your orders?
	BOOL		m_bDisplayOrder;		// Valid On Island

//@{
	BOOL		m_bReserveShares;		//!< Will reserve shares?
	BOOL		m_bSameReserveShares;	//!< Using the reserve setting for INCA for other place?
	BOOL		m_bReserveINETShow;		//!< Make all INET order be reserve order?
	int			m_nReserveINETShares;	//!< How many shares INET will most show.
	BOOL		m_bReserveARCAShow;		//!< Make all ARCA order be reserve order?
	int			m_nReserveARCAShares;	//!< How many shares ARCA will most show.
	BOOL		m_bReserveTRACShow;		//!< Make all TRAC order be reserve order?
	int			m_nReserveTRACShares;	//!< How many shares TRAC will most show.
	BOOL		m_bReserveMLYNShow;		//!< Make all MLYN order be reserve order?
	int			m_nReserveMLYNShares;	//!< How many shares MLYN will most show.
	BOOL		m_bReserveDATAShow;		//!< Make all DATA order be reserve order?
	int			m_nReserveDATAShares;	//!< How many shares DATA will most show.
//@}
	BOOL		m_bSoesHitECN;			//!< SOES EX default setting
	BOOL		m_bIsldAutoRoute;		//!< Enable/disable ISLD autorouting?
	BOOL		m_bArcaAutoRoute;		//!< Enable/disable ARCA autorouting?

	LPGTECNS		m_ecns32;			//!< A pointer to map GTSetting::m_ecns
	LPGTAXES		m_axes32;			//!< A pointer to map GTSetting::m_axes
	LPGTHIDDENS		m_hiddens32;		//!< A pointer to map GTSetting::m_hidden
//@{
	//! \brief [INTERNAL USED] DO NOT REVISE.
	int				m_nChartType;		// 1: One minute chart
	int				m_nChartDays;		// 0: Current day
//@}

	/*! \brief Short sell direct send option
	
		Normally (by default) the API handles the sell and shortsell in such a way: if some position is in hand as long, the sell/shortsell 
		will be divided into two seperate orders, the sell part will sell out the position in hand, and the extra shares will be shortsell.

		Some wants to skip this seperation. They will distinct the sell and shortsell by their side. So they want, if a order sends as shortsell, 
		it will go out as a whole as shortsell, no matter what position is in hand. 

		Set this to be TRUE will enable the direct sending of a shortsell order. By default, this is FALSE.
	*/
	BOOL		m_bShortsellDirectSend;	//added by sq

	/*! \brief Option Quote Server Address
	*/
	char szOptionQuoteAddress[256];
	/*! \brief Option Quote Server Port Number
	*/
	unsigned short nOptionQuotePort;

	/*! \brief Option Level 2 Server Address
	*/
	char szOptionLevel2Address[256];
	/*! \brief Option Level 2 Server Port Number
	*/
	unsigned short nOptionLevel2Port;

	/*! \brief Option Level 2 Server Address
	*/
	char szOptionChartAddress[256];
	/*! \brief Option Level 2 Server Port Number
	*/
	unsigned short nOptionChartPort;

	/*! \brief Routing Strategy, for INET only.
		Valid values:
			INET_RS_INET, INET_RS_DOTN, INET_RS_DOTA, INET_RS_DOTM, INET_RS_DOTO, INET_RS_DOTP,
			INET_RS_DOTI, INET_RS_DOTD, INET_RS_SPDY, INET_RS_STGY, INET_RS_SCAN, INET_RS_SWIM
	*/
	MMID		m_mmidRoutingStrategy;

	//! \brief Order throttle limit. 0 for unlimit.
	int				m_nThrottleLimit;
	//! \brief Throttle interval in millisecond. default 1000.
	DWORD			m_nThrottleInterval;
	//! \brief Throttle sleep in millisecond when active. default 1.
	DWORD			m_nThrottleSleep;
}GTSetting32;
#pragma pack()

DEFINE_MAP_CLASS(gtEcn, LPGTECNS, MMID, int)
DEFINE_MAP_CLASS(gtAxe, LPGTAXES, MMID, int)
DEFINE_MAP_CLASS(gtHidden, LPGTHIDDENS, MMID, int)

#endif//__GTSETTING32_H__

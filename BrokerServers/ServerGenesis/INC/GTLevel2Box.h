/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTLevel2Box.h: interface for the GTLevel2Box class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTLevel2Box.h
	\brief interface for the GTLevel2Box class.
 */

#if !defined(__GTLEVEL2BOX_H__)
#define __GTLEVEL2BOX_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTLevel232.h"
#include "..\Inc32\GTLevel2Box32.h"

#include "GTAPI_API.h"
#include "GTBidList.h"
#include "GTAskList.h"

class GTStock;
class GTSession;
/*! \ingroup cpp
*/
/**	\class GTLevel2Box
	\brief The level2 container.
	
	All level 2 records (GTLevel2) will be put into this. 

	Many useful methods are there to help you to either getting information for the best bid/ask or retrieving
	all the level2 records. 

	The most important member of the class is m_bid and m_ask. They are the bid and ask lists that contains the
	actual bid/ask data. One can access them and calling their public method. But for most of the useful method,
	the GTLevel2Box has defined corresponding method to facilitate the information retrieving. 


 */
class GTAPI_API GTLevel2Box : public GTLevel2Box32
{
	friend GTStock;
	friend GTSession;

public:
	GTSession	&m_session;		//!< Session reference.
	GTBidList	m_bid;			//!< Bid list.
	GTAskList	m_ask;			//!< Ask list.

public:
	GTLevel2Box(GTSession &session);	//!< Constructor
	virtual ~GTLevel2Box();				//!< Destructor

	virtual void ResetContent();		//!< Reset contents.
	virtual int Dump(FILE *fp, int nLevel) const;	//!< Debug used. Dump the content of the level2 box.

public:
	BOOL IsBusy() const					//! Is it busy in transferring data?
		{ return m_bTransferring; }

//! @name Frequently used methods
//@{
	/**
	 * \brief Get the best bid price.
	 * \return The best bid price
	 * \remark Call this to get the best bid price. 
	 */
	double GetBestBidPrice() const
		{ return m_bid.m_dblBestPrice; }
	/**
	 * \brief Get the best ask price.
	 * \return The best ask price
	 * \remark Call this to get the best ask price. 
	 */
	double GetBestAskPrice() const		
		{ return m_ask.m_dblBestPrice; }

	/**
	 * \brief Get the level information of the data in the bid list.
	 * \param [in, out] pdwShares The pointer to an array (long[]) that will be filled with the share numbers of each level.
	 * \param [in, out] pdblPrices The pointer to an array (double[]) that will be filled with the prices of each level.
	 * \param [in] nLevels The maximun level number will be retrieved.
	 * \return The actual number of levels that has been filled.
	 * \remark The building of level is according to the GTLevel2::dblLevelPrice which has been filled by the API system automatically.
		The calculation of the dblLevelPrice is according to your GTSetting32::m_nLevelRate. If m_nLevelRate is 100, the level precision
		is $0.01; if m_nLevelRate is 1000, each level has the price to the precicion of $0.001. 

		\par
		All the bid record will be put into levels by their level price. Calling of this method can retrieve the information about each level.
		\par Example
		This shows how to get the level information.
		\code
			long levelShare[N];
			double levelPrice[N];
			int levelNum = stock.m_level2.GetBidLevel(levelShare, levelPrice, N);
			for(int i=0; i<levelNum; ++i){
				//do something to levelShare[i] and levelPrice[i]
			}
		\endcode
	 */
	int GetBidLevel(long *pdwShares, double *pdblPrices, int nLevels) const		
		{ return m_bid.GetLevel(pdwShares, pdblPrices, nLevels); }
	/**
	 * \brief Get the level information of the data in the ask list.
	 * \param [in, out] pdwShares The pointer to an array (long[]) that will be filled with the share numbers of each level.
	 * \param [in, out] pdblPrices The pointer to an array (double[]) that will be filled with the prices of each level.
	 * \param [in] nLevels The maximun level number will be retrieved.
	 * \return The actual number of levels that has been filled.
	 * \remark The building of level is according to the GTLevel2::dblLevelPrice which has been filled by the API system automatically.
		The calculation of the dblLevelPrice is according to your GTSetting32::m_nLevelRate. If m_nLevelRate is 100, the level precision
		is $0.01; if m_nLevelRate is 1000, each level has the price to the precicion of $0.001. 

		\par
		All the bid record will be put into levels by their level price. Calling of this method can retrieve the information about each level.
		\par Example
		This shows how to get the level information.
		\code
			long levelShare[N];
			double levelPrice[N];
			int levelNum = stock.m_level2.GetAskLevel(levelShare, levelPrice, N);
			for(int i=0; i<levelNum; ++i){
				//do something to levelShare[i] and levelPrice[i]
			}
		\endcode
	 */
	int GetAskLevel(long *pdwShares, double *pdblPrices, int nLevels) const		
		{ return m_ask.GetLevel(pdwShares, pdblPrices, nLevels); }

	/**
	 * \brief Get the level information of the data in the bid list.
	 * \param [in, out] pdwShares The pointer to an array (long[]) that will be filled with the share numbers of each level.
	 * \param [in, out] pdblPrices The pointer to an array (double[]) that will be filled with the prices of each level.
	 * \param [in] nLevels The maximun level number will be retrieved.
	 * \param [in] mmid The mmid that the level2 sent from.
	 * \return The actual number of levels that has been filled.
	 * \remark The filtered version of GetBidLevel(long *pdwShares, double *pdblPrices, int nLevels). 
	 */
	int GetBidLevel(long *pdwShares, double *pdblPrices, int nLevels, MMID mmid) const	
		{ return m_bid.GetLevel(pdwShares, pdblPrices, nLevels, mmid); }
	/**
	 * \brief Get the level information of the data in the ask list.
	 * \param [in, out] pdwShares The pointer to an array (long[]) that will be filled with the share numbers of each level.
	 * \param [in, out] pdblPrices The pointer to an array (double[]) that will be filled with the prices of each level.
	 * \param [in] nLevels The maximun level number will be retrieved.
	 * \param [in] mmid The mmid that the level2 sent from.
	 * \return The actual number of levels that has been filled.
	 * \remark The filtered version of GetAskLevel(long *pdwShares, double *pdblPrices, int nLevels). 
	 */
	int GetAskLevel(long *pdwShares, double *pdblPrices, int nLevels, MMID mmid) const  
		{ return m_ask.GetLevel(pdwShares, pdblPrices, nLevels, mmid); }

	/**
	 * \brief Get a bid item.
	 * \param [in] nItem The index of the item one wants to retrieve.
	 * \return The pointer to the item or NULL if the \c nItem is invalid.
		\remark Call this to get one record. The internal level2 data are stored sorted. The record with index 0 is always the one
		with best price. Here the best price means, for bid it is the highest bid, for ask it is the lowest ask.
		\par
		If one wants to only get the best prices, one can read m_dblBestPrice which this class has derived from GTLevel2List32. That
		will be a little bit faster then GetItem and then read.

		\note 
		You can read the information from the record, but don't edit it.
	 */
	GTLevel2 *GetBidItem(int nItem) const		
		{ return m_bid.GetItem(nItem); }
	/**
	 * \brief Get a ask item.
	 * \param [in] nItem The index of the item one wants to retrieve.
	 * \return The pointer to the item or NULL if the \c nItem is invalid.
		\remark Call this to get one record. The internal level2 data are stored sorted. The record with index 0 is always the one
		with best price. Here the best price means, for bid it is the highest bid, for ask it is the lowest ask.
		\par
		If one wants to only get the best prices, one can read m_dblBestPrice which this class has derived from GTLevel2List32. That
		will be a little bit faster then GetItem and then read.

		\note 
		You can read the information from the record, but don't edit it.
	 */
	GTLevel2 *GetAskItem(int nItem) const		
		{ return m_ask.GetItem(nItem); }

	/**
	 * \brief Get the number of bid records in the list
	 * \return The number of bid records in the list.
	 */
	int GetBidCount() const						
		{ return m_bid.GetCount(); }
	/**
	 * \brief Get the number of ask records in the list
	 * \return The number of ask records in the list.
	 */
	int GetAskCount() const						
		{ return m_ask.GetCount(); }
//@}
public:
	virtual int OnBestBidPriceChanged();		//!< Callback internally used.
	virtual int OnBestAskPriceChanged();		//!< Callback internally used.

protected:
	virtual int OnGotRecord(GTLevel2 *pRcd);	//!< Callback internally used.
	virtual int OnGotRefresh();					//!< Callback internally used.
	virtual int OnGotDisplay();					//!< Callback internally used.
	virtual int OnGotClear(MMID mmid);			//!< Callback internally used.

	virtual int OnAutoCorrectDelete(const GTLevel2 *rcd);	//!< Callback internally used.

	BOOL CanAdd(GTLevel2 *pRcd) const;			//!< Can add a new record?
	BOOL CanAddBid(GTLevel2 *pRcd) const;		//!< Can add a new record?
	BOOL CanAddAsk(GTLevel2 *pRcd) const;		//!< Can add a new record?

public:
	int CorrectLevel2();						//!< Internal used. Correct the level2. \sa GTSetting32 about autocorrect.
	/**
	 * \brief Hide the record from marketmaker 
	 * \param [in] bHide true for hide, otherwise false.
	 * \return 0.
	 */
	int HideMarketMaker(BOOL bHide = TRUE);		
};

#endif // !defined(__GTLEVEL2BOX_H__)

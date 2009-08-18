/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTLevel2List.h: interface for the GTLevel2List class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTLevel2List.h
	\brief interface for the GTLevel2List class.
 */

#if !defined(__GTLEVEL2LIST_H__)
#define __GTLEVEL2LIST_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "GTAPI_API.h"
#include "GTLevel2.h"
#include "..\Inc32\GTLevel2List32.h"

#include "GTPendingBook.h"
#include "GetECNAliasName.h"

#define GTLEVEL2_ACTION_IGNORE	0x00
#define GTLEVEL2_ACTION_INSERT	0x01
#define GTLEVEL2_ACTION_UPDATE	0x02
#define GTLEVEL2_ACTION_DELETE	0x04

typedef void (* PFNOnBestPriceChanged)(LPARAM lParam);

class GTStock;
class GTLevel2Box;
class GTSession;
/*! \ingroup cpp
*/
/*!	\class GTLevel2List
	\brief The list for bid or ask level2 records.

	This class encapsules the list of level 2 records. It is the parent for GTBidList and GTAskList, which are the components
	for class GTLevel2Box, one can access by GTStock::m_level2.

	All the basic actions for the list are introduced in this class. API user might concern about the following:
	    <div class="tablediv">
        <table class="dtTABLE" cellspacing="0">
          <tr valign="top">
            <th width="50%">Function</th>
            <th width="50%">Description</th>
          </tr>
          <tr valign="top">
            <td width="50%">GetCount
			</td>
            <td width="50%">Get the number of records in the list.
			</td>
          </tr>
          <tr valign="top">
            <td width="50%">GetItem
			</td>
            <td width="50%">Get the item by index.
			</td>
          </tr>
          <tr valign="top">
            <td width="50%">FindFirst
			</td>
            <td width="50%">Find the first record whose mmid is specified by the parameter.
			</td>
          </tr>
          <tr valign="top">
            <td width="50%">GetLevel
			</td>
            <td width="50%">Get the array for level price and share.
			</td>
          </tr>
		</table>
		</div>
	Don't try to call those method might write to the level2 records list. They are used by the internal of the API system.

	\sa GTBidList GTAskList GTLevel2Box
*/
class GTAPI_API GTLevel2List : public GTLevel2List32
{
	friend GTStock;
	friend GTLevel2Box;

protected:
	GTSession	&m_session;			/*!< The reference to session*/
	LPVOID		m_data;				//!< Actual list of data

public:
	int						m_nLastAction;			//!< Internal used
	int						m_nLastActionIndex;		//!< Internal used
	int						m_nLastActionIndex2;	//!< Internal used

	PFNOnBestPriceChanged	m_pfnOnBestPriceChanged1;		//!< Callback. Internal used. Related to event GTAPI_MSG_STOCK_OnBestBidPriceChanged.
	LPARAM					m_lParamOnBestPriceChanged1;	//!< Callback param.

	PFNOnBestPriceChanged	m_pfnOnBestPriceChanged2;		//!< Callback. Internal used. Related to event GTAPI_MSG_STOCK_OnBestBidPriceChanged.
	LPARAM					m_lParamOnBestPriceChanged2;	//!< Callback param.

public:
	GTLevel2List(GTSession &session);		//!< Constructror
	virtual ~GTLevel2List();				//!< Destructror

	virtual void ResetContent();			//!< Reset all the content. 

public:
	int Add(GTLevel2 *pRcd);				//!< Add a record.
	int Delete(int nIndex);					//!< Delete a record.
	int Clear(MMID mmid);					//!< Remove the record from mmid
	int ClearMM();							//!< Remove all the MM recod. \sa GTLevel2::IsMM
	int CheckBestPrice();					//!< Check the best price. Called internally.

	/*!	\brief Get the record number.
		\return The number of GTLevel2 records in the list.
	*/
	int GetCount() const;			

	/*! \brief Get the item
		\param [in] nItem The index of the record to retrieve.
		\return A pointer to the record, or NULL if the \c nItem is invalid. This is pointed to the original data in the list. So don't revise it.

		\remark Call this to get one record. The internal level2 data are stored sorted. The record with index 0 is always the one
		with best price. Here the best price means, for bid it is the highest bid, for ask it is the lowest ask.

		If one wants to only get the best prices, one can read m_dblBestPrice which this class has derived from GTLevel2List32. That
		will be a little bit faster then GetItem and then read.

		\note 
		You can read the information from the record, but don't edit it.
	*/
	GTLevel2 *GetItem(int nItem) const;
	/**
	 * \brief Find the first level 2 record that from the mmid.
	 * \param [in] mmid The mmid that want to find.
	 * \return A pointer to the record, or NULL if failed. This is pointed to the original data in the list. So don't revise it.
	 * \remark You can read the information from the record, but don't edit it.
	 */
	GTLevel2 *FindFirst(MMID mmid) const;
	/**
	 * \brief Get the level information of the data in the list.
	 * \param [in, out] pdwShares The pointer to an array (long[]) that will be filled with the share numbers of each level.
	 * \param [in, out] pdblPrices The pointer to an array (double[]) that will be filled with the prices of each level.
	 * \param [in] nLevels The maximun level number will be retrieved.
	 * \return The actual number of levels that has been filled.
	 * \remark The building of level is according to the GTLevel2::dblLevelPrice which has been filled by the API system automatically.
		The calculation of the dblLevelPrice is according to your GTSetting32::m_nLevelRate. If m_nLevelRate is 100, the level precision
		is $0.01; if m_nLevelRate is 1000, each level has the price to the precicion of $0.001. 

		\par
		All the level2 record will be put into levels by their level price. Calling of this method can retrieve the information about each level.
		\par Example
		This shows how to get the level information.
		\code
			long levelShare[N];
			double levelPrice[N];
			GTLevel2List &list = stock.m_level2.m_bid;
			int levelNum = list.GetLevel(levelShare, levelPrice, N);
			for(int i=0; i<levelNum; ++i){
				//do something to levelShare[i] and levelPrice[i]
			}
		\endcode
	 */
	int GetLevel(long *pdwShares, double *pdblPrices, int nLevels) const;
	/**
	 *
	 * \brief Get the level information of the data from \c mmid in the list.
	 * \param [in, out] pdwShares The pointer to an array (long[]) that will be filled with the share numbers of each level.
	 * \param [in, out] pdblPrices The pointer to an array (double[]) that will be filled with the prices of each level.
	 * \param [in] nLevels The maximun level number will be retrieved.
	 * \param [in] mmid The filter mmid.
	 * \return The actual number of levels that has been filled.
	 * \remark The building of level is according to the GTLevel2::dblLevelPrice which has been filled by the API system automatically.
		The calculation of the dblLevelPrice is according to your GTSetting32::m_nLevelRate. If m_nLevelRate is 100, the level precision
		is $0.01; if m_nLevelRate is 1000, each level has the price to the precicion of $0.001. 
		\par
		All the level2 record will be put into levels by their level price. Calling of this method can retrieve the information about each level.
		The \c mmid is the filter to filter in the specified records.
		\par Example
		This shows how to get the level information.
		\code
			long levelShare[N];
			double levelPrice[N];
			GTLevel2List &list = stock.m_level2.m_bid;
			int levelNum = list.GetLevel(levelShare, levelPrice, N, MMID_ISLD);
			for(int i=0; i<levelNum; ++i){
				//do something to levelShare[i] and levelPrice[i]
			}
		\endcode
	 */
	int GetLevel(long *pdwShares, double *pdblPrices, int nLevels, MMID mmid) const;

	int CleanLevel2Reject(MMID Method, double PendPrice, LPCSTR RejectReason);		//!< Internal used. Don't call. Clear rejected level2 record.
	int CleanLevel2Cancel(MMID Method, double PendPrice, LPCSTR CancelReason);		//!< Internal used. Don't call. Clear Cancelled level2 record.

public:
	virtual int Clear(MMID method, double price) = 0;								//!< Internal used. Don't call. 
	virtual int ClearIsldRouteOut(double dblPrice) = 0;								//!< Internal used. Don't call. 

protected:
	int AddItem(GTLevel2 *pRcd);				//!< Internal used. Don't call. Add an item
	int DeleteItem(int nIndex);					//!< Internal used. Don't call. Delete an item
	int SearchItem(const GTLevel2 *pRcd) const;	//!< Internal used. Don't call. Search an item
	void RemoveExtraLevels(int nMaxLevels = 0);	//!< Internal used. Don't call. Remove extra level

	int Refresh();								//!< Internal used. Don't call. List refresh
	int Display();								//!< Internal used. Don't call. Start display.

protected:
	virtual int OnCompareItem(const GTLevel2 *pRcd1, const GTLevel2 *pRcd2) const;	//!< Internal used. Don't call. Operator compare
	virtual void OnDeleteItem(GTLevel2 *pRcd);										//!< Internal used. Don't call. Callback OnDeleteItem
	virtual int FindItem(const GTLevel2 *pRcd) const = 0;							//!< Internal used. Don't call. Find item

	virtual void OnBestPriceChanged();												//!< Internal used. Don't call. To raise the events.

	// Pending Book
protected:
	BOOL				m_bPendingBook;				//!< Internal used. Pendingbook enabled?
	GTPendingBook		m_pendingBook;				//!< Internal used. Pendingbook

protected:
	int  UpdateOwnShares(const CMapGTPending &pendings, const GTPending32 &pending); //!< Internal used. Don't call. Update own share
	int  RemoveOwnShares(const GTPending32 &pending);								//!< Internal used. Don't call. Remove own share
	int  UpdateLevel2OwnShares(MMID method, double dblPendPrice, int nOwnShares);	//!< Internal used. Don't call. Update own share

public:
	BOOL EnablePendingBook(BOOL bEnable);		//!< Internal used. Don't call. Enable/disable pending book.
};

#endif // !defined(__GTLEVEL2LIST_H__)

/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTBidList.h: interface for the GTBidList class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTBidList.h
	\brief interface for the GTBidList class.
 */

#if !defined(__GTBIDLIST_H__)
#define __GTBIDLIST_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "GTLevel2List.h"

class GTSession;
/*! \ingroup cpp
*/
/**	\class GTBidList
 *	\brief The bid list
 *	
	This is the bid list. Some special functions for bid data are defined. Others are drived from GTLevel2List.
 */
class GTAPI_API GTBidList : public GTLevel2List  
{
public:
	GTBidList(GTSession &session);	//!< Constructor
	virtual ~GTBidList();			//!< Destructor

public:
	using GTLevel2List::Clear;		

public:
	double GetInsidePrice(double dblPrice, double *pdblBestPrice) const;	//!< Get inside price for bid.

	virtual int Clear(MMID method, double price);							//! Clear bid list.
	virtual int ClearIsldRouteOut(double dblPrice);							//! Clear bid isld routeout.

protected:
	virtual int OnCompareItem(const GTLevel2 *pRcd1, const GTLevel2 *pRcd2) const;	//! Compare function for bid.
	virtual int FindItem(const GTLevel2 *pRcd) const;						//! Find bid record.
};

#endif // !defined(__GTBIDLIST_H__)

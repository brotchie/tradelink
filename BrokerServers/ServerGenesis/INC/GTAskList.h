/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTAskList.h: interface for the GTAskList class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTAskList.h
	\brief interface for the GTAskList class.
 */

#if !defined(__GTASKLIST_H__)
#define __GTASKLIST_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "GTLevel2List.h"

class GTSession;
/*! \ingroup cpp
*/
/**	\class GTAskList
 *	\brief The ask list
 *	
	This is the ask list. Some special functions for ask data are defined. Others are drived from GTLevel2List.
 */
class GTAPI_API GTAskList : public GTLevel2List  
{
public:
	GTAskList(GTSession &session);		//!< Constructor
	virtual ~GTAskList();				//!< Destructor

public:
	using GTLevel2List::Clear;

public:
	double GetInsidePrice(double dblPrice, double *pdblBestPrice) const;		//!< Get inside price for bid.
	
	virtual int Clear(MMID mmid, double price);									//! Clear bid list.
	virtual int ClearIsldRouteOut(double price);								//! Clear bid isld routeout.

protected:
	virtual int OnCompareItem(const GTLevel2 *pRcd1, const GTLevel2 *pRcd2) const;	//! Compare function for bid.
	virtual int FindItem(const GTLevel2 *pRcd) const;							//! Find bid record.
};

#endif // !defined(__GTASKLIST_H__)

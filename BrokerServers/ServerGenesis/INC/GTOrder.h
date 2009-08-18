/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTOrder.h: interface for the GTOrder class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTOrder.h
	\brief interface for the GTOrder class.
 */

#if !defined(__GTORDER_H__)
#define __GTORDER_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTOrder32.h"

/*! \ingroup cpp
*/
/*! \struct GTOrder
	\brief The order information. The same to GTOrder32 or tagGTOrder32.

	\copydoc tagGTOrder32
*/

typedef GTOrder32 GTOrder;

/*
class GTOrder : public GTOrder32
{
	GTOrder &operator=(const GTOrder32 &user32)
	{
		*(GTOrder32 *)this = user32;
		return *this;
	}
};
*/

/*! \typedef typedef CList<GTOrder, const GTOrder &> GTOrderList
*/
/**
	\brief The list of GTOrders

	Use this type to define a list of orders. Combined with GTStock.SendOutOrders() you can send a group of orders out.

*/
typedef CList<GTOrder, const GTOrder &> GTOrderList;

#endif // !defined(__GTORDER_H__)

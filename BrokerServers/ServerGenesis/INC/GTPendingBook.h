/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTPendingBook.h: interface for the GTPendingBook class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTPendingBook.h
	\brief interface for the GTPendingBookKey, GTPendingBook class.
 */

#if !defined(__GTPENDINGBOOK_H__)
#define __GTPENDINGBOOK_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "GTPending.h"

#define PENDING_BOOK_PRICE_RATE_	10000
#define PENDING_BOOK_PRICE(p)		long((p) * PENDING_BOOK_PRICE_RATE_ + 0.49)

/*! \class GTPendingBookKey
 *	\brief Key used in the map GTPendingBook.
 * [Internal Used]
 */
struct GTPendingBookKey
{
	MMID method;
	long dwPrice;

	int operator==(const GTPendingBookKey &key) const
	{
		return method == key.method
			&& dwPrice == key.dwPrice;
	}
};

template<>
AFX_INLINE UINT AFXAPI HashKey(const GTPendingBookKey &key)
{
	return key.method * 100 + key.dwPrice;
}

/*! \class GTPendingBook
 *	\brief Pending book map. Keep own share for some method and price.
 * [Internal Used]
 */
class GTPendingBook : public CMap<GTPendingBookKey, const GTPendingBookKey &, int, int>
{
public:
	GTPendingBook();
	virtual ~GTPendingBook();

public:
	int GetAt(const GTPendingBookKey &key) const;
	int GetAt(MMID mmid, double dblPrice) const;

	void SetAt(const GTPendingBookKey &key, int value);
	BOOL RemoveKey(const GTPendingBookKey &key);
};

#endif // !defined(__GTPENDINGBOOK_H__)

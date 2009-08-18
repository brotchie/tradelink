/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTOpenPosition.h: interface for the GTOpenPosition class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTOpenPosition.h
	\brief interface for the GTOpenPositionKey, GTOpenPosition class.
 */

#if !defined(__GTOPENPOSITION_H__)
#define __GTOPENPOSITION_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTOpenPosition32.h"

/*! \ingroup cpp
*/
/*! \class GTOpenPositionKey
	\brief GTOpenPositionKey is the alias of tagGTOpenPositionKey32. 

	\copydoc tagGTOpenPositionKey32
*/
typedef tagGTOpenPositionKey32 GTOpenPositionKey;
/*
struct GTOpenPositionKey : public tagGTOpenPositionKey32
{
	GTOpenPositionKey &operator=(const GTOpenPositionKey32 &user32)
	{
		*(GTOpenPositionKey32 *)this = user32;
		return *this;
	}
};
*/

/*! \class GTOpenPosition
    \brief GTOpenPosition is the alias of GTOpenPosition32 and tagGTOpenPosition32. 

	\copydoc tagGTOpenPosition32
*/
typedef GTOpenPosition32 GTOpenPosition;
/*
struct GTOpenPosition : public GTOpenPosition32
{
	GTOpenPosition &operator=(const GTOpenPosition32 &user32)
	{
		*(GTOpenPosition32 *)this = user32;
		return *this;
	}
};
*/

template<>
AFX_INLINE UINT AFXAPI HashKey(const GTOpenPositionKey &key)
{
	return HashKey(key.szAccountID) * 100 + HashKey(key.szStock);
}

inline int operator==(const GTOpenPositionKey &key1, const GTOpenPositionKey &key2)
{
	return strcmp(key1.szAccountID, key2.szAccountID) == 0
		&& strcmp(key1.szStock,     key2.szStock) == 0;
}

typedef CMap<GTOpenPositionKey, const GTOpenPositionKey &, GTOpenPosition, const GTOpenPosition &> CMapGTOpenPosition;

#define CMapGTOpen CMapGTOpenPosition

int Dump(FILE *fp, const GTOpenPosition &open, int nLevel);
int Dump(FILE *fp, const CMapGTOpen &opens, int nLevel);

#endif // !defined(__GTOPENPOSITION_H__)

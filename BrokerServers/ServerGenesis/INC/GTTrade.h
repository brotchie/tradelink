/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTTrade.h: interface for the GTTrade class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTTrade.h
	\brief interface for the GTTrade class.
 */

#if !defined(__GTTRADE_H__)
#define __GTTRADE_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTTrade32.h"

typedef GTTradeKey32 GTTradeKey;

/*
struct GTTradeKey : public GTTradeKey32
{
	GTTradeKey &operator=(const GTTradeKey32 &user32)
	{
		*(GTTradeKey32 *)this = user32;
		return *this;
	}
};
*/
/*! \ingroup cpp
*/
/*! \class GTTrade
    \brief Trade information. The same as GTTrade32 or tagGTTrade32.

	\copydoc tagGTTrade32
*/

#define GTTrade GTTrade32
/*
struct GTTrade : public GTTrade32
{
	GTTrade &operator=(const GTTrade32 &user32)
	{
		*(GTTrade32 *)this = user32;
		return *this;
	}
};
*/

template<>
AFX_INLINE UINT AFXAPI HashKey(const GTTradeKey &key)
{
	return key.dwTicketNo + key.dwMatchNo * 100;
}

inline int operator==(const GTTradeKey &key1, const GTTradeKey &key2)
{
	return key1.dwTicketNo == key2.dwTicketNo 
		&& key1.dwMatchNo == key2.dwMatchNo;
}

inline int operator==(const GTTradeKey &key, const GTTrade &trade)
{
	return key.dwTicketNo == trade.dwTicketNo 
		&& key.dwMatchNo == trade.dwMatchNo;
}

inline int operator==(const GTTrade &trade, const GTTradeKey &key)
{
	return key.dwTicketNo == trade.dwTicketNo 
		&& key.dwMatchNo == trade.dwMatchNo;
}

inline int operator==(const GTTrade &trade1, const GTTrade &trade2)
{
	return trade1.dwTicketNo == trade2.dwTicketNo 
		&& trade1.dwMatchNo == trade2.dwMatchNo;
}

inline GTTradeKey &Copy(GTTradeKey &key, const GTTrade &trade)
{
	key.dwTicketNo = trade.dwTicketNo;
	key.dwMatchNo = trade.dwMatchNo;

	return key;
}

typedef CMap<GTTradeKey, const GTTradeKey &, GTTrade, const GTTrade &>	CMapGTTrade;

int Dump(FILE *fp, const GTTrade &trade, int nLevel);
int Dump(FILE *fp, const CMapGTTrade &trade, int nLevel);

#endif // !defined(__GTTRADE_H__)

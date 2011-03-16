#pragma once
#if !defined(LS_L_SYMBOLS_H)
#define LS_L_SYMBOLS_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

namespace LightspeedTrader
{

namespace L_SecFlag
{
	enum L_SecFlagType
	{
		ANY = -1,
		NONE = 0,
		EQUITY = 1,
		FUTURE = 2,
		OPTION = 4
	};
}

extern "C" bool L_IsListed(char const *symbol);

extern "C" bool L_IsEquity(char const *symbol);
extern "C" bool L_IsFuture(char const *symbol);
extern "C" bool L_IsIndex(char const *symbol);

}

#endif // !defined(LS_L_SYMBOLS_H)

#pragma once
#if !defined(LS_L_SYMBOLS_H)
#define LS_L_SYMBOLS_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

namespace LightspeedTrader
{

extern "C" bool L_IsListed(char const *symbol);

extern "C" bool L_IsEquity(char const *symbol);
extern "C" bool L_IsFuture(char const *symbol);
extern "C" bool L_IsIndex(char const *symbol);

}

#endif // !defined(LS_L_SYMBOLS_H)

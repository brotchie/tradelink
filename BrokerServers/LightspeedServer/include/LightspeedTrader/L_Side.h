#pragma once
#if !defined(LS_L_SIDE_H)
#define LS_L_SIDE_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

namespace LightspeedTrader
{
	namespace L_Side
	{
		const char BUY = 'B';
		const char SELL = 'S';
		const char SHORT = 'T';
		inline bool IsBuy(char s) { return s == BUY; }
		inline bool IsSell(char s) { return !IsBuy(s); }
		inline bool IsShort(char s) { return s == SHORT; }
	}
	namespace L_SideFlag
	{
		enum L_SideFlagType
		{
			ANY = -1,
			NONE = 0,
			BUY = 1,
			SELL = 2,
		};
	}
	namespace L_PosFlag
	{
		enum L_PosFlagType
		{
			ANY = -1,
			NONE = 0,
			LONG = 1,
			SHORT = 2,
			FLAT = 4,
		};
	}
}

#endif // !defined(LS_L_SIDE_H)

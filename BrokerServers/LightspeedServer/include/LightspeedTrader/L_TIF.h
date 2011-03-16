#pragma once
#if !defined(LS_L_TIF_H)
#define LS_L_TIF_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

namespace LightspeedTrader
{
	namespace L_TIF
	{
		const long IOC = 0;
		const long FOK = 99997;
		const long DAY = 99999;
	}
	namespace L_DayFlag
	{
		enum L_DayFlagType
		{
			ANY = -1,
			NONE = 0,
			DAY = 1,
			NONDAY = 2,
		};
	}
}

#endif // !defined(LS_L_TIF_H)

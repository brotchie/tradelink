#pragma once
#if !defined(LS_L_CONSTANTS_H)
#define LS_L_CONSTANTS_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

namespace LightspeedTrader
{
	const char tick_none				= 'N';
	const char tick_up					= 'U';
	const char tick_down				= 'D';
	const char tick_halted				= 'H';

	const char imbalance_add			= '+';
	const char imbalance_remove			= '-';

	const char market_morning_before	= 'M';
	const char market_premarket			= 'J';
	const char market_preopening		= 'P';
	const char market_reg_session_open	= 'F';
	const char market_ext_session_open	= 'G';
	const char market_us_market_closed	= 'D';
	const char market_halted			= 'H';
	const char market_resumed			= 'I';

	const int borrowable_unknown		= -1;
	const int borrowable_easy			= 0;
	const int borrowable_hard			= 1;
	const int borrowable_threshold		= 2;

namespace L_Liquidity
{
	const char Add = 'A';
	const char Remove = 'R';
}
namespace L_Source
{
    const char NASQ				= 'Q';
	const char ADF				= 'D';
	const char UTP				= 'U';
	const char NATIONAL			= 'N';
	const char DIRECT			= ' ';
}

namespace L_TradeChange
{
	const unsigned short Last		= 0x0001;
	const unsigned short Low		= 0x0002;
	const unsigned short High		= 0x0004;
	const unsigned short Print		= 0x0008;
}
}


#endif // !defined(LS_L_CONSTANTS_H)


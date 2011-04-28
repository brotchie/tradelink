#pragma once
#if !defined(LS_L_CONSTANTS_H)
#define LS_L_CONSTANTS_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

namespace LightspeedTrader
{

namespace L_L2Source
{
	const char Nasdaq	= 'Q';
	const char National	= 'N';
}

namespace L_Market
{
	const char MorningBefore	= 'M';
	const char Premarket		= 'J';
	const char Preopening		= 'P';
	const char RegSessionOpen	= 'F';
	const char ExtSessionOpen	= 'G';
	const char Closed			= 'D';
	const char Halted			= 'H';
	const char Resumed			= 'I';
}

namespace L_Borrowable
{
	const int Unknown		= -1;
	const int Easy			= 0;
	const int Hard			= 1;
	const int Threshold		= 2;
}

namespace L_Link
{
	enum L_LinkStatus : long
	{
		ExecutorLost		= 0,
		ExecutorEstablished	= 1,
		QuoteLost			= 2,
		QuoteEstablished	= 3,
		MarketLost			= 4,
		MarketEstablished	= 5
	};
}
namespace L_Liquidity
{
	const char Add		= 'A';
	const char Remove	= 'R';
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


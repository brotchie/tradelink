#pragma once
#if !defined(LS_L_MESSAGEIDS_H)
#define LS_L_MESSAGEIDS_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

namespace LightspeedTrader
{
namespace L_MessageRanges
{
	const long internal_start	= 50;
	const long executor_start	= 200;
	const long account_start	= 300;
	const long quote_start		= 3000;
	const long index_start		= quote_start + 100;
	const long equity_start		= quote_start + 200;
	const long option_start		= quote_start + 500;
	const long chart_start		= quote_start + 600;
	const long extension_start	= 6000;
}
namespace L_Msg
{
	const long Error					= L_MessageRanges::quote_start + 0;
	const long ECN						= L_MessageRanges::equity_start + 13;
	const long ECNUpdate				= L_MessageRanges::equity_start + 14;
	const long DirectQuotesLost			= L_MessageRanges::equity_start + 15;
	const long L1						= L_MessageRanges::equity_start + 23;
	const long L1Update					= L_MessageRanges::equity_start + 24;
	const long L1Change					= L_MessageRanges::equity_start + 28;
	const long L2						= L_MessageRanges::equity_start + 33;
	const long L2Update					= L_MessageRanges::equity_start + 35;
	const long L2Refresh				= L_MessageRanges::equity_start + 36;
	const long Trade					= L_MessageRanges::equity_start + 43;
	const long TradeUpdate				= L_MessageRanges::equity_start + 44;
	const long TradeCorrection			= L_MessageRanges::equity_start + 45;
	const long TradeClosingReport		= L_MessageRanges::equity_start + 46;
	const long StockHalted				= L_MessageRanges::equity_start + 102;
	const long StockResumed				= L_MessageRanges::equity_start + 103;
	const long OrderImbalance			= L_MessageRanges::equity_start + 104;
	const long MarketStatus				= L_MessageRanges::equity_start + 101;
	const long ECNList					= L_MessageRanges::equity_start + 107;
	const long IndicationUpdate			= L_MessageRanges::equity_start + 127;

	const long ChartSnapshot			= L_MessageRanges::chart_start + 2;
	const long ChartUpdate				= L_MessageRanges::chart_start + 4;

	const long Index					= L_MessageRanges::index_start + 4;
	const long IndexUpdate				= L_MessageRanges::index_start + 5;

	const long LinkStatus				= L_MessageRanges::account_start + 0;
	const long AccountChange			= L_MessageRanges::account_start + 1;
	const long OrderChange				= L_MessageRanges::account_start + 2;
	const long PositionChange			= L_MessageRanges::account_start + 3;
	const long IllegalShortSell			= L_MessageRanges::account_start + 4;
	const long AccountMessage			= L_MessageRanges::account_start + 5;
	const long BPChange					= L_MessageRanges::account_start + 6;
	const long ForceCancelOrder			= L_MessageRanges::account_start + 7;
	const long OrderNotConfirmed		= L_MessageRanges::account_start + 9;
	const long AccountHistoryReceived	= L_MessageRanges::account_start + 10;
	const long PositionPendingSharesChanged= L_MessageRanges::account_start + 11;
	const long PositionAdded			= L_MessageRanges::account_start + 12;
	const long OrderRepriced			= L_MessageRanges::account_start + 14;
	const long PositionDeleted			= L_MessageRanges::account_start + 15;
	const long OrderRequested			= L_MessageRanges::account_start + 17;

	const long SymbolChanged			= L_MessageRanges::extension_start + 2;
	const long ShortLimitChange			= L_MessageRanges::extension_start + 6;
}
}

#endif // !defined(LS_L_MESSAGEIDS_H)

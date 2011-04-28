#pragma once
#if !defined(LS_L_SUMMARY_H)
#define LS_L_SUMMARY_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

#include "L_Observer.h"
#include "L_Chart.h"

namespace LightspeedTrader
{

class __declspec(novtable) L_Summary : public L_Observable
{
public:
	virtual bool L_IsInit() const = 0;
	virtual bool L_IsValid() const = 0;
	virtual char const *L_Symbol() const = 0;
	virtual char const *L_CompanyName() const = 0;
	virtual char const *L_Exchange() const = 0;
	virtual bool L_IsListed() const = 0;
	virtual double L_LastPrice() const = 0;
	virtual double L_Open() const = 0;
	virtual double L_High() const = 0;
	virtual double L_Low() const = 0;
	virtual double L_Close() const = 0;
	virtual long long L_Volume() const = 0;
	virtual double L_PrevClose() const = 0;
	virtual long long L_PrevVolume() const = 0;
	virtual double L_Bid() const = 0;
	virtual double L_Ask() const = 0;
	virtual long L_BidSize() const = 0;
	virtual long L_AskSize() const = 0;
	virtual char L_FSI() const = 0;
	virtual int L_Borrowable() const = 0;
	virtual double L_MarginRequirement() const = 0;

	virtual char const *L_PrimaryMarket() const = 0;
	virtual double L_PrimaryOpen() const = 0;
	virtual double L_PrimaryClose() const = 0;
	virtual double L_PrimaryLast() const = 0;
	virtual long long L_PrimaryVolume() const = 0;
	virtual double L_PrimaryBid() const = 0;
	virtual double L_PrimaryAsk() const = 0;
	virtual long L_PrimaryBidSize() const = 0;
	virtual long L_PrimaryAskSize() const = 0;

	virtual long long L_OIBuyVolumeReg() const = 0;
	virtual long long L_OISellVolumeReg() const = 0;
	virtual long long L_OIBuyVolume() const = 0;
	virtual long long L_OISellVolume() const = 0;
	virtual long long L_OITotalVolume() const = 0;
	virtual double L_OIRefPrice() const = 0;
	virtual char L_OICrossType() const = 0;
	virtual char L_OIMarket() const = 0;
	virtual double L_OIClearingPrice() const = 0;
	virtual double L_OINearPrice() const = 0;
	virtual double L_OIFarPrice() const = 0;
	virtual double L_OIContinuousPrice() const = 0;
	virtual double L_OIClosingOnlyPrice() const = 0;
	virtual time_t L_OITime() const = 0;

	virtual double L_PrimaryPrevClose() const = 0;
	virtual char L_SSRI() const = 0;

	virtual L_Chart *L_GetChart() const = 0;

	virtual double L_TFHigh(long timeframe) const = 0;
	virtual double L_TFLow(long timeframe) const = 0;
	virtual double L_TFPrevClose(long timeframe) const = 0;
	virtual long long L_TFVolume(long timeframe) const = 0;
	virtual double L_TFChange(long timeframe) const = 0;
	virtual double L_TFPercentChange(long timeframe) const = 0;
	virtual long L_TFHighLow(long timeframe) const = 0;
	virtual double L_TFPriceRange(long timeframe) const = 0;
	virtual double L_TFPercentPriceRange(long timeframe) const = 0;
	virtual double L_TFVolumeRate(long timeframe) const = 0;

	virtual long L_LastSize() const = 0;
	virtual long L_LastTime() const = 0;
	virtual long L_UpdateTime() const = 0;
	
	virtual double L_Gap() const = 0;
	virtual double L_GapPercent() const = 0;
	virtual double L_GapBid() const = 0;
	virtual double L_GapAsk() const = 0;
	virtual double L_PrimaryGap() const = 0;
	virtual double L_PrimaryGapPercent() const = 0;
	virtual double L_PrimaryGapBid() const = 0;
	virtual double L_PrimaryGapAsk() const = 0;

	virtual double L_IndicationBid() const = 0;
	virtual double L_IndicationAsk() const = 0;
	virtual long L_IndicationTime() const = 0;
};

}

#endif // !defined(LS_L_SUMMARY_H)


#include "stdafx.h"
#include "EWrapper.h"
// utility funcs
CString getField( TickType tickType) {
	switch( tickType)
	{
		case BID_SIZE:	                    return "bidSize";
		case BID:		                    return "bidPrice";
		case ASK:		                    return "askPrice";
		case ASK_SIZE:	                    return "askSize";
		case LAST:		                    return "lastPrice";
		case LAST_SIZE:	                    return "lastSize";
		case HIGH:		                    return "high";
		case LOW:		                    return "low";
		case VOLUME:	                    return "volume";
		case CLOSE:		                    return "close";
		case BID_OPTION_COMPUTATION:		return "bidOptComp";
		case ASK_OPTION_COMPUTATION:		return "askOptComp";
		case LAST_OPTION_COMPUTATION:		return "lastOptComp";
		case MODEL_OPTION:					return "optionModel";
        case OPEN:                          return "open";
        case LOW_13_WEEK:                   return "13WeekLow";
        case HIGH_13_WEEK:                  return "13WeekHigh";
        case LOW_26_WEEK:                   return "26WeekLow";
        case HIGH_26_WEEK:                  return "26WeekHigh";
        case LOW_52_WEEK:                   return "52WeekLow";
        case HIGH_52_WEEK:                  return "52WeekHigh";
        case AVG_VOLUME:                    return "AvgVolume";
        case OPEN_INTEREST:                 return "OpenInterest";
        case OPTION_HISTORICAL_VOL:         return "OptionHistoricalVolatility";
        case OPTION_IMPLIED_VOL:            return "OptionImpliedVolatility";
        case OPTION_BID_EXCH:               return "OptionBidExchStr";
        case OPTION_ASK_EXCH:               return "OptionAskExchStr";
        case OPTION_CALL_OPEN_INTEREST:     return "OptionCallOpenInterest";
        case OPTION_PUT_OPEN_INTEREST:      return "OptionPutOpenInterest";
        case OPTION_CALL_VOLUME:            return "OptionCallVolume";
        case OPTION_PUT_VOLUME:             return "OptionPutVolume";
        case INDEX_FUTURE_PREMIUM:          return "IndexFuturePremium";
        case BID_EXCH:                      return "bidExch";
        case ASK_EXCH:                      return "askExch";
        case AUCTION_VOLUME:                return "auctionVolume";
        case AUCTION_PRICE:                 return "auctionPrice";
        case AUCTION_IMBALANCE:             return "auctionImbalance";
        case MARK_PRICE:                    return "markPrice";
        case BID_EFP_COMPUTATION:           return "bidEFP";
        case ASK_EFP_COMPUTATION:           return "askEFP";
        case LAST_EFP_COMPUTATION:          return "lastEFP";
        case OPEN_EFP_COMPUTATION:          return "openEFP";
        case HIGH_EFP_COMPUTATION:          return "highEFP";
        case LOW_EFP_COMPUTATION:           return "lowEFP";
        case CLOSE_EFP_COMPUTATION:         return "closeEFP";
        case LAST_TIMESTAMP:                return "lastTimestamp";
        case SHORTABLE:                     return "shortable";
		default:		                    return "unknown";
	}
}
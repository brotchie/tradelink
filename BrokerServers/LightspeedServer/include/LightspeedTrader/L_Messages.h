#pragma once
#if !defined(LS_L_MESSAGES_H)
#define LS_L_MESSAGES_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

#include "L_MessageIds.h"
#include "L_Iterator.h"

namespace LightspeedTrader
{

class L_Execution;

class __declspec(novtable) L_Message
{
public:
	virtual long L_Type() const = 0;
	virtual char const *L_Symbol() const
	{
		return "";
	}
};

///
// Quote Messages
//

class __declspec(novtable) L_MsgError : public L_Message
{
public:
	enum { id = L_Msg::Error };
};

class __declspec(novtable) L_MsgTrade : public L_Message
{
public:
	enum { id = L_Msg::Trade };
	class __declspec(novtable) L_TradeTick
	{
	public:
		virtual double L_Price() const = 0;
		virtual long L_Volume() const = 0;
		virtual long L_ChangeIndicator() const = 0;
		virtual __time64_t L_Time() const = 0;
		virtual char const *L_Market() const = 0;
		virtual char L_SaleCondition() const = 0;
	};
	virtual L_TradeTick const *L_Begin() const = 0;
	virtual L_TradeTick const *L_End() const = 0;
	virtual size_t L_ValueSize() const = 0;

	L_TradeTick const *Next(L_TradeTick const *from) const { return reinterpret_cast<L_TradeTick const *>(reinterpret_cast<char const *>(from) + L_ValueSize()); }

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
	typedef C_PointerConstIterator<L_MsgTrade, L_TradeTick> iterator;
	iterator begin() const
	{
		return iterator(this, L_Begin());
	}
	iterator end() const
	{
		return iterator(this, L_End());
	}
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
};

class __declspec(novtable) L_MsgTradeUpdate : public L_Message
{
public:
	enum { id = L_Msg::TradeUpdate };

	virtual double L_Price() const = 0;
	virtual long long L_Volume() const = 0;
	virtual bool L_Printable() const = 0;
	virtual char const *L_Market() const = 0;
	virtual char L_SaleCondition() const = 0;
	virtual __time64_t L_Time() const = 0;
	virtual long L_ChangeIndicator() const = 0;
};

class __declspec(novtable) L_MsgTradeCorrection : public L_Message
{
public:
	enum { id = L_Msg::TradeCorrection };

	virtual double L_High() const = 0;
	virtual double L_Low() const = 0;
	virtual double L_Last() const = 0;
	virtual long long L_Volume() const = 0;
};

class __declspec(novtable) L_MsgTradeClosingReport : public L_Message
{
public:
	enum { id = L_Msg::TradeClosingReport };

	virtual double L_High() const = 0;
	virtual double L_Low() const = 0;
	virtual double L_Last() const = 0;
	virtual long long L_Volume() const = 0;
};

class __declspec(novtable) L_MsgL1 : public L_Message
{
public:
	enum { id = L_Msg::L1 };

	virtual char const *L_CompanyName() const = 0;
	virtual char const *L_Market() const = 0;
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
	virtual double L_PrimaryPrevClose() const = 0;
	virtual char L_SSRI() const = 0;
};

class __declspec(novtable) L_MsgL1Change : public L_Message
{
public:
	enum { id = L_Msg::L1Change };

	virtual char const *L_CompanyName() const = 0;
	virtual char const *L_Market() const = 0;
	virtual double L_Open() const = 0;
	virtual double L_High() const = 0;
	virtual double L_Low() const = 0;
	virtual double L_Close() const = 0;
	virtual long long L_Volume() const = 0;
	virtual double L_PrevClose() const = 0;
	virtual int L_Borrowable() const = 0;
	virtual double L_MarginRequirement() const = 0;
	virtual char L_SSRI() const = 0;
	virtual double L_PrimaryOpen() const = 0;
	virtual double L_PrimaryClose() const = 0;
};

class __declspec(novtable) L_MsgL1Update : public L_Message
{
public:
	enum { id = L_Msg::L1Update };

	virtual double L_Bid() const = 0;
	virtual double L_Ask() const = 0;
	virtual long L_BidSize() const = 0;
	virtual long L_AskSize() const = 0;
	virtual double L_Last() const = 0;
	virtual double L_UpdateLow() const = 0;
	virtual double L_UpdateHigh() const = 0;
	virtual long long L_Volume() const = 0;
	virtual char L_Source() const = 0;
	virtual double L_PrimaryLast() const = 0;
	virtual long long L_PrimaryVolume() const = 0;
	virtual double L_PrimaryBid() const = 0;
	virtual double L_PrimaryAsk() const = 0;
	virtual long L_PrimaryBidSize() const = 0;
	virtual long L_PrimaryAskSize() const = 0;
};

class __declspec(novtable) L_MsgL2 : public L_Message
{
public:
	enum { id = L_Msg::L2 };

	class __declspec(novtable) L_MMQuote
	{
	public:
		virtual char const *L_MMID() const = 0;
		virtual double L_Price() const = 0;
		virtual bool L_Closed() const = 0;
		virtual char L_Condition() const = 0;
		virtual long L_Size() const = 0;
		virtual long L_Time() const = 0;
	};

	virtual char L_Source() const = 0;
	virtual L_MMQuote const *L_BidsBegin() const = 0;
	virtual L_MMQuote const *L_BidsEnd() const = 0;
	virtual L_MMQuote const *L_AsksBegin() const = 0;
	virtual L_MMQuote const *L_AsksEnd() const = 0;
	virtual size_t L_ValueSize() const = 0;

	L_MMQuote const *Next(L_MMQuote const *from) const { return reinterpret_cast<L_MMQuote const *>(reinterpret_cast<char const *>(from) + L_ValueSize()); }

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
	typedef C_PointerConstIterator<L_MsgL2, L_MMQuote> iterator;
	iterator bids_begin() const
	{
		return iterator(this, L_BidsBegin());
	}
	iterator bids_end() const
	{
		return iterator(this, L_BidsEnd());
	}
	iterator asks_begin() const
	{
		return iterator(this, L_AsksBegin());
	}
	iterator asks_end() const
	{
		return iterator(this, L_AsksEnd());
	}
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
};

class __declspec(novtable) L_MsgL2Refresh : public L_MsgL2
{
public:
	enum { id = L_Msg::L2Refresh };
};

class __declspec(novtable) L_MsgL2Update : public L_Message
{
public:
	enum { id = L_Msg::L2Update };

	virtual char L_Source() const = 0;
	virtual char const *L_MMID() const = 0;
	virtual double L_Bid() const = 0;
	virtual double L_Ask() const = 0;
	virtual long L_BidSize() const = 0;
	virtual long L_AskSize() const = 0;
	virtual bool L_Closed() const = 0;
	virtual char L_Condition() const = 0;
	virtual long L_Time() const = 0;
};

class __declspec(novtable) L_MsgECNList : public L_Message
{
public:
	enum { id = L_Msg::ECNList };

	class __declspec(novtable) L_ECNListIterator
	{
	public:
		typedef std::bidirectional_iterator_tag l_iterator_category;
		typedef ptrdiff_t l_distance_type;
		typedef char const *l_value_type;
		typedef l_value_type const *l_pointer;
		typedef l_value_type l_reference;
		typedef size_t l_size_type;
		typedef l_value_type const *l_const_pointer;
		typedef l_value_type l_const_reference;
		typedef L_ECNListIterator l_this_iterator;

		virtual l_this_iterator *L_Copy() const = 0;
		virtual void L_Destroy() = 0;
		virtual bool L_IsEqual(l_this_iterator const *) const = 0;
		virtual void L_Increment() = 0;
		virtual void L_Decrement() = 0;
		virtual l_const_reference L_ConstDeref() const = 0;
		virtual void const *L_DirectData() const = 0;
	};

	virtual L_ECNListIterator const *L_Begin() const = 0;
	virtual L_ECNListIterator const *L_End() const = 0;

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
	typedef C_WrappedConstIterator<L_ECNListIterator> iterator;
	iterator begin() const
	{
		return iterator(L_Begin());
	}
	iterator end() const
	{
		return iterator(L_End());
	}
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
};

class __declspec(novtable) L_MsgECN : public L_Message
{
public:
	enum { id = L_Msg::ECN };

	class __declspec(novtable) L_ECNQuote
	{
	public:
		virtual double L_Price() const = 0;
		virtual long L_Size() const = 0;
		virtual long L_Time() const = 0;
	};
	virtual char const *L_MMID() const = 0;
	virtual L_ECNQuote const *L_BidsBegin() const = 0;
	virtual L_ECNQuote const *L_BidsEnd() const = 0;
	virtual L_ECNQuote const *L_AsksBegin() const = 0;
	virtual L_ECNQuote const *L_AsksEnd() const = 0;
	virtual size_t L_ValueSize() const = 0;

	L_ECNQuote const *Next(L_ECNQuote const *from) const { return reinterpret_cast<L_ECNQuote const *>(reinterpret_cast<char const *>(from) + L_ValueSize()); }

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
	typedef C_PointerConstIterator<L_MsgECN, L_ECNQuote> iterator;
	iterator bids_begin() const
	{
		return iterator(this, L_BidsBegin());
	}
	iterator bids_end() const
	{
		return iterator(this, L_BidsEnd());
	}
	iterator asks_begin() const
	{
		return iterator(this, L_AsksBegin());
	}
	iterator asks_end() const
	{
		return iterator(this, L_AsksEnd());
	}
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
};

class __declspec(novtable) L_MsgECNUpdate : public L_Message
{
public:
	enum { id = L_Msg::ECNUpdate };

	virtual char const *L_MMID() const = 0;
	virtual char L_Side() const = 0;
	virtual double L_Price() const = 0;
	virtual long L_Size() const = 0;
	virtual long L_Time() const = 0;
};

class __declspec(novtable) L_MsgMarketStatus : public L_Message
{
public:
	enum { id = L_Msg::MarketStatus };

	virtual char L_Status() const = 0;
};

class __declspec(novtable) L_MsgStockHalted : public L_Message
{
public:
	enum { id = L_Msg::StockHalted };

	virtual __time64_t L_Time() const = 0;
	virtual char const *L_Reason() const = 0;
};

class __declspec(novtable) L_MsgStockResumed : public L_Message
{
public:
	enum { id = L_Msg::StockResumed };

	virtual __time64_t L_Time() const = 0;
};

class __declspec(novtable) L_MsgDirectQuotesLost : public L_Message
{
public:
	enum { id = L_Msg::DirectQuotesLost };

	virtual char const *L_MMID() const = 0;
};

class __declspec(novtable) L_MsgOrderImbalance : public L_Message
{
public:
	enum { id = L_Msg::OrderImbalance };

	virtual bool L_Add() const = 0;
	virtual char L_RegImbalance() const = 0;
	virtual long long L_BuyVolumeReg() const = 0;
	virtual long long L_SellVolumeReg() const = 0;
	virtual long long L_BuyVolume() const = 0;
	virtual long long L_SellVolume() const = 0;
	virtual long long L_TotalVolume() const = 0;
	virtual double L_RefPrice() const = 0;
	virtual char L_CrossType() const = 0;
	virtual char L_Market() const = 0;
	virtual double L_ClearingPrice() const = 0;
	virtual double L_NearPrice() const = 0;
	virtual double L_FarPrice() const = 0;
	virtual double L_ContinuousPrice() const = 0;
	virtual double L_ClosingOnlyPrice() const = 0;
	virtual __time64_t L_Time() const = 0;
};

class __declspec(novtable) L_MsgChartSnapshot : public L_Message
{
public:
	enum { id = L_Msg::ChartSnapshot };

};

class __declspec(novtable) L_MsgChartUpdate : public L_Message
{
public:
	enum { id = L_Msg::ChartUpdate };

	virtual double L_Open() const = 0;
	virtual double L_High() const = 0;
	virtual double L_Low() const = 0;
	virtual double L_Close() const = 0;
	virtual long long L_Volume() const = 0;
	virtual __time64_t L_Time() const = 0;
};

class __declspec(novtable) L_MsgIndex : public L_Message
{
public:
	enum { id = L_Msg::Index };

	virtual double L_Value() const = 0;
	virtual double L_Change() const = 0;
	virtual double L_Open() const = 0;
	virtual double L_High() const = 0;
	virtual double L_Low() const = 0;
	virtual char const *L_Exchange() const = 0;
};

class __declspec(novtable) L_MsgIndexUpdate : public L_Message
{
public:
	enum { id = L_Msg::IndexUpdate };

	virtual double L_Value() const = 0;
	virtual double L_Change() const = 0;
};

class L_MsgIndicationUpdate : public L_Message
{
public:
	enum { id = L_Msg::IndicationUpdate };
	virtual double L_Bid() const = 0;
	virtual double L_Ask() const = 0;
	virtual long L_Time() const = 0;
};

//
// Quote Messages
///

///
// Account Messages
//

class __declspec(novtable) L_MsgAccountChange : public L_Message
{
public:
	enum { id = L_Msg::AccountChange };

};

class __declspec(novtable) L_MsgOrderRequested : public L_Message
{
public:
	enum { id = L_Msg::OrderRequested };

	virtual long L_CorrelationId() const = 0;
	virtual long L_Result() const = 0;
	virtual unsigned int L_SharesSent() const = 0;
	virtual long L_Order1ReferenceId() const = 0;
	virtual long L_Order2ReferenceId() const = 0;
	virtual long L_SSMReplacedReferenceId() const = 0;
};

namespace L_OrderChange
{
enum L_OrderChangeEnum
{
	Other = 0,
	Create = 1,
	Receive = 2,
	Exec = 3,
	CancelCreate = 4,
	CancelRejection = 5,
	Cancel = 6,
	Kill = 7,
	Rejection = 8
};
}
class __declspec(novtable) L_MsgOrderChange : public L_Message
{
public:
	enum { id = L_Msg::OrderChange };

	virtual long L_ReferenceId() const = 0;
	virtual long L_OrderId() const = 0;
	virtual long L_Category() const = 0;
	virtual L_Execution const *L_Exec() const = 0;
};

class __declspec(novtable) L_MsgPositionChange : public L_Message
{
public:
	enum { id = L_Msg::PositionChange };

};

class __declspec(novtable) L_MsgShortLimitChange : public L_Message
{
public:
	enum { id = L_Msg::ShortLimitChange };
	virtual long L_Size() const = 0;
};

//
// Account Messages
///

///
// App Notifier Messages

class __declspec(novtable) L_MsgSymbolChanged : public L_Message
{
public:
	enum { id = L_Msg::SymbolChanged };
};

class __declspec(novtable) L_MsgLinkStatus : public L_Message
{
public:
	enum { id = L_Msg::LinkStatus };

	virtual long L_Status() const = 0;
};

// App Notifier Messages
///

} // namespace LightspeedTrader



#endif // !defined(LS_L_MESSAGES_H)


#pragma once
#if !defined(LS_L_FULLQUOTE_H)
#define LS_L_FULLQUOTE_H

// Copyright © 2010-2011 Lightspeed Financial, Inc. All rights reserved.

#include "L_Observer.h"
#include "L_Iterator.h"
#include "L_Constants.h"

namespace LightspeedTrader
{

class __declspec(novtable) L_Quote
{
public:
	virtual char const *L_MMID() const = 0;
	virtual bool L_IsBid() const = 0;
	virtual double L_Price() const = 0;
	virtual long L_Size() const = 0;
	virtual long L_Time() const = 0;
	virtual char L_Condition() const = 0;

	virtual bool L_IsECN() const = 0;
	virtual bool L_IsDirect() const = 0;
	virtual char L_Source() const = 0;
	virtual bool L_IsClosed() const = 0;
	virtual bool L_IsCrossed() const = 0;
};

class __declspec(novtable) L_QuoteIterator
{
public:
	typedef std::bidirectional_iterator_tag l_iterator_category;
	typedef ptrdiff_t l_distance_type;
	typedef L_Quote l_value_type;
	typedef l_value_type const *l_pointer;
	typedef l_value_type &l_reference;
	typedef size_t l_size_type;
	typedef l_value_type const *l_const_pointer;
	typedef l_value_type const &l_const_reference;
	typedef L_QuoteIterator l_this_iterator;

	virtual l_this_iterator *L_Copy() const = 0;
	virtual void L_Destroy() = 0;
	virtual bool L_IsEqual(l_this_iterator const *) const = 0;
	virtual void L_Increment() = 0;
	virtual void L_Decrement() = 0;
	virtual l_const_reference L_ConstDeref() const = 0;
	virtual void const *L_DirectData() const = 0;
};

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
typedef C_WrappedConstIterator<L_QuoteIterator> quote_iterator;
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)

class __declspec(novtable) L_DirectBookIterator
{
public:
	typedef std::forward_iterator_tag l_iterator_category;
	typedef ptrdiff_t l_distance_type;
	typedef char const *l_value_type;
	typedef l_value_type const *l_pointer;
	typedef l_value_type l_reference;
	typedef size_t l_size_type;
	typedef l_value_type const *l_const_pointer;
	typedef l_value_type l_const_reference;
	typedef L_DirectBookIterator l_this_iterator;

	virtual l_this_iterator *L_Copy() const = 0;
	virtual void L_Destroy() = 0;
	virtual bool L_IsEqual(l_this_iterator const *) const = 0;
	virtual void L_Increment() = 0;
	virtual l_const_reference L_ConstDeref() const = 0;
	virtual void const *L_DirectData() const = 0;
};

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
typedef C_WrappedConstIterator<L_DirectBookIterator> direct_book_iterator;
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)

class __declspec(novtable) L_Trade
{
public:
	virtual double L_Price() const = 0;
	virtual long L_Volume() const = 0;
	virtual long L_ChangeIndicator() const = 0;
	virtual time_t L_Time() const = 0;
	virtual char const *L_Market() const = 0;
	virtual char L_SaleCondition() const = 0;
};

class __declspec(novtable) L_TradeIterator
{
public:
	typedef std::bidirectional_iterator_tag l_iterator_category;
	typedef ptrdiff_t l_distance_type;
	typedef L_Trade l_value_type;
	typedef l_value_type *l_pointer;
	typedef l_value_type &l_reference;
	typedef size_t l_size_type;
	typedef l_value_type const *l_const_pointer;
	typedef l_value_type const &l_const_reference;
	typedef L_TradeIterator l_this_iterator;

	virtual l_this_iterator *L_Copy() const = 0;
	virtual void L_Destroy() = 0;
	virtual bool L_IsEqual(l_this_iterator const *) const = 0;
	virtual void L_Increment() = 0;
	virtual void L_Decrement() = 0;
	virtual l_const_reference L_ConstDeref() const = 0;
	virtual void const *L_DirectData() const = 0;
};

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
typedef C_WrappedConstIterator<L_TradeIterator> trade_iterator;
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)

class __declspec(novtable) L_FullQuote : public L_Observable
{
public:
	virtual bool L_IsInit() const = 0;
	virtual bool L_IsValid() const = 0;
	virtual char const *L_Symbol() const = 0;
	virtual char const *L_Exchange() const = 0;
	virtual char const *L_PrimaryMarket() const = 0;
	virtual bool L_IsListed() const = 0;
	virtual double L_MarginRequirement() const = 0;
	virtual char const *L_CompanyName() const = 0;

	virtual double L_LastPrice() const = 0;
	virtual long L_LastSize() const = 0;
	virtual time_t L_LastTime() const = 0;
	virtual long long L_Volume() const = 0;

	virtual double L_Bid() const = 0;
	virtual double L_Ask() const = 0;
	virtual long L_BidSize() const = 0;
	virtual long L_AskSize() const = 0;

	virtual double L_VisibleBid() const = 0;
	virtual double L_VisibleAsk() const = 0;
	virtual long L_VisibleBidSize() const = 0;
	virtual long L_VisibleAskSize() const = 0;

	virtual L_Quote const *L_BestECNQuote(bool bid, char const *market) const = 0;
	virtual L_Quote const *L_BestL2Quote(bool bid, char const *market, char l2Source = L_L2Source::Nasdaq) const = 0;
	virtual L_Quote const *L_BestQuote(bool bid, char const *market) const = 0;

	virtual L_TradeIterator const *L_TradesBegin() const = 0;
	virtual L_TradeIterator const *L_TradesEnd() const = 0;
	virtual L_QuoteIterator const *L_BidsBegin() const = 0;
	virtual L_QuoteIterator const *L_BidsEnd() const = 0;
	virtual L_QuoteIterator const *L_AsksBegin() const = 0;
	virtual L_QuoteIterator const *L_AsksEnd() const = 0;
	virtual L_QuoteIterator const *L_ECNBidsBegin(char const *ecn) const = 0;
	virtual L_QuoteIterator const *L_ECNBidsEnd(char const *ecn) const = 0;
	virtual L_QuoteIterator const *L_ECNAsksBegin(char const *ecn) const = 0;
	virtual L_QuoteIterator const *L_ECNAsksEnd(char const *ecn) const = 0;
	virtual L_QuoteIterator const *L_L2BidsBegin(char source) const = 0;
	virtual L_QuoteIterator const *L_L2BidsEnd(char source) const = 0;
	virtual L_QuoteIterator const *L_L2AsksBegin(char source) const = 0;
	virtual L_QuoteIterator const *L_L2AsksEnd(char source) const = 0;
	virtual L_DirectBookIterator const *L_ECNNamesBegin() const = 0;
	virtual L_DirectBookIterator const *L_ECNNamesEnd() const = 0;

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
	trade_iterator trades_begin() const
	{
		return trade_iterator(L_TradesBegin());
	}
	trade_iterator trades_end() const
	{
		return trade_iterator(L_TradesEnd());
	}


	quote_iterator bids_begin() const
	{
		return quote_iterator(L_BidsBegin());
	}
	quote_iterator bids_end() const
	{
		return quote_iterator(L_BidsEnd());
	}

	quote_iterator asks_begin() const
	{
		return quote_iterator(L_AsksBegin());
	}
	quote_iterator asks_end() const
	{
		return quote_iterator(L_AsksEnd());
	}


	quote_iterator ecn_bids_begin(char const *ecn) const
	{
		return quote_iterator(L_ECNBidsBegin(ecn));
	}
	quote_iterator ecn_bids_end(char const *ecn) const
	{
		return quote_iterator(L_ECNBidsEnd(ecn));
	}

	quote_iterator ecn_asks_begin(char const *ecn) const
	{
		return quote_iterator(L_ECNAsksBegin(ecn));
	}
	quote_iterator ecn_asks_end(char const *ecn) const
	{
		return quote_iterator(L_ECNAsksEnd(ecn));
	}


	quote_iterator l2_bids_begin(char source) const
	{
		return quote_iterator(L_L2BidsBegin(source));
	}
	quote_iterator l2_bids_end(char source) const
	{
		return quote_iterator(L_L2BidsEnd(source));
	}

	quote_iterator l2_asks_begin(char source) const
	{
		return quote_iterator(L_L2AsksBegin(source));
	}
	quote_iterator l2_asks_end(char source) const
	{
		return quote_iterator(L_L2AsksEnd(source));
	}
	direct_book_iterator ecn_names_begin() const
	{
		return direct_book_iterator(L_ECNNamesBegin());
	}
	direct_book_iterator ecn_names_end() const
	{
		return direct_book_iterator(L_ECNNamesEnd());
	}
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
};

}


#endif // !defined(LS_L_FULLQUOTE_H)


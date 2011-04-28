#pragma once
#if !defined(LS_L_ACCOUNT_H)
#define LS_L_ACCOUNT_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

#include "L_Observer.h"
#include "L_Position.h"
#include "L_Order.h"
#include "L_BPType.h"
#include "L_Side.h"
#include "L_TIF.h"
#include "L_Constants.h"
#include "L_Symbols.h"
#include "L_Iterator.h"
#include "L_Execution.h"

namespace LightspeedTrader
{

class L_Summary;

class __declspec(novtable) L_LocateSymbolIterator
{
public:
	typedef std::forward_iterator_tag l_iterator_category;
	typedef ptrdiff_t l_distance_type;
	typedef char const *l_value_type;
	typedef l_value_type *l_pointer;
	typedef l_value_type l_reference;
	typedef size_t l_size_type;
	typedef l_value_type const *l_const_pointer;
	typedef l_value_type l_const_reference;
	typedef L_LocateSymbolIterator l_this_iterator;

	virtual l_this_iterator *L_Copy() const = 0;
	virtual void L_Destroy() = 0;
	virtual bool L_IsEqual(l_this_iterator const *) const = 0;
	virtual void L_Increment() = 0;
	virtual l_const_reference L_ConstDeref() const = 0;
	virtual void const *L_DirectData() const = 0;
};

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
typedef C_WrappedConstIterator<L_LocateSymbolIterator> locate_symbol_iterator;
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)


class __declspec(novtable) L_Account : public L_Observable
{
public:
	virtual char const *L_TraderId() const = 0;
	virtual char const *L_Firm() const = 0;
	virtual double L_MarginFactor() const = 0;

	virtual void L_SendOrder(
		L_Summary const *summary,
		long type,
		char side,
		unsigned long shares,
		double price,
		char const *market,
		long tif,
		bool hidden = false,
		unsigned long visibleShares = 0,
		double price2 = 0.0,
		char const *market2 = 0,
		long *correlationId = 0,
		char const *info = 0
		) = 0;

	virtual void L_CancelOrder(
		L_Order const *order
		) = 0;

	virtual double L_ClosedPL(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_OpenPL(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_MarkedPL(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_NetPL(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_MinClosedPL(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_MaxClosedPL(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_MaxIntradayValue(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_MaxIntradayDollarValue(int type = L_BPType::DEFAULT) const = 0;

	virtual double L_BuyingPowerInUse(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_BuyingPower(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_BaseBuyingPower(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_FloatingBPMultiple(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_Equity(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_RunningBalance(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_Value(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_LongValue(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_ShortValue(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_DollarValue(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_LongDollarValue(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_ShortDollarValue(int type = L_BPType::DEFAULT) const = 0;
	virtual double L_NetDollarValue(int type = L_BPType::DEFAULT) const = 0;

	virtual long L_OpenPositionsCount(int type = L_BPType::DEFAULT) const = 0;
	virtual long L_LongOpenPositionsCount(int type = L_BPType::DEFAULT) const = 0;
	virtual long L_ShortOpenPositionsCount(int type = L_BPType::DEFAULT) const = 0;
	virtual long L_PendingOrdersCount(int type = L_BPType::DEFAULT) const = 0;
	virtual long L_PendingBuyOrdersCount(int type = L_BPType::DEFAULT) const = 0;
	virtual long L_PendingSellOrdersCount(int type = L_BPType::DEFAULT) const = 0;
	virtual long L_SharesTraded(int type = L_BPType::DEFAULT) const = 0;
	virtual long L_NumTrades(int type = L_BPType::DEFAULT) const = 0;
	virtual long L_NumExecutions(int type = L_BPType::DEFAULT) const = 0;
	virtual long L_AddedLiquidity(int type = L_BPType::DEFAULT) const = 0;
	virtual long L_AddedLiquidityExecutions(int type = L_BPType::DEFAULT) const = 0;
	virtual long L_RemovedLiquidity(int type = L_BPType::DEFAULT) const = 0;
	virtual long L_RemovedLiquidityExecutions(int type = L_BPType::DEFAULT) const = 0;

	virtual long L_GetShortPositionLimit(char const *symbol) const = 0;

	virtual L_Position *L_FindPosition(char const *symbol) const = 0;

	virtual L_PositionListIterator const *L_PositionsBegin() const = 0;
	virtual L_PositionListIterator const *L_PositionsEnd() const = 0;
	virtual L_PositionListIterator const *L_OpenPositionsBegin() const = 0;
	virtual L_PositionListIterator const *L_OpenPositionsEnd() const = 0;
	virtual L_PositionListIterator const *L_LongPositionsBegin() const = 0;
	virtual L_PositionListIterator const *L_LongPositionsEnd() const = 0;
	virtual L_PositionListIterator const *L_ShortPositionsBegin() const = 0;
	virtual L_PositionListIterator const *L_ShortPositionsEnd() const = 0;

	virtual L_Order *L_FindOrder(long referenceId) const = 0;
	virtual L_Order *L_FindOrderByOrderId(long orderId) const = 0;

	virtual L_OrderListIterator const *L_OrdersBegin() const = 0;
	virtual L_OrderListIterator const *L_OrdersEnd() const = 0;
	virtual L_OrderListIterator const *L_OrdersBuyBegin() const = 0;
	virtual L_OrderListIterator const *L_OrdersBuyEnd() const = 0;
	virtual L_OrderListIterator const *L_OrdersSellBegin() const = 0;
	virtual L_OrderListIterator const *L_OrdersSellEnd() const = 0;
	virtual L_OrderListIterator const *L_ActiveOrdersBegin() const = 0;
	virtual L_OrderListIterator const *L_ActiveOrdersEnd() const = 0;
	virtual L_OrderListIterator const *L_ActiveOrdersBuyBegin() const = 0;
	virtual L_OrderListIterator const *L_ActiveOrdersBuyEnd() const = 0;
	virtual L_OrderListIterator const *L_ActiveOrdersSellBegin() const = 0;
	virtual L_OrderListIterator const *L_ActiveOrdersSellEnd() const = 0;

	virtual void L_CloseAllPositions(long securityFlags = L_SecFlag::ANY, long posFlags = L_PosFlag::ANY) = 0;
	virtual void L_CancelAllOrders(long dayFlags = L_DayFlag::ANY, long sideFlags = L_SideFlag::ANY, long securityFlags = L_SecFlag::ANY, long posFlags = L_PosFlag::ANY) = 0;
	virtual void L_CancelPositionOrders(L_Position const *pos, long dayFlags = L_DayFlag::ANY, long sideFlags = L_SideFlag::ANY) = 0;

	virtual L_LocateSymbolIterator const *L_LocateSymbolsBegin() const = 0;
	virtual L_LocateSymbolIterator const *L_LocateSymbolsEnd() const = 0;

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
	position_iterator positions_begin() const
	{
		return position_iterator(L_PositionsBegin());
	}
	position_iterator positions_end() const
	{
		return position_iterator(L_PositionsEnd());
	}
	position_iterator open_positions_begin() const
	{
		return position_iterator(L_OpenPositionsBegin());
	}
	position_iterator open_positions_end() const
	{
		return position_iterator(L_OpenPositionsEnd());
	}
	position_iterator long_positions_begin() const
	{
		return position_iterator(L_LongPositionsBegin());
	}
	position_iterator long_positions_end() const
	{
		return position_iterator(L_LongPositionsEnd());
	}
	position_iterator short_positions_begin() const
	{
		return position_iterator(L_ShortPositionsBegin());
	}
	position_iterator short_positions_end() const
	{
		return position_iterator(L_ShortPositionsEnd());
	}

	order_iterator orders_begin() const
	{
		return order_iterator(L_OrdersBegin());
	}
	order_iterator orders_end() const
	{
		return order_iterator(L_OrdersEnd());
	}
	order_iterator orders_buy_begin() const
	{
		return order_iterator(L_OrdersBuyBegin());
	}
	order_iterator orders_buy_end() const
	{
		return order_iterator(L_OrdersBuyEnd());
	}
	order_iterator orders_sell_begin() const
	{
		return order_iterator(L_OrdersSellBegin());
	}
	order_iterator orders_sell_end() const
	{
		return order_iterator(L_OrdersSellEnd());
	}
	order_iterator active_orders_begin() const
	{
		return order_iterator(L_ActiveOrdersBegin());
	}
	order_iterator active_orders_end() const
	{
		return order_iterator(L_ActiveOrdersEnd());
	}
	order_iterator active_orders_buy_begin() const
	{
		return order_iterator(L_ActiveOrdersBuyBegin());
	}
	order_iterator active_orders_buy_end() const
	{
		return order_iterator(L_ActiveOrdersBuyEnd());
	}
	order_iterator active_orders_sell_begin() const
	{
		return order_iterator(L_ActiveOrdersSellBegin());
	}
	order_iterator active_orders_sell_end() const
	{
		return order_iterator(L_ActiveOrdersSellEnd());
	}

	locate_symbol_iterator locate_symbols_begin() const
	{
		return locate_symbol_iterator(L_LocateSymbolsBegin());
	}
	locate_symbol_iterator locate_symbols_end() const
	{
		return locate_symbol_iterator(L_LocateSymbolsEnd());
	}

#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
};

}

#endif // !defined(LS_L_ACCOUNT_H)


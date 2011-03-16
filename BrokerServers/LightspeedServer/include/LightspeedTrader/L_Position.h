#pragma once
#if !defined(LS_L_POSITION_H)
#define LS_L_POSITION_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

#include "L_Iterator.h"
#include "L_Order.h"
#include "L_Execution.h"

namespace LightspeedTrader
{

class L_Position;

class __declspec(novtable) L_PositionListIterator
{
public:
	typedef std::forward_iterator_tag l_iterator_category;
	typedef ptrdiff_t l_distance_type;
	typedef L_Position *l_value_type;
	typedef l_value_type *l_pointer;
	typedef l_value_type l_reference;
	typedef size_t l_size_type;
	typedef l_value_type const *l_const_pointer;
	typedef l_value_type l_const_reference;
	typedef L_PositionListIterator l_this_iterator;

	virtual l_this_iterator *L_Copy() const = 0;
	virtual void L_Destroy() = 0;
	virtual bool L_IsEqual(l_this_iterator const *) const = 0;
	virtual void L_Increment() = 0;
	virtual l_const_reference L_ConstDeref() const = 0;
	virtual void const *L_DirectData() const = 0;
};

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
typedef C_WrappedConstIterator<L_PositionListIterator> position_iterator;
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)

class __declspec(novtable) L_Position
{
public:
	virtual char const *L_Symbol() const = 0;
	virtual int L_BPType() const = 0;
	virtual long L_Shares() const = 0;
	virtual double L_TotalPrice() const = 0;
	virtual double L_TotalPriceByAve() const = 0;
	virtual double L_AveragePrice() const = 0;
	virtual double L_AveragePriceByAve() const = 0;
	virtual double L_ClosedPL() const = 0;
	virtual double L_ClosedPLByAve() const = 0;
	virtual double L_OpenPL() const = 0;
	virtual double L_OpenPLByAve() const = 0;
	virtual double L_MarkedPL() const = 0;
	virtual double L_CostBasisTraditional() const = 0;
	virtual double L_CostBasisByAve() const = 0;
	virtual double L_MoneyInvested() const = 0;
	virtual double L_Value() const = 0;
	virtual double L_LongValue() const = 0;
	virtual double L_ShortValue() const = 0;
	virtual double L_DollarValue() const = 0;
	virtual double L_LongDollarValue() const = 0;
	virtual double L_ShortDollarValue() const = 0;
	virtual double L_MarginRequirement() const = 0;
	virtual double L_MarginFactor() const = 0;
	virtual double L_PLLastPrice() const = 0;

	virtual long L_PendingOrdersCount() const = 0;
	virtual long L_PendingBuyOrdersCount() const = 0;
	virtual long L_PendingSellOrdersCount() const = 0;

	virtual long L_SharesTraded() const = 0;
	virtual long L_NumTrades() const = 0;
	virtual long L_NumExecutions() const = 0;

	virtual long L_AddedLiquidity() const = 0;
	virtual long L_AddedLiquidityExecutions() const = 0;
	virtual long L_RemovedLiquidity() const = 0;
	virtual long L_RemovedLiquidityExecutions() const = 0;

	virtual L_OrderListIterator const *L_AllOrdersBegin() const = 0;
	virtual L_OrderListIterator const *L_AllOrdersEnd() const = 0;
	virtual L_OrderListIterator const *L_ActiveOrdersBegin() const = 0;
	virtual L_OrderListIterator const *L_ActiveOrdersEnd() const = 0;
	virtual L_OrderListIterator const *L_ActiveOrdersBuyBegin() const = 0;
	virtual L_OrderListIterator const *L_ActiveOrdersBuyEnd() const = 0;
	virtual L_OrderListIterator const *L_ActiveOrdersSellBegin() const = 0;
	virtual L_OrderListIterator const *L_ActiveOrdersSellEnd() const = 0;

	virtual L_ExecutionListIterator const *L_ExecutionsBegin() const = 0;
	virtual L_ExecutionListIterator const *L_ExecutionsEnd() const = 0;
	virtual L_ExecutionListIterator const *L_ExecutionsBuyBegin() const = 0;
	virtual L_ExecutionListIterator const *L_ExecutionsBuyEnd() const = 0;
	virtual L_ExecutionListIterator const *L_ExecutionsSellBegin() const = 0;
	virtual L_ExecutionListIterator const *L_ExecutionsSellEnd() const = 0;

	virtual L_ExecutionListIterator const *L_ExecutionsToMatchBegin() const = 0;
	virtual L_ExecutionListIterator const *L_ExecutionsToMatchEnd() const = 0;

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
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

	execution_iterator executions_begin() const
	{
		return execution_iterator(L_ExecutionsBegin());
	}
	execution_iterator executions_end() const
	{
		return execution_iterator(L_ExecutionsEnd());
	}
	execution_iterator executions_buy_begin() const
	{
		return execution_iterator(L_ExecutionsBuyBegin());
	}
	execution_iterator executions_buy_end() const
	{
		return execution_iterator(L_ExecutionsBuyEnd());
	}
	execution_iterator executions_sell_begin() const
	{
		return execution_iterator(L_ExecutionsSellBegin());
	}
	execution_iterator executions_sell_end() const
	{
		return execution_iterator(L_ExecutionsSellEnd());
	}

	execution_iterator executions_to_match_begin() const
	{
		return execution_iterator(L_ExecutionsToMatchBegin());
	}
	execution_iterator executions_to_match_end() const
	{
		return execution_iterator(L_ExecutionsToMatchEnd());
	}
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
};

}

#endif // !defined(LS_L_POSITION_H)


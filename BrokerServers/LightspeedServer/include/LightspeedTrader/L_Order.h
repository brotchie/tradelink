#pragma once
#if !defined(LS_L_ORDER_H)
#define LS_L_ORDER_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

#include "L_OrderErrors.h"
#include "L_Iterator.h"
#include "L_Execution.h"

namespace LightspeedTrader
{
namespace L_OrderType
{
	const long MARKET = 0;
	const long LIMIT = 1;
	const long STOP = 2;
	const long MOC = 3;
	const long LOC = 4;
	const long CO = 5;
	const long LOCI = 6;
	const long MOO = 7;
	const long LOO = 8;
	const long LOOI = 9;
	const long ADDO = 10;
	const long NROT = 11;
}

class __declspec(novtable) L_Order
{
public:
	virtual long L_ReferenceId() const = 0;
	virtual long L_OrderId() const = 0;

	virtual const char *L_Symbol() const = 0;
	virtual char L_TradeSide() const = 0;
	virtual long L_OriginalShares() const = 0;
	virtual double L_OriginalPrice() const = 0;
	virtual const char *L_OriginalMarket() const = 0;
	virtual const char *L_OriginalContra() const = 0;
	virtual const char *L_Market() const = 0;
	virtual const char *L_Contra() const = 0;
	virtual bool L_IsMarketOrder() const = 0;
	virtual long L_TIF() const = 0;
	virtual bool L_IsHidden() const = 0;
	virtual long L_VisibleShares() const = 0;
	virtual double L_SecondaryPrice() const = 0;

	virtual double L_TotalPrice() const = 0;
	virtual double L_AveragePrice() const = 0;
	virtual long L_ExecutedShares() const = 0;
	virtual long L_ActiveShares() const = 0;

	virtual L_ExecutionListIterator const *L_ExecutionsBegin() const = 0;
	virtual L_ExecutionListIterator const *L_ExecutionsEnd() const = 0;

	virtual double L_MarketPrice() const = 0;

	virtual time_t L_CreateTime() const = 0;
	virtual time_t L_CancelTime() const = 0;
	virtual long L_CreateTimeMillis() const = 0;
	virtual long L_ReceiveTimeMillis() const = 0;
	virtual long L_CancelCreateTimeMillis() const = 0;
	virtual long L_CancelTimeMillis() const = 0;

	virtual bool L_CancelInitiated() const = 0;
	virtual long L_RejectedShares() const = 0;
	virtual long L_KilledShares() const = 0;

	virtual long L_AddedLiquidity() const = 0;
	virtual long L_AddedLiquidityExecutions() const = 0;
	virtual long L_RemovedLiquidity() const = 0;
	virtual long L_RemovedLiquidityExecutions() const = 0;

	virtual double L_ClosedPL() const = 0;
	virtual double L_ClosedPLByAve() const = 0;

	virtual char const *L_Exchange() const = 0;
	virtual double L_MarginRequirement() const = 0;
	virtual int L_BPType() const = 0;

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
	execution_iterator executions_begin() const
	{
		return execution_iterator(L_ExecutionsBegin());
	}
	execution_iterator executions_end() const
	{
		return execution_iterator(L_ExecutionsEnd());
	}
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
};

class __declspec(novtable) L_OrderListIterator
{
public:
	typedef std::forward_iterator_tag l_iterator_category;
	typedef ptrdiff_t l_distance_type;
	typedef L_Order *l_value_type;
	typedef l_value_type *l_pointer;
	typedef l_value_type l_reference;
	typedef size_t l_size_type;
	typedef l_value_type const *l_const_pointer;
	typedef l_value_type l_const_reference;
	typedef L_OrderListIterator l_this_iterator;

	virtual l_this_iterator *L_Copy() const = 0;
	virtual void L_Destroy() = 0;
	virtual bool L_IsEqual(l_this_iterator const *) const = 0;
	virtual void L_Increment() = 0;
	virtual l_const_reference L_ConstDeref() const = 0;
	virtual void const *L_DirectData() const = 0;
};

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
typedef C_WrappedConstIterator<L_OrderListIterator> order_iterator;
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)

}

#endif // !defined(LS_L_ORDER_H)


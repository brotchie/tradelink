#pragma once
#if !defined(LS_L_EXECUTION_H)
#define LS_L_EXECUTION_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

#include "L_Iterator.h"

namespace LightspeedTrader
{

class __declspec(novtable) L_Execution
{
public:
	virtual long L_OrderId() const = 0;
	virtual time_t L_ExecTime() const = 0;
	virtual long L_ExecTimeMillis() const = 0;

	virtual char const *L_Symbol() const = 0;
	virtual char L_Side() const = 0;
	virtual long L_Shares() const = 0;
	virtual char const *L_Market() const = 0;
	virtual char const *L_Contra() const = 0;
	virtual char L_Liquidity() const = 0;
	virtual bool L_IsOvernight() const = 0;
	virtual double L_TotalPrice() const = 0;
	virtual double L_AveragePrice() const = 0;
	virtual double L_MoneyInvested() const = 0;
	virtual int L_BPType() const = 0;
};

class __declspec(novtable) L_ExecutionListIterator
{
public:
	typedef std::bidirectional_iterator_tag l_iterator_category;
	typedef ptrdiff_t l_distance_type;
	typedef L_Execution l_value_type;
	typedef l_value_type *l_pointer;
	typedef l_value_type &l_reference;
	typedef size_t l_size_type;
	typedef l_value_type const *l_const_pointer;
	typedef l_value_type const &l_const_reference;
	typedef L_ExecutionListIterator l_this_iterator;

	virtual l_this_iterator *L_Copy() const = 0;
	virtual void L_Destroy() = 0;
	virtual bool L_IsEqual(l_this_iterator const *) const = 0;
	virtual void L_Increment() = 0;
	virtual void L_Decrement() = 0;
	virtual l_const_reference L_ConstDeref() const = 0;
	virtual void const *L_DirectData() const = 0;
};

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)
typedef C_WrappedConstIterator<L_ExecutionListIterator> execution_iterator;
#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)

} // namespace LightspeedTrader

#endif // !defined(LS_L_EXECUTION_H)

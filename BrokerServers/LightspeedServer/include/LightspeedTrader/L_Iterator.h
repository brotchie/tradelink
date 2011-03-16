#pragma once
#if !defined(LS_L_ITERATOR_H)
#define LS_L_ITERATOR_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

#if !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)

#include <iterator>

namespace LightspeedTrader
{

template<typename l_Container, typename l_Value>
class C_PointerConstIterator
	: public std::iterator<
		std::random_access_iterator_tag,
		typename l_Value,
		ptrdiff_t,
		typename l_Value const *,
		typename l_Value const &
		>
{
public:
	C_PointerConstIterator() : ptr(0), valueSize(0)
	{
	}
	C_PointerConstIterator(l_Container const *cont, l_Value const *ptrInit) : ptr(ptrInit), valueSize(cont->L_ValueSize())
	{
	}
	C_PointerConstIterator(C_PointerConstIterator const &rhs) : valueSize(rhs.valueSize), ptr(rhs.ptr)
	{
	}
	reference operator*() const
	{
		return *ptr;
	}
	pointer operator->() const
	{
		return &**this;
	}
	C_PointerConstIterator &operator++()
	{
		ptr = reinterpret_cast<pointer>(reinterpret_cast<char const *>(ptr) + valueSize);
		return *this;
	}
	C_PointerConstIterator operator++(int)
	{
		C_PointerConstIterator tmp = *this;
		++*this;
		return tmp;
	}
	C_PointerConstIterator &operator--()
	{
		ptr = reinterpret_cast<pointer>(reinterpret_cast<char const *>(ptr) - valueSize);
		return *this;
	}
	C_PointerConstIterator operator--(int)
	{
		C_PointerConstIterator tmp = *this;
		--*this;
		return tmp;
	}
	C_PointerConstIterator &operator+=(difference_type offset)
	{
		ptr = reinterpret_cast<pointer>(reinterpret_cast<char const *>(ptr) + offset * valueSize);
		return *this;
	}
	C_PointerConstIterator operator+(difference_type offset) const
	{
		C_PointerConstIterator tmp = *this;
		tmp += offset;
		return tmp;
	}
	C_PointerConstIterator &operator-=(difference_type offset)
	{
		operator+=(-offset);
		return *this;
	}
	C_PointerConstIterator operator-(difference_type offset) const
	{
		C_PointerConstIterator tmp = *this;
		tmp -= offset;
		return tmp;
	}
	difference_type operator-(C_PointerConstIterator const &rhs) const
	{
		return (reinterpret_cast<char const *>(ptr) - reinterpret_cast<char const *>(rhs.ptr)) / valueSize;
	}
	reference operator[](difference_type offset) const
	{
		return *(*this + offset);
	}
	bool operator==(C_PointerConstIterator const &rhs) const
	{
		return ptr == rhs.ptr;
	}
	bool operator!=(C_PointerConstIterator const &rhs) const
	{
		return !(*this == rhs);
	}
	bool operator<(C_PointerConstIterator const &rhs) const
	{
		return ptr < rhs.ptr;
	}
	bool operator>(C_PointerConstIterator const &rhs) const
	{
		return rhs < *this;
	}
	bool operator<=(C_PointerConstIterator const &rhs) const
	{
		return !(rhs < *this);
	}
	bool operator>=(C_PointerConstIterator const &rhs) const
	{
		return !(*this < rhs);
	}

private:
	size_t valueSize;
	l_Value const *ptr;
};

template<typename l_Container, typename l_Value>
inline C_PointerConstIterator<l_Container, l_Value> operator+(
	typename C_PointerConstIterator<l_Container, l_Value>::difference_type offset
	, C_PointerConstIterator<l_Container, l_Value> lhs
	)
{
	lhs += offset;
	return lhs;
}

template<typename l_Container, typename l_Value>
class C_PointerIterator
	: public C_PointerConstIterator<l_Container, l_Value>
{
public:
	typedef C_PointerConstIterator<l_Container, l_Value> _const_iter;

	typedef std::random_access_iterator_tag iterator_category;
	typedef typename l_Value value_type;
	typedef size_t size_type;
	typedef ptrdiff_t difference_type;
	typedef typename l_Value *pointer;
	typedef typename l_Value &reference;

	C_PointerIterator()
	{
	}
	C_PointerIterator(l_Container const *cont, l_Value const *ptrInit) : _const_iter(cont, ptrInit)
	{
	}
	C_PointerIterator(C_PointerConstIterator const &rhs) : _const_iter(rhs)
	{
	}
	reference operator*() const
	{
		return *const_cast<value_type *>(&**static_cast<_const_iter *>(this));
	}
	pointer operator->() const
	{
		return &**this;
	}
	C_PointerIterator &operator++()
	{
		++*static_cast<_const_iter *>(this);
		return *this;
	}
	C_PointerIterator operator++(int)
	{
		C_PointerIterator tmp = *this;
		++*this;
		return tmp;
	}
	C_PointerIterator &operator--()
	{
		--*static_cast<_const_iter *>(this);
		return *this;
	}
	C_PointerIterator operator--(int)
	{
		C_PointerIterator tmp = *this;
		--*this;
		return tmp;
	}

	C_PointerIterator &operator+=(difference_type offset)
	{
		*static_cast<_const_iter *>(this) += offset;
		return *this;
	}

	C_PointerIterator operator+(difference_type offset) const
	{
		C_PointerIterator tmp = *this;
		tmp += offset;
		return tmp;
	}

	C_PointerIterator &operator-=(difference_type offset)
	{
		*this += -offset;
		return *this;
	}

	C_PointerIterator operator-(difference_type offset) const
	{
		C_PointerIterator tmp = *this;
		tmp -= offset;
		return tmp;
	}

	difference_type operator-(const _const_iter& rhs) const
	{
		return *static_cast<_const_iter *>(this) - rhs;
	}

	reference operator[](difference_type offset) const
	{
		return *(*this + offset);
	}
};

template<typename l_Container, typename l_Value>
inline C_PointerIterator<l_Container, l_Value> operator+(
	typename C_PointerIterator<l_Container, l_Value>::difference_type offset
	, C_PointerIterator<l_Container, l_Value> lhs
	)
{
	lhs += offset;
	return lhs;
}


template<typename l_Iterator>
class C_WrappedConstIterator
	: public std::iterator<
		typename l_Iterator::l_iterator_category,
		typename l_Iterator::l_value_type,
		typename l_Iterator::l_distance_type,
		typename l_Iterator::l_pointer,
		typename l_Iterator::l_reference
		>
{
public:
	typedef typename l_Iterator::l_const_pointer const_pointer;
	typedef typename l_Iterator::l_const_reference const_reference;

	C_WrappedConstIterator() : lit(0)
	{
	}
	C_WrappedConstIterator(l_Iterator const *litInit) : lit(litInit ? litInit->L_Copy() : 0)
	{
	}
	C_WrappedConstIterator(C_WrappedConstIterator const &rhs) : lit(0)
	{
		if (rhs.lit)
		{
			lit = rhs.lit->L_Copy();
		}
	}
	~C_WrappedConstIterator()
	{
		if (lit)
		{
			lit->L_Destroy();
		}
	}
	C_WrappedConstIterator &operator=(C_WrappedConstIterator const &rhs)
	{
		if (lit != rhs.lit)
		{
			if (lit)
			{
				lit->L_Destroy();
			}
			if (rhs.lit)
			{
				lit = rhs.lit->L_Copy();
			}
			else
			{
				lit = 0;
			}
		}
		return *this;
	}
	const_reference operator*() const
	{
		return lit->L_ConstDeref();
	}
	const_pointer operator->() const
	{
		return &**this;
	}
	C_WrappedConstIterator &operator++()
	{
		lit->L_Increment();
		return *this;
	}
	C_WrappedConstIterator operator++(int)
	{
		C_WrappedConstIterator tmp = *this;
		++*this;
		return tmp;
	}
	C_WrappedConstIterator &operator--()
	{
		lit->L_Decrement();
		return *this;
	}
	C_WrappedConstIterator operator--(int)
	{
		C_WrappedConstIterator tmp = *this;
		--*this;
		return tmp;
	}
	C_WrappedConstIterator &operator+=(difference_type offset)
	{
		lit->L_Advance(offset);
		return *this;
	}
	C_WrappedConstIterator operator+(difference_type offset) const
	{
		C_WrappedConstIterator tmp = *this;
		tmp += offset;
		return tmp;
	}
	C_WrappedConstIterator &operator-=(difference_type offset)
	{
		operator+=(-offset);
		return *this;
	}
	C_WrappedConstIterator operator-(difference_type offset) const
	{
		C_WrappedConstIterator tmp = *this;
		tmp -= offset;
		return tmp;
	}
	difference_type operator-(C_WrappedConstIterator const &rhs) const
	{
		if (rhs.lit)
		{
			return rhs.lit->L_DistanceTo(lit);
		}
		else
		{
			return 0;
		}
	}
	const_reference operator[](difference_type offset) const
	{
		return *(*this + offset);
	}
	bool operator==(C_WrappedConstIterator const &rhs) const
	{
		if (rhs.lit && lit)
		{
			return lit->L_IsEqual(rhs.lit);
		}
		else
		{
			return true;
		}
	}
	bool operator!=(C_WrappedConstIterator const &rhs) const
	{
		return !(*this == rhs);
	}
	bool operator<(C_WrappedConstIterator const &rhs) const
	{
		if (rhs.lit && lit)
		{
			return lit->L_IsLess(rhs.lit);
		}
		else
		{
			return false;
		}
	}
	bool operator>(C_WrappedConstIterator const &rhs) const
	{
		return rhs < *this;
	}
	bool operator<=(C_WrappedConstIterator const &rhs) const
	{
		return !(rhs < *this);
	}
	bool operator>=(C_WrappedConstIterator const &rhs) const
	{
		return !(*this < rhs);
	}

private:
	l_Iterator *lit;
};

template<class l_Iterator>
inline C_WrappedConstIterator<l_Iterator> operator+(
	typename C_WrappedConstIterator<l_Iterator>::difference_type offset
	, C_WrappedConstIterator<l_Iterator> lhs
	)
{
	lhs += offset;
	return lhs;
}


template<typename l_Iterator>
class C_WrappedIterator
	: public C_WrappedConstIterator<l_Iterator>
{
public:
	typedef C_WrappedConstIterator<l_Iterator> _const_iter;

	typedef typename l_Iterator::l_iterator_category iterator_category;
	typedef typename l_Iterator::l_value_type value_type;
	typedef typename l_Iterator::l_size_type size_type;
	typedef typename l_Iterator::l_difference_type difference_type;
	typedef typename l_Iterator::l_pointer pointer;
	typedef typename l_Iterator::l_reference reference;

	C_WrappedIterator()
	{
	}
	C_WrappedIterator(l_Iterator const *litInit) : _const_iter(litInit)
	{
	}
	C_WrappedIterator(C_WrappedConstIterator const &rhs) : _const_iter(rhs)
	{
	}
	C_WrappedIterator &operator=(C_WrappedConstIterator const &rhs)
	{
		return _const_iter::operator=(rhs);
	}
	reference operator*() const
	{
		return lit->L_Deref();
	}
	pointer operator->() const
	{
		return &**this;
	}
	C_WrappedIterator &operator++()
	{
		++*static_cast<_const_iter *>(this);
		return *this;
	}
	C_WrappedIterator operator++(int)
	{
		C_WrappedIterator tmp = *this;
		++*this;
		return tmp;
	}
	C_WrappedIterator &operator--()
	{
		--*static_cast<_const_iter *>(this);
		return *this;
	}
	C_WrappedIterator operator--(int)
	{
		C_WrappedIterator tmp = *this;
		--*this;
		return tmp;
	}

	C_WrappedIterator &operator+=(difference_type offset)
	{
		*static_cast<_const_iter *>(this) += offset;
		return *this;
	}

	C_WrappedIterator operator+(difference_type offset) const
	{
		C_WrappedIterator tmp = *this;
		tmp += offset;
		return tmp;
	}

	C_WrappedIterator &operator-=(difference_type offset)
	{
		*this += -offset;
		return *this;
	}

	C_WrappedIterator operator-(difference_type offset) const
	{
		C_WrappedIterator tmp = *this;
		tmp -= offset;
		return tmp;
	}

	difference_type operator-(const _const_iter& rhs) const
	{
		return *static_cast<_const_iter *>(this) - rhs;
	}

	reference operator[](difference_type offset) const
	{
		return *(*this + offset);
	}
};

template<typename l_Iterator>
inline C_WrappedIterator<l_Iterator> operator+(
	typename C_WrappedIterator<l_Iterator>::difference_type offset
	, C_WrappedIterator<l_Iterator> lhs
	)
{
	lhs += offset;
	return lhs;
}


} // namespace LightspeedTrader

#endif // !defined(LS_EXCLUDE_CLIENT_COMPILER_SPECIFIC)

#endif // !defined(LS_L_ITERATOR_H)

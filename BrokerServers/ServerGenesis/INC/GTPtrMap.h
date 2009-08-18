/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTPtrMap.h
	\brief interface for the GTPtrMap class.
 */

#pragma once

//! \cond INTERNAL
template <class Key, class ARG_KEY, class Type>
class GTPtrMap : public CMap<Key, ARG_KEY, Type *, Type *>
{
public:
	GTPtrMap(INT_PTR nBlockSize = 10)
		: CMap<Key, ARG_KEY, Type *, Type *>(nBlockSize)
	{
	}

	virtual ~GTPtrMap()
	{
		RemoveAll();
	}

public:
	void RemoveAll()
	{
		Key key;
		Type *ptr;
		POSITION pos = GetStartPosition();
		while(pos)
		{
			GetNextAssoc(pos, key, ptr);

			delete ptr;
		}
	
		CMap<Key, ARG_KEY, Type *, Type *>::RemoveAll();
	}

	BOOL RemoveKey(ARG_KEY key)
	{
		Type *ptr;
		if(Lookup(key, ptr))
			delete ptr;

		return CMap<Key, ARG_KEY, Type *, Type *>::RemoveKey(key);
	}
};
//! \endcond

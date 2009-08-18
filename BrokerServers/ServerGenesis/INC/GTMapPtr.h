// GTMapPtr.h: interface for the GTMapPtr class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTMapPtr.h
    \brief interface for the GTMapPtr class.
 */

#if !defined(__GTMAPPTR_H__)
#define __GTMAPPTR_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
//! \cond INTERNAL

template<class KEY, class ARG_KEY, class VALUE>
class GTMapPtr : public CMap<KEY, ARG_KEY, VALUE *, VALUE *>
{
public:
	GTMapPtr()
	{
	}
	virtual ~GTMapPtr()
	{
	}

public:
	void RemoveAll()
	{
		KEY key;

		POSITION pos = GetStartPosition();
		while(pos)
		{
			VALUE *pValue;
			GetNextAssoc(pos, key, pValue);
			delete pValue;
		}

		CMap<KEY, ARG_KEY, VALUE *, VALUE *>::RemoveAll();
	}

	BOOL RemoveKey(ARG_KEY key)
	{
		VALUE *pValue;

		if(Lookup(key, pValue))
			delete pValue;
	
		return CMap<KEY, ARG_KEY, VALUE *, VALUE *>::RemoveKey(pValue);
	}
};
//! \endcond

#endif // !defined(__GTMAPPTR_H__)

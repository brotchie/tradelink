/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTPtrList.h
	\brief interface for the GTPtrList class.
 */

#pragma once

//! \cond INTERNAL
template <class Type>
class GTPtrList : public CList<Type *, Type *>
{
public:

	GTPtrList(INT_PTR nBlockSize = 10)
		: CList<Type *, Type *>(nBlockSize)
	{
	}

	virtual ~GTPtrList(void)
	{
		RemoveAll();
	}

public:
	void RemoveAll()
	{
		POSITION pos = this->GetHeadPosition();
		while(pos)
		{
			Type *pData = this->GetNext(pos);
			
			delete pData;
		}
	
		CList<Type *, Type *>::RemoveAll();
	}

	void RemoveAt(POSITION position)
	{
		delete GetAt(position);
	
		CList<Type *, Type *>::RemoveAt(position);
	}
};

//! \endcond

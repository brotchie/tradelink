/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

#ifndef __CPPMAPCLASS_H__
#define __CPPMAPCLASS_H__

#include "..\Inc32\CMapClass.h"

#define IMPLEMENT_MAP_CLASS(Name, mapCppClass, mapCClass, CppValue, CppKey, CValue, CKey)		\
	CKey WINAPI Name##GetKey(MAP_PAIR_POS pos)										\
	{																				\
		mapCppClass::CPair *pPos = reinterpret_cast<mapCppClass::CPair *>(pos);		\
		return pPos->key;															\
	}																				\
	CValue WINAPI Name##GetValue(MAP_PAIR_POS pos)									\
	{																				\
		mapCppClass::CPair *pPos = reinterpret_cast<mapCppClass::CPair *>(pos);		\
		return pPos->value;															\
	}																				\
	void WINAPI Name##SetAt(mapCClass *mapVar, CValue value, CKey key)				\
	{																				\
		mapCppClass *pMap = reinterpret_cast<mapCppClass *>(mapVar);				\
		CppValue cppValue;															\
		CppKey   cppKey;															\
		(CValue &)cppValue = value;													\
		(CKey &)cppKey = key;														\
		pMap->SetAt(cppKey, cppValue);												\
	}																				\
	MAP_PAIR_POS WINAPI Name##GetFirstPosition(mapCClass *mapVar)					\
	{																				\
		mapCppClass *pMap = reinterpret_cast<mapCppClass *>(mapVar);				\
		return (MAP_PAIR_POS)pMap->PGetFirstAssoc();								\
	}																				\
	MAP_PAIR_POS WINAPI Name##GetNextPosition(mapCClass *mapVar, MAP_PAIR_POS pos)	\
	{																				\
		mapCppClass *pMap = reinterpret_cast<mapCppClass *>(mapVar);				\
		mapCppClass::CPair *pPos = reinterpret_cast<mapCppClass::CPair *>(pos);		\
		return (MAP_PAIR_POS)pMap->PGetNextAssoc(pPos);								\
	}																				\

#endif//__CPPMAPCLASS_H__

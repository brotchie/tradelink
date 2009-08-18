/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

#ifndef __CMAPCLASS_H__
#define __CMAPCLASS_H__

#define MAP_PAIR_POS void *

#ifdef __cplusplus
#define EXTERNC_MAP_CLASS extern "C" 
#else
#define EXTERNC_MAP_CLASS
#endif

#define DEFINE_MAP_CLASS(Name, mapCClass, CValue, CKey)		\
	EXTERNC_MAP_CLASS CKey WINAPI Name##GetKey(MAP_PAIR_POS pos);										\
	EXTERNC_MAP_CLASS CValue WINAPI Name##GetValue(MAP_PAIR_POS pos);									\
	EXTERNC_MAP_CLASS void WINAPI Name##SetAt(mapCClass *mapVar, CValue value, CKey key);				\
	EXTERNC_MAP_CLASS MAP_PAIR_POS WINAPI Name##GetFirstPosition(mapCClass *mapVar);					\
	EXTERNC_MAP_CLASS MAP_PAIR_POS WINAPI Name##GetNextPosition(mapCClass *mapVar, MAP_PAIR_POS pos);	\


#endif//__CMAPCLASS_H__

/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

#ifndef __GTAPI_API_H__
#define __GTAPI_API_H__

#if defined(GTAPI_EXPORT)
#define GTAPI_API __declspec(dllexport)
#elif defined(GTAPI_IMPORT)
#define GTAPI_API __declspec(dllimport)
#elif defined(GTAPI_LIB)
#define GTAPI_API
#else
#define GTAPI_API
#endif

#endif//__GTAPI_API_H__

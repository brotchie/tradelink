/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file OrderEntryErrMsg.h
	\brief Defines Order Entry Error Messages
 */

#ifndef __ORDERENTRYERRMSG_H__
#define __ORDERENTRYERRMSG_H__

#include "GTConst.h"

#define ORDER_RCD_ERROR_INVALID_ACCOUNTID		(ORDER_RCD_ERROR_0 + 1)
#define ORDER_RCD_ERROR_INVALID_SYMBOL			(ORDER_RCD_ERROR_0 + 2)
#define ORDER_RCD_ERROR_INVALID_BUYSELL			(ORDER_RCD_ERROR_0 + 3)
#define ORDER_RCD_ERROR_INVALID_SHARES			(ORDER_RCD_ERROR_0 + 4)
#define ORDER_RCD_ERROR_INVALID_MINIMUM			(ORDER_RCD_ERROR_0 + 5)
#define ORDER_RCD_ERROR_INVALID_TIMEINFORCE		(ORDER_RCD_ERROR_0 + 6)
#define ORDER_RCD_ERROR_INVALID_PRICE			(ORDER_RCD_ERROR_0 + 7)
#define ORDER_RCD_ERROR_INVALID_DISPLAY			(ORDER_RCD_ERROR_0 + 8)
#define ORDER_RCD_ERROR_INVALID_CAPACITY		(ORDER_RCD_ERROR_0 + 9)
#define ORDER_RCD_ERROR_INVALID_METHOD			(ORDER_RCD_ERROR_0 + 10)
#define ORDER_RCD_ERROR_INVALID_MMID			(ORDER_RCD_ERROR_0 + 11)
#define ORDER_RCD_ERROR_INVALID_INTERNALIZE		(ORDER_RCD_ERROR_0 + 12)
#define ORDER_RCD_ERROR_INVALID_DISCRETIONARY	(ORDER_RCD_ERROR_0 + 13)
#define ORDER_RCD_ERROR_INVALID_PEGGED			(ORDER_RCD_ERROR_0 + 14)
#define ORDER_RCD_ERROR_INVALID_SOES_TIF		(ORDER_RCD_ERROR_0 + 15)
#define ORDER_RCD_ERROR_INVALID_IMBALANCE		(ORDER_RCD_ERROR_0 + 16)
#define ORDER_RCD_ERROR_NO_MARKETONCLOSE		(ORDER_RCD_ERROR_0 + 17)
#define ORDER_RCD_ERROR_NO_MARKETONCLOSE_CXL	(ORDER_RCD_ERROR_0 + 18)
#define ORDER_RCD_ERROR_NO_VWAP_CXL				(ORDER_RCD_ERROR_0 + 19)

#define ORDER_RCD_ERROR_INVALID_STRIKE_PRICE	(ORDER_RCD_ERROR_0 + 20)
#define ORDER_RCD_ERROR_INVALID_OPTION_YEAR		(ORDER_RCD_ERROR_0 + 21)
#define ORDER_RCD_ERROR_INVALID_OPTION_OPENORCLOSE (ORDER_RCD_ERROR_0 + 22)

#define ORDER_RCD_ERROR_INVALID_ITS_ORDER		(ORDER_RCD_ERROR_0 + 23)
#define ORDER_RCD_ERROR_INVALID_ITS_CANCEL		(ORDER_RCD_ERROR_0 + 24)

#define ORDER_RCD_ERROR_OPTION_ENTITLEMENT		(ORDER_RCD_ERROR_0 + 25)

#define ORDER_RCD_ERROR_SPREAD_ENTITLEMENT		(ORDER_RCD_ERROR_0 + 30)
#define ORDER_RCD_ERROR_NO_MOC_LOC				(ORDER_RCD_ERROR_0 + 31)

#ifdef __cplusplus
extern "C"
{
#endif//__cplusplus

LPCSTR GetOrderEntryErrorMessage(int nReason);

#ifdef __cplusplus
}
#endif//__cplusplus

#endif//__ORDERENTRYERRMSG_H__

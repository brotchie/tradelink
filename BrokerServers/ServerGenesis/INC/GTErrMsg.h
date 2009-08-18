/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTErrMsg.h
	\brief interface for the GTErrMsg class.
 */

#ifndef __GTERRMSG_H__
#define __GTERRMSG_H__

#include "..\Inc32\GTErrMsg32.h"

/*! \ingroup cpp
*/
/*! \struct GTErrMsg
	\brief The order information. The same to GTErrMsg32 or tagGTErrMsg32.

	\copydoc tagGTErrMsg32
*/
typedef GTErrMsg32 GTErrMsg;

/*
struct GTErrMsg : public GTErrMsg32
{
};
*/

#endif//__GTERRMSG_H__

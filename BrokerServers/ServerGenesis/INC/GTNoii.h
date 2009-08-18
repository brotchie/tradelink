/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTNoii.h: interface for the GTNoii class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTNoii.h
	\brief interface for the GTNoii class.
 */

#if !defined(__GTNOII_H__)
#define __GTNOII_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTConst.h"
#include "..\Inc32\MMID.h"
#include "..\Inc32\GTime32.h"
#include "..\Inc32\GTNoii32.h"

#define QUOTE_NOII			41

/*! \ingroup cpp
*/
/*! \struct GTNoii
	\brief The NOII information. Has the same structure as GTNoii32 or tagGTNoii32

	\copydoc tagGTNoii32
*/
typedef tagGTNoii32 GTNoii;

#endif // !defined(__GTNOII_H__)

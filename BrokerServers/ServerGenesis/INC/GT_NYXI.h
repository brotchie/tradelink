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

#if !defined(__GT_NYXI_H__)
#define __GT_NYXI_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTConst.h"
#include "..\Inc32\MMID.h"
#include "..\Inc32\GTime32.h"
#include "..\Inc32\GT_NYXI_32.h"

// used in case statements, to identify the type of data, etc.
#define QUOTE_NYXI			42

/*! \ingroup cpp
*/
/*! \struct GT_NYXI
	\brief The NYSE Imbalance information. Has the same structure as GT_NYXI_32 or tag_GT_NYXI_32

	\copydoc tagGTNoii32
*/
typedef tag_GT_NYXI_32 GT_NYXI;

#endif // !defined(__GTNOII_H__)

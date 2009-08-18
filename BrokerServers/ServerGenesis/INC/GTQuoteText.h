/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTQuoteText.h: interface for the GTQuoteText class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTQuoteText.h
	\brief interface for the GTQuoteText class.
 */

#if !defined(__GTQUOTETEXT_H__)
#define __GTQUOTETEXT_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTQuoteText32.h"

#define QUOTE_TEXT				25
#define LEVEL2_TEXT				25

/*! \ingroup cpp
*/
/*! \struct GTQuoteText
	\brief GTQuoteText is the alias of GTQuoteText32 and tagGTQuoteText32.

	\copydoc tagGTQuoteText32
*/
#define GTQuoteText GTQuoteText32

#endif // !defined(__GTQUOTETEXT_H__)

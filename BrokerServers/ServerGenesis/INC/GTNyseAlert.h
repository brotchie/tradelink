/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTNyseAlert.h: interface for the GTNyseAlert class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTNyseAlert.h
	\brief interface for the GTNyseAlert class.
 */

#if !defined(__GTNYSEALERT_H__)
#define __GTNYSEALERT_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\Inc32\GTConst.h"
#include "..\Inc32\MMID.h"
#include "..\Inc32\GTime32.h"
#include "..\Inc32\GTNyseAlert32.h"

#define QUOTE_NYSEALERT			51

/*! \ingroup cpp
*/
/*! \struct GTNyseAlert
	\brief The NYSEALERT information. Has the same structure as GTNyseAlert32 or tagGTNyseAlert32

	\copydoc tagGTNyseAlert32
*/
typedef tagGTNyseAlert32 GTNyseAlert;

#endif // !defined(__GTNYSEALERT_H__)

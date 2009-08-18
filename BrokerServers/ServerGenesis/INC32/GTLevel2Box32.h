/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTLevel2Box32.h
	\brief interface for the GTLevel2Box32 class.
 */

#ifndef __GTLEVEL2BOX32_H__
#define __GTLEVEL2BOX32_H__

#include "GTLevel2List32.h"

//! \cond INTERNAL
typedef struct { unsigned _unused; }GTLEVEL2BOX32, *LPGTLEVEL2BOX32;
//! \endcond

/*! \ingroup c
*/
/*! \struct GTLevel2Box32
	\brief The same as tagGTLevel2Box32.

	\copydoc tagGTLevel2Box32
*/
/*! \typedef typedef tagGTLevel2Box32 GTLevel2Box32
*/
/*! \struct tagGTLevel2Box32
	\brief The level2 box structure.

	GTLevel2Box holds all the bid and ask records into two level2list. Look into GTLevel2Box instead for the details.
	\sa GTLevel2Box
*/
typedef struct tagGTLevel2Box32
{
	BOOL		m_bTransferring;
	BOOL		m_bNeedCorrect;
	BOOL		m_bHiddenMM;

	LPGTLEVEL2LIST32 m_bid32;
	LPGTLEVEL2LIST32 m_ask32;
}GTLevel2Box32;

#endif//__GTLEVEL2BOX32_H__

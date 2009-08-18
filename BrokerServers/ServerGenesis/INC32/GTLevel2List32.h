/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTLevel2List32.h
	\brief interface for the GTLevel2List32 class.
 */

#ifndef __GTLEVEL2LIST32_H__
#define __GTLEVEL2LIST32_H__

//! \cond INTERNAL
typedef struct { unsigned _unused; }GTLEVEL2LIST32, *LPGTLEVEL2LIST32;
//! \endcond
/*! \ingroup c
*/
/*! \struct GTLevel2List32
	\brief The same as tagGTLevel2List32.

	\copydoc tagGTLevel2List32
*/
/*! \typedef typedef tagGTLevel2List32 GTLevel2List32
*/
/*! \struct tagGTLevel2List32
	\brief The bid/ask level2 list structure.

	Two list for bid records and ask records are there in the Level2Box. Look at GTLevel2List instead of this for the details.
	\sa GTLevel2List
*/
typedef struct tagGTLevel2List32
{
	int			m_nMaxLevels;
	BOOL		m_bTransferring;
	double		m_dblBestPrice;
}GTLevel2List32;

#endif//__GTLEVEL2LIST32_H__

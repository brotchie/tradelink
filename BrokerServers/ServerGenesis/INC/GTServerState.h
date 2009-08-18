/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTServerState.h
	\brief interface for the GTServerState class.
 */

#ifndef __GTSERVERSTATE_H__
#define __GTSERVERSTATE_H__

#include "..\Inc32\GTServerState32.h"

/*! \ingroup cpp
*/
/*! \struct GTServerState
	\brief GTServerState is the alias of GTServerState32 and tagGTServerState32. 

	\copydoc tagGTServerState32
*/
typedef GTServerState32 GTServerState;

/*
struct GTServerState : public GTServerState32
{
};
*/

typedef CMap<int, int, GTServerState, const GTServerState &>	CMapGTServerState;

int Dump(FILE *fp, const CMapGTServerState &pendings, int nLevel);
int Dump(FILE *fp, const GTServerState &gtstate, int nLevel);

#endif//__GTSERVERSTATE_H__

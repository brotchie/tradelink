/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTServerState32.h
	\brief interface for the GTServerState32 class.
 */

#ifndef __GTSERVERSTATE32_H__
#define __GTSERVERSTATE32_H__
//! \cond INTERNAL
typedef struct { unsigned _unused; } GTSERVERSTATES, *LPGTSERVERSTATES;
//! \endcond

/*! \ingroup c
*/
/*! \struct GTServerState32
	\brief The same as tagGTServerState32.

	\copydoc tagGTServerState32
*/
/*! \typedef typedef tagGTServerState32 GTServerState32
*/
/*! \struct tagGTServerState32
	\brief The server status information structure

	Used in OnExecMsgState() callback. When a status a server is changed, this callback is called, and this structure is sent in
	for reference. The \b nSvrID tells which server status is changed, and you can read \b szServer for the name of this server.
	\b is telling who finds this change. \b nConnect tells the new status.
	\sa GTSession::OnExecMsgState GTAPI_MSG_SESSION_OnExecMsgState

*/
typedef struct tagGTServerState32
{
	int nSvrID;			//!< Server ID which the status is changed.
	int nReportSvrID;	//!< Report Server ID.
	int nConnect;		//!< Connect status. 
	char szServer[64];	//!< The server name string which the status is changed.
}GTServerState32;

#endif//__GTSERVERSTATE32_H__

/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTQuoteText32.h
	\brief interface for the GTQuoteText32 class.
 */

#ifndef __GTQUOTETEXT32_H__
#define __GTQUOTETEXT32_H__

#pragma pack(8)
/*! \ingroup c
*/
/*! \struct GTQuoteText32
	\brief Alias of tagGTQuoteText32.

	\copydoc tagGTQuoteText32
*/
/*! \typedef typedef tagGTQuoteText32 GTQuoteText32
*/
/*! \struct tagGTQuoteText32
	\brief The quote text information.

	Quote server and level2 server might send some text to the API. OnGotQuoteText/OnGotLevel2Text callback then will be triggered.
	\sa GTSession::OnGotLevel2Text GTSession::OnGotQuoteText GTAPI_MSG_SESSION_OnGotLevel2Text GTAPI_MSG_SESSION_OnGotQuoteText 
*/
typedef struct tagGTQuoteText32
{
	//! \brief Flag.
	long dwFlag;			//may be used for some purpose
	//! \brief Reserved
	long dwReserved;		//reserved

	//! \brief Content
	char szText[127];
}GTQuoteText32;
#pragma pack()

#endif//__GTQUOTETEXT32_H__

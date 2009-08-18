/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTStatus32.h
	\brief interface for the GTStatus32 class.
 */

#ifndef __GTStatus32_H__
#define __GTStatus32_H__

#include "..\Inc32\GTConst.h"

/*! \ingroup c
*/
/*! \struct GTStatus32
	\brief The order status information. Alias of tagGTStatus32.

	\copydoc tagGTStatus32
*/
/*! \typedef typedef tagGTStatus32 GTStatus32
*/
/*! \struct tagGTStatus32
	\brief The order status information

	\sa GTSession::OnExecMsgStatus GTAPI_MSG_SESSION_OnExecMsgStatus
*/
typedef struct tagGTStatus32
{
//@{
//! \brief Status Information
	char		szStatus[100];
}GTStatus32;


// IOI Message format:
// (PendStock, PendSide, PendShare):IOI:IOITransType,IOIQltyInd,IOIid,TicketNo,Stock,Side,IOIShares,BprBid,BprRefPrice,BprOffer
// Especially, values of IOITransType could be: N or C. N = New, C = Cancel. 
//  (**Pipeline will send a New IOI to indicate activity within the BPR for the specified symbol. 
//  A Cancel IOI will indicate the symbol is no longer defined as active.)
// When IOITransType = C (Cancel), we will put the value of IOIRefID 
//  (** Pipeline will send the IOIRefID corresponding to the New IOI previously sent 
//  when a symbol is deactivated. ) into IOIid. So by using IOIid, you could know which previous 
//  IOI message is no longer active.)

typedef struct tagGTIOIStatus32
{
	long	dwTicketNo;
	char	szStock[LEN_STOCK + 1];
	char	chSide;
	long	dwShares;

	char	chTransType;	// N, C
	char	chQualityInd;	// H, M, L
	int		nIOIid;

	double	dblBprBid;
	double	dblRefPrice;
	double	dblBprOffer;
}GTIOIStatus32;

#endif//__GTStatus32_H__

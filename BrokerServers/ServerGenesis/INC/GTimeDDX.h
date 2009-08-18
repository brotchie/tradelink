/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

#ifndef __GTIMEDDX_H__
#define __GTIMEDDX_H__

#include "..\Inc\GTime0.h"

void DDX_GTime(CDataExchange* pDX, UINT nID, GTime0 &gtTime);
void DDX_GTime(CDataExchange* pDX, UINT nID, DWORD &gtTime);

#endif//__GTIMEDDX_H__

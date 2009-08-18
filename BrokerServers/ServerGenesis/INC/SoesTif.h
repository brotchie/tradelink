/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

#ifndef __SOESTIF_H__
#define __SOESTIF_H__

#include "GTAPI_API.h"

#define SOES_TIF_OC		'Y'
#define SOES_TIF_NO		'N'
#define SOES_TIF_OO		'O'
#define SOES_TIF_IO		'I'
#define SOES_TIF_X		'X'
#define SOES_TIF_IOX	'x'

GTAPI_API const char *GetSoesTifName(char chSoesTif);

#endif//__SOESTIF_H__

#pragma once
#if !defined(LS_L_TIMEUTIL_H)
#define LS_L_TIMEUTIL_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.


namespace LightspeedTrader
{

extern "C" void L_GetUSEasternTime(UINT &h, UINT &m, UINT &s);
extern "C" void L_GetUSEasternTm(struct tm &t);
extern "C" __time64_t L_GetUSEasternMidnight();
extern "C" long L_GetMillisSinceMidnight();
extern "C" void L_MillisToTm(long millis, struct tm &t);

}

#endif // !defined(LS_L_TIMEUTIL_H)


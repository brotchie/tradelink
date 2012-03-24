#pragma once
#if !defined(LS_L_VERSION_H)
#define LS_L_VERSION_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

#define LS_API_VERSION_1 0
#define LS_API_VERSION_2 8
#define LS_API_VERSION_3 0
#define LS_API_VERSION_4 0

#define L_MAKEVERSION(C1, C2, C3, C4) ( unsigned long long(C4) | (unsigned long long(C3) << 16) | (unsigned long long(C2) << 32) | (unsigned long long(C1) << 48) )

#endif // !defined(LS_L_VERSION_H)


#pragma once
#if !defined(LS_LIGHTSPEEDTRADER_H)
#define LS_LIGHTSPEEDTRADER_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

#define LSEXPORT extern "C" __declspec(dllexport)

// Functions of the following declaration are to be added to one of CPP files
// in the API client DLL project.
//
// LSEXPORT void LSInitInstance();
// LSEXPORT void LSExitInstance();
// LSEXPORT BOOL LSPreTranslateMessage(MSG *pMsg);

#include "L_Account.h"
#include "L_Version.h"
#include "L_Side.h"
#include "L_Symbols.h"
#include "L_Constants.h"
#include "L_DoubleUtil.h"
#include "L_TimeUtil.h"
#include "L_Messages.h"
#include "L_FullQuote.h"
#include "L_Summary.h"
#include "L_Account.h"
#include "L_Application.h"

#endif // !defined(LS_LIGHTSPEEDTRADER_H)


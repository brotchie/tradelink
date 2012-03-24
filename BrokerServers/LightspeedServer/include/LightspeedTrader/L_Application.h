#pragma once
#if !defined(LS_L_APPLICATION_H)
#define LS_L_APPLICATION_H

// Copyright © 2001-2011 Lightspeed Financial, Inc. All rights reserved.

namespace LightspeedTrader
{

class L_Account;
class L_Summary;
class L_FullQuote;
class L_Observer;

extern "C" void L_ExitLightspeedExtension(unsigned int exitCode);
extern "C" HMODULE L_GetExtensionModule();

extern "C" char const *L_GetLightspeedVersion();
extern "C" HWND L_GetMainWnd();
extern "C" L_Account *L_GetAccount();
extern "C" void L_LogAppMessage(char const *message);
extern "C" void L_LogExtensionMessage(char const *message);
extern "C" void L_AddMessageToAppWnd(char const *message);
extern "C" void L_AddMessageToExtensionWnd(char const *message, COLORREF color = RGB(255, 255, 255));

extern "C" L_Summary *L_CreateSummary(char const *symbol);
extern "C" L_Summary *L_TryCreateSummary(char const *symbol);
extern "C" void L_DestroySummary(L_Summary *sum);

extern "C" L_FullQuote *L_CreateFullQuote(char const *symbol);
extern "C" L_FullQuote *L_TryCreateFullQuote(char const *symbol);
extern "C" void L_DestroyFullQuote(L_FullQuote *fullQuote);

extern "C" char const *L_GetFocusedSymbol();
extern "C" void L_SetFocusedSymbol(char const *symbol);

extern "C" void L_AttachToAppNotifier(L_Observer *dest);
extern "C" void L_DetachFromAppNotifier(L_Observer *dest);

extern "C" void L_PrePumpMessage();
extern "C" void L_PreProcessMessageFilter(int code, LPMSG lpMsg);

extern "C" void L_SubscribeToOrderImbalances(L_Observer *dest);
extern "C" void L_UnsubscribeFromOrderImbalances(L_Observer *dest);

extern "C" void L_SubscribeToIndications(L_Observer *dest);
extern "C" void L_UnsubscribeFromIndications(L_Observer *dest);

extern "C" void L_SubscribeToLevel1(char const *symbol, L_Observer* dest);
extern "C" void L_UnsubscribeFromLevel1(char const *symbol, L_Observer* dest);

extern "C" void L_SubscribeToTrades(char const *symbol, L_Observer *dest);
extern "C" void L_UnsubscribeFromTrades(char const *symbol, L_Observer *dest);

extern "C" void L_SubscribeToLevel2(char const *symbol, L_Observer* dest);
extern "C" void L_UnsubscribeFromLevel2(char const *symbol, L_Observer* dest);

extern "C" void L_SubscribeToECNList(L_Observer *dest);
extern "C" void L_UnsubscribeFromECNList(L_Observer *dest);

extern "C" void L_SubscribeToECN(char const *ecn, char const *symbol, L_Observer *dest);
extern "C" void L_UnsubscribeFromECN(char const *ecn, char const *symbol, L_Observer *dest);

extern "C" void L_SubscribeToMarketStatus(L_Observer *dest);
extern "C" void L_UnsubscribeFromMarketStatus(L_Observer *dest);

extern "C" long L_SummaryCount(char const *symbol);
extern "C" long L_L1SubscriptionCount(char const *symbol);
extern "C" long L_FullQuoteCount(char const *symbol);
extern "C" long L_TradesSubscriptionCount(char const *symbol);
extern "C" long L_L2SubscriptionCount(char const *symbol);
extern "C" long L_ECNSubscriptionCount(char const *ecn, char const *symbol);

extern "C" bool L_IsExecutorConnected();
extern "C" bool L_IsQuoteConnected();
extern "C" bool L_IsMarketConnected();

extern "C" char L_MarketStatus();

} // namespace LightspeedTrader

#endif // !defined(LS_L_APPLICATION_H)


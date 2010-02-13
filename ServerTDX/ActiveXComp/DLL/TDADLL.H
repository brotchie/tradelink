#ifndef _TDADLL_FILE
typedef int                 LongBool;
  #define _TDADLL_FILE
  #if _MSC_VER > 1000
    #pragma once
  #endif


#define TDADLLSPEC __declspec(dllimport) __stdcall

#ifdef __cplusplus
extern "C" {
#endif

void TDADLLSPEC TDAAddNasdaqSymbols(const char *SymbolList);

void TDADLLSPEC TDAAddNYSESymbols(const char *SymbolList);

int TDADLLSPEC TDAGetAccountIDs(int SessionHandle, char *AccountList, int MaxLength);

int TDADLLSPEC TDAGetAccountInfo(int SessionHandle, const char *AccID, void *ResultBuffer);

int TDADLLSPEC TDAGetDLLVer();

double TDADLLSPEC TDAGetLastDataTime(int SessionHandle);

int TDADLLSPEC TDAGetStreamingSites(int SessionHandle, const char *SiteList, int MaxLength);

int TDADLLSPEC TDAGetStreamStatus(int SessionHandle);

void TDADLLSPEC TDAGetStreamStatusStr(int StatusCode, char *StatusDesc, int MaxLength);

int TDADLLSPEC TDAGetSubscribedSymbols(int SessionHandle, int SubType, const char *SiteList, int MaxLength);

int TDADLLSPEC TDAGetSyncResult(int SessionHandle, int ResultID, void *Data);

int TDADLLSPEC TDAInitSession(int LogLevel);

int TDADLLSPEC TDAIsLoggedIn(int SessionHandle);

int TDADLLSPEC TDAKeepAlive(int SessionHandle);

int TDADLLSPEC TDAKeepAliveX(int SessionHandle);

int TDADLLSPEC TDALogout(int SessionHandle);

int TDADLLSPEC TDALogoutX(int SessionHandle);

int TDADLLSPEC TDAOrderCommand(int SessionHandle, int SubID, int EType, int SType, const char *Command);

int TDADLLSPEC TDAOrderCommandX(int SessionHandle, int EType, int SType, const char *Command, char *RetStr, int MaxLength);

int TDADLLSPEC TDARegisterSubscriber(int SessionHandle, int SubType, int SubHandle);

int TDADLLSPEC TDARequestBackfillX(int SessionHandle, const char *Symbol, int Days, int Freq, int *ReturnID);

int TDADLLSPEC TDARequestBackfill(int SessionHandle, int SubID, const char *Symbol, int Days, int Freq);

int TDADLLSPEC TDARequestBackfillForDatesX(int SessionHandle, const char *Symbol, char *FromDate, char *ToDate, int Freq, int *ReturnID);

int TDADLLSPEC TDARequestBackfillForDates(int SessionHandle, int SubID, const char *Symbol, char *FromDate, char *ToDate, int Freq);

int TDADLLSPEC TDARequestHistBackfillX(int SessionHandle, const char *Symbol, char *FromDate, char *ToDate, char *FreqType, int *ReturnID);

int TDADLLSPEC TDARequestHistBackfill(int SessionHandle, int SubID, const char *Symbol, char *FromDate, char *ToDate, char *FreqType);

int TDADLLSPEC TDARequestLastOrderStatus(int SessionHandle, const char *AccID);

int TDADLLSPEC TDARequestLastOrderStatusX(int SessionHandle, const char *AccID, char *LastOrderStatus);

int TDADLLSPEC TDARequestNewsHistory(int SessionHandle, int SubID, const char *Symbol);

int TDADLLSPEC TDARequestBalancesAndPositions(int SessionHandle, const char *Acc);

int TDADLLSPEC TDARequestBalancesAndPositionsX(int SessionHandle, const char *Acc, int *ReturnID, void *BPtr);

int TDADLLSPEC TDARequestBalancesAndPositionsWithQuotes(int SessionHandle, const char *Acc);

int TDADLLSPEC TDARequestBalancesAndPositionsWithQuotesX(int SessionHandle, const char *Acc, int *ReturnID, void *BPtr);

int TDADLLSPEC TDARequestPandL(int SessionHandle, const char *Acc);


int TDADLLSPEC TDARequestSnapshotQuotes(int SessionHandle, int SubID, const char *Symbols);

int TDADLLSPEC TDARequestSnapshotQuotesX(int SessionHandle, const char *Symbols, int *ReturnID);

int TDADLLSPEC TDARequestTransactions(int SessionHandle, const char *Acc, const char *Extra);

int TDADLLSPEC TDARequestTransactionsX(int SessionHandle, const char *Acc, const char *Extra, int *TransLen, int *TransID, int *FillLen, int *FillID);

int TDADLLSPEC TDASetCallback(int SessionHandle, int SubID, int EventID, void *CallBackFunc);

int TDADLLSPEC TDASetLogFile(const char *LogFileName);

int TDADLLSPEC TDASetLoginInfo(int SessionHandle, const char *Name, const char *Password);

int TDADLLSPEC TDASetLoginSite(int SessionHandle, const char *Address);

int TDADLLSPEC TDASetProxy(int SessionHandle, const char *HTTPURL, int HTTPPort, const char *HTTPSURL, int HTTPSPort, const char *LoginName, const char *LoginPass, LongBool UseSocks );

int TDADLLSPEC TDASetSourceApp(int SessionHandle, const char *SourceAppID);

int TDADLLSPEC TDASetStreamingSite(int SessionHandle, const char *Address);

int TDADLLSPEC TDAStart(int SessionHandle);

int TDADLLSPEC TDAStop(int SessionHandle);

int TDADLLSPEC TDASubscribe(int SessionHandle, int SubID, const char *SymList,int SubType);

int TDADLLSPEC TDATerminateSession(int SessionHandle);

int TDADLLSPEC TDAUnregisterSubscriber(int SessionHandle, int SubID);

int TDADLLSPEC TDAUnsubscribe(int SessionHandle, int SubID, const char *SymList,int SubType);

int TDADLLSPEC TDAUnsubscribeAll(int SessionHandle, int SubID);

int TDADLLSPEC TDASetLogLevel(int SessionHandle, int LogLevel);

int TDADLLSPEC TDARequestOptionChain(int SessionHandle, int SubID, const char* Symbol, const char* Extra);

int TDADLLSPEC TDARequestOptionChainX(int SessionHandle, const char* Symbol, const char* Extra, int *ReturnID);

int TDADLLSPEC TDASetHTTPTimeout(int SessionHandle, const int Timeout);

int TDADLLSPEC TDASetHTTPSimQueueSize(int SessionHandle, const int Size);

int TDADLLSPEC TDALiquidate(int SessionHandle, int SubID, const char *Acc, const char *Flags);

int TDADLLSPEC TDAGetSessionID(int SessionHandle, const char *SessionID, int MaxLength);

int TDADLLSPEC TDARequestMessageKey(int SessionHandle, int SubID, const char *AccNum);

int TDADLLSPEC TDARequestMessageKeyX(int SessionHandle, int SubID, const char *AccNum, char *RetStr, int MaxLength);

int TDADLLSPEC TDARequestAccountHistoryX(int SessionHandle, const char* AccNum, const char* ReqType, const char* StartDate, const char* EndDate, const char *Extra, int *ReturnID);

int TDADLLSPEC TDARequestAccountHistory(int SessionHandle, int SubID, const char* AccNum, const char* ReqType, const char* StartDate, const char* EndDate, const char *Extra);

int TDADLLSPEC TDASetL1SubFields(int SessionHandle, const char *SubFields);

  #undef TDADLLSPEC
                    
  #ifdef __cplusplus
  }                 
  #endif
#endif
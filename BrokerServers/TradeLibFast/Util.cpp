#include "stdafx.h"
#include "Util.h"
#include <cmath>


double unpack(long i) {
	if (i==0) return 0;
	// unpacks price in form whole/fractional from a single
	// long integer, converts to double
	int w = (int)(i >> 16);
	int f = (int)(i-(w<<16));
	double dec = (double)w;
	double frac = (double)f;
	frac /= 1000;
	dec += frac;
	return dec;
}



void TLTimeNow(std::vector<int> & nowtime)
{
	time_t now;
	time(&now);
	CTime ct(now);
	int date = (ct.GetYear()*10000) + (ct.GetMonth()*100) + ct.GetDay();
	int time = (ct.GetHour()*10000)+(ct.GetMinute()*100) + ct.GetSecond();
	nowtime.push_back(date);
	nowtime.push_back(time);
}

CString UniqueWindowName(CString rootname)
{
	HWND dest = FindWindow(NULL,(LPCSTR)(LPCTSTR)rootname);
	int i = -1;
	CString final(rootname);
	while (dest!=NULL)
	{
		i++;
		final = CString("");
		final.Format("%s.%i",rootname,i);
		dest = FindWindowA(NULL,(LPCSTR)(LPCTSTR)final);
	}
	return final;
}






CString gjoin(std::vector<CString>& vec, CString del)
{
	CString s(_T(""));
	for (size_t i = 0; i<vec.size(); i++)
			s += vec[i] + del;
	s.TrimRight(del);
	return s;
}

char* cleansvnrev(const char * dirtyrev)
{
	CString clean(dirtyrev);
	clean.Replace("$Rev: ","");
	clean.TrimRight(" $");
	return clean.GetBuffer();
}

CString SerializeIntVec(std::vector<int> input)
{
	std::vector<CString> tmp;
	for (size_t i = 0; i<input.size(); i++)
	{
		CString t; // setup tmp string
		t.Format("%i",input[i]); // convert integer into tmp string
		tmp.push_back(t); // push converted string onto vector
	}
	// join vector and return serialized structure
	return gjoin(tmp,",");
}

// adapted from Eddie Velasquez article
// http://www.codeproject.com/KB/string/tokenizer.aspx?display=PrintAll&fid=210&df=90&mpp=25&noise=3&sort=Position&view=Quick&fr=26

CTokenizer::CTokenizer(const CString& cs, const CString& csDelim):
	m_cs(cs),
	m_nCurPos(0)
{
	SetDelimiters(csDelim);
}

void CTokenizer::SetDelimiters(const CString& csDelim)
{
	for(int i = 0; i < csDelim.GetLength(); ++i)
		m_delim.set(static_cast<BYTE>(csDelim[i]));
}

bool CTokenizer::Next(CString& cs)
{
	cs.Empty();

	int nStartPos = m_nCurPos;
	while(m_nCurPos < m_cs.GetLength() && !m_delim[static_cast<BYTE>(m_cs[m_nCurPos])])
		++m_nCurPos;

	if(m_nCurPos >= m_cs.GetLength())
	{
		if (nStartPos<m_cs.GetLength())
			cs = m_cs.Mid(nStartPos,m_cs.GetLength()-nStartPos);
		return false;
	}
/*
	int nStartPos = m_nCurPos;
	while(m_nCurPos < m_cs.GetLength() && !m_delim[static_cast<BYTE>(m_cs[m_nCurPos])])
		++m_nCurPos;
*/	
	cs = m_cs.Mid(nStartPos, m_nCurPos - nStartPos);
	m_nCurPos++;

	return true;
}

CString	CTokenizer::Tail() const
{
	int nCurPos = m_nCurPos;

	while(nCurPos < m_cs.GetLength() && m_delim[static_cast<BYTE>(m_cs[nCurPos])])
		++nCurPos;

	CString csResult;

	if(nCurPos < m_cs.GetLength())
		csResult = m_cs.Mid(nCurPos);

	return csResult;
}

void gsplit(CString msg, CString del, std::vector<CString>& rec)
{
	CTokenizer tok(msg,del);
	CString m;
	while (tok.Next(m))
		rec.push_back(m);
	if (m.GetLength()>0)
		rec.push_back(m);

}


//////////////////////////////////////////////////////////////////////////////////////////
// On OnHandled Fault/Exception, write minidump to Current Working Directory 
//
/*********************
*
* Written with information gathered from: 
*   http://drdobbs.com/tools/185300443?pgno=1
*     and
*   http://www.codeproject.com/KB/debug/postmortemdebug_standalone1.aspx
* + Chimera's experience
*********************/
//#include <Windows.h>
#include "dbghelp.h"
typedef BOOL (WINAPI *MINIDUMPWRITEDUMP)(HANDLE hProcess, DWORD dwPid, HANDLE hFile, MINIDUMP_TYPE DumpType,
	CONST PMINIDUMP_EXCEPTION_INFORMATION ExceptionParam,
	CONST PMINIDUMP_USER_STREAM_INFORMATION UserStreamParam,
	CONST PMINIDUMP_CALLBACK_INFORMATION CallbackParam
	);
/////////////////////////////////////////////////////////////////////////////
//Global Variables :-(
LPTOP_LEVEL_EXCEPTION_FILTER sgOldExceptionFilter = NULL;
bool sgSuccessfullySetOurFilter = false;

LONG WinFaultHandler(struct _EXCEPTION_POINTERS *  ExInfo)
{ 
	////////////////////////////////////////////////
	// a. Write the minidump
	HMODULE hDll = NULL;
	char szDbgHelpPath[_MAX_PATH];
	// 1. find DbgHelp.dll [try next to my exe first]
	if (GetModuleFileName( NULL, szDbgHelpPath, _MAX_PATH ))
	{
		char *pSlash = strchr( szDbgHelpPath, '\\' );
		if (pSlash)
		{
			strcpy_s( pSlash+1, _MAX_PATH-(strlen(szDbgHelpPath)), "DBGHELP.DLL" );
			hDll = ::LoadLibrary( szDbgHelpPath );
		}
	}
	if (hDll==NULL)
	{
		// load any version we can
		hDll = ::LoadLibrary( "DBGHELP.DLL" );
	}
	// 2. Write the minidump if we can
	LPCTSTR szResult = NULL;
	char szScratch [_MAX_PATH];
	if (hDll)
	{
		MINIDUMPWRITEDUMP pDump = (MINIDUMPWRITEDUMP)::GetProcAddress( hDll, "MiniDumpWriteDump" );
		if (pDump)
		{
			char szDumpPath[_MAX_PATH];
			GetModuleFileName(NULL, szDumpPath, _MAX_PATH);
			// work out a good place for the dump file
			DWORD returnCode = GetModuleFileName(NULL, szScratch,_MAX_PATH); 
			if (returnCode != 0 && returnCode != _MAX_PATH)
			{
				char* cursor = strrchr(szScratch, '\\');
				if(cursor) {
					*cursor = '\0';
				}
			}
			else 
			{
				strcpy_s(szScratch, _MAX_PATH, ".");
			}
			strcpy_s( szDumpPath, _MAX_PATH, szScratch );
			strcat_s( szDumpPath, _MAX_PATH, "\\TradeLinkMiniDump" );
			strcat_s( szDumpPath, _MAX_PATH, ".dmp" );
			szScratch[0] = '\0'; // reset szScratch
			HANDLE hFile = ::CreateFile( szDumpPath, GENERIC_WRITE, FILE_SHARE_WRITE, NULL, CREATE_ALWAYS,
										FILE_ATTRIBUTE_NORMAL, NULL );

			if (hFile!=INVALID_HANDLE_VALUE)
			{
				_MINIDUMP_EXCEPTION_INFORMATION objMiniDumpInfo;

				objMiniDumpInfo.ThreadId = ::GetCurrentThreadId();
				objMiniDumpInfo.ExceptionPointers = ExInfo;
				objMiniDumpInfo.ClientPointers = NULL;

				// write the dump
				BOOL bOK = pDump( GetCurrentProcess(), GetCurrentProcessId(), hFile, MiniDumpNormal, &objMiniDumpInfo, NULL, NULL );
				if (bOK)
				{
					sprintf_s( szScratch, _MAX_PATH, "Saved dump file to '%s'", szDumpPath );
					szResult = szScratch;
				}
				else
				{
					sprintf_s( szScratch, _MAX_PATH, "Failed to save dump file to '%s' (error %d)", szDumpPath, GetLastError() );
					szResult = szScratch;
				}
				::CloseHandle(hFile);
			}
			else
			{
				sprintf_s( szScratch, _MAX_PATH, "Failed to create dump file '%s' (error %d)", szDumpPath, GetLastError() );
				szResult = szScratch;
			}
		}
		else
		{
			szResult = "DBGHELP.DLL too old (no MiniDumpWriteDump function in dll)";
		}
	}
	else
	{
		szResult = "DBGHELP.DLL not found";
	}

	////////////////////////////////////////////////
	// b. Write some info to a file [in case the minidump didn't work; or just to have additional info]
	FILE *logFile;
	if(fopen_s(&logFile, "WinFault.log", "a") == 0 && logFile != NULL)
	{
		const size_t arrSize = 256;
		char chrTime[arrSize];
		time_t objCurrentTime = 0;
		time(&objCurrentTime);
		ctime_s(chrTime, arrSize, &objCurrentTime);

		fprintf(logFile, "****************************************************\n");
		fprintf(logFile, "*** A Program Fault occurred at:");
		fprintf(logFile, chrTime);
		fprintf(logFile, "\n");
		fflush(logFile);

		if (szResult != NULL) 
		{
			fprintf(logFile, "*** MiniDump Results: %s\n", szResult);
			fflush(logFile);
		}

		if (ExInfo == NULL || ExInfo->ExceptionRecord == NULL) 
		{ 
			fprintf(logFile, "*** The information included in the ExceptionRecord was NULL. \n  * Unable to retrieve additional information.");
		} 
		else 
		{ 
			int    wsFault    = ExInfo->ExceptionRecord->ExceptionCode; // equivalent to GetExceptionCode () ?
			void  *codeAdress = ExInfo->ExceptionRecord->ExceptionAddress;
			char  *faultTx = NULL;
			switch(wsFault)
			{
			case EXCEPTION_ACCESS_VIOLATION          : faultTx = "ACCESS VIOLATION"         ; break;
			case EXCEPTION_DATATYPE_MISALIGNMENT     : faultTx = "DATATYPE MISALIGNMENT"    ; break;
			case EXCEPTION_BREAKPOINT                : faultTx = "BREAKPOINT"               ; break;
			case EXCEPTION_SINGLE_STEP               : faultTx = "SINGLE STEP"              ; break;
			case EXCEPTION_ARRAY_BOUNDS_EXCEEDED     : faultTx = "ARRAY BOUNDS EXCEEDED"    ; break;
			case EXCEPTION_FLT_DENORMAL_OPERAND      : faultTx = "FLT DENORMAL OPERAND"     ; break;
			case EXCEPTION_FLT_DIVIDE_BY_ZERO        : faultTx = "FLT DIVIDE BY ZERO"       ; break;
			case EXCEPTION_FLT_INEXACT_RESULT        : faultTx = "FLT INEXACT RESULT"       ; break;
			case EXCEPTION_FLT_INVALID_OPERATION     : faultTx = "FLT INVALID OPERATION"    ; break;
			case EXCEPTION_FLT_OVERFLOW              : faultTx = "FLT OVERFLOW"             ; break;
			case EXCEPTION_FLT_STACK_CHECK           : faultTx = "FLT STACK CHECK"          ; break;
			case EXCEPTION_FLT_UNDERFLOW             : faultTx = "FLT UNDERFLOW"            ; break;
			case EXCEPTION_INT_DIVIDE_BY_ZERO        : faultTx = "INT DIVIDE BY ZERO"       ; break;
			case EXCEPTION_INT_OVERFLOW              : faultTx = "INT OVERFLOW"             ; break;
			case EXCEPTION_PRIV_INSTRUCTION          : faultTx = "PRIV INSTRUCTION"         ; break;
			case EXCEPTION_IN_PAGE_ERROR             : faultTx = "IN PAGE ERROR"            ; break;
			case EXCEPTION_ILLEGAL_INSTRUCTION       : faultTx = "ILLEGAL INSTRUCTION"      ; break;
			case EXCEPTION_NONCONTINUABLE_EXCEPTION  : faultTx = "NONCONTINUABLE EXCEPTION" ; break;
			case EXCEPTION_STACK_OVERFLOW            : faultTx = "STACK OVERFLOW"           ; break;
			case EXCEPTION_INVALID_DISPOSITION       : faultTx = "INVALID DISPOSITION"      ; break;
			case EXCEPTION_GUARD_PAGE                : faultTx = "GUARD PAGE"               ; break;
			default: faultTx = "(unknown)";           break;
			}  
			fprintf(logFile, "*** Error code %08X: %s\n", wsFault, faultTx);
			fprintf(logFile, "****************************************************\n");
			fprintf(logFile, "***   Address: %08p\n", (intptr_t)codeAdress);
			fprintf(logFile, "***     Flags: %08X - Continuable %c?\n", ExInfo->ExceptionRecord->ExceptionFlags, ExInfo->ExceptionRecord->ExceptionFlags == 0 ? 'Y' : 'N'); // if not 0, should be EXCEPTION_NONCONTINUABLE (0x1)
			fflush(logFile);

			if( ( wsFault == EXCEPTION_ACCESS_VIOLATION ||    // the parameters are only present in these two scenarios
				wsFault == EXCEPTION_IN_PAGE_ERROR) && 
				ExInfo->ExceptionRecord->NumberParameters > 1)
			{
				const size_t arrSize=256;
				char arrOperationType[arrSize];
				
				switch (ExInfo->ExceptionRecord->ExceptionInformation[0]) { 
				case 0: 
					strcpy_s(arrOperationType, arrSize, "read from");
					break;
				case 1:
					strcpy_s(arrOperationType, arrSize,"write to");
					break;
				case 8: 
					strcpy_s(arrOperationType, arrSize, "User DEP");
					break;
				default:
					strcpy_s(arrOperationType, arrSize, "?");
					break;
				}
				fprintf(logFile, "****************************************************\n");
				fprintf(logFile, "*** Attempted a %s virtual address %08p \n", 
					arrOperationType,
					ExInfo->ExceptionRecord->ExceptionInformation[1]);
				// (only for EXCEPTION_IN_PAGE_ERROR) 
				// The third array element specifies the underlying NTSTATUS code that resulted in the exception.
				if  (ExInfo->ExceptionRecord->NumberParameters >= 3) 
				{
					fprintf(logFile, "*** Underlying NTSTATUS code %08p \n", 
						ExInfo->ExceptionRecord->ExceptionInformation[2]);
				}
				fflush(logFile);

			}
			if (ExInfo->ExceptionRecord->ExceptionRecord != NULL) 
			{ 
				fprintf(logFile, "Have nested exception records!\n");
				fflush(logFile);
			}   
		} // end have valid exception record

		fprintf(logFile, "****************************************************\n");
		fclose(logFile);
	} // end have valid log file ... 

	// Make this continue so we still get the default windows handler ! 
	return EXCEPTION_CONTINUE_SEARCH;
}

void InstallFaultHandler() 
{
	sgOldExceptionFilter = SetUnhandledExceptionFilter((LPTOP_LEVEL_EXCEPTION_FILTER)WinFaultHandler);
	sgSuccessfullySetOurFilter = true;
}

// revert back to whatever we were using before
void RevertFaultHandler() 
{
	if (sgSuccessfullySetOurFilter) 
	{ 
		SetUnhandledExceptionFilter(sgOldExceptionFilter);
		sgSuccessfullySetOurFilter = false;
	}
}

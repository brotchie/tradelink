#pragma once

/*----------------------------------------------------------------------
 * Purpose:
 *		Header file for inclusion by test code.
 *
 *            cfixaux.h        cfixkrio.h
 *              ^ ^ ^--------+     ^
 *             /   \          \   /
 *            /     \          \ /
 *		cfixapi.h  cfixpe.h  cfixkr.h
 *			^	  ^	  ^         
 *			|	 /	  |         
 *			|	/	  |         
 *		  [cfix]	cfix.h      
 *                    ^         
 *                    |         
 *                    |         
 *          [Test DLLs/Drivers] 
 *
 * Copyright:
 *		2008, Johannes Passing (passing at users.sourceforge.net)
 *
 * This file is part of cfix.
 *
 * cfix is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * cfix is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with cfix.  If not, see <http://www.gnu.org/licenses/>.
 */

#ifdef CFIX_KERNELMODE
	#ifndef CFIX_NO_LIB
	#pragma comment( lib, "aux_klib" )
	#pragma comment( lib, "cfixkdrv" )
	#endif

	#include <wdm.h>

	//
	// Routines are statically linked.
	//
	#define CFIXREPORTAPI
#else
	#ifndef CFIX_NO_LIB
	#pragma comment( lib, "cfix" )
	#endif

	#define CFIXREPORTAPI CFIXAPI

	#include <windows.h>
#endif

#include <cfixpe.h>

/*++
	Routine Description:
		Report an event to the current execution context.

		Only for use from within a testcase.
--*/
CFIXREPORTAPI CFIX_REPORT_DISPOSITION CFIXCALLTYPE CfixPeReportFailedAssertion(
	__in PCWSTR File,
	__in PCWSTR Routine,
	__in ULONG Line,
	__in PCWSTR Expression
	);

/*++
	Routine Description:
		Test Expected and Actual for equality. If the parameters do not
		equal, a failed assretion is reported.

		Only for use from within a testcase.
--*/
CFIXREPORTAPI CFIX_REPORT_DISPOSITION CFIXCALLTYPE CfixPeAssertEqualsUlong(
	__in ULONG Expected,
	__in ULONG Actual,
	__in PCWSTR File,
	__in PCWSTR Routine,
	__in ULONG Line,
	__in PCWSTR Expression,
	__reserved ULONG Reserved
	);

//
// Compat 1.0
//
#define CfixPeAssertEqualsDword CfixPeAssertEqualsUlong

/*++
	Routine Description:
		Report that testcase is inconclusive.

		Only for use from within a testcase.
--*/
CFIXREPORTAPI VOID CFIXCALLTYPE CfixPeReportInconclusiveness(
	__in PCWSTR Message
	);

/*++
	Routine Description:
		Report log event.

		Only for use from within a testcase.
--*/
CFIXREPORTAPI VOID __cdecl CfixPeReportLog(
	__in PCWSTR Format,
	...
	);

#define CFIX_ASSERT_EXPR( expr, msg )						\
	( void ) ( ( !! ( expr ) ) ||							\
		( CfixBreak != CfixPeReportFailedAssertion(			\
			__CFIX_WIDE( __FILE__ ),						\
			__CFIX_WIDE( __FUNCTION__ ),					\
			__LINE__,										\
			msg ) ||										\
		( __debugbreak(), 0 ) ) )

#define CFIX_ASSERT_EQUALS_DWORD( Expected, Actual )		\
	( void ) ( ( CfixBreak != CfixPeAssertEqualsDword(		\
			Expected,										\
			Actual,											\
			__CFIX_WIDE( __FILE__ ),						\
			__CFIX_WIDE( __FUNCTION__ ),					\
			__LINE__,										\
			__CFIX_WIDE( #Actual ),							\
			0 ) ||											\
		( __debugbreak(), 0 ) ) )
#define CFIX_ASSERT_EQUALS_ULONG CFIX_ASSERT_EQUALS_DWORD

#define CFIX_INCONCLUSIVE( msg )							\
	CfixPeReportInconclusiveness(	msg )

#define CFIX_LOG CfixPeReportLog

#define CFIX_ASSERT( expr )	CFIX_ASSERT_EXPR( expr, __CFIX_WIDE( #expr ) )

#ifndef CFIX_KERNELMODE
/*++
	Routine Description:
		Creates a thread like CreateThread does, but registers the
		thread appropriately s.t. assrertions, unhandled exceptions
		etc. can be properly handled by the framwwork.

		If the thread is aborted due to an unhandled exception, assertion
		etc, the thread's exit code is CFIX_E_THREAD_ABORTED.

	Parameters:
		See MSDN.
--*/
CFIXAPI HANDLE CFIXCALLTYPE CfixCreateThread(
	__in PSECURITY_ATTRIBUTES ThreadAttributes,
	__in SIZE_T StackSize,
	__in PTHREAD_START_ROUTINE StartAddress,
	__in PVOID Parameter,
	__in DWORD CreationFlags,
	__in PDWORD ThreadId
	);

#endif // CFIX_KERNELMODE
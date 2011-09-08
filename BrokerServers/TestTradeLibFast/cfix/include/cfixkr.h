#pragma once

/*----------------------------------------------------------------------
 * Purpose:
 *		Cfix Kernel API. Interface between test drivers and cfixkr.
 *
 *		Shared header file. Do not include directly!
 *		 
 *		N.B. Include wdm.h before including this header. Do not use for
 *		user mode modules.
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
#include <initguid.h>
#include <cfixaux.h>
#include <cfixkrio.h>

#ifdef _WIN64
#define CFIXCALLTYPE
#else
#define CFIXCALLTYPE __stdcall
#endif

#define CFIXKR_DEVICE_NT_NAME	L"\\Device\\Cfixkr"
#define CFIXKR_DEVICE_DOS_NAME	L"\\DosDevices\\Cfixkr"

/*----------------------------------------------------------------------
 *
 * Report Sink interface exposed by cfixkr driver for use by test drivers.
 *
 */

/*++
	Interface GUID.
		{118F0A99-C318-45da-B55C-6E19A4E1240C}
--*/
DEFINE_GUID(GUID_CFIXKR_REPORT_SINK, 
	0x118f0a99, 0xc318, 0x45da, 0xb5, 0x5c, 0x6e, 0x19, 0xa4, 0xe1, 0x24, 0xc);

#define CFIXKR_REPORT_SINK_VERSION 0x1000

/*++
	Routine Description:
		Report an event to the current execution context.

		Only for use from within a testcase.

		Callable at IRQL <= DISPATCH_LEVEL.
--*/
typedef CFIX_REPORT_DISPOSITION ( CFIXCALLTYPE * CFIXKR_REPORT_FAILED_ASSERTION_ROUTINE )(
	__in PVOID Context,
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

		Callable at IRQL <= DISPATCH_LEVEL.
--*/
typedef CFIX_REPORT_DISPOSITION ( CFIXCALLTYPE * CFIXKR_ASSERT_EQUALS_ULONG_ROUTINE )(
	__in PVOID Context,
	__in ULONG Expected,
	__in ULONG Actual,
	__in PCWSTR File,
	__in PCWSTR Routine,
	__in ULONG Line,
	__in PCWSTR Expression,
	__reserved ULONG Reserved
	);

/*++
	Routine Description:
		Report that testcase is inconclusive.

		Only for use from within a testcase.

		Callable at IRQL <= DISPATCH_LEVEL.
--*/
typedef VOID ( CFIXCALLTYPE * CFIXKR_REPORT_INCONCLUSIVENESS_ROUTINE )(
	__in PVOID Context,
	__in PCWSTR Message
	);

/*++
	Routine Description:
		Report log event.

		Only for use from within a testcase.

		Callable at IRQL <= DISPATCH_LEVEL.
--*/
typedef VOID ( CFIXCALLTYPE * CFIXKR_REPORT_LOG_ROUTINE )(
	__in PVOID Context,
	__in PCWSTR Format,
	__in va_list Args
	);

/*++
	Structure Description:
		See CFIXKR_REPORT_SINK_INTERFACE.
--*/
typedef struct _CFIXKR_REPORT_SINK_METHODS
{
	CFIXKR_REPORT_FAILED_ASSERTION_ROUTINE ReportFailedAssertion;
	CFIXKR_ASSERT_EQUALS_ULONG_ROUTINE AssertEqualsUlong;
	CFIXKR_REPORT_INCONCLUSIVENESS_ROUTINE ReportInconclusiveness;
	CFIXKR_REPORT_LOG_ROUTINE ReportLog;
} CFIXKR_REPORT_SINK_METHODS, *PCFIXKR_REPORT_SINK_METHODS;

/*++
	Structure Description:
		Interface between test driver and cfixkr.
--*/
typedef struct _CFIXKR_REPORT_SINK_INTERFACE
{
	INTERFACE Base;
	CFIXKR_REPORT_SINK_METHODS Methods;
} CFIXKR_REPORT_SINK_INTERFACE, *PCFIXKR_REPORT_SINK_INTERFACE;

#pragma once

/*----------------------------------------------------------------------
 * Purpose:
 *		Cfix Kernel API - I/O declarations. Used for communication
 *		between cfixkl and cfixkr.
 *
 *		Shared header file. Do not include directly!
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

//
// N.B. Load Addresses are typed as ULONGLONGs, even on i386. This 
// is necessary for WOW64 compatibility.
//

#define CFIXKR_TYPE					( ( ULONG )		0x8000 )
#define CFIXKR_IOCTL_BASE			( ( USHORT )	0x1000 )


#ifndef CTL_CODE
	#define CTL_CODE( DeviceType, Function, Method, Access ) (                 \
		((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method) \
	)

	#define METHOD_BUFFERED                 0
#endif 

/*----------------------------------------------------------------------
 *
 * CFIXKR_IOCTL_QUERY_TEST_MODULE.
 *
 */
typedef struct _CFIXKRIO_FIXTURE_ENTRY
{
	//
	// Type of entry: 
	//	- CfixEntryTypeSetup
	//	- CfixEntryTypeTeardown
	//	- CfixEntryTypeTestcase
	// (CfixEntryTypeEnd is not a valid value)
	//
	USHORT Type;
	USHORT Key;

	//
	// Offset of Name string, relative to TestModule.
	// The string is Unicode and not zero-terminated.
	//
	ULONG NameOffset;

	//
	// Length of Name in bytes.
	//
	ULONG NameLength;
} CFIXKRIO_FIXTURE_ENTRY, *PCFIXKRIO_FIXTURE_ENTRY;

typedef struct _CFIXKRIO_FIXTURE
{
	USHORT Key;

	//
	// Offset of Name string, relative to TestModule.
	// The string is Unicode and not zero-terminated.
	//
	ULONG NameOffset;

	//
	// Length of Name in bytes.
	//
	ULONG NameLength;
	ULONG EntryCount;
	CFIXKRIO_FIXTURE_ENTRY Entries[ ANYSIZE_ARRAY ];
} CFIXKRIO_FIXTURE, *PCFIXKRIO_FIXTURE;

typedef struct _CFIXKRIO_TEST_MODULE
{
	ULONG FixtureCount;
	ULONG FixtureOffsets[ ANYSIZE_ARRAY ];
} CFIXKRIO_TEST_MODULE, *PCFIXKRIO_TEST_MODULE;

typedef struct _CFIXKR_IOCTL_QUERY_TEST_MODULE_REQUEST
{
	//
	// Base/Load address of the driver to be queried.
	// The driver must have already been loaded.
	//
	ULONGLONG DriverBaseAddress;
} CFIXKR_IOCTL_QUERY_TEST_MODULE_REQUEST, 
*PCFIXKR_IOCTL_QUERY_TEST_MODULE_REQUEST;

typedef struct _CFIXKR_IOCTL_QUERY_TEST_MODULE_RESPONSE
{
	union
	{
		//
		// Required overall buffer size in bytes. Set when
		// STATUS_BUFFER_OVERFLOW was returned.
		//
		ULONG SizeRequired;

		//
		// Requested information. Set on successful return.
		//
		// Field is variable-length.
		//
		CFIXKRIO_TEST_MODULE TestModule;
	} u;
} CFIXKR_IOCTL_QUERY_TEST_MODULE_RESPONSE, 
*PCFIXKR_IOCTL_QUERY_TEST_MODULE_RESPONSE;

/*++
	IOCTL Description:
		Obtain fixture information from a loaded driver.

	Input:
		CFIXKR_IOCTL_QUERY_TEST_MODULE_REQUEST structure.
	
	Output:
		CFIXKR_IOCTL_QUERY_TEST_MODULE_RESPONSE structure.

	Return Values:
		STATUS_SUCCESS if successfully attached.
		STATUS_OBJECT_NAME_NOT_FOUND if driver not found
--*/
#define CFIXKR_IOCTL_QUERY_TEST_MODULE	CTL_CODE(	\
	CFIXKR_TYPE,									\
	CFIXKR_IOCTL_BASE + 1,							\
	METHOD_BUFFERED,								\
	FILE_WRITE_DATA )

/*----------------------------------------------------------------------
 *
 * CFIXKR_IOCTL_GET_TEST_MODULES.
 *
 */
typedef struct _CFIXKR_IOCTL_GET_TEST_MODULES_RESPONSE
{
	ULONG Count;
	ULONGLONG DriverLoadAddress[ ANYSIZE_ARRAY ];
} CFIXKR_IOCTL_GET_MODULES, 
*PCFIXKR_IOCTL_GET_MODULES;

/*++
	IOCTL Description:
		Obtain fixture information from a loaded driver.

	Input:
		None.
	
	Output:
		CFIXKR_IOCTL_GET_TEST_MODULES_RESPONSE structure.

	Return Values:
		STATUS_SUCCESS if buffer was large enough.
			Count will contain the number of elements written.
		STATUS_BUFFER_OVERFLOW if buffer was too small.
			DriverLoadAddress will contain as many elements as 
			Count initially specified.
			Count is set to the total number of elements available.
--*/
#define CFIXKR_IOCTL_GET_TEST_MODULES	CTL_CODE(	\
	CFIXKR_TYPE,									\
	CFIXKR_IOCTL_BASE + 2,							\
	METHOD_BUFFERED,								\
	FILE_WRITE_DATA )

/*----------------------------------------------------------------------
 *
 * CFIXKR_IOCTL_CALL_ROUTINE.
 *
 */
typedef enum _CFIXKR_EXCEPTION_RECORD_TYPE
{
	CfixkrExceptionRecordNone	= 0,
	CfixkrExceptionRecord32		= 1,
	CfixkrExceptionRecord64		= 2,
} CFIXKR_EXCEPTION_RECORD_TYPE;

#ifdef _WIN64
	#define CfixkrExceptionRecordDefault CfixkrExceptionRecord64
#else
	#define CfixkrExceptionRecordDefault CfixkrExceptionRecord32
#endif

typedef struct _CFIXKR_EXECUTION_EVENT
{
	//
	// Type of event. See CFIX_EVENT_TYPE.
	//
	USHORT Type;

	//
	// Size of this struct, including all variable-length data referred
	// to by the Info-members.
	//
	// Must be a multiple of 8.
	//
	USHORT Size;

	//
	// N.B. Offsets are relative to the beginning of *this*
	// structure. All strings are Unicode and not null-terminated.
	//
	union
	{
		struct
		{
			CFIXKR_EXCEPTION_RECORD_TYPE Type;

			//
			// Both 32 and 64 bit records to support WOW64. The client
			// has to convert properly.
			//
			union
			{
				EXCEPTION_RECORD   ExceptionRecord;
				EXCEPTION_RECORD32 ExceptionRecord32;
				EXCEPTION_RECORD64 ExceptionRecord64;
			} u;
		} UncaughtException;
		
		struct
		{
			USHORT FileOffset;
			USHORT FileLength;

			USHORT RoutineOffset;
			USHORT RoutineLength;

			ULONG Line;

			USHORT ExpressionOffset;
			USHORT ExpressionLength;
		} FailedAssertion;

		struct 
		{
			USHORT MessageOffset;
			USHORT MessageLength;
		} Inconclusiveness;

		struct 
		{
			USHORT MessageOffset;
			USHORT MessageLength;
		} Log;
	} Info;

	//
	// N.B. Structure is variable length.
	//
	CFIX_STACKTRACE StackTrace;
} CFIXKR_EXECUTION_EVENT, *PCFIXKR_EXECUTION_EVENT;

C_ASSERT( FIELD_OFFSET( CFIXKR_EXECUTION_EVENT, Info.Log.MessageOffset ) ==
		  FIELD_OFFSET( CFIXKR_EXECUTION_EVENT, Info.Inconclusiveness.MessageOffset ) );
C_ASSERT( FIELD_OFFSET( CFIXKR_EXECUTION_EVENT, Info.Log.MessageLength ) ==
		  FIELD_OFFSET( CFIXKR_EXECUTION_EVENT, Info.Inconclusiveness.MessageLength ) );

typedef struct _CFIXKR_IOCTL_CALL_ROUTINE_REQUEST
{
	ULONGLONG DriverBaseAddress;
	USHORT FixtureKey;
	USHORT RoutineKey;

	//
	// Dispositions to use when a event occurs. Unlike user mode
	// testing, these decisions must be made before the test routine
	// can be called.
	//
	struct
	{
		UCHAR FailedAssertion; 
		UCHAR UnhandledException;
	} Dispositions;

	USHORT Padding;
} CFIXKR_IOCTL_CALL_ROUTINE_REQUEST, 
*PCFIXKR_IOCTL_CALL_ROUTINE_REQUEST;

typedef struct _CFIXKR_IOCTL_CALL_ROUTINE_RESPONSE
{
	//
	// Indicates whether the routine has run in its entirety or
	// has been aborted by an exception (unhandled or triggered by
	// a failed assertion).
	//
	BOOLEAN RoutineRanToCompletion;

	//
	// If a failed assertion has occured and the disposition has
	// indicated the testrun to be aborted, this flag is set to TRUE.
	//
	// The following holds: RoutineRanToCompletion => ! AbortRun
	//
	BOOLEAN AbortRun;

	struct
	{
		//
		// Flag indicating that the buffer was too small to contain
		// all events that have been generated.
		//
		#define CFIXKR_CALL_ROUTINE_FLAG_EVENTS_TRUNCATED	1
		
		USHORT Flags;

		//
		// Number of event structures following this structure.
		//
		ULONG Count;
	} Events;
} CFIXKR_IOCTL_CALL_ROUTINE_RESPONSE, 
*PCFIXKR_IOCTL_CALL_ROUTINE_RESPONSE;

/*++
	IOCTL Description:
		Obtain fixture information from a loaded driver.

	Input:
		CFIXKR_IOCTL_CALL_ROUTINE_REQUEST structure.

	Output:
		CFIXKR_IOCTL_CALL_ROUTINE_RESPONSE structure.
	
	Output:
		None.
--*/
#define CFIXKR_IOCTL_CALL_ROUTINE	CTL_CODE(		\
	CFIXKR_TYPE,									\
	CFIXKR_IOCTL_BASE + 3,							\
	METHOD_BUFFERED,								\
	FILE_WRITE_DATA )

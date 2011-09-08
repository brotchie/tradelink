#pragma once

/*----------------------------------------------------------------------
 * Purpose:
 *		Cfix main header file. 
 *
 *		Note: Test code should include cfix.h.
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

#include <windows.h>
#include <cfixaux.h>
#include <cfixmsg.h>

struct _CFIX_FIXTURE;
struct _CFIX_TEST_CASE;
struct _CFIX_TEST_MODULE;

#ifndef OPTIONAL
#define OPTIONAL
#endif

/*----------------------------------------------------------------------
 *
 * Context Object.
 *
 */

#define CFIX_TEST_CONTEXT_VERSION MAKELONG( 1, 0 )

typedef struct _CFIX_TESTCASE_EXECUTION_EVENT
{
	CFIX_EVENT_TYPE Type;
	union
	{
		struct
		{
			EXCEPTION_RECORD ExceptionRecord;
		} UncaughtException;
		
		struct
		{
			PCWSTR File;
			PCWSTR Routine;
			UINT Line;
			PCWSTR Expression;
			DWORD LastError;
		} FailedAssertion;

		struct 
		{
			PCWSTR Message;
		} Inconclusiveness;

		struct 
		{
			PCWSTR Message;
		} Log;
	} Info;

	//
	// N.B. Structure is variable length.
	//
	CFIX_STACKTRACE StackTrace;
} CFIX_TESTCASE_EXECUTION_EVENT, *PCFIX_TESTCASE_EXECUTION_EVENT;

/*++
	Structure Description:
		Defines the interface of an execution context object.

		Note that the object can be used from multiple threads
		in parallel. The implemenatation must implement proper
		state tracking on a per-thread basis.
--*/
typedef struct _CFIX_EXECUTION_CONTEXT
{
	/*++
		Field Descritpion:
			Version, set to CFIX_TEST_CONTEXT_VERSION.
	--*/
	DWORD Version;

	/*++
		Routine Description:
			Report an event. See also the discussion of 
			QueryDefaultDisposition.

		Return Value:
			Disposition how caller should proceed.

			In the case of unhandled exceptions, CfixBreak instructs
			the framework not to handle the exception. Most likely,
			the process will experience an unhandled exception and
			the user has the chance to break in using the debugger.
	--*/
	CFIX_REPORT_DISPOSITION ( CFIXCALLTYPE * ReportEvent ) (
		__in struct _CFIX_EXECUTION_CONTEXT *This,
		__in PCFIX_TESTCASE_EXECUTION_EVENT Event
	);

	/*++
		Routine Description:
			Query the disposition for an event type, regardless of
			a specific instance of the event.

			This routine will not be called for user mode tests -
			ReportEvent is called for each event individually.

			For kernel mode tests, the disposition will be queried
			beforehand by calling this routine. Only after the test case
			has finished, will any events be reported via ReportEvent.
			The disposition returned by ReportEvent will in this case
			be ignored.

		Return Value:
			Disposition how caller should proceed.
	--*/
	CFIX_REPORT_DISPOSITION ( CFIXCALLTYPE * QueryDefaultDisposition ) (
		__in struct _CFIX_EXECUTION_CONTEXT *This,
		__in CFIX_EVENT_TYPE EventType
	);

	VOID ( CFIXCALLTYPE * BeforeFixtureStart ) (
		__in struct _CFIX_EXECUTION_CONTEXT *This,
		__in struct _CFIX_FIXTURE *Fixture
		);

	VOID ( CFIXCALLTYPE * AfterFixtureFinish ) (
		__in struct _CFIX_EXECUTION_CONTEXT *This,
		__in struct _CFIX_FIXTURE *Fixture,
		__in BOOL RanToCompletion
		);

	VOID ( CFIXCALLTYPE * BeforeTestCaseStart ) (
		__in struct _CFIX_EXECUTION_CONTEXT *This,
		__in struct _CFIX_TEST_CASE *TestCase
		);

	VOID ( CFIXCALLTYPE * AfterTestCaseFinish ) (
		__in struct _CFIX_EXECUTION_CONTEXT *This,
		__in struct _CFIX_TEST_CASE *TestCase,
		__in BOOL RanToCompletion
		);

	/*++
		Routine Description:
			Called when the test code wants to spawn a new thread that
			belongs to the currently executing testcase. Gives
			the execution context the opportunity to relate the
			new thread to its 'parent' thread.

			The implementation must unltimately call kernel32!Create 
			thread.
	--*/
	HANDLE ( CFIXCALLTYPE * CreateChildThread ) (
		__in struct _CFIX_EXECUTION_CONTEXT *This,
		__in PSECURITY_ATTRIBUTES ThreadAttributes,
		__in SIZE_T StackSize,
		__in PTHREAD_START_ROUTINE StartAddress,
		__in PVOID UserParameter,
		__in DWORD CreationFlags,
		__in PDWORD ThreadId
		);

	/*++
		Routine Description:
			Called when an unhandled exception occurs in test code.
			
			This gives the object the option to create dump files etc.
	--*/
	VOID ( CFIXCALLTYPE * OnUnhandledException ) (
		__in struct _CFIX_EXECUTION_CONTEXT *This,
		__in PEXCEPTION_POINTERS ExcpPointers
		);

} CFIX_EXECUTION_CONTEXT, *PCFIX_EXECUTION_CONTEXT;

#define CfixIsValidContext( Context ) ( 							\
	 ( Context ) &&													\
	 ( Context )->Version == CFIX_TEST_CONTEXT_VERSION &&			\
	 ( Context )->ReportEvent &&									\
	 ( Context )->BeforeFixtureStart &&								\
	 ( Context )->AfterFixtureFinish &&								\
	 ( Context )->BeforeTestCaseStart &&							\
	 ( Context )->AfterTestCaseFinish &&							\
	 ( Context )->CreateChildThread &&								\
	 ( Context )->OnUnhandledException )

/*----------------------------------------------------------------------
 *
 * Test Module/Fixture/Case.
 *
 */

/*++
	Description:
		Prototype of setup, teardown and testcase routines.

	Parameters:
		Module		Module fixture belongs to.
		TestCase	Testcase to run.

	Return Values:
		S_OK on success. Even if test failed but execution ran properly,
			S_OK is to be returned.
		CFIX_E_TESTRUN_ABORTED is tesrun is to be aborted.
		Failure HRESULT if an error occured and the whole run should
			be aborted.
--*/
typedef HRESULT ( CFIXCALLTYPE * CFIX_TESTCASE_ROUTINE ) (
	__in struct _CFIX_FIXTURE *Fixture,
	__in struct _CFIX_TEST_CASE *TestCase,
	__in PCFIX_EXECUTION_CONTEXT Context
	);

/*++
	Description:
		Prototype of setup, teardown and testcase routines.

	Parameters:
		Module		Module fixture belongs to.
		Fixture		Fixture to setup/teardown.

	Return Values:
		S_OK on success. Even if test failed but execution ran properly,
			S_OK is to be returned.
		Failure HRESULT if an error occured and the whole run should
			be aborted.
--*/
typedef HRESULT ( CFIXCALLTYPE * CFIX_SETUPTEARDOWN_ROUTINE ) (
	__in struct _CFIX_FIXTURE *Fixture,
	__in PCFIX_EXECUTION_CONTEXT Context
	);

/*++
	Routine Description:	
		Increment/Decrement module reference counter.
--*/
typedef VOID ( CFIXCALLTYPE * CFIX_ADJ_REFERENCES_ROUTINE ) (
	__in struct _CFIX_TEST_MODULE *TestModule
	);

/*++
	Routine Description:
		Retrieve information about a stackframe.
--*/
typedef HRESULT ( CFIXCALLTYPE * CFIX_GET_INFORMATION_STACKFRAME_ROUTINE ) (
	__in ULONGLONG Frame,
	__in SIZE_T ModuleNameCch,
	__out_ecount(ModuleNameCch) PWSTR ModuleName,
	__in SIZE_T FunctionNameCch,
	__out_ecount(FunctionNameCch) PWSTR FunctionName,
	__out PDWORD Displacement,
	__in SIZE_T SourceFileCch,
	__out_ecount(SourceFileCch) PWSTR SourceFile,
	__out PDWORD SourceLine 
	);

typedef struct _CFIX_TEST_CASE
{
	PCWSTR Name;
	ULONG_PTR Routine;
	struct _CFIX_FIXTURE *Fixture;
} CFIX_TEST_CASE, *PCFIX_TEST_CASE;

typedef struct _CFIX_FIXTURE
{
	WCHAR Name[ CFIX_MAX_FIXTURE_NAME_CCH ];
	ULONG_PTR SetupRoutine;
	ULONG_PTR TeardownRoutine;

	//
	// Implementation-defined.
	//
	ULONG_PTR Reserved;

	//
	// Backpointer to enclosing module.
	//
	struct _CFIX_TEST_MODULE *Module;

	//
	// # of elements in TestCases array.
	//
	UINT TestCaseCount;
	CFIX_TEST_CASE TestCases[ ANYSIZE_ARRAY ];
} CFIX_FIXTURE, *PCFIX_FIXTURE;

#define CFIX_TEST_MODULE_VERSION MAKELONG( 1, 0 )

typedef struct _CFIX_TEST_MODULE
{
	//
	// Version. Set to CFIX_TEST_MODULE_VERSION.
	//
	ULONG Version;

	struct
	{
		CFIX_TESTCASE_ROUTINE RunTestCase;
		CFIX_SETUPTEARDOWN_ROUTINE Setup;
		CFIX_SETUPTEARDOWN_ROUTINE Teardown;
		CFIX_ADJ_REFERENCES_ROUTINE Reference;
		CFIX_ADJ_REFERENCES_ROUTINE Dereference;
		CFIX_GET_INFORMATION_STACKFRAME_ROUTINE GetInformationStackFrame OPTIONAL;
	} Routines;
	PCWSTR Name;
	UINT FixtureCount;
	PCFIX_FIXTURE *Fixtures;
} CFIX_TEST_MODULE, *PCFIX_TEST_MODULE;


/*----------------------------------------------------------------------
 *
 * Testrun.
 *
 */
#define CFIX_ACTION_VERSION MAKELONG( 1, 0 )

typedef struct _CFIX_ACTION
{
	/*++
		Interface version - set to CFIX_ACTION_VERSION.
	--*/
	DWORD Version;

	/*++
		Routine Description:
			Run the action. The actual action greatly depends
			on the implementation chosen.
	--*/
	HRESULT ( CFIXCALLTYPE * Run ) (
		__in struct _CFIX_ACTION *This,
		__in PCFIX_EXECUTION_CONTEXT Context
		);
	
	/*++
		Routine Description:
			Increment reference count.
	--*/
	VOID ( CFIXCALLTYPE * Reference ) (
		__in struct _CFIX_ACTION *This
		);

	/*++
		Routine Description:
			Dencrement reference count. Object is deleted if
			count reaches zero.
	--*/
	VOID ( CFIXCALLTYPE * Dereference ) (
		__in struct _CFIX_ACTION *This
		);
} CFIX_ACTION, *PCFIX_ACTION;

#define CfixIsValidAction( Action ) (								\
	Action &&														\
	Action->Version == CFIX_ACTION_VERSION &&						\
	Action->Run &&													\
	Action->Reference &&											\
	Action->Dereference )											\

/*----------------------------------------------------------------------
 *
 * API.
 *
 */

/*++
	Routine Description:	
		Loads a given module and creates a test module
		based on the module's exports.

	Parameters:
		ModulePath		Path to DLL.
		TestModule		Module - delete with CfixDeleteTestModule.
--*/
CFIXAPI HRESULT CFIXCALLTYPE CfixCreateTestModuleFromPeImage(
	__in PCWSTR ModulePath,
	__out PCFIX_TEST_MODULE *TestModule
	);

/*++
	Routine Description:	
		Loads a given driver and creates a test module
		based on the drivers's exports.

	Parameters:
		DriverPath		Path to driver.
		Installed		Indicates whether the driver had to be installed
		Loaded			Indicates whether the driver had to be loaded first.
		TestModule		Module - delete with CfixDeleteTestModule.
--*/
HRESULT CFIXCALLTYPE CfixklCreateTestModuleFromDriver(
	__in PCWSTR DriverPath,
	__out PCFIX_TEST_MODULE *TestModule,
	__out_opt PBOOL Installed,
	__out_opt PBOOL Loaded
	);

/*++
	Routine Description:
		Creates an action that executes sn entire fixture.
--*/
CFIXAPI HRESULT CFIXCALLTYPE CfixCreateFixtureExecutionAction(
	__in PCFIX_FIXTURE Fixture,
	__out PCFIX_ACTION *Action
	);

/*++
	Routine Description:
		Create a composite action that executes other actions
		in sequence.
--*/
CFIXAPI HRESULT CFIXCALLTYPE CfixCreateSequenceAction(
	__out PCFIX_ACTION *Action
	);

/*++
	Routine Description:
		Add an action to a sequence action created by 
		CfixCreateSequenceAction.
--*/
CFIXAPI HRESULT CFIXCALLTYPE CfixAddEntrySequenceAction(
	__in PCFIX_ACTION SequenceAction,
	__in PCFIX_ACTION ActionToAdd
	);

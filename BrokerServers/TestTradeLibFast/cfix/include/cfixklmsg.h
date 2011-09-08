// Copyright:
//		2008, Johannes Passing (passing at users.sourceforge.net)
//
// This file is part of cfix.
//
// cfix is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// cfix is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with cfix.  If not, see <http://www.gnu.org/licenses/>.
//
//--------------------------------------------------------------------
// Definitions
//--------------------------------------------------------------------
//--------------------------------------------------------------------
//
//  Values are 32 bit values laid out as follows:
//
//   3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1
//   1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
//  +---+-+-+-----------------------+-------------------------------+
//  |Sev|C|R|     Facility          |               Code            |
//  +---+-+-+-----------------------+-------------------------------+
//
//  where
//
//      Sev - is the severity code
//
//          00 - Success
//          01 - Informational
//          10 - Warning
//          11 - Error
//
//      C - is the Customer code flag
//
//      R - is a reserved bit
//
//      Facility - is the facility code
//
//      Code - is the facility's status code
//
//
// Define the facility codes
//


//
// Define the severity codes
//


//
// MessageId: CFIXKL_E_CFIXKR_NOT_FOUND
//
// MessageText:
//
// The cfix kernel reflector driver (cfixkr) could not be found.
//
#define CFIXKL_E_CFIXKR_NOT_FOUND        ((HRESULT)0x8004A000L)

//
// MessageId: CFIXKL_E_CFIXKR_START_FAILED
//
// MessageText:
//
// The cfix kernel reflector driver (cfixkr) could not be started.
//
#define CFIXKL_E_CFIXKR_START_FAILED     ((HRESULT)0x8004A001L)

//
// MessageId: CFIXKL_E_CFIXKR_START_DENIED
//
// MessageText:
//
// The cfix kernel reflector driver (cfixkr) could not be started due to lacking privileges.
//
#define CFIXKL_E_CFIXKR_START_DENIED     ((HRESULT)0x8004A002L)

//
// MessageId: CFIXKL_E_UNKNOWN_LOAD_ADDRESS
//
// MessageText:
//
// The driver load address cannot be determined.
//
#define CFIXKL_E_UNKNOWN_LOAD_ADDRESS    ((HRESULT)0x8004A003L)

//
// MessageId: CFIXKL_E_INVALID_REFLECTOR_RESPONSE
//
// MessageText:
//
// Invalid reflector response received.
//
#define CFIXKL_E_INVALID_REFLECTOR_RESPONSE ((HRESULT)0x8004A004L)


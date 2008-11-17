#include "stdafx.h"
#include "TLServer_NP.h"
#include "Util.h"

namespace TradeLibFast
{

	//IMPLEMENT_DYNAMIC(TLServer_NP)

	TLServer_NP::TLServer_NP(void)
	{
	}

	TLServer_NP::~TLServer_NP()
	{
	}

	// declare completion callback because async write call depends on it
	VOID WINAPI WriteCompleted(DWORD,DWORD,LPOVERLAPPED);

	void TLServer_NP::newTick(TLTick k)
	{
		// make sure tick has date and sym
		if ((k.sym=="") || (k.date==0)) return;

		// build pipe path
		CString pipename;
		pipename.Format(_T("\\.\\pipe\\%s%i"),k.sym,k.date);

		// check to see if we have a handle already
		map<CString,int>::iterator pi = _pidx.find(pipename);

		// if we don't have pipe, create one and connect to it
		if (pi==_pidx.end())
		{
			// create it
			HANDLE pipeh = CreateNamedPipe(pipename,
				PIPE_ACCESS_OUTBOUND|FILE_FLAG_OVERLAPPED,
				PIPE_TYPE_MESSAGE|PIPE_READMODE_MESSAGE|PIPE_NOWAIT,
				1,512,512,1000,NULL);

			// if it created, connect to it and save pipe
			if (pipeh!=INVALID_HANDLE_VALUE)
			{
				ConnectNamedPipe(pipeh,(LPOVERLAPPED)CreateEvent(NULL,TRUE,TRUE,NULL));
				_pipes.push_back(pipeh);
			}
			else // if it didn't create, drop the tick
				return;
		}
		// serialize tick and buffer it
		LPTSTR buf = k.Serialize().GetBuffer();

		// this is a required structure for overlapped/asych i/o (dont' understand it yet)
		LPOVERLAPPED ol;

		// Write buffer to pipe (when completed, callback is run)
		WriteFileEx(_pipes[_pidx[pipename]],buf,sizeof(buf),
			ol,(LPOVERLAPPED_COMPLETION_ROUTINE)WriteCompleted);

	}

	VOID WINAPI WriteCompleted(DWORD dwErr, DWORD cbBytesWritten, 
		LPOVERLAPPED lpOverLap) 
	{ 
		// empty callback, server just sends out ticks blindly 
	}
}
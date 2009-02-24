#pragma once
#include "afxsock.h"

class TLServer_IP :
	public CAsyncSocket
{
	void OnReceive(int nErrorCode);
	bool m_bReadyToSend;
	virtual void OnSend(int nErrorCode);

public:
	virtual bool Send(const void* lpBuf, int nBufLen ); 
	TLServer_IP(void);
	~TLServer_IP(void);
};

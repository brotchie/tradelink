#include "stdafx.h"
#include "SendMsg.h"


LRESULT SendMsg(int type,LPCTSTR msg,LPCTSTR windname) {
	LRESULT result = 999;
	HWND hdlAnVig = FindWindowA(NULL,windname);
	
	if (hdlAnVig) {
		COPYDATASTRUCT CD;  // windows-provided structure for this purpose
		CD.dwData=type;		// stores type of message
		int len = 0;
		len = (int)strlen((char*)msg);

		CD.cbData = len+1;
		CD.lpData = (void*)msg;	//here's the data we're sending
		result = SendMessageA(hdlAnVig,WM_COPYDATA,0,(LPARAM)&CD);
	} /*else {
		AfxMessageBox((LPCTSTR)EXTMISSING);
	}*/
	return result;
}

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
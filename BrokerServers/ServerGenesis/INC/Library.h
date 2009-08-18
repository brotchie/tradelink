// Library.h: interface for the CLibrary class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(__LIBRARY_H__)
#define __LIBRARY_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

//! \cond INTERNAL
class CLibrary  
{
protected:
	HMODULE		m_hLibrary;
	char		m_szLibName[256];

public:
	CLibrary();
	virtual ~CLibrary();

public:
	operator HMODULE() const
		{ return m_hLibrary; }

	virtual int LoadLibrary(LPCSTR pszLib);
	virtual int FreeLibrary();

	FARPROC GetProcAddress(LPCSTR lpszProcName);
	FARPROC GetProcAddress(WORD wProcName);

	virtual void ResetProcAddress();
	virtual int  LoadProcAddress();
};

//! \endcond
#endif // !defined(__LIBRARY_H__)

/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTSetting.h
	\brief interface for the GTSetting class.
 */

#ifndef __GTSETTING_H__
#define __GTSETTING_H__

#include "GTAPI_API.h"
#include "..\Inc32\GTSetting32.h"

typedef CMap<MMID, MMID, int, int> CMapMMID;
typedef CMap<CString, LPCSTR, int, int> CMapString;
typedef CList<MMID, MMID> GTHitList;

#define GTLEVEL2_SORTNO_IME	1
#define GTLEVEL2_SORTNO_IEM	2

/*! \ingroup cpp
*/
/*! \class GTSetting
	\brief Global settings for the trading system.

	This is the default settings for the trading system. This class derived from
	tagGTSetting32 struct. 

	\copydoc tagGTSetting32
*/
class GTAPI_API GTSetting : public GTSetting32
{
public:
	//! \brief [for Level 2] The ECN list
	/*!
		The record with an MMID in this list will be market as ECN.
	*/
	CMapMMID	m_ecns;
	//! \brief [for Level 2] The AXE list
	/*!
		The record with an MMID in this list will be market as axe.
	*/
	CMapMMID	m_axes;
	//! \brief [for Level 2] The hidden list
	/*!
		The record with an MMID in this list will be excluded when add a level 2 record.
	*/
	CMapMMID	m_hidden;
	//! \brief The ETF symbol list
	/*!
		The symbol in this list will be treated as ETF.
	*/
	CMapString	m_etf;
	//! \brief The command user list
	/*!
		The user name in this list will be granted with remote access to the API functions.
	*/
	CMapString	m_cmd_user;

	//! \brief [INTERNAL USED] Test only.
	/*!
	*/
	GTHitList	m_directhit;

public:
	GTSetting();
	virtual ~GTSetting();

public:	
	//! \brief Is the mmid in the m_ecns list?
	BOOL IsECN(MMID mmid) const
		{ int v; return m_ecns.Lookup(mmid, v); }
	//! \brief Is the mmid a book MMID?
	BOOL IsBook(MMID mmid) const
		{ return IS_ECN_BOOK_(mmid); }
	//! \brief Is the mmid totalview?
	BOOL IsTotalView(MMID mmid) const
		{ return IS_TOTAL_VIEW(mmid); }
	//! \brief Is the mmid openview?
	BOOL IsOpenView(MMID mmid) const
		{ return IS_OPEN_VIEW(mmid); }
	//! \brief Is the mmid axed?
	BOOL IsAxe(MMID mmid) const
		{ int v; return m_axes.Lookup(mmid, v); }
	//! \brief Is the mmid in the hidden list?
	BOOL IsHidden(MMID mmid) const
		{ int v; return m_hidden.Lookup(mmid, v); }

public:
	/** \brief Set executor address
	 * \param [in] pszAddress A string that contains the address of the executor. This can be the IP, or valid DNS name.
	 * \param [in] nPort Service port number.
	 */
	void SetExecAddress(LPCSTR pszAddress, unsigned short nPort);
	/** \brief Set quote server address
	 * \param [in] pszAddress A string that contains the address of the quote server. This can be the IP, or valid DNS name.
	 * \param [in] nPort Service port number.
	 */
	void SetQuoteAddress(LPCSTR pszAddress, unsigned short nPort);
	/** \brief Set level 2 server address
	 * \param [in] pszAddress A string that contains the address of the level 2 server. This can be the IP, or valid DNS name.
	 * \param [in] nPort Service port number.
	 */
	void SetLevel2Address(LPCSTR pszAddress, unsigned short nPort);
	/** \brief Set chart server address
	 * \param [in] pszAddress A string that contains the address of the chart server. This can be the IP, or valid DNS name.
	 * \param [in] nPort Service port number.
	 */
	void SetChartAddress(LPCSTR pszAddress, unsigned short nPort);

public:
	//! \brief Calculate Level price for the inputed price and side
	double CalcLevelPrice(double dblPrice, char chSide) const;

public:
	//! \brief Get the display size of the order size
	int GetReserveShares(MMID method, int nOrderShares) const;
	//! \brief Set the reserve size of method
	int SetReserveShares(MMID method, int nReserveShares);
	//! \brief Get reserve status of method
	BOOL GetReserveShow(MMID method) const;
	//! \brief Set reserve status of method.
	BOOL SetReserveShow(MMID method, BOOL bReserveShow);
public:
	/** \brief Set option quote server address
	 * \param [in] pszAddress A string that contains the address of the quote server. This can be the IP, or valid DNS name.
	 * \param [in] nPort Service port number.
	 */
	void SetOptionQuoteAddress(LPCSTR pszAddress, unsigned short nPort);
	/** \brief Set option level 2 server address
	 * \param [in] pszAddress A string that contains the address of the level 2 server. This can be the IP, or valid DNS name.
	 * \param [in] nPort Service port number.
	 */
	void SetOptionLevel2Address(LPCSTR pszAddress, unsigned short nPort);
	/** \brief Set chart server address
	 * \param [in] pszAddress A string that contains the address of the chart server. This can be the IP, or valid DNS name.
	 * \param [in] nPort Service port number.
	 */
	void SetOptionChartAddress(LPCSTR pszAddress, unsigned short nPort);
};

#endif//__GTSETTING_H__

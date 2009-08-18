/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GTPrint.h: interface for the GTPrint class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GTPrint.h
	\brief interface for the GTPrint, GTPrints class.
 */

#if !defined(__GTPRINT_H__)
#define __GTPRINT_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "GTAPI_API.h"
#include "..\Inc32\GTPrint32.h"

/*! \ingroup cpp
*/
/*! \class GTPrint
    \brief Print information. The same as GTPrint32 or tagGTPrint32.

	\copydoc tagGTPrint32
*/
typedef GTPrint32 GTPrint;

/*
struct GTPrint : public GTPrint32
{
};
*/

/*! \ingroup cpp
*/
/*! \class GTPrints
	\brief The container for prints.
 *	
	All the prints are stored in this container. 

	Two methods are of the interests of the user. GetCount() gives how many records inside, Get() can help one to retrive one record.
 */
class GTAPI_API GTPrints
{
protected:
	LPVOID	m_data;				//!< Actual data stored.
	BOOL	m_bTransferring;	//!< Is in transferring?

public:
	int m_nMinPrints;			//!< Minimun number of records to clear when the data needs to be clear.
	int m_nMaxPrints;			//!< Maximun number of records in the storage.

public:
	double m_dblBidPrice;		//!< [internal used] bid price, used to decide the print color.
	double m_dblBidPriceInside;	//!< [internal used] bid price inside, used to decide the print color.
	double m_dblAskPrice;		//!< [internal used] ask price, used to decide the print color.
	double m_dblAskPriceInside;	//!< [internal used] ask price inside, used to decide the print color.

public:
	GTPrints();					//!< [internal used] Constructor
	virtual ~GTPrints();		//!< [internal used] Destructor

	virtual void ResetContent();	//!< [internal used] Reset the content.
	virtual int Dump(FILE *fp, int nLevel) const;	//!< [internal used] Dump the contents.

public:
	int Refresh();			//!< [internal used] Refresh the content.
	int Display();			//!< [internal used] Start to display the content. (Stop the transferring)

	int SetMinMax(int nMin, int nMax);	//!< [internal used] Set the max and min
	int Add(const GTPrint &print);		//!< [internal used] Add a print

	/**
	 * \brief Get how many record in the storage.
	 * \return The number of prints stored
	 */
	int GetCount() const;
	/**
	 * \brief Get one record.
	 * \param [in] nIndex The index of the record to get.
	 * \return A copy of the record.
	 * \remark Used to get one record. Here the \c nIndex is the index for the item. The newset print will be 0, the the larger the index, the older the print.

		\par
		This can be used with GetCount to iterate all the prints. Notice here the return is a copy of the print. 

		\note
		Passing a negative index or a index greater than the value returned by GetCount will result in a failed assertion.
	 */
	GTPrint Get(int nIndex) const;

public:
	void SetBidPrice(double dblBidPrice);	//!< [internal used] set bid price, used to decide the print color.
	void SetAskPrice(double dblAskPrice);	//!< [internal used] set ask price, used to decide the print color.

	int SetPrintColor(GTPrint &print);		//!< [internal used] set the print color manually.
};

#endif // !defined(__GTPRINT_H__)

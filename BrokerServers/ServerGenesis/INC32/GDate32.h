/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GDate32.h
	\brief interface for the GDate32 class.
 */

#if !defined(__GDATE32_H__)
#define __GDATE32_H__

#pragma pack(1)
/*! \ingroup c
	\ingroup cpp
 */
/*! \struct GDate32
	\brief Date formated in GTAPI. Alias of tagGDate32.

	\copydoc tagGDate32
*/
/*! \typedef typedef tagGDate32 GDate32
*/
/*! \struct tagGDate32
	\brief Date formated in GTAPI.

	This describes a date. dwDate is a combination of chDay, chMonth, nYear. This class defines the date 
	struct used in our system. A graphical representation of this class is: <br>
	|----------------------------dwDate(UInt32)-----------------------------|<br>
	|-----chDay------|------chMonth------|-----nYear(H)-----|----nYear(L)---|<br>
	0-----------------------------------------------------------------------32<br>

	You can visit it by dwDate as a whole, or, by chDay, chMonth and nYear respectively.

	\sa GDate0
*/
#pragma warning (push, 4)
#pragma warning (disable: 4201) // nonstandard extension used : nameless struct/union

typedef struct tagGDate32
{
	union
	{
		struct
		{
			unsigned char chDay;
			unsigned char chMonth;
			unsigned short nYear;
		};

		unsigned long dwDate;
	};
}GDate32;
#pragma pack()
#pragma warning (pop)

#define MAKEWDATE(y, m, d)	((unsigned long)(\
							((unsigned char)(d)) <<  0 |\
							((unsigned char)(m)) <<  8 |\
							((unsigned     )(y)) << 16))

#endif//__GDATE32_H__

/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/
/*! \file GTime32.h
	\brief interface for the GTime32 class.
 */

#ifndef __GTIME32_H__
#define __GTIME32_H__

#pragma pack(1)

/*! \ingroup c
	\ingroup cpp
*/
/*! \struct GTime32
	\brief Time formated in GTAPI. Alias of tagGTime32.

	\copydoc tagGTime32
*/
/*! \typedef typedef tagGTime32 GTime32
*/
/*! \struct tagGTime32
	\brief Time formated in GTAPI.

	This describes a time. dwTime is a combination of chSec100, chSec, chMin, chHour. This class defines the time 
	struct used in our system. A graphical representation of this class is: <br>
	|----------------------------dwTime(UInt32)-----------------------------|<br>
	|-----chHour------|------chMin------|------chSec------|-----chSec100----|<br>

	You can visit it by dwTime as a whole, or, by chHour, chMin and chSec respectively. For example, for the opening 
	time of the market 9:30:00, it GTime32 representation is dwTime=9300000. 

	\sa GTime0
*/
#pragma warning (push, 4)
#pragma warning (disable: 4201) // nonstandard extension used : nameless struct/union

typedef struct tagGTime32
{
	union
	{
		struct
		{
			unsigned char chSec100;
			unsigned char chSec;
			unsigned char chMin;
			unsigned char chHour;
		};

		unsigned long dwTime;
	};
}GTime32;
#pragma warning (pop)


#pragma pack()

/*! \def MAKEWTIME
  \brief Make a GTime32.

  \remark
  \a h: Hour\n
  \a m: Minute\n
  \a s: Second\n
*/
#ifndef MAKEWTIME
#define MAKEWTIME(h, m, s)	((unsigned long)(\
							((unsigned char)(0)) <<  0 |\
							((unsigned char)(s)) <<  8 |\
							((unsigned char)(m)) << 16 |\
							((unsigned char)(h)) << 24))
#endif

#ifdef __cplusplus
extern "C"
{
#endif//__cplusplus
	int WINAPI GetGTime32Seconds(const GTime32 *gt);
	int WINAPI DiffGTime32Seconds(const GTime32 *gt1, const GTime32 *gt2);
	int WINAPI GetGTime32Minutes(const GTime32 *gt);
	int WINAPI DiffGTime32Minutes(const GTime32 *gt1, const GTime32 *gt2);
#ifdef __cplusplus
}
#endif//__cplusplus

#endif//__GTIME32_H__

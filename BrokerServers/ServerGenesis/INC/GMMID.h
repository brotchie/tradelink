/****************************************************************************
 *                                                                          *
 *    All codes are property of Genesis Securities, LLC.                    *
 *    Any use without prior written permission is illegal.                  *
 *    This material cannot be distributed without prior written permission. *
 *                                                                          *
 ****************************************************************************/

// GMMID.h: interface for the GMMID class.
//
//////////////////////////////////////////////////////////////////////
/*! \file GMMID.h
    \brief interface for the GMMID, GMMID_ class.
 */

#if !defined(__GMMID_H__)
#define __GMMID_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "../Inc32/GTConst.h"
#include "../Inc32/MMID.h"

/** \class GMMID_
 *	\brief MMID class base.
 *	
 *	This class manages the operations to MMID. MMID can take two forms, string and unsigned long (MMID).
 *	This class makes the transforms between these forms easy and plain.
 *  \sa GMMID
 */
class GMMID_
{
public:
	union
	{
		MMID dwMMID;
		char szMMID[sizeof(MMID) + 1];
	};

public:
	void Init()
	{
		szMMID[sizeof(MMID)] = 0;
	}

	GMMID_ &operator=(MMID mmid)
	{
		dwMMID = mmid;
		return *this;
	}

	GMMID_ &operator=(LPCSTR mmid)
	{
		dwMMID = makemmid(mmid);
		return *this;
	}

	BOOL operator==(GMMID_ mmid) const
	{
		return dwMMID == mmid.dwMMID;
	}
	BOOL operator==(MMID mmid) const
	{
		return dwMMID == mmid;
	}
	BOOL operator==(LPCSTR mmid) const
	{
		return dwMMID == makemmid(mmid);
	}

	BOOL operator!=(GMMID_ mmid) const
	{
		return dwMMID != mmid.dwMMID;
	}
	BOOL operator!=(MMID mmid) const
	{
		return dwMMID != mmid;
	}
	BOOL operator!=(LPCSTR mmid) const
	{
		return dwMMID != makemmid(mmid);
	}

	operator MMID() const
	{
		return dwMMID;
	}

	operator LPCSTR() const
	{
		return szMMID;
	}
};

/** \class GMMID
 *	\brief MMID class.
 *	
 *	This class manages the operations to MMID. MMID can take two forms, string and unsigned long (MMID).
 *	This class makes the transforms between these forms easy and plain. Also operations like comparisons 
	assignments are privided in this class. 

    <div class="tablediv">
        <table class="dtTABLE" cellspacing="0">
          <tr valign="top">
            <th width="50%">Function</th>
            <th width="50%">Example</th>
          </tr>
          <tr valign="top">
            <td width="50%">Constructor</td>
            <td width="50%">GMMID isld("ISLD"); <br>GMMID brut(MMID_BRUT);
			</td>
			</td>
          </tr>
          <tr valign="top">
            <td width="50%">Assignment</td>
            <td width="50%">GMMID isld = "ISLD"; <br> GMMID brut = MMID_BRUT;
			</td>
          </tr>
          <tr valign="top">
            <td width="50%">Comparison</td>
            <td width="50%">isld == "ISLD";(true)<br> isld != MMID_ISLD;(false)<br> isld==brut;(false)
			</td>
          </tr>
          <tr valign="top">
            <td width="50%">Conversion</td>
            <td width="50%">order.place = isld; <br> printf("%s", isld);
			</td>
          </tr>
		</table>
	</div>

	\par Example, one has mmid "ISLD". To use it in all environment, one can do:
	\code
		GMMID isld = "ISLD";

		//Use isld in any place requires mmid.
	\endcode

	\sa GMMID_
 */
class GMMID : public GMMID_
{
public:
	GMMID()
	{
		szMMID[sizeof(MMID)] = 0;
	}

	GMMID(MMID mmid)
	{
		dwMMID = mmid;
		szMMID[sizeof(MMID)] = 0;
	}

	GMMID(LPCSTR mmid)
	{
		dwMMID = makemmid(mmid);
		szMMID[sizeof(MMID)] = 0;
	}

	GMMID_ &operator=(LPCSTR mmid)
	{
		dwMMID = makemmid(mmid);
		return *this;
	}
};

inline BOOL SetDlgItemMMID(HWND hDlg, UINT uID, MMID mmid)
{
	GMMID gmmid;
	gmmid = mmid;

	return ::SetDlgItemText(hDlg, uID, gmmid.szMMID);
}

inline BOOL SetDlgItemMMID(HWND hDlg, UINT uID, GMMID gmmid)
{
	return ::SetDlgItemText(hDlg, uID, gmmid.szMMID);
}

inline MMID TransToECN(MMID book)
{
	switch(book)
	{
	case MMID_ISB:
	case MMID_ISLD:
	case MMID_INET:
		return MMID_ISLD;
	case MMID_BRB:
	case MMID_BRUT:
		return MMID_BRUT;
	case MMID_RDB:
	case MMID_REDI:
		return MMID_REDI;
	case MMID_ARB:
	case MMID_ARCA:
		return MMID_ARCA;
	case MMID_INB:
	case MMID_INCA:
		return MMID_INCA;
	case MMID_GNB:
	case MMID_GNET:
		return MMID_GNET;
/*	case MMID_OES:
		return MMID_OES;
	case MMID_NYFX:
		return MMID_NYFX;*/
	}

	return MMID_UNKNOWN;
}

inline MMID TransToBook(MMID ecn)
{
	switch(ecn)
	{
	case MMID_ISB:
	case MMID_ISLD:
	case MMID_INET:
		return MMID_ISB;
	case MMID_BRB:
	case MMID_BRUT:
		return MMID_BRB;
	case MMID_RDB:
	case MMID_REDI:
		return MMID_RDB;
	case MMID_ARB:
	case MMID_ARCA:
		return MMID_ARB;
	case MMID_INB:
	case MMID_INCA:
		return MMID_INB;
	case MMID_GNB:
	case MMID_GNET:
		return MMID_GNB;
/*	case MMID_OES:	 // add by M.Y. 1/15/2005
		return MMID_OES;
	case MMID_NYFX:	 // add by M.Y. 1/15/2005
		return MMID_NYFX;*/

	}

	return MMID_UNKNOWN;
}

//Jason
inline MMID	GetMethod(const char * szMethod)
{
	if(strcmp(szMethod, METHOD_EXTERNAL_NAME_ISLAND) == 0)
		return METHOD_ISLD;
	// added by Kevin:
	else if(strcmp(szMethod, "INET") == 0)
		return METHOD_ISLD;
	else if(strcmp(szMethod, "SOES") == 0)
		return METHOD_SOES;
	else if(strcmp(szMethod, "SNet") == 0)
		return METHOD_SNET;
	else if(strcmp(szMethod, "SWST") == 0)
#ifdef USE_SWST
		return METHOD_SWST;
#else
		return METHOD_HELF;
#endif
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_HELF) == 0)
		return METHOD_HELF;
	else if(strcmp(szMethod, "BRUT") == 0)
		return METHOD_BRUT;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_BRTX) == 0)
		return METHOD_BRTX;
	else if(strcmp(szMethod, "REDI") == 0)
		return METHOD_REDI;
	else if(strcmp(szMethod, "ARCA") == 0)
		return METHOD_ARCA;
	else if(strcmp(szMethod, "INCA") == 0)
		return METHOD_INCA;
	else if(strcmp(szMethod, "GNET") == 0)
		return METHOD_GNET;
#ifdef USE_LSPD
	else if(strcmp(szMethod, "LSPD") == 0)
		return METHOD_LSPD;
#endif

	else if(strcmp(szMethod, "SLIP") == 0)
		return METHOD_SLIP;
	else if(strcmp(szMethod, "ATTN") == 0)
		return METHOD_ATTN;
	else if(strcmp(szMethod, "DATA") == 0)
		return METHOD_DATA;
	else if(strcmp(szMethod, "TRAC") == 0)
		return METHOD_TRAC;
	else if(strcmp(szMethod, "ERCO") == 0)
		return METHOD_ERCO;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_OES) == 0) // added by M.Y. 1/15/2005
		return METHOD_OES;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_OESX) == 0) // added by M.Y. 1/15/2005
		return METHOD_OESX;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_NYF) == 0) // added by M.Y. 1/15/2005
		return METHOD_NYF;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_NYFX) == 0) // added by M.Y. 1/15/2005
		return METHOD_NYFX;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_NITE) == 0) // added by M.Y. 1/15/2005
		return METHOD_NITE;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_BATS) == 0)
		return METHOD_BATS;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_AUTO) == 0)
		return METHOD_AUTO;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_CDRG) == 0)
		return METHOD_CDRG;	
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_CRSS) == 0)
		return METHOD_CRSS;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_NSXX) == 0)
		return METHOD_NSXX;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_GETCO) == 0)
		return METHOD_GETC;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_ISEQ) == 0)
		return METHOD_ISEQ;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_AUTP) == 0)
		return METHOD_AUTP;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_EDGA) == 0)
		return METHOD_EDGA;
	else if(strcmp(szMethod, METHOD_EXTERNAL_NAME_EDGX) == 0)
		return METHOD_EDGX;

//changed by Jason Ruan
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_BELZ) == 0)
		return METHOD_BELZ;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_PFTD) == 0)
		return METHOD_PFTD;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_BLZX) == 0)
		return METHOD_BLZX;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_BLZV) == 0)
		return METHOD_BLZV;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_MLYN) == 0)
		return METHOD_MLYN;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_MLNX) == 0)
		return METHOD_MLNX;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_ISE) == 0)
		return METHOD_ISEO;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_PPLN) == 0)
		return METHOD_PPLN;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_SENG) == 0)
		return METHOD_SENG;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_SEN2) == 0)
		return METHOD_SEN2;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_ITS) == 0)
		return METHOD_ITS;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_ITSX) == 0)
		return METHOD_ITSX;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_LSTK) == 0)
		return METHOD_LSTK;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_PFTD) == 0)
		return METHOD_PFTD;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_SLIP) == 0)
		return METHOD_SLIP;
	
//end change
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_SMART) == 0)
		return METHOD_SMRT;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_MLOP) == 0)
		return METHOD_MLOP;
	else if (strcmp(szMethod, METHOD_EXTERNAL_NAME_SMART1) == 0)
		return METHOD_SM01;

	else
		return METHOD_UNKNOWN;
}

inline const char * GetMethodExternalName(MMID method)
{
	switch(method)
	{
	case METHOD_ISLD:
		return METHOD_EXTERNAL_NAME_ISLAND;
		//return METHOD_EXTERNAL_NAME_INET; // kevin

	//case METHOD_INET:
	//	return METHOD_EXTERNAL_NAME_INET;  //kevin: METHOD_INET == METHOD_ISLD now

	case METHOD_ARCA:
		return METHOD_EXTERNAL_NAME_ARCA;
	
	case METHOD_INCA:
		return METHOD_EXTERNAL_NAME_INCA;

	case METHOD_TRAC:
		return METHOD_EXTERNAL_NAME_TRAC;

	case METHOD_SOES:
		return METHOD_EXTERNAL_NAME_SOES;

	case METHOD_BELZ:
		return METHOD_EXTERNAL_NAME_BELZ;

	case METHOD_PFTD:
		return METHOD_EXTERNAL_NAME_PFTD;

	case METHOD_BLZX:
		return METHOD_EXTERNAL_NAME_BLZX;

	case METHOD_BLZV:
		return METHOD_EXTERNAL_NAME_BLZV;

	case METHOD_MLYN:
		return METHOD_EXTERNAL_NAME_MLYN;

	case METHOD_MLNX:
		return METHOD_EXTERNAL_NAME_MLNX;
	
	case METHOD_SIZE:
		return METHOD_EXTERNAL_NAME_SIZE;

	case METHOD_HELF:
		return METHOD_EXTERNAL_NAME_HELF;

	case METHOD_SNET:
		return METHOD_EXTERNAL_NAME_SNET;

	case METHOD_BULT:
		return METHOD_EXTERNAL_NAME_BULT;

	case METHOD_LSTK:
		return METHOD_EXTERNAL_NAME_LSTK;

	case METHOD_INST:
		return METHOD_EXTERNAL_NAME_INST;
	
	case METHOD_BRUT:
		return METHOD_EXTERNAL_NAME_BRUT;

	case METHOD_BRTX:
		return METHOD_EXTERNAL_NAME_BRTX;

	case METHOD_REDI:
		return METHOD_EXTERNAL_NAME_REDI;

	case METHOD_GNET:
		return METHOD_EXTERNAL_NAME_GNET;

	case METHOD_LSPD:
		return METHOD_EXTERNAL_NAME_LSPD;

	case METHOD_ATTN:
		return METHOD_EXTERNAL_NAME_ATTN;

	case METHOD_ERCO:
		return METHOD_EXTERNAL_NAME_ERCO;

	case METHOD_DATA:
		return METHOD_EXTERNAL_NAME_DATA;

	case METHOD_PPLN:
		return METHOD_EXTERNAL_NAME_PPLN;

	case METHOD_SENG:
		return METHOD_EXTERNAL_NAME_SENG;

	case METHOD_SEN2:
		return METHOD_EXTERNAL_NAME_SEN2;

	case METHOD_ISEO:
		return METHOD_EXTERNAL_NAME_ISE;

	case METHOD_ITS:
		return METHOD_EXTERNAL_NAME_ITS;

	case METHOD_ITSX:
		return METHOD_EXTERNAL_NAME_ITSX;

	case METHOD_SMRT:
		return METHOD_EXTERNAL_NAME_SMART;
	
	case METHOD_OES:  // add by M.Y. 1/15/2005 for OES FIX
		return METHOD_EXTERNAL_NAME_OES; 

	case METHOD_OESX:  // add by M.Y. 1/15/2005 for OES FIX
		return METHOD_EXTERNAL_NAME_OESX; 
	
	case METHOD_NYF:  // add by M.Y. 1/15/2005 for NY FIX
		return METHOD_EXTERNAL_NAME_NYF; 

	case METHOD_NYFX:  // add by M.Y. 1/15/2005 for NY FIX
		return METHOD_EXTERNAL_NAME_NYFX;

	case METHOD_NITE:
		return METHOD_EXTERNAL_NAME_NITE;

	case METHOD_BATS:
		return METHOD_EXTERNAL_NAME_BATS;

	case METHOD_AUTO:
		return METHOD_EXTERNAL_NAME_AUTO;

	case METHOD_CDRG:
		return METHOD_EXTERNAL_NAME_CDRG;

	case METHOD_CRSS:
		return METHOD_EXTERNAL_NAME_CRSS;

	case METHOD_ISEQ:
		return METHOD_EXTERNAL_NAME_ISEQ;

	case METHOD_AUTP:
		return METHOD_EXTERNAL_NAME_AUTP;

	case METHOD_EDGA:
		return METHOD_EXTERNAL_NAME_EDGA;

	case METHOD_EDGX:
		return METHOD_EXTERNAL_NAME_EDGX;

	case METHOD_SLIP:
		return METHOD_EXTERNAL_NAME_SLIP;

	case METHOD_MLOP:
		return METHOD_EXTERNAL_NAME_MLOP;

	case METHOD_SM01:
		return METHOD_EXTERNAL_NAME_SMART1;

	default:
		return "Unknown";
	}
}

#ifdef __AFXWIN_H__
void DDX_Text(CDataExchange* pDX, UINT nID, GMMID &gmmid);
#endif//__AFXWIN_H__

#endif // !defined(__GMMID_H__)

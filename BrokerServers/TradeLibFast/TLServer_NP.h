#pragma once
#include "TradeLibFast.h"
#include <vector>
using namespace std;

namespace TradeLibFast
{

	class AFX_EXT_CLASS  TLServer_NP 
	{
		//DECLARE_DYNAMIC(TLServer_NP)

	public :
		TLServer_NP(void);
		~TLServer_NP();

		void newTick(TLTick k);

	protected:
		int newSecurity(TLSecurity sec);
		int symidx(CString symbol);
		vector<TLSecurity> _secs;
		vector<HANDLE> _pipes;

	};
}
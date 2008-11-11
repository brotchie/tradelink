#pragma once
#include "TradeLibFast.h"
#include <vector>
#include <map>
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

		map<CString,int> _pidx;
		vector<HANDLE> _pipes;

	};
}
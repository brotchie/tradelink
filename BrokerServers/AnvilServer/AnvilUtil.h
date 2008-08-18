#pragma once
#include "BusinessApi.h"

long MoneyToPacked(Money m);
CString TIFName(unsigned int tifid);
unsigned int TIFId(CString name);
CString ExchangeName(unsigned int exchangeid);
#pragma once
CString ConvertReceivedDataToString(CByteArray & data);
BOOL ConvertStringToSendData(const CString & s, CByteArray & msg);

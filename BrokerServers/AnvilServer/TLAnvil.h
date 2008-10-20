#pragma once


LPARAM ServiceMsg(const int gotType,CString gotMsg);
void TLUnload();
void AllClients(std::vector <CString> &subscriberids);
void Subscribers(CString stock,std::vector <CString> &subscriberids);
void TLKillDead(int deathInseconds);
void gsplit(CString msg, CString del, std::vector<CString>& rec);




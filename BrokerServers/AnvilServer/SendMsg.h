#ifndef _SENDMSG_H
#define _SENDMSG_H

#define WINDNAME "TradeLinkServer"
#define EXTMISSING "Unable to find TradeLink.  Please verify Anvil is running, aTradeLink.dll extension is loaded in Anvil extensions and TradeLink simulation/live setting matches Anvil's."

LRESULT SendMsg(int type,LPCTSTR msg,LPCTSTR windname = WINDNAME);
double unpack(long i);

#endif _SENDMSG_H

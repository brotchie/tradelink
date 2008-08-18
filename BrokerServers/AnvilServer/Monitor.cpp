
#include "Stdafx.h"
#include "Monitor.h"
#include "BusinessApi.h"
#include "Messages.h"
#include "TradeLinkServer.h"
#include "TradeLink.h"
#include "AnvilUtil.h"
#include "SendMsg.h"
using namespace TradeLinkServer;

Monitor::Monitor()
{
	void* iterator = B_CreateAccountIterator();
	B_StartIteration(iterator);
	Observable* acct;
	while (acct = B_GetNextAccount(iterator)) // loop through every available account
	{
		acct->Add(this); // add this object to account as an observer
		accounts.push_back(acct); // save the account
	}
	B_DestroyIterator(iterator);
}


typedef std::vector <int> filllist;
std::vector<filllist> accountfills;
std::vector<CString> accounts;
int AccountId(CString acct) { for (size_t i = 0; i<accounts.size(); i++) if (accounts[i]==acct) return i; return -1; }
bool hasFillID(CString acct,int id)
{
	int aid = AccountId(acct);
	if (aid==-1)
	{
		accounts.push_back(acct);
		filllist f;
		f.push_back(id);
		accountfills.push_back(f);
		return false;
	}
	filllist existing = accountfills[aid];
	for (size_t i= 0; i<existing.size(); i++)
		if (existing[i]==id) return true;
	existing.push_back(id);
	accountfills[aid] = existing;
	return false;
}
std::vector<Order*> ordercache;
bool hasOrder(unsigned int  TLid)
{
	return (TLid>=0) && (TLid<ordercache.size());
}

unsigned int AnvilId(unsigned int TLOrderId)
{
	if (!hasOrder(TLOrderId)) return -1;
	Order* o = ordercache[TLOrderId];
	return o->GetId();
}


Monitor::~Monitor()
{
	ordercache.clear();
	accountfills.clear();
}

unsigned int Monitor::cacheOrder(Order* o)
{
	for (unsigned int i = 0; i<ordercache.size(); i++)
		if (ordercache[i]==o) return i; // found order so we return it's index
	ordercache.push_back(o);
	return ordercache.size()-1; // didn't find order so we added it and returned index
}

void Monitor::Process(const Message* message, Observable* from, const Message* additionalInfo)
{
    switch(message->GetType())
    {
        case M_POOL_EXECUTION:
        if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_EXECUTION)
        {
			            MsgPoolExecution* msg = (MsgPoolExecution*)message;//to get the structure, just cast Message* to  MsgPoolExecution* (not used here)

//This is additional info structure prepared by Business.dll. It contains updated objects Position, Order Execution (look in BusinessApi.h).
//You can access objects' fields, but it is not recommended to change them (The fields are protected and you should not play any tricks to modify the fields. It will cause unpredictable results)
            AIMsgExecution* info = (AIMsgExecution*)additionalInfo;
            Order* order = info->m_order;
            const Position* position = info->m_position;
            const Execution* exec = info->m_execution;
			if ((order==NULL) || (position==NULL) || (exec==NULL)) return; // don't process null orders

			unsigned int thisid = order->GetId();
			CString ac = CString(B_GetAccountName(position->GetAccount()));
			if (!hasFillID(ac,thisid)) // don't send same notification twice
			{
				// build the serialized trade object
				CTime ct(msg->x_Time);
				int xd = (ct.GetYear()*10000)+(ct.GetMonth()*100)+ct.GetDay();
				int xt = (ct.GetHour()*100)+ct.GetMinute();
				TradeLinkServer::TLTrade fill;
				fill.id = thisid;
				fill.xsec = ct.GetSecond();
				fill.xtime = xt;
				fill.xdate = xd;
				fill.side = (order->GetSide()=='B');
				fill.comment = CString(order->GetUserDescription());
				fill.symbol = CString(msg->x_Symbol);
				fill.xprice = (double)msg->x_ExecutionPrice/1024;
				fill.xsize= msg->x_NumberOfShares;
				fill.exchange = CString(ExchangeName((long)msg->x_executionId));
				fill.account = CString(B_GetAccountName(position->GetAccount()));

				std::vector <CString> subname;
				AllClients(subname);
				for (size_t i = 0; i< subname.size(); i++) 
					SendMsg(EXECUTENOTIFY,fill.Serialize(),subname[i]);
			} // hasfillend
		} // has additional info end
		break;
        //case M_REQ_NEW_ORDER://New Order created and sent out by Business.dll (thriugh function call B_SendOrder).
        case M_POOL_ASSIGN_ORDER_ID://Original order sent has a unigue generated id. The server sends this message to notify you that the order was assigned a new id different from the original. Both ids are part of this notification structure. This message can come 1 or 2 times.
        case M_POOL_UPDATE_ORDER:// Order status is modified
        if(additionalInfo != NULL && additionalInfo->GetType() == M_AI_ORDER)
        {

            AIMsgOrder* info = (AIMsgOrder*)additionalInfo;
            Order* order = info->m_order;
            const Position* position = info->m_position;

			if ((order==NULL) || (position==NULL) || (info==NULL)) return; // don't process null orders

			if (order->isDead()) return; // don't notify on dead orders

			unsigned int max = ordercache.size();
			unsigned int index = cacheOrder(order);
			if (index!=max) // if index isn't at the end, we've already notified for order
				return;


			CTime ct = CTime::GetCurrentTime();
			TLOrder o;
			o.id = index;
			o.price = order->GetOrderPrice().toDouble();
			o.sec = ct.GetSecond();
			o.stop = order->GetStopPrice()->toDouble();
			o.time = (ct.GetHour()*100)+ct.GetMinute();
			o.date = (ct.GetYear()*10000)+(ct.GetMonth()*100)+ct.GetDay();
			o.size = order->GetSize();
			o.side = order->GetSide()=='B';
			o.comment = order->GetUserDescription();
			o.TIF = TIFName(order->GetTimeInForce());
			o.account = CString(B_GetAccountName(order->GetAccount()));
			o.symbol = CString(order->GetSymbol());
			std::vector <CString> subname;
			AllClients(subname);
			for (size_t i = 0; i< subname.size(); i++) 
				SendMsg(ORDERNOTIFY,o.Serialize(),subname[i]);
			break;
		} // has addt info / caseend
		case M_REQ_CANCEL_ORDER:
        {
			AIMsgOrder* info = (AIMsgOrder*)additionalInfo;
            Order* order = info->m_order;
			unsigned int anvilid = order->GetId();
			unsigned int id = cacheOrder(order);
			CString msg;
			msg.Format("%u",id);
			std::vector<CString> clients;
			AllClients(clients);
			for (size_t i = 0; i<clients.size(); i++)
				SendMsg(ORDERCANCELRESPONSE,msg,clients[i]);
			break;

        }
        break;
	} // switchend
}

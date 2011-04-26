using System;
[assembly: CLSCompliant(true)]

namespace TradeLink.API
{
    public delegate void ResponseStringDel(int idx, string data);
    public delegate void BoolDelegate(bool v);
    public delegate void TextIdxDelegate(string txt, int idx);
    public delegate void SymDelegate(string sym);
    public delegate void Int32Delegate(int val);
    public delegate void IntDelegate(int val);
    public delegate void LongDelegate(long val);
    public delegate void StringParamDelegate(string param);
    public delegate void TickDelegate(Tick t);
    public delegate void FillDelegate(Trade t);
    public delegate void OrderDelegate(Order o);
    public delegate void OrderSourceDelegate(Order o, int source);
    public delegate void LongSourceDelegate(long val, int source);
    public delegate long OrderDelegateStatus(Order o);
    public delegate void Int64Delegate(Int64 number);
    public delegate void SecurityDelegate(Security sec);
    public delegate void StringDecimalDelegate(string txt, decimal val);
    public delegate void DecimalDelgate(decimal val);
    public delegate void MessageTypesMsgDelegate(MessageTypes[] messages);
    public delegate void DebugDelegate(string msg);
    public delegate void SymbolRegisterDel(string client, string symbols);
    public delegate void ObjectArrayDelegate(object[] parameters);
    public delegate void PositionDelegate(Position pos);
    public delegate void DebugFullDelegate(Debug debug);
    public delegate decimal DecimalStringDelegate(string s);
    public delegate int IntStringDelegate(string s);
    public delegate string StringDelegate();
    public delegate Position[] PositionArrayDelegate(string account);
    public delegate void OrderCancelDelegate(string sym, bool side, long id);
    public delegate MessageTypes[] MessageArrayDelegate();
    public delegate void MessageArrayDel(MessageTypes[] messages);
    public delegate long UnknownMessageDelegate(MessageTypes t, string msg);
    public delegate long UnknownMessageDelegateSource(MessageTypes t, string msg,string source);
    public delegate void SymBarIntervalDelegate(string symbol, int interval);
    public delegate void ImbalanceDelegate(Imbalance imb);
    public delegate void VoidDelegate();
    public delegate void MessageDelegate(MessageTypes type, long source, long dest, long msgid, string request,ref string response);
    public delegate void MessageFullDelegate(GenericMessage m);
    public delegate void BasketDelegate(Basket b, int id);
    public delegate void BarListDelegate(BarList b);
    public delegate void ChartLabelDelegate(decimal price, int time, string label, System.Drawing.Color c);
    public delegate void ProviderDelegate(Providers p);
    public delegate void TicketDelegate(string space, string user, string password, string summary, string description, Priority pri, TicketStatus stat);
}

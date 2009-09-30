using System;

namespace TradeLink.API
{
    public delegate void StringParamDelegate(string param);
    public delegate void TickDelegate(Tick t);
    public delegate void FillDelegate(Trade t);
    public delegate void OrderDelegate(Order o);
    public delegate void IntDelegate(Int64 number);
    public delegate void UIntDelegate(UInt32 number);
    public delegate void SecurityDelegate(Security sec);
    public delegate void MessageTypesMsgDelegate(MessageTypes[] messages);
    public delegate void DebugDelegate(string msg);
    public delegate void ObjectArrayDelegate(object[] parameters);
    public delegate void PositionDelegate(Position pos);
    public delegate void DebugFullDelegate(Debug debug);
    public delegate decimal DecimalStringDelegate(string s);
    public delegate int IntStringDelegate(string s);
    public delegate string StringDelegate();
    public delegate Position[] PositionArrayDelegate(string account);
    public delegate void OrderCancelDelegate(string sym, bool side, uint id);
    public delegate MessageTypes[] MessageArrayDelegate();
    public delegate long UnknownMessageDelegate(MessageTypes t, string msg);
    public delegate void SymBarIntervalDelegate(string symbol, int interval);
    public delegate void ImbalanceDelegate(Imbalance imb);
    public delegate void VoidDelegate();
    public delegate void MessageDelegate(MessageTypes type, uint id, string data);
    public delegate void BasketDelegate(Basket b, int id);
    public delegate void BarListDelegate(BarList b);
    public delegate void ChartLabelDelegate(decimal price, int bar, string label);
}

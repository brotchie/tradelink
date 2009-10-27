using System;

namespace TradeLink.API
{
    public interface Order
    {
        string symbol { get; set; }
        string TIF { get; set; }
        bool side { get; set; }
        bool Side { get; }
        decimal price { get; set; }
        decimal stopp { get; set; }
        decimal trail { get; set; }
        string comment { get; set; }
        string ex { get; set; }
        string Exchange { get; set; }
        int size { get; set; }
        int UnsignedSize { get; }
        int date { get; set; }
        int time { get; set; }
        bool isFilled { get; }
        bool isLimit { get; }
        bool isStop { get; }
        bool isTrail { get; }
        bool isMarket { get; }
        SecurityType Security { get; set; }
        CurrencyType Currency { get; set; }
        Security Sec { get; }
        string Account { get; set; }
        string LocalSymbol { get; set; }
        uint id { get; set; }
        bool Fill(Order o);
        bool Fill(Tick t);
        bool Fill(Tick t, bool fillOPG);
        bool isValid { get; }
        int VirtualOwner { get; set; }
    }

    public class InvalidOrder : Exception { }
}

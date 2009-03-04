using System;

namespace TradeLink.API
{
    public interface Trade
    {
        uint id { get; set; }
        string symbol { get; set; }
        int xsize { get; set; }
        decimal xprice { get; set; }
        int xtime { get; set; }
        int xdate { get; set; }
        string comment { get; set; }
        bool side { get; set; }
        string Account { get; set; }
        string LocalSymbol { get; set; }
        string ex { get; set; }
        SecurityType Security { get; set; }
        Security Sec { get; }
        CurrencyType Currency { get; set; }
        int UnsignedSize { get; }
        bool isValid { get; }
        bool isFilled { get; }

    }

    public class InvalidTrade : Exception { }
}
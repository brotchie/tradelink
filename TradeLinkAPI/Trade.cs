using System;

namespace TradeLink.API
{
    public interface Trade
    {
        /// <summary>
        /// id of trade
        /// </summary>
        long id { get; set; }
        /// <summary>
        /// symbol traded
        /// </summary>
        string symbol { get; set; }
        /// <summary>
        /// executed size
        /// </summary>
        int xsize { get; set; }
        /// <summary>
        /// executed price
        /// </summary>
        decimal xprice { get; set; }
        /// <summary>
        /// executed time
        /// </summary>
        int xtime { get; set; }
        /// <summary>
        /// executed date
        /// </summary>
        int xdate { get; set; }
        /// <summary>
        /// side of trade (true=long)
        /// </summary>
        bool side { get; set; }
        /// <summary>
        /// account trade occured in
        /// </summary>
        string Account { get; set; }
        /// <summary>
        /// local symbol
        /// </summary>
        string LocalSymbol { get; set; }
        /// <summary>
        /// exchange/destination where trade occured
        /// </summary>
        string ex { get; set; }
        /// <summary>
        /// security type
        /// </summary>
        SecurityType Security { get; set; }
        /// <summary>
        /// full security information for trade
        /// </summary>
        Security Sec { get; }
        /// <summary>
        /// currency trade occured in
        /// </summary>
        CurrencyType Currency { get; set; }
        /// <summary>
        /// unsigned size of trade
        /// </summary>
        int UnsignedSize { get; }
        /// <summary>
        /// whether trade is valid
        /// </summary>
        bool isValid { get; }
        /// <summary>
        /// whether trade has been filled
        /// </summary>
        bool isFilled { get; }
        /// <summary>
        /// comment on trade
        /// </summary>
        [Obsolete]
        string comment { get; set; }

    }

    public class InvalidTrade : Exception { }
}
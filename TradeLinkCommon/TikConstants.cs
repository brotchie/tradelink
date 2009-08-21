
namespace TradeLink.Common
{
    /// <summary>
    /// constants for working with Tik files
    /// </summary>
    public static class TikConst
    {
        public const string EXT = "TIK";
        public const string DOT_EXT = ".TIK";
        public const string WILDCARD_EXT = "*.TIK";

        public const int VERSION = 2;

        // file field identifiers
        public const byte Version = 0; // two bytes of version follow
        public const byte StartData = 1; // end header, start ticks
        public const byte EndTick = 2; // next tick coming
        public const byte EndData = 3; // no more ticks
        public const byte TickFull = 32; // quote and trade present
        public const byte TickQuote = 33; // bid and ask only
        public const byte TickBid = 34; // bid only
        public const byte TickAsk = 35; // ask only
        public const byte TickTrade = 36; // trade only



    }
}


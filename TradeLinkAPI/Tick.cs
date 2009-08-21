using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    public interface Tick
    {
        /// <summary>
        /// symbol for tick
        /// </summary>
        string symbol { get; set; } 
        /// <summary>
        /// tick time in 24 format (4:59pm => 1659)
        /// </summary>
        int time { get; set; }
        /// <summary>
        /// tick date
        /// </summary>
        int date { get; set; }
        /// <summary>
        /// date and time represented as long, eg 8:05pm on 4th of July:
        /// 200907042005
        /// </summary>
        long datetime { get; set; } // datetime as long
        /// <summary>
        /// size of last trade
        /// </summary>
        int size { get; set; } 
        /// <summary>
        /// depth of last bid/ask quote
        /// </summary>
        int depth { get; set; } 
        /// <summary>
        /// long representation of last trade
        /// </summary>
        ulong ltrade { get; set; }
        /// <summary>
        /// long representation of bid price
        /// </summary>
        ulong lbid { get; set; }
        /// <summary>
        /// long representation of ask price
        /// </summary>
        ulong lask { get; set; }
        /// <summary>
        /// trade price
        /// </summary>
        decimal trade { get; set; } 
        /// <summary>
        /// bid price
        /// </summary>
        decimal bid { get; set; } 
        /// <summary>
        /// offer price
        /// </summary>
        decimal ask { get; set; } 
        /// <summary>
        /// condensed bid size (100->1)
        /// </summary>
        int bs { get; set; } 
        /// <summary>
        /// condensed offer size (100 ->1)
        /// </summary>
        int os { get; set; } 
        /// <summary>
        /// full bid size 
        /// </summary>
        int BidSize { get; set; } 
        /// <summary>
        /// full ask size 
        /// </summary>
        int AskSize { get; set; } 
        /// <summary>
        /// bid exchange
        /// </summary>
        string be { get; set; }
        /// <summary>
        /// ask exchange
        /// </summary>
        string oe { get; set; } 
        /// <summary>
        /// trade exchange
        /// </summary>
        string ex { get; set; } 
        bool isTrade { get; }
        bool hasBid { get; }
        bool hasAsk { get; }
        bool isFullQuote { get; }
        bool isQuote { get; }
        bool isValid { get; }
        bool isIndex { get; }
    }

    public class InvalidTick : Exception { }

}

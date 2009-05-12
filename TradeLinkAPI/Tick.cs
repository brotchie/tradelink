using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    public interface Tick
    {
        string symbol { get; set; } // symbol for security
        Security Sec { get; set; } // security type
        int time { get; set; }// time in 1601 format
        int date { get; set; }// date in 20070926 format
        long datetime { get; set; } // datetime as long
        int size { get; set; } // trade size
        int depth { get; set; } // trade depth
        decimal trade { get; set; } // trade price
        decimal bid { get; set; } // bid price
        decimal ask { get; set; } // offer price
        int bs { get; set; } // bid size (100 => 1)
        int os { get; set; } // offer size (100 => 1)
        int BidSize { get; set; } // bid size (100 => 100)
        int AskSize { get; set; } // offer size (100 => 100)
        string be { get; set; }// bid exchange
        string oe { get; set; } // offer exchange
        string ex { get; set; } // trade exchange
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

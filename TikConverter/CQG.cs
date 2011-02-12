using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace TikConverter
{
    public struct CQG 
    {
        public const string SOURCE = "CQG";
        // fields of tradestation files
        const int SYM = 0;
        const int DATE = 1;
        const int TIME = 3;
        const int TRADE = 4;

        // here is where a line is converted
        public static Tick parseline(string line, int defaultsize, int decimalplaces )
        {
            // split line
            string[] r = line.Split(',');
            // create tick for this symbol
            Tick k = new TickImpl(r[SYM]);
            // setup temp vars
            int iv = 0;
            decimal dv = 0;
            // parse date
            if (int.TryParse(r[DATE], out iv))
                k.date = iv;
            // parse time
            if (int.TryParse(r[TIME], out iv))
                k.time = iv * 100;
            // parse close as trade price
            if (decimal.TryParse(r[TRADE], out dv))
            {
                decimal divisor = (decimal)(  Math.Pow( 10, decimalplaces ) );
                // k.trade = (decimal)dv / 100;
                k.trade = (decimal) dv / divisor;
                k.size = defaultsize;
            }
            // return tick
            return k;
        }
    }
}

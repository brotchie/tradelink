using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace TikConverter
{
    public struct QCollector
    {
        public const string SOURCE = "QCollector";
        // fields of tradestation files
        const int DATE = 0;
        const int TIME = 1;
        const int TRADE = 2;
        const int SIZE = 3;
        const int EXCH = 4;
        const int BID = 5;
        const int BIDEX = 6;
        const int BIDSIZE = 7;
        const int ASK = 8;
        const int ASKEX = 9;
        const int ASKSIZE = 10;

        static decimal getd(string s)
        {
            decimal v;
            if (decimal.TryParse(s, out v))
                return v;
            return 0;
        }

        static int geti(string s)
        {
            int v;
            if (int.TryParse(s, out v))
                return v;
            return 0;
        }

        // here is where a line is converted
        public static Tick parseline(string line, string SYMBOL)
        {
            // split line
            string[] r = line.Split(',');
            // create tick for this symbol
            Tick k = new TickImpl(SYMBOL);
            // setup temp vars
            int iv = 0;
            decimal dv = 0;
            // parse date
            if (int.TryParse(r[DATE], out iv))
                k.date = iv + 20000000;
            // parse time
            if (int.TryParse(r[TIME], out iv))
                k.time = iv;
            // parse close as trade price
            if (decimal.TryParse(r[TRADE], out dv))
            {
                k.trade = dv;
                k.size = geti(r[SIZE]);
                k.ex = r[EXCH];
            }
            if (decimal.TryParse(r[BID], out dv))
            {
                k.bid = dv;
                k.bs = geti(r[BIDSIZE]);
                k.be = r[BIDEX];
            }
            if (decimal.TryParse(r[ASK], out dv))
            {
                k.ask = dv;
                k.os = geti(r[ASKSIZE]);
                k.oe = r[ASKEX];
            }
            // return tick
            return k;
        }
    }
}

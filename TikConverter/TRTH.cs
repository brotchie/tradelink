using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace TikConverter
{
    public struct TRTH
    {
       // GOOG,20-APR-2011,13:05:18.881,Quote,,,525.49,10,0,0
        const int SYM = 0;
        const int DATE = 1;
        const int TIME = 2;
        const int TYPE = 3;
        const int PRICE = 4;
        const int VOLUME = 5;
        const int BID = 6;
        const int BIDSIZE = 7;
        const int ASK = 8;
        const int ASKSIZE = 9;

        public static Tick parseline(string line)
        {
            string[] r = line.Split(',');
            TickImpl k = new TickImpl(r[SYM]);

            DateTime dt = DateTime.Parse(r[DATE] + ' ' + r[TIME]);
            k.time = Util.DT2FT(dt);
            k.date = dt.Year * 10000 + dt.Month * 100 + dt.Day;

            if(r[TYPE] == "Trade"){
                k.trade = Convert.ToDecimal(r[PRICE]);
                k.size = Convert.ToInt32(r[VOLUME]);
            } else {
                k.bid = Convert.ToDecimal(r[BID]);
                k.bs = Convert.ToInt32(r[BIDSIZE]);
                k.ask = Convert.ToDecimal(r[ASK]);
                k.os = Convert.ToInt32(r[ASKSIZE]);
            }
            return k;
        }
        
    }
}

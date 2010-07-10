using System;
using TradeLink.API;
using TradeLink.Common;

namespace TikConverter
{

    public struct TradingPhysicsTnS
    {
        const int TIME = 0;
        const int TYPE = 1;
        const int SHARES = 2;
        const int PRICE = 3;
        const int MPID = 4;

        // here is where a line is converted
        public static Tick parseline(string line, string sym, int date)
        {
            // split line
            string[] r = line.Split(',');
            // create tick for this symbol
            Tick k = new TickImpl(sym);
            k.date = date;
            char type = 'z';

            long mtime = 0;
            if(long.TryParse(r[TIME], out mtime))
            {
                int hr = (int) mtime / 3600000;
                int min = (int) (mtime % 3600000) / 60000;
                int sec = (int) (mtime % 60000) / 1000;
                int ftime = Util.TL2FT(hr, min, sec);
                k.time = ftime;
            }
            int size = 0;
            if (int.TryParse(r[SHARES], out size))
                k.size = size;
            decimal price = 0.0M;
            if(decimal.TryParse(r[PRICE], out price))
                k.trade = price / 10000;

            return k;
        }

    }

    public struct TradingPhysicsTV
    {
        const int TIME = 0;
        const int ORDERID = 1;
        const int TYPE = 2;
        const int SHARES = 3;
        const int PRICE = 4;
        const int MPID = 5;

        // here is where a line is converted
        public static Tick parseline(string line, string sym, int date)
        {
            // split line
            string[] r = line.Split(',');
            // create tick for this symbol
            Tick k = new TickImpl(sym);
            k.date = date;
            int orderid = 0;

            char type = 'z';
            char.TryParse(r[TYPE], out type);
            long mtime = 0;
            if (long.TryParse(r[TIME], out mtime))
            {
                int hr = (int) mtime / 3600000;
                int min = (int) (mtime % 3600000) / 60000;
                int sec = (int) (mtime % 60000) / 1000;
                int ftime = Util.TL2FT(hr, min, sec);
                k.time = ftime;
            }
            int size = 0;
            int.TryParse(r[SHARES], out size);
                
            decimal price = 0.0M;
            decimal.TryParse(r[PRICE], out price);
            switch(type)
            { 
                case 'B':
                    k.bid = price / 10000;
                    k.BidSize = size;
                    break;
                case 'S':
                    k.ask = price / 10000;
                    k.AskSize = size;
                    break;
                case 'F':
                case 'T':
                case 'E':
                    k.trade = price / 10000;
                    k.size = size;
                    break;
                case 'D':
                case 'X':
                    k.trade = price / 10000;
                    k.size = size;
                    break;
                default:
                    k.trade = price / 10000;
                    k.size = size;
                    break;

            }

            return k;
        }
    }
}
using System;
using TradeLink.API;
using TradeLink.Common;

public struct TradeStation
{
    // fields of tradestation files
    const int DATE = 0;
    const int TIME = 1;
    const int OPEN = 2;
    const int HIGH = 3;
    const int LOW = 4;
    const int CLOSE = 5;
    const int UP = 6;
    const int DOWN = 7;
    // here is where a line is converted
    public static Tick parseline(string line, string sym, int defaultsize)
    {
        // split line
        string[] r = line.Split(',');
        // create tick for this symbol
        Tick k = new TickImpl(sym);
        // setup temp vars
        int iv = 0;
        decimal dv = 0;
        DateTime date;
        // parse date
        if (DateTime.TryParse(r[DATE], out date))
            k.date = Util.ToTLDate(date);
        // parse time
        if (int.TryParse(r[TIME], out iv))
            k.time = iv * 100;
        // parse close as trade price
        if (decimal.TryParse(r[CLOSE], out dv))
        {
            k.trade = dv;
            k.size = defaultsize;
        }
        // return tick
        return k;
    }
}
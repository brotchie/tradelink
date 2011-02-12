using System;
using TradeLink.API;
using TradeLink.Common;

public struct MultiCharts
{
    // fields of multicharts data - exported from either aone tick chart, or as one tick from multicharts quotemanager
    // tick data in input file should be in the format: DD/MM/YYYY,HH:MM:SS,Price,Volume
    const int DATE = 0;
    const int TIME = 1;
    const int PRICE = 2;
    const int VOLUME = 3;
    // here is where a line is converted
    public static Tick parseline(string line, string sym)
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
        // parse time - remove colons to give format HHMMSS
        if (int.TryParse(r[TIME].Replace(":", ""), out iv))
            k.time = iv;
        // trade price
        if (decimal.TryParse(r[PRICE], out dv))
            k.trade = dv;
        // size of trade
        if (int.TryParse(r[VOLUME], out iv))
            k.size = iv;
        // return tick
        return k;
    }
}
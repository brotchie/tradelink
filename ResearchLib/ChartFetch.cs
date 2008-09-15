using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;

namespace Picker
{
    public class ChartFetch
    {
        public static BarList[] Fetch(string[] symbols)
        {
            List<BarList> l = new List<BarList>();
            foreach (string sym in symbols)
            {
                BarList bl = new BarList(BarInterval.Day, sym);
                if (bl.DayFromGoogle())
                    l.Add(bl);
            }
            return l.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TradeLink.API
{
    public interface BarList
    {
        string Symbol { get; }
        int First { get; }
        int Last { get; }
        int Count { get; }
        Bar RecentBar { get; }
        Bar this[int index] { get; }
        Bar this[int index, BarInterval interval] { get; }
        BarInterval Int { get; set; } // default interval
        bool Has(int MinBars, BarInterval interval);
        bool Has(int MinBars);
        void Reset();
        bool isValid { get; }
        IEnumerator GetEnumerator();
    }
}

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
        void newTick(Tick k);
        Bar this[int index] { get; }
        Bar this[int index, BarInterval interval] { get; }
        int DefaultCustomInterval { get; set; } // custom intervals
        BarInterval DefaultInterval { get; set; } // default interval
        bool Has(int MinBars, BarInterval interval);
        bool Has(int MinBars);
        void Reset();
        bool isValid { get; }
        IEnumerator GetEnumerator();
        event SymBarIntervalDelegate GotNewBar;
        decimal[] Open();
        decimal[] Close();
        decimal[] High();
        decimal[] Low();
        int[] Date();
        int[] Time();
        int[] Vol();
        BarInterval[] Intervals { get; }
        int[] CustomIntervals { get; }

    }
}

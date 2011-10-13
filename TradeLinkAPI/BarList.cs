using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TradeLink.API
{
    public interface BarList
    {
        /// <summary>
        /// symbol bar represents
        /// </summary>
        string Symbol { get; }
        /// <summary>
        /// index of first bar
        /// </summary>
        int First { get; }
        /// <summary>
        /// index of last bar
        /// </summary>
        int Last { get; }
        /// <summary>
        /// count of all bars
        /// </summary>
        int Count { get; }
        /// <summary>
        /// most recently occuring bar
        /// </summary>
        Bar RecentBar { get; }
        /// <summary>
        /// add tick to the bar
        /// </summary>
        /// <param name="k"></param>
        void newTick(Tick k);
        /// <summary>
        /// add point to bar (allows for negative values)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="time"></param>
        /// <param name="date"></param>
        /// <param name="size"></param>
        void newPoint(string symbol, decimal p, int time, int date, int size);
        /// <summary>
        /// get a bar from list using it's index.  
        /// index = 0 is oldest bar.  index = Last is newest bar.
        /// index = -1 is one bar back.  index = -5 is 5 bars back
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Bar this[int index] { get; }
        /// <summary>
        /// get a bar from list using it's index and interval
        /// index = 0 is oldest bar.  index = Last is newest bar.
        /// index = -1 is one bar back.  index = -5 is 5 bars back
        /// </summary>
        /// <param name="index"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        Bar this[int index, BarInterval interval] { get; }
        /// <summary>
        /// default interval for bar when getting bar data (in interval units)
        /// </summary>
        int DefaultCustomInterval { get; set; } 
        /// <summary>
        /// default interval for bar when getting bar data (in BarIntervals)
        /// </summary>
        BarInterval DefaultInterval { get; set; } // default interval
        /// <summary>
        /// check for minimum amount of bars, if they are present returns true
        /// </summary>
        /// <param name="MinBars"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        bool Has(int MinBars, BarInterval interval);
        /// <summary>
        /// returns true if minimum # of bars present in interval
        /// </summary>
        /// <param name="MinBars"></param>
        /// <returns></returns>
        bool Has(int MinBars);
        /// <summary>
        /// clears contents of a list, erases all data.
        /// </summary>
        void Reset();
        /// <summary>
        /// true if bar has symbol and some data
        /// </summary>
        bool isValid { get; }
        IEnumerator GetEnumerator();
        /// <summary>
        /// called when new bar is created.
        /// this happens after first tick in the new bar.
        /// last full bar will have index -1
        /// </summary>
        event SymBarIntervalDelegate GotNewBar;
        /// <summary>
        /// gets array of opening prices for all bars in list, ordered by their index.
        /// </summary>
        /// <returns></returns>
        decimal[] Open();
        /// <summary>
        /// gets array of closing prices for all bars in list, ordered by their index.
        /// </summary>
        /// <returns></returns>
        decimal[] Close();
        /// <summary>
        /// gets array of high prices for all bars in list, ordered by their index.
        /// </summary>
        /// <returns></returns>
        decimal[] High();
        /// <summary>
        /// gets array of low prices for all bars in list, ordered by their index.
        /// </summary>
        /// <returns></returns>
        decimal[] Low();
        /// <summary>
        /// gets array of dates for all bars in list, ordered by their index.
        /// </summary>
        /// <returns></returns>
        int[] Date();
        /// <summary>
        /// gets array of times for all bars in list, ordered by their index.
        /// </summary>
        /// <returns></returns>
        int[] Time();
        /// <summary>
        /// gets array of volumes for all bars in list, ordered by their index.
        /// </summary>
        /// <returns></returns>
        long[] Vol();
        /// <summary>
        /// gets a list of intervals available on the bar.
        /// </summary>
        BarInterval[] Intervals { get; }
        /// <summary>
        /// gets list of all standard and custom intervals available on the bar
        /// </summary>
        int[] CustomIntervals { get; }
        /// <summary>
        /// gets count of bars for a specific interval
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        int IntervalCount(BarInterval interval);
        /// <summary>
        /// gets count of bars for a specific interval
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        int IntervalCount(int interval);

    }
}

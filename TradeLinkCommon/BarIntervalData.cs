using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
namespace TradeLink.Common
{
    internal interface IntervalData
    {
        int Last();
        int Count();
        void Reset();
        bool isRecentNew();
        List<decimal> open();
        List<decimal> close();
        List<decimal> high();
        List<decimal> low();
        List<long> vol();
        List<int> date();
        List<int> time();
        event SymBarIntervalDelegate NewBar;
        Bar GetBar(int index, string symbol);
        Bar GetBar(string symbol);
        void newPoint(decimal p, int time, int date, int size);
        void newTick(Tick k);
        void addbar(Bar b);
    }
}

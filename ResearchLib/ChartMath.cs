using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;

namespace ResearchLib
{
    public class ChartMath
    {

        public decimal[] Highs(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (Bar b in chart)
                l.Add(b.High);
            return l.ToArray();
        }

        public decimal[] Lows(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (Bar b in chart)
                l.Add(b.Low);
            return l.ToArray();
        }

        public decimal[] Opens(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (Bar b in chart)
                l.Add(b.Open);
            return l.ToArray();
        }

        public decimal[] Closes(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (Bar b in chart)
                l.Add(b.Close);
            return l.ToArray();
        }

        public int [] Volumes(BarList chart)
        {
            List<int> l = new List<int>();
            foreach (Bar b in chart)
                l.Add(b.Volume);
            return l.ToArray();
        }

        public decimal[] HLRange(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (Bar b in chart)
                l.Add(b.High - b.Low);
            return l.ToArray();
        }

        public decimal[] CORange(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (Bar b in chart)
                l.Add(b.Close - b.Open);
            return l.ToArray();
        }

        public decimal[] TrueRange(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            for (int i = chart.Last; i > 0; i--)
            {
                Bar t = chart[i];
                Bar p = chart[i - 1];
                decimal max = t.High > p.Close ? t.High : p.Close;
                decimal min = t.Low < p.Close ? t.Low : p.Close;
                l.Add(max - min);
            }
            return l.ToArray();
        }
    }
}

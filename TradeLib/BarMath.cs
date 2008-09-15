using System;
using System.Collections.Generic;


namespace TradeLib
{
    public static class BarMath
    {
        /// <summary>
        /// Returns a bardate as an array of ints in the form [year,month,day]
        /// </summary>
        /// <param name="bardate">The bardate.</param>
        /// <returns></returns>
        public static int[] Date(int bardate)
        {
            int day = bardate % 100;
            int month = ((bardate - day) / 100) % 100;
            int year = (bardate - (month * 100) - day) / 10000;
            return new int[] { year, month, day };
        }
        public static int[] Date(Bar bar) { return Date(bar.Bardate); }
        /// <summary>
        /// Returns the highest-high of the barlist, for so many bars back.
        /// </summary>
        /// <param name="b">The barlist.</param>
        /// <param name="barsback">The barsback to consider.</param>
        /// <returns></returns>
        public static decimal HH(BarList b, int barsback)
        {// gets highest high
            if (barsback > b.Count) barsback = b.Count;
            decimal hh = decimal.MinValue;
            for (int i = b.Last; i > (b.Count - barsback); i--) hh = (b.Get(i).High > hh) ? b.Get(i).High : hh;
            return hh;
        }
        /// <summary>
        /// Returns the highest high for the entire barlist.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static decimal HH(BarList b) { return HH(b, b.Count); }
        /// <summary>
        /// The lowest low for the barlist, considering so many bars back.
        /// </summary>
        /// <param name="b">The barlist.</param>
        /// <param name="barsback">The barsback to consider.</param>
        /// <returns></returns>
        public static decimal LL(BarList b, int barsback)
        { // gets lowest low
            if (barsback > b.Count) barsback = b.Count;
            decimal ll = decimal.MaxValue;
            for (int i = b.Last; i > (b.Count - barsback); i--) ll = (b.Get(i).Low < ll) ? b.Get(i).Low : ll;
            return ll;
        }
        /// <summary>
        /// Lowest low for the entire barlist.
        /// </summary>
        /// <param name="b">The barlist.</param>
        /// <returns></returns>
        public static decimal LL(BarList b) { return LL(b, b.Count); }



        public static decimal[] Highs(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (Bar b in chart)
                l.Add(b.High);
            return l.ToArray();
        }

        public static decimal[] Lows(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (Bar b in chart)
                l.Add(b.Low);
            return l.ToArray();
        }

        public static decimal[] Opens(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (Bar b in chart)
                l.Add(b.Open);
            return l.ToArray();
        }

        public static decimal[] Closes(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (Bar b in chart)
                l.Add(b.Close);
            return l.ToArray();
        }

        public static int[] Volumes(BarList chart)
        {
            List<int> l = new List<int>();
            foreach (Bar b in chart)
                l.Add(b.Volume);
            return l.ToArray();
        }

        public static decimal[] HLRange(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (Bar b in chart)
                l.Add(b.High - b.Low);
            return l.ToArray();
        }

        public static decimal[] CORange(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (Bar b in chart)
                l.Add(b.Close - b.Open);
            return l.ToArray();
        }

        public static decimal[] TrueRange(BarList chart)
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

        public static BarList[] FetchCharts(string[] symbols)
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

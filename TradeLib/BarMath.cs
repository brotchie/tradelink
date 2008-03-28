using System;


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
            if (barsback > b.NumBars()) barsback = b.NumBars();
            decimal hh = decimal.MinValue;
            for (int i = b.BarZero; i > (b.NumBars() - barsback); i--) hh = (b.Get(i).High > hh) ? b.Get(i).High : hh;
            return hh;
        }
        /// <summary>
        /// Returns the highest high for the entire barlist.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static decimal HH(BarList b) { return HH(b, b.NumBars()); }
        /// <summary>
        /// The lowest low for the barlist, considering so many bars back.
        /// </summary>
        /// <param name="b">The barlist.</param>
        /// <param name="barsback">The barsback to consider.</param>
        /// <returns></returns>
        public static decimal LL(BarList b, int barsback)
        { // gets lowest low
            if (barsback > b.NumBars()) barsback = b.NumBars();
            decimal ll = decimal.MaxValue;
            for (int i = b.BarZero; i > (b.NumBars() - barsback); i--) ll = (b.Get(i).Low < ll) ? b.Get(i).Low : ll;
            return ll;
        }
        /// <summary>
        /// Lowest low for the entire barlist.
        /// </summary>
        /// <param name="b">The barlist.</param>
        /// <returns></returns>
        public static decimal LL(BarList b) { return LL(b, b.NumBars()); }
    }

}

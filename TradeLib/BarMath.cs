using System;


namespace TradeLib
{
    public static class BarMath
    {
        public static int[] Date(int bardate)
        {
            int day = bardate % 100;
            int month = ((bardate - day) / 100) % 100;
            int year = (bardate - (month * 100) - day) / 10000;
            return new int[] { year, month, day };
        }
        public static decimal HH(BarList b, int barsback)
        {// gets highest high
            if (barsback > b.NumBars()) barsback = b.NumBars();
            decimal hh = decimal.MinValue;
            for (int i = b.BarZero; i > (b.NumBars() - barsback); i--) hh = (b.Get(i).High > hh) ? b.Get(i).High : hh;
            return hh;
        }
        public static decimal HH(BarList b) { return HH(b, b.NumBars()); }
        public static decimal LL(BarList b, int barsback)
        { // gets lowest low
            if (barsback > b.NumBars()) barsback = b.NumBars();
            decimal ll = decimal.MaxValue;
            for (int i = b.BarZero; i > (b.NumBars() - barsback); i--) ll = (b.Get(i).Low < ll) ? b.Get(i).Low : ll;
            return ll;
        }
        public static decimal LL(BarList b) { return LL(b, b.NumBars()); }
    }

}

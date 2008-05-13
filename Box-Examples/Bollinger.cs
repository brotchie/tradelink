using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;

namespace Atlas
{
    public class Bollinger : TickIndicator, BarListIndicator
    {
        private int sds;
        private int lookback;
        private BarInterval bi;
        private decimal count = 0;
        private decimal sum = 0;
        private decimal devavg = 0;
        private double sd;
        private decimal ub;
        private decimal lb;
        private decimal mean = 0;
        private decimal sumsqtrade = 0;
        bool isbarcons = false;
        List<decimal> l = new List<decimal>();
        
        
        public Bollinger(int sds, BarInterval bi, int lookback)
        { this.sds = sds;
        this.bi = bi;
        this.lookback = lookback;
        isbarcons = true;
        }


        
        public Bollinger(int sds, int lookback)
        {
            this.sds = sds;
            this.lookback = lookback;
            isbarcons = false;
        }

        public Bollinger() : this(2,10) { }

        public bool newTick  ( Tick t )
        {
            if (isbarcons) throw new Exception("You can't call newTick method without using the right constructor");
                if (t.isTrade)
                {
                    count++;
                    l.Add(t.trade);

                    if (l.Count > lookback)
                    {
                        l.RemoveAt(0);
                        count--;
                    }

                    sum = 0;
                    sumsqtrade = 0;

                    foreach (decimal v in l)
                    {
                        sum += v;
                        sumsqtrade = sumsqtrade + v * v;
                    }


                    mean = sum / count;
                    devavg = (sumsqtrade - sum * sum / count) / count;
                }
                sd = Math.Sqrt((double)devavg);
                ub = mean + sds * (decimal)sd;
                lb = mean - sds * (decimal)sd;
                return true;
        }

        public bool hasLookbacks
        { get { return (l.Count == lookback); } }
        
        public bool newBar(BarList bl)
        {
            Bar obar = bl.Get(bl.BarZero);
            if (!isbarcons) throw new Exception("You can't call a newBar method without using the right constructor.");
            if (bl.NewBar)
                {
                    count++;
                    decimal open = obar.Open;

                    l.Add(open);

                    if (l.Count > lookback)
                    {
                        l.RemoveAt(0);
                        count--;
                    }

                    sum = 0;
                    sumsqtrade = 0;
                    foreach (decimal v in l)
                    {
                        sum += v;
                        sumsqtrade = sumsqtrade + v * v;
                    }
                    mean = sum / count;
                    devavg = (sumsqtrade - sum * sum / count) / count;
                }


                sd = Math.Sqrt((double)devavg);
                ub = mean + sds * (decimal)sd;
                lb = mean - sds * (decimal)sd;
                return true;
        }



        public decimal Mean
        { get { return mean; }}

        public decimal Devavg
        { get { return devavg; } }

        public double Sd
        { get { return sd; }}

        public decimal Upperband
        { get { return ub; } }

        public decimal Lowerband
        { get { return lb; } }

        public void Reset()
        {
            sum = 0;
            mean = 0;
            count = 0;
            sumsqtrade = 0;
            devavg = 0;
        }
    }
}

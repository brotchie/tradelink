using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;

namespace box
{
    /// <summary>
    /// Grey box that will monitor existing position and exit you if the market crosses the 5 minute moving average for your position.
    /// 
    /// Essentially a smarter stop-loss.
    /// </summary>
    public class GreyExit : Box
    {
        public Autopilot(NewsService ns)
            : base(ns)
        {
            Version = "$Rev: 1";
            Name = "AutoPilot"+CleanVersion;
        }

        decimal sum = 0;
        int ticks = 0;
        int secperint = 300;
        int starttime = 0;
        decimal MA = 0;

        protected override int Read(Tick tick, BarList bl,BoxInfo bi)
        {
            if (tick.isTrade)
            {
                sum += tick.trade;
                ticks++;
                MA = sum/ticks;

                if (((tick.time - starttime) % secperint) == 0) starttime = tick.time;
                if (starttime==0) return 0;

                bool pricecross = (((PosSize > 0) && (tick.trade < MA)) || ((PosSize < 0) && (tick.trade > MA)));
                return  (pricecross) ? -1 * PosSize : 0;
            }
            return 0;
           
        }

    }
}

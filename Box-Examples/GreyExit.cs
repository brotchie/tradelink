using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;

// we need this library to be able to read the parameter comments from our box
using System.ComponentModel;

namespace box
{
    /// <summary>
    /// Grey box that will monitor existing position and exit you if the market crosses the 5 minute moving average for your position.
    /// 
    /// Essentially a smarter stop-loss.
    /// </summary>
    public class GreyExit : Box
    {

        // here's the exit size we allow the user to choose at startup

        [DescriptionAttribute("% of our trade to exit when MA is crossed")]
        public decimal ExitSizePercent { get { return exitpercent; } set { exitpercent = value; } }


        public GreyExit()
            : base()
        {
            Version = "$Rev: 1";
            Name = "GreyBox"+CleanVersion;

            // here's how we prompt for parameters
            ParamPrompt param = new ParamPrompt(this); // read the parameters
            param.Show(); // display the prompt
        }

        // we define these working variables as private 
        // this way they aren't displayed to the user when prompting for parameters
        private decimal exitpercent = 0;
        private decimal sum = 0;
        private int ticks = 0;
        private int secperint = 300;
        private int starttime = 0;
        private decimal MA = 0;

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
                return  (pricecross) ? Norm2Min(Flat*exitpercent) : 0;
            }
            return 0;
           
        }

    }
}

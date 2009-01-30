using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;

// we need this library to be able to read the parameter comments from our response
using System.ComponentModel;

namespace Responses
{
    /// <summary>
    /// Grey response that will monitor existing position and exit you if the market crosses the 5 minute moving average for your position.
    /// 
    /// Essentially a smarter stop-loss.
    /// </summary>
    public class GreyExit : MarketResponse
    {

        // here's the exit size we allow the user to choose at startup

        [DescriptionAttribute("% of our trade to exit when MA is crossed")]
        public decimal ExitSizePercent { get { return exitpercent; } set { exitpercent = value; } }
        [DescriptionAttribute("Interval to use in MA calculation")]
        public BarInterval Interval { get { return _int; } set { _int = value; } }
        [DescriptionAttribute("Number of bars to use in SMA.")]
        public int MABars { get { return _bb; } set { _bb = value; } }
        [DescriptionAttribute("Exit when price crosses above.  Change to false to make exit signal on opposite cross.")]
        public bool ExitAbove { get { return _above; } set { _above = value; } }
        [DescriptionAttribute("Set ExitAbove automatically based on current position.")]
        public bool AutoExitAbove { get { return _autoabove; } set { _autoabove = value; } }


        public GreyExit()
            : base()
        {
            Name = "GreyResponse";

            // here's how we prompt for parameters
            ParamPrompt param = new ParamPrompt(this); // read the parameters
            param.Show(); // display the prompt
        }

        // we define these working variables as private 
        // this way they aren't displayed to the user when prompting for parameters
        private decimal exitpercent = 0;
        private decimal MA = 0;
        BarInterval _int = BarInterval.FiveMin;
        bool _above = true; // trigger when crosses above or below
        bool _autoabove = false; // set above automatically based on position
        int _bb = 2;

        protected override int Read(Tick tick, BarList bl)
        {
            if (tick.isTrade)
            {
                if (!bl.Has(_bb)) return 0;
                MA = SMA.BarSMA(bl, _int, _bb);

                if (Pos.isFlat) return 0;

                if (_autoabove)
                    _above = (Pos.isShort);

                bool pricecross = _above ? (tick.trade > MA) : (tick.trade < MA);

                return  (pricecross) ? Calc.Norm2Min(Pos.FlatSize*exitpercent,MINSIZE) : 0;
            }
            return 0;
           
        }

    }
}

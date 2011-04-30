using System;
using TradeLink.API;
using TradeLink.Common;
using System.Collections.Generic;
using System.ComponentModel;
using TicTacTec.TA.Library; // to use TA-lib indicators

namespace Responses
{
    public class LessonTalib : ResponseTemplate
    {
        BarListTracker blt = new BarListTracker();

        public LessonTalib()
        {
            // handle new bars
            blt.GotNewBar+=new SymBarIntervalDelegate(newbar);
            // indexing
            MA.NewTxt += new TextIdxDelegate(MA_NewTxt);
            // provide indicator names
            Indicators = gt.GetIndicatorNames(MA, LOOKBACK);
        }



        GenericTracker<decimal> MA = new GenericTracker<decimal>("MA");
        GenericTracker<int> LOOKBACK = new GenericTracker<int>("LOOKBACK");

        void newbar(string symbol, int interval)
        {
           // get tracker index for our symbol
           int idx = MA.getindex(symbol);
           // get some data
           BarList bl = blt[symbol, interval];
           // convert it to form that ta-lib understands
           double[] closes = Calc.Decimal2Double(bl.Close());
           // call your indicator
           double[] ma = new double[closes.Length];
           int bi,oi;
           Core.Sma(0,closes.Length-1,closes,LOOKBACK[idx],out bi,out oi,ma);
           // populate your tracker with most recent value
           Calc.TAPopulateGT(idx,oi,ref ma,MA);
           // display our indicators
           sendindicators(gt.GetIndicatorValues(idx, MA, LOOKBACK));
        }

        void MA_NewTxt(string txt, int idx)
        {
            // use a default lookback of 3
            LOOKBACK.addindex(txt, 3);
        }

        public override void GotTick(Tick k)
        {
            blt.newTick(k);
        }

    }
}

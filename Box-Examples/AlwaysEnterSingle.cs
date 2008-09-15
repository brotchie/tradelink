using System;
using TradeLib;

namespace box
{
    /// <summary>
    /// Enters with only a single share/contract 
    ///
    /// This box will always enter long on every tick, unless he's already in a position.
    /// 
    /// If he's in a position, he will exit at pre-defined stop and loss targets.  (then get back in)
    /// 
    /// Used for testing applications that use boxes.  (Quotopia, Gauntlet, etc)
    /// </summary>
    public class AlwaysEnterSingle : BlackBoxEasy
    {
        public AlwaysEnterSingle() : base() 
        {
            Name = "AlwaysEnter";
            MinSize = 1;
            DayStart = 0;
            DayEnd = 3000;
        }
        protected override bool EnterLong()
        {
            D("Entering Long");
            return true;
        }
        protected override bool EnterShort()
        {
            return false;
        }
        protected override bool Exit()
        {
            bool exit = (Profit > .15m) || (Profit < -.1m);
            if (exit) D("Exiting: " + Profit);
            return exit;
        }
    }
}

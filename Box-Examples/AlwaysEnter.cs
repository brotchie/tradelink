using System;
using TradeLib;

namespace box
{
    public class AlwaysEnter : TradeBox
    {
        public AlwaysEnter(NewsService ns) : base(ns) 
        {
            Name = "AlwaysEnter";
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

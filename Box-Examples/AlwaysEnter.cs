using System;
using TradeLib;

namespace box
{
    public class AlwaysEnter : TradeBox
    {
        public AlwaysEnter(NewsService ns) : base(ns) { }
        protected override bool EnterLong()
        {
            return true;
        }
        protected override bool EnterShort()
        {
            return false;
        }
        protected override bool Exit()
        {
            return (Profit > .15m) || (Profit < -.1m);
        }
    }
}

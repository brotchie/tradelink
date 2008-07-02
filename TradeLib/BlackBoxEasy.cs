using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    /// <summary>
    /// Feature-rich version of Box class.  More structure for trader/user.  Full-concept of a round-turn with explicit entry and exit definition.  Stoploss and profit-taking included.
    /// </summary>
    public class BlackBoxEasy : Box
    {
        public BlackBoxEasy() : base() { }
        protected decimal sPrice = 0;
        protected int lotsize = 100;
        protected decimal stop = .1m;
        protected decimal profit = 0;
        protected int profitsize = 0;
        protected bool side = true;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BlackBoxEasy"/> is processing long trades.
        /// </summary>
        /// <value><c>true</c> if it's considering long trades; otherwise, a short trade.</value>
        public bool Side { get { return side; } set { side = value; } }
        /// <summary>
        /// Gets or sets the stop loss.
        /// </summary>
        /// <value>The stop.</value>
        public decimal Stop { get { return stop; } set { stop = value; } }
        /// <summary>
        /// Gets or sets the profit target.
        /// </summary>
        /// <value>The profit target.</value>
        public decimal ProfitTarget { get { return profit; } set { profit = value; } }
        /// <summary>
        /// Gets or sets the size of the trade when the profit target is reached.
        /// </summary>
        /// <value>The size of the trade.</value>
        public int ProfitSize { get { return profitsize; } set { profitsize = value; } }
        protected BarList bl;
        protected Tick tick;
        protected decimal getMostRecentTrade() { return tick.trade; }
        protected decimal getMostRecentBid() { return tick.bid; }
        protected decimal getMostRecentAsk() { return tick.ask; }
        protected decimal getMostRecentTradeSize() { return tick.size; }
        protected decimal getMostRecentBidSize() { return tick.bs; }
        protected decimal getMostRecentAskSize() { return tick.os; }
        protected bool newTrade() { return (PosSize != 0) && (AvgPrice == 0); }
        /// <summary>
        /// Gets or sets the size of the entry trade.
        /// </summary>
        /// <value>The size of the entry.</value>
        protected int EntrySize { get { return lotsize; } set { lotsize = value; } }
        /// <summary>
        /// Override this method to provide rules to enter a long trade.
        /// </summary>
        /// <returns>
        /// true if a long trade should be entered with size EntrySize, false to do nothing.
        /// 
        /// Default : false (never enter)
        /// </returns>
        protected virtual bool EnterLong() { return false; }
        /// <summary>
        /// Override this method to provide rules to enter a short trade.
        /// 
        /// Default : false (never enter)
        /// </summary>
        /// <returns>true if a short trade should be entered with size EntrySize, false to do nothing.</returns>
        protected virtual bool EnterShort() { return false;  }
        /// <summary>
        /// Override this method to provide rules to exit trades (long AND short). 
        /// 
        /// Default : false (never exit)
        /// </summary>
        /// <returns>True if the trade should be exited (completely), false to do nothing.</returns>
        protected virtual bool Exit() { return false; }
        /// <summary>
        /// Override this method to provide criteria for tick/trades that should be ignored, and not parsed for entry/exit rules.
        /// 
        /// Default : false (never ignore)
        /// </summary>
        /// <returns>True to ignore a tick, false otherwise.</returns>
        protected virtual bool IgnoreTick() { return false; }
        protected decimal Profit { get { return (PosSize > 0) ? tick.trade - AvgPrice : AvgPrice - tick.trade; } }
        void checkMoveStop()
        {
            if (Math.Abs(sPrice - this.tick.trade) > (this.stop * 1.5m)) sPrice = tick.trade - ((stop * 1.5m) * ((PosSize < 0) ? -1 : 1));
        }
        bool hitStop() { return (((tick.trade - sPrice) * ((PosSize < 0) ? 1 : -1))<=0); }
        bool hitProfit() { return (ProfitSize>0) && (ProfitTarget>0) && (Profit > ProfitTarget) && (PosSize == EntrySize); }
        void getStop() { if (Stop!=0) sPrice = AvgPrice - (stop * ((PosSize>0) ? 1 : -1)); }
        bool Enter()
        {
            Side = true;
            if (this.EnterLong()) return true;
            Side = false;
            if (this.EnterShort()) return true;
            return false;
        }
        BoxInfo _boxinfo = new BoxInfo();
        public BoxInfo Info { get { return _boxinfo; } }
        protected override int Read(Tick t,BarList barlist,BoxInfo bi)
        {
            this.tick = new Tick(t); // save tick to member for child classes
            this.bl = barlist; // save bars for same purpose
            _boxinfo = bi;

            int adjust = 0;
            if (newTrade()) getStop(); 
            else if (PosSize != 0) // if we have entry price, check exits
            {
                if (!IgnoreTick()) checkMoveStop();
                if (!IgnoreTick() && this.Exit()) adjust = PosSize * -1;
                else if (this.hitProfit()) adjust = (PosSize > 0) ? ProfitSize : ProfitSize * -1;
                else if (this.hitStop()) adjust = PosSize * -1;
            }
            else if (IgnoreTick()) adjust = 0;
            else if (this.Enter()) // if haven't entered, check entry criteria
                adjust = EntrySize * (Side ? 1 : -1);
            return adjust;
        }
    }
}

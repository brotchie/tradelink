using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.Common
{
    /// <summary>
    /// create market order to flat all or part of a position
    /// </summary>
    public class MarketOrderFlat : OrderImpl
    {
        /// <summary>
        /// flat 100% of a position
        /// </summary>
        /// <param name="current"></param>
        public MarketOrderFlat(Position current) : this(current, 1m) { }
        /// <summary>
        /// flat specified % of a position. 
        /// if the position is an odd-lot, optionally normalize to a standard lot size (if normalizeSize = true)
        /// if normalizing, specify lot size
        /// </summary>
        /// <param name="current"></param>
        /// <param name="percent"></param>
        /// <param name="normalizeSize"></param>
        /// <param name="MinimumLotSize"></param>
        public MarketOrderFlat(Position current, decimal percent, bool normalizeSize, int MinimumLotSize) : base(current.Symbol, normalizeSize ? Calc.Norm2Min((decimal)percent * current.FlatSize, MinimumLotSize) : (int)(percent*current.FlatSize)) { }
        /// <summary>
        /// flat portion of position, normalizing to 100 shares to avoid odd lots.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="percent"></param>
        public MarketOrderFlat(Position current, decimal percent) : this(current, percent, true, 100) { }
    }
}

using System;


namespace TradeLib
{
    public class BoxMath
    {
        // this is not very clear from the method names, but these are OpenPL functions
        /// <summary>
        /// Gets the open PL on a per-share basis, ignoring the size of the position.
        /// </summary>
        /// <param name="LastTrade">The last trade.</param>
        /// <param name="AvgPrice">The avg price.</param>
        /// <param name="PosSize">Size of the pos.</param>
        /// <returns></returns>
        public static decimal SharePL(decimal LastTrade, decimal AvgPrice, int PosSize)
        {
            return (PosSize == 0) ? 0 : SharePL(LastTrade, AvgPrice, PosSize > 0);
        }
        /// <summary>
        /// Gets the open PL on a per-share basis, ignoring the size of the position.
        /// </summary>
        /// <param name="LastTrade">The last trade.</param>
        /// <param name="AvgPrice">The avg price.</param>
        /// <param name="Side">if set to <c>true</c> [side].</param>
        /// <returns></returns>
        public static decimal SharePL(decimal LastTrade, decimal AvgPrice, bool Side)
        {
            return Side ? LastTrade - AvgPrice : AvgPrice - LastTrade;
        }
        public static decimal SharePL(decimal LastTrade, Position Pos)
        {
            return SharePL(LastTrade, Pos.AvgPrice, Pos.Size);
        }
        /// <summary>
        /// Gets the open PL considering all the shares held in a position.
        /// </summary>
        /// <param name="LastTrade">The last trade.</param>
        /// <param name="AvgPrice">The avg price.</param>
        /// <param name="PosSize">Size of the pos.</param>
        /// <returns></returns>
        public static decimal PositionPL(decimal LastTrade, decimal AvgPrice, int PosSize)
        {
            return PosSize * (LastTrade - AvgPrice);
        }
        public static decimal PositionPL(decimal LastTrade, Position Pos)
        {
            return PositionPL(LastTrade, Pos.AvgPrice, Pos.Size);
        }

        // these are for calculating closed pl
        // they do not adjust positions themselves
        /// <summary>
        /// Gets the closed PL on a per-share basis, ignoring how many shares are held.
        /// </summary>
        /// <param name="existing">The existing position.</param>
        /// <param name="closing">The portion of the position that's being closed/changed.</param>
        /// <returns></returns>
        public static decimal CloseSharePL(Position existing, Position closing)
        {
            if (!existing.isValid || !closing.isValid) 
                throw new Exception("Invalid position provided. (existing:" + existing.ToString() + " closing:" + closing.ToString());
            if (existing.Flat) return 0; // nothing to close
            if (closing.Flat) return 0; // nothing to close
            if (existing.Side == closing.Side) return 0; // if we're adding, nothing to close
            return existing.Side ? closing.AvgPrice - existing.AvgPrice : existing.AvgPrice - closing.AvgPrice;
        }

        /// <summary>
        /// Gets the closed PL on a position basis, the PL that is registered to the account for the entire shares transacted.
        /// </summary>
        /// <param name="existing">The existing position.</param>
        /// <param name="closing">The portion of the position being changed/closed.</param>
        /// <returns></returns>
        public static decimal ClosePositionPL(Position existing, Position closing)
        {
            return CloseSharePL(existing, closing) * Math.Abs(closing.Size);
        }

    }
}

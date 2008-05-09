using System;


namespace TradeLib
{
    public class BoxMath
    {
        /// <summary>
        /// Gets the open PL on a per-share basis, ignoring the size of the position.
        /// </summary>
        /// <param name="LastTrade">The last trade.</param>
        /// <param name="AvgPrice">The avg price.</param>
        /// <param name="PosSize">Size of the pos.</param>
        /// <returns></returns>
        public static decimal OpenPT(decimal LastTrade, decimal AvgPrice, int PosSize)
        {
            return (PosSize == 0) ? 0 : OpenPT(LastTrade, AvgPrice, PosSize > 0);
        }
        /// <summary>
        /// Gets the open PL on a per-share basis (also called points or PT), ignoring the size of the position.
        /// </summary>
        /// <param name="LastTrade">The last trade.</param>
        /// <param name="AvgPrice">The avg price.</param>
        /// <param name="Side">if set to <c>true</c> [side].</param>
        /// <returns></returns>
        public static decimal OpenPT(decimal LastTrade, decimal AvgPrice, bool Side)
        {
            return Side ? LastTrade - AvgPrice : AvgPrice - LastTrade;
        }
        public static decimal OpenPT(decimal LastTrade, Position Pos)
        {
            return OpenPT(LastTrade, Pos.AvgPrice, Pos.Size);
        }
        /// <summary>
        /// Gets the open PL considering all the shares held in a position.
        /// </summary>
        /// <param name="LastTrade">The last trade.</param>
        /// <param name="AvgPrice">The avg price.</param>
        /// <param name="PosSize">Size of the pos.</param>
        /// <returns></returns>
        public static decimal OpenPL(decimal LastTrade, decimal AvgPrice, int PosSize)
        {
            return PosSize * (LastTrade - AvgPrice);
        }
        public static decimal OpenPL(decimal LastTrade, Position Pos)
        {
            return OpenPL(LastTrade, Pos.AvgPrice, Pos.Size);
        }

        // these are for calculating closed pl
        // they do not adjust positions themselves
        /// <summary>
        /// Gets the closed PL on a per-share basis, ignoring how many shares are held.
        /// </summary>
        /// <param name="existing">The existing position.</param>
        /// <param name="closing">The portion of the position that's being closed/changed.</param>
        /// <returns></returns>
        public static decimal ClosePT(Position existing, Trade adjust)
        {
            if (!existing.isValid || !adjust.isValid) 
                throw new Exception("Invalid position provided. (existing:" + existing.ToString() + " adjustment:" + adjust.ToString());
            if (existing.Flat) return 0; // nothing to close
            if (existing.Side == adjust.Side) return 0; // if we're adding, nothing to close
            return existing.Side ? adjust.Price - existing.Price : existing.Price - adjust.Price;
        }

        /// <summary>
        /// Gets the closed PL on a position basis, the PL that is registered to the account for the entire shares transacted.
        /// </summary>
        /// <param name="existing">The existing position.</param>
        /// <param name="closing">The portion of the position being changed/closed.</param>
        /// <returns></returns>
        public static decimal ClosePL(Position existing, Trade adjust)
        {
            return ClosePT(existing, adjust) * Math.Abs(adjust.Size);
        }

        /// <summary>
        /// Normalizes any order size to the minimum lot size specified by MinSize.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static int Norm2Min(decimal size,int MINSIZE)
        {
            int wmult = (int)Math.Ceiling(size / MINSIZE);
            return wmult * MINSIZE;
        }

    }
}

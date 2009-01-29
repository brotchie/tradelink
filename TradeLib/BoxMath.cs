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
            if (existing.isFlat) return 0; // nothing to close
            if (existing.isLong == adjust.Side) return 0; // if we're adding, nothing to close
            return existing.isLong ? adjust.Price - existing.Price : existing.Price - adjust.Price;
        }

        /// <summary>
        /// Gets the closed PL on a position basis, the PL that is registered to the account for the entire shares transacted.
        /// </summary>
        /// <param name="existing">The existing position.</param>
        /// <param name="closing">The portion of the position being changed/closed.</param>
        /// <returns></returns>
        public static decimal ClosePL(Position existing, Trade adjust)
        {
            int closedsize = Math.Abs(adjust.Size + existing.Size);
            return ClosePT(existing, adjust) * (closedsize==0 ? Math.Abs(adjust.Size) : closedsize);
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

        /// <summary>
        /// Provides an offsetting price from a position.
        /// Positive offset will be a profit offset, negative offset will be stop.
        /// </summary>
        /// <param name="p">Position</param>
        /// <param name="offset">Offset amount</param>
        /// <returns>Offset price</returns>
        public static decimal OffsetPrice(Position p, decimal offset) { return OffsetPrice(p.AvgPrice, p.isLong, offset); }
        public static decimal OffsetPrice(decimal AvgPrice, bool side, decimal offset)
        {
            return side ? AvgPrice + offset : AvgPrice - offset;
        }
        /// <summary>
        /// Defaults to 100% of position at target.
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">your target</param>
        /// <returns>profit taking limit order</returns>
        public static Order PositionProfit(Position p, decimal offset) { return PositionProfit(p, offset, 1, false, 1); }
        /// <summary>
        /// Generates profit taking order for a given position, at a specified per-share profit target.  
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">target price, per share/contract</param>
        /// <param name="percent">percent of the position to close with this order</param>
        /// <returns></returns>
        public static Order PositionProfit(Position p, decimal offset, decimal percent) { return PositionProfit(p, offset, percent, false, 1); }
        /// <summary>
        /// Generates profit taking order for a given position, at a specified per-share profit target.  
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">target price, per share/contract</param>
        /// <param name="percent">percent of the position to close with this order</param>
        /// <param name="normalizesize">whether to normalize order to be an even-lot trade</param>
        /// <param name="MINSIZE">size of an even lot</param>
        /// <returns></returns>
        public static Order PositionProfit(Position p, decimal offset, decimal percent, bool normalizesize, int MINSIZE)
        {
            Order o = new Order();
            if (!p.isValid || p.isFlat) return o;
            decimal price = OffsetPrice(p,Math.Abs(offset));
            int size = !normalizesize ? (int)(p.FlatSize * percent) : Norm2Min(p.FlatSize*percent,MINSIZE);
            o = new LimitOrder(p.Symbol, !p.isLong, size, price);
            return o;
        }
        /// <summary>
        /// Generate a stop order for a position, at a specified per-share/contract price.  Defaults to 100% of position.
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">how far away stop is</param>
        /// <returns></returns>
        public static Order PositionStop(Position p, decimal offset) { return PositionStop(p,offset,1,false,1); }
        /// <summary>
        /// Generate a stop order for a position, at a specified per-share/contract price
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">how far away stop is</param>
        /// <param name="percent">what percent of position to close</param>
        /// <returns></returns>
        public static Order PositionStop(Position p, decimal offset, decimal percent) { return PositionStop(p, offset, percent, false, 1); }
        /// <summary>
        /// Generate a stop order for a position, at a specified per-share/contract price
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">how far away stop is</param>
        /// <param name="percent">what percent of position to close</param>
        /// <param name="normalizesize">whether to normalize size to even-lots</param>
        /// <param name="MINSIZE">size of an even lot</param>
        /// <returns></returns>
        public static Order PositionStop(Position p, decimal offset, decimal percent, bool normalizesize, int MINSIZE)
        {
            Order o = new Order();
            if (!p.isValid || p.isFlat) return o;
            decimal price = OffsetPrice(p, Math.Abs(offset)*-1);
            int size = !normalizesize ? (int)(p.FlatSize * percent) : Norm2Min(p.FlatSize * percent, MINSIZE);
            o = new StopOrder(p.Symbol, !p.isLong, size, price);
            return o;
        }

    }
}

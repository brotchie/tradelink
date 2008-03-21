using System;


namespace TradeLib
{
    public class BoxMath
    {
        // this is not very clear from the method names, but these are OpenPL functions
        public static decimal SharePL(decimal LastTrade, decimal AvgPrice, int PosSize)
        {
            return (PosSize == 0) ? 0 : SharePL(LastTrade, AvgPrice, PosSize > 0);
        }
        public static decimal SharePL(decimal LastTrade, decimal AvgPrice, bool Side)
        {
            return Side ? LastTrade - AvgPrice : AvgPrice - LastTrade;
        }
        public static decimal SharePL(decimal LastTrade, Position Pos)
        {
            return SharePL(LastTrade, Pos.AvgPrice, Pos.Size);
        }
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
        public static decimal CloseSharePL(Position existing, Position closing)
        {
            if (!existing.isValid || !closing.isValid) 
                throw new Exception("Invalid position provided. (existing:" + existing.ToString() + " closing:" + closing.ToString());
            if (existing.Flat) return 0; // nothing to close
            if (closing.Flat) return 0; // nothing to close
            if (existing.Side == closing.Side) return 0; // if we're adding, nothing to close
            return existing.Side ? closing.AvgPrice - existing.AvgPrice : existing.AvgPrice - closing.AvgPrice;
        }

        public static decimal ClosePositionPL(Position existing, Position closing)
        {
            return CloseSharePL(existing, closing) * Math.Abs(closing.Size);
        }

    }
}

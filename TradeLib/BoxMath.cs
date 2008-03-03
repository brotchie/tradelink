using System;


namespace TradeLib
{
    public class BoxMath
    {
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
    }
}

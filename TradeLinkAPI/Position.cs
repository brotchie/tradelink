using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    public interface Position
    {
        string Symbol { get; }
        decimal AvgPrice { get; }
        int Size { get; }
        int UnsignedSize { get; }
        bool isLong { get; }
        bool isShort { get; }
        bool isFlat { get; }
        decimal ClosedPL { get; }
        int FlatSize { get; }
        string Account { get; }
        bool isValid { get; }
        decimal Adjust(Position newPosition);
        decimal Adjust(Trade newFill);
        Trade ToTrade();
    }

    public class InvalidPosition : Exception {}
}

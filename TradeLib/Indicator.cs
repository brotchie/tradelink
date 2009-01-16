using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    public interface TickIndicator
    {
        bool newTick(Tick t);
    }

    public interface BarListIndicator
    {
        bool newBar(BarList bl);
    }

    public interface BarInterface
    {
        decimal High { get; }
        decimal Low { get; }
        decimal Open { get; }
        decimal Close { get; }
        int Volume { get; }
    }
}

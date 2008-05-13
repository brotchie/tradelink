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
}

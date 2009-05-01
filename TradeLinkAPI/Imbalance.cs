using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    public interface Imbalance
    {
        string Symbol { get; }
        bool isValid { get; }
        bool hasImbalance { get; }
        bool hadImbalance { get; }
        int ThisImbalance { get; }
        int PrevImbalance { get; }
        int ThisTime { get; }
        int PrevTime { get; }
        int InfoImbalance { get; }
        string Exchange { get; }
    }
}

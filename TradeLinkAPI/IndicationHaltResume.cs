using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    public interface HaltResume
    {
        string Symbol { get; }
        string Exchange { get; }
        int Time { get; }
        string Status { get; }
        string Reason { get; }
    }

    public interface Indication
    {
        string Symbol { get; }
        string Exchange { get; }
        int Time { get; }
        bool isValid { get; }
        decimal High { get; }
        decimal Low { get; }
    }
}

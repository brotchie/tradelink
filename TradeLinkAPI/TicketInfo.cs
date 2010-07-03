using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    public enum Priority
    {
        Highest = 1,
        High = 2,
        Normal = 3,
        Low = 4,
        Lowest = 5,
    }

    public enum TicketStatus
    {
        New,
        Accepted,
        ClosedInvalid,
        ClosedFixed,
        Test,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    public interface Security
    {
        string Symbol { get; set; }
        string FullName { get;  }
        string DestEx { get; set; }
        SecurityType Type { get; set; }
        bool isValid { get; }
        bool hasDest { get; }
        bool hasType { get; }
    }

    public class InvalidSecurity : Exception { }
}

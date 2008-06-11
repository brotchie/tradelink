using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    /// <summary>
    /// Abstract definition of instruments, such as Stock/Index/Future, etc.
    /// </summary>
    public abstract class Instrument
    {
        public abstract string Name { get; set; }
        public abstract bool isValid { get; }
        public abstract Security SecurityType { get; }
    }
}
